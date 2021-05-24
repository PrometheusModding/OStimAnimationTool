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

        private Animation? tempAnim = null;
        private bool activeEdit = false;

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
            if (activeEdit is false)
            {
                tempAnim = this.MemberwiseClone() as Animation;
                activeEdit = true;
            }
        }

        public void CancelEdit()
        {
            if (activeEdit is true)
            {
                if (tempAnim is null)
                    throw new NullReferenceException();

                this.setName = tempAnim.setName;
                this.animationName = tempAnim.animationName;
                activeEdit = false;
            }
        }

        public void EndEdit()
        {
            if (activeEdit is true)
            {
                tempAnim = null;
                activeEdit = false;
            }
        }
    }

    public class AnimationList : List<Animation> { }

    public class AnimationDatabase : ObservableCollection<Animation>
    {
    }
}
