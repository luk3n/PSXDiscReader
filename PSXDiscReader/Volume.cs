using System;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PSXDiscReader
{
    public sealed class Volume
    {
        #region Private fields

        private const int publisherId = 0x13E;
        private const int cdXASignature = 0x400;

        #endregion

        #region Constructor

        internal Volume(byte[] sectors)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(sectors)))
            {
                Type = reader.ReadSByte();
                Identifier = new string(reader.ReadChars(5));
                Version = reader.ReadSByte();
                reader.BaseStream.Position += 1;
                System = Regex.Replace(new string(reader.ReadChars(32)), " {2,}", " ");
                VolumeIdentifier = Regex.Replace(new string(reader.ReadChars(32)), " {2,}", " ");
                reader.BaseStream.Position += 8;
                Size = reader.ReadUInt32(); // little endian!!
                reader.BaseStream.Position = publisherId;
                Publisher = Regex.Replace(new string(reader.ReadChars(128)), " {2,}", " ");
                Preparer = Regex.Replace(new string(reader.ReadChars(128)), " {2,}", " ");
                Application = new string(reader.ReadChars(128));
                CopyrightFilename = Regex.Replace(new string(reader.ReadChars(37)), " {2,}", " ");
                AbstractFilename = Regex.Replace(new string(reader.ReadChars(37)), " {2,}", " ");
                BibliographicFilename = Regex.Replace(new string(reader.ReadChars(37)), " {2,}", " ");
                VolumeCreation = ConvertToDateTime(reader.ReadChars(16));
                reader.BaseStream.Position += 1;
                VolumeModification = ConvertToDateTime(reader.ReadChars(16));
                reader.BaseStream.Position += 1;
                VolumeExpiration = ConvertToDateTime(reader.ReadChars(16));
                reader.BaseStream.Position += 1;
                VolumeEffective = ConvertToDateTime(reader.ReadChars(16));
                reader.BaseStream.Position += 1;
                reader.BaseStream.Position = cdXASignature;
                CDXA_Signature = new string(reader.ReadChars(8));
            }
        }

        #endregion

        #region Private methods

        private DateTime? ConvertToDateTime(char[] date)
        {
            if (DateTime.TryParseExact(new string(date), "yyyyMMddHHmmssFF", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                return result;
            else
                return null;
        }

        #endregion

        #region Properties

        public sbyte Type
        {
            get;
            private set;
        }

        public string Identifier
        {
            get;
            private set;
        }

        public sbyte Version
        {
            get;
            private set;
        }

        public string System
        {
            get;
            private set;
        }

        public string VolumeIdentifier
        {
            get;
            private set;
        }

        public string Publisher
        {
            get;
            private set;
        }

        public string Preparer
        {
            get;
            private set;
        }

        public string Application
        {
            get;
            private set;
        }

        public string CopyrightFilename
        {
            get;
            private set;
        }

        public string AbstractFilename
        {
            get;
            private set;
        }

        public string BibliographicFilename
        {
            get;
            private set;
        }

        public DateTime? VolumeCreation
        {
            get;
            private set;
        }

        public DateTime? VolumeModification
        {
            get;
            private set;
        }

        public DateTime? VolumeExpiration
        {
            get;
            private set;
        }

        public DateTime? VolumeEffective
        {
            get;
            private set;
        }

        public UInt32 Size
        {
            get;
            private set;
        }

        public string CDXA_Signature
        {
            get;
            private set;
        }

        public string Data
        {
            get;
            private set;
        }

        #endregion
    }
}
