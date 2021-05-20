using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Windows.System.Power;
using System.ComponentModel;
using System.Drawing.Text;
using FontStyle = System.Drawing.FontStyle;
using Color = System.Drawing.Color;
using Brush = System.Drawing.Brush;

namespace ModernFlyouts.Navigation
{
    /// <summary>
    /// Interaction logic for BatteryModulePage.xaml
    /// </summary>
    public partial class BatteryModulePage : Page
    {
        private bool _isExit;
        private NotifyIcon _notifyIcon;
        Window batteryWindow;
        Color textcolor;
        public BatteryModulePage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            _notifyIcon = new NotifyIcon();
            var str = PowerManager.RemainingChargePercent.ToString();
            ModernWpf.Controls.AppBarButton a = new ModernWpf.Controls.AppBarButton();
            if (ModernWpf.ThemeManager.Current.ActualApplicationTheme == ModernWpf.ApplicationTheme.Dark)
            {
                textcolor = Color.White;
            }
            else if (ModernWpf.ThemeManager.Current.ActualApplicationTheme == ModernWpf.ApplicationTheme.Light)
            {
                textcolor = Color.Black;
            }
            if (PowerManager.RemainingChargePercent != 100)
            {
                var fontToUse = new Font("Segoe UI", 16, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brushToUse = new SolidBrush(textcolor);
                var bitmapText = new Bitmap(16, 16);
                var g = Graphics.FromImage(bitmapText);

                IntPtr hIcon;

                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString(str, fontToUse, brushToUse, -4, -2);
                hIcon = bitmapText.GetHicon();
                _notifyIcon.Icon = Icon.FromHandle(hIcon);
            }
            else
            {
                var fontToUse = new Font("Trebuchet MS", 12, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brushToUse = new SolidBrush(textcolor);
                var bitmapText = new Bitmap(16, 16);
                var g = Graphics.FromImage(bitmapText);

                IntPtr hIcon;

                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString(str, fontToUse, brushToUse, -3, 0);
                hIcon = bitmapText.GetHicon();
                _notifyIcon.Icon = Icon.FromHandle(hIcon);
            }
            batteryWindow = new BatteryWindow();
            batteryWindow.Closing += batteryWindow_Closing;


            _notifyIcon.MouseDown += (s, args) => ShowbatteryWindow(s, args);

            //_notifyIcon.Icon = Fluent_Flyouts.Properties.Resources.ea93_Sm6_icon;
            _notifyIcon.Visible = true;

            CreateContextMenu();
            PowerManager.RemainingChargePercentChanged += ChargeChange;
            tooltipchange();
        }

        private void tooltipchange()
        {
            int th;
            int tmin;
            var power = SystemInformation.PowerStatus;
            var secondsRemaining = power.BatteryLifeRemaining;
            if (secondsRemaining >= 0)
            {
                var tremaining = TimeSpan.FromSeconds(secondsRemaining);
                //   System.Windows.MessageBox.Show(tremaining.TotalHours.ToString());
                if (tremaining.TotalHours < 1)
                {
                    Dispatcher.Invoke(() =>
                    {
                        _notifyIcon.Text = tremaining.Minutes + " minutes (" + PowerManager.RemainingChargePercent +
                                           "%) remaining";
                    });
                }
                else
                {
                    var ttime = tremaining.TotalHours.ToString().Split('.').ToList();
                    ttime = tremaining.TotalHours.ToString().Split('.').ToList();
                    th = int.Parse(ttime[0]);
                    if (ttime.Count == 2)
                        tmin = (int)(Convert.ToDouble("." + ttime[1]) * 60);
                    else
                        tmin = 0;


                    Dispatcher.Invoke(() =>
                    {
                        _notifyIcon.Text = th + " hr " + tmin + " min (" + PowerManager.RemainingChargePercent +
                                           "%) remaining";

                        // timeremaining.Text = string.Format("{0:00}h {1:00}m", (int)tremaining.TotalHours, tremaining.Minutes);
                    });
                }
            }
        }

        public void ChargeChange(object sender, object e)
        {
            var str = PowerManager.RemainingChargePercent.ToString();
            if (PowerManager.RemainingChargePercent != 100)
            {
                var fontToUse = new Font("Microsoft Sans Serif", 16, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brushToUse = new SolidBrush(textcolor);
                var bitmapText = new Bitmap(16, 16);
                var g = Graphics.FromImage(bitmapText);

                IntPtr hIcon;

                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString(str, fontToUse, brushToUse, -4, -2);
                hIcon = bitmapText.GetHicon();
                _notifyIcon.Icon = Icon.FromHandle(hIcon);
            }
            else
            {
                var fontToUse = new Font("Trebuchet MS", 12, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush brushToUse = new SolidBrush(textcolor);
                var bitmapText = new Bitmap(16, 16);
                var g = Graphics.FromImage(bitmapText);

                IntPtr hIcon;

                g.Clear(Color.Transparent);
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString(str, fontToUse, brushToUse, -3, 0);
                hIcon = bitmapText.GetHicon();
                _notifyIcon.Icon = Icon.FromHandle(hIcon);
            }

            // _notifyIcon.Text = "ToolTipText";
            tooltipchange();
        }

        public void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
                new ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) =>
            {
                _notifyIcon.Visible = false;
            };

        }

        private void ExitApplication()
        {
            _isExit = true;
            batteryWindow.Hide();
            //batteryWindow.
            // System.Windows.Forms.Application.Exit();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }
        private void ShowbatteryWindow(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button.ToString() == "Left")
            {
                //System.Windows.MessageBox.Show(e.Button.ToString());
                if (batteryWindow.IsVisible)
                {
                    if (batteryWindow.WindowState == WindowState.Minimized) batteryWindow.WindowState = WindowState.Normal;
                    batteryWindow.Activate();
                }
                else
                {
                    batteryWindow.Show();
                    batteryWindow.Activate();
                }
            }
        }

        private void batteryWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                batteryWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }
    }
}