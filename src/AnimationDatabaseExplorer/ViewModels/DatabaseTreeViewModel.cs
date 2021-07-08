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

        public DatabaseTreeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            OpenSetTabCommand = new DelegateCommand<AnimationSet>(OpenSet);
            AddDestinationCommand = new DelegateCommand<AnimationSet>(AddDestination);
        }

        public DelegateCommand<AnimationSet> OpenSetTabCommand { get; }
        public DelegateCommand<AnimationSet> AddDestinationCommand { get; }

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
