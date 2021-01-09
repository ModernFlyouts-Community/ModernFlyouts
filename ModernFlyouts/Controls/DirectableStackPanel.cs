using ModernWpf.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

// Should this be moved to the ModernWpfCommunityToolkit so that it is available to everyone?
namespace ModernFlyouts.Controls
{
    public enum StackingDirection
    {
        Ascending,
        Descending
    }

    public class DirectableStackPanel : SimpleStackPanel
    {
        #region Properties

        #region Direction

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register(
                nameof(Direction),
                typeof(StackingDirection),
                typeof(DirectableStackPanel),
                new FrameworkPropertyMetadata(StackingDirection.Ascending, FrameworkPropertyMetadataOptions.AffectsArrange));

        public StackingDirection Direction
        {
            get => (StackingDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        #endregion

        #endregion

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            UIElementCollection children = InternalChildren;
            bool fHorizontal = Orientation == Orientation.Horizontal;
            bool ascending = Direction == StackingDirection.Ascending;
            Rect rcChild = new Rect(arrangeSize);
            double previousChildSize = 0.0;
            double spacing = Spacing;


            for (int i = 0, count = children.Count; i < count; i++)
            {
                var index = ascending ? i : (count - 1) - i;

                UIElement child = children[index];
                SetZIndex(child, i);

                if (child == null) { continue; }

                if (fHorizontal)
                {
                    rcChild.X += previousChildSize;
                    previousChildSize = child.DesiredSize.Width;
                    rcChild.Width = previousChildSize;
                    rcChild.Height = Math.Max(arrangeSize.Height, child.DesiredSize.Height);
                }
                else
                {
                    rcChild.Y += previousChildSize;
                    previousChildSize = child.DesiredSize.Height;
                    rcChild.Height = previousChildSize;
                    rcChild.Width = Math.Max(arrangeSize.Width, child.DesiredSize.Width);
                }

                if (child.Visibility != Visibility.Collapsed)
                {
                    previousChildSize += spacing;
                }

                child.Arrange(rcChild);
            }

            return arrangeSize;
        }
    }
}
