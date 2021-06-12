#region

using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Prism.Regions;

#endregion

namespace OStimAnimationTool.Core.Regions
{
    public class DockPanelRegionAdapter : RegionAdapterBase<DockPanel>
    {
        public DockPanelRegionAdapter(RegionBehaviorFactory behaviorFactory)
            : base(behaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, DockPanel regionTarget)
        {
            region.Views.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems != null)
                            foreach (FrameworkElement item in e.NewItems)
                                regionTarget.Children.Add(item);

                        break;
                    }
                    case NotifyCollectionChangedAction.Remove:
                    {
                        if (e.OldItems != null)
                            foreach (FrameworkElement item in e.OldItems)
                                regionTarget.Children.Remove(item);

                        break;
                    }
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new Region();
        }
    }
}
