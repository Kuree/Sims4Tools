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
    public class CFNDResource : AResource
    {
        #region Attributes

        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }
        private uint version = 0x6;
        private S4CatalogCommon commonA;
        private byte unk01;
        private byte unk02;
        private TGIBlock modlRef1;
        private uint materialVariant;
        private ulong swatchGrouping;
        private float float1;
        private float float2;
        private TGIBlock trimRef;
        private TGIBlock modlRef2;
        private ColorList colors;

        #endregion Attributes ================================================================

        #region Content Fields

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "CTProductFoundation"; }
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
        public byte Unk01
        {
            get { return unk01; }
            set { if (unk01 != value) { unk01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(4)]
        public byte Unk02
        {
            get { return unk02; }
            set { if (unk02 != value) { unk02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(5)]
        public TGIBlock MODLRef1
        {
            get { return modlRef1; }
            set { if (!modlRef1.Equals(value)) { modlRef1 = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public uint MaterialVariant
        {
            get { return materialVariant; }
            set { if (materialVariant != value) { materialVariant = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(7)]
        public ulong SwatchGrouping
        {
            get { return swatchGrouping; }
            set { if (swatchGrouping != value) { swatchGrouping = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(8)]
        public float Float1
        {
            get { return float1; }
            set { if (float1 != value) { float1 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(9)]
        public float Float2
        {
            get { return float2; }
            set { if (float2 != value) { float2 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }

        [ElementPriority(10)]
        public TGIBlock TRIMRef
        {
            get { return trimRef; }
            set { if (!trimRef.Equals(value)) { trimRef = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(11)]
        public TGIBlock MODLRef2
        {
            get { return modlRef2; }
            set { if (!modlRef2.Equals(value)) { modlRef2 = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(12)]
        public ColorList Colors
        {
            get { return colors; }
            set { if (!colors.Equals( value)) { colors = new ColorList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
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
            this.unk01 = br.ReadByte();
            this.unk02 = br.ReadByte();
            this.modlRef1 = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, s);
            this.materialVariant = br.ReadUInt32();
            this.swatchGrouping = br.ReadUInt64();
            this.float1 = br.ReadSingle();
            this.float2 = br.ReadSingle();
            this.trimRef = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, s);
            this.modlRef2 = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, s);
            this.colors = new ColorList(this.OnResourceChanged, s);
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
            if (this.modlRef1 == null) { this.modlRef1 = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG); }
            this.modlRef1.UnParse(s);
            bw.Write(this.materialVariant);
            bw.Write(this.swatchGrouping);
            bw.Write(this.float1);
            bw.Write(this.float2);
            if (this.trimRef == null) { this.trimRef = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG); }
            this.trimRef.UnParse(s);
            if (this.modlRef2 == null) { this.modlRef2 = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG); }
            this.modlRef2.UnParse(s);
            if (this.colors == null) { this.colors = new ColorList(this.OnResourceChanged); }
            this.colors.UnParse(s);
            return s;
        }

        #endregion DataIO ===================================================

        #region Constructors
        public CFNDResource(int APIversion, Stream s)
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

    public class CFNDResourceHandler : AResourceHandler
    {
        public CFNDResourceHandler()
        {
            this.Add(typeof(CFNDResource), new List<string>(new string[] { "0x2FAE983E", }));
        }
    }
}