using System.Windows;
using System.Windows.Controls;
using Fluent;

namespace OStimAnimationTool.Core
{
    public class ExtendedRibbonWindow : RibbonWindow
    {
        public static readonly DependencyProperty OverlayCanvasProperty =
            DependencyProperty.Register("OverlayCanvas", typeof(Canvas), typeof(ExtendedRibbonWindow));

        public static readonly DependencyProperty DropTargetProperty =
            DependencyProperty.Register("DropTarget", typeof(UIElement), typeof(ExtendedRibbonWindow));

        public Canvas OverlayCanvas
        {
            get => (Canvas) GetValue(OverlayCanvasProperty);
            set => SetValue(OverlayCanvasProperty, value);
        }

        public UIElement DropTarget
        {
            get => (UIElement) GetValue(DropTargetProperty);
            set => SetValue(DropTargetProperty, value);
        }
    }
}
