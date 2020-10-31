using System;

namespace ModernFlyouts.Helpers
{
    internal static class PackUriHelper
    {
        public static Uri GetAbsoluteUri(string path)
        {
            return new Uri($"pack://application:,,,/ModernFlyouts;component/{path}");
        }
    }
}