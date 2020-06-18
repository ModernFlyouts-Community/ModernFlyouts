using System.Windows.Input;

namespace ModernFlyouts
{
    public class LockKeysHelper : HelperBase
    {
        private LockKeysControl lockKeysControl;

        public override event ShowFlyoutEventHandler ShowFlyoutRequested;

        public LockKeysHelper()
        {
            Initialize();
        }

        public void Initialize()
        {
            AlwaysHandleDefaultFlyout = false;

            PrimaryContent = null;
            PrimaryContentVisible = false;

            lockKeysControl = new LockKeysControl();

            SecondaryContent = lockKeysControl;
            SecondaryContentVisible = true;

            OnEnabled();
        }

        private void KeyPressed(Key Key)
        {
            if (Key == Key.CapsLock || Key == Key.Capital)
            {
                var islock = !Keyboard.IsKeyToggled(Key.CapsLock);
                Prepare(LockKeys.CapsLock, islock);
                ShowFlyout();
            }
            else if (Key == Key.NumLock)
            {
                var islock = !Keyboard.IsKeyToggled(Key.NumLock);
                Prepare(LockKeys.NumLock, islock);
                ShowFlyout();
            }
            else if (Key == Key.Scroll)
            {
                var islock = !Keyboard.IsKeyToggled(Key.Scroll);
                Prepare(LockKeys.ScrollLock, islock);
                ShowFlyout();
            }    

            void ShowFlyout()
            {
                ShowFlyoutRequested?.Invoke(this);
            }
        }

        private void Prepare(LockKeys key, bool islock)
        {
            var msg = key.ToString() + (islock ? " is on." : " is off.");
            lockKeysControl.txt.Text = msg;
        }

        private enum LockKeys
        {
            CapsLock,
            NumLock,
            ScrollLock
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();

            if (IsEnabled)
            {
                FlyoutHandler.Instance.KeyboardHook.KeyDown += KeyPressed;
            }
        }

        protected override void OnDisabled()
        {
            base.OnDisabled();

            FlyoutHandler.Instance.KeyboardHook.KeyDown -= KeyPressed;
        }
    }
}