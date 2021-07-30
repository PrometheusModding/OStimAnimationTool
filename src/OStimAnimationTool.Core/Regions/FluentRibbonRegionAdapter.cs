using System.Collections.Specialized;
using Fluent;
using Prism.Regions;

namespace OStimAnimationTool.Core.Regions
{
    public class FluentRibbonRegionAdapter : RegionAdapterBase<Ribbon>
    {
        public FluentRibbonRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, Ribbon regionTarget)
        {
            region.Views.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems != null)
                            foreach (var view in e.NewItems)
                                AddViewToRegion(view, regionTarget);

                        break;
                    }
                    case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldItems != null)
                            foreach (var view in e.OldItems)
                                RemoveViewFromRegion(view, regionTarget);

                        break;
                    }
                }
            };
        }

        private static void RemoveViewFromRegion(object view, Ribbon regionTarget)
        {
            if (view is RibbonTabItem ribbonTabItem)
                regionTarget.Tabs.Add(ribbonTabItem);
        }

        private static void AddViewToRegion(object view, Ribbon regionTarget)
        {
            if (view is RibbonTabItem ribbonTabItem)
                regionTarget.Tabs.Add(ribbonTabItem);
        }


        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
