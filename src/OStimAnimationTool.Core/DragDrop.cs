using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Fluent;

namespace OStimAnimationTool.Core
{
    public enum DropState
    {
        CanDrop,
        CannotDrop
    }

    public class DragDropEventArgs : EventArgs
    {
        public object? DataContext;

        public DragDropEventArgs()
        {
        }

        public DragDropEventArgs(object dataContext)
        {
            DataContext = dataContext;
        }
    }

    public class DragDropPreviewBase : UserControl
    {
        public static readonly DependencyProperty DropStateProperty =
            DependencyProperty.Register(nameof(DropStateProperty), typeof(DropState), typeof(DragDropPreviewBase),
                new UIPropertyMetadata(DropStateChanged));

        public DragDropPreviewBase()
        {
            ScaleTransform scale = new(1f, 1f);
            SkewTransform skew = new(0f, 0f);
            RotateTransform rotate = new(0f);
            TranslateTransform trans = new(0f, 0f);
            TransformGroup transGroup = new();
            transGroup.Children.Add(scale);
            transGroup.Children.Add(skew);
            transGroup.Children.Add(rotate);
            transGroup.Children.Add(trans);

            RenderTransform = transGroup;
        }

        public DropState DropState
        {
            get => (DropState)GetValue(DropStateProperty);
            set => SetValue(DropStateProperty, value);
        }


        private static void DropStateChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var instance = (DragDropPreviewBase)element;
            instance.StateChangedHandler(element, e);
        }

