using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Data;

namespace ModernFlyouts.Utilities
{
    public class LocalizationHelper
    {
        private static CultureInfo systemUICulture;

        #region Properties

        private static LanguageInfo currentLanguage;

        public static LanguageInfo CurrentLanguage
        {
            get { return currentLanguage; }
            set
            {
                if (currentLanguage != value)
                {
                    currentLanguage = value;
                    OnStaticPropertyChanged();
                    OnCurrentLanguageChanged();
                }
            }
        }

        public static ObservableCollection<LanguageInfo> SupportedLanguages { get; private set; }

        #endregion

        public static void Initialize()
        {
            systemUICulture = CultureInfo.CurrentUICulture;
            SupportedLanguages = GetAllSupportedLanguages();
            var language = AppDataHelper.Language;
            CurrentLanguage = SupportedLanguages.First(x => x.LanguageName == language);
        }

        private static ObservableCollection<LanguageInfo> GetAllSupportedLanguages()
        {
            var supportedLanguages = new ObservableCollection<LanguageInfo>();

            supportedLanguages.Add(new LanguageInfo(string.Empty));

            var resourceManager = Properties.Strings.ResourceManager;
            var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (var culture in cultures)
            {
                try
                {
                    if (culture.Equals(CultureInfo.InvariantCulture))
                    {
                        continue;
                    }

                    var resourceSet = resourceManager.GetResourceSet(culture, true, false);
                    if (resourceSet != null)
                    {
                        supportedLanguages.Add(new LanguageInfo(culture));
                    }
                }
                catch (CultureNotFoundException)
                {

                }
            }

            return supportedLanguages;
        }

        private static void OnCurrentLanguageChanged()
        {
            var uiCulture = CurrentLanguage.CultureInfo ?? systemUICulture;
            Thread.CurrentThread.CurrentUICulture = uiCulture;
            AppDataHelper.Language = CurrentLanguage.LanguageName;
        }

        public static string GetLocalisedEnumValue(Enum value)
        {
            var resourceId = $"Enums.{value.GetType().Name}.{value}";
            Debug.WriteLine(resourceId);
            try
            {
                return Properties.Strings.ResourceManager.GetString(resourceId, Properties.Strings.Culture);
            }
            catch
            {
                return value.ToString();
            }
        }

        public static event PropertyChangedEventHandler StaticPropertyChanged;

        private static void OnStaticPropertyChanged([CallerMemberName] string propertyName = "")
        {
            StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LanguageInfo
    {
        public LanguageInfo(CultureInfo cultureInfo)
        {
            CultureInfo = cultureInfo;
            Initialize();
        }

        public LanguageInfo(string cultureName)
        {
            if (!string.IsNullOrEmpty(cultureName))
            {
                CultureInfo = new CultureInfo(cultureName);
            }
            Initialize();
        }

        public CultureInfo CultureInfo { get; private set; }

        public string DisplayName { get; private set; }

        public string LanguageName { get; private set; }

        private void Initialize()
        {
            if (CultureInfo != null)
            {
                DisplayName = $"{CultureInfo.NativeName} - {CultureInfo.EnglishName} [{CultureInfo.Name}]";
                LanguageName = CultureInfo.Name;
            }
            else
            {
                DisplayName = Properties.Strings.Settings_SystemDefault;
                LanguageName = string.Empty;
            }
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public class EnumToLocalizedStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Enum @enum)
            {
                return LocalizationHelper.GetLocalisedEnumValue(@enum);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
