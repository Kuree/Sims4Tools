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
    public class CRALResource : AResource
    {
        #region Attributes

        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }
        private uint version = 0x08;
        private S4CatalogCommon commonA;
        private Gp8references refList;
        private uint materialVariant;
        private ulong swatchGrouping;
        private uint unk02;
        private ColorList colors;
        
        #endregion

        #region Content Fields

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "StairRail"; }
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
        public Gp8references ReferenceList
        {
            get { return refList; }
            set { if (refList != value) { refList = new Gp8references(kRecommendedApiVersion, this.OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public uint MaterialVariant
        {
            get { return materialVariant; }
            set { if (materialVariant != value) { materialVariant = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(7)]
        public ulong SwatchGrouping//UnkIID01
        {
            get { return swatchGrouping; }
            set { if (swatchGrouping != value) { swatchGrouping = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(8)]
        public uint Unk02
        {
            get { return unk02; }
            set { if (unk02 != value) { unk02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(10)]
        public ColorList Colors
        {
            get { return colors; }
            set { if (colors != value) { colors = new ColorList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }

        public string Value { get { return ValueBuilder; } }


        #endregion

        #region Data I/O

        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.commonA = new S4CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged, s);
            this.refList = new Gp8references(kRecommendedApiVersion,this.OnResourceChanged,s);
            this.materialVariant = br.ReadUInt32();
            this.swatchGrouping = br.ReadUInt64();
            this.unk02 = br.ReadUInt32();
            this.colors = new ColorList(this.OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            if (this.commonA == null) { commonA = new S4CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged); }
            this.commonA.UnParse(s);
            if (this.refList == null) { this.refList = new Gp8references(kRecommendedApiVersion, this.OnResourceChanged); }
            this.refList.UnParse(s);
            bw.Write(this.materialVariant);
            bw.Write(this.swatchGrouping);
            bw.Write(this.unk02);
            if (this.colors == null) { this.colors = new ColorList(this.OnResourceChanged); }
            this.colors.UnParse(s);
            return s;
        }

        #endregion DataIO =====================================================================

        #region Constructors
        public CRALResource(int APIversion, Stream s)
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
        #endregion Constructors
    }

    /// <summary>
    /// ResourceHandler for CSTRResource wrapper
    /// </summary>
    public class CRALResourceHandler : AResourceHandler
    {
        public CRALResourceHandler()
        {
            this.Add(typeof(CRALResource), new List<string>(new[] { "0x1C1CF1F7" }));
        }
    }
}