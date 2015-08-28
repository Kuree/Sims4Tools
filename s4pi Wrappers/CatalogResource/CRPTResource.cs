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
    public class CRPTResource : AResource
    {
        #region Attributes

        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }
        private uint version = 0x5;
        private S4CatalogCommon commonA;
        private uint unk01;
        private TGIBlock matdRef;
        private TGIBlock floorRef;
        private ulong unkIID01;
        private ColorList colors;

        #endregion Attributes ================================================================

        #region Content Fields

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "RoofPattern"; }
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
        [ElementPriority(4)]
        public uint Unk01
        {
            get { return unk01; }
            set { if (unk01 != value) { unk01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public TGIBlock MATDRef
        {
            get { return matdRef; }
            set { if (matdRef != value) { matdRef = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(7)]
        public TGIBlock FloorRef
        {
            get { return floorRef; }
            set { if (floorRef != value) { floorRef = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(8)]
        public ulong SwatchGrouping//UnkIID01
        {
            get { return unkIID01; }
            set { if (unkIID01 != value) { unkIID01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(9)]
        public ColorList Colors
        {
            get { return colors; }
            set { if (colors != value) { colors = new ColorList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
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
            this.matdRef = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG,s);
            this.floorRef = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG, s);
            this.unkIID01 = br.ReadUInt64();
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
            if (this.matdRef == null) { this.matdRef = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG); }
            this.matdRef.UnParse(s);
            if (this.floorRef == null) { this.floorRef = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG); }
            this.floorRef.UnParse(s);
            bw.Write(this.unkIID01);
            if (this.colors == null) { this.colors = new ColorList(this.OnResourceChanged); }
            this.colors.UnParse(s);
            return s;
        }

        #endregion DataIO ===================================================

        #region Constructors
        public CRPTResource(int APIversion, Stream s)
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

    /// <summary>
    /// ResourceHandler for CFLRResource wrapper
    /// </summary>
    public class CRPTResourceHandler : AResourceHandler
    {
        public CRPTResourceHandler()
        {
            this.Add(typeof(CRPTResource), new List<string>(new[] { "0xF1EDBD86" }));
        }
    }
}