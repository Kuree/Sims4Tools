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
using System.Text;

namespace CatalogResource
{
    public class WallFloorPatternCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        //--version
        WallFloorPatternMaterialList materialList = null;
        //--common
        Pattern patternType;
        uint materialVPXYIndex;
        SortFlagsType sortFlags;
        string surfaceType;
        Terrain terrainType;
        uint argb;
        //--tgilist
        #endregion

        #region Constructors
        public WallFloorPatternCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public WallFloorPatternCatalogResource(int APIversion, Stream unused, WallFloorPatternCatalogResource basis)
            : this(APIversion,
            basis.version,
            basis.materialList,
            basis.common,
            basis.patternType,
            basis.materialVPXYIndex,
            basis.sortFlags,
            basis.surfaceType,
            basis.terrainType,
            basis.argb,
            basis.list) { }
        public WallFloorPatternCatalogResource(int APIversion,
            uint version,
            IEnumerable<WallFloorPatternMaterial> materialList,
            Common common,
            Pattern patternType,
            uint materialVPXYIndex,
            SortFlagsType sortFlags,
            string surfaceType,
            Terrain terrainType,
            uint argb,
            TGIBlockList ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.materialList = new WallFloorPatternMaterialList(OnResourceChanged, materialList);
            this.patternType = patternType;
            this.materialVPXYIndex = materialVPXYIndex;
            this.sortFlags = sortFlags;
            this.surfaceType = surfaceType;
            this.terrainType = terrainType;
            this.argb = argb;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);

            this.materialList = new WallFloorPatternMaterialList(OnResourceChanged, s);
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.patternType = (Pattern)r.ReadUInt32();
            this.materialVPXYIndex = r.ReadUInt32();
            this.sortFlags = (SortFlagsType)r.ReadUInt32();
            this.surfaceType = BigEndianUnicodeString.Read(s);
            this.terrainType = (Terrain)r.ReadUInt32();
            this.argb = r.ReadUInt32();

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(WallFloorPatternCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (materialList == null) materialList = new WallFloorPatternMaterialList(OnResourceChanged);
            materialList.UnParse(s);
            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);
            w.Write((uint)patternType);
            w.Write(materialVPXYIndex);
            w.Write((uint)sortFlags);
            BigEndianUnicodeString.Write(s, surfaceType);
            w.Write((uint)terrainType);
            w.Write(argb);

            base.UnParse(s);

            w.Flush();

