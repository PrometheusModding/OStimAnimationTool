using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace AnimationDatabaseExplorer.Models
{
    public class AnimationSet : ObservableCollection<Animation>, IEquatable<AnimationSet>, INotifyPropertyChanged
    {
        private string _animationClass = string.Empty;
        private string _setName;

        public AnimationSet(string setName)
        {
            _setName = setName;
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

        public bool Equals(AnimationSet? other)
        {
            if (other is null)
                throw new NullReferenceException();

            return _setName.Equals(other._setName);
        }
        
        public new event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
