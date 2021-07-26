using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DynamicData;
using DynamicData.Binding;
using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using OStimAnimationTool.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;

namespace OStimConversionTool.ViewModels
{
    public class BadXmlException : XmlException
    {
        public BadXmlException(string message) : base(message)
        {
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private string _oSexDirectory = string.Empty;

        public MainWindowViewModel(IDialogService dialogService, IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            NewDatabaseCommand = new DelegateCommand(NewAnimationDatabase);
            LoadDatabaseCommand = new DelegateCommand(LoadDatabase);
            SaveDatabaseCommand = new DelegateCommand(SaveDatabase, () => AnimationDatabase.IsValueCreated);
        }

        public DelegateCommand NewDatabaseCommand { get; }
        public DelegateCommand LoadDatabaseCommand { get; }
        public DelegateCommand SaveDatabaseCommand { get; }

        //TODO:Overhaul Database save system
        /*private static bool WellFormedDatabase()
        {
            return AnimationDatabase.IsValueCreated &&
                   AnimationDatabase.Instance.Modules.All(module => module.AnimationSets.All(animationSet =>
                       !string.IsNullOrEmpty(animationSet
                           .AnimationClass) &&
                       !string.IsNullOrEmpty(animationSet
                           .PositionKey)));
        }*/

        private void NewAnimationDatabase()
        {
            _dialogService.ShowDialog("NewAnimationDatabaseDialog", result =>
            {
                if (result.Result != ButtonResult.OK) return;
                if (!string.IsNullOrEmpty(result.Parameters.GetValue<string>("name")))
                {
                    AnimationDatabase.Instance.Name = result.Parameters.GetValue<string>("name");
                }

                FolderBrowserDialog folderBrowserDialog = new();
                {
                    folderBrowserDialog.UseDescriptionForTitle = true;
                    folderBrowserDialog.Description =
                        @"Choose the following Path in your OSex Animation: Data\meshes\0SA\mod\0Sex\scene";
                    folderBrowserDialog.ShowDialog();

                    // Loading of the default OSex Animations
                    var oSexXmlDirectory = folderBrowserDialog.SelectedPath;
                    _oSexDirectory = oSexXmlDirectory.Replace("scene", "anim");
                    LoadOSexAnimations(oSexXmlDirectory);
                }

                AnimationDatabase.Instance.Modules.ToObservableChangeSet().AutoRefresh(x => x.AnimationSets).Subscribe(
                    _ =>
                    {
                        foreach (var module in AnimationDatabase.Instance.Modules)
                        {
                            module.AnimationSets.ToObservableChangeSet().AutoRefresh(x => x.SceneId)
                                .Subscribe(_ => SaveDatabaseCommand.RaiseCanExecuteChanged());
                        }
                    });

                _regionManager.RequestNavigate("TreeViewRegion", "DatabaseTreeView");
                _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish();
            });
        }

        // Method for loading OSex Animations into a new Database
        private void LoadOSexAnimations(string sceneDir)
        {
            // Creating the Modules depending on the Folders inside your OSex Scene Directory
            foreach (var moduleDir in Directory.GetDirectories(sceneDir))
            {
                var moduleName = Path.GetFileName(moduleDir);
                var module = new Module(moduleName);
                AnimationDatabase.Instance.Modules.Add(module);
            }

            foreach (var moduleDir in Directory.GetDirectories(sceneDir))
            {
                foreach (var positionDir in Directory.GetDirectories(moduleDir))
                {
                    foreach (var classDir in Directory.GetDirectories(positionDir))
                    {
                        foreach (var file in Directory.GetFiles(classDir))
                        {
                            try
                            {
                                // Loading Xml Files
                                XElement doc = XElement.Load(file);

                                // Getting necessary information from the .xml
                                var sceneId = doc.Attribute("id")?.Value;
                                if (sceneId is null) throw new BadXmlException($"{file}: Bad Xml, Faulty SceneID");
                                var actors = doc.Attribute("actors")?.Value;
                                if (actors is null)
                                    throw new BadXmlException($"{file}: Bad Xml, Missing Actors Attribute");
                                var animationId = doc.Element("anim")?.Attribute("id")?.Value;
                                if (animationId is null)
                                    throw new BadXmlException($"{file}: Bad Xml, Missing AnimationID");

                                // Getting optional information from the .xml
                                var description = doc.Element("info")?
                                    .Attribute("name")?.Value;
                                var animator = doc.Element("info")?
                                    .Attribute("animator")?.Value;

                                var sceneIdArray = sceneId.Split('|');

                                if (sceneIdArray.Length != 4)
                                    throw new BadXmlException($"{file}: Bad Xml, Faulty SceneID");

                                // Checking if SceneID matches File Location
                                if (sceneIdArray[0] != Path.GetFileName(moduleDir))
                                    throw new BadXmlException(
                                        $"{file}: Bad Xml, (Module) SceneID doesn't match file location");
                                if (sceneIdArray[1].Replace("!", "") != Path.GetFileName(positionDir))
                                    throw new BadXmlException(
                                        $"{file}: Bad Xml, (PositionKey) SceneID doesn't match file location");
                                if (sceneIdArray[2] != Path.GetFileName(classDir))
                                    throw new BadXmlException(
                                        $"{file}: Bad Xml, (AnimationClass) SceneID doesn't match file location");
                                if (sceneIdArray[3] != Path.GetFileNameWithoutExtension(file))
                                    throw new BadXmlException(
                                        $"{file}: Bad Xml, (SetName) SceneID doesn't match file location");

                                // Modifying AnimationSet with correct sceneID if already in the Database else creates a new AnimationSet
                                var animationSet = SetFinder(sceneIdArray);
                                if (animationSet is null) throw new BadXmlException($"{file}: BadXml");

                                if (description != null) animationSet.Description = description;
                                if (animator != null) animationSet.Animator = animator;
                                animationSet.Is0SexAnimation = true;

                                AnimationSet? destinationAnimationSet;
                                switch (animationSet)
                                {
                                    case TransitionAnimationSet transitionAnimationSet:
                                        var destinationId = doc.Element("anim")?.Attribute("dest")?.Value;
                                        if (destinationId is null)
                                            throw new BadXmlException(
                                                $"{file}: BadXml, Transition is missing Destination");

                                        // Appending to SceneID if "^" Notation is used
                                        if (destinationId.StartsWith('^'))
                                        {
                                            destinationId = sceneId + destinationId[1..];
                                        }

                                        // Setting the Transition Destination
                                        var destinationIdArray = destinationId.Split('|');
                                        if (destinationIdArray.Length != 4)
                                            throw new BadXmlException(
                                                $"{file}: BadXml, Destination has faulty SceneID");

                                        destinationAnimationSet = SetFinder(destinationIdArray);
                                        if (destinationAnimationSet is null)
                                            throw new BadXmlException(
                                                $"{file}: BadXml, Destination has faulty SceneID");
                                        transitionAnimationSet.Destination = destinationAnimationSet;

                                        // Defining Animations Contained in AnimationSet
                                        for (var i = 0; i < int.Parse(actors); i++)
                                        {
                                            var oldPath = sceneIdArray[0].Equals("EMF")
                                                ? Path.Combine(_oSexDirectory, sceneIdArray[0],
                                                    sceneIdArray[1].Replace("!", ""),
                                                    sceneIdArray[2], transitionAnimationSet.ParentSet,
                                                    transitionAnimationSet.Emf,
                                                    animationId + $"_{i}.hkx")
                                                : Path.Combine(_oSexDirectory, sceneIdArray[0],
                                                    sceneIdArray[1].Replace("!", ""),
                                                    sceneIdArray[2], transitionAnimationSet.ParentSet,
                                                    animationId + $"_{i}.hkx");

                                            var animation = new Animation(oldPath, animationSet)
                                            {
                                                Speed = 0,
                                                Actor = i
                                            };

                                            if (File.Exists(animation.OldPath))
                                            {
                                                animationSet.Animations.Add(animation);
                                            }
                                        }

                                        break;

                                    case HubAnimationSet hubAnimationSet:
                                    {
                                        // Destinations are different in the AutoStart-Xml
                                        var destinations = file == "AutoStartBasic"
                                            ? doc.Elements("togs").Elements("tog0").Elements("dest").Attributes("id")
                                                .Select(d => d.Value).ToList()
                                            : doc.Elements("nav").Elements("tab").Elements("page").Elements("option")
                                                .Attributes("go").Select(d => d.Value).ToList();

                                        // Adding destinations to the AnimationSets Destination List
                                        // Annotation: LINQ handles "^"-Notation
                                        foreach (var destIdArray in destinations.Select(s => s.StartsWith('^')
                                            ? sceneId + s[1..]
                                            : s).Select(destId => destId.Split('|')))
                                        {
                                            if (destIdArray.Length != 4)
                                                throw new BadXmlException(
                                                    $"{file}: BadXml, Destination has faulty SceneID");

                                            destinationAnimationSet = SetFinder(destIdArray);
                                            if (destinationAnimationSet is null)
                                                throw new BadXmlException(
                                                    $"{file}: BadXml, Destination has faulty SceneID");
                                            hubAnimationSet.Destinations.Add(destinationAnimationSet);
                                        }

                                        // Defining Animations Contained in AnimationSet
                                        var animations = doc.Element("speed") is null
                                            ? new List<string> {animationId}
                                            : doc.Elements("speed").Elements("sp").Elements("anim").Attributes("id")
                                                .Select(animation => animation.Value);

                                        foreach (var anim in animations)
                                        {
                                            for (var i = 0; i < int.Parse(actors); i++)
                                            {
                                                var oldPath = Path.Combine(_oSexDirectory, sceneIdArray[0],
                                                    sceneIdArray[1].Replace("!", ""), sceneIdArray[2], sceneIdArray[3],
                                                    anim + $"_{i}.hkx");
                                                
                                                var animation = new Animation(oldPath, animationSet)
                                                {
                                                    Speed = (int) char.GetNumericValue(anim[^1]),
                                                    Actor = i
                                                };

                                                if (File.Exists(animation.OldPath))
                                                {
                                                    animationSet.Animations.Add(animation);
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                            catch (XmlException e)
                            {
                                Console.WriteLine(e);
                                if (e is BadXmlException)
                                {
                                    MessageBox.Show(e.Message);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadDatabase()
        {
            /*
                        var fileDialog = new OpenFileDialog
                        {
                            Filter = "Animation Database file (*.xml)|*xml",
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                        };
            
                        if (fileDialog.ShowDialog() != true) return;
            
                        AnimationDatabase.Instance.SafePath = Path.GetDirectoryName(fileDialog.FileName)!;
                        XElement doc = XElement.Load(fileDialog.FileName);
                        AnimationDatabase.Instance.Name = doc.Attribute("Name")?.Value ?? string.Empty;
                        var elements = doc.Elements("Hub").ToList();
                        elements.AddRange(doc.Elements("Transition"));
                        foreach (var set in elements)
                        {
                            var animationSet = SetFinder(set.Attribute("SceneID")?.Value ?? string.Empty);
                            animationSet.Animator = set.Attribute("Animator")?.Value ?? string.Empty;
                            animationSet.Description = set.Attribute("Description")?.Value ?? string.Empty;
                            animationSet.Is0SexAnimation = true;
            
                            foreach (var animation in set.Elements("Animation"))
                                animationSet.Animations.Add(new Animation(animation.Attribute("OldPath")?.Value ?? string.Empty,
                                    animationSet)
                                {
                                    Actor = Convert.ToInt32(animation.Attribute("Actor")?.Value),
                                    Speed = Convert.ToInt32(animation.Attribute("Speed")?.Value)
                                });
            
                            switch (animationSet)
                            {
                                case TransitionAnimationSet transitionAnimationSet:
                                    transitionAnimationSet.Destination =
                                        SetFinder(set.Attribute("Destination")?.Value ?? string.Empty);
                                    break;
                                case HubAnimationSet hubAnimationSet:
                                    foreach (var destination in set.Elements("Destinations").Attributes("Destination")
                                        .Select(destination => destination.Value))
                                        hubAnimationSet.Destinations.Add(SetFinder(destination));
                                    break;
                            }
            
                            animationSet.ChangedThisSession = false;
                        }
            
                        foreach (var module in AnimationDatabase.Instance.Modules)
                        {
                            module.AnimationSets.ToObservableChangeSet().AutoRefresh(x => x.SceneId)
                                .Subscribe(_ => SaveDatabaseCommand.RaiseCanExecuteChanged());
            
                            _regionManager.RequestNavigate("TreeViewRegion", "DatabaseTreeView");
                            _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish();
                        }*/
        }

        private static void SaveDatabase()
        {
            if (string.IsNullOrEmpty(AnimationDatabase.Instance.SafePath))
            {
                FolderBrowserDialog folderBrowserDialog = new();
                {
                    folderBrowserDialog.ShowDialog();
                    AnimationDatabase.Instance.SafePath = folderBrowserDialog.SelectedPath;
                }
            }

            DatabaseScriber databaseScriber = new();
            databaseScriber.XmlScriber();
            databaseScriber.FnisScriber();
            databaseScriber.DatabaseFileScriber();
        }

        private static AnimationSet? SetFinder(IReadOnlyList<string> sceneId)
        {
            foreach (var module in AnimationDatabase.Instance.Modules)
            {
                if (!module.Name.Equals(sceneId[0])) continue;

                AnimationSet animationSet;

                foreach (var set in module.AnimationSets)
                {
                    if (!set.SetName.Equals(sceneId[3])) continue;
                    animationSet = set;
                    return animationSet;
                }

                var positionKey = sceneId[1];
                var animationClass = sceneId[2];
                var setName = sceneId[3];

                animationSet = setName.Contains('+')
                    ? new TransitionAnimationSet(module, positionKey, animationClass, setName)
                    : new HubAnimationSet(module, positionKey, animationClass, setName);

                module.AnimationSets.Add(animationSet);

                return animationSet;
            }

            return null;
        }
        
        private static Module ModuleFinder(string name)
        {
            foreach (var module in AnimationDatabase.Instance.Modules)
                if (module.Name == name)
                    return module;

            var newModule = new Module(name);
            AnimationDatabase.Instance.Modules.Add(newModule);
            return newModule;
        }
    }
}
