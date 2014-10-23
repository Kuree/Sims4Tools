using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s4pi.Interfaces;
using System.IO;

namespace CatalogResource.TS4
{
    public class FloorCatalogResource : ObjectCatalogResource
    {
        private uint unknown1;
        private uint unknown2;
        private uint unknown3;
        private uint unknown4;
        private MATDList matdList;
        private SwatchColorList colorList;
        private uint unknown5;
        private ulong catalogGroupID;

        public FloorCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.unknown4 = r.ReadUInt32();
            this.matdList = new MATDList(OnResourceChanged, s, false);
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.unknown5 = r.ReadUInt32();
            this.catalogGroupID = r.ReadUInt64();
        }

        protected override Stream UnParse()
        {
            var s =  base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            w.Write(this.unknown4);
            if (this.matdList == null) this.matdList = new MATDList(OnResourceChanged);
            matdList.UnParse(s);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            w.Write(this.unknown5);
            w.Write(this.catalogGroupID);
            return s;
        }
        [ElementPriority(15)]
        public uint Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(16)]
        public uint Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(17)]
        public uint Unknown3 { get { return this.unknown3; } set { if (!this.unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(18)]
        public uint Unknown4 { get { return this.unknown4; } set { if (!this.unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown4 = value; } } }
        [ElementPriority(19)]
        public MATDList MatdList { get { return this.matdList; } set { if (!this.matdList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.matdList = value; } } }
        [ElementPriority(20)]
        public SwatchColorList ColorList { get { return this.colorList; } set { if (!this.colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(21)]
        public ulong CatalogGroupID { get { return this.catalogGroupID; } set { if (!this.catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(22)]
        public uint Unknown5 { get { return this.unknown5; } set { if (!this.unknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown5 = value; } } }
    }
}
