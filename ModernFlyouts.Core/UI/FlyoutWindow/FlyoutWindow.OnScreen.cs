using ModernFlyouts.Core.Display;

namespace ModernFlyouts.Core.UI
{
    public partial class FlyoutWindow
    {
        private void PositionOnScreenFlyout(ref double x, ref double y, double width, double height)
        {
            var screen = Screen.PrimaryScreen;
            double screenX = screen.WorkingArea.X;
            double screenY = screen.WorkingArea.Y;
            double screenWidth = screen.WorkingArea.Width;
            double screenHeight = screen.WorkingArea.Height;

            bool xHandled = false;
            bool yHandled = false;

            if (Alignment.HasFlag(FlyoutWindowAlignment.Left))
            {
                x = screenX + effectiveMarginLeft;
                xHandled = true;
            }
            else if (Alignment.HasFlag(FlyoutWindowAlignment.Right))
            {
                x = screenX + screenWidth - width - effectiveMarginRight;
                xHandled = true;
            }

            if (Alignment.HasFlag(FlyoutWindowAlignment.Top))
            {
                y = screenY + effectiveMarginTop;
                yHandled = true;
            }
            else if (Alignment.HasFlag(FlyoutWindowAlignment.Bottom))
            {
                y = screenY + screenHeight - height - effectiveMarginBottom;
                yHandled = true;
            }

            // Which means x should be center aligned
            if (!xHandled)
            {
                x = screenX + (screenWidth / 2) - (width / 2) + effectiveMarginLeft - effectiveMarginRight;
            }
            // Which means y should be center aligned
            if (!yHandled)
            {
                y = screenX + (screenHeight / 2) - (height / 2) + effectiveMarginTop - effectiveMarginBottom;
            }
        }
    }
}
