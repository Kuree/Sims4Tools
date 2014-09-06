using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        ushort secondarySortIndex;
        private uint propertyID;
        uint auralMaterialHash;
        PramFlag parmFlags;
        ulong excludePartFlags;
        uint excludeModifierRegionFlags;
        FlagList flagList;
        uint simlolencePrice;
        uint partTitleKey;
        uint partDesptionKey;
        byte uniqueTextureSpace;
        int bodyType;
        int unused1;
        uint ageGender;
        byte unused2;
        byte unused3;
        SwatchColorList swatchColorCode;
        byte buffResKey;
        byte varientThumbnailKey;
        byte nakedKey;
        byte parentKey;
        int sortLayer;
        LODBlockList lodBlockList;
        List<byte> slotKey;
        byte difussShadowKey;
        byte shadowKey;
        byte compositionMethod;
        byte regionMapKey;
        byte overrides;
        byte normalMapKey;
        byte specularMapKey;
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
            this.secondarySortIndex = r.ReadUInt16();
            propertyID = r.ReadUInt32();
            this.auralMaterialHash = r.ReadUInt32();
            this.parmFlags = (PramFlag)r.ReadByte();
            this.excludePartFlags = r.ReadUInt64();
            this.excludeModifierRegionFlags = r.ReadUInt32();
            
            flagList = new FlagList(OnResourceChanged, s);

            this.simlolencePrice = r.ReadUInt32();
            this.partTitleKey = r.ReadUInt32();
            this.partDesptionKey = r.ReadUInt32();
            this.uniqueTextureSpace = r.ReadByte();
            this.bodyType = r.ReadInt32();
            this.unused1 = r.ReadInt32();
            this.ageGender = r.ReadUInt32();
            this.unused2 = r.ReadByte();
            this.unused3 = r.ReadByte();

            swatchColorCode = new SwatchColorList(OnResourceChanged, s);

            this.buffResKey = r.ReadByte();
            this.varientThumbnailKey = r.ReadByte();
            this.nakedKey = r.ReadByte();
            this.parentKey = r.ReadByte();
            this.sortLayer = r.ReadByte();


            // TGI block list
            long currentPosition = r.BaseStream.Position;
            r.BaseStream.Position = tgiOffset;            
            byte count4 = r.ReadByte();
            tgiList = new CountedTGIBlockList(OnResourceChanged, "IGT", count4, s);
            r.BaseStream.Position = currentPosition;

            lodBlockList = new LODBlockList(null, s, tgiList);

            byte count = r.ReadByte();
            this.slotKey = new List<byte>(count);
            for (byte i = 0; i < count; i++) this.slotKey.Add(r.ReadByte());

            this.difussShadowKey = r.ReadByte();
            this.shadowKey = r.ReadByte();
            this.compositionMethod = r.ReadByte();
            this.regionMapKey = r.ReadByte();
            this.overrides = r.ReadByte();
            this.normalMapKey = r.ReadByte();
            this.specularMapKey = r.ReadByte();
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
            w.Write(secondarySortIndex);
            w.Write(propertyID);
            w.Write(auralMaterialHash);
            w.Write((byte)parmFlags);
            w.Write(excludePartFlags);
            w.Write(excludeModifierRegionFlags);
            flagList.UnParse(s);
            w.Write(simlolencePrice);
            w.Write(partTitleKey);
            w.Write(partDesptionKey);
            w.Write(uniqueTextureSpace);
            w.Write(bodyType);
            w.Write(unused1);
            w.Write(ageGender);
            w.Write(unused2);
            w.Write(unused3);
            swatchColorCode.UnParse(s);
            w.Write(buffResKey);
            w.Write(varientThumbnailKey);
            w.Write(nakedKey);
            w.Write(parentKey);
            w.Write(sortLayer);
            lodBlockList.UnParse(s);

            w.Write((byte)this.slotKey.Count);
            foreach (var b in this.slotKey) w.Write(b);
            w.Write(difussShadowKey);
            w.Write(shadowKey);
            w.Write(compositionMethod);
            w.Write(regionMapKey);
            w.Write(overrides);
            w.Write(normalMapKey);
            w.Write(specularMapKey);

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
        public uint OutfitGroup { get { return propertyID; } set { if (!value.Equals(propertyID)) propertyID = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(9)]
        public FlagList CASFlagList { get { return flagList; } set { if (!value.Equals(flagList)) flagList = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(13)]
        public SwatchColorList SwatchColorCode { get { return swatchColorCode; } set { if (!swatchColorCode.Equals(value)) swatchColorCode = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(15)]
        public LODBlockList LodBlockList { get { return lodBlockList; } set { if (!lodBlockList.Equals(value)) lodBlockList = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(17)]
        public CountedTGIBlockList TGIList { get { return tgiList; } set { if (!value.Equals(tgiList)) { OnResourceChanged(this, EventArgs.Empty); this.tgiList = value; } } }
        public String Value { get { return ValueBuilder; } }
        #endregion

        #region Sub-Class
        public class LODInfoEntryList : AHandlerElement, IEquatable<LODInfoEntryList>
        {
            const int recommendedApiVersion = 1;

            private CountedTGIBlockList tgiList;

            byte level;
            UInt32 unused;
            LodAssets[] lodAssetList { get; set; }
            byte[] lodKeyList { get; set; }
            ByteIndexList indexList;
            public LODInfoEntryList(int APIversion, EventHandler handler, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; }
            public LODInfoEntryList(int APIversion, EventHandler handler, Stream s, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; Parse(s); }
            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.level = r.ReadByte();

                this.unused = r.ReadUInt32();
                lodAssetList = new LodAssets[r.ReadByte()];
                for (int i = 0; i < lodAssetList.Length; i++) lodAssetList[i] = new LodAssets();
                lodKeyList = new byte[r.ReadByte()];


                byte[] byteList = new byte[r.ReadByte()];
                for (int i = 0; i < byteList.Length; i++)
                    byteList[i] = r.ReadByte();
                indexList = new ByteIndexList(handler, byteList, tgiList);


            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.level);
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
            public byte Level { get { return level; } set { if (!value.Equals(level)) { level = value; OnElementChanged(); } } }
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

            #region Sub-class
            public class LodAssets : AHandlerElement, IEquatable<LodAssets>
            {
                const int recommendedApiVersion = 1;
                // this part need to be implemented entirely
                public int sorting { get; set; }
                public int specLevel { get; set; }
                public int castShadow { get; set; }

                #region AHandlerElement Members
                public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
                public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
                #endregion

                #region Data I/O
                protected void Parse(Stream s)
                {
                    BinaryReader r = new BinaryReader(s);
                    this.sorting = r.ReadInt32();
                    this.specLevel = r.ReadInt32();
                    this.castShadow = r.ReadInt32();
                }
                #endregion

                #region IEquatable
                public bool Equals(LodAssets other)
                {
                    return this.sorting == other.sorting && this.specLevel == other.specLevel && this.castShadow == other.castShadow;
                }
                #endregion
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


        public class SwatchColor : AHandlerElement, IEquatable<SwatchColor>
        {
            private Color color;
            public SwatchColor(int APIversion, EventHandler handler, Stream s) :base(APIversion, handler)
            {
                BinaryReader r = new BinaryReader(s);
                this.color = Color.FromArgb(r.ReadInt32());
            }
            public SwatchColor(int APIversion, EventHandler handler, Color color) : base(APIversion, handler) { this.color = color; }
            public void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(this.color.ToArgb()); }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public bool Equals(SwatchColor other) { return other.Equals(this.color); }

            public Color Color { get { return this.color; } set { if (!color.Equals(value)) { this.color = value; OnElementChanged(); } } }
            public string Value { get { { return this.color.IsKnownColor? this.color.ToKnownColor().ToString(): this.color.Name; } } }
        }

        public class SwatchColorList : DependentList<SwatchColor>
        {
            public SwatchColorList(EventHandler handler) : base(handler) { }
            public SwatchColorList(EventHandler handler, Stream s) : base(handler) { Parse(s); }

            #region Data I/O
            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                byte count = r.ReadByte();
                for (int i = 0; i < count; i++)
                    base.Add(new SwatchColor(1, handler, s));
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((byte)base.Count);
                foreach (var color in this)
                    color.UnParse(s);
            }

            protected override SwatchColor CreateElement(Stream s) { return new SwatchColor(1, handler, Color.Black); }
            protected override void WriteElement(Stream s, SwatchColor element) { element.UnParse(s); }
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

        [Flags]
        public enum PramFlag : byte
        {
            ShowInCASDemo = 1 << 5,
            ShowInSimInfoDemo = 1 << 4,
            ShowInUI = 1 << 3,
            AllowForRandom = 1 << 2,
            DefaultThumbnailPart = 1 << 1,
            DefaultForBodyType = 1
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
