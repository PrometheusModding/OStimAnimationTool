using System.Windows;
using System.Windows.Controls;

namespace OStimAnimationTool.Core.Controls
{
    public class SetWorkspaceTabItem : TabItem
    {
        public static readonly DependencyProperty DropTargetProperty =
            DependencyProperty.Register("DropTarget", typeof(UIElement), typeof(SetWorkspaceTabItem));

        public UIElement DropTarget
        {
            get => (UIElement)GetValue(DropTargetProperty);
            set => SetValue(DropTargetProperty, value);
        }
    }
}
