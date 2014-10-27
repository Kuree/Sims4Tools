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
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s4pi.Interfaces;
using System.IO;

namespace CatalogResource.TS4
{
    public class FloorCatalogResource : ObjectCatalogResource
    {
        #region Attributes
        private uint unknown1;
        private uint unknown2;
        private uint unknown3;
        private uint unknown4;
        private MATDList matdList;
        private SwatchColorList colorList;
        private uint unknown5;
        private ulong catalogGroupID;
        #endregion

        public FloorCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.unknown4 = r.ReadUInt32();
            this.matdList = new MATDList(OnResourceChanged, s, false);
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.unknown5 = r.ReadUInt32();
            this.catalogGroupID = r.ReadUInt64();
        }

        protected override Stream UnParse()
        {
            var s =  base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            w.Write(this.unknown4);
            if (this.matdList == null) this.matdList = new MATDList(OnResourceChanged);
            matdList.UnParse(s);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            w.Write(this.unknown5);
            w.Write(this.catalogGroupID);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public uint Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(16)]
        public uint Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(17)]
        public uint Unknown3 { get { return this.unknown3; } set { if (!this.unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(18)]
        public uint Unknown4 { get { return this.unknown4; } set { if (!this.unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown4 = value; } } }
        [ElementPriority(19)]
        public MATDList MatdList { get { return this.matdList; } set { if (!this.matdList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.matdList = value; } } }
        [ElementPriority(20)]
        public SwatchColorList ColorList { get { return this.colorList; } set { if (!this.colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(21)]
        public ulong CatalogGroupID { get { return this.catalogGroupID; } set { if (!this.catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(22)]
        public uint Unknown5 { get { return this.unknown5; } set { if (!this.unknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown5 = value; } } }
        #endregion

        #region Clone Code
        public override TGIBlock[] NestedTGIBlockList
        {
            get
            {
                List<TGIBlock> result = new List<TGIBlock>();
                foreach (var matd in this.matdList)
                    result.Add(matd.MATDTGI);
                return result.ToArray();
            }
            set
            {
                var list = value;
                if (list.Length != NestedTGIBlockList.Length) throw new InvalidDataException("Invalid clone operation");
                for (int i = 0; i < list.Length; i++)
                {
                    var tgi = NestedTGIBlockList[i];
                    tgi.Instance = list[i].Instance;
                    tgi.ResourceType = list[i].ResourceType;
                    tgi.ResourceGroup = tgi.ResourceGroup;
                }
            }
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
