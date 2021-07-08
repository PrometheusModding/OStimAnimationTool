#region

using System;
using Prism.Mvvm;

#endregion

namespace OStimAnimationTool.Core.Models
{
    public class Animation : BindableBase, IEquatable<Animation>
    {
        private int _actor;
        private AnimationSet _animationSet;
        private string _oldPath;
        private int _speed;
        
        public Animation(string oldPath, AnimationSet animationSet)
        {
            _oldPath = oldPath;
            _animationSet = animationSet;
        }

        public string AnimationName
        {
            get
            {
                return AnimationSet switch
                {
                    HubAnimationSet => "0Sx" + _animationSet.ModuleName + $"_{_animationSet.AnimationClass}" +
                                       $"-{_animationSet.SetName}" + $"_S{_speed.ToString()}" + $"_{_actor.ToString()}",
                    TransitionAnimationSet => "0Sx" + _animationSet.ModuleName + $"_{_animationSet.AnimationClass}" +
                                              $"-{_animationSet.SetName}" + $"_{_actor.ToString()}",
                    _ => string.Empty
                };
            }
        }

        public void NameChanged()
        {
            RaisePropertyChanged(nameof(AnimationName));
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

        public int Speed
        {
            get => _speed;
            set => SetProperty(ref _speed, value, NameChanged);
        }

        public AnimationSet AnimationSet
        {
            get => _animationSet;
            set => SetProperty(ref _animationSet, value);
        }

        public bool Equals(Animation? other)
        {
            if (other is null)
                throw new NullReferenceException();

            return AnimationName.Equals(other.AnimationName);
        }
    }
}
