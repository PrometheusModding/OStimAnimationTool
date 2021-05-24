using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OStimConversionTool
{
    public class Animation : IEquatable<Animation>, IEditableObject, INotifyPropertyChanged
    {
        private string _setName;
        private string _animationName;
        private string _animationClass;
        private bool _isTransition;

        private Animation? _tempAnim;
        private bool _activeEdit;

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

        public Animation(string setName, string animName, string animClass)
        {
            _setName = setName;
            _animationName = animName;
            _animationClass = animClass;
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

        public void BeginEdit()
        {
            if (_activeEdit is not false) return;
            _tempAnim = MemberwiseClone() as Animation;
            _activeEdit = true;
        }

        public void CancelEdit()
        {
            if (_activeEdit is not true) return;
            if (_tempAnim is null)
                throw new NullReferenceException();

            _setName = _tempAnim._setName;
            _animationName = _tempAnim._animationName;
            _activeEdit = false;
        }

        public void EndEdit()
        {
            if (_activeEdit is not true) return;
            _tempAnim = null;
            _activeEdit = false;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Animation);
        }

        public override int GetHashCode()
        {
            return _animationName.GetHashCode();
        }
    }

    public class AnimationList : List<Animation> { }

    public class AnimationDatabase : ObservableCollection<Animation>
    {
    }
}
