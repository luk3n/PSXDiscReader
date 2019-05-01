using System.Runtime.InteropServices;

namespace PSXDiscReader
{
    public enum MEDIA_TYPE : uint
    {
        Unknown,
        F5_1Pt2_512,
        F3_1Pt44_512,
        F3_2Pt88_512,
        F3_20Pt8_512,
        F3_720_512,
        F5_360_512,
        F5_320_512,
        F5_320_1024,
        F5_180_512,
        F5_160_512,
        RemovableMedia,
        FixedMedia,
        F3_120M_512,
        F3_640_512,
        F5_640_512,
        F5_720_512,
        F3_1Pt2_512,
        F3_1Pt23_1024,
        F5_1Pt23_1024,
        F3_128Mb_512,
        F3_230Mb_512,
        F8_256_128,
        F3_200Mb_512,
        F3_240M_512,
        F3_32M_512
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DISK_GEOMETRY
    {
        public long Cylinders;
        public MEDIA_TYPE MediaType;
        public int TracksPerCylinder;
        public int SectorsPerTrack;
        public int BytesPerSector;

        public long DiskSize
        {
            get { return Cylinders * TracksPerCylinder * SectorsPerTrack * BytesPerSector; }
        }
    }
}