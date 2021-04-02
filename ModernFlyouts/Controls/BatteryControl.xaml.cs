using DocumentFormat.OpenXml.Drawing.Charts;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using ModernFlyouts.Helpers;
using ModernFlyouts.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Windows.System.Power;

namespace ModernFlyouts.Controls
{
    /// <summary>
    /// Interaction logic for BatteryControl.xaml
    /// </summary>
    public partial class BatteryControl : System.Windows.Controls.UserControl
    {
        public float NominalCapacity;  //Measured in Watt hours
        public float RealCapacity;
        public float CurrentCapacity;

        public float ChargePercent;

        public float CurrentVoltage;

        public TimeSpan TimeToDischarge;

        public Core.Utilities.BatteryStatus Status;

        public float PowerChargeRate;
        public BatteryControl()
        {
            InitializeComponent();
            try
            {

                ///Commented out battery dll code since it doesnt work.
                
                // string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                //   RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true);
                //key.SetValue("Fluent Flyouts", System.Windows.Forms.Application.ExecutablePath.ToString());
                //System.Windows.MessageBox.Show("Started");
                //  lis.matrix.Clear();
                //string output = Fluent_Flyouts.Properties.Settings.Default.test;
                //List<List<int>> Product = JsonConvert.DeserializeObject<List<List<int>>>(output);

                if (Status == Core.Utilities.BatteryStatus.Unavailable)
                {
                    //  ChargeText.Content = "?";
                    ChgRate.Content = "?";
                    NomCap.Content = "?";
                    RealCap.Content = "?";

                    Voltage.Content = "?";

                }
                else
                {
                    //  ChargeText.Content = Math.Round(battery.ChargePercent) + "%";
                    if (Status == Core.Utilities.BatteryStatus.Charging)
                    {
                        ChargeRateLabel.Content = "Charge Rate:";
                        ChgRate.Content = (Math.Round(-PowerChargeRate * 10) / 10f) + " W";
                    }
                    else
                    {
                        ChargeRateLabel.Content = "Discharge Rate:";
                        ChgRate.Content = (Math.Round(-PowerChargeRate * 10) / 10f) + " W";
                    }
                    RealCap.Content = (Math.Round(RealCapacity * 10) / 10f) + $" Wh ({Math.Round((RealCapacity / NominalCapacity) * 100)}%)";
                    NomCap.Content = (Math.Round(NominalCapacity * 10) / 10f) + " Wh";
                    Voltage.Content = (Math.Round(CurrentVoltage * 100) / 100f) + " V";
                }
                addtolist();

                Test();

                RankGraph.Series = graph.Series;


                Vis.counter = 0;
                PowerManager.RemainingChargePercentChanged += PowerManager_RemainingChargePercentChanged;
                PowerManager.BatteryStatusChanged += PowerManager_RemainingChargePercentChanged;
                PowerManager.EnergySaverStatusChanged += PowerManager_RemainingChargePercentChanged;
            }
            catch
            {

            }
        }
        public void Update()
        {
            try
            {
                if (!(bool)WMIToolHelper.QuerySingle("root/WMI", "BatteryStatus", "Active"))
                {
                    Status = Core.Utilities.BatteryStatus.Unavailable;
                    return;
                }
                Status = Core.Utilities.BatteryStatus.Discharging;
                if ((bool)WMIToolHelper.QuerySingle("root/WMI", "BatteryStatus", "Charging"))
                    Status = Core.Utilities.BatteryStatus.Charging;
            }
            catch (Exception)
            {
                Status = Core.Utilities.BatteryStatus.Unavailable;
                return;
            }
            try
            {
                NominalCapacity = ((uint)WMIToolHelper.QuerySingle("root/WMI", "BatteryStaticData", "DesignedCapacity")) / 1000f;
            }
            catch { }

            try
            {
                RealCapacity = ((uint)WMIToolHelper.QuerySingle("root/WMI", "BatteryFullChargedCapacity", "FullChargedCapacity")) / 1000f;
            }
            catch { }
            try
            {
                CurrentCapacity = ((uint)WMIToolHelper.QuerySingle("root/WMI", "BatteryStatus", "RemainingCapacity")) / 1000f;
            }
            catch { }
            try
            {
                ChargePercent = CurrentCapacity / RealCapacity * 100f;
            }
            catch { }
            try
            {
                CurrentVoltage = ((uint)WMIToolHelper.QuerySingle("root/WMI", "BatteryStatus", "Voltage")) / 1000f;
            }
            catch { }
            try
            {
                TimeToDischarge = TimeSpan.FromSeconds((uint)WMIToolHelper.QuerySingle("root/WMI", "BatteryRuntime", "EstimatedRuntime"));
            }
            catch { }
            if (Status == Core.Utilities.BatteryStatus.Discharging)
            {
                PowerChargeRate = -((int)WMIToolHelper.QuerySingle("root/WMI", "BatteryStatus", "DischargeRate")) / 1000f;
            }
            else
            {
                PowerChargeRate = ((int)WMIToolHelper.QuerySingle("root/WMI", "BatteryStatus", "ChargeRate")) / 1000f;
            }
        }

