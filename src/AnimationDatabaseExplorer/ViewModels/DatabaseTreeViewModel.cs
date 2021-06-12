#region

using AnimationDatabaseExplorer.Models;
using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class DatabaseTreeViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private AnimationDatabase _animationDatabase = new("New Animation Database");

        public DatabaseTreeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            OpenSetTabCommand = new DelegateCommand<AnimationSet>(OpenSet);
        }

        public AnimationDatabase AnimationDatabase
        {
            get => _animationDatabase;
            private set => SetProperty(ref _animationDatabase, value);
        }

        public DelegateCommand<AnimationSet> OpenSetTabCommand { get; }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (!string.IsNullOrEmpty(navigationContext.Parameters.GetValue<string>("name")))
                AnimationDatabase.Name = navigationContext.Parameters.GetValue<string>("name");

            _eventAggregator.GetEvent<OpenDatabaseEvent>().Publish(AnimationDatabase);
            Title = AnimationDatabase.Name;
        }

        private void OpenSet(AnimationSet animationSet)
        {
            var p = new NavigationParameters {{"animationSet", animationSet}};
            _regionManager.RequestNavigate("WorkspaceRegion", "SetWorkspaceView", p);
        }
    }
}
