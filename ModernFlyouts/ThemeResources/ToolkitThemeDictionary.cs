using ModernFlyouts.Helpers;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ModernFlyouts
{
    public class ToolkitThemeDictionary
    {
        internal const string LightKey = "Light";
        internal const string DarkKey = "Dark";
        internal const string HighContrastKey = "HighContrast";

        private static Dictionary<string, ResourceDictionary> _defaultThemeDictionaries = new();

        public static void SetKey(ResourceDictionary themeDictionary, string key)
        {
            var baseThemeDictionary = GetToolkitThemeDictionary(key);
            themeDictionary.MergedDictionaries.Add(baseThemeDictionary);
        }

        private static ResourceDictionary GetToolkitThemeDictionary(string key)
        {
            if (!_defaultThemeDictionaries.TryGetValue(key, out ResourceDictionary dictionary))
            {
                dictionary = new ResourceDictionary { Source = GetDefaultSource(key) };
                _defaultThemeDictionaries[key] = dictionary;
            }
            return dictionary;
        }

        private static Uri GetDefaultSource(string theme)
        {
            return PackUriHelper.GetAbsoluteUri($"ThemeResources/{theme}.xaml");
        }
    }
}
