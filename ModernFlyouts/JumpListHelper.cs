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
                Title = "Settings",
                Description = "Open settings window",
                ApplicationPath = AppPath,
                Arguments = arg_settings
            };
            jumpList.JumpItems.Add(settingsTask);

            JumpTask restoreTask = new JumpTask
            {
                Title = "Restore Default",
                Description = "Restores the windows default flyout and safely quits this app",
                ApplicationPath = AppPath,
                Arguments = arg_restore
            };
            jumpList.JumpItems.Add(restoreTask);

            JumpTask exitTask = new JumpTask
            {
                Title = "Exit",
                Description = "Exit the app safely",
                ApplicationPath = AppPath,
                Arguments = arg_exit
            };
            jumpList.JumpItems.Add(exitTask);

            jumpList.Apply();
        }
    }
}
