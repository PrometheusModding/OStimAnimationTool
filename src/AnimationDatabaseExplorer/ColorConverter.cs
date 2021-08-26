using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Colors = OStimAnimationTool.Core.Models.Navigation.Colors;

namespace AnimationDatabaseExplorer
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value switch
            {
                Colors.Blue => Color.FromRgb(93, 198, 208),
                Colors.Red => Color.FromRgb(231, 102, 113),
                _ => System.Windows.Media.Colors.Magenta
            };

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
