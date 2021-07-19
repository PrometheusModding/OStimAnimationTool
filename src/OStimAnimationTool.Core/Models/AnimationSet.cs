using System;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Mvvm;
using static System.String;

namespace OStimAnimationTool.Core.Models
{
    public class AnimationSet : BindableBase, IEquatable<AnimationSet>
    {
        private string _animationClass = Empty;
        private ObservableCollection<Animation> _animations = new();
        private string _animator = Empty;
        private string _description = Empty;
        private Module _module = new("0MF");
        private string _positionKey = Empty;
        private string _setName;

        public AnimationSet(string setName)
        {
            _setName = setName;
        }

        public string SceneID => _module?.Name + $"|{_positionKey}" + $"|{_animationClass}" + $"|{_setName}";

        public bool ChangedThisSession { get; set; }

        public string PositionKey
        {
            get => _positionKey;
            set => SetProperty(ref _positionKey, value, () =>
            {
                RaisePropertyChanged(nameof(SceneID));
                foreach (var animation in Animations) animation.NameChanged();
                ChangedThisSession = true;
            });
        }

        public ObservableCollection<Animation> Animations
        {
            get => _animations;
            set => SetProperty(ref _animations, value);
        }

        public Module Module
        {
            get => _module;
            set => SetProperty(ref _module, value, () => ChangedThisSession = true);
        }

        public string Animator
        {
            get => _animator;
            set => SetProperty(ref _animator, value, () => ChangedThisSession = true);
        }

        public string AnimationClass
        {
            get => _animationClass;
            set => SetProperty(ref _animationClass, value, () =>
            {
                RaisePropertyChanged(nameof(SceneID));
                foreach (var animation in Animations) animation.NameChanged();
                ChangedThisSession = true;
            });
        }

        public string SetName
        {
            get => _setName;
            set => SetProperty(ref _setName, value, () =>
            {
                RaisePropertyChanged(nameof(SceneID));
                foreach (var animation in Animations) animation.NameChanged();
                ChangedThisSession = true;
            });
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value, () => ChangedThisSession = true);
        }

        public int Actors => GetActorCount();

        public bool Equals(AnimationSet? other)
        {
            if (other is null)
                throw new NullReferenceException();

            return SetName.Equals(other.SetName);
        }

        private int GetActorCount()
        {
            return Animations.Select(animation => animation.Actor).Prepend(1).Max() + 1;
        }

        public void NameChanged()
        {
            RaisePropertyChanged(nameof(SceneID));
        }
    }
}
