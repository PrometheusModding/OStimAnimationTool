using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Prism.Regions;

namespace AnimationDatabaseExplorer
{
    internal class CloseTabAction : TriggerAction<Button>
    {
        protected override void Invoke(object parameter)
        {
            if (parameter is not RoutedEventArgs args)
                return;

            var tabItem = FindParent<TabItem>(args.OriginalSource as DependencyObject);
            if (tabItem is null)
                return;

            var tabControl = FindParent<TabControl>(tabItem);

            var region = RegionManager.GetObservableRegion(tabControl).Value;
            if (region is null)
                return;

            RemoveItemFromRegion(tabItem, region);
        }

        private static void RemoveItemFromRegion(object item, IRegion region)
        {
            var navigationContext = new NavigationContext(region.NavigationService, null);
            if (CanRemove(item, navigationContext)) region.Remove(item);
        }

        private static bool CanRemove(object item, NavigationContext navigationContext)
        {
            var canRemove = true;

            if (item is IConfirmNavigationRequest confirmRequestItem)
                confirmRequestItem.ConfirmNavigationRequest(navigationContext, result => { canRemove = result; });

            if (item is FrameworkElement frameworkElement && canRemove)
                if (frameworkElement.DataContext is IConfirmNavigationRequest confirmNavigationRequest)
                    confirmNavigationRequest.ConfirmNavigationRequest(navigationContext,
                        result => { canRemove = result; });

            return canRemove;
        }

        private static T? FindParent<T>(DependencyObject? child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            return parentObject switch
            {
                null => null,
                T parent => parent,
                _ => FindParent<T>(parentObject)
            };
        }
    }
}
