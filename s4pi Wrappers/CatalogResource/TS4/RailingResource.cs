using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CatalogResource.TS4
{
    public class RailingResource : ObjectCatalogResource
    {
        public RailingResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            return s;
        }
        #endregion
    }
}
