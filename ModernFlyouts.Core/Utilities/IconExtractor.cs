/*
 *  IconExtractor/IconUtil for .NET
 *  Copyright (C) 2014 Tsuda Kageyu. All rights reserved.
 *
 *  Redistribution and use in source and binary forms, with or without
 *  modification, are permitted provided that the following conditions
 *  are met:
 *
 *   1. Redistributions of source code must retain the above copyright
 *      notice, this list of conditions and the following disclaimer.
 *   2. Redistributions in binary form must reproduce the above copyright
 *      notice, this list of conditions and the following disclaimer in the
 *      documentation and/or other materials provided with the distribution.
 *
 *  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 *  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED
 *  TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 *  PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER
 *  OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 *  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 *  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 *  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 *  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 *  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 *  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace ModernFlyouts.Core.Utilities
{
    public sealed class IconExtractor
    {
        ////////////////////////////////////////////////////////////////////////
        // Constants

        // Flags for LoadLibraryEx().

        private const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;

        // Resource types for EnumResourceNames().

        private readonly static IntPtr RT_ICON = (IntPtr)3;
        private readonly static IntPtr RT_GROUP_ICON = (IntPtr)14;

        private const int MAX_PATH = 260;

        ////////////////////////////////////////////////////////////////////////
        // Fields

        private byte[][] iconData;   // Binary data of each icon.

        ////////////////////////////////////////////////////////////////////////
        // Public properties

        /// <summary>
        /// Gets the full path of the associated file.
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of the icons in the associated file.
        /// </summary>
        public int Count
        {
            get { return iconData.Length; }
        }

        /// <summary>
        /// Initializes a new instance of the IconExtractor class from the specified file name.
        /// </summary>
        /// <param name="fileName">The file to extract icons from.</param>
        public IconExtractor(string fileName)
        {
            Initialize(fileName);
        }

        /// <summary>
        /// Extracts an icon from the file.
        /// </summary>
        /// <param name="index">Zero based index of the icon to be extracted.</param>
        /// <returns>A System.Drawing.Icon object.</returns>
        /// <remarks>Always returns new copy of the Icon. It should be disposed by the user.</remarks>
        public Icon GetIcon(int index)
        {
            if (index < 0 || Count <= index)
                throw new ArgumentOutOfRangeException(nameof(index));

            // Create an Icon based on a .ico file in memory.

            using var ms = new MemoryStream(iconData[index]);
            return new Icon(ms);
        }

        /// <summary>
        /// Extracts all the icons from the file.
        /// </summary>
        /// <returns>An array of System.Drawing.Icon objects.</returns>
        /// <remarks>Always returns new copies of the Icons. They should be disposed by the user.</remarks>
        public Icon[] GetAllIcons()
        {
            var icons = new List<Icon>();
            for (int i = 0; i < Count; ++i)
                icons.Add(GetIcon(i));

            return icons.ToArray();
        }

        private void Initialize(string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));

            IntPtr hModule = IntPtr.Zero;
            try
            {
                hModule = NativeMethods.LoadLibraryEx(fileName, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE);
                if (hModule == IntPtr.Zero)
                    throw new Win32Exception();

                FileName = GetFileName(hModule);

                // Enumerate the icon resource and build .ico files in memory.

                var tmpData = new List<byte[]>();

                bool callback(IntPtr h, IntPtr t, IntPtr name, IntPtr l)
                {
                    // Refer the following URL for the data structures used here:
                    // http://msdn.microsoft.com/en-us/library/ms997538.aspx

                    // RT_GROUP_ICON resource consists of a GRPICONDIR and GRPICONDIRENTRY's.

                    var dir = GetDataFromResource(hModule, RT_GROUP_ICON, name);

                    // Calculate the size of an entire .icon file.

                    int count = BitConverter.ToUInt16(dir, 4);  // GRPICONDIR.idCount
                    int len = 6 + 16 * count;                   // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count
                    for (int i = 0; i < count; ++i)
                        len += BitConverter.ToInt32(dir, 6 + 14 * i + 8);   // GRPICONDIRENTRY.dwBytesInRes

                    using (var dst = new BinaryWriter(new MemoryStream(len)))
                    {
                        // Copy GRPICONDIR to ICONDIR.

                        dst.Write(dir, 0, 6);

                        int picOffset = 6 + 16 * count; // sizeof(ICONDIR) + sizeof(ICONDIRENTRY) * count

                        for (int i = 0; i < count; ++i)
                        {
                            // Copy GRPICONDIRENTRY to ICONDIRENTRY.

                            dst.Seek(6 + 16 * i, SeekOrigin.Begin);

                            dst.Write(dir, 6 + 14 * i, 12);  // First 12bytes are identical.
                            dst.Write(picOffset);               // Write offset instead of ID.

                            // Copy a picture.

                            dst.Seek(picOffset, SeekOrigin.Begin);

                            ushort id = BitConverter.ToUInt16(dir, 6 + 14 * i + 12);    // GRPICONDIRENTRY.nID
                            var pic = GetDataFromResource(hModule, RT_ICON, (IntPtr)id);

                            dst.Write(pic, 0, pic.Length);

                            picOffset += pic.Length;
                        }

                        tmpData.Add(((MemoryStream)dst.BaseStream).ToArray());
                    }

                    return true;
                }
                NativeMethods.EnumResourceNames(hModule, RT_GROUP_ICON, callback, IntPtr.Zero);

                iconData = tmpData.ToArray();
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                    NativeMethods.FreeLibrary(hModule);
            }
        }

        private static byte[] GetDataFromResource(IntPtr hModule, IntPtr type, IntPtr name)
        {
            // Load the binary data from the specified resource.

            IntPtr hResInfo = NativeMethods.FindResource(hModule, name, type);
            if (hResInfo == IntPtr.Zero)
                throw new Win32Exception();

            IntPtr hResData = NativeMethods.LoadResource(hModule, hResInfo);
            if (hResData == IntPtr.Zero)
                throw new Win32Exception();

            IntPtr pResData = NativeMethods.LockResource(hResData);
            if (pResData == IntPtr.Zero)
                throw new Win32Exception();

            uint size = NativeMethods.SizeofResource(hModule, hResInfo);
            if (size == 0)
                throw new Win32Exception();

            byte[] buf = new byte[size];
            Marshal.Copy(pResData, buf, 0, buf.Length);

            return buf;
        }

        private static string GetFileName(IntPtr hModule)
        {
            // Alternative to GetModuleFileName() for the module loaded with
            // LOAD_LIBRARY_AS_DATAFILE option.

            // Get the file name in the format like:
            // "\\Device\\HarddiskVolume2\\Windows\\System32\\shell32.dll"

            string fileName;
            {
                var buf = new StringBuilder(MAX_PATH);
                int len = NativeMethods.GetMappedFileName(
                    NativeMethods.GetCurrentProcess(), hModule, buf, buf.Capacity);
                if (len == 0)
                    throw new Win32Exception();

                fileName = buf.ToString();
            }

            // Convert the device name to drive name like:
            // "C:\\Windows\\System32\\shell32.dll"

            for (char c = 'A'; c <= 'Z'; ++c)
            {
                var drive = c + ":";
                var buf = new StringBuilder(MAX_PATH);
                int len = NativeMethods.QueryDosDevice(drive, buf, buf.Capacity);
                if (len == 0)
                    continue;

                var devPath = buf.ToString();
                if (fileName.StartsWith(devPath))
                    return drive + fileName[devPath.Length..];
            }

            return fileName;
        }

        #region NativeMethods

        private static class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            [SuppressUnmanagedCodeSecurity]
            public static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

            [DllImport("kernel32.dll", SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            [SuppressUnmanagedCodeSecurity]
            public static extern bool EnumResourceNames(IntPtr hModule, IntPtr lpszType, ENUMRESNAMEPROC lpEnumFunc, IntPtr lParam);

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            [SuppressUnmanagedCodeSecurity]
            public static extern IntPtr FindResource(IntPtr hModule, IntPtr lpName, IntPtr lpType);

            [DllImport("kernel32.dll", SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            public static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

            [DllImport("kernel32.dll", SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            public static extern IntPtr LockResource(IntPtr hResData);

            [DllImport("kernel32.dll", SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            public static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

            [DllImport("kernel32.dll", SetLastError = true)]
            [SuppressUnmanagedCodeSecurity]
            public static extern IntPtr GetCurrentProcess();

            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            [SuppressUnmanagedCodeSecurity]
            public static extern int QueryDosDevice(string lpDeviceName, StringBuilder lpTargetPath, int ucchMax);

            [DllImport("psapi.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            [SuppressUnmanagedCodeSecurity]
            public static extern int GetMappedFileName(IntPtr hProcess, IntPtr lpv, StringBuilder lpFilename, int nSize);
        }

        [UnmanagedFunctionPointer(CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Unicode)]
        [SuppressUnmanagedCodeSecurity]
        internal delegate bool ENUMRESNAMEPROC(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);

        #endregion
    }

    public static class IconUtil
    {
        private delegate byte[] GetIconDataDelegate(Icon icon);

        private static readonly GetIconDataDelegate getIconData;

        static IconUtil()
        {
            // Create a dynamic method to access Icon.iconData private field.

            var dm = new DynamicMethod(
                "GetIconData", typeof(byte[]), new Type[] { typeof(Icon) }, typeof(Icon));

            var fi = typeof(Icon).GetField(
#if NETCOREAPP
                "_iconData",
#else
                "iconData",
#endif
                BindingFlags.Instance | BindingFlags.NonPublic);

            var gen = dm.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, fi);
            gen.Emit(OpCodes.Ret);

            getIconData = (GetIconDataDelegate)dm.CreateDelegate(typeof(GetIconDataDelegate));
        }

        /// <summary>
        /// Split an Icon consists of multiple icons into an array of Icon each
        /// consists of single icons.
        /// </summary>
        /// <param name="icon">A System.Drawing.Icon to be split.</param>
        /// <returns>An array of System.Drawing.Icon.</returns>
        public static Icon[] Split(Icon icon)
        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon));

            // Create an .ico file in memory, then split it into separate icons.

            var src = GetIconData(icon);

            var splitIcons = new List<Icon>();
            {
                int count = BitConverter.ToUInt16(src, 4);

                for (int i = 0; i < count; i++)
                {
                    int length = BitConverter.ToInt32(src, 6 + 16 * i + 8);    // ICONDIRENTRY.dwBytesInRes
                    int offset = BitConverter.ToInt32(src, 6 + 16 * i + 12);   // ICONDIRENTRY.dwImageOffset

                    using var dst = new BinaryWriter(new MemoryStream(6 + 16 + length));
                    // Copy ICONDIR and set idCount to 1.

                    dst.Write(src, 0, 4);
                    dst.Write((short)1);

                    // Copy ICONDIRENTRY and set dwImageOffset to 22.

                    dst.Write(src, 6 + 16 * i, 12); // ICONDIRENTRY except dwImageOffset
                    dst.Write(22);                   // ICONDIRENTRY.dwImageOffset

                    // Copy a picture.

                    dst.Write(src, offset, length);

                    // Create an icon from the in-memory file.

                    dst.BaseStream.Seek(0, SeekOrigin.Begin);
                    splitIcons.Add(new Icon(dst.BaseStream));
                }
            }

            return splitIcons.ToArray();
        }

        /// <summary>
        /// Converts an Icon to a GDI+ Bitmap preserving the transparent area.
        /// </summary>
        /// <param name="icon">An System.Drawing.Icon to be converted.</param>
        /// <returns>A System.Drawing.Bitmap Object.</returns>
        public static Bitmap ToBitmap(Icon icon)
        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon));

            // Quick workaround: Create an .ico file in memory, then load it as a Bitmap.

            using var ms = new MemoryStream();
            icon.Save(ms);
            return (Bitmap)Image.FromStream(ms);
        }

        /// <summary>
        /// Extracts the specified Icon, finds the Icon whose size is bigger than the specified size and converts that into a GDI+ Bitmap preserving the transparent area.
        /// </summary>
        /// <param name="size">The size of the icon to be bigger than</param>
        /// <param name="ico">An System.Drawing.Icon to be extracted and converted.</param>
        /// <returns></returns>
        public static Image GetImageFromIconBiggerThan(int size, Icon ico)
        {
            var icos = Split(ico);
            foreach (var i in icos)
            {
                if (i.Width >= size)
                {
                    return ToBitmap(i);
                }
            }

            return ico.ToBitmap();
        }

        /// <summary>
        /// Gets the bit depth of an Icon.
        /// </summary>
        /// <param name="icon">An System.Drawing.Icon object.</param>
        /// <returns>The biggest bit depth of the icons.</returns>
        public static int GetBitCount(Icon icon)
        {
            if (icon == null)
                throw new ArgumentNullException(nameof(icon));

            // Create an .ico file in memory, then read the header.

            var data = GetIconData(icon);

            int count = BitConverter.ToInt16(data, 4);
            int bitDepth = 0;
            for (int i = 0; i < count; ++i)
            {
                int depth = BitConverter.ToUInt16(data, 6 + 16 * i + 6);    // ICONDIRENTRY.wBitCount
                if (depth > bitDepth)
                    bitDepth = depth;
            }

            return bitDepth;
        }

        private static byte[] GetIconData(Icon icon)
        {
            var data = getIconData(icon);
            if (data != null)
            {
                return data;
            }
            else
            {
                using var ms = new MemoryStream();
                icon.Save(ms);
                return ms.ToArray();
            }
        }
    }
}
