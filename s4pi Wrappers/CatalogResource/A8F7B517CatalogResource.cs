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
    class A8F7B517CatalogResource : ObjectCatalogResource
    {
        #region Attributes
        TGIBlock cobjTGIReference1;
        TGIBlock cobjTGIReference2;
        TGIBlock cobjTGIReference3;
        #endregion

        public A8F7B517CatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.cobjTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cobjTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cobjTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            if (this.cobjTGIReference1 == null) this.cobjTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cobjTGIReference1.UnParse(s);
            if (this.cobjTGIReference2 == null) this.cobjTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cobjTGIReference2.UnParse(s);
            if (this.cobjTGIReference3 == null) this.cobjTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cobjTGIReference3.UnParse(s);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public TGIBlock CobjTGIReference1 { get { return cobjTGIReference1; } set { if (!cobjTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cobjTGIReference1 = value; } } }
        [ElementPriority(16)]
        public TGIBlock CobjTGIReference2 { get { return cobjTGIReference2; } set { if (!cobjTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cobjTGIReference2 = value; } } }
        [ElementPriority(17)]
        public TGIBlock CobjTGIReference3 { get { return cobjTGIReference3; } set { if (!cobjTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cobjTGIReference3 = value; } } }
        #endregion

        #region Clone
        public override TGIBlock[] NestedTGIBlockList
        {
            get
            {
                return new TGIBlock[] { this.CobjTGIReference1, this.CobjTGIReference2, this.CobjTGIReference3 };
            }
            set
            {
                base.SetTGIList(value);
            }
        }

        //Not sure if I should remove this, or return null
        //protected override object GroupingID { get { return this.CatalogGroupID; } set { this.CatalogGroupID = (ulong)value; } }
        #endregion

    }
}
