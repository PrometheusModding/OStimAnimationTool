using Prism.Ioc;
using Prism.Regions;

namespace OStimAnimationTool.Core.Prism
{
    public class ScopedRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        private readonly IContainerExtension _container;

        public ScopedRegionNavigationContentLoader(IContainerExtension container) : base(container)
        {
            _container = container;
        }

        protected override void AddViewToRegion(IRegion region, object view)
        {
            region.Add(view, null, CreateRegionManagerScope(view));
        }

        private static bool CreateRegionManagerScope(object view)
        {
            var createRegionManagerScope = false;

            if (view is ICreateRegionManagerScope viewHasScopedRegions)
                createRegionManagerScope = viewHasScopedRegions.CreateRegionManagerScope;

            return createRegionManagerScope;
        }
    }
}
