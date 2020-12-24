using System;

namespace ModernFlyouts.Core.UI
{
    [Flags]
    public enum FlyoutWindowAlignment
    {
        Center = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8
    }

    public enum FlyoutWindowExpandDirection
    {
        Auto = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    public enum FlyoutWindowPlacementMode
    {
        Auto = 0,
        Manual = 1
    }

    public enum FlyoutWindowType
    {
        OnScreen = 0,
        Tray = 1
    }
}
