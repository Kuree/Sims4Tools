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
            this.modlList = new CountedTGIBlockList(OnResourceChanged, "ITG", tgiList);
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
                for (int i = 0; i < 8; i++) tgiList[i] = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.modlList = new CountedTGIBlockList(OnResourceChanged, tgiList);
            }
            foreach (var tgi in this.modlList)
            {
                // There is a bug in Peter's code.
                // The TGIBlcok being copied for several times and the TGI order got lost
                w.Write(tgi.Instance);
                w.Write(tgi.ResourceType);
                w.Write(tgi.ResourceGroup);
            }
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            if (colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            return s;
        }
        #endregion

        #region Clone
        public override TGIBlock[] NestedTGIBlockList { get { return this.modlList.ToArray(); } }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public CountedTGIBlockList MOLDList { get { return this.modlList; } set { if (!this.modlList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlList = value; } } }
        [ElementPriority(16)]
        public uint Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(17)]
        public ulong Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(18)]
        public uint Unknown3 { get { return this.unknown3; } set { if (!this.unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(19)]
        public SwatchColorList ColorList { get { return this.colorList; } set { if (!this.colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        #endregion
    }
}
