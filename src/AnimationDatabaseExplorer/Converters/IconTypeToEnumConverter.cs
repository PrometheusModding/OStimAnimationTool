using System;
using System.Globalization;
using System.Windows.Data;
using OStimAnimationTool.Core.Models.Navigation;

namespace AnimationDatabaseExplorer.Converters
{
    public class IconTypeToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                Tab => Enum.GetValues(typeof(TabIcons)),
                Page => Enum.GetValues(typeof(PageIcons)),
                Option => Enum.GetValues(typeof(OptionIcons)),
                _ => Array.Empty<object>()
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
