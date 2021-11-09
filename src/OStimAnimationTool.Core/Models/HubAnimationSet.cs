using System.Collections.ObjectModel;
using OStimAnimationTool.Core.Models.Navigation;

namespace OStimAnimationTool.Core.Models
{
    public class HubAnimationSet : AnimationSet
    {
        private ObservableCollection<Tab> _navTabs = new();

        public HubAnimationSet(Module module, string positionKey, string animationClass, string setName) : base(module,
            positionKey, animationClass, setName)
        {
        }

        public HubAnimationSet(Module module, string setName) : base(module, setName)
        {
        }

        public ObservableCollection<Tab> NavTabs
        {
            get => _navTabs;
            set => SetProperty(ref _navTabs, value);
        }
    }
}
