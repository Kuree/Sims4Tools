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

// This code is based on Snaitf's analyze

using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s4pi.Miscellaneous
{
    public class StyleLookResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        static bool checking = s4pi.Settings.Settings.Checking;

        private uint version;
        private AgeGenderFlags ageGender;
        private ulong groupingID;
        private byte unknown1;
        private ulong simOutfitReference;
        private ulong textureReference;
        private ulong simDataReference;
        private uint nameHash;
        private uint descHash;
        private DataBlobHandler unknown2;
        private uint unknown3;
        private ulong animationReference1;
        private string animationStateName1;
        private ulong animationReference2;
        private string animationStateName2;
        private SwatchColorList colorList;
        FlagList flagList;
        private byte unknown6;

        public StyleLookResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null || stream.Length == 0) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }
        
        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            s.Position = 0;
            this.version = r.ReadUInt32();
            this.ageGender = (AgeGenderFlags)r.ReadUInt32();
            this.groupingID = r.ReadUInt64();
            this.unknown1 = r.ReadByte();
            this.simOutfitReference = r.ReadUInt64();
            this.textureReference = r.ReadUInt64();
            this.simDataReference = r.ReadUInt64();
            this.nameHash = r.ReadUInt32();
            this.descHash = r.ReadUInt32();
            this.unknown2 = new DataBlobHandler(recommendedApiVersion, OnResourceChanged, r.ReadBytes(14));
            this.unknown3 = r.ReadUInt32();
            this.animationReference1 = r.ReadUInt64();
            this.animationStateName1 = System.Text.Encoding.ASCII.GetString(r.ReadBytes(r.ReadInt32()));
            this.animationReference2 = r.ReadUInt64();
            this.animationStateName2 = System.Text.Encoding.ASCII.GetString(r.ReadBytes(r.ReadInt32()));
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            flagList = new FlagList(OnResourceChanged, s);
            this.unknown6 = r.ReadByte();
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(this.version);
            w.Write((uint)this.ageGender);
            w.Write(this.groupingID);
            w.Write(this.unknown1);
            w.Write(this.simOutfitReference);
            w.Write(this.textureReference);
            w.Write(this.simDataReference);
            w.Write(this.nameHash);
            w.Write(this.descHash);
            this.unknown2.UnParse(ms);
            w.Write(this.unknown3);
            w.Write(this.animationReference1);
            w.Write(Encoding.ASCII.GetByteCount(this.animationStateName1));
            w.Write(Encoding.ASCII.GetBytes(this.animationStateName1));
            w.Write(this.animationReference2);
            w.Write(Encoding.ASCII.GetByteCount(this.animationStateName2));
            w.Write(Encoding.ASCII.GetBytes(this.animationStateName2));
            this.colorList.UnParse(ms);
            if (this.flagList == null) this.flagList = new FlagList(OnResourceChanged);
            flagList.UnParse(ms);
            w.Write(this.unknown6);
            ms.Position = 0;
            return ms;
        }
        #endregion

        #region Sub Types
        [Flags]
        public enum AgeGenderFlags : uint
        {
            Unknown1 = 0x00000001,
            Unknown2 = 0x00000002,
            Child = 0x00000004,
            Teen = 0x00000008,
            YoungAdult = 0x00000010,
            Adult = 0x00000020,
            Elder = 0x00000040,
            Male = 0x00001000,
            Female = 0x00002000
        }
        #endregion

        #region Content Fields
        public string Value { get { return ValueBuilder; } }
        [ElementPriority(0)]
        public uint Version { get { return this.version; } set { if (!this.version.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.version = value; } } }
        [ElementPriority(1)]
        public AgeGenderFlags AgeGender { get { return this.ageGender; } set { if (!this.ageGender.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ageGender = value; } } }
        [ElementPriority(2)]
        public ulong GroupingID { get { return this.groupingID; } set { if (!this.groupingID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.groupingID = value; } } }
        [ElementPriority(3)]
        public byte Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(4)]
        public ulong SimOutfitReference { get { return this.simOutfitReference; } set { if (!this.simOutfitReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.simOutfitReference = value; } } }
        [ElementPriority(5)]
        public ulong TextureReference { get { return this.textureReference; } set { if (!this.textureReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.textureReference = value; } } }
        [ElementPriority(6)]
        public ulong SimDataReference { get { return this.simDataReference; } set { if (!this.simDataReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.simDataReference = value; } } }
        [ElementPriority(7)]
        public uint NameHash { get { return this.nameHash; } set { if (!this.nameHash.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.nameHash = value; } } }
        [ElementPriority(8)]
        public uint DescHash { get { return this.descHash; } set { if (!this.descHash.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.descHash = value; } } }
        [ElementPriority(9)]
        public DataBlobHandler Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(10)]
        public uint Unknown3 { get { return this.unknown3; } set { if (!this.unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(11)]
        public ulong AnimationReference1 { get { return this.animationReference1; } set { if (!this.animationReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.animationReference1 = value; } } }
        [ElementPriority(12)]
        public string AnimationStateName1 { get { return this.animationStateName1; } set { if (!this.animationStateName1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.animationStateName1 = value; } } }
        [ElementPriority(13)]
        public ulong AnimationReference2 { get { return this.animationReference2; } set { if (!this.animationReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.animationReference2 = value; } } }
        [ElementPriority(14)]
        public string AnimationStateName2 { get { return this.animationStateName2; } set { if (!this.animationStateName2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.animationStateName2 = value; } } }
        [ElementPriority(15)]
        public SwatchColorList ColorList { get { return this.colorList; } set { if (!this.colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(16)]
        public FlagList CASFlagList { get { return flagList; } set { if (!value.Equals(flagList)) flagList = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(17)]
        public byte Unknown6 { get { return this.unknown6; } set { if (!this.unknown6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown6 = value; } } }
        #endregion

        public class Flag : AHandlerElement, IEquatable<Flag>
        {
            CASPFlags flagCategory;
            ushort flagValue;

            public Flag(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Flag(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Flag(int APIversion, EventHandler handler, Flag basis) : this(APIversion, handler, basis.flagCategory, basis.flagValue) { }
            public Flag(int APIversion, EventHandler handler, CASPFlags flagCategory, ushort flagValue)
                : base(APIversion, handler)
            {
                this.flagCategory = flagCategory;
                this.flagValue = flagValue;
            }
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.flagCategory = (CASPFlags)r.ReadUInt16();
                this.flagValue = r.ReadUInt16();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((ushort)this.flagCategory);
                w.Write(this.flagValue);
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
            public ushort FlagValue { get { return this.flagValue; } set { if (value != this.flagValue) { OnElementChanged(); this.flagValue = value; } } }

            public string Value { get { return ValueBuilder; } }
        }

        public class FlagList : DependentList<Flag>
        {
            public FlagList(EventHandler handler) : base(handler) { }
            public FlagList(EventHandler handler, Stream s) : base(handler, s) { }
            public FlagList(EventHandler handler, IEnumerable<Flag> ilt) : base(handler, ilt) { }

            /*
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
             * */

            protected override Flag CreateElement(Stream s) { return new Flag(recommendedApiVersion, handler, s); }
            protected override void WriteElement(Stream s, Flag element) { element.UnParse(s); }
        }

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


    }


    public class StyleLookResourceHandler : AResourceHandler
    {
        public StyleLookResourceHandler()
        {
            this.Add(typeof(StyleLookResource), new List<string>(new string[] { "0x71BDB8A2", }));
        }
    }
}
