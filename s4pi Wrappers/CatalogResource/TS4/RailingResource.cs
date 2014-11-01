using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using s4pi.Interfaces;

namespace CatalogResource.TS4
{
    public class RailingResource : ObjectCatalogResource
    {
        public RailingResource(int APIversion, Stream s) : base(APIversion, s) { }
        private CountedTGIBlockList modlList;
        private uint unknown1;
        private ulong unknown2;
        private uint unknown3;
        private SwatchColorList colorList;

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            TGIBlock[] tgiList = new TGIBlock[8];
            for (int i = 0; i < 8; i++) tgiList[i] = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlList = new CountedTGIBlockList(OnResourceChanged, tgiList);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt64();
            this.unknown3 = r.ReadUInt32();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            if (this.modlList == null)
            {                                           
                TGIBlock[] tgiList = new TGIBlock[8];
                for (int i = 0; i < 8; i++) tgiList[i] = new TGIBlock(RecommendedApiVersion, OnResourceChanged);
                this.modlList = new CountedTGIBlockList(OnResourceChanged, tgiList);
            }
            foreach (var tgi in this.modlList) tgi.UnParse(s);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            if (colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            return s;
        }
        #endregion
    }
}
