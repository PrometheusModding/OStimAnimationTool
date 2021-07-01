#region

using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace OStimAnimationTool.Core.Models
{
    public class AnimationDatabase : ObservableCollection<AnimationSet>
    {
        public AnimationDatabase(string name)
        {
            Name = name;
            ModuleKey = Name[..3];
        }

        public string Name { get; set; }

        public string SafePath { get; set; } = string.Empty;

        public string ModuleKey { get; set; }

        public bool Contains(Animation animation)
        {
            return this.Any(animSet => animSet.Animations.Contains(animation));
        }

        public void Add(Animation animation)
        {
            foreach (var animSet in this)
                if (animation.AnimationName.Contains(animSet.SetName))
                    animSet.Animations.Add(animation);
        }
    }
}
