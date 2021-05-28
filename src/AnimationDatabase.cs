using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace OStimConversionTool
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum AnimationClassList
    {
        [Description("Vaginal")]
        Sx,

        [Description("Anal")]
        An,

        [Description("Foreplay")]
        foreplay
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum SubAnimationClassList
    {
        [Description("Blowjob")]
        BJ,

        [Description("Handjob")]
        HJ,

        [Description("Cuddling")]
        cuddling,

        [Description("Fingering")]
        fingering,

        [Description("Footjob")]
        FJ,

        [Description("Cunnilingus")]
        VJ,

        [Description("Boobjob")]
        BoJ,

        [Description("Breastfeeding")]
        BoF
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum BlowjobClassList
    {
        [Description("(none)")]
        BJ,

        [Description("Head Held Blowjob")]
        HhBJ,

        [Description("Penisjob (Blowjob with Jerking)")]
        PJ,

        [Description("Head Held Penisjob")]
        HhPJ,

        [Description("Self")]
        SJ
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum HandjobClassList
    {
        [Description("(none)")]
        HJ,

        [Description("Masturbate")]
        Po,

        [Description("Head Held Masturbate")]
        HhPo,

        [Description("Apart Handjob")]
        ApHJ,

        [Description("Dual Handjob")]
        DHJ
    }

    public class Animation : IEquatable<Animation>, IEditableObject, INotifyPropertyChanged
    {
        private string _setName;
        private string _animationName;
        private string _animationClass = string.Empty;
        private string _animator;
        private string _animationInfo = string.Empty;
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

        public Animation(string setName, string animName, string animator)
        {
            _setName = setName;
            _animationName = animName;
            _animator = animator;
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
