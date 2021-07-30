using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Prism.Mvvm;
using static System.String;
using static System.Text.Json.JsonSerializer;

namespace OStimAnimationTool.Core.Models
{
    public class AnimationSet : BindableBase, IEquatable<AnimationSet>
    {
        private Module _module = new("");
        private string _animationClass = Empty;
        private string _positionKey = Empty;
        private string _setName = Empty;
        private ObservableCollection<Animation> _animations = new();
        private string _animator = Empty;
        private string _description = Empty;
        private bool _is0SexAnimation;

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

    public class AnimationSetConverter : JsonConverter<AnimationSet>
    {
        private enum TypeDiscriminator
        {
            Hub = 1,
            Transition = 2
        }
        
        public override bool CanConvert(Type typeToConvert) => typeof(AnimationSet).IsAssignableFrom(typeToConvert);

        public override AnimationSet Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            if (propertyName != "TypeDiscriminator")
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            var typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
            AnimationSet animationSet = typeDiscriminator switch
            {
                TypeDiscriminator.Hub => new HubAnimationSet(),
                TypeDiscriminator.Transition => new TransitionAnimationSet(),
                _ => throw new JsonException()
            };
            reader.

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndObject:
                        return animationSet;
                    case JsonTokenType.PropertyName:
                        propertyName = reader.GetString();
                        reader.Read();
                        switch (propertyName)
                        {
                            case "Destinations":
                                var destinations = Deserialize<ObservableCollection<AnimationSet>>(reader.GetString() ?? throw new JsonException());
                                if (destinations != null) ((HubAnimationSet) animationSet).Destinations = destinations;
                                break;
                            case "Destination":
                                var destination = Read(ref reader, typeof(AnimationSet), options);
                                ((TransitionAnimationSet) animationSet).Destination = destination;
                                break;
                            case "Module":
                                var module = Deserialize<Module>(reader.GetString() ?? throw new JsonException());
                                if (module != null) animationSet.Module = module;
                                break;
                            case "PositionKey":
                                var positionKey = reader.GetString();
                                if (positionKey != null) animationSet.PositionKey = positionKey;
                                break;
                            case "AnimationClass":
                                var animationClass = reader.GetString();
                                if (animationClass != null) animationSet.AnimationClass = animationClass;
                                break;
                            case "Name":
                                var name = reader.GetString();
                                if (name != null) animationSet.SetName = name;
                                break;
                            case "Animations":
                                var animations = Deserialize<ObservableCollection<Animation>>(reader.GetString() ?? throw new JsonException());
                                if (animations != null) animationSet.Animations = animations;
                                break;
                            case "Animator":
                                var animator = reader.GetString();
                                if (animator != null) animationSet.Animator = animator;
                                break;
                            case "Description":
                                var description = reader.GetString();
                                if (description != null) animationSet.Description = description;
                                break;
                            case "Is0SexAnimation":
                                var is0SexAnimation = reader.GetBoolean();
                                animationSet.Is0SexAnimation = is0SexAnimation;
                                break;
                            case "ChangedThisSession":
                                break;
                        }

                        break;
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, AnimationSet animationSet, JsonSerializerOptions options)
        {
            switch (animationSet)
            {
                case HubAnimationSet hubAnimationSet:
                    Serialize(writer, hubAnimationSet, DatabaseScriber.Options);
                    break;
                
                case TransitionAnimationSet transitionAnimationSet:
                    Serialize(writer, transitionAnimationSet, animationSet.GetType(), DatabaseScriber.Options);
                    break;
            }
        }
    }
}
