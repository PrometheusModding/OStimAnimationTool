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
        private bool _is0SexAnimation;
        private Module _module = new("");
        private string _positionKey = Empty;
        private string _setName = Empty;

        public AnimationSet()
        {
        }

        public AnimationSet(string setName)
        {
            _setName = setName;
        }

        protected AnimationSet(Module module, string positionKey, string animationClass, string setName)
        {
            _module = module;
            _positionKey = positionKey;
            _animationClass = animationClass;
            _setName = setName;
        }

        public string SceneId => _module.Name + $"|{_positionKey}" + $"|{_animationClass}" + $"|{_setName}";

        public Module Module
        {
            get => _module;
            set => SetProperty(ref _module, value, () =>
            {
                RaisePropertyChanged(nameof(SceneId));
                ChangedThisSession = true;
            });
        }

        public string PositionKey
        {
            get => _positionKey;
            set => SetProperty(ref _positionKey, value, () =>
            {
                RaisePropertyChanged(nameof(SceneId));
                ChangedThisSession = true;
            });
        }

        public string AnimationClass
        {
            get => _animationClass;
            set => SetProperty(ref _animationClass, value, () =>
            {
                RaisePropertyChanged(nameof(SceneId));
                foreach (var animation in Animations) animation.NameChanged();
                ChangedThisSession = true;
            });
        }

        public string SetName
        {
            get => _setName;
            set => SetProperty(ref _setName, value, () =>
            {
                RaisePropertyChanged(nameof(SceneId));
                foreach (var animation in Animations) animation.NameChanged();
                ChangedThisSession = true;
            });
        }

        public ObservableCollection<Animation> Animations
        {
            get => _animations;
            set => SetProperty(ref _animations, value);
        }

        public string Animator
        {
            get => _animator;
            set => SetProperty(ref _animator, value, () => ChangedThisSession = true);
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value, () => ChangedThisSession = true);
        }

        public bool Is0SexAnimation
        {
            get => _is0SexAnimation;
            set => SetProperty(ref _is0SexAnimation, value);
        }

        // Variable to determine if this AnimationSet Changed this Session.
        // Gets set if certain Properties change.
        public bool ChangedThisSession { get; private set; }

        // Returns the Amount of Actors in the AnimationSet based on the Animations in this Set.
        public int Actors => Animations.Select(animation => animation.Actor).Prepend(1).Max() + 1;

        public bool Equals(AnimationSet? other)
        {
            return SetName.Equals(other?.SceneId);
        }

        // Method to raise PropertyChanged Notifications for SceneId from within Module
        public void SceneIdChanged()
        {
            RaisePropertyChanged(nameof(SceneId));
        }
    }
}
