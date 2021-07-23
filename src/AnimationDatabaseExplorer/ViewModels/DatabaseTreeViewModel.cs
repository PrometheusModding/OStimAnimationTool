using System.Collections.ObjectModel;
using System.Windows.Controls;
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
        private ObservableCollection<Module> _modules;

        public DatabaseTreeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _modules = AnimationDatabase.Instance.Modules;

            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            OpenSetTabCommand = new DelegateCommand<AnimationSet>(OpenSet);
            DragDropCommand = new DelegateCommand<object[]>(DragDropAction);
            SearchTreeViewCommand = new DelegateCommand<string>(SearchTreeView);
        }

        public ObservableCollection<Module> Modules
        {
            get => _modules;
            private set => SetProperty(ref _modules, value);
        }


        public DelegateCommand<AnimationSet> OpenSetTabCommand { get; }

        public DelegateCommand<object[]> DragDropCommand { get; }
        public DelegateCommand<string> SearchTreeViewCommand { get; }


        private void SearchTreeView(string searchFilter)
        {
            if (string.IsNullOrEmpty(searchFilter))
            {
                Modules = AnimationDatabase.Instance.Modules;
            }
            else
            {
                Module? activeModule = null;
                Modules = new ObservableCollection<Module>();
                foreach (var module in AnimationDatabase.Instance.Modules)
                {
                    foreach (var animationSet in module.AnimationSets)
                    {
                        if (!animationSet.SetName.ToLower().Contains(searchFilter.Trim().ToLower())) continue;
                        if (!Modules.Contains(module))
                        {
                            var newModule = new Module(module.Name) {Creatures = module.Creatures};
                            newModule.AnimationSets = new ObservableCollection<AnimationSet>();
                                Modules.Add(newModule);
                                activeModule = newModule;
                        }

                        activeModule?.AnimationSets.Add(animationSet);
                    }
                }
            }
        }

        private void DragDropAction(object[] dataContext)
        {
            if (dataContext[0] is not AnimationSet animationSet) return;
            if (dataContext[1] is TreeViewItem {DataContext: Module module})
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
            var p = new NavigationParameters {{"animationSet", animationSet}};
            _regionManager.RequestNavigate("WorkspaceRegion", "SetWorkspaceView", p);
        }
    }
}
