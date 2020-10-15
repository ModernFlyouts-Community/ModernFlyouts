using System.Diagnostics;
using System.Windows.Shell;

namespace ModernFlyouts
{
    public class JumpListHelper
    {
        public static string AppPath = string.Empty;

        internal const string arg_settings = "/settings";
        internal const string arg_exit = "/exit-safe";
        internal const string arg_restore = "/restore";
        internal const char arg_delimiter = ' ';

        public static void CreateJumpList()
        {
            AppPath = Process.GetCurrentProcess().MainModule.FileName;

            JumpList jumpList = new JumpList();
            JumpTask settingsTask = new JumpTask
            {
                Title = Properties.Strings.SettingsItem,
                Description = Properties.Strings.SettingsItemDescription,
                ApplicationPath = AppPath,
                Arguments = arg_settings,
                IconResourceIndex = -1
            };
            jumpList.JumpItems.Add(settingsTask);

            JumpTask restoreTask = new JumpTask
            {
                Title = Properties.Strings.RestoreDefaultItem,
                Description = Properties.Strings.RestoreDefaultItemDescription,
                ApplicationPath = AppPath,
                Arguments = arg_restore,
                IconResourceIndex = -1,
            };
            jumpList.JumpItems.Add(restoreTask);

            JumpTask exitTask = new JumpTask
            {
                Title = Properties.Strings.ExitItem,
                Description = Properties.Strings.ExitItemDescription,
                ApplicationPath = AppPath,
                Arguments = arg_exit,
                IconResourceIndex = -1
            };
            jumpList.JumpItems.Add(exitTask);

            jumpList.Apply();
        }
    }
}
