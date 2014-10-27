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
    public class FountainPoolCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        //--version
        //--common
        ShapeType shape;
        uint cwstIndex;
        uint cwalFloorIndex;
        uint cwalWallIndex;
        uint cfndIndex;
        uint groundCutoutDDSIndex;
        uint xScale;
        uint yScale;
        //--tgilist
        #endregion

        #region Constructors
        public FountainPoolCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public FountainPoolCatalogResource(int APIversion, Stream unused, FountainPoolCatalogResource basis)
            : this(APIversion,
            basis.version,
            basis.common,
            basis.shape,
            basis.cwstIndex,
            basis.cwalFloorIndex,
            basis.cwalWallIndex,
            basis.cfndIndex,
            basis.xScale,
            basis.yScale,
            basis.list) { }
        public FountainPoolCatalogResource(int APIversion,
            uint version,
            Common common,
            ShapeType wallType,
            uint cwstIndex,
            uint cwalFloorIndex,
            uint cwalWallIndex,
            uint cfndIndex,
            uint xScale,
            uint yScale,
            TGIBlockList ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.shape = wallType;
            this.cwstIndex = cwstIndex;
            this.cwalFloorIndex = cwalFloorIndex;
            this.cwalWallIndex = cwalWallIndex;
            this.cfndIndex = cfndIndex;
            this.xScale = xScale;
            this.yScale = yScale;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);

            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.shape = (ShapeType)r.ReadUInt32();
            this.cwstIndex = r.ReadUInt32();
            this.cwalFloorIndex = r.ReadUInt32();
            this.cwalWallIndex = r.ReadUInt32();
            this.cfndIndex = r.ReadUInt32();
            this.groundCutoutDDSIndex = r.ReadUInt32();
            this.xScale = r.ReadUInt32();
            this.yScale = r.ReadUInt32();

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(WallCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);
            w.Write((uint)shape);
            w.Write(cwstIndex);
            w.Write(cwalFloorIndex);
            w.Write(cwalWallIndex);
            w.Write(cfndIndex);
            w.Write(groundCutoutDDSIndex);
            w.Write(xScale);
            w.Write(yScale);

            base.UnParse(s);

            w.Flush();

            return s;
        }
        #endregion

        #region Sub-classes
        public enum ShapeType : uint
        {
            Convex = 0x00000000,
            Concave = 0x00000001,
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21), TGIBlockListContentField("TGIBlocks")]
        public ShapeType Shape { get { return shape; } set { if (shape != value) { shape = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22), TGIBlockListContentField("TGIBlocks")]
        public uint CWSTIndex { get { return cwstIndex; } set { if (cwstIndex != value) { cwstIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23), TGIBlockListContentField("TGIBlocks")]
        public uint CWALFloorIndex { get { return cwalFloorIndex; } set { if (cwalFloorIndex != value) { cwalFloorIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(24), TGIBlockListContentField("TGIBlocks")]
        public uint CWALWallIndex { get { return cwalWallIndex; } set { if (cwalWallIndex != value) { cwalWallIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(25), TGIBlockListContentField("TGIBlocks")]
        public uint CFNDIndex { get { return cfndIndex; } set { if (cfndIndex != value) { cfndIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(26), TGIBlockListContentField("TGIBlocks")]
        public uint GroundCutoutDDSIndex { get { return groundCutoutDDSIndex; } set { if (groundCutoutDDSIndex != value) { groundCutoutDDSIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(27)]
        public uint XScale { get { return xScale; } set { if (xScale != value) { xScale = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(28)]
        public uint YScale { get { return yScale; } set { if (yScale != value) { yScale = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert TGIBlockList: no ElementPriority
        #endregion
    }
}
