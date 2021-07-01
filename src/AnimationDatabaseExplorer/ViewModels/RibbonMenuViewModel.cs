#region

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
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
        private AnimationDatabase? _animationDatabase;

        public RibbonMenuViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            AddAnimationSetCommand = new DelegateCommand(AddAnimationSet, ActiveDatabase);
            AddSlAnimationSetCommand = new DelegateCommand(AddSlAnimationSet);
            SaveDatabaseCommand = new DelegateCommand(SaveDatabase);
            OpenNavNodeViewCommand = new DelegateCommand(OpenNavNodeView);

            _regionManager = regionManager;

            eventAggregator.GetEvent<OpenDatabaseEvent>().Subscribe(SetDataContext);
            eventAggregator.GetEvent<SaveDatabaseEvent>().Subscribe(SaveDatabase);
            eventAggregator.GetEvent<ChangeAnimationClassEvent>().Subscribe(ChangeAnimationClass);
        }

        public DelegateCommand AddAnimationSetCommand { get; }
        public DelegateCommand AddSlAnimationSetCommand { get; }
        public DelegateCommand SaveDatabaseCommand { get; }
        public DelegateCommand OpenNavNodeViewCommand { get; }

        private void OpenNavNodeView()
        {
            var p = new NavigationParameters {{"animationDatabase", _animationDatabase}};
            _regionManager.RequestNavigate("WorkspaceRegion", "NavNodeView", p);

            foreach (var animationSet in _animationDatabase!)
                if (string.IsNullOrEmpty(animationSet.AnimationClass))
                    Console.WriteLine(animationSet.SetName);
        }

        private void ChangeAnimationClass()
        {
            SaveDatabaseCommand.RaiseCanExecuteChanged();
        }

        private bool ActiveDatabase()
        {
            return _animationDatabase != null;
        }

        private bool WellFormedDatabase()
        {
            return _animationDatabase is not null &&
                   _animationDatabase.All(animationSet => !string.IsNullOrEmpty(animationSet.AnimationClass));
        }

        private void SetDataContext(AnimationDatabase animationDatabase)
        {
            _animationDatabase = animationDatabase;

            AddAnimationSetCommand.RaiseCanExecuteChanged();
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
                var animation = new Animation(Path.GetFileName(filename[..^4]), filename);
                var setName = Path.GetFileName(filename[..^4]);

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

                var animationSet = new AnimationSet(setName);

                if (!_animationDatabase!.Contains(animationSet))
                    _animationDatabase.Add(animationSet);

                if (!_animationDatabase.Contains(animation))
                    _animationDatabase.Add(animation);
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
                var animation = new Animation(Path.GetFileName(filename[..^4]), filename);
                var animationSet = new AnimationSet(Path.GetFileName(filename[..^10]));

                animation.Actor = (int) char.GetNumericValue(Path.GetFileName(filename)[^8]);
                animation.Speed = (int) char.GetNumericValue(Path.GetFileName(filename)[^5]);

                if (!_animationDatabase!.Contains(animationSet))
                    _animationDatabase.Add(animationSet);

                if (!_animationDatabase.Contains(animation))
                    _animationDatabase.Add(animation);
            }

            SaveDatabaseCommand.RaiseCanExecuteChanged();
        }

        private void SaveDatabase()
        {
            if (string.IsNullOrEmpty(_animationDatabase!.SafePath))
            {
                FolderBrowserDialog folderBrowserDialog = new();
                {
                    folderBrowserDialog.ShowDialog();
                    _animationDatabase.SafePath = folderBrowserDialog.SelectedPath;
                }
            }
            
            DatabaseScriber databaseScriber = new(_animationDatabase);
            databaseScriber.XmlScriber();
            databaseScriber.FnisScriber();
            databaseScriber.DatabaseFileScriber();
        }
    }
}
