using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace OStimConversionTool.Converters
{
    public class MultiBindingToUiElementListConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Cast<UIElement>().ToList();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
