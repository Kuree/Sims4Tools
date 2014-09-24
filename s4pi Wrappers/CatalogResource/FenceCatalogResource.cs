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
    public class FenceCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        MaterialList materialList = null;//Version>=0x07
        uint modelVPXYIndex;
        uint diagonalVPXYIndex;
        uint postVPXYIndex;
        uint tileSpacing;
        bool canWalkOver;
        bool shouldNotGetThickSnow;//Version>=0x0A
        bool snowPostShapeIsCircle;//Version>=0x0A
        float snowThicknessPostScaleFactor;//Version>=0x0A
        float snowThicknessRailScaleFactor;//Version>=0x0A
        float snowThicknessPostVerticalOffset;//Version>=0x0A
        float snowThicknessRailVerticalOffset;//Version>=0x0A
        bool hasWall;//Version>=0x0A
        bool risesAboveWall;//Version>=0x08
        uint wallIndex;
        #endregion

        #region Constructors
        public FenceCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public FenceCatalogResource(int APIversion, Stream unused, FenceCatalogResource basis)
            : this(APIversion, basis.version,
            basis.materialList,
            basis.common, basis.modelVPXYIndex, basis.diagonalVPXYIndex, basis.postVPXYIndex, basis.tileSpacing, basis.canWalkOver,
            basis.shouldNotGetThickSnow, basis.snowPostShapeIsCircle,
            basis.snowThicknessPostScaleFactor, basis.snowThicknessRailScaleFactor,
            basis.snowThicknessPostVerticalOffset, basis.snowThicknessRailVerticalOffset, basis.hasWall,
            basis.risesAboveWall, basis.wallIndex,
            basis.list) { }

        // Current version
        public FenceCatalogResource(int APIversion, uint version,
            MaterialList materialList,//Version>=0x07
            Common common, uint modelVPXYIndex, uint diagonalVPXYIndex, uint postVPXYIndex, uint tileSpacing, bool canWalkOver,
            bool shouldNotGetThickSnow, bool snowPostShapeIsCircle,//Version>=0x0A
            float snowThicknessPostScaleFactor, float snowThicknessRailScaleFactor,//Version>=0x0A
            float snowThicknessPostVerticalOffset, float snowThicknessRailVerticalOffset, bool hasWall,//Version>=0x0A
            bool risesAboveWall, uint wallIndex,//Version>=0x08
            TGIBlockList ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.materialList = materialList != null ? new MaterialList(OnResourceChanged, materialList) : null;
            this.common = new Common(requestedApiVersion, OnResourceChanged, common);
            this.modelVPXYIndex = modelVPXYIndex;
            this.diagonalVPXYIndex = diagonalVPXYIndex;
            this.postVPXYIndex = postVPXYIndex;
            this.tileSpacing = tileSpacing;
            this.canWalkOver = canWalkOver;
            this.shouldNotGetThickSnow = shouldNotGetThickSnow;
            this.snowPostShapeIsCircle = snowPostShapeIsCircle;
            this.snowThicknessPostScaleFactor = snowThicknessPostScaleFactor;
            this.snowThicknessRailScaleFactor = snowThicknessRailScaleFactor;
            this.snowThicknessPostVerticalOffset = snowThicknessPostVerticalOffset;
            this.snowThicknessRailVerticalOffset = snowThicknessRailVerticalOffset;
            this.hasWall = hasWall;
            this.risesAboveWall = risesAboveWall;
            this.wallIndex = wallIndex;
        }

        //Version < 0x07
        public FenceCatalogResource(int APIversion, uint version,
            Common common, uint modelVPXYIndex, uint diagonalVPXYIndex, uint postVPXYIndex, uint tileSpacing, bool canWalkOver,
            TGIBlockList ltgib)
            : this(APIversion, version,
            null,
            common, modelVPXYIndex, diagonalVPXYIndex, postVPXYIndex, tileSpacing, canWalkOver,
            false, false, 0f, 0f, 0f, 0f, false,
            false, 0,
            ltgib)
        {
            if (checking) if (version >= 0x00000007)
                    throw new InvalidOperationException(String.Format("Constructor requires materialList for version {0}", version));
        }

        //Version 0x07
        public FenceCatalogResource(int APIversion, uint version,
            MaterialList materialList,//Version>=0x07
            Common common, uint modelVPXYIndex, uint diagonalVPXYIndex, uint postVPXYIndex, uint tileSpacing, bool canWalkOver,
            TGIBlockList ltgib)
            : this(APIversion, version,
            materialList,
            common, modelVPXYIndex, diagonalVPXYIndex, postVPXYIndex, tileSpacing, canWalkOver,
            false, false, 0f, 0f, 0f, 0f, false,
            false, 0,
            ltgib)
        {
            if (checking) if (version >= 0x00000008)
                    throw new InvalidOperationException(String.Format("Constructor requires risesAboveWall and wallIndex for version {0}", version));
        }
        
        //Version 0x08
        public FenceCatalogResource(int APIversion, uint version,
            MaterialList materialList,//Version>=0x07
            Common common, uint modelVPXYIndex, uint diagonalVPXYIndex, uint postVPXYIndex, uint tileSpacing, bool canWalkOver,
            bool risesAboveWall, uint wallIndex,//Version>=0x08
            TGIBlockList ltgib)
            : this(APIversion, version,
            materialList,
            common, modelVPXYIndex, diagonalVPXYIndex, postVPXYIndex, tileSpacing, canWalkOver,
            false, false, 0f, 0f, 0f,0f, false,
            risesAboveWall, wallIndex,
            ltgib)
        {
            if (checking) if (version > 0x00000008)
                    throw new InvalidOperationException(String.Format("Constructor requires " +
                        "shouldNotGetThickSnow, snowPostShapeIsCircle, snowThicknessPostScaleFactor, snowThicknessRailScaleFactor, snowThicknessPostVerticalOffset, snowThicknessRailVerticalOffset and hasWall" +
                        " for version {0}", version));
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.materialList = (this.version >= 0x00000007) ? new MaterialList(OnResourceChanged, s) : null;
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.modelVPXYIndex = r.ReadUInt32();
            this.diagonalVPXYIndex = r.ReadUInt32();
            this.postVPXYIndex = r.ReadUInt32();
            this.tileSpacing = r.ReadUInt32();
            this.canWalkOver = r.ReadByte() > 0;
            if (this.version >= 0x00000008)
            {
                if (this.version >= 0x0000000a)
                {
                    this.shouldNotGetThickSnow = r.ReadByte() > 0;
                    this.snowPostShapeIsCircle = r.ReadByte() > 0;
                    this.snowThicknessPostScaleFactor = r.ReadSingle();
                    this.snowThicknessRailScaleFactor = r.ReadSingle();
                    this.snowThicknessPostVerticalOffset = r.ReadSingle();
                    this.snowThicknessRailVerticalOffset = r.ReadSingle();
                    this.hasWall = r.ReadByte() > 0;
                }
                if (this.version < 0x0000000a || this.hasWall)
                {
                    this.risesAboveWall = r.ReadByte() > 0;
                    this.wallIndex = r.ReadUInt32();
                }
            }

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(FenceCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            if (version >= 0x00000007)
            {
                if (materialList == null) materialList = new MaterialList(OnResourceChanged);
                materialList.UnParse(s);
            }
            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);
            w.Write(modelVPXYIndex);
            w.Write(diagonalVPXYIndex);
            w.Write(postVPXYIndex);
            w.Write(tileSpacing);
            w.Write((byte)(canWalkOver ? 1 : 0));
            if (this.version >= 0x00000008)
            {
                if (this.version >= 0x0000000a)
                {
                    w.Write((byte)(shouldNotGetThickSnow ? 1 : 0));
                    w.Write((byte)(snowPostShapeIsCircle ? 1 : 0));
                    w.Write(snowThicknessPostScaleFactor);
                    w.Write(snowThicknessRailScaleFactor);
                    w.Write(snowThicknessPostVerticalOffset);
                    w.Write(snowThicknessRailVerticalOffset);
                    w.Write((byte)(hasWall ? 1 : 0));
                }
                if (this.version < 0x0000000a || this.hasWall)
                {
                    w.Write((byte)(risesAboveWall ? 1 : 0));
                    w.Write(wallIndex);
                }
            }

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
            get {
                List<string> res = base.ContentFields;
                if (this.version < 0x0000000A)
                {
                    res.Remove("ShouldNotGetThickSnow");
                    res.Remove("SnowPostShapeIsCircle");
                    res.Remove("SnowThicknessPostScaleFactor");
                    res.Remove("SnowThicknessRailScaleFactor");
                    res.Remove("SnowThicknessPostVerticalOffset");
                    res.Remove("SnowThicknessRailVerticalOffset");
                    res.Remove("HasWall");
                    if (this.version < 0x00000008)
                    {
                        res.Remove("RisesAboveWall");
                        res.Remove("WallIndex");
                        if (this.version < 0x00000007)
                        {
                            res.Remove("Materials");
                        }
                    }
                }
                else
                {
                    if (!this.hasWall)
                    {
                        res.Remove("RisesAboveWall");
                        res.Remove("WallIndex");
                    }
                }
                return res;
            }
        }
        #endregion

        #region Sub-types
        public class PolygonPoint : AHandlerElement, IEquatable<PolygonPoint>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            float x = 0f;
            float z = 0f;
            #endregion

            #region Constructors
            public PolygonPoint(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public PolygonPoint(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public PolygonPoint(int APIversion, EventHandler handler, PolygonPoint basis)
                : this(APIversion, handler, basis.x, basis.z) { }
            public PolygonPoint(int APIversion, EventHandler handler, float X, float Z)
                : base(APIversion, handler)
            {
                this.x = X;
                this.z = Z;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.x = r.ReadSingle();
                this.z = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(x);
                w.Write(z);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            /// <summary>
            /// The list of available field names on this API object
            /// </summary>
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<PolygonPoint> Members

            public bool Equals(PolygonPoint other)
            {
                return this.x == other.x && this.z == other.z;
            }

            public override bool Equals(object obj)
            {
                return obj as PolygonPoint != null ? this.Equals(obj as PolygonPoint) : false;
            }

            public override int GetHashCode()
            {
                return x.GetHashCode() ^ z.GetHashCode();
            }

            #endregion

            #region Content Fields
            public float X { get { return x; } set { if (x != value) { x = value; OnElementChanged(); } } }
            public float Z { get { return z; } set { if (z != value) { z = value; OnElementChanged(); } } }

            public string Value { get { return String.Format("[X: {0}] [Z: {1}]", x, z); } }
            #endregion
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        [ElementPriority(12)]
        public MaterialList Materials
        {
            get { if (version < 0x00000007) throw new InvalidOperationException(); return materialList; }
            set { if (version < 0x00000007) throw new InvalidOperationException(); if (materialList != value) { materialList = value == null ? null : new MaterialList(OnResourceChanged, value); } OnResourceChanged(this, EventArgs.Empty); }
        }
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21), TGIBlockListContentField("TGIBlocks")]
        public uint ModelVPXYIndex { get { return modelVPXYIndex; } set { if (modelVPXYIndex != value) { modelVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22), TGIBlockListContentField("TGIBlocks")]
        public uint DiagonalVPXYIndex { get { return diagonalVPXYIndex; } set { if (diagonalVPXYIndex != value) { diagonalVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23), TGIBlockListContentField("TGIBlocks")]
        public uint PostVPXYIndex { get { return postVPXYIndex; } set { if (postVPXYIndex != value) { postVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(24)]
        public uint TileSpacing { get { return tileSpacing; } set { if (tileSpacing != value) { tileSpacing = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(25)]
        public bool CanWalkOver { get { return canWalkOver; } set { if (canWalkOver != value) { canWalkOver = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(26)]
        public bool ShouldNotGetThickSnow
        {
            get { if (version < 0x0000000a) throw new InvalidOperationException(); return shouldNotGetThickSnow; }
            set { if (version < 0x0000000a) throw new InvalidOperationException(); if (shouldNotGetThickSnow != value) { shouldNotGetThickSnow = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(27)]
        public bool SnowPostShapeIsCircle
        {
            get { if (version < 0x0000000a) throw new InvalidOperationException(); return snowPostShapeIsCircle; }
            set { if (version < 0x0000000a) throw new InvalidOperationException(); if (snowPostShapeIsCircle != value) { snowPostShapeIsCircle = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(28)]
        public float SnowThicknessPostScaleFactor
        {
            get { if (version < 0x0000000a) throw new InvalidOperationException(); return snowThicknessPostScaleFactor; }
            set { if (version < 0x0000000a) throw new InvalidOperationException(); if (snowThicknessPostScaleFactor != value) { snowThicknessPostScaleFactor = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(29)]
        public float SnowThicknessRailScaleFactor
        {
            get { if (version < 0x0000000a) throw new InvalidOperationException(); return snowThicknessRailScaleFactor; }
            set { if (version < 0x0000000a) throw new InvalidOperationException(); if (snowThicknessRailScaleFactor != value) { snowThicknessRailScaleFactor = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(30)]
        public float SnowThicknessPostVerticalOffset
        {
            get { if (version < 0x0000000a) throw new InvalidOperationException(); return snowThicknessPostVerticalOffset; }
            set { if (version < 0x0000000a) throw new InvalidOperationException(); if (snowThicknessPostVerticalOffset != value) { snowThicknessPostVerticalOffset = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(31)]
        public float SnowThicknessRailVerticalOffset
        {
            get { if (version < 0x0000000a) throw new InvalidOperationException(); return snowThicknessRailVerticalOffset; }
            set { if (version < 0x0000000a) throw new InvalidOperationException(); if (snowThicknessRailVerticalOffset != value) { snowThicknessRailVerticalOffset = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(32)]
        public bool HasWall
        {
            get { if (version < 0x0000000a) throw new InvalidOperationException(); return hasWall; }
            set { if (version < 0x0000000a) throw new InvalidOperationException(); if (hasWall != value) { hasWall = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(33)]
        public bool RisesAboveWall
        {
            get { if (version < 0x00000008 || (version >= 0x0000000a && !hasWall)) throw new InvalidOperationException(); return risesAboveWall; }
            set { if (version < 0x00000008 || (version >= 0x0000000a && !hasWall)) throw new InvalidOperationException(); if (risesAboveWall != value) { risesAboveWall = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(34), TGIBlockListContentField("TGIBlocks")]
        public uint WallIndex
        {
            get { if (version < 0x00000008 || (version >= 0x0000000a && !hasWall)) throw new InvalidOperationException(); return wallIndex; }
            set { if (version < 0x00000008 || (version >= 0x0000000a && !hasWall)) throw new InvalidOperationException(); if (wallIndex != value) { wallIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }

        //--insert TGIBlockList: no ElementPriority
        #endregion
    }
}
