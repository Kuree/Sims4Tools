using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource.TS4
{
    public class ObjectInstanceResource : ObjectCatalogResource
    {
        #region Attributes
        private DataBlobHandler unknown1;
        private uint unknownFlags1;
        private uint unknownFlags2;
        private uint unknownFlags3;
        private uint unknownFlags4;
        private uint unknownFlags5;
        private DataBlobHandler unknown2;
        private SwatchColorList colorList;
        private DataBlobHandler unknownFlags;
        private bool buildBuyMode;
        private uint unknown3;
        #endregion

        public ObjectInstanceResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.unknown1 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(7 * 4));
            this.unknownFlags1 = r.ReadUInt32();
            this.unknownFlags2 = r.ReadUInt32();
            this.unknownFlags3 = r.ReadUInt32();
            this.unknownFlags4 = r.ReadUInt32();
            this.unknownFlags5 = r.ReadUInt32();
            this.unknown2 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(18));
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.unknownFlags = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(5));
            this.buildBuyMode = r.ReadBoolean();
            if (base.Version >= 0x19) this.unknown3 = r.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            var s =  base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            if (this.unknown1 == null) this.unknown1 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, new byte[7 * 4]);
            this.unknown1.UnParse(s);
            w.Write(this.unknownFlags1);
            w.Write(this.unknownFlags2);
            w.Write(this.unknownFlags3);
            w.Write(this.unknownFlags4);
            w.Write(this.unknownFlags5);
            if (this.unknown2 == null) this.unknown2 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, new byte[18]);
            this.unknown2.UnParse(s);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            if (this.unknownFlags == null) this.unknownFlags = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, new byte[5]);
            w.Write(this.buildBuyMode);
            if (base.Version >= 0x13) w.Write(this.unknown3);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public DataBlobHandler Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(16)]
        public uint UnknownFlag1 { get { return this.unknownFlags1; } set { if (!this.unknownFlags1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags1 = value; } } }
        [ElementPriority(17)]
        public uint UnknownFlag2 { get { return this.unknownFlags2; } set { if (!this.unknownFlags2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags2 = value; } } }
        [ElementPriority(18)]
        public uint UnknownFlag3 { get { return this.unknownFlags3; } set { if (!this.unknownFlags3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags3 = value; } } }
        [ElementPriority(19)]
        public uint UnknownFlag4 { get { return this.unknownFlags4; } set { if (!this.unknownFlags4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags4 = value; } } }
        [ElementPriority(20)]
        public uint UnknownFlag5 { get { return this.unknownFlags5; } set { if (!this.unknownFlags5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags5 = value; } } }
        [ElementPriority(21)]
        public DataBlobHandler Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(22)]
        public SwatchColorList SwatchColors { get { return this.colorList; } set { if (!this.colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(23)]
        public DataBlobHandler UnknownFlags { get { return this.unknownFlags; } set { if (!this.unknownFlags.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags = value; } } }
        [ElementPriority(24)]
        public bool BuildBuyMode { get { return this.buildBuyMode; } set { if (!this.buildBuyMode.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.buildBuyMode = value; } } }
        [ElementPriority(25)]
        public uint Unknown3 { get { return this.unknown3; } set { if (!this.unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        public override List<string> ContentFields { get { var res = base.ContentFields; if (base.Version >= 0x13) { res.Remove("Unknown3"); } return res; } }
        #endregion

        #region Clone Code
        #endregion   
    }
}
