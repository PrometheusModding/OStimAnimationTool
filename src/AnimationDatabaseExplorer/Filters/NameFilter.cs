using System.Collections.ObjectModel;
using OStimAnimationTool.Core.Interfaces;
using OStimAnimationTool.Core.Models;
using static System.String;

namespace AnimationDatabaseExplorer.Filters
{
    public class NameFilter : IDatabaseFilter
    {
        public NameFilter(string sceneType)
        {
            FilterParameter = sceneType;
        }

        public string FilterParameter { get; set; }

        public ObservableCollection<Module> Apply(ObservableCollection<Module> modules)
        {
            if (IsNullOrEmpty(FilterParameter))
            {
                return modules;
            }

            var tempModules = new ObservableCollection<Module>();
            Module? tempModule = null;

            foreach (var module in modules)
            foreach (var animationSet in module.AnimationSets)
            {
                if (!animationSet.SetName.ToLower().Contains(FilterParameter.Trim().ToLower())) continue;

                if (!tempModules.Contains(module))
                {
                    tempModule = new Module(module.Name)
                    {
                        Creatures = module.Creatures,
                        AnimationSets = new ObservableCollection<AnimationSet>()
                    };

                    tempModules.Add(tempModule);
                }

                tempModule?.AnimationSets.Add(animationSet);
            }

            return tempModules;
        }
    }
}
