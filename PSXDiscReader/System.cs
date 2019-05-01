using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace PSXDiscReader
{
    /// <summary>
    /// The PSX disc system area
    /// </summary>
    public sealed class System
    {
        #region Private fields

        private const int psxLogoOffset = 0x800;
        private const int logoSize = 0x3279;
        private const string logoHash = "ad1c76163b4c603c543bf40d84d44acd";

        #endregion

        #region Constructor

        internal System(byte[] sectors)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(sectors)))
            {
                Licence = new string(reader.ReadChars(70));
                Region = DetermineRegion(Licence.Substring(Licence.Length - 6).Trim());

                Licence = (Region == Region.Japan) ? Licence.Substring(0, Licence.Length - 5) : Licence;
                Licence = Regex.Replace(Licence, " {2,}", " ");

                reader.BaseStream.Position = psxLogoOffset;
                Logo = reader.ReadBytes(logoSize);

                LogoFingerprint = GenerateFingerprint(Logo);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Indicates whether the PSX logo was modded or not.
        /// </summary>
        public bool LogoIsPristine
        {
            get { return (logoHash == LogoFingerprint); }
        }

        #endregion

        #region Private methods

        private Region DetermineRegion(string suffix)
        {
            switch (suffix)
            {
                case "ica":
                    return Region.America;
                case "pe":
                    return Region.Europe;
                case "00000":
                    return Region.Japan;
                default:
                    return Region.Unknown;
            }
        }

        private string GenerateFingerprint(byte[] logo)
        {
            using (MD5 md5 = MD5.Create())
            {
                return BitConverter.ToString(md5.ComputeHash(logo)).Replace("-", string.Empty).ToLower();
            }
        }

        #endregion

        #region Properties

        public string Licence
        {
            get;
            private set;
        }

        public byte[] Logo // TMD format!! It can be viewed with PSXprev
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a MD5 hash of the logo
        /// </summary>
        public string LogoFingerprint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the game region
        /// </summary>
        public Region Region
        {
            get;
            private set;
        }

        #endregion
    }
}
