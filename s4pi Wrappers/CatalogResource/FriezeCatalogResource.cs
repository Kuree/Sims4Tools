/***************************************************************************
 *  Copyright (C) 2014 by Snaitf                                           *
 *  http://modthesims.info/member/Snaitf                                   *
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
    class FriezeCatalogResource : ObjectCatalogResource
    {
        #region Attributes
        TGIBlock trimTGIReference;
        TGIBlock modlTGIReference1;
        uint unknown1;
        ulong catalogGroupID;
        uint unknown2;
        uint unknown3;
        TGIBlock modlTGIReference2;
        SwatchColorList colorList;
        #endregion

        public FriezeCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.trimTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknown1 = r.ReadUInt32();
            this.catalogGroupID = r.ReadUInt64();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.modlTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.colorList = new SwatchColorList(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            if (this.trimTGIReference == null) this.trimTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.trimTGIReference.UnParse(s);
            if (this.modlTGIReference1 == null) this.modlTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference1.UnParse(s);
            w.Write(this.unknown1);
            w.Write(this.catalogGroupID);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            if (this.modlTGIReference2 == null) this.modlTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference2.UnParse(s);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public TGIBlock TrimTGIReference { get { return trimTGIReference; } set { if (!trimTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.trimTGIReference = value; } } }
        [ElementPriority(16)]
        public TGIBlock ModlTGIReference1 { get { return modlTGIReference1; } set { if (!modlTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference1 = value; } } }
        [ElementPriority(17)]
        public uint Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(18)]
        public ulong CatalogGroupID { get { return catalogGroupID; } set { if (!catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(19)]
        public uint Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(20)]
        public uint Unknown3 { get { return unknown3; } set { if (!unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(21)]
        public TGIBlock ModlTGIReference2 { get { return modlTGIReference2; } set { if (!modlTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference2 = value; } } }
        [ElementPriority(22)]
        public SwatchColorList ColorList { get { return colorList; } set { if (!colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        #endregion
    }
}