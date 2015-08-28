/***************************************************************************
 *  Copyright (C) 2014 by Inge Jones                                       *
 *                                                                         *
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
using System;
using System.Collections.Generic;
using System.IO;

using s4pi.Interfaces;

namespace CatalogResource
{
    public class COBJResource : AResource
    {
        #region Attributes

        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }
 
        private uint version = 0x19;
        private S4CatalogCommon commonA;
        private uint hashIndicator = 0x1;
        private uint hash01 = 0x811C9DC5;
        private uint hash02 = 0x811C9DC5;
        private uint hash03 = 0x811C9DC5;
        private uint unk02 = 0x2;
        private uint hash04 = 0x811C9DC5;
        private uint hash05 = 0x811C9DC5;
        private uint unkFlags01;
        private uint categoryFlags;
        private uint unkFlags03;
        private uint unkFlags04;
        private uint placementFlags;
        private ulong unkIID01;
        private byte unk03;
        private ulong unkIID02;
        private byte unk04;
        private ColorList colors;
        private byte unk05;
        private byte unk06;
        private byte unk07;
        private byte unk08;
        private byte unk09;
        private byte build_buy;
        private uint unk10;
        private uint unk11;
        private uint unk12;
        private uint unk13;
        
        #endregion Attributes ======================================================

        #region Content Fields

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "CatalogObject"; }
        }
        [ElementPriority(1)]
        public uint Version
        {
            get { return version; }
            set { if (version != value) { version = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(2)]
        public S4CatalogCommon CommonBlock
        {
            get { return commonA; }
            set { if (commonA != value) { commonA = new S4CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }

        [ElementPriority(5)]
        public uint HashIndicator
        {
            get { return hashIndicator; }
            set { if (hashIndicator != value) { hashIndicator = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public uint Hash01
        { 
            get { return hash01; }
            set { if (hash01 != value) { hash01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(7)]
        public uint Hash02
        {
            get { return hash02; }
            set { if (hash02 != value) { hash02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(8)]
        public uint Hash03
        {
            get { return hash03; }
            set { if (hash03 != value) { hash03 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(9)]
        public uint Unk02
        {
            get { return unk02; }
            set { if (unk02 != value) { unk02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(10)]
        public uint Hash04
        {
            get { return hash04; }
            set { if (hash04 != value) { hash04 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(11)]
        public uint Hash05
        {
            get { return hash05; }
            set { if (hash05 != value) { hash05 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(18)]
        public uint UnkFlags01
        {
            get { return unkFlags01; }
            set { if (unkFlags01 != value) { unkFlags01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(19)]
        public uint CategoryFlags
        {
            get { return categoryFlags; }
            set { if (categoryFlags != value) { categoryFlags = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(20)]
        public uint UnkFlags03
        {
            get { return unkFlags03; }
            set { if (unkFlags03 != value) { unkFlags03 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(22)]
        public uint UnkFlags04
        {
            get { return unkFlags04; }
            set { if (unkFlags04 != value) { unkFlags04 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(24)]
        public uint PlacementFlags
        {
            get { return placementFlags; }
            set { if (placementFlags != value) { placementFlags = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(25)]
        public ulong UnkIID01
        {
            get { return unkIID01; }
            set { if (unkIID01 != value) { unkIID01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(26)]
        public byte SlotWeight
        {
            get { return unk03; }
            set { if (unk03 != value) { unk03 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(27)]
        public ulong SwatchGrouping//UnkIID02
        {
            get { return unkIID02; }
            set { if (unkIID02 != value) { unkIID02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(28)]
        public byte Unk04
        {
            get { return unk04; }
            set { if (unk04 != value) { unk04 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(29)]
        public ColorList Colors
        {
            get { return colors; }
            set { if (colors != value) { colors = new ColorList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(30)]
        public byte Unk05
        {
            get { return unk05; }
            set { if (unk05 != value) { unk05 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(32)]
        public byte Unk06
        {
            get { return unk06; }
            set { if (unk06 != value) { unk06 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(34)]
        public byte Unk07
        {
            get { return unk07; }
            set { if (unk07 != value) { unk07 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(35)]
        public byte Unk08
        {
            get { return unk08; }
            set { if (unk08 != value) { unk08 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(36)]
        public byte Unk09
        {
            get { return unk09; }
            set { if (unk09 != value) { unk09 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(37)]
        public byte BuildBuy
        {
            get { return build_buy; }
            set { if (build_buy != value) { build_buy = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(38)]
        public uint Unk10
        {
            get
            {
                if (version < 0x00000019) throw new InvalidOperationException();
                return unk10;
            }
            set
            {
                if (version < 0x00000019) throw new InvalidOperationException();
                if (unk10 != value)
                {
                    unk10 = value; OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        [ElementPriority(39)]
        public uint Unk11
        {
            get
            {
                if (version < 0x00000019) throw new InvalidOperationException();
                return unk11;
            }
            set
            {
                if (version < 0x00000019) throw new InvalidOperationException();
                if (unk11 != value)
                {
                    unk11 = value; OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        [ElementPriority(40)]
        public uint Unk12
        {
            get
            {
                if (version < 0x00000019) throw new InvalidOperationException();
                return unk12;
            }
            set
            {
                if (version < 0x00000019) throw new InvalidOperationException();
                if (unk12 != value)
                {
                    unk12 = value; OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        [ElementPriority(41)]
        public uint Unk13
        {
            get
            {
                if (version < 0x00000019) throw new InvalidOperationException();
                return unk13;
            }
            set
            {
                if (version < 0x00000019) throw new InvalidOperationException();
                if (unk13 != value)
                {
                    unk13 = value; OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }
        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                if (this.version < 0x00000019)
                {
                    res.Remove("Unk10");
                    res.Remove("Unk11");
                    res.Remove("Unk12");
                    res.Remove("Unk13");
                }
                return res;
            }
        }


        #endregion ContentFields ===========================================================================

        #region Data I/O

        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.commonA = new S4CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged, s);
            this.hashIndicator = br.ReadUInt32();
            this.hash01 = br.ReadUInt32();
            this.hash02 = br.ReadUInt32();
            this.hash03 = br.ReadUInt32();
            this.unk02 = br.ReadUInt32();
            this.hash04 = br.ReadUInt32();
            this.hash05 = br.ReadUInt32();
            this.unkFlags01 = br.ReadUInt32();
            this.categoryFlags = br.ReadUInt32();
            this.unkFlags03 = br.ReadUInt32();
            this.unkFlags04 = br.ReadUInt32();
            this.placementFlags = br.ReadUInt32();
            this.unkIID01 = br.ReadUInt64();
            this.unk03 = br.ReadByte();
            this.unkIID02 = br.ReadUInt64();
            this.unk04 = br.ReadByte();
            this.colors = new ColorList(this.OnResourceChanged, s);
            this.unk05 = br.ReadByte();
            this.unk06 = br.ReadByte();
            this.unk07 = br.ReadByte();
            this.unk08 = br.ReadByte();
            this.unk09 = br.ReadByte();
            this.build_buy = br.ReadByte();
            if (this.version >= 0x19)
            {
                this.unk10 = br.ReadUInt32();
                this.unk11 = br.ReadUInt32();
                this.unk12 = br.ReadUInt32();
                this.unk13 = br.ReadUInt32();
            }
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            if (this.commonA == null) { this.commonA = new S4CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged); }
            this.commonA.UnParse(s);
            bw.Write(this.hashIndicator);
            bw.Write(this.hash01);
            bw.Write(this.hash02);
            bw.Write(this.hash03);
            bw.Write(this.unk02);
            bw.Write(this.hash04);
            bw.Write(this.hash05);
            bw.Write(this.unkFlags01);
            bw.Write(this.categoryFlags);
            bw.Write(this.unkFlags03);
            bw.Write(this.unkFlags04);
            bw.Write(this.placementFlags);
            bw.Write(this.unkIID01);
            bw.Write(this.unk03);
            bw.Write(this.unkIID02);
            bw.Write(this.unk04);
            if (this.colors == null) { this.colors = new ColorList(this.OnResourceChanged); }
            this.colors.UnParse(s);
            bw.Write(this.unk05);
            bw.Write(this.unk06);
            bw.Write(this.unk07);
            bw.Write(this.unk08);
            bw.Write(this.unk09);
            bw.Write(this.build_buy);
            if (this.version >= 0x19)
            {
                bw.Write(this.unk10);
                bw.Write(this.unk11);
                bw.Write(this.unk12);
                bw.Write(this.unk13);
            }
            return s;
        }

        #endregion DataIO ================================

        #region Constructors
        public COBJResource(int APIversion, Stream s)
            : base(APIversion, s)
        {
            if (s == null || s.Length == 0)
            {
                s = UnParse();
                OnResourceChanged(this, EventArgs.Empty);
            }
            s.Position = 0;
            this.Parse(s);
        }
        #endregion
    }

    public class COBJResourceHandler : AResourceHandler
    {
        public COBJResourceHandler()
        {
            this.Add(typeof(COBJResource), new List<string>(new[] { "0x319E4F1D" }));
        }
    }
}