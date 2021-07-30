using System;
using System.Collections.Generic;
using Prism.Mvvm;

namespace OStimAnimationTool.Core.Models
{
    public class Animation : BindableBase, IEquatable<Animation>
    {
        private int _actor;
        private AnimationSet _animationSet = new();
        private string _creature = string.Empty;
        private List<string> _fnisArgs = new();
        private string _oldPath = string.Empty;
        private int _speed;

        public Animation(AnimationSet animationSet, string animationName)
        {
            _animationSet = animationSet;

            switch (animationSet)
            {
                case HubAnimationSet:
                    _speed = animationName[^3];
                    _actor = animationName[^1];
                    break;
                case TransitionAnimationSet:
                    _actor = animationName[^1];
                    break;
            }
        }

        public Animation(string oldPath, AnimationSet animationSet)
        {
            _oldPath = oldPath;
            _animationSet = animationSet;
        }

        public Animation(string oldPath, AnimationSet animationSet, int speed, int actor, List<string> fnisArgs,
            string creature)
        {
            _oldPath = oldPath;
            _animationSet = animationSet;
            _speed = speed;
            _actor = actor;
            _fnisArgs = fnisArgs;
            _creature = creature;
        }

        public string AnimationName
        {
            get
            {
                return AnimationSet switch
                {
                    HubAnimationSet => "0Sx" + _animationSet.Module.Name + $"_{_animationSet.AnimationClass}" +
                                       $"-{_animationSet.SetName}" + $"_S{_speed.ToString()}" + $"_{_actor.ToString()}",
                    TransitionAnimationSet => "0Sx" + _animationSet.Module.Name + $"_{_animationSet.AnimationClass}" +
                                              $"-{_animationSet.SetName}" + $"_{_actor.ToString()}",
                    _ => string.Empty
                };
            }
        }

        public AnimationSet AnimationSet
        {
            get => _animationSet;
            set => SetProperty(ref _animationSet, value);
        }

        public int Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value, NameChanged);
        }

        public int Actor
        {
            get => _actor;
            set => SetProperty(ref _actor, value, NameChanged);
        }

        public string OldPath
        {
            get => _oldPath;
            set => SetProperty(ref _oldPath, value);
        }

        public List<string> FnisArgs
        {
            get => _fnisArgs;
            set => SetProperty(ref _fnisArgs, value);
        }

        public string Creature
        {
            get => _creature;
            set => SetProperty(ref _creature, value);
        }

        public bool Equals(Animation? other)
        {
            return other is not null && AnimationName.Equals(other.AnimationName);
        }

        public void NameChanged()
        {
            RaisePropertyChanged(nameof(AnimationName));
        }
    }
}
