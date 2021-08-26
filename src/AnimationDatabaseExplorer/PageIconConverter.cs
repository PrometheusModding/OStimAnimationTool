using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OStimAnimationTool.Core.Models.Navigation;

namespace AnimationDatabaseExplorer
{
    public class PageIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource path = value switch
            {
                PageIcons.MAss => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mass_border.png")),
                PageIcons.MCuirass => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mcuirass.png")),
                PageIcons.MGenIm => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgenim_border.png")),
                PageIcons.MGenSignF => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgensignf.png")),
                PageIcons.MGenSignM => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgensignm.png")),
                PageIcons.MHand => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mhand.png")),
                PageIcons.MHandEx => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mhandex.png")),
                PageIcons.MHeart => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mheart.png")),
                PageIcons.MIntimateF => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mintimatef.png")),
                PageIcons.MMagi => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mmagi.png")),
                PageIcons.MOrif => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/morif.png")),
                PageIcons.MShirt => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mshirt.png")),
                PageIcons.MTri => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtri_border.png")),
                PageIcons.MTriEx => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtriex.png")),
                PageIcons.MTriTri => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtritri.png")),
                PageIcons.MWhipCream => new BitmapImage(new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mwhipcream_border.png")),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };

            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
