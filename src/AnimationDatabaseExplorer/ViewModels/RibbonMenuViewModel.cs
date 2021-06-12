#region

using System;
using System.IO;
using System.Windows.Forms;
using AnimationDatabaseExplorer.Models;
using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using Prism.Commands;
using Prism.Events;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class RibbonMenuViewModel : ViewModelBase
    {
        private AnimationDatabase? _animationDatabase;

        public RibbonMenuViewModel(IEventAggregator eventAggregator)
        {
            AddAnimationSetCommand = new DelegateCommand(AddAnimationSet, ActiveDatabase);
            SaveDatabaseCommand = new DelegateCommand(SaveDatabase, WellFormedDatabase);

            eventAggregator.GetEvent<OpenDatabaseEvent>().Subscribe(SetDataContext);
            eventAggregator.GetEvent<SaveDatabaseEvent>().Subscribe(SaveDatabase);
            eventAggregator.GetEvent<ChangeAnimationClassEvent>().Subscribe(ChangeAnimationClass);
        }

        public DelegateCommand AddAnimationSetCommand { get; }
        public DelegateCommand SaveDatabaseCommand { get; }

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
            if (_animationDatabase is null)
                return false;
            foreach (var animationSet in _animationDatabase)
                if (string.IsNullOrEmpty(animationSet.AnimationClass))
                    return false;

            return true;
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
                var animation = new Animation(Path.GetFileName(filename.Remove(filename.Length - 4)), filename);
                var animationSet =
                    new AnimationSet(Path.GetFileName(filename.Remove(filename.Length - 10)));

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

            foreach (var animationSet in _animationDatabase!)
            {
                var animationClass = animationSet.AnimationClass;
                var setName = animationSet.SetName;
                var setDir = Path.Combine(_animationDatabase.SafePath, @"meshes\0SA\mod\0Sex\anim\",
                    _animationDatabase.ModuleKey, animationClass, setName);

                if (!Directory.Exists(setDir))
                    Directory.CreateDirectory(setDir);

                foreach (var animation in animationSet)
                    File.Copy(animation.OldPath, Path.Combine(setDir, animation.AnimationName + ".hkx"));

                DatabaseScriber databaseScriber = new(_animationDatabase, animationSet);
                databaseScriber.XmlScriber();
                databaseScriber.FnisScriber();
            }
        }
    }
}
