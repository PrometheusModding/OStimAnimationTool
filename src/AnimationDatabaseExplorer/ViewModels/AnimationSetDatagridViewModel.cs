using System;
using System.IO;
using AnimationDatabaseExplorer.Models;
using Microsoft.Win32;
using OStimAnimationTool.Core;
using Prism.Commands;
using Prism.Regions;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class AnimationSetDatagridViewModel : TabViewModelBase
    {
        private AnimationDatabase _animationDatabase;
        private string _name;
        private IRegionManager _regionManager;

        public AnimationSetDatagridViewModel()
        {
            AddAnimationSetCommand = new DelegateCommand(AddAnimationSet);
            OpenSetDetailCommand = new DelegateCommand<AnimationSet>(OpenSetDetail);
        }

        public AnimationDatabase AnimationDatabase
        {
            get => _animationDatabase;
            set => SetProperty(ref _animationDatabase, value);
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
            _regionManager = navigationContext.Parameters.GetValue<IRegionManager>("regionManager");
            AnimationDatabase = new AnimationDatabase(_name);
            Title = _name;
        }

        public void AddAnimationSet()
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
            if (animationSet is null)
                return;

            var p = new NavigationParameters();
            p.Add("animationSet", animationSet);
            p.Add("regionManager", _regionManager);
            _regionManager.RequestNavigate("TabRegion", "AnimationSetDetailView", p);
        }
    }
}
