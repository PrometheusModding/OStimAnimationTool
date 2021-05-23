using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OStimConversionTool
{
    public class Animation : IEditableObject, INotifyPropertyChanged
    {
        private string setName;
        private string animationName;

        public string SetName
        {
            get { return this.setName; }
            set
            {
                if (value != this.setName)
                {
                    this.setName = value;
                    NotifyPropertyChanged("AnimationSet");
                }
            }
        }

        public string AnimationName
        {
            get { return this.animationName; }
            set
            {
                if (value != this.animationName)
                {
                    this.setName = value;
                    NotifyPropertyChanged("AnimationName");
                }
            }
        }

        public Animation(string animName, string setName)
        {
            this.setName = setName;
            animationName = animName;
        }

        public bool Equals(Animation? other)
        {
            if (other is null)
                throw new NullReferenceException();

            if (animationName.Equals(other.animationName))
                return true;

            return false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void BeginEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            throw new NotImplementedException();
        }
    }

    public class AnimationList : List<Animation> { }

    public class AnimationDatabase : ObservableCollection<Animation>
    {
    }
}
