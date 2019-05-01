using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSXDiscReader
{
    public sealed class PSXDisc : Disc
    {
        #region Constructor

        public PSXDisc(string path) : base(path)
        {
            System = new System(ReadSector(4, 11));
            Volume = new Volume(ReadSector(16));
        }

        #endregion

        #region Properties

        public System System
        {
            get;
            private set;
        }

        public Volume Volume
        {
            get;
            private set;
        }

        #endregion
    }
}
