using Prism.Mvvm;

namespace OStimAnimationTool.Core.Models.Navigation
{
    public class Option : BindableBase
    {
        private AnimationSet _destination;
        private OptionIcons _icon;

        public Option(AnimationSet destination)
        {
            _destination = destination;
        }

        public AnimationSet Destination
        {
            get => _destination;
            set => SetProperty(ref _destination, value);
        }

        public OptionIcons Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }
    }
}
