using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace ModernFlyouts
{
    public sealed class AppxPackage
    {
        private List<AppxApp> _apps = new List<AppxApp>();
        private IAppxManifestProperties _properties;

        private AppxPackage()
        {
        }

        public string FullName { get; private set; }
        public string Path { get; private set; }
        public string Publisher { get; private set; }
        public string PublisherId { get; private set; }
        public string ResourceId { get; private set; }
        public string FamilyName { get; private set; }
        public string ApplicationUserModelId { get; private set; }
        public string Logo { get; private set; }
        public string PublisherDisplayName { get; private set; }
        public string Description { get; private set; }
        public string DisplayName { get; private set; }
        public bool IsFramework { get; private set; }
        public Version Version { get; private set; }
        public AppxPackageArchitecture ProcessorArchitecture { get; private set; }

        public IReadOnlyList<AppxApp> Apps
        {
            get
            {
                return _apps;
            }
        }

        public IEnumerable<AppxPackage> DependencyGraph
        {
            get
            {
                return QueryPackageInfo(FullName, PackageConstants.PACKAGE_FILTER_ALL_LOADED).Where(p => p.FullName != FullName);
            }
        }

        public string FindHighestScaleQualifiedImagePath(string resourceName)
        {
            if (resourceName == null)
                throw new ArgumentNullException("resourceName");

            const string scaleToken = ".scale-";
            var sizes = new List<int>();
            string name = System.IO.Path.GetFileNameWithoutExtension(resourceName);
            string ext = System.IO.Path.GetExtension(resourceName);
            foreach (var file in Directory.EnumerateFiles(System.IO.Path.Combine(Path, System.IO.Path.GetDirectoryName(resourceName)), name + scaleToken + "*" + ext))
            {
                string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                int pos = fileName.IndexOf(scaleToken) + scaleToken.Length;
                string sizeText = fileName.Substring(pos);
                int size;
                if (int.TryParse(sizeText, out size))
                {
                    sizes.Add(size);
                }
            }
            if (sizes.Count == 0)
                return null;

            sizes.Sort();
            return System.IO.Path.Combine(Path, System.IO.Path.GetDirectoryName(resourceName), name + scaleToken + sizes.Last() + ext);
        }

        public override string ToString()
        {
            return FullName;
        }

        public static AppxPackage FromWindow(IntPtr handle)
        {
            int processId;
            GetWindowThreadProcessId(handle, out processId);
            if (processId == 0)
                return null;

            return FromProcess(processId);
        }

        public static AppxPackage FromProcess(Process process)
        {
            if (process == null)
            {
                return null;
            }

            try
            {
                return FromProcess(process.Handle);
            }
            catch
            {
                // probably access denied on .Handle
                return null;
            }
        }

        public static AppxPackage FromProcess(int processId)
        {
            const int QueryLimitedInformation = 0x1000;
            IntPtr hProcess = OpenProcess(QueryLimitedInformation, false, processId);
            try
            {
                return FromProcess(hProcess);
            }
            finally
            {
                if (hProcess != IntPtr.Zero)
                {
                    CloseHandle(hProcess);
                }
            }
        }

        public static AppxPackage FromProcess(IntPtr hProcess)
        {
            if (hProcess == IntPtr.Zero)
                return null;

            // hprocess must have been opened with QueryLimitedInformation
            int len = 0;
            GetPackageFullName(hProcess, ref len, null);
            if (len == 0)
                return null;

            var sb = new StringBuilder(len);
            string fullName = GetPackageFullName(hProcess, ref len, sb) == 0 ? sb.ToString() : null;
            if (string.IsNullOrEmpty(fullName)) // not an AppX
                return null;

            var package = QueryPackageInfo(fullName, PackageConstants.PACKAGE_FILTER_HEAD).First();

            len = 0;
            GetApplicationUserModelId(hProcess, ref len, null);
            sb = new StringBuilder(len);
            package.ApplicationUserModelId = GetApplicationUserModelId(hProcess, ref len, sb) == 0 ? sb.ToString() : null;
            return package;
        }

        public string GetPropertyStringValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return GetStringValue(_properties, name);
        }

        public bool GetPropertyBoolValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return GetBoolValue(_properties, name);
        }

        public string LoadResourceString(string resource)
        {
            return LoadResourceString(FullName, resource);
        }

        private static IEnumerable<AppxPackage> QueryPackageInfo(string fullName, PackageConstants flags)
        {
            IntPtr infoRef;
            OpenPackageInfoByFullName(fullName, 0, out infoRef);
            if (infoRef != IntPtr.Zero)
            {
                IntPtr infoBuffer = IntPtr.Zero;
                try
                {
                    int len = 0;
                    int count;
                    GetPackageInfo(infoRef, flags, ref len, IntPtr.Zero, out count);
                    if (len > 0)
                    {
                        var factory = (IAppxFactory)new AppxFactory();
                        infoBuffer = Marshal.AllocHGlobal(len);
                        int res = GetPackageInfo(infoRef, flags, ref len, infoBuffer, out count);
                        for (int i = 0; i < count; i++)
                        {
                            var info = (PACKAGE_INFO)Marshal.PtrToStructure(infoBuffer + i * Marshal.SizeOf(typeof(PACKAGE_INFO)), typeof(PACKAGE_INFO));
                            var package = new AppxPackage();
                            package.FamilyName = Marshal.PtrToStringUni(info.packageFamilyName);
                            package.FullName = Marshal.PtrToStringUni(info.packageFullName);
                            package.Path = Marshal.PtrToStringUni(info.path);
                            package.Publisher = Marshal.PtrToStringUni(info.packageId.publisher);
                            package.PublisherId = Marshal.PtrToStringUni(info.packageId.publisherId);
                            package.ResourceId = Marshal.PtrToStringUni(info.packageId.resourceId);
                            package.ProcessorArchitecture = info.packageId.processorArchitecture;
                            package.Version = new Version(info.packageId.VersionMajor, info.packageId.VersionMinor, info.packageId.VersionBuild, info.packageId.VersionRevision);

                            // read manifest
                            string manifestPath = System.IO.Path.Combine(package.Path, "AppXManifest.xml");
                            const int STGM_SHARE_DENY_NONE = 0x40;
                            IStream strm;
                            SHCreateStreamOnFileEx(manifestPath, STGM_SHARE_DENY_NONE, 0, false, IntPtr.Zero, out strm);
                            if (strm != null)
                            {
                                var reader = factory.CreateManifestReader(strm);
                                package._properties = reader.GetProperties();
                                package.Description = package.GetPropertyStringValue("Description");
                                package.DisplayName = package.GetPropertyStringValue("DisplayName");
                                package.Logo = package.GetPropertyStringValue("Logo");
                                package.PublisherDisplayName = package.GetPropertyStringValue("PublisherDisplayName");
                                package.IsFramework = package.GetPropertyBoolValue("Framework");

                                var apps = reader.GetApplications();
                                while (apps.GetHasCurrent())
                                {
                                    var app = apps.GetCurrent();
                                    var appx = new AppxApp(app);
                                    appx.Description = GetStringValue(app, "Description");
                                    appx.DisplayName = GetStringValue(app, "DisplayName");
                                    appx.EntryPoint = GetStringValue(app, "EntryPoint");
                                    appx.Executable = GetStringValue(app, "Executable");
                                    appx.Id = GetStringValue(app, "Id");
                                    appx.Logo = GetStringValue(app, "Logo");
                                    appx.SmallLogo = GetStringValue(app, "SmallLogo");
                                    appx.StartPage = GetStringValue(app, "StartPage");
                                    appx.Square150x150Logo = GetStringValue(app, "Square150x150Logo");
                                    appx.Square30x30Logo = GetStringValue(app, "Square30x30Logo");
                                    appx.BackgroundColor = GetStringValue(app, "BackgroundColor");
                                    appx.ForegroundText = GetStringValue(app, "ForegroundText");
                                    appx.WideLogo = GetStringValue(app, "WideLogo");
                                    appx.Wide310x310Logo = GetStringValue(app, "Wide310x310Logo");
                                    appx.ShortName = GetStringValue(app, "ShortName");
                                    appx.Square310x310Logo = GetStringValue(app, "Square310x310Logo");
                                    appx.Square70x70Logo = GetStringValue(app, "Square70x70Logo");
                                    appx.MinWidth = GetStringValue(app, "MinWidth");
                                    package._apps.Add(appx);
                                    apps.MoveNext();
                                }
                                Marshal.ReleaseComObject(strm);
                            }
                            yield return package;
                        }
                        Marshal.ReleaseComObject(factory);
                    }
                }
                finally
                {
                    if (infoBuffer != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(infoBuffer);
                    }
                    ClosePackageInfo(infoRef);
                }
            }
        }

        public static string LoadResourceString(string packageFullName, string resource)
        {
            if (packageFullName == null)
                throw new ArgumentNullException("packageFullName");

            if (string.IsNullOrWhiteSpace(resource))
                return null;

            const string resourceScheme = "ms-resource:";
            if (!resource.StartsWith(resourceScheme))
                return null;

            string part = resource.Substring(resourceScheme.Length);
            string url;

            if (part.StartsWith("/"))
            {
                url = resourceScheme + "//" + part;
            }
            else
            {
                url = resourceScheme + "///resources/" + part;
            }

            string source = string.Format("@{{{0}? {1}}}", packageFullName, url);
            var sb = new StringBuilder(1024);
            int i = SHLoadIndirectString(source, sb, sb.Capacity, IntPtr.Zero);
            if (i != 0)
                return null;

            return sb.ToString();
        }

        private static string GetStringValue(IAppxManifestProperties props, string name)
        {
            if (props == null)
                return null;

            string value;
            props.GetStringValue(name, out value);
            return value;
        }

        private static bool GetBoolValue(IAppxManifestProperties props, string name)
        {
            bool value;
            props.GetBoolValue(name, out value);
            return value;
        }

        internal static string GetStringValue(IAppxManifestApplication app, string name)
        {
            string value;
            app.GetStringValue(name, out value);
            return value;
        }

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
        internal static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int SHCreateStreamOnFileEx(string fileName, int grfMode, int attributes, bool create, IntPtr reserved, out IStream stream);

        [DllImport("user32.dll")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int OpenPackageInfoByFullName(string packageFullName, int reserved, out IntPtr packageInfoReference);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPackageInfo(IntPtr packageInfoReference, PackageConstants flags, ref int bufferLength, IntPtr buffer, out int count);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int ClosePackageInfo(IntPtr packageInfoReference);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPackageFullName(IntPtr hProcess, ref int packageFullNameLength, StringBuilder packageFullName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetApplicationUserModelId(IntPtr hProcess, ref int applicationUserModelIdLength, StringBuilder applicationUserModelId);

        [Flags]
        private enum PackageConstants
        {
            PACKAGE_FILTER_ALL_LOADED = 0x00000000,
            PACKAGE_PROPERTY_FRAMEWORK = 0x00000001,
            PACKAGE_PROPERTY_RESOURCE = 0x00000002,
            PACKAGE_PROPERTY_BUNDLE = 0x00000004,
            PACKAGE_FILTER_HEAD = 0x00000010,
            PACKAGE_FILTER_DIRECT = 0x00000020,
            PACKAGE_FILTER_RESOURCE = 0x00000040,
            PACKAGE_FILTER_BUNDLE = 0x00000080,
            PACKAGE_INFORMATION_BASIC = 0x00000000,
            PACKAGE_INFORMATION_FULL = 0x00000100,
            PACKAGE_PROPERTY_DEVELOPMENT_MODE = 0x00010000,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct PACKAGE_INFO
        {
            public int reserved;
            public int flags;
            public IntPtr path;
            public IntPtr packageFullName;
            public IntPtr packageFamilyName;
            public PACKAGE_ID packageId;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        private struct PACKAGE_ID
        {
            public int reserved;
            public AppxPackageArchitecture processorArchitecture;
            public ushort VersionRevision;
            public ushort VersionBuild;
            public ushort VersionMinor;
            public ushort VersionMajor;
            public IntPtr name;
            public IntPtr publisher;
            public IntPtr resourceId;
            public IntPtr publisherId;
        }
    }

    public sealed class AppxApp
    {
        private AppxPackage.IAppxManifestApplication _app;

        internal AppxApp(AppxPackage.IAppxManifestApplication app)
        {
            _app = app;
        }

        public string GetStringValue(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return AppxPackage.GetStringValue(_app, name);
        }

        // we code well-known but there are others (like Square71x71Logo, Square44x44Logo, whatever ...)
        // https://msdn.microsoft.com/en-us/library/windows/desktop/hh446703.aspx
        public string Description { get; internal set; }
        public string DisplayName { get; internal set; }
        public string EntryPoint { get; internal set; }
        public string Executable { get; internal set; }
        public string Id { get; internal set; }
        public string Logo { get; internal set; }
        public string SmallLogo { get; internal set; }
        public string StartPage { get; internal set; }
        public string Square150x150Logo { get; internal set; }
        public string Square30x30Logo { get; internal set; }
        public string BackgroundColor { get; internal set; }
        public string ForegroundText { get; internal set; }
        public string WideLogo { get; internal set; }
        public string Wide310x310Logo { get; internal set; }
        public string ShortName { get; internal set; }
        public string Square310x310Logo { get; internal set; }
        public string Square70x70Logo { get; internal set; }
        public string MinWidth { get; internal set; }
    }

    public enum AppxPackageArchitecture
    {
        x86 = 0,
        Arm = 5,
        x64 = 9,
        Neutral = 11,
        Arm64 = 12
    }
}
