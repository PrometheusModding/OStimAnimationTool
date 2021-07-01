#region

using System;
using System.Collections.Specialized;
using System.Windows;
using OStimAnimationTool.Core.Interfaces;
using Prism.Regions;

#endregion

namespace OStimAnimationTool.Core.Behaviors
{
    public class RegionManagerAwareBehavior : RegionBehavior
    {
        public const string BehaviorKey = "RegionManagerAwareBehavior";

        protected override void OnAttach()
        {
            Region.Views.CollectionChanged += Views_CollectionChanged;
        }

        private void Views_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems != null)
                        foreach (var item in e.NewItems)
                        {
                            var regionManager = Region.RegionManager;

                            if (item is FrameworkElement element)
                                if (element.GetValue(RegionManager.RegionManagerProperty) is IRegionManager
                                    scopedRegionManager)
                                    regionManager = scopedRegionManager;

                            InvokeOnRegionManagerAwareElement(item, x => x.RegionManager = regionManager);
                        }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems != null)
                        foreach (var item in e.OldItems)
                            InvokeOnRegionManagerAwareElement(item, x => x.RegionManager = null);
                    break;
                }
            }
        }

        private static void InvokeOnRegionManagerAwareElement(object item, Action<IRegionManagerAware> invocation)
        {
            switch (item)
            {
                case IRegionManagerAware regionManagerAwareItem:
                    invocation(regionManagerAwareItem);
                    break;
                case FrameworkElement {DataContext: IRegionManagerAware regionManagerAwareDataContext} frameworkElement:
                {
                    if (frameworkElement.Parent is FrameworkElement
                    {
                        DataContext: IRegionManagerAware
                        regionManagerAwareDataContextParent
                    })
                        if (regionManagerAwareDataContext == regionManagerAwareDataContextParent)
                            return;

                    invocation(regionManagerAwareDataContext);
                    break;
                }
            }
        }
    }
}
