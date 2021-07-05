#region

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using OStimAnimationTool.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class RibbonMenuViewModel : ViewModelBase
    {
        private readonly IRegionManager _regionManager;

        public RibbonMenuViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            AddAnimationSetCommand = new DelegateCommand(AddAnimationSet, ActiveDatabase);
            AddSlAnimationSetCommand = new DelegateCommand(AddSlAnimationSet, ActiveDatabase);
            SaveDatabaseCommand = new DelegateCommand(SaveDatabase, WellFormedDatabase);
            OpenNavNodeViewCommand = new DelegateCommand(OpenNavNetworkView, ActiveDatabase);

            _regionManager = regionManager;

            eventAggregator.GetEvent<OpenDatabaseEvent>().Subscribe(SetDataContext);
            eventAggregator.GetEvent<SaveDatabaseEvent>().Subscribe(SaveDatabase);
            eventAggregator.GetEvent<ChangeAnimationClassEvent>().Subscribe(ChangeAnimationClass);
        }

        public DelegateCommand AddAnimationSetCommand { get; }
        public DelegateCommand AddSlAnimationSetCommand { get; }
        public DelegateCommand SaveDatabaseCommand { get; }
        public DelegateCommand OpenNavNodeViewCommand { get; }

        private void OpenNavNetworkView()
        {
            var p = new NavigationParameters {{"animationDatabase", AnimationDatabase.Instance}};
            _regionManager.RequestNavigate("WorkspaceRegion", "NavNetworkView", p);

            foreach (var animationSet in AnimationDatabase.Instance.AnimationSets)
                if (string.IsNullOrEmpty(animationSet.AnimationClass))
                    Console.WriteLine(animationSet.SetName);
        }

        private void ChangeAnimationClass()
        {
            SaveDatabaseCommand.RaiseCanExecuteChanged();
        }

        private static bool ActiveDatabase()
        {
            return AnimationDatabase.IsValueCreated;
        }

        private static bool WellFormedDatabase()
        {
            return AnimationDatabase.IsValueCreated && AnimationDatabase.Instance.AnimationSets.All(animationSet => !string.IsNullOrEmpty(animationSet.AnimationClass));
        }

        private void SetDataContext()
        {
            SaveDatabaseCommand.RaiseCanExecuteChanged();
            AddAnimationSetCommand.RaiseCanExecuteChanged();
            AddSlAnimationSetCommand.RaiseCanExecuteChanged();
        }

        private void AddAnimationSet()
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
                var animation = new Animation(Path.GetFileName(filename[..^4]), filename, animationSet);

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

                if (!AnimationDatabase.Instance.AnimationSets.Contains(animationSet))
                    AnimationDatabase.Instance.AnimationSets.Add(animationSet);

                if (!AnimationDatabase.Instance.Contains(animation))
                    AnimationDatabase.Instance.Add(animation);
            }

            SaveDatabaseCommand.RaiseCanExecuteChanged();
        }

        private void AddSlAnimationSet()
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
                var animationSet = new AnimationSet(Path.GetFileName(filename[..^10]));
                var animation = new Animation(Path.GetFileName(filename[..^4]), filename, animationSet)
                {
                    Actor = (int) char.GetNumericValue(Path.GetFileName(filename)[^8]),
                    Speed = (int) char.GetNumericValue(Path.GetFileName(filename)[^5])
                };


                if (!AnimationDatabase.Instance.AnimationSets.Contains(animationSet))
                    AnimationDatabase.Instance.AnimationSets.Add(animationSet);

                if (!AnimationDatabase.Instance.Contains(animation))
                    AnimationDatabase.Instance.Add(animation);
            }

            SaveDatabaseCommand.RaiseCanExecuteChanged();
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

            DatabaseScriber databaseScriber = new(AnimationDatabase.Instance);
            databaseScriber.XmlScriber();
            databaseScriber.FnisScriber();
            databaseScriber.DatabaseFileScriber();
        }
    }
}
