using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Prism.Mvvm;

namespace OStimAnimationTool.Core.Models
{
    public class Module : BindableBase, IEquatable<Module>
    {
        private ObservableCollection<AnimationSet> _animationSets = new();
        private string _name = string.Empty;

        public Module()
        {
        }

        public Module(string name)
        {
            _name = name;
        }

        public List<string> Creatures { get; init; } = new();

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, () =>
            {
                foreach (var animationSet in AnimationSets) animationSet.SceneIdChanged();
            });
        }

        public ObservableCollection<AnimationSet> AnimationSets
        {
            get => _animationSets;
            set => SetProperty(ref _animationSets, value);
        }

        public bool Equals(Module? other)
        {
            return Name == other?.Name;
        }
    }

    class ModuleConverter :JsonConverter<Module>
    {
        public override Module? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Module module, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            
            writer.WriteString("Name", module.Name);
            JsonSerializer.Serialize(writer, module.AnimationSets);
            
            writer.WriteEndObject();
        }
    }
}
