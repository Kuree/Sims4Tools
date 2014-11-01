using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using s4pi.Interfaces;

namespace CatalogResource.TS4
{
    public class StairCatalogResource : ObjectCatalogResource
    {
        #region Attributes
        private uint unknown1;
        private uint unknown2;
        private uint unknown3;
        private uint unknown4;

        TGIBlock modlTGIReference1;
        TGIBlock modlTGIReference2;
        TGIBlock modlTGIReference3;
        TGIBlock unknownTGIreference;
        TGIBlock wallReference;
        TGIBlock objectReference;
        uint unknown5;
        SwatchColorList colorList;
        byte unknown6;

        #endregion
        public StairCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.unknown4 = r.ReadUInt32();
            this.modlTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknownTGIreference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.wallReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.objectReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknown5 = r.ReadByte();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.unknown6 = r.ReadByte();
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            w.Write(this.unknown4);
            if (this.modlTGIReference1 == null) this.modlTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference1.UnParse(s);
            if (this.modlTGIReference2 == null) this.modlTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference2.UnParse(s);
            if (this.modlTGIReference3 == null) this.modlTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference3.UnParse(s);
            if (this.unknownTGIreference == null) this.unknownTGIreference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.unknownTGIreference.UnParse(s);
            if (this.wallReference == null) this.wallReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.wallReference.UnParse(s);
            if (this.objectReference == null) this.objectReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.objectReference.UnParse(s);
            w.Write(unknown5);
            this.colorList.UnParse(s);
            w.Write(unknown6);
            return s;
        }
        #endregion

        #region Clone
        public override TGIBlock[] NestedTGIBlockList
        {
            get
            {
                List<TGIBlock> tgiList = new List<TGIBlock>();
                tgiList.Add(this.modlTGIReference1);
                tgiList.Add(this.modlTGIReference2);
                tgiList.Add(this.modlTGIReference3);
                tgiList.Add(this.unknownTGIreference);
                tgiList.Add(this.wallReference);
                tgiList.Add(this.objectReference);
                return tgiList.ToArray();
            }
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public uint Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); unknown1 = value; } } }
        [ElementPriority(16)]
        public uint Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); unknown2 = value; } } }
        [ElementPriority(17)]
        public uint Unknown3 { get { return unknown3; } set { if (!unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); unknown3 = value; } } }
        [ElementPriority(18)]
        public uint Unknown4 { get { return unknown4; } set { if (!unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); unknown4 = value; } } }
        [ElementPriority(19)]
        public TGIBlock ModlTGIReference1 { get { return modlTGIReference1; } set { if (!modlTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); modlTGIReference1 = value; } } }
        [ElementPriority(20)]
        public TGIBlock ModlTGIReference2 { get { return modlTGIReference2; } set { if (!modlTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); modlTGIReference2 = value; } } }
        [ElementPriority(21)]
        public TGIBlock ModlTGIReference3 { get { return modlTGIReference3; } set { if (!modlTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); modlTGIReference3 = value; } } }
        [ElementPriority(22)]
        public TGIBlock UnknownTGIreference { get { return unknownTGIreference; } set { if (!unknownTGIreference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); unknownTGIreference = value; } } }
        [ElementPriority(23)]
        public TGIBlock WallReference { get { return wallReference; } set { if (!wallReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); wallReference = value; } } }
        [ElementPriority(24)]
        public TGIBlock ObjectReference { get { return objectReference; } set { if (!objectReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); objectReference = value; } } }
        [ElementPriority(25)]
        public uint Unknown5 { get { return unknown5; } set { if (!unknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); unknown5 = value; } } }
        [ElementPriority(26)]
        public SwatchColorList ColorList { get { return colorList; } set { if (!colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); colorList = value; } } }
        [ElementPriority(27)]
        public byte Unknown6 { get { return unknown6; } set { if (!unknown6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); unknown6 = value; } } }
        #endregion
    }
}
