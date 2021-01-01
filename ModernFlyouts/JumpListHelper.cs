using System.Diagnostics;
using System.IO;
using System.Windows.Shell;

namespace ModernFlyouts
{
    public class JumpListHelper
    {
        public static string AppPath { get; set; } = string.Empty;

        internal const string arg_settings = "/settings";
        internal const string arg_exit = "/exit-safe";
        internal const string arg_restore = "/restore";
        internal const string arg_appupdated = "/app-restarted-after-update";
        internal const char arg_delimiter = ' ';

        public static void CreateJumpList()
        {
            AppPath = Path.Combine(Directory.GetCurrentDirectory(), "ModernFlyoutsLauncher.exe");

            JumpList jumpList = new();
            JumpTask settingsTask = new()
            {
                Title = Properties.Strings.SettingsItem,
                Description = Properties.Strings.SettingsItemDescription,
                ApplicationPath = AppPath,
                Arguments = arg_settings,
                IconResourceIndex = -1
            };
            jumpList.JumpItems.Add(settingsTask);

            JumpTask restoreTask = new()
            {
                Title = Properties.Strings.RestoreDefaultItem,
                Description = Properties.Strings.RestoreDefaultItemDescription,
                ApplicationPath = AppPath,
                Arguments = arg_restore,
                IconResourceIndex = -1,
            };
            jumpList.JumpItems.Add(restoreTask);

            JumpTask exitTask = new()
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
