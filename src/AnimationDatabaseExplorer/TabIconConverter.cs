using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OStimAnimationTool.Core.Models.Navigation;

namespace AnimationDatabaseExplorer
{
    public class TabIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource path =
                new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/ssub.png"));

            path = value switch
            {
                TabIcons.SSub => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/ssub.png")),
                TabIcons.SDom => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/sdom.png")),
                _ => path
            };

            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
