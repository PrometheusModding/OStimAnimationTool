#region

using System;
using System.ComponentModel;

#endregion

namespace OStimAnimationTool.Core.Models
{
    public class Animation : IEquatable<Animation>, IEditableObject, INotifyPropertyChanged
    {
        private bool _activeEdit;
        private string _animationName;
        private bool _isTransition;
        private string _oldPath = string.Empty;

        private Animation? _tempAnim;

        public Animation(string animationName)
        {
            _animationName = animationName;
        }

        public Animation(string animationName, string oldPath)
        {
            _animationName = animationName;
            _oldPath = oldPath;
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

        public string OldPath
        {
            get => _oldPath;
            set
            {
                if (value == _oldPath) return;
                _oldPath = value;
                NotifyPropertyChanged(nameof(OldPath));
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

            _animationName = _tempAnim._animationName;
            _activeEdit = false;
        }

        public void EndEdit()
        {
            if (_activeEdit != true) return;
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
