#region

using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;

#endregion

namespace AnimationDatabaseExplorer.Dialogs
{
    // VM for Dialog shown upon Creation of new AnimationDatabase
    internal class NewAnimationDatabaseDialogViewModel : BindableBase, IDialogAware
    {
        private string _name = string.Empty;

        public NewAnimationDatabaseDialogViewModel()
        {
            ConfirmDialogCommand = new DelegateCommand(ConfirmDialog);
        }

        public DelegateCommand ConfirmDialogCommand { get; }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }

        public string Title => "Create New Animation-Database";
        public event Action<IDialogResult>? RequestClose;

        private void ConfirmDialog()
        {
            const ButtonResult result = ButtonResult.OK;

            var p = new DialogParameters { { "name", Name } };
            RequestClose?.Invoke(new DialogResult(result, p));
        }
    }
}
