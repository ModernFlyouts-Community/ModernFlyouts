using System;
using System.Diagnostics;
using System.Windows.Media;

namespace ModernFlyouts
{
    public abstract class SourceAppInfo
    {
        #region Properties

        public ImageSource AppImage { get; protected set; }

        public string AppName { get; protected set; } = string.Empty;

        protected string AppId { get; set; }

        #endregion

        public abstract event EventHandler InfoFetched;

        public static SourceAppInfo FromAppId(string appId)
        {
            if (string.IsNullOrEmpty(appId) || string.IsNullOrWhiteSpace(appId))
            {
                return null;
            }

            if (IsAppWin32(appId))
            {
                return new SourceDesktopAppInfo(appId);
            }
            else
            {
                return new SourceModernAppInfo(appId);
            }
        }

        public abstract void Activate();

        internal static bool IsAppWin32(string appId)
        {
            Debug.WriteLine($"{nameof(SourceAppInfo)}: appId:{appId}");
            return appId.EndsWith(".exe", StringComparison.OrdinalIgnoreCase);
        }
    }
}
