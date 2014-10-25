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
        private byte buildBuyMode;
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
            this.buildBuyMode = r.ReadByte();
            if (base.Version >= 0x13) this.unknown3 = r.ReadUInt32();
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
        public override List<string> ContentFields { get { var res = base.ContentFields; if (base.Version >= 0x13) { res.Remove("Unknown3"); } return res; } }
        #endregion

        #region Clone Code
        #endregion   
    }
}
