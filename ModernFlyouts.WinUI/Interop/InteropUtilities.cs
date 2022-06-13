using System;
using System.Runtime.InteropServices;

namespace ModernFlyouts.WinUI.Interop;

internal static class InteropUtilities
{
    /// <exception cref="COMException"></exception>
    public static HWND EnsureNonNull(this HWND value)
    {
        if (value.Value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static ushort EnsureNonZero(this ushort value)
    {
        if (value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static uint EnsureNonZero(this uint value)
    {
        if (value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static int EnsureNonZero(this int value)
    {
        if (value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static nint EnsureNonZero(this nint value)
    {
        if (value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static nuint EnsureNonZero(this nuint value, Exception? exception = null)
    {
        if (value == 0)
        {
            if (exception == null)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
            }
            else
            {
                throw exception;
            }
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static BOOL EnsureNonZero(this BOOL value)
    {
        if (value.Value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static HBRUSH EnsureNonZero(this HBRUSH value)
    {
        if (value.IsNull)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }
}
