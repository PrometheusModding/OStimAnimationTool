using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using OStimAnimationTool.Core.Models.Navigation;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using Colors = OStimAnimationTool.Core.Models.Navigation.Colors;

namespace AnimationDatabaseExplorer.Converters
{
    public class PageIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            WpfDrawingSettings settings = new();
            FileSvgReader reader = new(settings);

            Uri? backgroundUri;
            Uri? background1Uri = null;
            Uri? layer1Uri;

            if (values[1] is not PageIcons pageIcons) throw new InvalidOperationException();

            switch (pageIcons)
            {
                case PageIcons.MAss:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mass_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mass.svg");
                    break;
                case PageIcons.MCuirass:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mcuirass_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mcuirass.svg");
                    break;
                case PageIcons.MGenim:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgenim_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgenim.svg");
                    break;
                case PageIcons.MGenimm2:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgenimm2_background1.svg");
                    background1Uri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgenimm2_background2.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgenimm2.svg");
                    break;
                case PageIcons.MGenSignF:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgensignf_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgensignf.svg");
                    break;
                case PageIcons.MGenSignM:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgensignm_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mgensignm.svg");
                    break;
                case PageIcons.MHand:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mhand_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mhand.svg");
                    break;
                case PageIcons.MHandEx:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mhand_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mhandex.svg");
                    break;
                case PageIcons.MHeart:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mheart_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mheart.svg");
                    break;
                case PageIcons.MIntimateF:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mintimatef_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mintimatef.svg");
                    break;
                case PageIcons.MMagi:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mmagi_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mmagi.svg");
                    break;
                case PageIcons.MOrif:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/morif_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/morif.svg");
                    break;
                case PageIcons.MShirt:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mshirt_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mshirt.svg");
                    break;
                case PageIcons.MTri:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtri_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtri.svg");
                    break;
                case PageIcons.MTriEx:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtri_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtriex.svg");
                    break;
                case PageIcons.MTriTri:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtritri_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mtritri.svg");
                    break;
                case PageIcons.MWhipcream:
                    backgroundUri =
                        new Uri(
                            "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mwhipcream_background.svg");
                    layer1Uri = new Uri(
                        "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Page/mwhipcream.svg");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var background = reader.Read(Application.GetResourceStream(backgroundUri)?.Stream);

            ApplyBrush(new SolidColorBrush(values[0] switch
            {
                Colors.Blue => Color.FromRgb(93, 198, 208),
                Colors.Red => Color.FromRgb(231, 102, 113),
                _ => System.Windows.Media.Colors.Fuchsia
            }), background);

            var layer1 = reader.Read(Application.GetResourceStream(layer1Uri)?.Stream);

            var x = pageIcons is PageIcons.MGenimm2 ? layer1.Bounds.Width / 2 - background.Bounds.Width : (layer1.Bounds.Width - background.Bounds.Width) / 2;
            var y = (layer1.Bounds.Height - background.Bounds.Height) / 2;
            background.Transform = new TranslateTransform(-background.Bounds.X + x, -background.Bounds.Y + y);
            layer1.Transform = new TranslateTransform(-layer1.Bounds.X, -layer1.Bounds.Y);

            var icon = new DrawingGroup();
            icon.Children.Add(background);

            if (background1Uri is not null)
            {
                var background1 = reader.Read(Application.GetResourceStream(background1Uri)?.Stream);

                y = (layer1.Bounds.Height - background1.Bounds.Height) / 2;
                background1.Transform = new TranslateTransform(-background1.Bounds.X + background.Bounds.Width + x, -background1.Bounds.Y + y);
                icon.Children.Add(background1);
            }

            icon.Children.Add(layer1);
            return new DrawingImage(icon);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static void ApplyBrush(Brush brush, DrawingGroup drawingGroup)
        {
            foreach (var drawing in drawingGroup.Children)
                switch (drawing)
                {
                    case DrawingGroup drawingGrp:
                        ApplyBrush(brush, drawingGrp);
                        break;
                    case GeometryDrawing geometryDrawing:
                        geometryDrawing.Brush = brush;
                        break;
                }
        }
    }
}