        private void addtolist()
        {
            if (Lis.matrix[0][0] == 0) Lis.matrix.Clear();
            //Fluent_Flyouts.Properties.Settings.Default.test = "[[0,0]]";
            //Fluent_Flyouts.Properties.Settings.Default.Save();
            //System.Windows.MessageBox.Show(lis.output.ToString());
            // System.Windows.MessageBox.Show(lis.matrix.Count.ToString());
            var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            var secondsSinceEpoch = (int)t.TotalSeconds;

            var count2 = Lis.matrix.Count;
            //System.Windows.MessageBox.Show(PowerManager.);
            if (Lis.matrix.Count > 0)
                if (Lis.matrix[count2 - 1][1] >= 90 && Lis.matrix[count2 - 1][1] > PowerManager.RemainingChargePercent)
                    Lis.matrix.Clear();
            var count = Lis.matrix.Count;

            Lis.matrix.Add(new List<int>());
            Lis.matrix[count].Add(secondsSinceEpoch);
            Lis.matrix[count].Add(PowerManager.RemainingChargePercent);



            Lis.final = secondsSinceEpoch + SystemInformation.PowerStatus.BatteryLifeRemaining;

            var output = JsonConvert.SerializeObject(Lis.matrix);

            ///  Properties.Settings.Default.output = output;
            ///  Properties.Settings.Default.Save();

            Debug.WriteLine(output);
            Test();
        }

        private static void Test()
        {
            graph.Series[0].Values.Clear();
            graph.Series[1].Values.Clear();

            for (int i = 0; i < Lis.matrix.Count; i++)
            {
                int x = Lis.matrix[i][0];
                int y = Lis.matrix[i][1];
                graph.Series[0].Values.Add(new ObservablePoint(x, y));
            }

            graph.Series[1].Values
                .Add(new ObservablePoint(Lis.matrix[Lis.matrix.Count - 1][0], Lis.matrix[Lis.matrix.Count - 1][1]));
            graph.Series[1].Values.Add(new ObservablePoint(Lis.final, 0));
        }
        //Window code
        /* private void Window_Loaded(object sender, EventArgs e)
         {
             try
             {
                 Powerrefresh();
                 Vis.counter = 0;
                 // System.Windows.MessageBox.Show("Start");
                 //percentage.Text = status.BatteryLifePercent.ToString("P0");
                 //timeremaining.Text = status.BatteryLifeRemaining.ToString();
                 var desktopWorkingArea = SystemParameters.WorkArea;
                 Left = desktopWorkingArea.Right - Width;
                 Top = desktopWorkingArea.Bottom - Height;
                 //RefreshStatus;
                 //DispatcherTimer timer = new DispatcherTimer();
                 // timer.Interval = TimeSpan.FromSeconds(5);
                 // timer.Tick += RefreshStatus;
                 // timer.Start();
             }
             catch
             {
             }
         }
         private void Window_Deactivated(object sender, EventArgs e)
         {
             Dispatcher.Invoke(() => { Close(); });
             //  Vis.counter = 1;
         }*/


