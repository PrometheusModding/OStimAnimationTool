using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

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

        public string SafePath { get; set; } = string.Empty;
    }
}
