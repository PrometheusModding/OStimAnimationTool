using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using AnimationDatabaseExplorer.Filters;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Interfaces;
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
        private readonly Collection<IDatabaseFilter> _filters = new();
        private ObservableCollection<Module> _modules;

        public DatabaseTreeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _modules = AnimationDatabase.Instance.Modules;

            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            OpenSetTabCommand = new DelegateCommand<AnimationSet>(OpenSet);
            DragDropCommand = new DelegateCommand<object[]>(DragDropAction);
            AddNameFilterCommand = new DelegateCommand<string>(AddNameFilter);
            AddSceneTypeFilterCollection = new DelegateCommand<string>(AddTypeFilter);
        }

        public ObservableCollection<Module> Modules
        {
            get => _modules;
            private set => SetProperty(ref _modules, value);
        }


        public DelegateCommand<AnimationSet> OpenSetTabCommand { get; }
        public DelegateCommand<object[]> DragDropCommand { get; }
        public DelegateCommand<string> AddNameFilterCommand { get; }
        public DelegateCommand<string> AddSceneTypeFilterCollection { get; }


        private void AddNameFilter(string searchFilter)
        {
            AddFilter(new NameFilter(searchFilter));

            var tempModules = _filters.Aggregate(AnimationDatabase.Instance.Modules,
                (current, filter) => filter.Apply(current));
            Modules = tempModules;
        }

        private void AddTypeFilter(string sceneType)
        {
            AddFilter(new SceneTypeFilter(sceneType));

            var tempModules = _filters.Aggregate(AnimationDatabase.Instance.Modules,
                (current, filter) => filter.Apply(current));
            Modules = tempModules;
        }

        private void DragDropAction(object[] dataContext)
        {
            if (dataContext[0] is not AnimationSet animationSet) return;
            if (dataContext[1] is TreeViewItem { DataContext: Module module })
            {
                animationSet.Module.AnimationSets.Remove(animationSet);
                module.AnimationSets.Add(animationSet);
                animationSet.Module = module;
            }
            else
            {
                _eventAggregator.GetEvent<AddDestinationEvent>().Publish(animationSet);
            }
        }

        //Logic for Opening a new AnimationSet Tab
        private void OpenSet(AnimationSet animationSet)
        {
            var p = new NavigationParameters { { "animationSet", animationSet } };
            _regionManager.RequestNavigate("WorkspaceRegion", "SetWorkspaceView", p);
        }

        private void AddFilter(IDatabaseFilter databaseFilter)
        {
            foreach (var filter in _filters)
            {
                if (filter.GetType() != databaseFilter.GetType()) continue;

                filter.FilterParameter = databaseFilter.FilterParameter;
                return;
            }

            _filters.Add(databaseFilter);
        }
    }
}
