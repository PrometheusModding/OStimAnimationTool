using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace OStimAnimationTool.Core.Models
{
    public class Module : BindableBase
    {
        private string _name;
        private ObservableCollection<AnimationSet> _animationSets = new();

        public Module(string name)
        {
            _name = name;
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, () =>
            {
                foreach (var animationSet in AnimationSets)
                {
                    animationSet.NameChanged();
                }
            });
        }

        public ObservableCollection<AnimationSet> AnimationSets
        {
            get => _animationSets;
            set => SetProperty(ref _animationSets, value);
        }
    }
}
