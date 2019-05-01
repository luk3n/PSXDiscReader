using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Linq;

namespace PSXDiscReader
{
    public abstract class Disc
    {
        #region P/Invokes

        public enum WinMoveMethod : uint
        {
            Begin = 0,
            Current = 1,
            End = 2
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(
            string lpFileName,
            [MarshalAs(UnmanagedType.U4)] FileAccess dwDesiredAccess,
            [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
            IntPtr lpSecurityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
            [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean SetFilePointerEx(
            [In] SafeFileHandle fileHandle,
            [In] Int64 distanceToMove,
            [Out] out Int64 newOffset,
            [In] WinMoveMethod moveMethod);

        [DllImport("kernel32", SetLastError = true)]
        public extern static Boolean ReadFile(
            [In] SafeFileHandle handle,
            [Out] Byte[] buffer,
            [In] Int32 numBytesToRead,
            [Out] out Int32 numBytesRead,
            [In] IntPtr overlapped);

        #endregion

        #region Constructor

        protected Disc(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException("You must set a path to the disc drive!");

            if (!DriveInfo.GetDrives().Any(d => d.Name.Trim('\\') == path & d.DriveType == DriveType.CDRom))
                throw new DriveNotFoundException("That drive not exists!");

            Handle = CreateFile(string.Format(@"\\.\{0}", path), FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            DiskDeviceWrapper deviceIo = new DiskDeviceWrapper(Handle);
            Geometry = deviceIo.DiskGetDriveGeometry();
        }

        #endregion

        #region Properties

        public SafeFileHandle Handle
        {
            get;
            protected set;
        }

        public DISK_GEOMETRY Geometry
        {
            get;
            protected set;
        }

        #endregion

        #region Public methods

        public byte[] ReadSector(int sector)
        {
            Int64 newOffset;
            byte[] rawData = new byte[Geometry.BytesPerSector];
            Int32 numberOfBytesRead;
            SetFilePointerEx(Handle, Geometry.BytesPerSector * sector, out newOffset, WinMoveMethod.Begin);
            ReadFile(Handle, rawData, Geometry.BytesPerSector, out numberOfBytesRead, IntPtr.Zero);

            return rawData;
        }

        public byte[] ReadSector(int startSector, int endSector)
        {
            Int64 newOffset;
            byte[] rawData = new byte[Geometry.BytesPerSector * ((endSector + 1) - startSector)];
            Int32 numberOfBytesRead;
            SetFilePointerEx(Handle, Geometry.BytesPerSector * startSector, out newOffset, WinMoveMethod.Begin);
            ReadFile(Handle, rawData, Geometry.BytesPerSector * ((endSector + 1) - startSector), out numberOfBytesRead, IntPtr.Zero);

            return rawData;
        }

        #endregion
    }
}
