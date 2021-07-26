using System.Collections.ObjectModel;

namespace OStimAnimationTool.Core.Models
{
    public class HubAnimationSet : AnimationSet
    {
        private ObservableCollection<AnimationSet> _destinations = new();

        public HubAnimationSet(string setName) : base(setName)
        {
        }
        
        public HubAnimationSet(Module module, string positionKey, string animationClass, string setName) : base(module, positionKey, animationClass, setName)
        {
        }

        public ObservableCollection<AnimationSet> Destinations
        {
            get => _destinations;
            set => SetProperty(ref _destinations, value);
        }
    }
}
