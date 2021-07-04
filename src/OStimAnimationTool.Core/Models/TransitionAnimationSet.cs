using System.Text.RegularExpressions;

namespace OStimAnimationTool.Core.Models
{
    public class TransitionAnimationSet : AnimationSet
    {
        private AnimationSet _destination = new(string.Empty);

        public TransitionAnimationSet(string setName) : base(setName)
        {
        }

        public string ParentSet => GetParentSet();

        public AnimationSet Destination
        {
            get => _destination;
            set => SetProperty(ref _destination, value);
        }

        private string GetParentSet()
        {
            var m = Regex.Match(SetName, @"\+");
            return m.Success ? SetName[..m.Index] : SetName;
        }
    }
}
