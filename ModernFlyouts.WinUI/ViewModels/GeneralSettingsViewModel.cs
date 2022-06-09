using System;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using ModernFlyouts.WinUI.Helpers;
using ModernFlyouts.WinUI.Services;

using ModernFlyouts.WinUI.Contracts.Services;
using ModernFlyouts.WinUI.Models;

using Windows.ApplicationModel;
using Microsoft.UI.Xaml;
using ModernFlyouts.Core;
using ModernFlyouts.WinUI.Views;
using CommunityToolkit.WinUI.Helpers;

namespace ModernFlyouts.WinUI.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/pages/settings.md
    public class GeneralSettingsViewModel : ObservableObject
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        private readonly ILocalizationService _localizationService;

        private LanguageItem _selectedLanguage;
        public LanguageItem SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                if (SetProperty(ref _selectedLanguage, value) is true)
                {
                    OnSelectedLanguageChanged(value);
                }
            }
        }

        private void OnSelectedLanguageChanged(LanguageItem value)
        {
            _localizationService.SetLanguageAsync(value);
            IsLocalizationChanged = true;
        }

        private bool _isLocalizationChanged;
        public bool IsLocalizationChanged
        {
            get { return _isLocalizationChanged; }
            set { SetProperty(ref _isLocalizationChanged, value); }
        }

        //private List<LanguageItem> _availableLanguages;
        //public List<LanguageItem> AvailableLanguages
        //{
        //    get { return _availableLanguages; }
        //    set { SetProperty(ref _availableLanguages, value); }
        //}


        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { SetProperty(ref _elementTheme, value); }
        }

        public string ModernFlyoutsVersion
        {
            get
            {
                return SystemInformation.Instance.ApplicationVersion.ToFormattedString();
            }
        }
        //private ICommand _switchThemeCommand;

        //public ICommand SwitchThemeCommand
        //{
        //    get
        //    {
        //        if (_switchThemeCommand == null)
        //        {
        //            _switchThemeCommand = new RelayCommand<ElementTheme>(
        //                async (param) =>
        //                {
        //                    ElementTheme = param;
        //                    await ThemeSelectorService.SetThemeAsync(param);
        //                });
        //        }

        //        return _switchThemeCommand;
        //    }
        //}

        public GeneralSettingsViewModel()
        {
        }

        //public async Task InitializeAsync()
        //{
        //    ModernFlyoutsVersion = ModernFlyoutsVersion()
        //    await Task.CompletedTask;
        //}




        //internal IAsyncRelayCommand LicenseCommand { get; }

        //private async Task ExecuteLicenseCommandAsync()
        //{
        //    await IWindowManager.ShowContentDialogAsync(
        //        new MarkdownContentDialog(
        //            await AssetsHelper.GetLicenseAsync()),
        //        Strings.Close,
        //        title: "License");
        //}


    }
}