        private void PowerManager_RemainingChargePercentChanged(object sender, object e)
        {
            Debug.WriteLine("perchange");
            addtolist();
            Powerrefresh();
        }
     //   Battery battery;
        public float[] ChargeRate = new float[100];
        public float[] CurrentCharge = new float[100];
        [DllImport("Kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern void GetSystemPowerStatus();
        public void Powerrefresh()
        {
            try
            {
                //Process.Start("cmd", "c:\windows\downloaded program files\cmd.exe echo hi && pause");

                if (Status == Core.Utilities.BatteryStatus.Unavailable)
                {
                    //  ChargeText.Content = "?";
                    ChgRate.Content = "?";
                    NomCap.Content = "?";
                    RealCap.Content = "?";

                    Voltage.Content = "?";

                }
                else
                {
                    //  ChargeText.Content = Math.Round(battery.ChargePercent) + "%";
                    if (Status == Core.Utilities.BatteryStatus.Charging)
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            // your code here.

                            ChargeRateLabel.Content = "Charge Rate:";

                            ChgRate.Content = (Math.Round(PowerChargeRate * 10) / 10f) + " W";


                        });
                    }
                    else
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            ChargeRateLabel.Content = "Discharge Rate:";

                            ChgRate.Content = (Math.Round(-PowerChargeRate * 10) / 10f) + " W";


                        });
                    }
                    this.Dispatcher.Invoke(() =>
                    {
                        RealCap.Content = (Math.Round(RealCapacity * 10) / 10f) + $" Wh ({Math.Round((RealCapacity / NominalCapacity) * 100)}%)";
                        NomCap.Content = (Math.Round(NominalCapacity * 10) / 10f) + " Wh";
                        Voltage.Content = (Math.Round(CurrentVoltage * 100) / 100f) + " V";
                    });
                }

                var power = SystemInformation.PowerStatus;
                Test();
                //Series[0].Values.Add(new ObservablePoint(160, 80));

                //System.Windows.MessageBox.Show(power.PowerLineStatus.ToString());

                // Battery Remaining
                int th;
                int tmin;
                string hval;
                string tval;

                var tsince = TimeSpan.FromSeconds(Lis.matrix[Lis.matrix.Count - 1][0] - Lis.matrix[0][0]);
                var ttime = tsince.TotalHours.ToString().Split('.').ToList();

                if (tsince.TotalHours < 1)
                {
                    Dispatcher.Invoke(() => { timesince.Text = tsince.Minutes + " minutes ago"; });
                }
                else
                {
                    th = int.Parse(ttime[0]);
                    if (ttime.Count == 2)
                        tmin = (int)(Convert.ToDouble("." + ttime[1]) * 60);
                    else
                        tmin = 0;

                    if (th == 1)
                        hval = " hour ";
                    else
                        hval = " hours ";
                    if (tmin == 1)
                        tval = " minute ";
                    else
                        tval = " minutes ";
                    Dispatcher.Invoke(() =>
                    {
                        if (tmin == 0)
                            timesince.Text = th + hval + "ago";
                        else
                            timesince.Text = th + hval + tmin + tval + "ago";
                        // timeremaining.Text = string.Format("{0:00}h {1:00}m", (int)tremaining.TotalHours, tremaining.Minutes);
                    });
                }

                ttime.Clear();


                var secondsRemaining = power.BatteryLifeRemaining;
                if (secondsRemaining >= 0)
                {
                    var tremaining = TimeSpan.FromSeconds(secondsRemaining);
                    //   System.Windows.MessageBox.Show(tremaining.TotalHours.ToString());
                    if (tremaining.TotalHours < 1)
                    {
                        Dispatcher.Invoke(() => { timeremaining.Text = tremaining.Minutes + " minutes"; });
                    }
                    else
                    {
                        ttime = tremaining.TotalHours.ToString().Split('.').ToList();
                        th = int.Parse(ttime[0]);
                        if (ttime.Count == 2)
                            tmin = (int)(Convert.ToDouble("." + ttime[1]) * 60);
                        else
                            tmin = 0;

                        if (th == 1)
                            hval = " hour ";
                        else
                            hval = " hours ";
                        if (tmin == 1)
                            tval = " minute ";
                        else
                            tval = " minutes ";
                        Dispatcher.Invoke(() =>
                        {
                            if (tmin == 0)
                                timeremaining.Text = th + hval + "remaining";
                            else
                                timeremaining.Text = th + hval + tmin + tval + "remaining";
                            // timeremaining.Text = string.Format("{0:00}h {1:00}m", (int)tremaining.TotalHours, tremaining.Minutes);
                        });
                    }

                    //timeremaining.Text = string.Format("{0} min", secondsRemaining / 60);
                    //timeremaining.Text = string.Format("{0} sec", secondsRemaining);
                }
                else
                {
                    if (PowerManager.RemainingChargePercent == 100)
                        Dispatcher.Invoke(() =>
                        {
                            var margin = timeremaining.Margin;
                            margin.Left = 200;
                            timeremaining.Margin = margin;
                            if (SystemInformation.PowerStatus.PowerLineStatus.ToString() == "Online")
                                timeremaining.Text = "Plugged In\nFully Charged";
                            else
                                timeremaining.Text = "Fully Charged";
                        });
                    else
                        Dispatcher.Invoke(() =>
                        {
                            var margin = timeremaining.Margin;
                            margin.Left = 179;
                            timeremaining.Margin = margin;
                            timeremaining.Text = "Plugged In\nCharging";
                        });

                    var powerval2 = power.BatteryChargeStatus.ToString().Split(',').ToList();
                    //System.Windows.MessageBox.Show(powerval2[1].ToString());
                    if (powerval2.Count == 1 && SystemInformation.PowerStatus.PowerLineStatus.ToString() != "Online")
                        Dispatcher.Invoke(() => { timeremaining.Text = "Discharging"; });
                }

                // Battery Status
                // percentage.Text = power.BatteryChargeStatus.ToString();
                var powerval = power.BatteryChargeStatus.ToString().Split(',').ToList();
                string[] nbat =
                {
                "\uE850", "\uE851", "\uE852", "\uE853", "\uE854", "\uE855", "\uE856", "\uE857", "\uE858", "\uE859", "\uE83F"
            };
                string[] cbat =
                {
                "\uE85A", "\uE85B", "\uE85C", "\uE85D", "\uE85E", "\uE85F", "\uE860", "\uE861", "\uE862", "\uE83E", "\uEA93"
            };
                string[] sbat =
                {
                "\uE863", "\uE864", "\uE865", "\uE866", "\uE867", "\uE868", "\uE869", "\uE86A", "\uE86B", "\uEA94", "\uEA95"
            };
                var bval = "nbat";

                if (PowerManager.EnergySaverStatus.ToString() == "On")
                    bval = "sbat";

                if (powerval.Count > 1 && powerval[1] == " Charging") bval = "cbat";
                if (SystemInformation.PowerStatus.PowerLineStatus.ToString() == "Online") bval = "cbat";


                decimal batper10 = PowerManager.RemainingChargePercent / 10;
                var batlvl = Convert.ToInt32(Math.Floor(batper10));

                Dispatcher.Invoke(() =>
                {
                    percentage.Text = PowerManager.RemainingChargePercent + "%";
                    if (bval == "nbat")
                        baticon.Text = nbat[batlvl];
                    else if (bval == "sbat")
                        baticon.Text = sbat[batlvl];
                    else if (bval == "cbat") baticon.Text = cbat[batlvl];
                });
            }
            catch
            {

            }
        }



        private static class Lis
        {
            /// public static readonly string output = Properties.Settings.Default.output;
            /// use [[0,0]] for now as default since no application settings
            public static readonly List<List<int>> matrix = JsonConvert.DeserializeObject<List<List<int>>>("[[0,0]]");



            public static int final;
        }

        private static class graph
        {

            public static readonly SeriesCollection Series = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Battery",
                    Values = new ChartValues<ObservablePoint>(),
                    PointGeometry = null
                },

                new LineSeries
                {
                    Title = "Estimate",
                    Values = new ChartValues<ObservablePoint>(),
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Gray,
                    StrokeDashArray = new DoubleCollection {2},
                    PointGeometry = null
                }
            };
        }

        private static class Vis
        {
            public static int counter;
        }

        private readonly ProcessStartInfo _startInfo = new ProcessStartInfo("cmd")
        {
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == 3)
            {
                var process = new Process { StartInfo = _startInfo };
                process.Start();
                process.StandardInput.WriteLine("powercfg /SETACTIVE " + "8c5e7fda-e8bf-4a96-9a85-a6e23a8c635c");
                process.StandardInput.WriteLine("exit");
                process.StandardOutput.ReadToEnd();
                process.Dispose();
            }
            else if (e.NewValue == 2)
            {
                var process = new Process { StartInfo = _startInfo };
                process.Start();
                process.StandardInput.WriteLine("powercfg /SETACTIVE " + "381b4222-f694-41f0-9685-ff5bb260df2e");
                process.StandardInput.WriteLine("exit");
                process.StandardOutput.ReadToEnd();
                process.Dispose();
            }
            else
            {
                var process = new Process { StartInfo = _startInfo };
                process.Start();
                process.StandardInput.WriteLine("powercfg /SETACTIVE " + "a1841308-3541-4fab-bc81-f71556f20b4a");
                process.StandardInput.WriteLine("exit");
                process.StandardOutput.ReadToEnd();
                process.Dispose();
            }
        }
    }
}