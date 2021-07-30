using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AnimationDatabaseExplorer
{
    public class DragDropConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<UIElement> elements = new();
            foreach (var value in values)
                switch (value)
                {
                    case ItemCollection items:
                        if (items[0] is TreeViewItem treeViewItem)
                            elements.AddRange(from object? item in treeViewItem.Items
                                select (UIElement)treeViewItem.ItemContainerGenerator.ContainerFromItem(item));

                        break;
                    case UIElement uiElement:
                        elements.Add(uiElement);
                        break;
                }

            return elements;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
