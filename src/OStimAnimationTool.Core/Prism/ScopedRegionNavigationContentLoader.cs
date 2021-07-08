#region

using OStimAnimationTool.Core.Interfaces;
using Prism.Ioc;
using Prism.Regions;

#endregion

namespace OStimAnimationTool.Core.Prism
{
    public class ScopedRegionNavigationContentLoader : RegionNavigationContentLoader
    {
        public ScopedRegionNavigationContentLoader(IContainerExtension container) : base(container)
        {
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
