using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OStimConversionTool
{
    public class Animation : IEquatable<Animation>, IEditableObject, INotifyPropertyChanged
    {
        private string setName;
        private string animationName;
        private string animationClass;
        private bool isTransition = false;

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
                    NotifyPropertyChanged(nameof(SetName));
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
                    this.animationName = value;
                    NotifyPropertyChanged(nameof(AnimationName));
                }
            }
        }

        public string AnimationClass
        {
            get { return this.animationClass; }
            set
            {
                if (value != this.animationClass)
                {
                    this.animationClass = value;
                    NotifyPropertyChanged(nameof(AnimationClass));
                }
            }
        }

        public bool IsTransition
        {
            get { return this.isTransition; }
            set
            {
                if (value != this.isTransition)
                {
                    this.isTransition = value;
                    NotifyPropertyChanged(nameof(IsTransition));
                }
            }
        }

        public Animation(string setName, string animName, string animClass)
        {
            this.setName = setName;
            animationName = animName;
            animationClass = animClass;
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
