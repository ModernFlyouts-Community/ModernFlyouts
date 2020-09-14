using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace ModernFlyouts
{
    internal class StartupHelper
    {
        private const string StartupId = "ModernFlyoutsStartupId";

        public static async Task<bool> GetRunAtStartupEnabled()
        {
            StartupTask startupTask = await StartupTask.GetAsync(StartupId);

            return startupTask.State == StartupTaskState.Enabled;
        }

        public static async void SetRunAtStartupEnabled(bool value)
        {
            StartupTask startupTask = await StartupTask.GetAsync(StartupId);
            
            if (value)
            {
                await startupTask.RequestEnableAsync();
            }
            else
            {
                startupTask.Disable();
            }
        }
    }
}
