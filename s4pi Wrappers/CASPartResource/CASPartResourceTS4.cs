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
        FlagList flagList;
        byte unknown3;
        byte swatchIndex;
        DataBlobHandler unknown4;
        uint[] swatchColorCode;
        DataBlobHandler unknown5;
        UnknownClassList unknown6;
        DataBlobHandler unknown7;
        public TGIBlockList tgiList;
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

            flagList = new FlagList(OnResourceChanged, s);

            this.unknown3 = r.ReadByte();
            this.swatchIndex = r.ReadByte();

            unknown4 = new DataBlobHandler(1, null, r.ReadBytes(2 * 3 * 4 + 1));

            byte count2 = r.ReadByte();
            swatchColorCode = new uint[count2];
            for (byte i = 0; i < count2; i++)
                swatchColorCode[i] = r.ReadUInt32();

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
            w.Write(unknown1);
            w.Write(outfitGroup);
            unknown2.UnParse(s);
            flagList.UnParse(s);
            w.Write(this.unknown3);
            w.Write(this.swatchIndex);
            unknown4.UnParse(s);
            w.Write((byte)swatchColorCode.Length);
            foreach(var value in swatchColorCode) w.Write(value);
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
        public byte Unknown1 { get { return unknown1; } set { if (!value.Equals(unknown1)) unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(7)]
        public uint OutfitGroup { get { return outfitGroup; } set { if (!value.Equals(outfitGroup)) outfitGroup = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(8)]
        public DataBlobHandler Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(9)]
        public FlagList CASFlagList { get { return flagList; } set { if (!value.Equals(flagList)) flagList = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(10)]
        public byte Unknown3 { get { return unknown3; } set { if (!value.Equals(unknown3)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(11)]
        public byte SwatchIndex { get { return swatchIndex; } set { if (!swatchIndex.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.swatchIndex = value; } } }
        [ElementPriority(12)]
        public DataBlobHandler Unknown4 { get { return unknown4; } set { if (!unknown4.Equals(value)) unknown4 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(13)]
        public UInt32[] SwatchColorCode { get { return swatchColorCode; } set { if (!swatchColorCode.Equals(value)) swatchColorCode = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(14)]
        public DataBlobHandler Unknown5 { get { return unknown5; } set { if (!unknown5.Equals(value)) unknown5 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(15)]
        public UnknownClassList Unknown6 { get { return unknown6; } set { if (!unknown6.Equals(value)) unknown6 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(16)]
        public DataBlobHandler Unknown7 { get { return unknown7; } set { if (!unknown7.Equals(value)) unknown7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(17)]
        public TGIBlockList TGIList { get { return tgiList; } set { if (!value.Equals(tgiList)) { OnResourceChanged(this, EventArgs.Empty); this.tgiList = value; } } }
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

            #region IEquatable
            public bool Equals(UnknownClass other)
            {
                return this.unknown1 == other.unknown1 && this.unknown2.Equals(other.unknown2) && this.indexList.Equals(other.indexList);
            }
            #endregion
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

        public class Flag : AHandlerElement, IEquatable<Flag>
        {
            CASPFlags flagCatagory;
            ushort flagValue;

            public Flag(int APIversion, EventHandler handler, Stream s) :base(APIversion, handler)
            {
                BinaryReader r = new BinaryReader(s);
                this.flagCatagory = (CASPFlags)r.ReadUInt16();
                this.flagValue = r.ReadUInt16();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((ushort)this.flagCatagory);
                w.Write(this.flagValue);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public bool Equals(Flag other)
            {
                return this.flagValue == other.flagValue && this.flagCatagory == other.flagCatagory;
            }

            [ElementPriority(0)]
            public CASPFlags FlagCatagory { get { return this.flagCatagory; } set { if (value != this.flagCatagory) { OnElementChanged(); this.flagCatagory = value; } } }
            [ElementPriority(1)]
            public ushort FlagValue { get { return this.flagValue; } set { if (value != this.flagValue) { OnElementChanged(); this.flagValue = value; } } }
            
        }

        public class FlagList : DependentList<Flag> 
        {
            public FlagList(EventHandler handler, Stream s) : base(handler, s) { }

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                uint count = r.ReadUInt32();
                for (int i = 0; i < count; i++)
                    base.Add(new Flag(recommendedApiVersion, handler, s));
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)base.Count);
                foreach (var flag in this)
                    flag.UnParse(s);
            }
            #endregion

            protected override Flag CreateElement(Stream s) { return new Flag(recommendedApiVersion, null, s); }
            protected override void WriteElement(Stream s, Flag element) { element.UnParse(s); }
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
            Body = 0x0053,
            Shoes = 0x0054,
            Unknown5 = 0x005B,
            ClothingMaterial = 0x0060,
            Hair_Again = 0x0063,
            Hair_Maybe = 0x0063,
            Eyebrows_Maybe = 0x0063,
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
