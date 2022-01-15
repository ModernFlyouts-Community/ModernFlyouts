using Windows.Devices.Power;
using Fluent.Icons;

namespace ModernFlyouts.Uwp.Helpers
{
    public class BatteryHelper
    {
        /// <summary>
        /// Converts BatterReport info into a percentage value of battery remaining.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a percentage value of the battery as a string with %.</returns>
        public static string GetPercentageText(BatteryReport report) => GetPercentage(report).ToString("F0") + "%";

        /// <summary>
        /// Gets an icon that represents current battery level.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a FluentSymbol representing battery level./returns>
        public static FluentSymbol GetPercentageIcon(BatteryReport report)
        {
            return ((int)GetPercentage(report)) switch
            {
                int i when i > 0 && i <= 10 => FluentSymbol.BatteryWarning24,
                int i when i > 10 && i <= 20 => FluentSymbol.Battery124,
                int i when i > 20 && i <= 30 => FluentSymbol.Battery224,
                int i when i > 30 && i <= 40 => FluentSymbol.Battery324,
                int i when i > 40 && i <= 50 => FluentSymbol.Battery424,
                int i when i > 50 && i <= 60 => FluentSymbol.Battery524,
                int i when i > 60 && i <= 70 => FluentSymbol.Battery624,
                int i when i > 70 && i <= 80 => FluentSymbol.Battery724,
                int i when i > 80 && i <= 90 => FluentSymbol.Battery824,
                int i when i > 90 && i < 99 => FluentSymbol.Battery924,
                _ => FluentSymbol.BatteryFull24
            };
        }

        /// <summary>
        /// Converts BatterReport info into a percentage value of battery remaining.
        /// </summary>
        /// <param name="report">A BatteryReport object.</param>
        /// <returns>Returns a percentage value of the battery as a double./returns>
        public static double GetPercentage(BatteryReport report)
        {
            return (((double)report.RemainingCapacityInMilliwattHours / (double)report.FullChargeCapacityInMilliwattHours) * 100);
        }
    }
}

// Bluetooth device battery
// BT-LE
// https://docs.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile?view=winrt-22000
// https://github.com/microsoft/Windows-universal-samples/blob/main/Samples/BluetoothLE/cs/DisplayHelpers.cs

//BT -Classic
// https://github.com/SpartanX1/bluetooth_classic_battery_windows

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        Console.WriteLine("Available paired devices..");
//        DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));

//        if (devices.Count == 0)
//        {
//            Console.WriteLine("No devices found");
//            Exit();
//        }

//        for (int i = 0; i < devices.Count; i++)
//        {
//            Console.WriteLine(String.Format("{0}. {1}\t{2}", i, devices[i].Name, devices[i].Id));
//        }

//        Console.WriteLine("Select a device to connect..");
//        int selectedDevice = Int32.Parse(Console.ReadLine());

//        BluetoothDevice blDevice = await BluetoothDevice.FromIdAsync(devices[selectedDevice].Id);

//        if (blDevice != null)
//        {
//            Console.WriteLine("Discovering Rfcomm services..");
//            RfcommDeviceServicesResult rfcommResult = await blDevice.GetRfcommServicesAsync();
//            if (rfcommResult.Services.Count == 0)
//            {
//                Console.WriteLine("No services found");
//                Exit();
//            }
//            for (int i = 0; i < rfcommResult.Services.Count; i++)
//            {
//                Console.WriteLine(String.Format("{0}. {1}", i, rfcommResult.Services[i].ServiceId.Uuid));
//            }
//            Console.WriteLine("Select a service to connect..");
//            int selectedService = Int32.Parse(Console.ReadLine());

//            try
//            {
//                StreamSocket socket = new StreamSocket();
//                await socket.ConnectAsync(rfcommResult.Services[selectedService].ConnectionHostName, rfcommResult.Services[selectedService].ConnectionServiceName);
//                Console.WriteLine("Connected to service: " + rfcommResult.Services[selectedService].ServiceId.Uuid);

//                CancellationTokenSource source = new CancellationTokenSource();
//                CancellationToken cancelToken = source.Token;
//                Task listenOnChannel = new TaskFactory().StartNew(async () =>
//                {
//                    while (true)
//                    {
//                        if (cancelToken.IsCancellationRequested)
//                        {
//                            return;
//                        }
//                        await ReadWrite.Read(socket, source);
//                    }
//                }, cancelToken);

//                Console.ReadKey();
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("Could not connect to service " + e.Message);
//                Exit();
//            }
//        }
//        else
//        {
//            Exit();
//        }
//    }

//    static void Exit()
//    {
//        Console.ReadKey();
//        Environment.Exit(1);
//    }
//}

//static class ReadWrite
//{
//    public static async Task Read(StreamSocket socket, CancellationTokenSource source)
//    {
//        IBuffer buffer = new Windows.Storage.Streams.Buffer(1024);
//        uint bytesRead = 1024;

//        IBuffer result = await socket.InputStream.ReadAsync(buffer, bytesRead, InputStreamOptions.Partial);
//        await Write(socket, "OK");

//        DataReader reader = DataReader.FromBuffer(result);
//        var output = reader.ReadString(result.Length);

//        if (output.Length != 0)
//        {
//            Console.WriteLine("Recieved :" + output.Replace("\r", " "));
//            if (output.Contains("IPHONEACCEV"))
//            {
//                try
//                {
//                    var batteryCmd = output.Substring(output.IndexOf("IPHONEACCEV"));
//                    Console.WriteLine("Battery level :" + (Int32.Parse(batteryCmd.Substring(batteryCmd.LastIndexOf(",") + 1)) + 1) * 10);
//                    source.Cancel();
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("Could not retrieve " + e.Message);
//                }
//            }
//        }
//    }

//    public static async Task Write(StreamSocket socket, string str)
//    {
//        var bytesWrite = CryptographicBuffer.ConvertStringToBinary("\r\n" + str + "\r\n", BinaryStringEncoding.Utf8);
//        await socket.OutputStream.WriteAsync(bytesWrite);
//    }
//}