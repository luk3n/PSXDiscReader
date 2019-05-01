using Microsoft.Win32.SafeHandles;

namespace PSXDiscReader
{
    public class DiskDeviceWrapper : DeviceIoWrapperBase
    {
        public DiskDeviceWrapper(SafeFileHandle handle, bool ownsHandle = false)
            : base(handle, ownsHandle)
        {
        }

        /// <summary><see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/aa365169(v=vs.85).aspx"/></summary>
        public DISK_GEOMETRY DiskGetDriveGeometry()
        {
            return DeviceIoControlHelper.InvokeIoControl<DISK_GEOMETRY>(Handle, (0x00000007 << 16) | (0x0000 << 2) | 0 | (0 << 14));
        }
    }
}