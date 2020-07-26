using System;
using System.Diagnostics;
using System.Windows;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using System.Text;

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

        public override void Activate()
        {
            ActivateAsync();
        }

        private async void ActivateAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    var args = @"shell:appsFolder\" + AppId;
                    ProcessStartInfo processStartInfo = new ProcessStartInfo() { FileName = "explorer.exe", Arguments = args, UseShellExecute = true };
                    Process.Start(processStartInfo);
                }
                catch { }
            });
        }

        private async void FetchInfos()
        {
            string logoPath = string.Empty;
            await Task.Run(() =>
            {
                foreach (var p in Process.GetProcesses())
                {
                    var package = AppxPackage.FromProcess(p);
                    if (package != null)
                    {
                        if (package.ApplicationUserModelId == AppId)
                        {
                            logoPath = package.FindHighestScaleQualifiedImagePath(package.Logo);
                            AppName = GetProperDisplayName(package);

                            break;
                        }
                    }
                }
            });

            if (File.Exists(logoPath))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    AppImage = new BitmapImage(new Uri(logoPath));
                }, System.Windows.Threading.DispatcherPriority.Send);
            }

            InfoFetched?.Invoke(this, null);
        }

        public static string GetProperDisplayName(AppxPackage package)
        {
            if (!package.DisplayName.Contains("ms-resource"))
            {
                return package.DisplayName;
            }

            StringBuilder sb = new StringBuilder();
            int result;

            result = AppxPackage.SHLoadIndirectString(
                @"@{" + Path.GetFileName(package.Path) + "? ms-resource://" + package.ResourceId + "/resources/" + package.DisplayName.Split(':')[1] + "}",
                sb, -1,
                IntPtr.Zero
            );

            if (result == 0)
            {
                return sb.ToString();
            }

            return string.Empty;
        }
    }
}
