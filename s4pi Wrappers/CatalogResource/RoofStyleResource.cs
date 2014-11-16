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
    class RoofStyleResource : ObjectCatalogResource
    {
        #region Attributes
        uint unknown1;
        TGIBlock crmtTGIReference;
        TGIBlock crtrTGIReference1;
        TGIBlock crtrTGIReference2;
        TGIBlock toolTGIReference;
        uint unknown2;
        #endregion

        public RoofStyleResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.unknown1 = r.ReadUInt32();
            this.crmtTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.crtrTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.crtrTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.toolTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknown2 = r.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown1);
            if (this.crmtTGIReference == null) this.crmtTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.crmtTGIReference.UnParse(s);
            if (this.crtrTGIReference1 == null) this.crtrTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.crtrTGIReference1.UnParse(s);
            if (this.crtrTGIReference2 == null) this.crtrTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.crtrTGIReference2.UnParse(s);
            if (this.toolTGIReference == null) this.toolTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.toolTGIReference.UnParse(s);
            w.Write(this.unknown2);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public uint Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(16)]
        public TGIBlock CrmtTGIReference { get { return crmtTGIReference; } set { if (!crmtTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.crmtTGIReference = value; } } }
        [ElementPriority(17)]
        public TGIBlock CrtrTGIReference1 { get { return crtrTGIReference1; } set { if (!crtrTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.crtrTGIReference1 = value; } } }
        [ElementPriority(18)]
        public TGIBlock CrtrTGIReference2 { get { return crtrTGIReference2; } set { if (!crtrTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.crtrTGIReference2 = value; } } }
        [ElementPriority(19)]
        public TGIBlock ToolTGIReference { get { return toolTGIReference; } set { if (!toolTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.toolTGIReference = value; } } }
        [ElementPriority(20)]
        public uint Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        #endregion

        #region Clone
        public override TGIBlock[] NestedTGIBlockList
        {
            get
            {
                return new TGIBlock[] { this.crmtTGIReference, this.crtrTGIReference1, this.crtrTGIReference2, this.toolTGIReference };
            }
            set
            {
                base.SetTGIList(value);
            }
        }
        #endregion
    }
}