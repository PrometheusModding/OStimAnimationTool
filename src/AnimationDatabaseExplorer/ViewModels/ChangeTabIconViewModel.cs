using System;
using MaterialDesignThemes.Wpf;
using OStimAnimationTool.Core.Events;
using OStimAnimationTool.Core.Models.Navigation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class ChangeTabIconViewModel : BindableBase
    {
        private object? _navElement;

        private ChangeTabIconViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ChangeIconEvent>().Subscribe(navElement => NavElement = navElement);

            ApplyIconCommand = new DelegateCommand<object>(ApplyIcon);
        }

        public object? NavElement
        {
            get => _navElement;
            set => SetProperty(ref _navElement, value);
        }

        public DelegateCommand<object> ApplyIconCommand { get; }

        private void ApplyIcon(object icon)
        {
            switch (icon)
            {
                case TabIcons tabIcons:
                    if (_navElement is Tab tab) tab.Icon = tabIcons;
                    break;
                case PageIcons pageIcons:
                    if (_navElement is Page page) page.Icon = pageIcons;
                    break;
                case OptionIcons optionIcons:
                    if (_navElement is Option option) option.Icon = optionIcons;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            DialogHost.CloseDialogCommand.Execute(null, null);
        }
    }
}
