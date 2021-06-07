using OStimAnimationTool.Core;
using Prism.Regions;

namespace AnimationDatabaseExplorer.ViewModels
{
    public class AnimationSetDetailViewModel : TabViewModelBase
    {
        public override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }
    }
}