            return s;
        }
        #endregion

        #region Sub-classes
        public class WallFloorPatternMaterial : Material,
            IComparable<WallFloorPatternMaterial>, IEqualityComparer<WallFloorPatternMaterial>, IEquatable<WallFloorPatternMaterial>
        {
            #region Attributes
            uint unknown4;
            uint unknown5;
            uint unknown6;
            #endregion

            #region Constructors
            internal WallFloorPatternMaterial(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            internal WallFloorPatternMaterial(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public WallFloorPatternMaterial(int APIversion, EventHandler handler, WallFloorPatternMaterial basis)
                : base(APIversion, handler, basis) { this.unknown4 = basis.unknown4; this.unknown5 = basis.unknown5; this.unknown6 = basis.unknown6; }
            public WallFloorPatternMaterial(int APIversion, EventHandler handler, byte materialType, uint unknown1, ushort unknown2,
                MaterialBlock mb, IEnumerable<TGIBlock> ltgib, uint unknown3, uint unknown4, uint unknown5, uint unknown6)
                : base(APIversion, handler, materialType, unknown1, unknown2, mb, ltgib, unknown3) { this.unknown4 = unknown4; this.unknown5 = unknown5; this.unknown6 = unknown6; }
            #endregion

            #region Data I/O
            protected override void Parse(Stream s)
            {
                base.Parse(s);
                BinaryReader r = new BinaryReader(s);
                unknown4 = r.ReadUInt32();
                unknown5 = r.ReadUInt32();
                unknown6 = r.ReadUInt32();
            }

            public override void UnParse(Stream s)
            {
                base.UnParse(s);
                BinaryWriter w = new BinaryWriter(s);
                w.Write(unknown4);
                w.Write(unknown5);
                w.Write(unknown6);
            }
            #endregion

            #region IComparable<WallFloorPatternMaterial> Members

            public int CompareTo(WallFloorPatternMaterial other) { int res = base.CompareTo(other); if (res != 0) return res; return unknown4.CompareTo(other.unknown4); }

            #endregion

            #region IEqualityComparer<WallFloorPatternMaterial> Members

            public bool Equals(WallFloorPatternMaterial x, WallFloorPatternMaterial y) { return x.Equals(y); }

            public int GetHashCode(WallFloorPatternMaterial obj) { return obj.GetHashCode(); }

            public override int GetHashCode() { return base.GetHashCode() ^ unknown4.GetHashCode(); }

            #endregion

            #region IEquatable<WallFloorPatternMaterial> Members

            public bool Equals(WallFloorPatternMaterial other) { return this.CompareTo(other) == 0; }

            public override bool Equals(object obj)
            {
                return obj as WallFloorPatternMaterial != null ? this.Equals(obj as WallFloorPatternMaterial) : false;
            }

            #endregion

            #region AApiVersionedFields
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region ICloneable Members

            public override object Clone() { return new WallFloorPatternMaterial(requestedApiVersion, handler, this); }

            #endregion

            #region Content Fields
            public uint Unknown4 { get { return unknown4; } set { if (unknown4 != value) { unknown4 = value; OnElementChanged(); } } }
            public uint Unknown5 { get { return unknown5; } set { if (unknown5 != value) { unknown5 = value; OnElementChanged(); } } }
            public uint Unknown6 { get { return unknown6; } set { if (unknown6 != value) { unknown6 = value; OnElementChanged(); } } }
            #endregion
        }

        public class WallFloorPatternMaterialList : DependentList<WallFloorPatternMaterial>
        {
            #region Constructors
            internal WallFloorPatternMaterialList(EventHandler handler) : base(handler) { }
            internal WallFloorPatternMaterialList(EventHandler handler, Stream s) : base(handler, s) { }
            public WallFloorPatternMaterialList(EventHandler handler, IEnumerable<WallFloorPatternMaterial> lme) : base(handler, lme) { }
            #endregion

            #region Data I/O
            protected override WallFloorPatternMaterial CreateElement(Stream s) { return new WallFloorPatternMaterial(0, elementHandler, s); }
            protected override void WriteElement(Stream s, WallFloorPatternMaterial element) { element.UnParse(s); }
            #endregion
        }

        public enum Pattern : uint
        {
            Floor = 0x00000001,
            Wall = 0x00000002,
            Ceiling = 0x00000003,
            FloorEdge = 0x00000004,
        }

        [Flags]
        public enum SortFlagsType : uint
        {
            Miscellaneous_Miscellaneous = 0x00000002,
            Masonry_Carpet = 0x00000004,
            Paint_Linoleum = 0x00000008,
            Paneling_Masonry = 0x00000010,
            RockStone_Metal = 0x00000020,
            Siding_Rock_Stone = 0x00000040,
            Tile_Tile = 0x00000080,
            Wallpaper_Wood = 0x00000100,
            WallSets_CeilingTile = 0x00000200,
        }

        public enum Terrain : uint
        {
            Default = 0x00000000,
            Asphalt01 = 0x00000001,
            Cement01 = 0x00000002,
            Cobblestone01 = 0x00000003,

            Concrete01 = 0x00000004,
            Concrete02 = 0x00000005,
            Carpet01 = 0x00000006,
            Dirt01 = 0x00000007,

            Dirt02 = 0x00000008,
            Flagstone01 = 0x00000009,
            Footpath = 0x0000000A,
            Glass01 = 0x0000000B,

            Grass01 = 0x0000000C,
            Grass02 = 0x0000000D,
            Gravel01 = 0x0000000E,
            Gravel02 = 0x0000000F,


            Ice01 = 0x00000010,
            Linoleum01 = 0x00000011,
            Mud01 = 0x00000012,
            Mulch01 = 0x00000013,

            Rock01 = 0x00000014,
            Rock02 = 0x00000015,
            Sand01 = 0x00000016,
            Sand02 = 0x00000017,

            Wood01 = 0x00000018,
            Wood02 = 0x00000019,
            Wood03 = 0x0000001A,
            Marble01 = 0x0000001B,

            Masonry01 = 0x0000001C,
            Metal01 = 0x0000001D,
            Plastic01 = 0x0000001E,
            Road01 = 0x0000001F,


            Road01_Sidewalk = 0x00000020,
            Snow01 = 0x00000021,
            Tile01 = 0x00000022,
            Water_Deep = 0x00000023,

            Water_Knees = 0x00000024,
            Water_PondOrPool = 0x00000025,
            Water_Puddle = 0x00000026,
            Water_Waist = 0x00000027,

            LotCenter = 0x00000028,
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        [ElementPriority(12)]
        public WallFloorPatternMaterialList Materials { get { return materialList; } set { if (materialList != value) { materialList = value == null ? null : new WallFloorPatternMaterialList(OnResourceChanged, value); } OnResourceChanged(this, EventArgs.Empty); } }
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21)]
        public Pattern PatternType { get { return patternType; } set { if (patternType != value) { patternType = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22), TGIBlockListContentField("TGIBlocks")]
        public uint MaterialVPXYIndex { get { return materialVPXYIndex; } set { if (materialVPXYIndex != value) { materialVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23)]
        public SortFlagsType SortFlags { get { return sortFlags; } set { if (sortFlags != value) { sortFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(24)]
        public string Unknown10 { get { return surfaceType; } set { if (surfaceType != value) { surfaceType = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(25)]
        public Terrain TerrainType { get { return terrainType; } set { if (terrainType != value) { terrainType = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(26)]
        public uint ARGB { get { return argb; } set { if (argb != value) { argb = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert TGIBlockList: no ElementPriority
        #endregion
    }
}
