using System;
using Prism;
using Prism.Mvvm;
using Prism.Regions;

namespace OStimAnimationTool.Core
{
    public class TabViewModelBase : BindableBase, INavigationAware, IActiveAware
    {
        private bool _isActive;
        private string _title;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }


        public event EventHandler IsActiveChanged;

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
        }
    }
}
