using ModernFlyouts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModernFlyouts.Contracts.Services
{
    public interface ILocalizationService
    {
        List<LanguageItem> Languages { get; }

        LanguageItem GetCurrentLanguageItem();
        Task InitializeAsync();
        Task SetLanguageAsync(LanguageItem languageItem);
    }
}