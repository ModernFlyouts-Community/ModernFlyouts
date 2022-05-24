using CommunityToolkit.WinUI.Notifications;
using System;
using Windows.UI.Notifications;

namespace ModernFlyouts.WinUI.Services
{
    internal partial class ToastNotificationsService
    {
        public void ShowToastNotificationSample()
        {

            // Create the toast content

            var content = new ToastContent()
            {
                // More about the Launch property at https://docs.microsoft.com/dotnet/api/CommunityToolkit.WinUI.notifications.toastcontent
                Launch = "ToastContentActivationParams",

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        HeroImage = new ToastGenericHeroImage()
                        {
                            Source = "ms-appx:///Assets/Hero.png"
                        },

                        Children =
                        {
                            new AdaptiveImage()
                            {
                                Source = "ms-appx:///Assets/Hero.png"
                            },
                            new AdaptiveText()
                            {
                                Text = "Sample Toast Welcome to ModernFlyouts"
                            },

                            new AdaptiveText()
                            {
                                 Text = @"Click OK to to learn how to customise your flyouts and start setting up ModernFlyouts."
                            }
                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        // More about Toast Buttons at https://docs.microsoft.com/dotnet/api/CommunityToolkit.WinUI.notifications.toastbutton
                        new ToastButton("OK", "ToastButtonActivationArguments")
                        {
                            ActivationType = ToastActivationType.Foreground
                        },

                        new ToastButtonDismiss("Cancel")
                    }
                }
            };

            // Add the content to the toast
            var toast = new ToastNotification(content.GetXml())
            {
                // TODO WTS: Set a unique identifier for this notification within the notification group. (optional)
                // More details at https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification.tag
                Tag = "ToastTag"
            };

            // And show the toast
            ShowToastNotification(toast);
        }
    }
}
