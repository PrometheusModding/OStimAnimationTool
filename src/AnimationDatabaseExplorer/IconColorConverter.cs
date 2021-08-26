using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OStimAnimationTool.Core.Models.Navigation;
using static System.Windows.Media.Colors;
using Color = System.Windows.Media.Color;
using Colors = OStimAnimationTool.Core.Models.Navigation.Colors;

namespace AnimationDatabaseExplorer
{
    public class IconColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            BitmapSource source = new BitmapImage(
                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mass.png"));
            Color color;
            
            foreach (var value in values)
            {
                switch (value)
                {
                    case PageIcons:
                        source = value switch
                        {
                            PageIcons.MAss => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mass.png")),
                            PageIcons.MCuirass => new BitmapImage(new Uri(
                                "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mcuirass.png")),
                            PageIcons.MGenIm => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgenim.png")),
                            PageIcons.MGenSignF => new BitmapImage(new Uri(
                                "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgensignf.png")),
                            PageIcons.MGenSignM => new BitmapImage(new Uri(
                                "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgensignm.png")),
                            PageIcons.MHand => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mhand.png")),
                            PageIcons.MHandEx => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mhandex.png")),
                            PageIcons.MHeart => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mheart.png")),
                            PageIcons.MIntimateF => new BitmapImage(new Uri(
                                "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mintimatef.png")),
                            PageIcons.MMagi => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mmagi.png")),
                            PageIcons.MOrif => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/morif.png")),
                            PageIcons.MShirt => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mshirt.png")),
                            PageIcons.MTri => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtri.png")),
                            PageIcons.MTriEx => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtriex.png")),
                            PageIcons.MTriTri => new BitmapImage(
                                new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtritri.png")),
                            PageIcons.MWhipCream => new BitmapImage(new Uri(
                                "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mwhipcream.png")),
                            _ => throw new ArgumentOutOfRangeException()
                        };
                        break;
                    
                    case Colors:
                        color = value switch
                        {
                            Colors.Blue => Color.FromRgb(93, 198, 208),
                            Colors.Red => Color.FromRgb(231, 102, 113),
                            _ => Fuchsia
                        };
                        break;
                }
            }

            if (source.Format != PixelFormats.Bgra32)
            {   
                return source;
            }

            var bytesPerPixel = (source.Format.BitsPerPixel + 7) / 8;
            var stride = bytesPerPixel * source.PixelWidth;
            var buffer = new byte[stride * source.PixelHeight];

            source.CopyPixels(buffer, stride, 0);

            for (var y = 0; y < source.PixelHeight; y++)
            {
                for (var x = 0; x < source.PixelWidth; x++)
                {
                    var i = stride * y + bytesPerPixel * x;
                    var a = buffer[i + 3];

                    if (a == 0) continue;

                    buffer[i] = color.B;
                    buffer[i + 1] = color.G;
                    buffer[i + 2] = color.R;
                    buffer[i + 3] = color.A;
                }
            }

            return BitmapSource.Create(
                source.PixelWidth, source.PixelHeight,
                source.DpiX, source.DpiY,
                source.Format, null, buffer, stride);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
