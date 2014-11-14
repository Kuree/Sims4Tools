/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
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
using System.Linq;
using System.Text;
using s4pi.Interfaces;

namespace CatalogResource.TS4
{
    public class RoofTrimResource: ObjectCatalogResource
    {
        #region Attributes
        ushort unknown1;
        TGIBlock unknownTGIReference1;
        uint unknown2;
        TGIBlock unknownTGIReference2;
        ulong catalogGroupID;
        SwatchColorList colorList;
        #endregion

        public RoofTrimResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.unknown1 = r.ReadUInt16();
            this.unknownTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknown2 = r.ReadUInt32();
            this.unknownTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.catalogGroupID = r.ReadUInt64();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown1);
            if (this.unknownTGIReference1 == null) this.unknownTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.unknownTGIReference1.UnParse(s);
            w.Write(this.unknown2);
            if (this.unknownTGIReference2 == null) this.unknownTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.unknownTGIReference2.UnParse(s);
            w.Write(this.catalogGroupID);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public ushort Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(16)]
        public TGIBlock UnknownTGIReference1 { get { return unknownTGIReference1; } set { if (!unknownTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownTGIReference1 = value; } } }
        [ElementPriority(17)]
        public uint Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(18)]
        public TGIBlock UnknownTGIReference2 { get { return unknownTGIReference2; } set { if (!unknownTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownTGIReference2 = value; } } }
        [ElementPriority(19)]
        public ulong CatalogGroupID { get { return catalogGroupID; } set { if (!catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(20)]
        public SwatchColorList ColorList { get { return colorList; } set { if (!colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }

        #endregion

        #region Clone
        public override TGIBlock[] NestedTGIBlockList
        {
            get
            {
                List<TGIBlock> tgiList = new List<TGIBlock>();
                tgiList.Add(this.unknownTGIReference1);
                tgiList.Add(this.unknownTGIReference2);
                return tgiList.ToArray();
            }
            set { base.SetTGIList(value); }
        }

        internal override List<string> RenumberingFields
        {
            get
            {
                var res = base.RenumberingFields;
                res.Add("CatalogGroupID");
                return res;
            }
        }

        protected override object GroupingID { get { return this.CatalogGroupID; } set { this.CatalogGroupID = (ulong)value; } }
        #endregion

    }
}
