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
using DialogResult = System.Windows.Forms.DialogResult;

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

        // Method for initializing New Database
        private void NewAnimationDatabase()
        {
            _dialogService.ShowDialog("NewAnimationDatabaseDialog", result =>
            {
                if (result.Result != ButtonResult.OK) return;

                // Clearing Database
                AnimationDatabase.Instance.Modules.Clear();
                AnimationDatabase.Instance.Misc.Clear();
                AnimationDatabase.Instance.SafePath = string.Empty;

                if (!string.IsNullOrEmpty(result.Parameters.GetValue<string>("name")))
                    AnimationDatabase.Instance.Name = result.Parameters.GetValue<string>("name");

                // Loading default OSex Animations
                FolderBrowserDialog folderBrowserDialog = new();
                {
                    folderBrowserDialog.UseDescriptionForTitle = true;
                    folderBrowserDialog.Description =
                        @"Please select your OSex scene Folder";
                    folderBrowserDialog.ShowDialog();

                    var oSexXmlDirectory = folderBrowserDialog.SelectedPath;
                    LoadOSexAnimations(oSexXmlDirectory);
                }

                AnimationDatabase.Instance.Modules.ToObservableChangeSet().AutoRefresh(x => x.AnimationSets).Subscribe(
                    _ =>
                    {
                        foreach (var module in AnimationDatabase.Instance.Modules)
                            module.AnimationSets.ToObservableChangeSet().AutoRefresh(x => x.SceneId)
                                .Subscribe(_ => SaveDatabaseCommand.RaiseCanExecuteChanged());
                    });

                _regionManager.RequestNavigate("TreeViewRegion", "DatabaseTreeView");
                _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish();
            });
        }

        // Method for loading OSex Animations into a new Database
        private static void LoadOSexAnimations(string sceneDir)
        {
            var oSexDirectory = sceneDir.Replace("scene", "anim");
            // Creating the Modules depending on the Folders inside your OSex Scene Directory
            foreach (var moduleDir in Directory.GetDirectories(sceneDir))
            {
                var moduleName = Path.GetFileName(moduleDir);
                var module = new Module(moduleName);
                AnimationDatabase.Instance.Modules.Add(module);
            }

            foreach (var moduleDir in Directory.GetDirectories(sceneDir))
            foreach (var positionDir in Directory.GetDirectories(moduleDir))
            foreach (var classDir in Directory.GetDirectories(positionDir))
            foreach (var file in Directory.GetFiles(classDir))
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
                            if (destinationId.StartsWith('^')) destinationId = sceneId + destinationId[1..];

                            // Setting the Transition Destination
                            var destinationIdArray = destinationId.Split('|');
                            if (destinationIdArray.Length != 4)
                                throw new BadXmlException(
                                    $"{file}: BadXml, Destination has faulty SceneID");

                            destinationAnimationSet = SetFinder(destinationIdArray);
                            if (destinationAnimationSet is null)
                                throw new BadXmlException(
                                    $"{file}: BadXml, Destination has faulty SceneID");
                            destinationAnimationSet.Is0SexAnimation = true;
                            transitionAnimationSet.Destination = destinationAnimationSet;

                            // Defining Animations Contained in AnimationSet
                            for (var i = 0; i < int.Parse(actors); i++)
                            {
                                var oldPath = sceneIdArray[0].Equals("EMF")
                                    ? Path.Combine(oSexDirectory, sceneIdArray[0],
                                        sceneIdArray[1].Replace("!", ""),
                                        sceneIdArray[2], transitionAnimationSet.ParentSet,
                                        transitionAnimationSet.Emf,
                                        animationId + $"_{i}.hkx")
                                    : Path.Combine(oSexDirectory, sceneIdArray[0],
                                        sceneIdArray[1].Replace("!", ""),
                                        sceneIdArray[2], transitionAnimationSet.ParentSet,
                                        animationId + $"_{i}.hkx");

                                var animation = new Animation(oldPath, animationSet)
                                {
                                    Speed = 0,
                                    Actor = i
                                };

                                if (File.Exists(animation.OldPath)) animationSet.Animations.Add(animation);
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
                                destinationAnimationSet.Is0SexAnimation = true;
                                hubAnimationSet.Destinations.Add(destinationAnimationSet);
                            }

                            // Defining Animations Contained in AnimationSet
                            var animations = doc.Element("speed") is null
                                ? new List<string> { animationId }
                                : doc.Elements("speed").Elements("sp").Elements("anim").Attributes("id")
                                    .Select(animation => animation.Value);

                            foreach (var anim in animations)
                                for (var i = 0; i < int.Parse(actors); i++)
                                {
                                    var oldPath = Path.Combine(oSexDirectory, sceneIdArray[0],
                                        sceneIdArray[1].Replace("!", ""), sceneIdArray[2], sceneIdArray[3],
                                        anim + $"_{i}.hkx");

                                    var animation = new Animation(oldPath, animationSet)
                                    {
                                        Speed = (int)char.GetNumericValue(anim[^1]),
                                        Actor = i
                                    };

                                    if (File.Exists(animation.OldPath)) animationSet.Animations.Add(animation);
                                }

                            break;
                        }
                    }
                }
                catch (XmlException e)
                {
                    if (e is BadXmlException) MessageBox.Show(e.Message);
                }
        }

        // Method responsible for loading Database Files
        private void LoadDatabase()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Animation Database file (*.xml)|*xml",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                AnimationDatabase.Instance.Modules.Clear();
                AnimationDatabase.Instance.Misc.Clear();

                var databaseFile = fileDialog.FileName;

                // Load Database Xml
                XElement doc = XElement.Load(databaseFile);

                AnimationDatabase.Instance.Name = doc.Attribute("Name")?.Value ??
                                                  throw new BadXmlException($"{databaseFile}: Invalid Name");
                AnimationDatabase.Instance.SafePath = Path.GetDirectoryName(fileDialog.FileName) ??
                                                      throw new BadXmlException($"{databaseFile}Invalid Safe Location");

                // Parser for Database Xml
                foreach (var moduleElement in doc.Elements("Module"))
                {
                    var moduleName = moduleElement.Attribute("Name")?.Value;
                    if (moduleName is null) throw new BadXmlException($"{databaseFile}: Invalid Module Name");

                    var module = new Module(moduleName);
                    AnimationDatabase.Instance.Modules.Add(module);

                    // Different parsing for Hub- and Transitionanimationsets
                    foreach (var hubAnimationSetElement in moduleElement.Elements("Hub"))
                    {
                        var sceneId = hubAnimationSetElement.Attribute("SceneID")?.Value;
                        if (sceneId is null) throw new BadXmlException($"{databaseFile}: Invalid SceneID");

                        var animationSet = SetFinder(sceneId.Split('|'));
                        if (animationSet is not HubAnimationSet hubAnimationSet)
                            throw new BadXmlException($"{databaseFile}: Invalid Animationset");

                        animationSet.Module = module;
                        animationSet.Animator = hubAnimationSetElement.Attribute("Animator")?.Value ??
                                                string.Empty;
                        animationSet.Description = hubAnimationSetElement.Attribute("Description")?.Value ??
                                                   string.Empty;

                        foreach (var destination in hubAnimationSetElement.Elements("Destination"))
                        {
                            var destinationSceneId = destination.Value;

                            var destinationSet = SetFinder(destinationSceneId.Split('|'));
                            if (destinationSet is null)
                                throw new BadXmlException($"{databaseFile}: Invalid Animationset");

                            hubAnimationSet.Destinations.Add(destinationSet);
                        }

                        foreach (var animationElement in hubAnimationSetElement.Elements("Animation"))
                        {
                            var name = animationElement.Attribute("Name")?.Value;
                            if (name is null) throw new BadXmlException($"{databaseFile}: Invalid animation name");
                            
                            var fnisArgs = animationElement.Attribute("FnisArgument")?.Value.Split(',');
                            if (fnisArgs is null) throw new BadXmlException($"{databaseFile}: Invalid FNIS arguments");
                            
                            var creature = animationElement.Attribute("Creature")?.Value;
                            if (creature is null) throw new BadXmlException($"{databaseFile}: Invalid Creature");
                            
                            var animation = new Animation(animationSet, name, fnisArgs, creature);
                            animationSet.Animations.Add(animation);
                        }
                    }

                    foreach (var transitionAnimationSetElement in moduleElement.Elements("Transition"))
                    {
                        var sceneId = transitionAnimationSetElement.Attribute("SceneID")?.Value;
                        if (sceneId is null) throw new BadXmlException($"{databaseFile}: Invalid SceneID");

                        var animationSet = SetFinder(sceneId.Split('|'));
                        if (animationSet is not TransitionAnimationSet transitionAnimationSet)
                            throw new BadXmlException($"{databaseFile}: Invalid Animationset");

                        animationSet.Module = module;
                        animationSet.Animator = transitionAnimationSetElement.Attribute("Animator")?.Value ??
                                                string.Empty;
                        animationSet.Description = transitionAnimationSetElement.Attribute("Description")?.Value ??
                                                   string.Empty;

                        var destinationSceneId = transitionAnimationSetElement.Attribute("Destination")?.Value;
                        if (destinationSceneId is null)
                            throw new BadXmlException($"{databaseFile}: Invalid SceneID");

                        var destination = SetFinder(destinationSceneId.Split('|'));
                        if (destination is null) throw new BadXmlException($"{databaseFile}: Invalid Animationset");

                        transitionAnimationSet.Destination = destination;

                        foreach (var animationElement in transitionAnimationSetElement.Elements("Animation"))
                        {
                            var name = animationElement.Attribute("Name")?.Value;
                            if (name is null) throw new BadXmlException($"{databaseFile}: Invalid animation name");
                            
                            var fnisArgs = animationElement.Attribute("FnisArgument")?.Value.Split(',');
                            if (fnisArgs is null) throw new BadXmlException($"{databaseFile}: Invalid FNIS arguments");
                            
                            var creature = animationElement.Attribute("Creature")?.Value;
                            if (creature is null) throw new BadXmlException($"{databaseFile}: Invalid Creature");
                            
                            var animation = new Animation(animationSet, name, fnisArgs, creature);
                            animationSet.Animations.Add(animation);
                        }
                    }
                }
            }
            catch (BadXmlException e)
            {
                MessageBox.Show(e.Message);
            }

            _regionManager.RequestNavigate("TreeViewRegion", "DatabaseTreeView");
            _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish();
        }

        // Method for Saving Database
        private static void SaveDatabase()
        {
            if (string.IsNullOrEmpty(AnimationDatabase.Instance.SafePath))
            {
                FolderBrowserDialog folderBrowserDialog = new();
                {
                    folderBrowserDialog.ShowDialog();
                }
                
                if (!Directory.Exists(folderBrowserDialog.SelectedPath)) return;
                AnimationDatabase.Instance.SafePath = folderBrowserDialog.SelectedPath;
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
    }
}
