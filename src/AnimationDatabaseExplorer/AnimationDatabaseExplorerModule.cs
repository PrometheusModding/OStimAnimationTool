#region

using AnimationDatabaseExplorer.Dialogs;
using AnimationDatabaseExplorer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

#endregion

namespace AnimationDatabaseExplorer
{
    public class AnimationDatabaseExplorerModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public AnimationDatabaseExplorerModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion("RibbonRegion", typeof(RibbonMenuView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NewAnimationDatabaseDialog, NewAnimationDatabaseDialogViewModel>();
            containerRegistry.RegisterForNavigation<SetDetailView>();
            containerRegistry.RegisterForNavigation<AnimationDetailView>();
            containerRegistry.RegisterForNavigation<DatabaseTreeView>();
            containerRegistry.RegisterForNavigation<SetWorkspaceView>();
            containerRegistry.RegisterForNavigation<NavNodeView>();
        }
    }
}
