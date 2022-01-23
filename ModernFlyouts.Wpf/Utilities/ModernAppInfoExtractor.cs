using ModernFlyouts.Standard.Classes;
using NPSMLib;
using System;
using System.IO;
using System.Threading.Tasks;
using ModernFlyouts.Core.Utilities;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;

/* 
 * This code was imported from the legacy ModernFlyouts app
 */

namespace ModernFlyouts.Utilities
{
    public class ModernAppInfoExtractor
    {
        private static AppListEntry sourceApp;
        private static int currentAppIndex;

        public static async Task<MediaSession> GetModernAppInfo(NowPlayingSession nps)
        {
            string appUserModelId = nps.SourceAppId;
            string path = string.Empty;
            MediaSession ms = new MediaSession();
            string DisplayName = "UWP";
            if (string.IsNullOrEmpty(nps.SourceAppId)
                || string.IsNullOrWhiteSpace(nps.SourceAppId))
            {
                await Task.Run(() =>
                {
                    appUserModelId = AppxUtilities.GetAppUserModelIdForProcess(nps);
                });
            }

            try
            {
                var pm = new PackageManager();
                var packages = pm.FindPackagesForUser(string.Empty);

                async Task<AppListEntry> GetAppListEntry()
                {
                    foreach (var package in packages)
                    {
                        var result = await package.GetAppListEntriesAsync();
                        for (int i = 0; i < result.Count; i++)
                        {
                            var app = result[i];

                            if (app.AppUserModelId == appUserModelId)
                            {
                                path = package.InstalledLocation.Path;
                                currentAppIndex = i;
                                return app;
                            }
                        }
                    }

                    return null;
                }

                sourceApp = await GetAppListEntry();
            }
            catch { }
            if (sourceApp == null)
            {
                ms.AppName = "DisplayName";
                ms.MediaPlayingSession = nps;
                return ms;
            }

            try
            {
                DisplayName = sourceApp.DisplayInfo.DisplayName;
            }
            catch { }
            string logoPath = string.Empty;
            try
            {
                logoPath = AppxUtilities.GetRefinedLogoPath(path, currentAppIndex);
            }
            catch { }
            MemoryStream memoryStream = new MemoryStream();
            if (File.Exists(logoPath))
            {

                byte[] fileBytes = File.ReadAllBytes(logoPath);
                memoryStream.Write(fileBytes, 0, fileBytes.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
            }
            ms.AppName = DisplayName;
            ms.AppIcon = memoryStream;
            ms.MediaPlayingSession = nps;
            return ms;
        }
    }
}
