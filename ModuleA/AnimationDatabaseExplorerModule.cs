using AnimationDatabaseExplorer.Dialogs;
using AnimationDatabaseExplorer.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

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
            _regionManager.RegisterViewWithRegion("RibbonMenuRegion", typeof(RibbonMenuView));
            _regionManager.RegisterViewWithRegion("MenuRegion", typeof(MenuView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<NewAnimationDatabaseDialog, NewAnimationDatabaseDialogViewModel>();
            containerRegistry.RegisterForNavigation<AnimationSetDatagridView>();
            containerRegistry.RegisterForNavigation<AnimationSetDetailView>();
        }
    }
}
