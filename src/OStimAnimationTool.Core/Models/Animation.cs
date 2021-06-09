using System;
using System.ComponentModel;

namespace AnimationDatabaseExplorer.Models
{
    public class Animation : IEquatable<Animation>, IEditableObject, INotifyPropertyChanged
    {
        private bool _activeEdit;
        private string _animationName;
        private bool _isTransition;

        private Animation? _tempAnim;

        public Animation(string animName)
        {
            _animationName = animName;
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
