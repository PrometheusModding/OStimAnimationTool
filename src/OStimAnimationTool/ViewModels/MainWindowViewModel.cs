#region

using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;

#endregion

namespace OStimConversionTool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IDialogService dialogService, IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            NewAnimationDatabaseCommand = new DelegateCommand(NewAnimationDatabase);
            SaveDatabaseCommand = new DelegateCommand(SaveDatabase);
        }

        public DelegateCommand NewAnimationDatabaseCommand { get; }
        public DelegateCommand SaveDatabaseCommand { get; }

        private void NewAnimationDatabase()
        {
            _dialogService.ShowDialog("NewAnimationDatabaseDialog", result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    NavigationParameters p = new()
                        {{"name", result.Parameters.GetValue<string>("name")}};

                    _regionManager.RequestNavigate("TreeViewRegion", "DatabaseTreeView", p);
                }
            });
        }

        private void SaveDatabase()
        {
            _eventAggregator.GetEvent<SaveDatabaseEvent>().Publish();
        }
    }
}
