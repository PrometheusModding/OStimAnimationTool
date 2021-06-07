using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace OStimConversionTool
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AnimationClassList
    {
        [Description("Vaginal")] Sx,

        [Description("Anal")] An,

        [Description("Foreplay")] foreplay
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SubAnimationClassList
    {
        [Description("Blowjob")] BJ,

        [Description("Handjob")] HJ,

        [Description("Cuddling")] cuddling,

        [Description("Fingering")] fingering,

        [Description("Footjob")] FJ,

        [Description("Cunnilingus")] VJ,

        [Description("Boobjob")] BoJ,

        [Description("Breastfeeding")] BoF
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum BlowjobClassList
    {
        [Description("(none)")] BJ,

        [Description("Head Held Blowjob")] HhBJ,

        [Description("Penisjob (Blowjob with Jerking)")]
        PJ,

        [Description("Head Held Penisjob")] HhPJ,

        [Description("Self")] SJ,

        [Description("69 with Blowjob")] VBJ
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum HandjobClassList
    {
        [Description("(none)")] HJ,

        [Description("Masturbate")] Po,

        [Description("Head Held Masturbate")] HhPo,

        [Description("Apart Handjob")] ApHJ,

        [Description("Dual Handjob")] DHJ,

        [Description("69 with Handjob")] VHJ
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CuddlingClassList
    {
        [Description("Standing Apart")] Ap,

        [Description("Standing Apart Undressing")]
        ApU,

        [Description("Embracing")] Em,

        [Description("Holding")] Ho,

        [Description("Rough Holding")] Ro
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum FingeringClassList
    {
        [Description("Rubbing Clit")] Cr,

        [Description("1 Finger")] Pf1,

        [Description("2 Fingers")] Pf2
    }

    public class Animation : IEquatable<Animation>, IEditableObject, INotifyPropertyChanged
    {
        private bool _activeEdit;
        private string _animationClass = string.Empty;
        private string _animationInfo = string.Empty;
        private string _animationName;
        private string _animator;
        private bool _isTransition;
        private string _setName;

        private Animation? _tempAnim;

        public Animation(string setName, string animName, string animClass, string animator)
        {
            _setName = setName;
            _animationName = animName;
            _animationClass = animClass;
            _animator = animator;
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

        public int GetSetSize(AnimationDatabase animationDatabase)
        {
            var count = animationDatabase.Count(anim => anim.SetName.Equals(_setName));

            return count / 2;
        }
    }

    public class AnimationDatabase : ObservableCollection<Animation>
    {
    }
}
