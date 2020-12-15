using ModernFlyouts.Helpers;
using ModernFlyouts.Utilities;
using System.Windows.Input;

namespace ModernFlyouts
{
    public class LockKeysFlyoutHelper : FlyoutHelperBase
    {
        private LockKeysControl lockKeysControl;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        #region Properties

        private bool capsLockEnabled = DefaultValuesStore.LockKeysModule_CapsLockEnabled;

        public bool CapsLockEnabled
        {
            get => capsLockEnabled;
            set
            {
                if (SetProperty(ref capsLockEnabled, value))
                {
                    AppDataHelper.LockKeysModule_CapsLockEnabled = value;
                }
            }
        }

        private bool numLockEnabled = DefaultValuesStore.LockKeysModule_NumLockEnabled;

        public bool NumLockEnabled
        {
            get => numLockEnabled;
            set
            {
                if (SetProperty(ref numLockEnabled, value))
                {
                    AppDataHelper.LockKeysModule_NumLockEnabled = value;
                }
            }
        }

        private bool scrollLockEnabled = DefaultValuesStore.LockKeysModule_ScrollLockEnabled;

        public bool ScrollLockEnabled
        {
            get => scrollLockEnabled;
            set
            {
                if (SetProperty(ref scrollLockEnabled, value))
                {
                    AppDataHelper.LockKeysModule_ScrollLockEnabled = value;
                }
            }
        }

        private bool insertEnabled = DefaultValuesStore.LockKeysModule_InsertEnabled;

        public bool InsertEnabled
        {
            get => insertEnabled;
            set
            {
                if (SetProperty(ref insertEnabled, value))
                {
                    AppDataHelper.LockKeysModule_InsertEnabled = value;
                }
            }
        }

        #endregion

        public LockKeysFlyoutHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = false;

            CapsLockEnabled = AppDataHelper.LockKeysModule_CapsLockEnabled;
            NumLockEnabled = AppDataHelper.LockKeysModule_NumLockEnabled;
            ScrollLockEnabled = AppDataHelper.LockKeysModule_ScrollLockEnabled;
            InsertEnabled = AppDataHelper.LockKeysModule_InsertEnabled;

            lockKeysControl = new LockKeysControl();

            PrimaryContent = lockKeysControl;

            OnEnabled();
        }

        private bool IsKeyPressed;
        private void KeyPressed(Key key, int virtualKey)
        {
            if (IsKeyPressed)
            {
                return;
            }
            IsKeyPressed = true;
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
                var lk = lockKey.Value;
                if ((lk == LockKeys.CapsLock && !capsLockEnabled) || (lk == LockKeys.NumLock && !numLockEnabled)
                    || (lk == LockKeys.ScrollLock && !scrollLockEnabled) || (lk == LockKeys.Insert && !insertEnabled))
                {
                    return;
                }

                if (lk == LockKeys.Insert && Keyboard.Modifiers != ModifierKeys.None)
                {
                    return;
                }

                Prepare(lk, !Keyboard.IsKeyToggled(key));
                ShowFlyout();
            }

            void ShowFlyout()
            {
                ShowFlyoutRequested?.Invoke(this);
            }
        }

        private void KeyReleased(Key key, int virtualKey)
        {
            IsKeyPressed = false;
        }

        private void Prepare(LockKeys key, bool islock)
        {
            string msg;

            if (key != LockKeys.Insert)
            {
                string keyName = key switch
                {
                    LockKeys.CapsLock => Properties.Strings.LockKeysFlyoutHelper_CapsLock,
                    LockKeys.NumLock => Properties.Strings.LockKeysFlyoutHelper_NumLock,
                    LockKeys.ScrollLock => Properties.Strings.LockKeysFlyoutHelper_ScrollLock,
                    _ => string.Empty,
                };

                msg = string.Format(islock ? Properties.Strings.LockKeysFlyoutHelper_KeyIsOn : Properties.Strings.LockKeysFlyoutHelper_KeyIsOff, keyName);
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
                FlyoutHandler.Instance.KeyboardHook.KeyUp += KeyReleased;
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            FlyoutHandler.Instance.KeyboardHook.KeyDown -= KeyPressed;
            FlyoutHandler.Instance.KeyboardHook.KeyUp -= KeyReleased;

            AppDataHelper.LockKeysModuleEnabled = IsEnabled;
        }
    }
}
