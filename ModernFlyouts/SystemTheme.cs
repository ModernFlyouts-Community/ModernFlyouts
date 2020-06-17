namespace ModernFlyouts
{
    public class SystemTheme
    {
        public static bool GetIsSystemLightTheme() => ReadDWord("SystemUsesLightTheme");

        public static bool GetUseAccentColor() => ReadDWord("ColorPrevalence");

        private static bool ReadDWord(string valueName)
        {
            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (regkey == null) return false;

            return (int)regkey.GetValue(valueName, 0) > 0;
        }
    }
}