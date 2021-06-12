#region

using OStimAnimationTool.Core;
using OStimAnimationTool.Core.Models;
using Prism.Regions;

#endregion

namespace AnimationDatabaseExplorer.ViewModels
{
    public class AnimationDetailViewModel : ViewModelBase
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
