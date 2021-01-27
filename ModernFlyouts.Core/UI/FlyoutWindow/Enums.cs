using System;

namespace ModernFlyouts.Core.UI
{
    [Flags]
    public enum FlyoutWindowAlignments
    {
        Center = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8
    }

    public enum FlyoutWindowExpandDirection
    {
        Auto,
        Up,
        Down,
        Left,
        Right
    }

    public enum FlyoutWindowPlacementMode
    {
        Auto,
        Manual
    }

    public enum FlyoutWindowType
    {
        OnScreen,
        Tray
    }
}
