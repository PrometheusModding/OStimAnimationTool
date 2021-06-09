using OStimAnimationTool.Core.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        private readonly IDialogService _dialogService;
        private readonly IEventAggregator _eventAggregator;

        public MenuViewModel(IDialogService dialogService, IEventAggregator eventAggregator)
        {
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            NewAnimationDatabaseCommand = new DelegateCommand(CreateNewAnimDatabase);
        }

        public DelegateCommand NewAnimationDatabaseCommand { get; }

        private void CreateNewAnimDatabase()
        {
            _dialogService.ShowDialog("NewAnimationDatabaseDialog", result =>
            {
                if (result.Result == ButtonResult.OK)
                    _eventAggregator.GetEvent<NewAnimationDatabaseEvent>()
                        .Publish(result.Parameters.GetValue<string>("name"));
            });
        }
    }
}
