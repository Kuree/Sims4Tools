/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace CASPartResource
{
    public class CASPartResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        


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
        ParmFlag parmFlags;
        ExcludePartFlag excludePartFlags;
        uint excludeModifierRegionFlags;
        FlagList flagList;
        uint simlolencePrice;
        uint partTitleKey;
        uint partDesptionKey;
        byte uniqueTextureSpace;
        int bodyType;
        int unused1;
        AgeGenderFlags ageGender;
        byte unused2;
        byte unused3;
        SwatchColorList swatchColorCode;
        byte buffResKey;
        byte varientThumbnailKey;
        ulong voiceEffectHash;
        byte nakedKey;
        byte parentKey;
        int sortLayer;
        LODBlockList lodBlockList;
        SimpleList<byte> slotKey;
        byte diffuseShadowKey;
        byte shadowKey;
        byte compositionMethod;
        byte regionMapKey;
        byte overrides;
        byte normalMapKey;
        byte specularMapKey;
        uint sharedUVMapSpace;
        private CountedTGIBlockList tgiList;
        #endregion

        public CASPartResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null || stream.Length == 0) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

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
            this.parmFlags = (ParmFlag)r.ReadByte();
            this.excludePartFlags = (ExcludePartFlag)r.ReadUInt64();
            this.excludeModifierRegionFlags = r.ReadUInt32();

            flagList = new FlagList(OnResourceChanged, s);

            this.simlolencePrice = r.ReadUInt32();
            this.partTitleKey = r.ReadUInt32();
            this.partDesptionKey = r.ReadUInt32();
            this.uniqueTextureSpace = r.ReadByte();
            this.bodyType = r.ReadInt32();
            this.unused1 = r.ReadInt32();
            this.ageGender = (AgeGenderFlags)r.ReadUInt32();
            this.unused2 = r.ReadByte();
            this.unused3 = r.ReadByte();

            swatchColorCode = new SwatchColorList(OnResourceChanged, s);

            this.buffResKey = r.ReadByte();
            this.varientThumbnailKey = r.ReadByte();
            if (this.version >= 0x1C) this.voiceEffectHash = r.ReadUInt64();
            this.nakedKey = r.ReadByte();
            this.parentKey = r.ReadByte();
            this.sortLayer = r.ReadInt32();

            // Don't move any of this before the -----
            // TGI block list
            long currentPosition = r.BaseStream.Position;
            r.BaseStream.Position = tgiOffset;
            byte count4 = r.ReadByte();
            tgiList = new CountedTGIBlockList(OnResourceChanged, "IGT", count4, s);
            r.BaseStream.Position = currentPosition;
            lodBlockList = new LODBlockList(null, s, tgiList);
            //-------------

            byte count = r.ReadByte();
            this.slotKey = new SimpleList<byte>(null);
            for (byte i = 0; i < count; i++) this.slotKey.Add(r.ReadByte());

            this.diffuseShadowKey = r.ReadByte();
            this.shadowKey = r.ReadByte();
            this.compositionMethod = r.ReadByte();
            this.regionMapKey = r.ReadByte();
            this.overrides = r.ReadByte();
            this.normalMapKey = r.ReadByte();
            this.specularMapKey = r.ReadByte();
            if (this.version >= 0x1B) this.sharedUVMapSpace = r.ReadUInt32();


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
            w.Write((ulong)excludePartFlags);
            w.Write(excludeModifierRegionFlags);
            if (this.flagList == null) this.flagList = new FlagList(OnResourceChanged);
            flagList.UnParse(s);
            w.Write(simlolencePrice);
            w.Write(partTitleKey);
            w.Write(partDesptionKey);
            w.Write(uniqueTextureSpace);
            w.Write(bodyType);
            w.Write(unused1);
            w.Write((uint)ageGender);
            w.Write(unused2);
            w.Write(unused3);
            if (this.swatchColorCode == null) this.swatchColorCode = new SwatchColorList(OnResourceChanged);
            swatchColorCode.UnParse(s);
            w.Write(buffResKey);
            w.Write(varientThumbnailKey);
            if (this.version >= 0x1C) w.Write(voiceEffectHash);
            w.Write(nakedKey);
            w.Write(parentKey);
            w.Write(sortLayer);
            if (this.lodBlockList == null) this.lodBlockList = new LODBlockList(OnResourceChanged);
            lodBlockList.UnParse(s);
            if (this.slotKey == null) this.slotKey = new SimpleList<byte>(OnResourceChanged);
            w.Write((byte)this.slotKey.Count);
            foreach (var b in this.slotKey) w.Write(b);
            w.Write(diffuseShadowKey);
            w.Write(shadowKey);
            w.Write(compositionMethod);
            w.Write(regionMapKey);
            w.Write(overrides);
            w.Write(normalMapKey);
            w.Write(specularMapKey);
            if (this.version >= 0x1B) w.Write(sharedUVMapSpace);
            long tgiPosition = w.BaseStream.Position;
            w.BaseStream.Position = 4;
            w.Write(tgiPosition - 8);
            w.BaseStream.Position = tgiPosition;
            if (this.tgiList == null) this.tgiList = new CountedTGIBlockList(OnResourceChanged);
            w.Write((byte)tgiList.Count);
            foreach (var tgi in tgiList) tgi.UnParse(s);

            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(0)]
        public uint Version { get { return version; } set { if (value != version)version = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(1)]
        public uint TGIoffset { get { return tgiOffset; } }
        [ElementPriority(2)]
        public uint PresetCount { get { return presetCount; } set { if (value != presetCount) presetCount = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(3)]
        public string Name { get { return name; } set { if (!value.Equals(name)) name = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(4)]
        public float SortPriority { get { return sortPriority; } set { if (!value.Equals(sortPriority)) sortPriority = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(5)]
        public UInt16 SecondarySortIndex { get { return secondarySortIndex; } set { if (!value.Equals(secondarySortIndex)) secondarySortIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(6)]
        public uint PropertyID { get { return propertyID; } set { if (!value.Equals(propertyID)) propertyID = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(7)]
        public uint AuralMaterialHash { get { return auralMaterialHash; } set { if (!value.Equals(this.auralMaterialHash)) { this.auralMaterialHash = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(8)]
        public ParmFlag ParmFlags { get { return parmFlags; } set { if (!value.Equals(parmFlags)) parmFlags = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(9)]
        public ExcludePartFlag ExcludePartFlags { get { return excludePartFlags; } set { if (!value.Equals(excludePartFlags)) excludePartFlags = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(10)]
        public uint ExcludeModifierRegionFlags { get { return excludeModifierRegionFlags; } set { if (!value.Equals(excludeModifierRegionFlags)) excludeModifierRegionFlags = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(11)]
        public FlagList CASFlagList { get { return flagList; } set { if (!value.Equals(flagList)) flagList = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(12)]
        public uint SimlolencePrice { get { return simlolencePrice; } set { if (!value.Equals(simlolencePrice)) SimlolencePrice = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(13)]
        public uint PartTitleKey { get { return partTitleKey; } set { if (!value.Equals(partTitleKey)) partTitleKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(14)]
        public uint PartDesptionKey { get { return partDesptionKey; } set { if (!value.Equals(partDesptionKey)) partDesptionKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(15)]
        public byte UniqueTextureSpace { get { return uniqueTextureSpace; } set { if (!value.Equals(uniqueTextureSpace)) uniqueTextureSpace = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(16)]
        public int BodyType { get { return bodyType; } set { if (!value.Equals(bodyType)) bodyType = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(17)]
        public int Unused1 { get { return unused1; } set { if (!value.Equals(unused1)) unused1 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(18)]
        public AgeGenderFlags AgeGender { get { return ageGender; } set { if (!value.Equals(ageGender)) ageGender = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(19)]
        public byte Unused2 { get { return unused2; } set { if (!value.Equals(unused2)) unused2 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(20)]
        public byte Unused3 { get { return unused3; } set { if (!value.Equals(unused3)) unused3 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(21)]
        public SwatchColorList SwatchColorCode { get { return swatchColorCode; } set { if (!swatchColorCode.Equals(value)) swatchColorCode = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(22), TGIBlockListContentField("TGIList")]
        public byte BuffResKey { get { return buffResKey; } set { if (!value.Equals(buffResKey)) buffResKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(23), TGIBlockListContentField("TGIList")]
        public byte VarientThumbnailKey { get { return varientThumbnailKey; } set { if (!value.Equals(varientThumbnailKey)) varientThumbnailKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(23)]
        public ulong VoiceEffectHash { get { return voiceEffectHash; } set { if (!value.Equals(voiceEffectHash)) voiceEffectHash = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(24), TGIBlockListContentField("TGIList")]
        public byte NakedKey { get { return nakedKey; } set { if (!value.Equals(nakedKey)) nakedKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(25), TGIBlockListContentField("TGIList")]
        public byte ParentKey { get { return parentKey; } set { if (!value.Equals(parentKey)) parentKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(26)]
        public int SortLayer { get { return sortLayer; } set { if (!value.Equals(sortLayer)) sortLayer = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(27)]
        public LODBlockList LodBlockList { get { return lodBlockList; } set { if (!lodBlockList.Equals(value)) lodBlockList = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(28), TGIBlockListContentField("TGIList")]
        public SimpleList<byte> SlotKey { get { return slotKey; } set { if (!value.Equals(slotKey)) slotKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(29), TGIBlockListContentField("TGIList")]
        public byte DiffuseShadowKey { get { return diffuseShadowKey; } set { if (!value.Equals(diffuseShadowKey)) diffuseShadowKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(30), TGIBlockListContentField("TGIList")]
        public byte ShadowKey { get { return shadowKey; } set { if (!value.Equals(shadowKey)) shadowKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(31), TGIBlockListContentField("TGIList")]
        public byte CompositionMethod { get { return compositionMethod; } set { if (!value.Equals(compositionMethod)) compositionMethod = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(32), TGIBlockListContentField("TGIList")]
        public byte RegionMapKey { get { return regionMapKey; } set { if (!value.Equals(regionMapKey)) regionMapKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(33), TGIBlockListContentField("TGIList")]
        public byte Overrides { get { return overrides; } set { if (!value.Equals(overrides)) overrides = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(34), TGIBlockListContentField("TGIList")]
        public byte NormalMapKey { get { return normalMapKey; } set { if (!value.Equals(normalMapKey)) normalMapKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(35), TGIBlockListContentField("TGIList")]
        public byte SpecularMapKey { get { return specularMapKey; } set { if (!value.Equals(specularMapKey)) specularMapKey = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(36)]
        public uint SharedUVMapSpace { 
            get { if (this.version < 0x1B) { throw new InvalidOperationException("Version not supported"); } else { return this.sharedUVMapSpace; }}
            set { if (version < 0x1B) { throw new InvalidOperationException("Version not Supported"); } this.sharedUVMapSpace = value; }
        }

        [ElementPriority(37)]
        public CountedTGIBlockList TGIList { get { return tgiList; } set { if (!value.Equals(tgiList)) { OnResourceChanged(this, EventArgs.Empty); this.tgiList = value; } } }
        public String Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                if (this.version < 0x1B) { res.Remove("SharedUVMapSpace"); }
                if (this.version < 0x1C) { res.Remove("VoiceEffectHash"); }
                return res;
            }
        }
        #endregion

        #region Sub-Class
        public class LODInfoEntry : AHandlerElement, IEquatable<LODInfoEntry>
        {
            const int recommendedApiVersion = 1;

            private CountedTGIBlockList tgiList;

            byte level;
            UInt32 unused;
            LODAssetList lodAssetList;
            ByteIndexList lodKeyList;

            public LODInfoEntry(int APIversion, EventHandler handler) : base(APIversion, handler) { this.UnParse(new MemoryStream()); }
            public LODInfoEntry(int APIversion, EventHandler handler, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; }
            public LODInfoEntry(int APIversion, EventHandler handler, Stream s, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; Parse(s); }
            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.level = r.ReadByte();

                this.unused = r.ReadUInt32();

                lodAssetList = new LODAssetList(null, s);

                byte[] byteList = new byte[r.ReadByte()];
                for (int i = 0; i < byteList.Length; i++)
                    byteList[i] = r.ReadByte();
                lodKeyList = new ByteIndexList(handler, byteList, tgiList);


            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.level);
                w.Write(this.unused);

                if (this.lodAssetList == null) { this.lodAssetList = new LODAssetList(handler); }
                this.lodAssetList.UnParse(s);

                if (this.lodKeyList == null) { this.lodKeyList = new ByteIndexList(handler, new CountedTGIBlockList(handler)); }
                w.Write((byte)lodKeyList.Count);
                foreach (byte b in lodKeyList)
                    w.Write(b);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region Content Fields
            [ElementPriority(0)]
            public byte Level { get { return level; } set { if (!value.Equals(level)) { level = value; OnElementChanged(); } } }
            public uint Unused { get { return unused; } set { if (!value.Equals(this.unused)) { this.unused = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public LODAssetList LodAssetList { get { return this.lodAssetList; } set { if (!value.Equals(this.lodAssetList)) { this.lodAssetList = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public ByteIndexList LODKeyList { get { return lodKeyList; } set { if (!value.Equals(lodKeyList)) value = lodKeyList; OnElementChanged(); } }
            public string Value { get { return ValueBuilder; } }

            #endregion

            #region IEquatable
            public bool Equals(LODInfoEntry other)
            {
                return this.level == other.level && this.unused == other.unused && this.lodKeyList.Equals(other.lodKeyList);
            }
            #endregion

            #region Sub-class
            public class LodAssets : AHandlerElement, IEquatable<LodAssets>
            {
                #region Attribute
                const int recommendedApiVersion = 1;

                int sorting;
                int specLevel;
                int castShadow;

                #endregion

                #region Constructor
                public LodAssets(int APIversion, EventHandler handler) : base(APIversion, handler) { }
                public LodAssets(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

                #endregion

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

                public void UnParse(Stream s)
                {
                    BinaryWriter w = new BinaryWriter(s);
                    w.Write(this.sorting);
                    w.Write(this.specLevel);
                    w.Write(this.castShadow);
                }
                #endregion

                #region IEquatable
                public bool Equals(LodAssets other)
                {
                    return this.sorting == other.sorting && this.specLevel == other.specLevel && this.castShadow == other.castShadow;
                }
                #endregion


                #region Content Fields
                public int Sorting { get { return this.sorting; } set { if (!value.Equals(this.sorting)) { OnElementChanged(); this.sorting = value; } } }
                public int SpecLevel { get { return this.specLevel; } set { if (!value.Equals(this.specLevel)) { OnElementChanged(); this.specLevel = value; } } }
                public int CastShadow { get { return this.castShadow; } set { if (!value.Equals(this.castShadow)) { OnElementChanged(); this.castShadow = value; } } }

                public string Value { get { return ValueBuilder; } }
                #endregion
            }

            public class LODAssetList : DependentList<LodAssets>
            {
                public LODAssetList(EventHandler handler) : base(handler) { }
                public LODAssetList(EventHandler handler, Stream s) : base(handler) { Parse(s); }

                #region Data I/O
                protected override void Parse(Stream s)
                {
                    BinaryReader r = new BinaryReader(s);
                    byte count = r.ReadByte();
                    for (int i = 0; i < count; i++)
                        base.Add(new LodAssets(1, handler, s));
                }

                public override void UnParse(Stream s)
                {
                    BinaryWriter w = new BinaryWriter(s);
                    w.Write((byte)base.Count);
                    foreach (var asset in this)
                        asset.UnParse(s);
                }

                protected override LodAssets CreateElement(Stream s) { return new LodAssets(1, handler, s); }
                protected override void WriteElement(Stream s, LodAssets element) { element.UnParse(s); }
                #endregion
            }
            #endregion
        }

        public class LODBlockList : DependentList<LODInfoEntry>
        {
            #region Attributes
            CountedTGIBlockList tgiList;
            #endregion

            #region Constructors
            public LODBlockList(EventHandler handler) : this(handler, new CountedTGIBlockList(handler)) { }
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
                    base.Add(new LODInfoEntry(1, handler, s, tgiList));
                }
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((byte)base.Count);
                foreach (var unknownClass in this)
                {
                    unknownClass.UnParse(s);
                }
            }

            protected override LODInfoEntry CreateElement(Stream s) { return new LODInfoEntry(1, handler, tgiList); }
            protected override void WriteElement(Stream s, LODInfoEntry element) { element.UnParse(s); }
            #endregion

        }


        public class SwatchColor : AHandlerElement, IEquatable<SwatchColor>
        {
            private Color color;
            public SwatchColor(int APIversion, EventHandler handler, Stream s)
                : base(APIversion, handler)
            {
                BinaryReader r = new BinaryReader(s);
                this.color = Color.FromArgb(r.ReadInt32());
            }
            public SwatchColor(int APIversion, EventHandler handler, Color color) : base(APIversion, handler) { this.color = color; }
            public SwatchColor(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(this.color.ToArgb()); }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public bool Equals(SwatchColor other) { return other.Equals(this.color); }

            public Color Color { get { return this.color; } set { if (!color.Equals(value)) { this.color = value; OnElementChanged(); } } }
            public string Value { get { { return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B); } } } // Color code consists with HTML code
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
            CASPFlags flagCategory;
            CASPFlagValues flagValue;

            public Flag(int APIversion, EventHandler handler, Stream s)
                : base(APIversion, handler)
            {
                BinaryReader r = new BinaryReader(s);
                this.flagCategory = (CASPFlags)r.ReadUInt16();
                this.flagValue = (CASPFlagValues)r.ReadUInt16();
            }

            public Flag(int APIversion, EventHandler handler) : base(APIversion, handler) { }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((ushort)this.flagCategory);
                w.Write((ushort)this.flagValue);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public bool Equals(Flag other)
            {
                return this.flagValue == other.flagValue && this.flagCategory == other.flagCategory;
            }

            [ElementPriority(0)]
            public CASPFlags FlagCatagory { get { return this.flagCategory; } set { if (value != this.flagCategory) { OnElementChanged(); this.flagCategory = value; } } }
            [ElementPriority(1)]
            public CASPFlagValues FlagValue { get { return this.flagValue; } set { if (value != this.flagValue) { OnElementChanged(); this.flagValue = value; } } }

            public string Value { get { return ValueBuilder; } }
        }

        public class FlagList : DependentList<Flag>
        {
            public FlagList(EventHandler handler) : base(handler) { }
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

            protected override Flag CreateElement(Stream s) { return new Flag(recommendedApiVersion, handler, s); }
            protected override void WriteElement(Stream s, Flag element) { element.UnParse(s); }
        }

        #endregion
    }

    public class CASPartResourceHandler : AResourceHandler
    {
        public CASPartResourceHandler()
        {
            if (s4pi.Settings.Settings.IsTS4)
                this.Add(typeof(CASPartResource), new List<string>(new string[] { "0x034AEECB", }));
        }
    }

}
