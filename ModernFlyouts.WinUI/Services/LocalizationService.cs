using ModernFlyouts.Contracts.Services;
using ModernFlyouts.Models;
using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModernFlyouts.Services
{ 
    public class LocalizationService : ILocalizationService
    {
        private const string LocalizationTagSettingsKey = "LocalizationTag";

        private readonly ILocalSettingsService _localSettingsService;

        private readonly ResourceManager _resourceManager;
        private readonly ResourceContext _resourceContext;

        private LanguageItem _currentLanguageItem = new(Tag: "en-EN", DisplayName: "English");

        public List<LanguageItem> Languages { get; } = new();

        public LocalizationService(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;

            _resourceManager = new();
            _resourceContext = _resourceManager.CreateResourceContext();
        }

        public async Task InitializeAsync()
        {
            RegisterLanguageFromResource();

            string languageTag = await GetLanguageTagFromSettingsAsync();

            if (languageTag is not null && GetLanguageItem(languageTag) is LanguageItem languageItem)
            {
                await SetLanguageAsync(languageItem);
            }
            else
            {
                await SetLanguageAsync(_currentLanguageItem);
            }
        }

        public LanguageItem GetCurrentLanguageItem() => _currentLanguageItem;

        public async Task SetLanguageAsync(LanguageItem languageItem)
        {
            if (Languages.Contains(languageItem) is true)
            {
                _currentLanguageItem = languageItem;

                Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = languageItem.Tag;
                _resourceContext.QualifierValues["Language"] = languageItem.Tag;

                await _localSettingsService.SaveSettingAsync(LocalizationTagSettingsKey, languageItem.Tag);
            }
        }

        private LanguageItem GetLanguageItem(string languageTag)
        {
            return Languages.FirstOrDefault(item => item.Tag == languageTag);
        }

        private async Task<string> GetLanguageTagFromSettingsAsync()
        {
            return await _localSettingsService.ReadSettingAsync<string>(LocalizationTagSettingsKey);
        }

        private void RegisterLanguageFromResource()
        {
            ResourceMap resourceMap = _resourceManager.MainResourceMap.GetSubtree("LanguageList");

            for (uint i = 0; i < resourceMap.ResourceCount; i++)
            {
                var resource = resourceMap.GetValueByIndex(i);
                Languages.Add(new LanguageItem(resource.Key, resource.Value.ValueAsString));
            }
        }
    }
}