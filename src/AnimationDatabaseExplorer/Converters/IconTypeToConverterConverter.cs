using System;
using System.Globalization;
using System.Windows.Data;
using OStimAnimationTool.Core.Models.Navigation;

namespace AnimationDatabaseExplorer.Converters
{
    public class IconTypeToConverterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            IMultiValueConverter converter = values[0] switch
            {
                Tab => new TabIconConverter(),
                Page => new PageIconConverter(),
                Option => new OptionIconConverter(),
                _ => new EmptyMultiValueConverter()
            };

            return converter.Convert(values[1..], targetType, parameter, culture);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
