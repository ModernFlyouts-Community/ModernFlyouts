using System;
using System.Diagnostics;
using static ModernFlyouts.Interop.NativeMethods;

namespace ModernFlyouts.Interop
{
    public class DUIHook
    {
        #region Constants

        private const uint WINEVENT_OUTOFCONTEXT = 0x0000; // Events are ASYNC

        private const uint WINEVENT_SKIPOWNTHREAD = 0x0001; // Don't call back for events on installer's thread

        private const uint WINEVENT_SKIPOWNPROCESS = 0x0002; // Don't call back for events on installer's process

        private const uint WINEVENT_INCONTEXT = 0x0004; // Events are SYNC, this causes your dll to be injected into every process

        private const uint EVENT_MIN = 0x00000001;

        private const uint EVENT_MAX = 0x7FFFFFFF;

        private const uint EVENT_SYSTEM_SOUND = 0x0001;

        private const uint EVENT_SYSTEM_ALERT = 0x0002;

        private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;

        private const uint EVENT_SYSTEM_MENUSTART = 0x0004;

        private const uint EVENT_SYSTEM_MENUEND = 0x0005;

        private const uint EVENT_SYSTEM_MENUPOPUPSTART = 0x0006;

        private const uint EVENT_SYSTEM_MENUPOPUPEND = 0x0007;

        private const uint EVENT_SYSTEM_CAPTURESTART = 0x0008;

        private const uint EVENT_SYSTEM_CAPTUREEND = 0x0009;

        private const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;

        private const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

        private const uint EVENT_SYSTEM_CONTEXTHELPSTART = 0x000C;

        private const uint EVENT_SYSTEM_CONTEXTHELPEND = 0x000D;

        private const uint EVENT_SYSTEM_DRAGDROPSTART = 0x000E;

        private const uint EVENT_SYSTEM_DRAGDROPEND = 0x000F;

        private const uint EVENT_SYSTEM_DIALOGSTART = 0x0010;

        private const uint EVENT_SYSTEM_DIALOGEND = 0x0011;

        private const uint EVENT_SYSTEM_SCROLLINGSTART = 0x0012;

        private const uint EVENT_SYSTEM_SCROLLINGEND = 0x0013;

        private const uint EVENT_SYSTEM_SWITCHSTART = 0x0014;

        private const uint EVENT_SYSTEM_SWITCHEND = 0x0015;

        private const uint EVENT_SYSTEM_MINIMIZESTART = 0x0016;

        private const uint EVENT_SYSTEM_MINIMIZEEND = 0x0017;

        private const uint EVENT_SYSTEM_DESKTOPSWITCH = 0x0020;

        private const uint EVENT_SYSTEM_END = 0x00FF;

        private const uint EVENT_OEM_DEFINED_START = 0x0101;

        private const uint EVENT_OEM_DEFINED_END = 0x01FF;

        private const uint EVENT_UIA_EVENTID_START = 0x4E00;

        private const uint EVENT_UIA_EVENTID_END = 0x4EFF;

        private const uint EVENT_UIA_PROPID_START = 0x7500;

        private const uint EVENT_UIA_PROPID_END = 0x75FF;

        private const uint EVENT_CONSOLE_CARET = 0x4001;

        private const uint EVENT_CONSOLE_UPDATE_REGION = 0x4002;

        private const uint EVENT_CONSOLE_UPDATE_SIMPLE = 0x4003;

        private const uint EVENT_CONSOLE_UPDATE_SCROLL = 0x4004;

        private const uint EVENT_CONSOLE_LAYOUT = 0x4005;

        private const uint EVENT_CONSOLE_START_APPLICATION = 0x4006;

        private const uint EVENT_CONSOLE_END_APPLICATION = 0x4007;

        private const uint EVENT_CONSOLE_END = 0x40FF;

        private const uint EVENT_OBJECT_CREATE = 0x8000; // hwnd ID idChild is created item

        private const uint EVENT_OBJECT_DESTROY = 0x8001; // hwnd ID idChild is destroyed item

        private const uint EVENT_OBJECT_SHOW = 0x8002; // hwnd ID idChild is shown item

        private const uint EVENT_OBJECT_HIDE = 0x8003; // hwnd ID idChild is hidden item

        private const uint EVENT_OBJECT_REORDER = 0x8004; // hwnd ID idChild is parent of zordering children

        private const uint EVENT_OBJECT_FOCUS = 0x8005; // hwnd ID idChild is focused item

