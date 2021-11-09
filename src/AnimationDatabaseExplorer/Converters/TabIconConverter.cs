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
    public class TabIconConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            WpfDrawingSettings settings = new();
            FileSvgReader reader = new(settings);

            var background = reader.Read(Application
                .GetResourceStream(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/background.svg"))
                ?.Stream);
            ApplyBrush(new SolidColorBrush(values[0] switch
            {
                Colors.Blue => Color.FromRgb(93, 198, 208),
                Colors.Red => Color.FromRgb(231, 102, 113),
                _ => Color.FromRgb(93, 198, 208)
            }), background);

            var layer1 = reader.Read(Application.GetResourceStream(values[1] switch
            {
                TabIcons.SDom => new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/sdom.svg"),
                TabIcons.SFemale => new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/sfemale.svg"),
                TabIcons.SMale => new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/smale.svg"),
                TabIcons.SObs => new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/sobs.svg"),
                TabIcons.SPlus => new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/splus.svg"),
                TabIcons.SSub => new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Tab/ssub.svg"),
                _ => throw new InvalidOperationException()
            })?.Stream);

            var x = (layer1.Bounds.Width - background.Bounds.Width) / 2;
            var y = (layer1.Bounds.Height - background.Bounds.Height) / 2;
            background.Transform = new TranslateTransform(-background.Bounds.X + x, -background.Bounds.Y + y);
            layer1.Transform = new TranslateTransform(-layer1.Bounds.X, -layer1.Bounds.Y);

            var icon = new DrawingGroup();
            icon.Children.Add(background);
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
            {
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
}
