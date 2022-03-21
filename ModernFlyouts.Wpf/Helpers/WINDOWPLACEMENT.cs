using System;
using System.Runtime.InteropServices;

namespace ModernFlyouts.Wpf.Helpers
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Interop")]
    public struct WINDOWPLACEMENT
    {
        public int Length { get; set; }

        public int Flags { get; set; }

        public int ShowCmd { get; set; }

        public POINT MinPosition { get; set; }

        public POINT MaxPosition { get; set; }

        public RECT NormalPosition { get; set; }
    }
}