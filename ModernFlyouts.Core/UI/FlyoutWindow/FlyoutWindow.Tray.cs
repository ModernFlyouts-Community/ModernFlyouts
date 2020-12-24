using ModernFlyouts.Core.Interop;
using System.Windows;

namespace ModernFlyouts.Core.UI
{
    public partial class FlyoutWindow
    {
        private void PositionTrayFlyout(ref double x, ref double y, double width, double height)
        {
            var taskbar = WindowsTaskbar.Current;
            var tbBounds = taskbar.Bounds;
            //var maxHeight = taskbar.ContainingScreen.WorkingArea.Height;
            //if (taskbar.IsAutoHideEnabled && (taskbar.Location == WindowsTaskbar.Position.Top || taskbar.Location == WindowsTaskbar.Position.Bottom))
            //{
            //    maxHeight -= tbBounds.Height;
            //}

            switch (taskbar.Location)
            {
                case WindowsTaskbar.Position.Left:
                    x = tbBounds.Right + effectiveMarginLeft;
                    y = tbBounds.Bottom - height - effectiveMarginBottom;
                    ActualExpandDirection = FlyoutWindowExpandDirection.Right;
                    break;
                case WindowsTaskbar.Position.Right:
                    x = tbBounds.Left - width - effectiveMarginRight;
                    y = tbBounds.Bottom - height - effectiveMarginBottom;
                    ActualExpandDirection = FlyoutWindowExpandDirection.Left;
                    break;
                case WindowsTaskbar.Position.Top:
                    x = taskbar.IsRightToLeftLayout ?
                        tbBounds.Left + effectiveMarginLeft : tbBounds.Right - width - effectiveMarginRight;
                    y = tbBounds.Bottom + effectiveMarginTop;
                    ActualExpandDirection = FlyoutWindowExpandDirection.Down;
                    break;
                case WindowsTaskbar.Position.Bottom:
                    x = taskbar.IsRightToLeftLayout ?
                        tbBounds.Left + effectiveMarginLeft : tbBounds.Right - width - effectiveMarginRight;
                    y = tbBounds.Top - height - effectiveMarginBottom;
                    ActualExpandDirection = FlyoutWindowExpandDirection.Up;
                    break;
            }
        }
    }
}
