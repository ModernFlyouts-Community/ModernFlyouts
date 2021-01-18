using ModernFlyouts.Helpers;
using ModernFlyouts.UI;
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernFlyouts.Controls
{
    public partial class SessionsPanel : UserControl
    {
        public SessionsPanel()
        {
            InitializeComponent();

            ContentScrollViewer.CommandBindings.Add(new CommandBinding(ScrollBar.LineLeftCommand, OnScrollCommand, OnQueryScrollCommand));
            ContentScrollViewer.CommandBindings.Add(new CommandBinding(ScrollBar.LineRightCommand, OnScrollCommand, OnQueryScrollCommand));
            ContentScrollViewer.CommandBindings.Add(new CommandBinding(ScrollBar.LineUpCommand, OnScrollCommand, OnQueryScrollCommand));
            ContentScrollViewer.CommandBindings.Add(new CommandBinding(ScrollBar.LineDownCommand, OnScrollCommand, OnQueryScrollCommand));
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
            if (ScrollViewerHelperEx.GetIsAnimating(ContentScrollViewer))
            {
                return;
            }

            double offset = 0;
            Orientation orientation = Orientation.Horizontal;

            if (command == ScrollBar.LineUpCommand)
            {
                offset = Math.Min(Math.Max(0, ContentScrollViewer.VerticalOffset - UIManager.DefaultSessionControlHeight - UIManager.DefaultSessionsPanelVerticalSpacing), ContentScrollViewer.ScrollableHeight);
                orientation = Orientation.Vertical;
            }
            else if (command == ScrollBar.LineDownCommand)
            {
                offset = Math.Min(Math.Max(0, ContentScrollViewer.VerticalOffset + UIManager.DefaultSessionControlHeight + UIManager.DefaultSessionsPanelVerticalSpacing), ContentScrollViewer.ScrollableHeight);
                orientation = Orientation.Vertical;
            }
            else if (command == ScrollBar.LineLeftCommand)
            {
                offset = Math.Min(Math.Max(0, ContentScrollViewer.HorizontalOffset - UIManager.FlyoutWidth), ContentScrollViewer.ScrollableWidth);
                orientation = Orientation.Horizontal;
            }
            else if (command == ScrollBar.LineRightCommand)
            {
                offset = Math.Min(Math.Max(0, ContentScrollViewer.HorizontalOffset + UIManager.FlyoutWidth), ContentScrollViewer.ScrollableWidth);
                orientation = Orientation.Horizontal;
            }

            ScrollViewerHelperEx.ScrollToOffset(ContentScrollViewer, orientation, offset, 367);
        }
    }
}
