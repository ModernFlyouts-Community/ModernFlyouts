using Microsoft.UI.Xaml.Shapes;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ModernFlyouts.WinUI.Helpers;
using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModernFlyouts.WinUI.Contracts.Services;
using ModernFlyouts.WinUI.Services;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ModernFlyouts.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public Window Window => m_window;

        public static bool IsElevated { get; set; }

        public static bool IsUserAnAdmin { get; set; }

        // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static IHost _host = Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Default Activation Handler
                //services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

                // Other Activation Handlers

                // Services
                //services.AddSingleton<ILocalSettingsService, LocalSettingsServicePackaged>();

                services.AddSingleton<ILocalizationService, LocalizationService>();
                //services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
                //services.AddTransient<INavigationViewService, NavigationViewService>();

                //services.AddSingleton<IActivationService, ActivationService>();
                //services.AddSingleton<IPageService, PageService>();
                //services.AddSingleton<INavigationService, NavigationService>();

                //// Core Services
                //services.AddSingleton<IFileService, FileService>();

                //// Views and ViewModels
                //services.AddTransient<SettingsViewModel>();
                //services.AddTransient<SettingsPage>();
                //services.AddTransient<MainViewModel>();
                //services.AddTransient<MainPage>();
                //services.AddTransient<ShellPage>();
                //services.AddTransient<ShellViewModel>();

                // Configuration
                //services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));
            })
            .Build();

        public static T GetService<T>()
            where T : class
            => _host.Services.GetService(typeof(T)) as T;



        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            UnhandledException += App_UnhandledException;
            JumpListHelper.CreateJumpListAsync();
        }

        private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO: Log and handle exceptions as appropriate.
            // For more details, see https://docs.microsoft.com/windows/winui/api/microsoft.ui.xaml.unhandledexceptioneventargs.
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            // If this is the first instance launched, then register it as the "main" instance.
            // If this isn't the first instance launched, then "main" will already be registered,
            // so retrieve it.
            var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");

            // If the instance that's executing the OnLaunched handler right now
            // isn't the "main" instance.
            if (!mainInstance.IsCurrent)
            {
                // Redirect the activation (and args) to the "main" instance, and exit.
                var activatedEventArgs =
                    Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
                await mainInstance.RedirectActivationToAsync(activatedEventArgs);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
                return;
            }

            m_window = new SettingsWindow();
            m_window.Activate();


            //TODO Fix Remote desktop white screen issue
           // m_TransparentAppWindow = new TransparentWindow();
        }

        internal Window m_window;
        //public Window m_TransparentAppWindow;




        ////Cannot change runtime.
        ////Issue: https://github.com/microsoft/microsoft-ui-xaml/issues/4474
        //private void SetAppTheme(Common.AppTheme theme)
        //{
        //    switch (theme)
        //    {
        //        case Common.AppTheme.Auto:
        //            //Nothing
        //            break;
        //        case Common.AppTheme.Light:
        //            this.RequestedTheme = ApplicationTheme.Light;
        //            break;
        //        case Common.AppTheme.Dark:
        //            this.RequestedTheme = ApplicationTheme.Dark;
        //            break;
        //    }
        //}



    }
}
