using System;
using System.ComponentModel;

namespace AnimationDatabaseExplorer.Models
{
    public class Animation : IEquatable<Animation>, IEditableObject, INotifyPropertyChanged
    {
        private bool _activeEdit;
        private string _animationClass;
        private string _animationInfo = string.Empty;
        private string _animationName;
        private string _animator;
        private bool _isTransition;
        private string _setName;

        private Animation? _tempAnim;

        public Animation(string animName)
        {
            _animationName = animName;
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

        public string AnimationName
        {
            get => _animationName;
            set
            {
                if (value == _animationName) return;
                _animationName = value;
                NotifyPropertyChanged(nameof(AnimationName));
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

        public string AnimationInfo
        {
            get => _animationInfo;
            set
            {
                if (value == _animationInfo) return;
                _animationInfo = value;
                NotifyPropertyChanged(nameof(AnimationInfo));
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

        public void BeginEdit()
        {
            if (_activeEdit) return;
            _tempAnim = MemberwiseClone() as Animation;
            _activeEdit = true;
        }

        public void CancelEdit()
        {
            if (_activeEdit != true) return;
            if (_tempAnim is null)
                throw new NullReferenceException();

            _setName = _tempAnim._setName;
            _animationName = _tempAnim._animationName;
            _activeEdit = false;
        }

        public void EndEdit()
        {
            if (_activeEdit != true) return;
            _tempAnim = null;
            _activeEdit = false;
        }

        public bool Equals(Animation? other)
        {
            if (other is null)
                throw new NullReferenceException();

            return _animationName.Equals(other._animationName);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /*public int GetSetSize(AnimationDatabase animationDatabase)
        {
            var count = animationDatabase.Count(anim => anim.SetName.Equals(_setName));

            return count / 2;
        }*/
    }
}
