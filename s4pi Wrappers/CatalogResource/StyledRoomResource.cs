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
    class StyledRoomResource : ObjectCatalogResource
    {
        #region Attributes
        uint unknown1;
        uint unknown2;
        uint unknown3;
        uint unknown4;
        uint unknown5;
        ulong catalogGroupID;
        SwatchColorList colorList;
        TGIBlock unknownTGIReference;
        uint unknown6;
        uint unknown7;
        #endregion

        public StyledRoomResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.unknown4 = r.ReadUInt32();
            this.unknown5 = r.ReadUInt32();
            this.catalogGroupID = r.ReadUInt64();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.unknownTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknown6 = r.ReadUInt32();
            this.unknown7 = r.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            w.Write(this.unknown4);
            w.Write(this.unknown5);
            w.Write(this.catalogGroupID);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            if (this.unknownTGIReference == null) this.unknownTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.unknownTGIReference.UnParse(s);
            w.Write(this.unknown6);
            w.Write(this.unknown7);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public uint Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(16)]
        public uint Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(17)]
        public uint Unknown3 { get { return unknown3; } set { if (!unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(18)]
        public uint Unknown4 { get { return unknown4; } set { if (!unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown4 = value; } } }
        [ElementPriority(19)]
        public uint Unknown5 { get { return unknown5; } set { if (!unknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown5 = value; } } }
        [ElementPriority(20)]
        public ulong CatalogGroupID { get { return catalogGroupID; } set { if (!catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(21)]
        public SwatchColorList ColorList { get { return colorList; } set { if (!colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(22)]
        public TGIBlock UnknownTGIReference { get { return unknownTGIReference; } set { if (!unknownTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownTGIReference = value; } } }
        [ElementPriority(23)]
        public uint Unknown6 { get { return unknown6; } set { if (!unknown6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown6 = value; } } }
        [ElementPriority(24)]
        public uint Unknown7 { get { return unknown7; } set { if (!unknown7.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown7 = value; } } }
        #endregion

        #region Clone
        public override TGIBlock[] NestedTGIBlockList
        {
            get
            {
                return new TGIBlock[] { this.unknownTGIReference };
            }
            set
            {
                base.SetTGIList(value);
            }
        }
        #endregion
    }
}