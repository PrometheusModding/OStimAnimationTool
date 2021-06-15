#region

using System;
using Prism;
using Prism.Mvvm;
using Prism.Regions;

#endregion

namespace OStimAnimationTool.Core
{
    public class ViewModelBase : BindableBase, INavigationAware, IActiveAware
    {
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }


        public event EventHandler? IsActiveChanged;

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
