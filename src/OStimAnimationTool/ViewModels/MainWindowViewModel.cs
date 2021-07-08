using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
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
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace OStimConversionTool.ViewModels
{
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
            SaveDatabaseCommand = new DelegateCommand(SaveDatabase, WellFormedDatabase);
        }

        public DelegateCommand NewDatabaseCommand { get; }
        public DelegateCommand LoadDatabaseCommand { get; }
        public DelegateCommand SaveDatabaseCommand { get; }

        private static bool WellFormedDatabase()
        {
            return AnimationDatabase.IsValueCreated &&
                   AnimationDatabase.Instance.AnimationSets.All(animationSet =>
                       !string.IsNullOrEmpty(animationSet.AnimationClass) &&
                       !string.IsNullOrEmpty(animationSet.PositionKey) &&
                       !string.IsNullOrEmpty(animationSet.ModuleName));
        }

        private void NewAnimationDatabase()
        {
            _dialogService.ShowDialog("NewAnimationDatabaseDialog", result =>
            {
                if (result.Result != ButtonResult.OK) return;
                if (!string.IsNullOrEmpty(result.Parameters.GetValue<string>("name")))
                    AnimationDatabase.Instance.Name = result.Parameters.GetValue<string>("name");

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

                AnimationDatabase.Instance.AnimationSets.ToObservableChangeSet().AutoRefresh(x => x.SceneID)
                    .Subscribe(_ => SaveDatabaseCommand.RaiseCanExecuteChanged());

                _regionManager.RequestNavigate("TreeViewRegion", "DatabaseTreeView");
                _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish();
            });
        }

        // Method for loading OSex Animations into a new Database (Might get added to the constructor later on for now code stays here)
        private void LoadOSexAnimations(string dir)
        {
            try
            {
                if (Path.GetDirectoryName(dir) != "EMF")
                    foreach (var f in Directory.GetFiles(dir))
                    {
                        XElement doc = XElement.Load(f);

                        // gets all the necessary information from the .xml
                        var sceneID = doc.Attribute("id")?.Value ?? string.Empty;
                        var animationID = doc.Element("anim")?.Attribute("id")?.Value ?? string.Empty;
                        var actorCount = doc.Attribute("actors")?.Value ?? "2";
                        var setDescription = doc.Element("info")?
                            .Attribute("name")?.Value ?? string.Empty;
                        var animator = doc.Element("info")?
                            .Attribute("animator")?.Value ?? string.Empty;

                        if (f == "AutoStartBasic")
                            sceneID = doc.Element("togs")
                                ?.Element("tog0")
                                ?.Element("tog1")
                                ?.Attribute("id")
                                ?.Value ?? string.Empty;

                        //returns the AnimationSet with the correct sceneID if already in the Database or creates a new AnimationSet
                        var animationSet = SetFinder(sceneID);
                        animationSet.Description = setDescription;
                        animationSet.Animator = animator;
                        List<string> animations = new() {animationID};
                        string name = animationSet.SetName;

                        switch (animationSet)
                        {
                            //different parsing for different .xml types
                            case TransitionAnimationSet transitionAnimationSet:
                                var dest = doc
                                    .Element("anim")?
                                    .Attribute("dest")?
                                    .Value ?? string.Empty;
                                dest = dest.StartsWith('^')
                                    ? sceneID + dest[1..]
                                    : dest;
                                transitionAnimationSet.Destination = SetFinder(dest);
                                name = transitionAnimationSet.ParentSet;
                                break;
                            case HubAnimationSet hubAnimationSet:
                            {
                                List<string> destinations;
                                if (f == "AutoStartBasic")
                                    destinations = doc.Elements("togs")
                                        .Elements("tog0")
                                        .Elements("dest")
                                        .Attributes("id")
                                        .Select(d => d.Value)
                                        .ToList();
                                else
                                    destinations = doc.Elements("nav")
                                        .Elements("tab")
                                        .Elements("page")
                                        .Elements("option")
                                        .Attributes("go")
                                        .Select(d => d.Value)
                                        .ToList();

                                animations = doc.Elements("speed")
                                    .Elements("sp")
                                    .Elements("anim")
                                    .Attributes("id")
                                    .Select(animation => animation.Value)
                                    .ToList();

                                // Adds destinations to the animationset's Destination List
                                foreach (var destination in destinations
                                    .Select(d => d[0]
                                        .Equals('^')
                                        ? sceneID + d[1..]
                                        : d))
                                    hubAnimationSet.Destinations.Add(SetFinder(destination));
                                break;
                            }
                        }

                        foreach (var anim in animations)
                            for (var i = 0; i < int.Parse(actorCount); i++)
                            {
                                var oldPath = Path.Combine(_oSexDirectory, animationSet.ModuleName,
                                    animationSet.PositionKey.Replace("!", ""),
                                    animationSet.AnimationClass, name, anim + $"_{i}.hkx");
                                var animation = new Animation(oldPath, animationSet)
                                {
                                    Speed = (int) char.GetNumericValue(anim[^1]),
                                    Actor = i
                                };
                                if (File.Exists(animation.OldPath))
                                    animationSet.Animations.Add(animation);
                            }
                    }

                // recursive File searching
                foreach (var d in Directory.GetDirectories(dir)) LoadOSexAnimations(d);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadDatabase()
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Animation Database file (*.xml)|*xml",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() != true) return;

            AnimationDatabase.Instance.SafePath = Path.GetDirectoryName(fileDialog.FileName)!;
            XElement doc = XElement.Load(fileDialog.FileName);
            AnimationDatabase.Instance.Name = doc.Attribute("Name")?.Value ?? string.Empty;
            var elements = doc.Elements("HubAnimationSet").ToList();
            elements.AddRange(doc.Elements("TransitionAnimationSet"));
            foreach (var set in elements)
            {
                var animationSet = SetFinder(set.Attribute("SceneID")?.Value ?? string.Empty);
                animationSet.Animator = set.Attribute("Animator")?.Value ?? string.Empty;
                animationSet.Description = set.Attribute("Description")?.Value ?? string.Empty;

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
            }

            AnimationDatabase.Instance.AnimationSets.ToObservableChangeSet().AutoRefresh(x => x.SceneID)
                .Subscribe(_ => SaveDatabaseCommand.RaiseCanExecuteChanged());

            _regionManager.RequestNavigate("TreeViewRegion", "DatabaseTreeView");
            _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish();
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

        private static AnimationSet SetFinder(string sceneID)
        {
            foreach (var animSet in AnimationDatabase.Instance.AnimationSets)
                if (animSet.SceneID.Equals(sceneID))
                    return animSet;

            var idMatches = Regex.Matches(sceneID, @"[^\|]+");
            if (idMatches.Count != 4) return new AnimationSet("");
            var moduleName = idMatches[0].ToString();
            var positionKey = idMatches[1].ToString();
            var animationClass = idMatches[2].ToString();
            var setName = idMatches[3].ToString();

            AnimationSet animationSet = setName.Contains('+')
                ? new TransitionAnimationSet(setName)
                : new HubAnimationSet(setName);

            animationSet.ModuleName = moduleName;
            animationSet.PositionKey = positionKey;
            animationSet.AnimationClass = animationClass;

            AnimationDatabase.Instance.AnimationSets.Add(animationSet);
            return animationSet;
        }
    }
}
