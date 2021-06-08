using System.Collections.ObjectModel;
using System.Linq;

namespace AnimationDatabaseExplorer.Models
{
    public class AnimationDatabase : ObservableCollection<AnimationSet>
    {
        public AnimationDatabase(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public new bool Contains(AnimationSet animationSet)
        {
            return this.Any(animSet => animSet.SetName.Equals(animationSet.SetName));
        }

        public bool Contains(Animation animation)
        {
            return this.Any(animSet => animSet.Contains(animation));
        }

        public void Add(Animation animation)
        {
            foreach (var animSet in this)
                if (animation.AnimationName.Contains(animSet.SetName))
                    animSet.Add(animation);
        }
    }
}
