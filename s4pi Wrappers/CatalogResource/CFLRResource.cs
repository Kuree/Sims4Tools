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
    public class CFLRResource : AResource
    {
        #region Attributes

        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }
        private uint version = 0x06;
        private S4CatalogCommon commonA;
        private uint hashIndicator = 0x1;
        private uint hash01 = 0x811C9DC5;
        private uint hash02 = 0x811C9DC5;
        private uint hash03 = 0x811C9DC5;
        private CountedTGIBlockList matdList;
        private ColorList colors;
        private uint unk02;
        private ulong unkIID01;

        #endregion Attributes ================================================================

        #region Content Fields

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "CTProductFloorPattern"; }
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
        [ElementPriority(20)]
        public uint HashIndicator
        {
            get { return hashIndicator; }
            set { if (hashIndicator != value) { hashIndicator = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(21)]
        public uint Hash01
        {
            get { return hash01; }
            set { if (hash01 != value) { hash01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(22)]
        public uint Hash02
        {
            get { return hash02; }
            set { if (hash02 != value) { hash02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(23)]
        public uint Hash03
        {
            get { return hash03; }
            set { if (hash03 != value) { hash03 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(25)]
        public CountedTGIBlockList MaterialList
        {
            get { return matdList; }
            set { if (matdList != value) { matdList = new CountedTGIBlockList(this.OnResourceChanged,TGIBlock.Order.ITG, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(26)]
        public ColorList Colors
        {
            get { return colors; }
            set { if (colors != value) { colors = new ColorList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(27)]
        public uint Unk02
        {
            get { return unk02; }
            set { if (unk02 != value) { unk02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }

        [ElementPriority(28)]
        public ulong SwatchGrouping//UnkIID01
        {
            get { return unkIID01; }
            set { if (unkIID01 != value) { unkIID01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        public string Value { get { return ValueBuilder; } }


        #endregion

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
            int count = Convert.ToUInt16(br.ReadUInt32());
            this.matdList = new CountedTGIBlockList(this.OnResourceChanged, TGIBlock.Order.ITG, count, s);
            this.colors = new ColorList(this.OnResourceChanged, s);
            this.unk02 = br.ReadUInt32();
            this.unkIID01 = br.ReadUInt64();
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            if (this.commonA == null) { commonA = new S4CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged); }
            this.commonA.UnParse(s);
            bw.Write(this.hashIndicator);
            bw.Write(this.hash01);
            bw.Write(this.hash02);
            bw.Write(this.hash03);
            if (this.matdList == null) { this.matdList = new CountedTGIBlockList(this.OnResourceChanged, TGIBlock.Order.ITG); }
            bw.Write(Convert.ToUInt32(matdList.Count));
            this.matdList.UnParse(s);
            if (this.colors == null) { this.colors = new ColorList(this.OnResourceChanged); }
            this.colors.UnParse(s);
            bw.Write(this.unk02);
            bw.Write(this.unkIID01);
            return s;
        }


        #endregion DataIO ===================================================

        #region Constructors
        public CFLRResource(int APIversion, Stream s)
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
    public class CFLRResourceHandler : AResourceHandler
    {
        public CFLRResourceHandler()
        {
            this.Add(typeof(CFLRResource), new List<string>(new string[] { "0xB4F762C9", }));
        }
    }
}