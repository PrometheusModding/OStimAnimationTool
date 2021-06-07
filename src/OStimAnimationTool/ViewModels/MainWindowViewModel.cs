using OStimAnimationTool.Core.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace OStimConversionTool.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(Navigate);
            eventAggregator.GetEvent<NewAnimationDatabaseEvent>().Subscribe(OnNewAnimationDatabase);
        }

        public DelegateCommand<string> NavigateCommand { get; set; }

        private void OnNewAnimationDatabase(string name)
        {
            NavigationParameters p = new () {{"name", name}};

            _regionManager.RequestNavigate("TabRegion", "AnimationSetDatagridView", p);
        }


        private void Navigate(string navigationPath)
        {
            _regionManager.RequestNavigate("TabRegion", navigationPath);
        }
    }
}
