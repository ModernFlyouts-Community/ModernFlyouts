using ModernFlyouts.Utilities;
using System.Windows.Input;

namespace ModernFlyouts
{
    public class LockKeysFlyoutHelper : FlyoutHelperBase
    {
        private LockKeysControl lockKeysControl;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        public LockKeysFlyoutHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = false;

            lockKeysControl = new LockKeysControl();

            PrimaryContent = lockKeysControl;

            OnEnabled();
        }

        private void KeyPressed(Key key, int virtualKey)
        {
            LockKeys? lockKey = key switch
            {
                Key.CapsLock => LockKeys.CapsLock,
                Key.NumLock => LockKeys.NumLock,
                Key.Scroll => LockKeys.ScrollLock,
                Key.Insert => LockKeys.Insert,
                _ => null
            };

            if (lockKey.HasValue)
            {
                Prepare(lockKey.Value, !Keyboard.IsKeyToggled(key));
                ShowFlyout();
            }

            void ShowFlyout()
            {
                ShowFlyoutRequested?.Invoke(this);
            }
        }

        private void Prepare(LockKeys key, bool islock)
        {
            string msg;
            if (key != LockKeys.Insert)
            {
                msg = string.Format(islock ? Properties.Strings.LockKeysFlyoutHelper_KeyIsOn : Properties.Strings.LockKeysFlyoutHelper_KeyIsOff, key.ToString());
                lockKeysControl.LockGlyph.Glyph = islock ? CommonGlyphs.Lock : CommonGlyphs.Unlock;
            }
            else
            {
                msg = islock ? Properties.Strings.LockKeysFlyoutHelper_OvertypeMode : Properties.Strings.LockKeysFlyoutHelper_InsertMode;
                lockKeysControl.LockGlyph.Glyph = string.Empty;
            }
            
            lockKeysControl.txt.Text = msg;
        }

        private enum LockKeys
        {
            CapsLock,
            NumLock,
            ScrollLock,
            Insert
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            AppDataHelper.LockKeysModuleEnabled = IsEnabled;

            if (IsEnabled)
            {
                FlyoutHandler.Instance.KeyboardHook.KeyDown += KeyPressed;
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            FlyoutHandler.Instance.KeyboardHook.KeyDown -= KeyPressed;

            AppDataHelper.LockKeysModuleEnabled = IsEnabled;
        }
    }
}