        private const uint EVENT_OBJECT_SELECTION = 0x8006; // hwnd ID idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex

        private const uint EVENT_OBJECT_SELECTIONADD = 0x8007; // hwnd ID idChild is item added

        private const uint EVENT_OBJECT_SELECTIONREMOVE = 0x8008; // hwnd ID idChild is item removed

        private const uint EVENT_OBJECT_SELECTIONWITHIN = 0x8009; // hwnd ID idChild is parent of changed selected items

        private const uint EVENT_OBJECT_STATECHANGE = 0x800A; // hwnd ID idChild is item w/ state change

        private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B; // hwnd ID idChild is moved/sized item

        private const uint EVENT_OBJECT_NAMECHANGE = 0x800C; // hwnd ID idChild is item w/ name change

        private const uint EVENT_OBJECT_DESCRIPTIONCHANGE = 0x800D; // hwnd ID idChild is item w/ desc change

        private const uint EVENT_OBJECT_VALUECHANGE = 0x800E; // hwnd ID idChild is item w/ value change

        private const uint EVENT_OBJECT_PARENTCHANGE = 0x800F; // hwnd ID idChild is item w/ new parent

        private const uint EVENT_OBJECT_HELPCHANGE = 0x8010; // hwnd ID idChild is item w/ help change

        private const uint EVENT_OBJECT_DEFACTIONCHANGE = 0x8011; // hwnd ID idChild is item w/ def action change

        private const uint EVENT_OBJECT_ACCELERATORCHANGE = 0x8012; // hwnd ID idChild is item w/ keybd accel change

        private const uint EVENT_OBJECT_INVOKED = 0x8013; // hwnd ID idChild is item invoked

        private const uint EVENT_OBJECT_TEXTSELECTIONCHANGED = 0x8014; // hwnd ID idChild is item w? test selection change

        private const uint EVENT_OBJECT_CONTENTSCROLLED = 0x8015;

        private const uint EVENT_SYSTEM_ARRANGMENTPREVIEW = 0x8016;

        private const uint EVENT_OBJECT_END = 0x80FF;

        private const uint EVENT_AIA_START = 0xA000;

        private const uint EVENT_AIA_END = 0xAFFF;

        #endregion Constants

        private WinEventDelegate procDelegate;
        private IntPtr HHookID = IntPtr.Zero;

        public event DUIHookEventHandler DUIShown;

        public event DUIHookEventHandler DUIHidden;

        public event DUIHookEventHandler DUIDestroyed;

        public delegate void DUIHookEventHandler();

        public DUIHook()
        {
            procDelegate = new WinEventDelegate(WinEventProc);
        }

        public void Hook()
        {
            HHookID = SetWinEventHook(EVENT_OBJECT_CREATE, EVENT_OBJECT_STATECHANGE, IntPtr.Zero, procDelegate, DUIHandler.ProcessId, 0, WINEVENT_OUTOFCONTEXT);
            if (HHookID == IntPtr.Zero)
            {
                throw new Exception("Could not set DUI hook");
            }

            Debug.WriteLine(nameof(DUIHook) + ": Hooked!");
        }

        private void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            if (idObject != 0 || idChild != 0)
            {
                return;
            }
            if (hwnd == DUIHandler.HWndHost)
            {
                switch (eventType)
                {
                    case EVENT_OBJECT_SHOW:
                        DUIShown?.Invoke();
                        Debug.WriteLine(nameof(DUIHook) + ": DUI Shown!");
                        break;

                    case EVENT_OBJECT_DESTROY:
                        DUIDestroyed?.Invoke();
                        Debug.WriteLine(nameof(DUIHook) + ": DUI Destroyed!");
                        break;

                    case EVENT_OBJECT_HIDE:
                        DUIHidden?.Invoke();
                        Debug.WriteLine(nameof(DUIHook) + ": DUI Hidden!");
                        break;

                    default:
                        break;
                }
            }
        }

        public void Rehook()
        {
            Unhook();
            Hook();

            Debug.WriteLine(nameof(DUIHook) + ": Rehooked!");
        }

        public void Unhook()
        {
            if (!(HHookID == IntPtr.Zero))
            {
                UnhookWinEvent(HHookID);
                Debug.WriteLine(nameof(DUIHook) + ": Unhooked!");
            }
        }

        ~DUIHook()
        {
            Unhook();
        }
    }
}