        protected virtual void StateChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }

    public class DragDrop
    {
        private static readonly Lazy<DragDrop> Lazy = new(() => new DragDrop());
        private Point _delta;
        private Canvas? _dragDropContainer;
        private DragDropPreviewBase? _dragDropPreviewControl;
        private object? _dragDropPreviewControlDataContext;
        private UIElement? _dropTarget;
        private List<UIElement>? _dropTargets;
        private Point _initialPosition;
        private ICommand? _itemDroppedCommand;
        private bool _mouseCaptured;
        private RibbonWindow? _topWindow;
        private static DragDrop Instance => Lazy.Value;

        private static T? FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            return parentObject switch
            {
                null => null,
                T parent => parent,
                _ => FindParent<T>(parentObject)
            };
        }

        private static bool SufficientMovement(Point initialPosition, Point currentPosition)
        {
            return Math.Abs(currentPosition.X - initialPosition.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                   Math.Abs(currentPosition.Y - initialPosition.Y) >= SystemParameters.MinimumVerticalDragDistance;
        }

        #region ItemDroppedProperty

        public static readonly DependencyProperty ItemDroppedProperty = DependencyProperty.RegisterAttached(
            "ItemDropped", typeof(ICommand), typeof(DragDrop),
            new PropertyMetadata(AttachOrRemoveItemDroppedEvent));

        public static ICommand GetItemDropped(DependencyObject element)
        {
            return (ICommand)element.GetValue(ItemDroppedProperty);
        }

        public static void SetItemDropped(DependencyObject element, ICommand value)
        {
            element.SetValue(ItemDroppedProperty, value);
        }

        private static void AttachOrRemoveItemDroppedEvent(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region DragDropContainerProperty

        public static readonly DependencyProperty DragDropContainerProperty =
            DependencyProperty.RegisterAttached("DragDropContainer", typeof(Panel), typeof(DragDrop),
                new PropertyMetadata(default(UIElement)));

        public static Panel GetDragDropContainer(DependencyObject element)
        {
            return (Panel)element.GetValue(DragDropContainerProperty);
        }

        public static void SetDragDropContainer(DependencyObject element, Panel value)
        {
            element.SetValue(DragDropContainerProperty, value);
        }

        #endregion

        #region DragDropPreviewControlProperty

        public static readonly DependencyProperty DragDropPreviewControlProperty =
            DependencyProperty.RegisterAttached("DragDropPreviewControl", typeof(DragDropPreviewBase),
                typeof(DragDrop), new PropertyMetadata(default(UIElement)));

        public static DragDropPreviewBase GetDragDropPreviewControl(DependencyObject element)
        {
            return (DragDropPreviewBase)element.GetValue(DragDropPreviewControlProperty);
        }

        public static void SetDragDropPreviewControl(DependencyObject element, DragDropPreviewBase value)
        {
            element.SetValue(DragDropPreviewControlProperty, value);
        }

        #endregion

        #region DragDropPreviewControlDataContextProperty

        public static readonly DependencyProperty DragDropPreviewControlDataContextProperty =
            DependencyProperty.RegisterAttached("DragDropPreviewControlDataContext", typeof(object),
                typeof(DragDrop), new PropertyMetadata(default(object)));

        public static object GetDragDropPreviewControlDataContext(DependencyObject element)
        {
            return element.GetValue(DragDropPreviewControlDataContextProperty);
        }

        public static void SetDragDropPreviewControlDataContext(DependencyObject element, object value)
        {
            element.SetValue(DragDropPreviewControlDataContextProperty, value);
        }

        #endregion

        #region DropTargetProperty

        public static readonly DependencyProperty DropTargetProperty =
            DependencyProperty.RegisterAttached("DropTarget", typeof(List<UIElement>), typeof(DragDrop),
                new PropertyMetadata(default(string)));

        public static List<UIElement> GetDropTarget(DependencyObject element)
        {
            return (List<UIElement>)element.GetValue(DropTargetProperty);
        }

        public static void SetDropTarget(DependencyObject element, UIElement value)
        {
            element.SetValue(DropTargetProperty, value);
        }

        #endregion

        #region IsDragSourceProperty

        public static readonly DependencyProperty IsDragSourceProperty =
            DependencyProperty.RegisterAttached("IsDragSource", typeof(bool), typeof(DragDrop),
                new PropertyMetadata(false, IsDragSourceChanged));

        public static bool GetIsDragSource(DependencyObject element)
        {
            return (bool)element.GetValue(IsDragSourceProperty);
        }

        public static void SetIsDragSource(DependencyObject element, bool value)
        {
            element.SetValue(IsDragSourceProperty, value);
        }

        private static void IsDragSourceChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            if (element is not UIElement dragSource) return;

            if (e.NewValue is true)
            {
                dragSource.PreviewMouseLeftButtonDown += Instance.DragSource_PreviewMouseLeftButtonDown;
                dragSource.PreviewMouseLeftButtonUp += Instance.DragSource_PreviewMouseLeftButtonUp;
                dragSource.PreviewMouseMove += Instance.DragSource_PreviewMouseMove;
            }
            else
            {
                dragSource.PreviewMouseLeftButtonDown -= Instance.DragSource_PreviewMouseLeftButtonDown;
                dragSource.PreviewMouseLeftButtonUp -= Instance.DragSource_PreviewMouseLeftButtonUp;
                dragSource.PreviewMouseMove -= Instance.DragSource_PreviewMouseMove;
            }
        }

        private void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.OriginalSource is Visual visual)
                    _topWindow = FindParent<RibbonWindow>(visual) ?? throw new InvalidOperationException();
                else
                    return;

                _initialPosition = e.GetPosition(_topWindow);

                _dragDropContainer = GetDragDropContainer((DependencyObject)sender) as Canvas ??
                                     FindParent<Canvas>(visual);

                _dropTargets = GetDropTarget(sender as DependencyObject ?? throw new Exception());

                _dragDropPreviewControlDataContext =
                    GetDragDropPreviewControlDataContext((DependencyObject)sender);

                _itemDroppedCommand = GetItemDropped((DependencyObject)sender);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragDropPreviewControlDataContext = null;
            _mouseCaptured = false;

            _dragDropPreviewControl?.ReleaseMouseCapture();
        }

        private void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseCaptured || _dragDropPreviewControlDataContext is null ||
                !SufficientMovement(_initialPosition, e.GetPosition(_topWindow))) return;

            _dragDropPreviewControl = GetDragDropPreviewControl((DependencyObject)sender);
            _dragDropPreviewControl.DataContext = _dragDropPreviewControlDataContext;
            _dragDropPreviewControl.Opacity = 0.7;

            if (_dragDropContainer is null) return;

            _dragDropContainer.Children.Add(_dragDropPreviewControl);
            _mouseCaptured = Mouse.Capture(_dragDropPreviewControl);

            Mouse.OverrideCursor = Cursors.Hand;

            Canvas.SetLeft(_dragDropPreviewControl, _initialPosition.X - 20);
            Canvas.SetTop(_dragDropPreviewControl, _initialPosition.Y - 15);

            _dragDropContainer.PreviewMouseMove += DragDropContainer_PreviewMouseMove;
            _dragDropContainer.PreviewMouseUp += DragDropContainer_PreviewMouseUp;
        }

        private void DragDropContainer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var currentPoint = e.GetPosition(_topWindow);

            Mouse.OverrideCursor = Cursors.Hand;
            currentPoint.X -= 20;
            currentPoint.Y -= 15;

            _delta = new Point(_initialPosition.X - currentPoint.X, _initialPosition.Y - currentPoint.Y);
            var target = new Point(_initialPosition.X - _delta.X, _initialPosition.Y - _delta.Y);

            if (_dragDropPreviewControl is null) return;
            Canvas.SetLeft(_dragDropPreviewControl, target.X);
            Canvas.SetTop(_dragDropPreviewControl, target.Y);

            _dragDropPreviewControl.DropState = DropState.CannotDrop;

            if (_dropTargets is null)
            {
                AnimateDropState();
                return;
            }

            foreach (var dropTarget in _dropTargets)
            {
                var transform = dropTarget.TransformToVisual(_dragDropContainer);
                var dropBoundingBox = transform.TransformBounds(new Rect(0, 0, dropTarget.RenderSize.Width,
                    dropTarget.RenderSize.Height));

                if (e.GetPosition(_dragDropContainer).X > dropBoundingBox.Left &&
                    e.GetPosition(_dragDropContainer).X < dropBoundingBox.Right &&
                    e.GetPosition(_dragDropContainer).Y > dropBoundingBox.Top &&
                    e.GetPosition(_dragDropContainer).Y < dropBoundingBox.Bottom)
                {
                    _dragDropPreviewControl.DropState = DropState.CanDrop;
                    _dropTarget = dropTarget;
                }

                //if (_itemDroppedCommand?.CanExecute() is false)
                //    _dragDropPreviewControl.DropState = DropState.CannotDrop;

                AnimateDropState();
            }
        }

        private void AnimateDropState()
        {
            if (_dragDropPreviewControl == null) return;
            switch (_dragDropPreviewControl.DropState)
            {
                case DropState.CanDrop:

                    if (_dragDropPreviewControl.Resources.Contains("canDropChanged"))
                        ((Storyboard)_dragDropPreviewControl.Resources["canDropChanged"]).Begin(
                            _dragDropPreviewControl);

                    break;
                case DropState.CannotDrop:
                    if (_dragDropPreviewControl.Resources.Contains("cannotDropChanged"))
                        ((Storyboard)_dragDropPreviewControl.Resources["cannotDropChanged"]).Begin(
                            _dragDropPreviewControl);

                    break;
            }
        }

        private static DoubleAnimation CreateDoubleAnimation(double to)
        {
            var anim = new DoubleAnimation
            {
                To = to, Duration = TimeSpan.FromMilliseconds(250), AccelerationRatio = 0.1, DecelerationRatio = 0.9
            };

            return anim;
        }

        private void DragDropContainer_PreviewMouseUp(object sender, MouseEventArgs e)
        {
            if (_dragDropPreviewControl != null)
                switch (_dragDropPreviewControl.DropState)
                {
                    case DropState.CanDrop:
                        try
                        {
                            var scaleXAnim = CreateDoubleAnimation(0);
                            Storyboard.SetTargetProperty(scaleXAnim,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));

                            var scaleYAnim = CreateDoubleAnimation(0);
                            Storyboard.SetTargetProperty(scaleYAnim,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));

                            var opacityAnim = CreateDoubleAnimation(0);
                            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("(UIElement.Opacity)"));

                            var canDropSb = new Storyboard { FillBehavior = FillBehavior.Stop };
                            canDropSb.Children.Add(scaleXAnim);
                            canDropSb.Children.Add(scaleYAnim);
                            canDropSb.Children.Add(opacityAnim);
                            canDropSb.Completed += (o, args) => { FinalizePreviewControlMouseUp(); };

                            canDropSb.Begin(_dragDropPreviewControl);

                            object?[] dataContext = { _dragDropPreviewControlDataContext, _dropTarget };
                            _itemDroppedCommand?.Execute(dataContext);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }

                        break;
                    case DropState.CannotDrop:
                        try
                        {
                            var translateXAnim = CreateDoubleAnimation(_delta.X);
                            Storyboard.SetTargetProperty(translateXAnim,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.X)"));

                            var translateYAnim = CreateDoubleAnimation(_delta.Y);
                            Storyboard.SetTargetProperty(translateYAnim,
                                new PropertyPath(
                                    "(UIElement.RenderTransform).(TransformGroup.Children)[3].(TranslateTransform.Y)"));

                            var opacityAnim = CreateDoubleAnimation(0);
                            opacityAnim.BeginTime = TimeSpan.FromMilliseconds(150);
                            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("(UIElement.Opacity)"));

                            var cannotDropSb = new Storyboard { FillBehavior = FillBehavior.Stop };
                            cannotDropSb.Children.Add(translateXAnim);
                            cannotDropSb.Children.Add(translateYAnim);
                            cannotDropSb.Children.Add(opacityAnim);
                            cannotDropSb.Completed += (s, args) => { FinalizePreviewControlMouseUp(); };

                            cannotDropSb.Begin(_dragDropPreviewControl);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }

                        break;
                }

            _dragDropPreviewControlDataContext = null;
            _mouseCaptured = false;
        }

        private void FinalizePreviewControlMouseUp()
        {
            if (_dragDropContainer is not null)
            {
                _dragDropContainer.Children.Remove(_dragDropPreviewControl);
                _dragDropContainer.PreviewMouseMove -= DragDropContainer_PreviewMouseMove;
                _dragDropContainer.PreviewMouseUp -= DragDropContainer_PreviewMouseUp;
            }

            _dragDropPreviewControl?.ReleaseMouseCapture();
            _dragDropPreviewControl = null;
            Mouse.OverrideCursor = null;
        }

        #endregion
    }
}
