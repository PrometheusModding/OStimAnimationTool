using AnimationDatabaseExplorer.Models;
using OStimAnimationTool.Core;
using Prism.Mvvm;
using Prism.Regions;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class AnimationDetailViewModel : TabViewModelBase
    {
        private Animation _animation = new("");

        public Animation Animation
        {
            get => _animation;
            private set => SetProperty(ref _animation, value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("animation"))
                Animation = navigationContext.Parameters.GetValue<Animation>("animation");
        }
    }
}
