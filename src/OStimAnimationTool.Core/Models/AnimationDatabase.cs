using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Prism.Mvvm;

namespace OStimAnimationTool.Core.Models
{
    public sealed class AnimationDatabase : BindableBase
    {
        private static readonly Lazy<AnimationDatabase> Lazy = new(() => new AnimationDatabase());
        private List<string> _misc = new();
        private ObservableCollection<Module> _modules = new();
        private string _name = "New Animation Database";
        
        public static AnimationDatabase Instance => Lazy.Value;

        public static bool IsValueCreated => Lazy.IsValueCreated;

        public ObservableCollection<Module> Modules
        {
            get => _modules;
            set => SetProperty(ref _modules, value);
        }

        public List<string> Misc
        {
            get => _misc;
            set => SetProperty(ref _misc, value);
        }

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string SafePath { get; set; } = string.Empty;

        public bool Contains(AnimationSet animationSet)
        {
            return Modules.Any(module => module.AnimationSets.Contains(animationSet));
        }
    }
    
    public class DatabaseConverter : JsonConverter<AnimationDatabase>
    {
        public override AnimationDatabase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            
        }

        public override void Write(Utf8JsonWriter writer, AnimationDatabase database, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            
            writer.WriteString("Name", database.Name);
            JsonSerializer.Serialize(writer, database.Modules);
            
            writer.WriteEndObject();
        }
    }
}
