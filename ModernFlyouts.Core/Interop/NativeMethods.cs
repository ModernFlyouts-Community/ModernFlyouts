using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace ModernFlyouts.Core.Interop
{
    public static class NativeMethods
    {
        #region Enums

        public enum GetWindowLongFields
        {
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20
        }

        [Flags]
        public enum WindowStyles : uint
        {
            WS_OVERLAPPED = 0x00000000,
            WS_POPUP = 0x80000000,
            WS_CHILD = 0x40000000,
            WS_MINIMIZE = 0x20000000,
            WS_VISIBLE = 0x10000000,
            WS_DISABLED = 0x08000000,
            WS_CLIPSIBLINGS = 0x04000000,
            WS_CLIPCHILDREN = 0x02000000,
            WS_MAXIMIZE = 0x01000000,
            WS_BORDER = 0x00800000,
            WS_DLGFRAME = 0x00400000,
            WS_VSCROLL = 0x00200000,
            WS_HSCROLL = 0x00100000,
            WS_SYSMENU = 0x00080000,
            WS_THICKFRAME = 0x00040000,
            WS_GROUP = 0x00020000,
            WS_TABSTOP = 0x00010000,

            WS_MINIMIZEBOX = 0x00020000,
            WS_MAXIMIZEBOX = 0x00010000,

            WS_CAPTION = WS_BORDER | WS_DLGFRAME,
            WS_TILED = WS_OVERLAPPED,
            WS_ICONIC = WS_MINIMIZE,
            WS_SIZEBOX = WS_THICKFRAME,
            WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

            WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
            WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
            WS_CHILDWINDOW = WS_CHILD,
        }

        [Flags]
        public enum ExtendedWindowStyles : uint
        {
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_TRANSPARENT = 0x00000020,

            WS_EX_MDICHILD = 0x00000040,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_WINDOWEDGE = 0x00000100,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_CONTEXTHELP = 0x00000400,

            WS_EX_RIGHT = 0x00001000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,

            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_APPWINDOW = 0x00040000,

            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

            WS_EX_LAYERED = 0x00080000,

            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_LAYOUTRTL = 0x00400000,

            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_NOACTIVATE = 0x08000000
        }

        public static class SWP
        {
            public static readonly int
            NOSIZE = 0x0001,
            NOMOVE = 0x0002,
            NOZORDER = 0x0004,
            NOREDRAW = 0x0008,
            NOACTIVATE = 0x0010,
            DRAWFRAME = 0x0020,
            FRAMECHANGED = 0x0020,
            SHOWWINDOW = 0x0040,
            HIDEWINDOW = 0x0080,
            NOCOPYBITS = 0x0100,
            NOOWNERZORDER = 0x0200,
            NOREPOSITION = 0x0200,
            NOSENDCHANGING = 0x0400,
            DEFERERASE = 0x2000,
            ASYNCWINDOWPOS = 0x4000;
        }

        public enum ShowWindowCommands
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3,
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct WNDCLASSEX
        {
            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public int style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public RECT(Rect rect)
            {
                Left = (int)rect.Left;
                Right = (int)rect.Right;
                Top = (int)rect.Top;
                Bottom = (int)rect.Bottom;
            }

            public Size Size
            {
                get => new(Right - Left, Bottom - Top);
            }

            public int Height => Bottom - Top;

            public int Width => Right - Left;

            public Rect ToRect() => new(Left, Top, Width, Height);
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINTSTRUCT
        {
            public int x;
            public int y;

            public POINTSTRUCT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class COMRECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public COMRECT()
            {
            }

            public COMRECT(Rect r)
            {
                left = (int)r.X;
                top = (int)r.Y;
                right = (int)r.Right;
                bottom = (int)r.Bottom;
            }

            public COMRECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public KBDLLHOOKSTRUCTFlags flags;
            public uint time;
            public UIntPtr dwExtraInfo;
        }

        [Flags()]
        internal enum KBDLLHOOKSTRUCTFlags : uint
        {
            LLKHF_EXTENDED = 0x1,
            LLKHF_INJECTED = 0x10,
            LLKHF_ALTDOWN = 0x20,
            LLKHF_UP = 0x80
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int Length;

            public int Flags;

            public ShowWindowCommands ShowCmd;

            public POINT MinPosition;

            public POINT MaxPosition;

            public RECT NormalPosition;

            public static WINDOWPLACEMENT Default
            {
                get
                {
                    WINDOWPLACEMENT result = default;
                    result.Length = Marshal.SizeOf(result);
                    return result;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 4)]
        internal class MONITORINFOEX
        {
            internal int cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            internal RECT rcMonitor = new();
            internal RECT rcWork = new();
            internal int dwFlags = 0;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            internal char[] szDevice = new char[32];
        }

        #endregion

        #region Windowing related

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
        internal static extern IntPtr CreateWindowEx(int dwExStyle,
            ushort regResult,
            [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
            UInt32 dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowInBand")]
        public static extern IntPtr CreateWindowInBand(int dwExStyle,
                   ushort atomBomb,
                   [MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
                   uint dwStyle,
                   int x,
                   int y,
                   int nWidth,
                   int nHeight,
                   IntPtr hWndParent,
                   IntPtr hMenu,
                   IntPtr hInstance,
                   IntPtr lpParam,
                   int dwBand);

        [DllImport("user32.dll", SetLastError = true, EntryPoint = "RegisterClassEx")]
        internal static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

        [DllImport("user32.dll")]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        internal static extern bool UpdateWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

        public static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return GetWindowLongPtr32(hWnd, nIndex);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        internal static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", SetLastError = false)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern int SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ShowWindowAsync(IntPtr windowHandle, int nCmdShow);

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        internal static extern int GetDpiForWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = false)]
        internal static extern IntPtr GetShellWindow();

        [DllImport("user32.dll", SetLastError = false)]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public static string GetWindowClassName(IntPtr hWnd)
        {
            int nRet;
            StringBuilder className = new(256);
            nRet = GetClassName(hWnd, className, className.Capacity);
            if (nRet != 0)
            {
                return className.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static void SetToolWindow(IntPtr hWnd)
        {
            ApplyWindowStyles(hWnd, 0, WindowStyles.WS_SYSMENU,
                ExtendedWindowStyles.WS_EX_NOACTIVATE | ExtendedWindowStyles.WS_EX_TOOLWINDOW, 0);
        }

        public static void ApplyWindowStyles(IntPtr hWnd, WindowStyles wsToAdd = 0,
            WindowStyles wsToRemove = 0, ExtendedWindowStyles wsEXToAdd = 0, ExtendedWindowStyles wsEXToRemove = 0)
        {
            var aws = (uint)wsToAdd;
            var rws = (uint)wsToRemove;
            var awsex = (uint)wsEXToAdd;
            var rwsex = (uint)wsEXToRemove;
            var style = (long)GetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_STYLE);
            var exstyle = (long)GetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE);
            style |= aws; style &= ~rws;
            exstyle |= awsex; exstyle &= ~rwsex;
            SetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_STYLE, new(style));
            SetWindowLongPtr(hWnd, (int)GetWindowLongFields.GWL_EXSTYLE, new(exstyle));
        }

        #endregion

        #region Hooking

        internal delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        internal delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern int SetWindowsHookEx(int idHook, HookProc hookProc, IntPtr hInstance, int wParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll")]
        internal static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterShellHookWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        #endregion

        #region Miscellaneous

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        internal static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        internal static extern bool ReleaseCapture();

        #endregion

        #region Detect Fullscreen Window

        // This part of the source code is borrowed from the PowerToys codebase
        // https://github.com/microsoft/PowerToys/blob/master/src/modules/launcher/PowerLauncher/Helper/WindowsInteropHelper.cs#L112

        private const string WindowClassConsole = "ConsoleWindowClass";
        private const string WindowClassWinTab = "Flip3D";
        private const string WindowClassProgman = "Progman";
        private const string WindowClassWorkerW = "WorkerW";

        public static bool IsWindowFullscreen()
        {
            var hWnd = GetForegroundWindow();

            if (hWnd != IntPtr.Zero)
            {
                if (hWnd != GetDesktopWindow() && hWnd != GetShellWindow())
                {
                    var windowClass = GetWindowClassName(hWnd);

                    // for Win+Tab (Flip3D)
                    if (windowClass == WindowClassWinTab)
                    {
                        return false;
                    }

                    GetWindowRect(hWnd, out RECT appBounds);

                    // for console (ConsoleWindowClass), we have to check for negative dimensions
                    if (windowClass == WindowClassConsole)
                    {
                        return appBounds.Top < 0 && appBounds.Bottom < 0;
                    }

                    // for desktop (Progman or WorkerW, depends on the system), we have to check
                    if (windowClass == WindowClassProgman || windowClass == WindowClassWorkerW)
                    {
                        var hWndDesktop = FindWindowEx(hWnd, IntPtr.Zero, "SHELLDLL_DefView", null);
                        hWndDesktop = FindWindowEx(hWndDesktop, IntPtr.Zero, "SysListView32", "FolderView");
                        if (hWndDesktop != IntPtr.Zero)
                        {
                            return false;
                        }
                    }

                    Rect screenBounds = Display.Screen.FromHWnd(hWnd).Bounds;
                    if ((appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Screen APIs

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool GetMonitorInfo(HandleRef hmonitor, [In, Out] MONITORINFOEX info);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern bool EnumDisplayMonitors(HandleRef hdc, COMRECT rcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr MonitorFromWindow(HandleRef handle, int flags);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern bool SystemParametersInfo(int nAction, int nParam, ref RECT rc, int nUpdate);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr MonitorFromPoint(POINTSTRUCT pt, int flags);

        internal delegate bool MonitorEnumProc(IntPtr monitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr lParam);

        internal const int SM_CMONITORS = 80;
        internal const int SM_CXSCREEN = 0;
        internal const int SM_CYSCREEN = 1;
        internal const int SPI_GETWORKAREA = 48;
        internal const int SPI_SETWORKAREA = 47;

        #endregion

        #region Window Message Constants

        internal const int SC_MOUSEMOVE = 0xF012;

        public const int WM_DESTROY = 0x2;
        public const int WM_ACTIVATE = 0x6;
        public const int WM_SYSCOMMAND = 0x112;
        public const int WM_PAINT = 0x0f;
        public const int WM_DPICHANGED = 0x02E0;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_MOUSEACTIVATE = 0x21;
        public const int WM_EXITSIZEMOVE = 0x0232;
        public const int WM_QUERYENDSESSION = 0x11;
        public const int WM_DISPLAYCHANGE = 0x7E;
        public const int WM_SETTINGCHANGE = 0x1A;

        #endregion
    }
}
