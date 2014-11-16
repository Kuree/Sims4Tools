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
    class RoofPatternResource : ObjectCatalogResource
    {
        #region Attributes
        uint unknown;
        TGIBlock matdTGIReference;
        TGIBlock cflrTGIReference;
        ulong catalogGroupID;
        SwatchColorList colorList;
        #endregion

        public RoofPatternResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.unknown = r.ReadUInt32();
            this.matdTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cflrTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.catalogGroupID = r.ReadUInt64();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown);
            if (this.matdTGIReference == null) this.matdTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.matdTGIReference.UnParse(s);
            if (this.cflrTGIReference == null) this.cflrTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cflrTGIReference.UnParse(s);
            w.Write(this.catalogGroupID);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public uint Unknown { get { return unknown; } set { if (!unknown.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown = value; } } }
        [ElementPriority(16)]
        public TGIBlock MatdTGIReference { get { return matdTGIReference; } set { if (!matdTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.matdTGIReference = value; } } }
        [ElementPriority(17)]
        public TGIBlock CflrTGIReference { get { return cflrTGIReference; } set { if (!cflrTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cflrTGIReference = value; } } }
        [ElementPriority(18)]
        public ulong CatalogGroupID { get { return catalogGroupID; } set { if (!catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(19)]
        public SwatchColorList ColorList { get { return colorList; } set { if (!colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        #endregion

        #region Clone
        public override TGIBlock[] NestedTGIBlockList
        {
            get
            {
                return new TGIBlock[] { this.matdTGIReference, this.cflrTGIReference };
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