using ModernFlyouts.Core.Interop;
using System;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Interop
{
    public class ShellMessageHookHandler : IWndProcHookHandler
    {
        private enum HookMessageEnum : uint
        {
            HOOK_MEDIA_PLAYPAUSE = 917504,
            HOOK_MEDIA_PREVIOUS = 786432,
            HOOK_MEDIA_NEXT = 720896,
            HOOK_MEDIA_STOP = 851968,
            HOOK_MEDIA_VOLPLUS = 655360,
            HOOK_MEDIA_VOLMINUS = 589824,
            HOOK_MEDIA_VOLMUTE = 524288
        }

        private uint messageShellHookId;

        public ShellMessageHookHandler()
        {
        }

        public uint OnHwndCreated(IntPtr hWnd, out bool register)
        {
            RegisterShellHookWindow(hWnd);
            messageShellHookId = RegisterWindowMessage("SHELLHOOK");

            register = true;

            return messageShellHookId;
        }

        public IntPtr OnWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == messageShellHookId)
            {
                if (wParam == (IntPtr)55)
                {
                    //Brightness
                    FlyoutHandler.Instance.BrightnessFlyoutHelper?.OnExternalUpdated();
                }
                else if (wParam == (IntPtr)12)
                {
                    switch ((long)lParam)
                    {
                        case (long)HookMessageEnum.HOOK_MEDIA_NEXT:
                        case (long)HookMessageEnum.HOOK_MEDIA_PREVIOUS:
                        case (long)HookMessageEnum.HOOK_MEDIA_PLAYPAUSE:
                        case (long)HookMessageEnum.HOOK_MEDIA_STOP:
                            //Media
                            FlyoutHandler.Instance.AudioFlyoutHelper?.OnExternalUpdated(true);
                            break;

                        case (long)HookMessageEnum.HOOK_MEDIA_VOLMINUS:
                        case (long)HookMessageEnum.HOOK_MEDIA_VOLMUTE:
                        case (long)HookMessageEnum.HOOK_MEDIA_VOLPLUS:
                            //Volume
                            FlyoutHandler.Instance.AudioFlyoutHelper?.OnExternalUpdated(false);
                            break;

                        default:
                            //Ignore mouse side buttons and other keyboard special keys
                            break;
                    }
                }
            }

            return IntPtr.Zero;
        }
    }
}
