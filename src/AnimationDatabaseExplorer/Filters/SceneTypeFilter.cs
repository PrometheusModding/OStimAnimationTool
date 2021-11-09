using System.Collections.ObjectModel;
using OStimAnimationTool.Core.Interfaces;
using OStimAnimationTool.Core.Models;

namespace AnimationDatabaseExplorer.Filters
{
    public class SceneTypeFilter : IDatabaseFilter
    {
        public SceneTypeFilter(string sceneType)
        {
            FilterParameter = sceneType;
        }

        public string FilterParameter { get; set; }

        public ObservableCollection<Module> Apply(ObservableCollection<Module> modules)
        {
            if (FilterParameter == "All")
            {
                return modules;
            }

            var tempModules = new ObservableCollection<Module>();
            Module? tempModule = null;

            foreach (var module in modules)
            foreach (var animationSet in module.AnimationSets)
            {
                switch (FilterParameter)
                {
                    case "Hub":
                        if (animationSet is not HubAnimationSet) continue;
                        break;
                    case "Transition":
                        if (animationSet is not TransitionAnimationSet) continue;
                        break;
                }

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
