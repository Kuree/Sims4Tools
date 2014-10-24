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
using System.IO;
using System.Linq;
using System.Text;
using s4pi.Interfaces;

namespace CatalogResource.TS4
{
    public class WallCatalogResource : ObjectCatalogResource
    {
        private MATDList matdList;
        private ThumbnailList thumList;
        private uint unknown1;
        private SwatchColorList colorList;
        private ulong catalogGroupID;

        public WallCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.matdList = new MATDList(OnResourceChanged, s);
            this.thumList = new ThumbnailList(OnResourceChanged, s);
            this.unknown1 = r.ReadUInt32();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.catalogGroupID = r.ReadUInt64();
        }

        protected override Stream UnParse()
        {
            var s =  base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            if (this.matdList == null) this.matdList = new MATDList(OnResourceChanged);
            matdList.UnParse(s);
            if (this.thumList == null) this.thumList = new ThumbnailList(OnResourceChanged);
            this.thumList.UnParse(s);
            w.Write(this.unknown1);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            w.Write(this.catalogGroupID);
            return s;
        }
        [ElementPriority(15)]
        public MATDList MatdList { get { return this.matdList; } set { if (!this.matdList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.matdList = value; } } }
        [ElementPriority(16)]
        public ThumbnailList ThumList { get { return this.thumList; } set { if (!this.thumList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.thumList = value; } } }
        [ElementPriority(17)]
        public uint Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(18)]
        public SwatchColorList ColorList { get { return this.colorList; } set { if (!this.colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(19)]
        public ulong CatalogGroupID { get { return this.catalogGroupID; } set { if (!this.catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }

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
