using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CASPartResource
{
    public class CASPartResourceTS4 : AResource
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
        byte unknown1;
        private uint outfitGroup;
        DataBlobHandler unknown2;
        IndexList<UInt32> unknown3;
        DataBlobHandler unknown4;
        uint[] unknown5;
        DataBlobHandler unknown6;
        UnknownClassList unknown7;
        DataBlobHandler unknown8;
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
            unknown1 = r.ReadByte();
            outfitGroup = r.ReadUInt32();
            unknown2 = new DataBlobHandler(recommendedApiVersion, OnResourceChanged, r.ReadBytes(17));
            uint count = r.ReadUInt32();
            uint[] unknown2List = new uint[count];
            for (uint i = 0; i < count; i++)
                unknown2List[i] = r.ReadUInt32();
            unknown3 = new IndexList<uint>(OnResourceChanged, unknown2List);

            unknown4 = new DataBlobHandler(1, null, r.ReadBytes(2 * 3 * 4 + 1 + 2));

            byte count2 = r.ReadByte();
            unknown5 = new uint[count2];
            for (byte i = 0; i < count2; i++)
                unknown5[i] = r.ReadUInt32();

            unknown6 = new DataBlobHandler(1, null, r.ReadBytes(2 * 4));

            // TGI block list
            long currentPosition = r.BaseStream.Position;
            r.BaseStream.Position = tgiOffset;
            tgiList = new TGIBlockList(null);
            byte count4 = r.ReadByte();
            for (int i = 0; i < count4; i++)
                tgiList.Add(new TGIBlock(1, null, "IGT", s));
            r.BaseStream.Position = currentPosition;

            unknown7 = new UnknownClassList(null, s, tgiList);

            unknown8 = new DataBlobHandler(1, null, r.ReadBytes(10));            
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
            w.Write(unknown1);
            w.Write(outfitGroup);
            unknown2.UnParse(s);
            w.Write((uint)unknown3.Count);
            foreach (var value in unknown3) w.Write(value);
            unknown4.UnParse(s);
            w.Write((byte)unknown5.Length);
            foreach(var value in unknown5) w.Write(value);
            unknown6.UnParse(s);
            unknown7.UnParse(s);
            unknown8.UnParse(s);

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
        public byte Unknown1 { get { return unknown1; } set { if (!value.Equals(unknown1)) unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(7)]
        public uint OutfitGroup { get { return outfitGroup; } set { if (!value.Equals(outfitGroup)) outfitGroup = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(8)]
        public DataBlobHandler Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(9)]
        public IndexList<UInt32> Unknown3 { get { return unknown3; } set { if (!value.Equals(unknown3)) unknown3 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(10)]
        public DataBlobHandler Unknown4 { get { return unknown4; } set { if (!unknown4.Equals(value)) unknown4 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(11)]
        public UInt32[] Unknown5 { get { return unknown5; } set { if (!unknown5.Equals(value)) unknown5 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(12)]
        public DataBlobHandler Unknown6 { get { return unknown6; } set { if (!unknown6.Equals(value)) unknown6 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(13)]
        public UnknownClassList Unknown7 { get { return unknown7; } set { if (!unknown7.Equals(value)) unknown7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(14)]
        public DataBlobHandler Unknown8 { get { return unknown8; } set { if (!unknown8.Equals(value)) unknown8 = value; OnResourceChanged(this, EventArgs.Empty); } }
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

        public class Flag
        {
            short flagCatagory;
            short flagValue;

            public Flag(short flagCatagory, short flagValue)
            {

            }
        }

        public class FlagList //: DependentList<Flag>
        {

        }

        #region Flags
        public enum CASPFlags : ushort
        {
            Color = 0x0041,
            Style = 0x0042,
            Unknown1 = 0x0044,
            Unknown2 = 0x0045,
            Unknown3 = 0x0046,
            EyeMakeupColor = 0x0048,
            Hair = 0x004B,
            Skirt = 0x004C,
            Unknown4 = 0x004E,
            Hat = 0x004F,
            Top = 0x0051,
            Bottom = 0x0052,
            Unknown5 = 0x0046,
        }
        #endregion

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
