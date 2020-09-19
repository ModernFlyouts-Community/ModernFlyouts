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
            if (key == Key.CapsLock || key == Key.Capital)
            {
                var islock = !Keyboard.IsKeyToggled(Key.CapsLock);
                Prepare(LockKeys.CapsLock, islock);
                ShowFlyout();
            }
            else if (key == Key.NumLock)
            {
                var islock = !Keyboard.IsKeyToggled(Key.NumLock);
                Prepare(LockKeys.NumLock, islock);
                ShowFlyout();
            }
            else if (key == Key.Scroll)
            {
                var islock = !Keyboard.IsKeyToggled(Key.Scroll);
                Prepare(LockKeys.ScrollLock, islock);
                ShowFlyout();
            }
            else if (key == Key.Insert)
            {
                var islock = !Keyboard.IsKeyToggled(Key.Insert);
                Prepare(LockKeys.Insert, islock);
                ShowFlyout();
            }

            void ShowFlyout()
            {
                ShowFlyoutRequested?.Invoke(this);
            }
        }

        private void Prepare(LockKeys key, bool islock)
        {
            string msg = string.Empty;

            if (key != LockKeys.Insert)
            {
                msg = key.ToString() + (islock ? " is on" : " is off");
            }
            else
            {
                msg = islock ? "Overtype Mode" : "Insert Mode";
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