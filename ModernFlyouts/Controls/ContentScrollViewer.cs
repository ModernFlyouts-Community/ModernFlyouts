using ModernFlyouts.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernFlyouts.Controls
{
    public class ContentScrollViewer : ScrollViewer
    {

        #region Properties

        #region ContentWidth

        public static readonly DependencyProperty ContentWidthProperty =
            DependencyProperty.Register(
                nameof(ContentWidth),
                typeof(double),
                typeof(ContentScrollViewer),
                new PropertyMetadata(0.0));

        public double ContentWidth
        {
            get => (double)GetValue(ContentWidthProperty);
            set => SetValue(ContentWidthProperty, value);
        }

        #endregion

        #region ContentHeight

        public static readonly DependencyProperty ContentHeightProperty =
            DependencyProperty.Register(
                nameof(ContentHeight),
                typeof(double),
                typeof(ContentScrollViewer),
                new PropertyMetadata(0.0));

        public double ContentHeight
        {
            get => (double)GetValue(ContentHeightProperty);
            set => SetValue(ContentHeightProperty, value);
        }

        #endregion

        #region VerticalSpacing

        public static readonly DependencyProperty VerticalSpacingProperty =
            DependencyProperty.Register(
                nameof(VerticalSpacing),
                typeof(double),
                typeof(ContentScrollViewer),
                new PropertyMetadata(0.0));

        public double VerticalSpacing
        {
            get => (double)GetValue(VerticalSpacingProperty);
            set => SetValue(VerticalSpacingProperty, value);
        }

        #endregion

        #endregion

        static ContentScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentScrollViewer),
                new FrameworkPropertyMetadata(typeof(ContentScrollViewer)));
        }

        public ContentScrollViewer()
        {
            CommandBindings.Add(new CommandBinding(ScrollBar.LineLeftCommand, OnScrollCommand, OnQueryScrollCommand));
            CommandBindings.Add(new CommandBinding(ScrollBar.LineRightCommand, OnScrollCommand, OnQueryScrollCommand));
            CommandBindings.Add(new CommandBinding(ScrollBar.LineUpCommand, OnScrollCommand, OnQueryScrollCommand));
            CommandBindings.Add(new CommandBinding(ScrollBar.LineDownCommand, OnScrollCommand, OnQueryScrollCommand));
        }

        private void OnQueryScrollCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            bool isHorizontal = Keyboard.Modifiers == ModifierKeys.Shift;
            if (isHorizontal)
            {
                if (e.Delta < 0)
                {
                    ScrollContent(ScrollBar.LineRightCommand);
                }
                else
                {
                    ScrollContent(ScrollBar.LineLeftCommand);
                }
            }
            else
            {
                if (e.Delta < 0)
                {
                    ScrollContent(ScrollBar.LineDownCommand);
                }
                else
                {
                    ScrollContent(ScrollBar.LineUpCommand);
                }
            }
        }

        private void OnScrollCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ScrollContent(e.Command);
        }

        private void ScrollContent(ICommand command)
        {
            if (ScrollViewerHelperEx.GetIsAnimating(this))
            {
                return;
            }

            double offset = 0;
            Orientation orientation = Orientation.Horizontal;

            if (command == ScrollBar.LineUpCommand)
            {
                offset = Math.Min(Math.Max(0, VerticalOffset - ContentHeight - VerticalSpacing), ScrollableHeight);
                orientation = Orientation.Vertical;
            }
            else if (command == ScrollBar.LineDownCommand)
            {
                offset = Math.Min(Math.Max(0, VerticalOffset + ContentHeight + VerticalSpacing), ScrollableHeight);
                orientation = Orientation.Vertical;
            }
            else if (command == ScrollBar.LineLeftCommand)
            {
                offset = Math.Min(Math.Max(0, HorizontalOffset - ContentWidth), ScrollableWidth);
                orientation = Orientation.Horizontal;
            }
            else if (command == ScrollBar.LineRightCommand)
            {
                offset = Math.Min(Math.Max(0, HorizontalOffset + ContentWidth), ScrollableWidth);
                orientation = Orientation.Horizontal;
            }

            ScrollViewerHelperEx.ScrollToOffset(this, orientation, offset, 367);
        }
    }
}
