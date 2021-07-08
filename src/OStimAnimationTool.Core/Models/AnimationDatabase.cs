using System;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace OStimAnimationTool.Core.Models
{
    public sealed class AnimationDatabase : BindableBase
    {
        private static readonly Lazy<AnimationDatabase> Lazy = new(() => new AnimationDatabase());
        private ObservableCollection<AnimationSet> _animationSets = new();
        private string _name = "New Animation Database";

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
    }
}
