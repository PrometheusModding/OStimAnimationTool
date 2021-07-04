using System.Windows;
using System.Windows.Controls;

namespace OStimConversionTool
{
    public class ExtendedRibbonWindow : Fluent.RibbonWindow
    {
        public static readonly DependencyProperty OverlayCanvas = DependencyProperty.Register(nameof(OverlayCanvas), typeof(Canvas), typeof(ExtendedRibbonWindow));

        public static Canvas GetOverlayCanvas(DependencyObject element)
        {
            return (Canvas) element.GetValue(OverlayCanvas);
        }

        public static void SetOverlayCanvas(DependencyObject element, Canvas value)
        {
            element.SetValue(OverlayCanvas, value);
        }
    }
}
