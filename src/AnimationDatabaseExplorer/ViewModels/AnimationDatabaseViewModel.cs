using System;
using System.IO;
using AnimationDatabaseExplorer.Models;
using Microsoft.Win32;
using OStimAnimationTool.Core;
using Prism.Commands;
using Prism.Regions;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class AnimationDatabaseViewModel : TabViewModelBase
    {
        private readonly IRegionManager _regionManager;
        private AnimationDatabase _animationDatabase = new("");
        private string _name = string.Empty;

        protected AnimationDatabaseViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            AddAnimationSetCommand = new DelegateCommand(AddAnimationSet);
            OpenSetDetailCommand = new DelegateCommand<AnimationSet>(OpenSetDetail);
        }

        public AnimationDatabase AnimationDatabase
        {
            get => _animationDatabase;
            private set => SetProperty(ref _animationDatabase, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public DelegateCommand AddAnimationSetCommand { get; }

        public DelegateCommand<AnimationSet> OpenSetDetailCommand { get; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            _name = navigationContext.Parameters.GetValue<string>("name");
            AnimationDatabase = new AnimationDatabase(_name);
            Title = _name;
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
                var animation = new Animation(Path.GetFileName(filename));
                var animationSet =
                    new AnimationSet(Path.GetFileName(filename).Remove(openFileDialog.SafeFileName.Length - 10));

                if (!_animationDatabase.Contains(animationSet))
                    _animationDatabase.Add(animationSet);

                if (!_animationDatabase.Contains(animation))
                    _animationDatabase.Add(animation);
            }
        }

        private void OpenSetDetail(AnimationSet animationSet)
        {
            var p = new NavigationParameters {{"animationSet", animationSet}};
            _regionManager.RequestNavigate("TabRegion", "AnimationSetDetailView", p);
        }
    }
}
