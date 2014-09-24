/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
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
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    public class RoofPatternCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        uint topMaterialVPXYIndex;//topMaterialVPXYIndex
        uint undersideMaterialVPXYIndex;//undersideMaterialVPXYIndex
        uint sideStripsMaterialVPXYIndex;//sideStripsMaterialVPXYIndex
        #endregion

        #region Constructors
        public RoofPatternCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public RoofPatternCatalogResource(int APIversion, Stream unused, RoofPatternCatalogResource basis)
            : base(APIversion, basis.version, basis.common, basis.list)
        {
            this.common = new Common(requestedApiVersion, OnResourceChanged, basis.common);
            this.topMaterialVPXYIndex = basis.topMaterialVPXYIndex;
            this.undersideMaterialVPXYIndex = basis.undersideMaterialVPXYIndex;
            this.sideStripsMaterialVPXYIndex = basis.sideStripsMaterialVPXYIndex;
        }
        public RoofPatternCatalogResource(int APIversion, uint version, Common common,
            uint topMaterialVPXYIndex, uint undersideMaterialVPXYIndex, uint sideStripsMaterialVPXYIndex,
            TGIBlockList ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.common = new Common(requestedApiVersion, OnResourceChanged, common);
            this.topMaterialVPXYIndex = topMaterialVPXYIndex;
            this.undersideMaterialVPXYIndex = undersideMaterialVPXYIndex;
            this.sideStripsMaterialVPXYIndex = sideStripsMaterialVPXYIndex;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.topMaterialVPXYIndex = r.ReadUInt32();
            this.undersideMaterialVPXYIndex = r.ReadUInt32();
            this.sideStripsMaterialVPXYIndex = r.ReadUInt32();

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(RoofPatternCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);

            w.Write(topMaterialVPXYIndex);
            w.Write(undersideMaterialVPXYIndex);
            w.Write(sideStripsMaterialVPXYIndex);

            base.UnParse(s);

            w.Flush();

            return s;
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21), TGIBlockListContentField("TGIBlocks")]
        public uint TopMaterialVPXYIndex { get { return topMaterialVPXYIndex; } set { if (topMaterialVPXYIndex != value) { topMaterialVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22), TGIBlockListContentField("TGIBlocks")]
        public uint UndersideMaterialVPXYIndex { get { return undersideMaterialVPXYIndex; } set { if (undersideMaterialVPXYIndex != value) { undersideMaterialVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23), TGIBlockListContentField("TGIBlocks")]
        public uint SideStripsMaterialVPXYIndex { get { return sideStripsMaterialVPXYIndex; } set { if (sideStripsMaterialVPXYIndex != value) { sideStripsMaterialVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert TGIBlockList: no ElementPriority
        #endregion
    }
}
