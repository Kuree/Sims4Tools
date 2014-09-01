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
        LODBlockList lodBlockList;
        DataBlobHandler unknown7;
        private CountedTGIBlockList tgiList;
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
            byte count4 = r.ReadByte();
            tgiList = new CountedTGIBlockList(OnResourceChanged, "IGT", count4, s);
            r.BaseStream.Position = currentPosition;

            lodBlockList = new LODBlockList(null, s, tgiList);

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
            lodBlockList.UnParse(s);
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
        public LODBlockList LodBlockList { get { return lodBlockList; } set { if (!lodBlockList.Equals(value)) lodBlockList = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(16)]
        public DataBlobHandler Unknown7 { get { return unknown7; } set { if (!unknown7.Equals(value)) unknown7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(17)]
        public CountedTGIBlockList TGIList { get { return tgiList; } set { if (!value.Equals(tgiList)) { OnResourceChanged(this, EventArgs.Empty); this.tgiList = value; } } }
        public String Value { get { return ValueBuilder; } }
        #endregion

        #region Sub-Class
        public class LODInfoEntryList : AHandlerElement, IEquatable<LODInfoEntryList>
        {
            const int recommendedApiVersion = 1;

            private CountedTGIBlockList tgiList;

            byte lodNumber;
            byte unknown1;
            DataBlobHandler unknown2;
            ByteIndexList indexList;
            public LODInfoEntryList(int APIversion, EventHandler handler, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; }
            public LODInfoEntryList(int APIversion, EventHandler handler, Stream s, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; Parse(s); }
            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.lodNumber = r.ReadByte();
                unknown1 = r.ReadByte();
                unknown2 = new DataBlobHandler(1, null, r.ReadBytes(16));
                byte[] byteList = new byte[r.ReadByte()];
                for (int i = 0; i < byteList.Length; i++)
                    byteList[i] = r.ReadByte();
                indexList = new ByteIndexList(handler, byteList, tgiList);
                
                
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.lodNumber);
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
            public byte LODNumber { get { return lodNumber; } set { if (!value.Equals(lodNumber)) { lodNumber = value; OnElementChanged(); } } }
            [ElementPriority(1)]
            public byte Unknown1 { get { return unknown1; } set { if (value != unknown1) unknown1 = value; OnElementChanged(); } }
            [ElementPriority(2)]
            public DataBlobHandler Unknown2 { get { return unknown2; } set { if (!value.Equals(unknown2)) unknown2 = value; OnElementChanged(); } }
            [ElementPriority(3)]
            public ByteIndexList IndexList { get { return indexList; } set { if (!value.Equals(indexList)) value = indexList; OnElementChanged(); } }
            public string Value { get { return ValueBuilder; } }

            #endregion

            #region IEquatable
            public bool Equals(LODInfoEntryList other)
            {
                return this.unknown1 == other.unknown1 && this.unknown2.Equals(other.unknown2) && this.indexList.Equals(other.indexList);
            }
            #endregion
        }

        public class LODBlockList : DependentList<LODInfoEntryList>
        {
            #region Attributes
            CountedTGIBlockList tgiList;
            #endregion

            #region Constructors
            public LODBlockList(EventHandler handler, CountedTGIBlockList tgiList) : base(handler) { this.tgiList = tgiList; }
            public LODBlockList(EventHandler handler, Stream s, CountedTGIBlockList tgiList) : base(handler) { this.tgiList = tgiList; Parse(s); }
            #endregion


            #region Data I/O
            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                byte count = r.ReadByte();
                for (int i = 0; i < count; i++)
                {
                    base.Add(new LODInfoEntryList(1, handler, s, tgiList));
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

            protected override LODInfoEntryList CreateElement(Stream s) { return new LODInfoEntryList(1, handler, tgiList); }
            protected override void WriteElement(Stream s, LODInfoEntryList element) { element.UnParse(s); }
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

            public string Value { get { return ValueBuilder; } }
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
            Mood = 0x0040,
            Color = 0x0041,
            Style = 0x0042,
            Theme = 0x0043,
            AgeAppropriate = 0x0044,
            Archetype = 0x0045,
            OutfitCategory = 0x0046,
            Skill = 0x0047,
            EyeColor = 0x0048,
            Persona = 0x0049,
            Special = 0x004A,
            HairColor = 0x004B,
            ColorPalette = 0x004C,
            Hair = 0x004D,
            FacialHair = 0x004E,
            Hat = 0x004F,
            FaceMakeup = 0x0050,
            Top = 0x0051,
            Bottom = 0x0052,
            Body = 0x0053,
            Shoes = 0x0054,
            BottomAccessory = 0x0055,
            BuyCatEE = 0x0056,
            BuyCatPA = 0x0057,
            BuyCatLD = 0x0058,
            BuyCatSS = 0x0059,
            BuyCatVO = 0x005A,
            Uniform = 0x005B,
            Accessories = 0x005C,
            BuyCatMAG = 0x005D,
            FloorPattern = 0x005E,
            WallPattern = 0x005F,
            Fabric = 0x0060,
            Build = 0x0061,
            Pattern = 0x0062,
            HairLength = 0x0063,
            HairTexture = 0x0064,
            TraitGroup = 0x0065,
            SkinHue = 0x0066,
            Reward = 0x0067,
            TerrainPaint = 0x0068,
            EyebrowThickness = 0x0069,
            EyebrowShape = 0x006A,
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
