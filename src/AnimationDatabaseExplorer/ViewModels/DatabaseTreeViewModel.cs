#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    // VM for the TreeView on the left Side of the Main Window
    public class DatabaseTreeViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        
        public DatabaseTreeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            OpenSetTabCommand = new DelegateCommand<AnimationSet>(OpenSet);
        }
        
        public DelegateCommand<AnimationSet> OpenSetTabCommand { get; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            // Gets the name from the NewAnimationDatabaseDialog
            if (!string.IsNullOrEmpty(navigationContext.Parameters.GetValue<string>("name")))
                AnimationDatabase.Instance.Name = navigationContext.Parameters.GetValue<string>("name");

            // Publishes the OpenDatabase Event, Receivers: RibbonMenuViewModel
            _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish();
            
            FolderBrowserDialog folderBrowserDialog = new();
            {
                folderBrowserDialog.UseDescriptionForTitle = true;
                folderBrowserDialog.Description =
                    @"Choose the following Path in your OSex Animation: Data\meshes\0SA\mod\0Sex\scene\0MF";
                folderBrowserDialog.ShowDialog();
                
                // Loading of the default OSex Animations
                LoadOSexAnimations(folderBrowserDialog.SelectedPath);
            }
        }

        #region Methods

        // Method for loading OSex Animations into a new Database (Might get added to the constructor later on for now code stays here)
        private void LoadOSexAnimations(string rootDir)
        {
            try
            {
                foreach (var f in Directory.GetFiles(rootDir))
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
                    List<string> animations = new(){animationID};
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
                                    .Select(dest => dest.Value)
                                    .ToList();
                            else
                                destinations = doc.Elements("nav")
                                    .Elements("tab")
                                    .Elements("page")
                                    .Elements("option")
                                    .Attributes("go")
                                    .Select(dest => dest.Value)
                                    .ToList();

                            animations = doc.Elements("speed")
                                .Elements("sp")
                                .Elements("anim")
                                .Attributes("id")
                                .Select(animation => animation.Value)
                                .ToList();

                            // Adds destinations to the animationset's Destination List
                            foreach (var destination in destinations
                                .Select(dest => dest[0]
                                    .Equals('^')
                                    ? sceneID + dest[1..]
                                    : dest))
                                hubAnimationSet.Destinations.Add(SetFinder(destination));

                            break;
                        }
                    }
                    
                    foreach (var anim in animations)
                        for (var i = 0; i < int.Parse(actorCount); i++)
                        {
                            var animation = new Animation(anim + $"_{i}")
                            {
                                OldPath = Path.Combine(
                                    @"C:\Users\Admin\Downloads\OSex-17209-2-02SE-Alpha\OSex 2.02S Alpha\Data\meshes\0SA\mod\0Sex\anim\",
                                    animationSet.ModuleName, animationSet.PositionKey.Replace("!", ""),
                                    animationSet.AnimationClass, name, anim + $"_{i}.hkx"),
                                Speed = (int) char.GetNumericValue(anim[^1]),
                                Actor = i
                            };
                            if (File.Exists(animation.OldPath))
                                animationSet.Animations.Add(animation);
                        }
                }

                // recursive File searching
                foreach (var dir in Directory.GetDirectories(rootDir)) LoadOSexAnimations(dir);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static AnimationSet SetFinder(string sceneID)
        {
            foreach (var animSet in AnimationDatabase.Instance.AnimationSets)
                if (animSet.SceneID.Equals(sceneID))
                    return animSet;

            var idMatches = Regex.Matches(sceneID, @"[^\|]+");
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

        //Logic for Opening a new AnimationSet Tab
        private void OpenSet(AnimationSet animationSet)
        {
            var p = new NavigationParameters {{"animationSet", animationSet}};
            _regionManager.RequestNavigate("WorkspaceRegion", "SetWorkspaceView", p);
        }

        #endregion
    }
}
