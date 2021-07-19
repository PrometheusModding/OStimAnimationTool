#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using OStimAnimationTool.Core.Interfaces;
using Prism.Regions;

#endregion

namespace OStimAnimationTool.Core.Behaviors
{
    public class DependentViewRegionBehavior : RegionBehavior
    {
        public const string BehaviorKey = "DependentViewRegionBehavior";
        private readonly Dictionary<object, List<DependentViewInfo>> _dependentViewCache = new();

        protected override void OnAttach()
        {
            Region.ActiveViews.CollectionChanged += Views_CollectionChanged;
        }

        private void Views_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems != null)
                        foreach (var view in e.NewItems)
                        {
                            var viewList = new List<DependentViewInfo>();

                            if (_dependentViewCache.ContainsKey(view))
                            {
                                viewList = _dependentViewCache[view];
                            }
                            else
                            {
                                foreach (var atr in GetCustomAttributes<DependentViewAttribute>(view.GetType()))
                                {
                                    var info = CreateDependentView(atr);

                                    if (info.View is ISupportDataContext infoDataContext &&
                                        view is ISupportDataContext viewDataContext)
                                        infoDataContext.DataContext =
                                            viewDataContext.DataContext;

                                    viewList.Add(info);
                                }

                                if (!_dependentViewCache.ContainsKey(view))
                                    _dependentViewCache.Add(view, viewList);
                            }

                            viewList.ForEach(x => Region.RegionManager.Regions[x.TargetRegionName].Add(x.View));
                        }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems != null)
                        foreach (var oldView in e.OldItems)
                        {
                            if (_dependentViewCache.ContainsKey(oldView))
                                _dependentViewCache[oldView].ForEach(x =>
                                    Region.RegionManager.Regions[x.TargetRegionName].Remove(x.View));
                            if (!ShouldKeepAlive(oldView))
                                _dependentViewCache.Remove(oldView);
                        }

                    break;
                }
            }
        }

        private static bool ShouldKeepAlive(object oldView)
        {
            var lifetime = GetItemOrContextLifeTime(oldView);
            if (lifetime != null)
                return lifetime.KeepAlive;

            var lifetimeAttribute = GetItemOrContextLifeTimeAttribute(oldView);
            return lifetimeAttribute == null || lifetimeAttribute.KeepAlive;
        }

        private static RegionMemberLifetimeAttribute? GetItemOrContextLifeTimeAttribute(object oldView)
        {
            var lifeAttribute = GetCustomAttributes<RegionMemberLifetimeAttribute>(oldView.GetType()).FirstOrDefault();
            if (lifeAttribute != null)
                return lifeAttribute;

            if (oldView is FrameworkElement {DataContext: { }} frameworkElement)
            {
                var dataContext = frameworkElement.DataContext;
                var contextLifeTimeAttribute = GetCustomAttributes<RegionMemberLifetimeAttribute>(dataContext.GetType())
                    .FirstOrDefault();
                return contextLifeTimeAttribute;
            }

            return null;
        }

        private static IRegionMemberLifetime? GetItemOrContextLifeTime(object oldView)
        {
            if (oldView is IRegionMemberLifetime regionLifeTime)
                return regionLifeTime;

            if (oldView is FrameworkElement framework)
                return framework.DataContext as IRegionMemberLifetime;

            return null;
        }

        private static DependentViewInfo CreateDependentView(DependentViewAttribute atr)
        {
            var info = new DependentViewInfo
            {
                TargetRegionName = atr.TargetRegionName, View = Activator.CreateInstance(atr.Type)
            };

            return info;
        }

        private static IEnumerable<T> GetCustomAttributes<T>(ICustomAttributeProvider type)
        {
            return type.GetCustomAttributes(typeof(T), true).OfType<T>();
        }

        private class DependentViewInfo
        {
            public object? View { get; init; }
            public string? TargetRegionName { get; init; }
        }
    }
}
