using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace OStimAnimationTool.Core.Models
{
    public class Module : BindableBase, IEquatable<Module>
    {
        private ObservableCollection<AnimationSet> _animationSets = new();
        private string _name;

        public Module(string name)
        {
            _name = name;
        }

        public List<string>? Creatures { get; set; }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, () =>
            {
                foreach (var animationSet in AnimationSets) animationSet.SceneIdChanged();
            });
        }

        public ObservableCollection<AnimationSet> AnimationSets
        {
            get => _animationSets;
            set => SetProperty(ref _animationSets, value);
        }

        public bool Equals(Module? other)
        {
            return Name == other?.Name;
        }
    }
}
