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

        /*private void AnimationsSelect()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Havok Animation files (*.hkx)|*hkx|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (openFileDialog.ShowDialog() != true) return;

            _sourceDir = openFileDialog.FileName;
            _sourceDir = _sourceDir.Remove(_sourceDir.Length - openFileDialog.SafeFileName.Length);

            foreach (var filename in openFileDialog.FileNames)
            {
                _animationName = !string.IsNullOrEmpty(AnimationSetnameTextbox.Text)
                    ? AnimationSetnameTextbox.Text
                    : Path.GetFileName(filename).Remove(openFileDialog.SafeFileName.Length - 10);
                _animationClass = !string.IsNullOrEmpty(AnimationClassTextbox.Text)
                    ? AnimationClassTextbox.Text
                    : string.Empty;
                Animation anim = new(_animationName, Path.GetFileName(filename), _animationClass, _animator);

                if (!_animationDatabase.Contains(anim))
                    _animationDatabase.Add(anim);
            }

            if (AnimationDatabaseView != null)
            {
                AnimationDatabaseView.GroupDescriptions.Clear();
                AnimationDatabaseView.GroupDescriptions.Add(new PropertyGroupDescription("SetName"));
            }
        }*/
    }
}
