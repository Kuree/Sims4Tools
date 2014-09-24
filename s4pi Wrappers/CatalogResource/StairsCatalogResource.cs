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
    public class StairsCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        MaterialList materialList = null;
        uint steps4xModelVPXYIndex;
        uint steps1xModelVPXYIndex;
        uint wallCapModelVPXYIndex;
        uint catalogRailing;
        uint catalogWall;
        uint catalogWallFloorPattern;
        uint catalogFence;
        #endregion

        #region Constructors
        public StairsCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public StairsCatalogResource(int APIversion, Stream unused, StairsCatalogResource basis)
            : base(APIversion, basis.version, basis.common, basis.list)
        {
            this.materialList = (basis.version >= 0x00000003) ? new MaterialList(OnResourceChanged, basis.materialList) : null;
            this.common = new Common(requestedApiVersion, OnResourceChanged, basis.common);
            this.steps4xModelVPXYIndex = basis.steps4xModelVPXYIndex;
            this.steps1xModelVPXYIndex = basis.steps1xModelVPXYIndex;
            this.wallCapModelVPXYIndex = basis.wallCapModelVPXYIndex;
            this.catalogRailing = basis.catalogRailing;
            this.catalogWall = basis.catalogWall;
            this.catalogWallFloorPattern = basis.catalogWallFloorPattern;
            this.catalogFence = basis.catalogFence;
        }
        public StairsCatalogResource(int APIversion, uint version,
            Common common,
            uint steps4xModelVPXYIndex, uint steps1xModelVPXYIndex, uint wallCapModelVPXYIndex,
            uint catalogRailing, uint catalogWall, uint catalogWallFloorPattern, uint catalogFence,
            TGIBlockList ltgib)
            : this(APIversion, version,
            null,
            common,
            steps4xModelVPXYIndex, steps1xModelVPXYIndex, wallCapModelVPXYIndex, catalogRailing, catalogWall, catalogWallFloorPattern, catalogFence,
            ltgib)
        {
            if (checking) if (version >= 0x00000003)
                    throw new InvalidOperationException(String.Format("Constructor requires MaterialList for version {0}", version));
        }
        public StairsCatalogResource(int APIversion, uint version,
            MaterialList materialList,
            Common common,
            uint steps4xModelVPXYIndex, uint steps1xModelVPXYIndex, uint wallCapModelVPXYIndex,
            uint catalogRailing, uint catalogWall, uint catalogWallFloorPattern, uint catalogFence,
            TGIBlockList ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.materialList = materialList != null ? new MaterialList(OnResourceChanged, materialList) : null;
            this.common = new Common(requestedApiVersion, OnResourceChanged, common);
            this.steps4xModelVPXYIndex = steps4xModelVPXYIndex;
            this.steps1xModelVPXYIndex = steps1xModelVPXYIndex;
            this.wallCapModelVPXYIndex = wallCapModelVPXYIndex;
            this.catalogRailing = catalogRailing;
            this.catalogWall = catalogWall;
            this.catalogWallFloorPattern = catalogWallFloorPattern;
            this.catalogFence = catalogFence;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);

            this.materialList = (this.version >= 0x00000003) ? new MaterialList(OnResourceChanged, s) : null;
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.steps4xModelVPXYIndex = r.ReadUInt32();
            this.steps1xModelVPXYIndex = r.ReadUInt32();
            this.wallCapModelVPXYIndex = r.ReadUInt32();
            this.catalogRailing = r.ReadUInt32();
            this.catalogWall = r.ReadUInt32();
            this.catalogWallFloorPattern = r.ReadUInt32();
            this.catalogFence = r.ReadUInt32();

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(RoofStyleCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (version >= 0x00000003)
            {
                if (materialList == null) materialList = new MaterialList(OnResourceChanged);
                materialList.UnParse(s);
            }

            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);

            w.Write(steps4xModelVPXYIndex);
            w.Write(steps1xModelVPXYIndex);
            w.Write(wallCapModelVPXYIndex);
            w.Write(catalogRailing);
            w.Write(catalogWall);
            w.Write(catalogWallFloorPattern);
            w.Write(catalogFence);

            base.UnParse(s);

            w.Flush();

            return s;
        }
        #endregion

        #region AApiVersionedFields
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                if (this.version < 0x00000003) res.Remove("Materials");
                return res;
            }
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        [ElementPriority(12)]
        public MaterialList Materials
        {
            get { if (version < 0x00000003) throw new InvalidOperationException(); return materialList; }
            set { if (version < 0x00000003) throw new InvalidOperationException(); if (materialList != value) { materialList = value == null ? null : new MaterialList(OnResourceChanged, value); } OnResourceChanged(this, EventArgs.Empty); }
        }
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21), TGIBlockListContentField("TGIBlocks")]
        public uint Steps4xModelVPXYIndex { get { return steps4xModelVPXYIndex; } set { if (steps4xModelVPXYIndex != value) { steps4xModelVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22), TGIBlockListContentField("TGIBlocks")]
        public uint Steps1xModelVPXYIndex { get { return steps1xModelVPXYIndex; } set { if (steps1xModelVPXYIndex != value) { steps1xModelVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23), TGIBlockListContentField("TGIBlocks")]
        public uint WallCapModelVPXYIndex { get { return wallCapModelVPXYIndex; } set { if (wallCapModelVPXYIndex != value) { wallCapModelVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(34), TGIBlockListContentField("TGIBlocks")]
        public uint CatalogRailingIndex { get { return catalogRailing; } set { if (catalogRailing != value) { catalogRailing = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(35), TGIBlockListContentField("TGIBlocks")]
        public uint CatalogWallIndex { get { return catalogWall; } set { if (catalogWall != value) { catalogWall = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(36), TGIBlockListContentField("TGIBlocks")]
        public uint CatalogWallFloorPatternIndex { get { return catalogWallFloorPattern; } set { if (catalogWallFloorPattern != value) { catalogWallFloorPattern = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(37), TGIBlockListContentField("TGIBlocks")]
        public uint CatalogFenceIndex { get { return catalogFence; } set { if (catalogFence != value) { catalogFence = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert TGIBlockList: no ElementPriority
        #endregion
    }
}
