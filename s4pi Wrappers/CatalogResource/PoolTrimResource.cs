/***************************************************************************
 *  Copyright (C) 2014 by Snaitf                                           *
 *  http://modthesims.info/member/Snaitf                                   *
 *  Keyi Zhang kz005@bucknell.edu                                          *
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
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    class PoolTrimResource : ObjectCatalogResource
    {
        #region Attributes
        TGIBlock trimTGIReference;
        uint unknown1;
        TGIBlock modlTGIReference;
        ulong catalogGroupID;
        SwatchColorList colorList;
        uint unknown2;
        #endregion

        public PoolTrimResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.trimTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknown1 = r.ReadUInt32();
            this.modlTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);            
            this.catalogGroupID = r.ReadUInt64();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.unknown2 = r.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            if (this.trimTGIReference == null) this.trimTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.trimTGIReference.UnParse(s);
            w.Write(this.unknown1);
            if (this.modlTGIReference == null) this.modlTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference.UnParse(s);            
            w.Write(this.catalogGroupID);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            w.Write(this.unknown2);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public TGIBlock TrimTGIReference { get { return trimTGIReference; } set { if (!trimTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.trimTGIReference = value; } } }
        [ElementPriority(16)]
        public uint Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(17)]
        public TGIBlock ModlTGIReference1 { get { return modlTGIReference; } set { if (!modlTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference = value; } } }
        [ElementPriority(18)]
        public ulong CatalogGroupID { get { return catalogGroupID; } set { if (!catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(19)]
        public SwatchColorList ColorList { get { return colorList; } set { if (!colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(20)]
        public uint Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }        
        #endregion

        #region Clone
        public override TGIBlock[] NestedTGIBlockList
        {
            get
            {
                return new TGIBlock[] { this.trimTGIReference, this.modlTGIReference };
            }
            set
            {
                base.SetTGIList(value);
            }
        }

        protected override object GroupingID { get { return this.CatalogGroupID; } set { this.CatalogGroupID = (ulong)value; } }
        #endregion
    }
}
