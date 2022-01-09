using NPSMLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

/* 
 * This code was imported from the legacy ModernFlyouts app
 */

namespace ModernFlyouts.Core.Utilities
{
    public class AppxUtilities
    {
        public static string GetRefinedLogoPath(string appPath, int currentAppIndex)
        {
            var resourceName = GetLogoPathFromAppPath(appPath, currentAppIndex);
            const string targetSizeToken = ".targetsize-";
            const string scaleToken = ".scale-";
            SortedDictionary<int, string> files = new SortedDictionary<int, string>();
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

        private static string GetLogoPathFromAppPath(string appPath, int currentAppIndex)
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

        public static string GetAppUserModelIdForProcess(NowPlayingSession nps)
        {
            using var process = Process.GetProcessById((int)nps.PID);
            if (process == null)
                return string.Empty;

            int amuidBufferLength = 512;
            StringBuilder amuidBuffer = new StringBuilder(amuidBufferLength);

            GetApplicationUserModelId(process.Handle, ref amuidBufferLength, amuidBuffer);
            return amuidBuffer.ToString();
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
        internal static extern int GetApplicationUserModelId(
         IntPtr hProcess,
         ref int applicationUserModelIdLength,
         [MarshalAs(UnmanagedType.LPWStr)] StringBuilder applicationUserModelId);

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
    }
}
