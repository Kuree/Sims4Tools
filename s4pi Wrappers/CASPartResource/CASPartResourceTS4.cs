using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CASPartResource
{
    class CASPartResourceTS4 : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        public uint version;
        uint tgiOffset;
        uint presetCount;
        string name;
        float sortPriority;
        byte colorCode;
        private uint outfitGroup;
        DataBlobHandler unknown1;
        IndexList<UInt32> unknown2;
        DataBlobHandler unknown3;
        uint[] unknown4;
        DataBlobHandler unknown5;
        UnknownClassList unknown6;
        DataBlobHandler unknown7;
        public TGIBlockList tgiList { get; set; }
        #endregion

        public CASPartResourceTS4(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            version = r.ReadUInt32();
            tgiOffset = r.ReadUInt32() + 8;
            presetCount = r.ReadUInt32();
            if (presetCount != 0) throw new Exception("Found non-zero one");
            name = BigEndianUnicodeString.Read(s);

            sortPriority = r.ReadSingle();
            colorCode = r.ReadByte();
            outfitGroup = r.ReadUInt32();
            unknown1 = new DataBlobHandler(recommendedApiVersion, OnResourceChanged, r.ReadBytes(18));
            uint count = r.ReadUInt32();
            uint[] unknown2List = new uint[count];
            for (uint i = 0; i < count; i++)
                unknown2List[i] = r.ReadUInt32();
            unknown2 = new IndexList<uint>(OnResourceChanged, unknown2List);

            unknown3 = new DataBlobHandler(1, null, r.ReadBytes(2 * 3 * 4 + 1 + 2));

            byte count2 = r.ReadByte();
            unknown4 = new uint[count2];
            for (byte i = 0; i < count2; i++)
                unknown4[i] = r.ReadUInt32();

            unknown5 = new DataBlobHandler(1, null, r.ReadBytes(2 * 4));

            // TGI block list
            long currentPosition = r.BaseStream.Position;
            r.BaseStream.Position = tgiOffset;
            tgiList = new TGIBlockList(null);
            byte count4 = r.ReadByte();
            for (int i = 0; i < count4; i++)
                tgiList.Add(new TGIBlock(1, null, "IGT", s));
            r.BaseStream.Position = currentPosition;

            unknown6 = new UnknownClassList(null, s, tgiList);

            unknown7 = new DataBlobHandler(1, null, r.ReadBytes(10));            
        }

        protected override Stream UnParse()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(this.version);
            w.Write(0); // tgi offset
            w.Write(presetCount);
            BigEndianUnicodeString.Write(s, name);
            w.Write(sortPriority);
            w.Write(colorCode);
            w.Write(outfitGroup);
            unknown1.UnParse(s);
            w.Write((uint)unknown2.Count);
            foreach (var value in unknown2) w.Write(value);
            unknown3.UnParse(s);
            w.Write((byte)unknown4.Length);
            foreach(var value in unknown4) w.Write(value);
            unknown5.UnParse(s);
            unknown6.UnParse(s);
            unknown7.UnParse(s);

            long tgiPosition = w.BaseStream.Position;
            w.BaseStream.Position = 4;
            w.Write(tgiPosition - 8);
            w.BaseStream.Position = tgiPosition;

            w.Write((byte)tgiList.Count);
            foreach (var tgi in tgiList) tgi.UnParse(s);

            return s;
        }
        #endregion
        
        #region Content Fields
        [ElementPriority(0)]
        public uint Version { get { return version; } set { if (value != version)version = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(1)]
        public uint TGIoffset { get { return tgiOffset; } set { if (value != tgiOffset) tgiOffset = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(2)]
        public uint PresetCount { get { return presetCount; } set { if (value != presetCount) presetCount = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(3)]
        public string Name { get { return name; } set { if (!value.Equals(name)) name = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(4)]
        public float SortPriority { get { return sortPriority; } set { if (!value.Equals(sortPriority)) sortPriority = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(5)]
        public byte ColorCode { get { return colorCode; } set { if (!value.Equals(colorCode)) colorCode = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(6)]
        public uint OutfitGroup { get { return outfitGroup; } set { if (!value.Equals(outfitGroup)) outfitGroup = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(7)]
        public DataBlobHandler Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(8)]
        public IndexList<UInt32> Unknown2 { get { return unknown2; } set { if (!value.Equals(unknown2)) unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(9)]
        public DataBlobHandler Unknown3 { get { return unknown3; } set { if (!unknown3.Equals(value)) unknown3 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(10)]
        public UInt32[] Unknown4 { get { return unknown4; } set { if (!unknown4.Equals(value)) unknown4 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(11)]
        public DataBlobHandler Unknown5 { get { return unknown5; } set { if (!unknown5.Equals(value)) unknown5 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(12)]
        public UnknownClassList Unknown6 { get { return unknown6; } set { if (!unknown6.Equals(value)) unknown6 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(13)]
        public DataBlobHandler Unknown7 { get { return unknown7; } set { if (!unknown7.Equals(value)) unknown7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        public String Value { get { return ValueBuilder; } }
        #endregion

        #region Sub-Class
        public class UnknownClass : AHandlerElement, IEquatable<UnknownClass>
        {
            const int recommendedApiVersion = 1;

            private TGIBlockList tgiList;

            ushort unknown1;
            DataBlobHandler unknown2;
            IndexList<byte> indexList;
            public UnknownClass(int APIversion, EventHandler handler, TGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; }
            public UnknownClass(int APIversion, EventHandler handler, Stream s, TGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; Parse(s); }
            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                unknown1 = r.ReadUInt16();
                unknown2 = new DataBlobHandler(1, null, r.ReadBytes(16));
                byte[] byteList = new byte[r.ReadByte()];
                for (int i = 0; i < byteList.Length; i++)
                    byteList[i] = r.ReadByte();
                indexList = new IndexList<byte>(handler, byteList, ParentTGIBlocks: tgiList);
                
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(unknown1);
                unknown2.UnParse(s);
                w.Write((byte)indexList.Count);
                foreach (byte b in indexList)
                    w.Write(b);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region Content Fields
            [ElementPriority(0)]
            public ushort Unknown1 { get { return unknown1; } set { if (value != unknown1) unknown1 = value; OnElementChanged(); } }
            [ElementPriority(1)]
            public DataBlobHandler Unknown2 { get { return unknown2; } set { if (!value.Equals(unknown2)) unknown2 = value; OnElementChanged(); } }
            [ElementPriority(2)]
            public IndexList<byte> IndexList { get { return indexList; } set { if (!value.Equals(indexList)) value = indexList; OnElementChanged(); } }
            public string Value { get { return ValueBuilder; } }

            #endregion
            public bool Equals(UnknownClass other)
            {
                return this.unknown1 == other.unknown1 && this.unknown2.Equals(other.unknown2) && this.indexList.Equals(other.indexList);
            }
        }

        public class UnknownClassList : DependentList<UnknownClass>
        {
            #region Attributes
            TGIBlockList tgiList;
            #endregion

            #region Constructors
            public UnknownClassList(EventHandler handler, TGIBlockList tgiList) : base(handler) { this.tgiList = tgiList; }
            public UnknownClassList(EventHandler handler, Stream s, TGIBlockList tgiList) : base(handler, s) { this.tgiList = tgiList; }
            #endregion


            #region Data I/O
            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                byte count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    base.Add(new UnknownClass(1, handler, s, tgiList));
                }
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((byte)base.Count);
                foreach(var unknownClass in this)
                {
                    unknownClass.UnParse(s);
                }
            }

            protected override UnknownClass CreateElement(Stream s) { return new UnknownClass(1, handler, tgiList); }
            protected override void WriteElement(Stream s, UnknownClass element) { element.UnParse(s); }
            #endregion
            
        }

        #endregion
    }

    public class CASPartResourceTS4Handler : AResourceHandler
    {
        public CASPartResourceTS4Handler()
        {
            if(s4pi.Settings.Settings.IsTS4)
                this.Add(typeof(CASPartResourceTS4), new List<string>(new string[] { "0x034AEECB", }));
        }
    }

}
