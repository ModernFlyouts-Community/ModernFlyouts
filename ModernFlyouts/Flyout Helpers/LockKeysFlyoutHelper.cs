using Microsoft.VisualBasic.Logging;
using ModernFlyouts.Controls;
using ModernFlyouts.Helpers;
using ModernFlyouts.Utilities;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System;

namespace ModernFlyouts
{
    public class LockKeysFlyoutHelper : FlyoutHelperBase
    {
        private LockKeysControl lockKeysControl;

        private DrawingImage drawingImage;

        #region Properties

        private bool useSmallFlyout = DefaultValuesStore.UseSmallFlyout;

        public bool UseSmallFlyout
        {
            get => useSmallFlyout;
            set
            {
                if (SetProperty(ref useSmallFlyout, value))
                {
                    OnUseSmallFlyoutChanged();
                }
            }
        }

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

            drawingImage = new DrawingImage();

            UseSmallFlyout = AppDataHelper.UseSmallFlyout;

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
                RequestShowFlyout();
            }
        }

        private void KeyReleased(Key key, int virtualKey)
        {
            IsKeyPressed = false;
        }

        private void Prepare(LockKeys key, bool islock)
        {
            string msg = string.Empty;
            lockKeysControl.Pad.Width = 0;
            if (!(UseSmallFlyout))
            {
                lockKeysControl.Pad.Width = 10;
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

                }
                else
                {
                    msg = islock ? Properties.Strings.LockKeysFlyoutHelper_OvertypeMode : Properties.Strings.LockKeysFlyoutHelper_InsertMode;
                }

                
            }

            resetIcons();
            switch (key)
            {
                case LockKeys.CapsLock:
                    if (islock == false) lockKeysControl.caps_off.Visibility = Visibility.Visible;
                    if (islock) lockKeysControl.caps_on.Visibility = Visibility.Visible;
                    break;
                case LockKeys.NumLock:
                    if (islock == false) lockKeysControl.num_off.Visibility = Visibility.Visible;
                    if (islock) lockKeysControl.num_on.Visibility = Visibility.Visible;
                    break;
                case LockKeys.ScrollLock:
                    if (islock == false) lockKeysControl.scroll_lock_off.Visibility = Visibility.Visible;
                    if (islock) lockKeysControl.scroll_lock_on.Visibility = Visibility.Visible;
                    break;
                case LockKeys.Insert:
                    if (islock == false) lockKeysControl.insert_on.Visibility = Visibility.Visible; // this if statement needs to be false on purpose because islock is swapped for insert
                    if (islock) lockKeysControl.insert_off.Visibility = Visibility.Visible;
                    break;
            }
            lockKeysControl.txt.Text = msg;
        }

        private void resetIcons()
        {
            lockKeysControl.caps_off.Visibility = Visibility.Collapsed;
            lockKeysControl.caps_on.Visibility = Visibility.Collapsed;
            lockKeysControl.num_off.Visibility = Visibility.Collapsed;
            lockKeysControl.num_on.Visibility = Visibility.Collapsed;
            lockKeysControl.scroll_lock_off.Visibility = Visibility.Collapsed;
            lockKeysControl.scroll_lock_on.Visibility = Visibility.Collapsed;
            lockKeysControl.insert_off.Visibility = Visibility.Collapsed;
            lockKeysControl.insert_on.Visibility = Visibility.Collapsed;
        }

        private enum LockKeys
        {
            CapsLock,
            NumLock,
            ScrollLock,
            Insert
        }

        private void OnUseSmallFlyoutChanged()
        {
            AppDataHelper.UseSmallFlyout = UseSmallFlyout;
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
