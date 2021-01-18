using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Management.Deployment;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Core.AppInformation
{
    internal class SourceModernAppInfo : SourceAppInfo
    {
        public SourceModernAppInfo(SourceAppInfoData data)
        {
            Data = data;
        }

        public override event EventHandler InfoFetched;

        private AppListEntry sourceApp;
        private int currentAppIndex;

        public override void Activate()
        {
            try
            {
                if (Data.DataType == SourceAppInfoDataType.FromAppUserModelId)
                {
                    _ = sourceApp?.LaunchAsync();
                }
                else if (Data.DataType == SourceAppInfoDataType.FromProcessId)
                {
                    using var sourceProcess = Process.GetProcessById((int)Data.ProcessId);
                    IntPtr hWnd = IsWindow(Data.MainWindowHandle)
                        ? Data.MainWindowHandle : sourceProcess?.MainWindowHandle ?? IntPtr.Zero;
                    SourceDesktopAppInfo.ActivateWindow(hWnd);
                }
            }
            catch { }
        }

        public override async void FetchInfosAsync()
        {
            string appUserModelId = Data.AppUserModelId;
            string path = string.Empty;

            if (string.IsNullOrEmpty(Data.AppUserModelId)
                || string.IsNullOrWhiteSpace(Data.AppUserModelId))
            {
                await Task.Run(() =>
                {
                    appUserModelId = GetAppUserModelIdForProcess();
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
                return;

            try
            {
                DisplayName = sourceApp.DisplayInfo.DisplayName;
            }
            catch { }

            await Task.Run(() =>
            {
                string logoPath = string.Empty;
                try
                {
                    logoPath = GetRefinedLogoPath(path);
                }
                catch { }

                if (File.Exists(logoPath))
                {
                    MemoryStream memoryStream = new();
                    byte[] fileBytes = File.ReadAllBytes(logoPath);
                    memoryStream.Write(fileBytes, 0, fileBytes.Length);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    LogoStream = memoryStream;
                }
            });

            InfoFetched?.Invoke(this, null);
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
            SortedDictionary<int, string> files = new();
            string name = Path.GetFileNameWithoutExtension(resourceName);
            string ext = Path.GetExtension(resourceName);

            string finalSizeToken;
            if (Directory.EnumerateFiles(Path.Combine(appPath, Path.GetDirectoryName(resourceName)), name + targetSizeToken + "*" + ext).Any())
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
                string sizeText = string.Empty;
                if (fileName.Contains('_'))
                {
                    int endpos = fileName.IndexOf('_', pos);
                    sizeText = fileName[pos..endpos];
                }
                else
                {
                    sizeText = fileName[pos..];
                }

                if (int.TryParse(sizeText, out int size))
                {
                    if (!files.ContainsKey(size))
                    {
                        files.Add(size, file);
                    }
                }
            }
            if (files.Count == 0)
                return null;

            return files.First().Value;
        }

        private string GetAppUserModelIdForProcess()
        {
            using var process = Process.GetProcessById((int)Data.ProcessId);
            if (process == null)
                return string.Empty;

            int amuidBufferLength = 512;
            StringBuilder amuidBuffer = new(amuidBufferLength);

            GetApplicationUserModelId(process.Handle, ref amuidBufferLength, amuidBuffer);
            return amuidBuffer.ToString();
        }

        protected override void Disconnect()
        {
            base.Disconnect();
            sourceApp = null;
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
        private interface IAppxManifestApplication
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
