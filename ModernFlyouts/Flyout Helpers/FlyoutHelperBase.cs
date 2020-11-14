using Microsoft.Toolkit.Mvvm.ComponentModel;
using System.Windows;

namespace ModernFlyouts
{
    public abstract class FlyoutHelperBase : ObservableObject
    {
        public FlyoutHelperBase Instance { get; set; }

        public abstract event ShowFlyoutEventHandler ShowFlyoutRequested;

        public delegate void ShowFlyoutEventHandler(FlyoutHelperBase sender);

        #region Properties

        private FrameworkElement primaryContent;

        public FrameworkElement PrimaryContent
        {
            get => primaryContent;
            set => SetProperty(ref primaryContent, value);
        }

        private FrameworkElement secondaryContent;

        public FrameworkElement SecondaryContent
        {
            get => secondaryContent;
            set => SetProperty(ref secondaryContent, value);
        }

        private bool primaryContentVisible = true;

        public bool PrimaryContentVisible
        {
            get => primaryContentVisible;
            set => SetProperty(ref primaryContentVisible, value);
        }

        private bool secondaryContentVisible;

        public bool SecondaryContentVisible
        {
            get => secondaryContentVisible;
            set => SetProperty(ref secondaryContentVisible, value);
        }

        public bool AlwaysHandleDefaultFlyout { get; protected set; }

        private bool isEnabled = true;

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (SetProperty(ref isEnabled, value))
                {
                    OnIsEnabledChanged();
                }
            }
        }

        #endregion Properties

        private void OnIsEnabledChanged()
        {
            if (isEnabled)
            {
                OnEnabled();
            }
            else
            {
                OnDisabled();
            }
        }

        protected virtual void OnEnabled()
        {
        }

        protected virtual void OnDisabled()
        {
            if (FlyoutHandler.Instance.FlyoutWindow.FlyoutHelper == this)
            {
                FlyoutHandler.Instance.FlyoutWindow.Visible = false;
            }
        }
    }
}