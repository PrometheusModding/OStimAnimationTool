#region

using System.Windows;
using System.Windows.Controls;
using Fluent;
using NodeNetwork;
using OStimAnimationTool.Core.Behaviors;
using OStimAnimationTool.Core.Commands;
using OStimAnimationTool.Core.Prism;
using OStimAnimationTool.Core.Regions;
using OStimConversionTool.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

#endregion

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
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            regionAdapterMappings.RegisterMapping<DockPanel>(Container.Resolve<DockPanelRegionAdapter>());
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
