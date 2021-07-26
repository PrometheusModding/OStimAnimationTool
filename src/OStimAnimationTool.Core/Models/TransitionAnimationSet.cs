using System;
using System.Text.RegularExpressions;

namespace OStimAnimationTool.Core.Models
{
    public class TransitionAnimationSet : AnimationSet
    {
        private AnimationSet _destination = new(string.Empty);

        public TransitionAnimationSet(string setName) : base(setName)
        {
        }
        
        public TransitionAnimationSet(Module module, string positionKey, string animationClass, string setName) : base(module, positionKey, animationClass, setName)
        {
        }

        public AnimationSet Destination
        {
            get => _destination;
            set => SetProperty(ref _destination, value);
        }

        public string ParentSet => GetParentSet();

        public string Emf => GetEmf();

        private string GetEmf()
        {
            var m = Regex.Match(SetName, @"\+");
            var a = new Range(m.Index + 1, m.Index + 3);
            return SetName[a];
        }

        private string GetParentSet()
        {
            var m = Regex.Match(SetName, @"\+");
            return m.Success ? SetName[..m.Index] : SetName;
        }
    }
}
