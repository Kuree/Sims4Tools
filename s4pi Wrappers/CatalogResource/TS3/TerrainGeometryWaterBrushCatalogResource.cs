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
    public class TerrainGeometryWaterBrushCatalogResource : CatalogResource
    {
        #region Attributes
        //--Version
        uint brushVersion;
        //--CommonBlock
        BrushOperation normalOperation;
        BrushOperation oppositeOperation;
        TGIBlock profileTexture;
        BrushOrientation orientation;
        float width;
        float strength;
        byte baseTextureValue;
        float wiggleAmount;
        #endregion

        #region Constructors
        public TerrainGeometryWaterBrushCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public TerrainGeometryWaterBrushCatalogResource(int APIversion, Stream unused, TerrainGeometryWaterBrushCatalogResource basis)
            : this(APIversion,
                basis.version,
                basis.brushVersion,
                basis.common,
                basis.normalOperation,
                basis.oppositeOperation,
                basis.profileTexture,
                basis.orientation,
                basis.width,
                basis.strength,
                basis.baseTextureValue,
                basis.wiggleAmount
            ) { }
        public TerrainGeometryWaterBrushCatalogResource(int APIversion,
            uint version,
            uint brushVersion,
            Common common,
            BrushOperation normalOperation,
            BrushOperation oppositeOperation,
            TGIBlock profileTexture,
            BrushOrientation orientation,
            float width,
            float strength,
            byte baseTextureValue,
            float wiggleAmount
            )
            : base(APIversion, version, common)
        {
            this.brushVersion = brushVersion;
            this.normalOperation = normalOperation;
            this.oppositeOperation = oppositeOperation;
            this.profileTexture = profileTexture;
            this.orientation = orientation;
            this.width = width;
            this.strength = strength;
            this.baseTextureValue = baseTextureValue;
            this.wiggleAmount = wiggleAmount;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.brushVersion = r.ReadUInt32();
            common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.normalOperation = (BrushOperation)r.ReadUInt32();
            this.oppositeOperation = (BrushOperation)r.ReadUInt32();
            this.profileTexture = new TGIBlock(requestedApiVersion, OnResourceChanged, s);
            this.orientation = (BrushOrientation)r.ReadUInt32();
            this.width = r.ReadSingle();
            this.strength = r.ReadSingle();
            this.baseTextureValue = r.ReadByte();
            this.wiggleAmount = r.ReadSingle();
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(brushVersion);
            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);
            w.Write((uint)normalOperation);
            w.Write((uint)oppositeOperation);
            if (profileTexture == null) profileTexture = new TGIBlock(requestedApiVersion, OnResourceChanged);
            profileTexture.UnParse(s);
            w.Write((uint)orientation);
            w.Write(width);
            w.Write(strength);
            w.Write(baseTextureValue);
            w.Write(wiggleAmount);

            w.Flush();

            return s;
        }
        #endregion

        #region Sub-classes
        public enum BrushOperation : uint
        {
            None = 0x00000000,
            Raise = 0x00000001,
            Lower = 0x00000002,
            Smoothen = 0x00000003,
            Level = 0x00000004,
            AddPaint = 0x00000005,
            ErasePaint = 0x00000006,
            AddWater = 0x00000007,
        }

        public enum BrushOrientation : uint
        {
            Fixed = 0x00000000,
            AlignWithMouseMoveDirection = 0x00000001,
            RandomWiggle = 0x00000002,
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        [ElementPriority(12)]
        public uint BrushVersion { get { return brushVersion; } set { if (brushVersion != value) { brushVersion = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21)]
        public BrushOperation NormalOperation { get { return normalOperation; } set { if (normalOperation != value) { normalOperation = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22)]
        public BrushOperation OppositeOperation { get { return oppositeOperation; } set { if (oppositeOperation != value) { oppositeOperation = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23)]
        public TGIBlock ProfileTexture { get { return profileTexture; } set { if (profileTexture != value) { profileTexture = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(24)]
        public BrushOrientation Orientation { get { return orientation; } set { if (orientation != value) { orientation = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(25)]
        public float Width { get { return width; } set { if (width != value) { width = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(26)]
        public float Strength { get { return strength; } set { if (strength != value) { strength = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(27)]
        public byte BaseTextureValue { get { return baseTextureValue; } set { if (baseTextureValue != value) { baseTextureValue = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(28)]
        public float WiggleAmount { get { return wiggleAmount; } set { if (wiggleAmount != value) { wiggleAmount = value; OnResourceChanged(this, EventArgs.Empty); } } }
        #endregion
    }
}
