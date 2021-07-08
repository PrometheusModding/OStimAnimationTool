using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DynamicData;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using OStimAnimationTool.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using ReactiveUI;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class RibbonMenuViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;

        public RibbonMenuViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            
            AddAnimationSetCommand = new DelegateCommand(AddAnimationSet, ActiveDatabase);
            AddSlAnimationSetCommand = new DelegateCommand(AddSlAnimationSet, ActiveDatabase);
            OpenNavNodeViewCommand = new DelegateCommand(OpenNavNetworkView, ActiveDatabase);

            eventAggregator.GetEvent<OpenDatabaseEvent>().Subscribe(() => AddSlAnimationSetCommand.RaiseCanExecuteChanged());
        }

        public DelegateCommand AddAnimationSetCommand { get; }
        public DelegateCommand AddSlAnimationSetCommand { get; }
        public DelegateCommand OpenNavNodeViewCommand { get; }

        private void OpenNavNetworkView()
        {
            var p = new NavigationParameters {{"animationDatabase", AnimationDatabase.Instance}};
            _regionManager.RequestNavigate("WorkspaceRegion", "NavNetworkView", p);

            foreach (var animationSet in AnimationDatabase.Instance.AnimationSets)
                if (string.IsNullOrEmpty(animationSet.AnimationClass))
                    Console.WriteLine(animationSet.SetName);
        }
        
        private static bool ActiveDatabase()
        {
            return AnimationDatabase.IsValueCreated;
        }

        private static void AddAnimationSet()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Havok Animation files (*.hkx)|*hkx|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() != true) return;

            foreach (var filename in openFileDialog.FileNames)
            {
                var setName = Path.GetFileName(filename[..^4]);
                var animationSet = new AnimationSet(setName);
                var animation = new Animation(filename, animationSet);

                var actorMatch = Regex.Match(setName, @"A(\d)");
                if (actorMatch.Success)
                    animation.Actor = int.Parse(actorMatch.Groups[1].Value);

                var speedMatch = Regex.Match(setName, @"S(\d)");
                if (speedMatch.Success)
                    animation.Speed = int.Parse(speedMatch.Groups[1].Value);

                if (speedMatch.Success && actorMatch.Success)
                    setName = actorMatch.Groups[1].Index < speedMatch.Groups[1].Index
                        ? setName[..(actorMatch.Groups[1].Index - 1)]
                        : setName[..(speedMatch.Groups[1].Index - 1)];
            }
        }

        private static void AddSlAnimationSet()
        {
            var fileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Havok Animation files (*.hkx)|*hkx|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (fileDialog.ShowDialog() != true) return;

            string setName = string.Empty;
            AnimationSet animationSet = new(string.Empty);

            foreach (var filename in fileDialog.FileNames)
            {
                if (!setName.Equals(Path.GetFileName(filename[..^10])))
                {
                    animationSet = SetFinder(Path.GetFileName(filename[..^10])); 
                }

                if (animationSet == null) continue;
                var animation = new Animation(filename, animationSet)
                {
                    Actor = (int) char.GetNumericValue(Path.GetFileName(filename)[^8]) == 1 ? 1 : 0,
                    Speed = (int) char.GetNumericValue(Path.GetFileName(filename)[^5]) - 1
                };
                
                if (!AnimationDatabase.Instance.AnimationSets.Contains(animationSet))
                    AnimationDatabase.Instance.AnimationSets.Add(animationSet);
                    
                if(!animationSet.Animations.Contains(animation))
                    animationSet.Animations.Add(animation);
            }
        }

        private static AnimationSet SetFinder(string setName)
        {
            foreach (var animationSet in AnimationDatabase.Instance.AnimationSets)
            {
                if (setName.Equals(animationSet.SetName))
                    return animationSet;
            }

            return new HubAnimationSet(setName);
        }
    }
}
