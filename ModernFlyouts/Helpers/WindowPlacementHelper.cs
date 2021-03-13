using ModernFlyouts.Core.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using static ModernFlyouts.Core.Interop.NativeMethods;

namespace ModernFlyouts.Helpers
{
    public static class WindowPlacementHelper
    {
        private static readonly Encoding encoding = new UTF8Encoding();
        private static readonly XmlSerializer serializer = new(typeof(WINDOWPLACEMENT));

        public static void SetPlacement(IntPtr windowHandle, string placementXml)
        {
            if (string.IsNullOrEmpty(placementXml))
            {
                return;
            }

            WINDOWPLACEMENT placement;
            byte[] xmlBytes = encoding.GetBytes(placementXml);

            try
            {
                using (MemoryStream memoryStream = new(xmlBytes))
                {
                    placement = (WINDOWPLACEMENT)serializer.Deserialize(memoryStream);
                }

                placement.Length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                placement.Flags = 0;
                placement.ShowCmd = placement.ShowCmd == ShowWindowCommands.ShowMinimized ? ShowWindowCommands.Normal : placement.ShowCmd;
                SetWindowPlacement(windowHandle, ref placement);
            }
            catch (InvalidOperationException)
            {
                // Parsing placement XML failed. Fail silently.
            }
        }

        public static string GetPlacement(IntPtr windowHandle)
        {
            WINDOWPLACEMENT placement = new();
            GetWindowPlacement(windowHandle, out placement);

            using MemoryStream memoryStream = new();
            using XmlTextWriter xmlTextWriter = new(memoryStream, Encoding.UTF8);
            serializer.Serialize(xmlTextWriter, placement);
            byte[] xmlBytes = memoryStream.ToArray();
            return encoding.GetString(xmlBytes);
        }
    }
}
