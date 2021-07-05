#region

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;

#endregion

namespace OStimAnimationTool.Core.Models
{
    public sealed class AnimationDatabase : BindableBase
    {
        private static readonly Lazy<AnimationDatabase> Lazy = new(() => new AnimationDatabase());
        private string _name = "New Animation Database";
        private ObservableCollection<AnimationSet> _animationSets = new();

        private AnimationDatabase()
        {
        }

        public static AnimationDatabase Instance => Lazy.Value;
        public static bool IsValueCreated => Lazy.IsValueCreated;

        public ObservableCollection<AnimationSet> AnimationSets
        {
            get => _animationSets;
            set => SetProperty(ref _animationSets, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string SafePath { get; set; } = string.Empty;

        public bool Contains(Animation animation)
        {
            return AnimationSets.Any(animSet => animSet.Animations.Contains(animation));
        }

        public void Add(Animation animation)
        {
            foreach (var animSet in AnimationSets)
                if (animation.AnimationName.Contains(animSet.SetName))
                    animSet.Animations.Add(animation);
        }
    }
}
