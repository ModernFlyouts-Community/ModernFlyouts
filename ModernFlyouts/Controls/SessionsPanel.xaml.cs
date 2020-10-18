using ModernFlyouts.Utilities;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ModernFlyouts
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

        private void OnScrollCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ScrollBar.LineUpCommand)
            {
                double offset = Math.Min(Math.Max(0, ContentScrollViewer.VerticalOffset - UIManager.SessionControlHeight), ContentScrollViewer.ScrollableHeight);
                ScrollViewerHelperEx.ScrollToVerticalOffset(ContentScrollViewer, offset);
            }
            else if (e.Command == ScrollBar.LineDownCommand)
            {
                double offset = Math.Min(Math.Max(0, ContentScrollViewer.VerticalOffset + UIManager.SessionControlHeight), ContentScrollViewer.ScrollableHeight);
                ScrollViewerHelperEx.ScrollToVerticalOffset(ContentScrollViewer, offset);
            }
            else if (e.Command == ScrollBar.LineLeftCommand)
            {
                double offset = Math.Min(Math.Max(0, ContentScrollViewer.HorizontalOffset - UIManager.FlyoutWidth), ContentScrollViewer.ScrollableWidth);
                ScrollViewerHelperEx.ScrollToHorizontalOffset(ContentScrollViewer, offset);
            }
            else if (e.Command == ScrollBar.LineRightCommand)
            {
                double offset = Math.Min(Math.Max(0, ContentScrollViewer.HorizontalOffset + UIManager.FlyoutWidth), ContentScrollViewer.ScrollableWidth);
                ScrollViewerHelperEx.ScrollToHorizontalOffset(ContentScrollViewer, offset);
            }
        }
    }
}
