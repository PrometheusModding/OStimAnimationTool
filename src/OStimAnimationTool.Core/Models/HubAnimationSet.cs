using System.Collections.Generic;

namespace OStimAnimationTool.Core.Models
{
    public class HubAnimationSet : AnimationSet
    {
        private List<AnimationSet> _destinations = new();

        public HubAnimationSet(string setName) : base(setName)
        {
        }

        public List<AnimationSet> Destinations
        {
            get => _destinations;
            set => SetProperty(ref _destinations, value);
        }
    }
}
