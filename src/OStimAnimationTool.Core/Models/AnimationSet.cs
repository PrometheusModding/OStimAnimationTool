#region

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using OStimAnimationTool.Core.Models;
using static System.String;

#endregion

namespace AnimationDatabaseExplorer.Models
{
    public class AnimationSet : ObservableCollection<Animation>, IEquatable<AnimationSet>, INotifyPropertyChanged
    {
        private string _animationClass = Empty;
        private string _animator = Empty;
        private string _description = Empty;
        private bool _isTransition;
        private string _setName;
        private string _transitionDestination = Empty;

        public AnimationSet(string setName)
        {
            _setName = setName;
        }

        public string Animator
        {
            get => _animator;
            set
            {
                if (value == _animator) return;
                _animator = value;
                NotifyPropertyChanged(nameof(Animator));
            }
        }

        public string TransitionDestination
        {
            get => _transitionDestination;
            set
            {
                if (value == _transitionDestination) return;
                _transitionDestination = value;
                NotifyPropertyChanged(nameof(TransitionDestination));
            }
        }

        public bool IsTransition
        {
            get => _isTransition;
            set
            {
                if (value == _isTransition) return;
                _isTransition = value;
                NotifyPropertyChanged(nameof(IsTransition));
            }
        }

        public string AnimationClass
        {
            get => _animationClass;
            set
            {
                if (value == _animationClass) return;
                _animationClass = value;
                NotifyPropertyChanged(nameof(AnimationClass));
            }
        }

        public string SetName
        {
            get => _setName;
            set
            {
                if (value == _setName) return;
                _setName = value;
                NotifyPropertyChanged(nameof(SetName));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (value == _description) return;
                _description = value;
                NotifyPropertyChanged(nameof(Description));
            }
        }

        public int ActorCount => GetActorCount();

        public bool Equals(AnimationSet? other)
        {
            if (other is null)
                throw new NullReferenceException();

            return _setName.Equals(other._setName);
        }

        public new event PropertyChangedEventHandler? PropertyChanged;

        private int GetActorCount()
        {
            var actorCount = 0;
            foreach (var animation in this)
                if (char.GetNumericValue(animation.AnimationName[^1]) > actorCount)
                    actorCount = (int) char.GetNumericValue(animation.AnimationName[^1]);
            return actorCount;
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
