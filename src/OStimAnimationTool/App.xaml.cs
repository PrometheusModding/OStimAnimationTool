using System.Windows;
using Fluent;
using NodeNetwork;
using OStimAnimationTool.Core.Behaviors;
using OStimAnimationTool.Core.Controls;
using OStimAnimationTool.Core.Prism;
using OStimAnimationTool.Core.Regions;
using OStimConversionTool.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace OStimConversionTool
{
    public partial class App
    {
        protected override Window CreateShell()
        {
            NNViewRegistrar.RegisterSplat();
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, ScopedRegionNavigationContentLoader>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping<Ribbon>(Container.Resolve<FluentRibbonRegionAdapter>());
        }

        protected override void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory regionBehaviors)
        {
            base.ConfigureDefaultRegionBehaviors(regionBehaviors);
            regionBehaviors.AddIfMissing(RegionManagerAwareBehavior.BehaviorKey, typeof(RegionManagerAwareBehavior));
            regionBehaviors.AddIfMissing<DependentViewRegionBehavior>(DependentViewRegionBehavior.BehaviorKey);
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }
    }
}
