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
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    class _48C28979CatalogResource : ObjectCatalogResource
    {
        #region Attributes
        uint unknown1;
        uint unknown2;
        uint unknown3;
        uint unknown4;
        uint unknown5;
        uint unknown6;
        uint unknown7;
        DataBlobHandler dataBlob1;
        ulong unknown8;
        uint unknown9;
        uint unknown10;

        DataBlobHandler dataBlob2;

        uint unknown11;
        #endregion

        public _48C28979CatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

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
            this.unknown6 = r.ReadUInt32();
            this.unknown7 = r.ReadUInt32();
            this.dataBlob1 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(29));
            this.unknown8 = r.ReadUInt64();
            this.unknown9 = r.ReadUInt32();
            this.unknown10 = r.ReadUInt32();

            if (base.Version >= 0x19)
                this.dataBlob2 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(16));

            this.unknown11 = r.ReadUInt32();
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
            w.Write(this.unknown6);
            w.Write(this.unknown7);
            if (this.dataBlob1 == null) this.dataBlob1 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, new byte[29]);
            this.dataBlob1.UnParse(s);
            w.Write(this.unknown8);
            w.Write(this.unknown9);
            w.Write(this.unknown10);

            if (base.Version >= 0x19)
            {
                if (this.dataBlob2 == null) this.dataBlob2 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, new byte[16]);
                this.dataBlob2.UnParse(s);
            }

            w.Write(this.unknown11);
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
        public uint Unknown6 { get { return unknown6; } set { if (!unknown6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown6 = value; } } }
        [ElementPriority(21)]
        public uint Unknown7 { get { return unknown7; } set { if (!unknown7.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown7 = value; } } }
        [ElementPriority(22)]
        public DataBlobHandler DataBlob1 { get { return this.dataBlob1; } set { if (!this.dataBlob1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.dataBlob1 = value; } } }
        [ElementPriority(23)]
        public ulong Unknown8 { get { return unknown8; } set { if (!unknown8.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown8 = value; } } }
        [ElementPriority(24)]
        public uint Unknown9 { get { return unknown9; } set { if (!unknown9.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown9 = value; } } }
        [ElementPriority(25)]
        public uint Unknown10 { get { return unknown10; } set { if (!unknown10.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown10 = value; } } }
        [ElementPriority(26)]
        public DataBlobHandler DataBlob2 { get { return this.dataBlob2; } set { if (!this.dataBlob2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.dataBlob2 = value; } } }
        [ElementPriority(27)]
        public uint Unknown11 { get { return unknown11; } set { if (!unknown11.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown11 = value; } } }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;

                if (base.Version <= 0x18)
                    res.Remove("DataBlob2");

                return res;
            }
        }
        #endregion

        #region Clone
        //Clone Code
        #endregion
    }
}
