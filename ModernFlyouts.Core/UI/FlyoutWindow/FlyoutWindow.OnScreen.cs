using ModernFlyouts.Core.Display;

namespace ModernFlyouts.Core.UI
{
    public partial class FlyoutWindow
    {
        private void PositionOnScreenFlyout(ref double x, ref double y, double width, double height)
        {
            var margin = Margin;
            var offset = Offset;
            var screen = Screen.PrimaryScreen;
            double screenX = screen.WorkingArea.X;
            double screenY = screen.WorkingArea.Y;
            double screenWidth = screen.WorkingArea.Width;
            double screenHeight = screen.WorkingArea.Height;

            bool xHandled = false;
            bool yHandled = false;

            var alignment = Alignment;

            if (alignment.HasFlag(FlyoutWindowAlignments.Left))
            {
                x = screenX + effectiveMarginLeft;
                xHandled = true;
            }
            else if (alignment.HasFlag(FlyoutWindowAlignments.Right))
            {
                x = screenX + screenWidth - width - effectiveMarginRight;
                xHandled = true;
            }

            if (alignment.HasFlag(FlyoutWindowAlignments.Top))
            {
                y = screenY + effectiveMarginTop;
                yHandled = true;
            }
            else if (alignment.HasFlag(FlyoutWindowAlignments.Bottom))
            {
                y = screenY + screenHeight - height - effectiveMarginBottom;
                yHandled = true;
            }

            // Which means x should be center aligned
            if (!xHandled)
            {
                double offsetX = ((offset.Left + offset.Right) / 2 - offset.Left) * DpiScale;
                x = screenX + (screenWidth / 2) - (width / 2) + margin.Left - margin.Right + offsetX;
            }
            // Which means y should be center aligned
            if (!yHandled)
            {
                double offsetY = ((offset.Top + offset.Bottom) / 2 - offset.Top) * DpiScale;
                y = screenX + (screenHeight / 2) - (height / 2) + margin.Top - margin.Bottom + offsetY;
            }
        }

        private FlyoutWindowExpandDirection CalculatedActualExpandDirectionOnScreen()
        {
            FlyoutWindowExpandDirection expandDirection = FlyoutWindowExpandDirection.Down;
            var margin = Margin;

            if (PlacementMode == FlyoutWindowPlacementMode.Auto)
            {
                var alignment = Alignment;

                if (alignment.HasFlag(FlyoutWindowAlignments.Left))
                {
                    if (alignment.HasFlag(FlyoutWindowAlignments.Top))
                    {
                        expandDirection = (margin.Left < margin.Top) ?
                            FlyoutWindowExpandDirection.Right : FlyoutWindowExpandDirection.Down;
                    }
                    else if (alignment.HasFlag(FlyoutWindowAlignments.Bottom))
                    {
                        expandDirection = (margin.Left < margin.Bottom) ?
                            FlyoutWindowExpandDirection.Right : FlyoutWindowExpandDirection.Up;
                    }
                    else
                    {
                        expandDirection = FlyoutWindowExpandDirection.Right;
                    }
                }
                else if (alignment.HasFlag(FlyoutWindowAlignments.Right))
                {
                    if (alignment.HasFlag(FlyoutWindowAlignments.Top))
                    {
                        expandDirection = (margin.Right < margin.Top) ?
                            FlyoutWindowExpandDirection.Left : FlyoutWindowExpandDirection.Down;
                    }
                    else if (alignment.HasFlag(FlyoutWindowAlignments.Bottom))
                    {
                        expandDirection = (margin.Right < margin.Bottom) ?
                            FlyoutWindowExpandDirection.Left : FlyoutWindowExpandDirection.Up;
                    }
                    else
                    {
                        expandDirection = FlyoutWindowExpandDirection.Left;
                    }
                }
                else
                {
                    if (alignment.HasFlag(FlyoutWindowAlignments.Top))
                    {
                        expandDirection = FlyoutWindowExpandDirection.Down;
                    }
                    else if (alignment.HasFlag(FlyoutWindowAlignments.Bottom))
                    {
                        expandDirection = FlyoutWindowExpandDirection.Up;
                    }
                    else
                    {
                        expandDirection = (margin.Bottom < margin.Top) ?
                            FlyoutWindowExpandDirection.Up : FlyoutWindowExpandDirection.Down;
                    }
                }
            }

            return expandDirection;
        }
    }
}
