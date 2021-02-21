using System;
using System.Diagnostics;
using System.IO;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Core.AppInformation
{
    public abstract class SourceAppInfo : IDisposable
    {
        private bool disposedValue;

        #region Properties

        public Stream LogoStream { get; protected set; }

        public string DisplayName { get; protected set; } = string.Empty;

        protected SourceAppInfoData Data { get; set; }

        #endregion

        public abstract event EventHandler InfoFetched;

        public static SourceAppInfo FromData(SourceAppInfoData data)
        {
            if (data.DataType == SourceAppInfoDataType.FromAppUserModelId)
            {
                if (string.IsNullOrEmpty(data.AppUserModelId)
                    || string.IsNullOrWhiteSpace(data.AppUserModelId))
                    return null;

                if (IsAppUnpackaged(data))
                {
                    return new SourceDesktopAppInfo(data);
                }
                else
                {
                    return new SourceModernAppInfo(data);
                }
            }
            else if (data.DataType == SourceAppInfoDataType.FromProcessId)
            {
                if (data.ProcessId == 0)
                    return null;

                if (IsAppPackaged(data))
                {
                    return new SourceModernAppInfo(data);
                }
                else
                {
                    return new SourceDesktopAppInfo(data);
                }
            }

            return null;
        }

        public static SourceAppInfo FromAppUserModelId(string appUserModelId)
        {
            SourceAppInfoData data = new()
            {
                AppUserModelId = appUserModelId,
                DataType = SourceAppInfoDataType.FromAppUserModelId
            };

            return FromData(data);
        }

        public static SourceAppInfo FromProcessId(uint processId, IntPtr hWnd = default)
        {
            SourceAppInfoData data = new()
            {
                ProcessId = processId,
                MainWindowHandle = hWnd,
                DataType = SourceAppInfoDataType.FromProcessId
            };

            return FromData(data);
        }

        public abstract void Activate();

        public abstract void FetchInfosAsync();

        protected virtual void Disconnect()
        {
        }

        internal static bool IsAppUnpackaged(SourceAppInfoData data)
        {
            return data.AppUserModelId.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);
        }

        internal static bool IsAppPackaged(SourceAppInfoData data)
        {
            using var process = Process.GetProcessById((int)data.ProcessId);
            return IsImmersiveProcess(process.Handle);
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                Disconnect();
                LogoStream?.Dispose();
                LogoStream = null;
                disposedValue = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
