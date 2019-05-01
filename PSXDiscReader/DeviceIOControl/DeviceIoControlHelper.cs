using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace PSXDiscReader
{
    public static class DeviceIoControlHelper
    {
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool DeviceIoControl(
            SafeFileHandle hDevice,
            uint IoControlCode,
            [MarshalAs(UnmanagedType.AsAny)]
            [In] object InBuffer,
            uint nInBufferSize,
            [MarshalAs(UnmanagedType.AsAny)]
            [Out] object OutBuffer,
            uint nOutBufferSize,
            ref uint pBytesReturned,
            [In] IntPtr Overlapped
            );

        public static T InvokeIoControl<T>(SafeFileHandle handle, uint controlCode)
        {
            uint returnedBytes = 0;

            object output = default(T);
            uint outputSize;

#if NETCORE
            outputSize = (uint)Marshal.SizeOf<T>();
#else
            outputSize = (uint)Marshal.SizeOf(typeof(T));
#endif

            bool success = DeviceIoControl(handle, controlCode, null, 0, output, outputSize, ref returnedBytes, IntPtr.Zero);

            if (!success)
            {
                handle.Close();
                int lastError = Marshal.GetLastWin32Error();
                throw new Win32Exception(lastError);
            }

            return (T)output;
        }
    }
}