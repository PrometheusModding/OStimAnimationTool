using System;
using System.Collections.ObjectModel;

namespace AnimationDatabaseExplorer.Models
{
    public class AnimationSet : ObservableCollection<Animation>, IEquatable<AnimationSet>
    {
        private string _setName;

        public AnimationSet(string setName)
        {
            _setName = setName;
        }

        public string SetName
        {
            get => _setName;
            set
            {
                if (value == _setName) return;
                _setName = value;
            }
        }

        public bool Equals(AnimationSet? other)
        {
            if (other is null)
                throw new NullReferenceException();

            return _setName.Equals(other._setName);
        }
    }
}
