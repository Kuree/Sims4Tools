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
 *  s4pi is distributed in the hope that it will be useful,                *
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
    public class STRMResource : AResource
    {

        #region Attributes

        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }
        private uint version = 0x6;
        private S4CatalogCommon commonA;
        private uint unk01;
        private uint unk02;
        private uint unk03;
        private uint unk04;
        private uint unk05;
        private ulong swatchGrouping;
        private ColorList colors;
        private TGIBlock unkRef1;
        private uint unk06;
        private uint unk07;

        #endregion Attributes ================================================================

        #region Content Fields

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "StyledRoom"; }
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
            set { if (!commonA.Equals(value)) { commonA = new S4CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(3)]
        public uint Unk01
        {
            get { return unk01; }
            set { if (unk01 != value) { unk01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(4)]
        public uint Unk02
        {
            get { return unk02; }
            set { if (unk02 != value) { unk02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(5)]
        public uint Unk03
        {
            get { return unk03; }
            set { if (unk03 != value) { unk03 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public uint Unk04
        {
            get { return unk04; }
            set { if (unk04 != value) { unk04 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(7)]
        public uint Unk05
        {
            get { return unk05; }
            set { if (unk05 != value) { unk05 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(8)]
        public ulong SwatchGrouping
        {
            get { return swatchGrouping; }
            set { if (swatchGrouping != value) { swatchGrouping = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(9)]
        public ColorList Colors
        {
            get { return colors; }
            set { if (!colors.Equals( value)) { colors = new ColorList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(10)]
        public TGIBlock UnkRef1
        {
            get { return unkRef1; }
            set { if (!unkRef1.Equals(value)) { unkRef1 = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(11)]
        public uint Unk06
        {
            get { return unk06; }
            set { if (unk06 != value) { unk06 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(12)]
        public uint Unk07
        {
            get { return unk07; }
            set { if (unk07 != value) { unk07 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }


        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                return res;
            }
        }

        #endregion

        #region Data I/O

        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.commonA = new S4CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged, s);
            this.unk01 = br.ReadUInt32();
            this.unk02 = br.ReadUInt32();
            this.unk03 = br.ReadUInt32();
            this.unk04 = br.ReadUInt32();
            this.unk05 = br.ReadUInt32();
            this.swatchGrouping = br.ReadUInt64();
            this.colors = new ColorList(this.OnResourceChanged, s);
            this.unkRef1 = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, s);
            this.unk06 = br.ReadUInt32();
            this.unk07 = br.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            if (this.commonA == null) { this.commonA = new S4CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged); }
            this.commonA.UnParse(s);
            bw.Write(this.unk01);
            bw.Write(this.unk02);
            bw.Write(this.unk03);
            bw.Write(this.unk04);
            bw.Write(this.unk05);
            bw.Write(this.swatchGrouping);
            if (this.colors == null) { this.colors = new ColorList(this.OnResourceChanged); }
            this.colors.UnParse(s);
            if (this.unkRef1 == null) { this.unkRef1 = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG); }
            this.unkRef1.UnParse(s);
            bw.Write(this.unk06);
            bw.Write(this.unk07);
            return s;
        }

        #endregion DataIO ===================================================

        #region Constructors
        public STRMResource(int APIversion, Stream s)
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
    public class STLRResourceHandler : AResourceHandler
    {
        public STLRResourceHandler()
        {
            this.Add(typeof(STRMResource), new List<string>(new string[] { "0x74050B1F", }));
        }
    }
}