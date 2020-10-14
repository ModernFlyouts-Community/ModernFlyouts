using System;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Windows.Management.Deployment;
using Windows.ApplicationModel.Core;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace ModernFlyouts
{
    internal class SourceModernAppInfo : SourceAppInfo
    {
        public SourceModernAppInfo(string appId)
        {
            AppId = appId;
            FetchInfos();
        }

        public override event EventHandler InfoFetched;

        private AppListEntry sourceApp;
        private int currentAppIndex = 0;

        public override void Activate()
        {
            try
            {
                _ = sourceApp?.LaunchAsync();
            }
            catch { }
        }

        private async void FetchInfos()
        {
            try
            {
                var pm = new PackageManager();
                var packages = pm.FindPackagesForUser("");
                string path = string.Empty;

                async Task<AppListEntry> GetAppListEntry()
                {
                    foreach (var package in packages)
                    {
                        var result = await package.GetAppListEntriesAsync();
                        for (int i = 0; i < result.Count; i++)
                        {
                            var app = result[i];

                            if (app.AppUserModelId == AppId)
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
                AppName = sourceApp.DisplayInfo.DisplayName;

                
                var logoPath = GetRefinedLogoPath(path);

                if (File.Exists(logoPath))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    byte[] fileBytes = File.ReadAllBytes(logoPath);
                    memoryStream.Write(fileBytes, 0, fileBytes.Length);
                    memoryStream.Position = 0;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.StreamSource = memoryStream;
                        image.EndInit();
                        AppImage = image;
                    }, System.Windows.Threading.DispatcherPriority.Send);
                }

                InfoFetched?.Invoke(this, null);
            }
            catch { }
        }

        private string GetLogoPathFromAppPath(string appPath)
        {
            var factory = (IAppxFactory)new AppxFactory();
            string logo = string.Empty;

            string manifestPath = Path.Combine(appPath, "AppXManifest.xml");
            const int STGM_SHARE_DENY_NONE = 0x40;

            SHCreateStreamOnFileEx(manifestPath, STGM_SHARE_DENY_NONE, 0, false, IntPtr.Zero, out IStream strm);
            if (strm != null)
            {
                var reader = factory.CreateManifestReader(strm);
                var apps = reader.GetApplications();
                int i = 0;

                while (apps.GetHasCurrent())
                {
                    var app = apps.GetCurrent();
                    if (currentAppIndex == i)
                    {
                        app.GetStringValue("Square44x44Logo", out logo);
                        break;
                    }
                    else
                    {
                        i++;
                        apps.MoveNext();
                    }
                }
                Marshal.ReleaseComObject(strm);
            }

            Marshal.ReleaseComObject(factory);
            return logo;
        }

        private string GetRefinedLogoPath(string appPath)
        {
            var resourceName = GetLogoPathFromAppPath(appPath);
            const string targetSizeToken = ".targetsize-";
            const string scaleToken = ".scale-";
            var sizes = new List<int>();
            string name = Path.GetFileNameWithoutExtension(resourceName);
            string ext = Path.GetExtension(resourceName);

            string finalSizeToken;
            if (Directory.EnumerateFiles(Path.Combine(appPath, Path.GetDirectoryName(resourceName)), name + targetSizeToken + "*" + ext).Count() > 0)
            {
                finalSizeToken = targetSizeToken;
            }
            else
            {
                finalSizeToken = scaleToken;
            }

            foreach (var file in Directory.EnumerateFiles(Path.Combine(appPath, Path.GetDirectoryName(resourceName)), name + finalSizeToken + "*" + ext))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                int pos = fileName.IndexOf(finalSizeToken) + finalSizeToken.Length;
                string sizeText = fileName.Substring(pos);
                if (int.TryParse(sizeText, out int size))
                {
                    sizes.Add(size);
                }
            }
            if (sizes.Count == 0)
                return null;

            sizes.Sort();
            return Path.Combine(appPath, Path.GetDirectoryName(resourceName), name + finalSizeToken + sizes.First() + ext);
        }

        #region Appx Things

        [Guid("5842a140-ff9f-4166-8f5c-62f5b7b0c781"), ComImport]
        private class AppxFactory
        {
        }

        [Guid("BEB94909-E451-438B-B5A7-D79E767B75D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAppxFactory
        {
            void _VtblGap0_2(); // skip 2 methods
            IAppxManifestReader CreateManifestReader(IStream inputStream);
        }

        [Guid("4E1BD148-55A0-4480-A3D1-15544710637C"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAppxManifestReader
        {
            void _VtblGap0_1(); // skip 1 method
            IAppxManifestProperties GetProperties();
            void _VtblGap1_5(); // skip 5 methods
            IAppxManifestApplicationsEnumerator GetApplications();
        }

        [Guid("9EB8A55A-F04B-4D0D-808D-686185D4847A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAppxManifestApplicationsEnumerator
        {
            IAppxManifestApplication GetCurrent();
            bool GetHasCurrent();
            bool MoveNext();
        }

        [Guid("5DA89BF4-3773-46BE-B650-7E744863B7E8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAppxManifestApplication
        {
            [PreserveSig]
            int GetStringValue([MarshalAs(UnmanagedType.LPWStr)] string name, [MarshalAs(UnmanagedType.LPWStr)] out string vaue);
        }

        [Guid("03FAF64D-F26F-4B2C-AAF7-8FE7789B8BCA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAppxManifestProperties
        {
            [PreserveSig]
            int GetBoolValue([MarshalAs(UnmanagedType.LPWStr)] string name, out bool value);
            [PreserveSig]
            int GetStringValue([MarshalAs(UnmanagedType.LPWStr)] string name, [MarshalAs(UnmanagedType.LPWStr)] out string vaue);
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int SHCreateStreamOnFileEx(string fileName, int grfMode, int attributes, bool create, IntPtr reserved, out IStream stream);

        #endregion
    }
}
