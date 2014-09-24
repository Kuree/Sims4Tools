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
    public class TerrainPaintBrushCatalogResource : TerrainGeometryWaterBrushCatalogResource
    {
        #region Attributes
        TGIBlock brushTexture = null;
        TerrainType terrain;//version>=0x04
        CategoryType category;//version>=0x04
        #endregion

        #region Constructors
        public TerrainPaintBrushCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public TerrainPaintBrushCatalogResource(int APIversion, Stream unused, TerrainPaintBrushCatalogResource basis)
            : base(APIversion, null, basis)
        {
            this.brushTexture = (TGIBlock)basis.brushTexture.Clone(OnResourceChanged);
            this.terrain = basis.terrain;
            this.category = basis.category;
        }
        public TerrainPaintBrushCatalogResource(int APIversion,
            uint version,
            uint brushVersion,
            Common common,
            BrushOperation normalOperation, BrushOperation oppositeOperation, TGIBlock profileTexture, BrushOrientation orientation,
            float width, float strength, byte baseTextureValue, float wiggleAmount,
            TGIBlock brushTexture
            )
            : this(APIversion,
                version,
                brushVersion,
                common,
                normalOperation,
                oppositeOperation,
                profileTexture,
                orientation,
                width,
                strength,
                baseTextureValue,
                wiggleAmount,
                brushTexture,
                0, 0
            )
        {
            if (checking) if (version >= 0x00000004)
                    throw new InvalidOperationException(String.Format("Constructor requires terrain and category for version {0}", version));
        }
        public TerrainPaintBrushCatalogResource(int APIversion,
            uint version,
            uint brushVersion,
            Common common,
            BrushOperation normalOperation, BrushOperation oppositeOperation, TGIBlock profileTexture, BrushOrientation orientation,
            float width, float strength, byte baseTextureValue, float wiggleAmount,
            TGIBlock brushTexture,
            TerrainType terrain, CategoryType category
            )
            : base(APIversion,
                version,
                brushVersion,
                common,
                normalOperation,
                oppositeOperation,
                profileTexture,
                orientation,
                width,
                strength,
                baseTextureValue,
                wiggleAmount
            )
        {
            this.brushTexture = (TGIBlock)brushTexture.Clone(OnResourceChanged);
            this.terrain = terrain;
            this.category = category;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            this.brushTexture = new TGIBlock(requestedApiVersion, OnResourceChanged, s);

            if (version >= 4)
            {
                BinaryReader r = new BinaryReader(s);
                this.terrain = (TerrainType)r.ReadUInt32();
                this.category = (CategoryType)r.ReadUInt32();
            }

            if (checking) if (this.GetType().Equals(typeof(TerrainPaintBrushCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            if (brushTexture == null) brushTexture = new TGIBlock(requestedApiVersion, OnResourceChanged);
            brushTexture.UnParse(s);

            if (version >= 4)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)terrain);
                w.Write((uint)category);
            }

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
                if (this.version < 0x00000004)
                {
                    res.Remove("Terrain");
                    res.Remove("Category");
                }
                return res;
            }
        }
        #endregion

        #region Sub-classes
        public enum TerrainType : uint
        {
            Grass = 0x1,
            Flowers = 0x2,
            Rock = 0x3,
            DirtSand = 0x4,
            Other = 0x5,
        }
        public enum CategoryType : uint
        {
            None = 0x00,
            Grass = 0x01,
            Flowers = 0x02,
            Rock = 0x03,
            Dirt_Sand = 0x04,
            Other = 0x05,
        }
        #endregion

        #region Content Fields
        //--insert TerrainGeometryWaterBrushCatalogResource
        [ElementPriority(41)]
        public TGIBlock BrushTexture
        {
            get { return brushTexture; }
            set { if (brushTexture != value) { brushTexture = new TGIBlock(requestedApiVersion, OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(42)]
        public TerrainType Terrain { get { if (version < 0x00000004) throw new InvalidOperationException(); return terrain; } set { if (version < 0x00000004) throw new InvalidOperationException(); if (terrain != value) { terrain = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(43)]
        public CategoryType Category { get { if (version < 0x00000004) throw new InvalidOperationException(); return category; } set { if (version < 0x00000004) throw new InvalidOperationException(); if (category != value) { category = value; OnResourceChanged(this, EventArgs.Empty); } } }
        #endregion
    }
}
