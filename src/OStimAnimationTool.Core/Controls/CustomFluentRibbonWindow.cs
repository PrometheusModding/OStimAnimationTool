using System.Windows;
using System.Windows.Controls;
using Fluent;

namespace OStimAnimationTool.Core.Controls
{
    // Extension of the RibbonWindow to support DragDrop
    public class CustomRibbonWindow : RibbonWindow
    {
        // Dependency Property for the OverlayCanvas which displays the Preview of the dragged Object
        public static readonly DependencyProperty OverlayCanvasProperty =
            DependencyProperty.Register("OverlayCanvas", typeof(Canvas), typeof(CustomRibbonWindow));

        // Dependency Property for DropTarget in MainWindow
        public static readonly DependencyProperty DropTargetProperty =
            DependencyProperty.Register("DropTarget", typeof(UIElement), typeof(CustomRibbonWindow));

        public Canvas OverlayCanvas
        {
            get => (Canvas)GetValue(OverlayCanvasProperty);
            set => SetValue(OverlayCanvasProperty, value);
        }

        public UIElement DropTarget
        {
            get => (UIElement)GetValue(DropTargetProperty);
            set => SetValue(DropTargetProperty, value);
        }
    }
}
