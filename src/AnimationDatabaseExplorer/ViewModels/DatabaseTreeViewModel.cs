using System.Collections.ObjectModel;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using OStimAnimationTool.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace AnimationDatabaseExplorer.ViewModels
{
    // VM for the TreeView on the left Side of the Main Window
    public class DatabaseTreeViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private ObservableCollection<AnimationSet> _animationSets;

        public DatabaseTreeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _animationSets = AnimationDatabase.Instance.AnimationSets;

            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            OpenSetTabCommand = new DelegateCommand<AnimationSet>(OpenSet);
            AddDestinationCommand = new DelegateCommand<AnimationSet>(AddDestination);
            SearchTreeViewCommand = new DelegateCommand<string>(SearchTreeView);
        }

        public ObservableCollection<AnimationSet> AnimationSets
        {
            get => _animationSets;
            private set => SetProperty(ref _animationSets, value);
        }


        public DelegateCommand<AnimationSet> OpenSetTabCommand { get; }
        public DelegateCommand<AnimationSet> AddDestinationCommand { get; }
        public DelegateCommand<string> SearchTreeViewCommand { get; }


        private void SearchTreeView(string searchFilter)
        {
            if (string.IsNullOrEmpty(searchFilter))
            {
                AnimationSets = AnimationDatabase.Instance.AnimationSets;
            }
            else
            {
                AnimationSets = new ObservableCollection<AnimationSet>();
                foreach (var n in AnimationDatabase.Instance.AnimationSets)
                    if (n.SetName.ToLower().StartsWith(searchFilter.Trim().ToLower()))
                        AnimationSets.Add(n);
            }
        }

        private void AddDestination(AnimationSet animationSet)
        {
            _eventAggregator.GetEvent<AddDestinationEvent>().Publish(animationSet);
        }

        //Logic for Opening a new AnimationSet Tab
        private void OpenSet(AnimationSet animationSet)
        {
            var p = new NavigationParameters {{"animationSet", animationSet}};
            _regionManager.RequestNavigate("WorkspaceRegion", "SetWorkspaceView", p);
        }
    }
}
