using AnimationDatabaseExplorer.Models;
using OStimAnimationTool.Core;
using Prism.Regions;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class AnimationSetDetailViewModel : TabViewModelBase
    {
        private AnimationSet _animationSet;


        public AnimationSet AnimationSet
        {
            get => _animationSet;
            set => SetProperty(ref _animationSet, value);
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameters.ContainsKey("animationSet"))
                AnimationSet = navigationContext.Parameters.GetValue<AnimationSet>("animationSet");
        }

        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }
    }
}
