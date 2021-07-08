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
        private string _moduleName = Empty;
        private string _positionKey = Empty;
        private string _setName;

        public AnimationSet(string setName)
        {
            _setName = setName;
        }

        public string SceneID => _moduleName + $"|{_positionKey}" + $"|{_animationClass}" + $"|{_setName}";

        public string ModuleName
        {
            get => _moduleName;
            set { SetProperty(ref _moduleName, value, () =>
            {
                RaisePropertyChanged(nameof(SceneID));
                foreach (var animation in Animations)
                {
                    animation.NameChanged();
                }
            });
            }
        }

        public string PositionKey
        {
            get => _positionKey;
            set => SetProperty(ref _positionKey, value, () =>
            {
                RaisePropertyChanged(nameof(SceneID));
                foreach (var animation in Animations)
                {
                    animation.NameChanged();
                }
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
            set => SetProperty(ref _animator, value);
        }

        public string AnimationClass
        {
            get => _animationClass;
            set => SetProperty(ref _animationClass, value, () =>
            {
                RaisePropertyChanged(nameof(SceneID));
                foreach (var animation in Animations)
                {
                    animation.NameChanged();
                }
            });
        }

        public string SetName
        {
            get => _setName;
            set => SetProperty(ref _setName, value, () =>
            {
                RaisePropertyChanged(nameof(SceneID));
                foreach (var animation in Animations)
                {
                    animation.NameChanged();
                }
            });
        }

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
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
    }
}
