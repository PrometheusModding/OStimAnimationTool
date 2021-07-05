#region

using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using Prism.Mvvm;

#endregion

namespace OStimAnimationTool.Core.Models
{
    public class Animation :  BindableBase, IEquatable<Animation>
    {
        private int _actor;
        private string _animationName;
        private string _oldPath = string.Empty;
        private int _speed;
        private AnimationSet? _animationSet;

        public Animation(string animationName, string oldPath, AnimationSet animationSet)
        {
            _animationName = animationName;
            _oldPath = oldPath;
            _animationSet = animationSet;
        }

        public string AnimationName
        {
            get
            {
                return _animationSet switch
                {
                    HubAnimationSet => "0Sx" + _animationSet.ModuleName + $"_{_animationSet.AnimationClass}" +
                                       $"-{_animationSet.SetName}" + $"_S{_speed.ToString()}" + $"_{_actor.ToString()}",
                    TransitionAnimationSet => "0Sx" + _animationSet.ModuleName + $"_{_animationSet.AnimationClass}" +
                                              $"-{_animationSet.SetName}" + $"_{_actor.ToString()}",
                    _ => string.Empty
                };
            }
        }

        public int Actor
        {
            get => _actor;
            set => SetProperty(ref _actor, value);
        }

        public string OldPath
        {
            get => _oldPath;
            set => SetProperty(ref _oldPath, value);
        }

        public int Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value);
        }
        public bool Equals(Animation? other)
        {
            if (other is null)
                throw new NullReferenceException();

            return _animationName.Equals(other._animationName);
        }
    }
}
