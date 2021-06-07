using Prism.Regions;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace OStimAnimationTool.Core.Regions
{
    public class StackPanelRegionAdapter : RegionAdapterBase<StackPanel>
    {
        public StackPanelRegionAdapter(RegionBehaviorFactory behaviorFactory)
            : base(behaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, StackPanel regionTarget)
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
