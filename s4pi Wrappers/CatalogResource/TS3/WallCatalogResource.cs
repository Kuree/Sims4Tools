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
    public class WallCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        //--version
        uint unknown2;
        //--common
        Wall wallType;
        Partition partitionType;
        PartitionFlagsType partitionFlags;
        VerticalSpan verticalSpanType;
        PartitionsBlockedFlagsType partitionsBlockedFlags;
        PartitionsBlockedFlagsType adjacentPartitionsBlockedFlags;
        PartitionTool partitionToolMode;
        ToolUsageFlagsType toolUsageFlags;
        uint defaultPatternIndex;
        WallThickness wallThicknessType;
        //--tgilist
        #endregion

        #region Constructors
        public WallCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public WallCatalogResource(int APIversion, Stream unused, WallCatalogResource basis)
            : this(APIversion,
            basis.version,
            basis.unknown2,
            basis.common,
            basis.wallType,
            basis.partitionType,
            basis.partitionFlags,
            basis.verticalSpanType,
            basis.partitionsBlockedFlags,
            basis.adjacentPartitionsBlockedFlags,
            basis.partitionToolMode,
            basis.toolUsageFlags,
            basis.defaultPatternIndex,
            basis.wallThicknessType,
            basis.list) { }
        public WallCatalogResource(int APIversion,
            uint version,
            uint unknown2,
            Common common,
            Wall wallType,
            Partition partitionType,
            PartitionFlagsType partitionFlags,
            VerticalSpan verticalSpanType,
            PartitionsBlockedFlagsType partitionsBlockedFlags,
            PartitionsBlockedFlagsType adjacentPartitionsBlockedFlags,
            PartitionTool partitionToolMode,
            ToolUsageFlagsType toolUsageFlags,
            uint defaultPatternIndex,
            WallThickness wallThicknessType,
            TGIBlockList ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.unknown2 = unknown2;
            this.wallType = wallType;
            this.partitionType = partitionType;
            this.partitionFlags = partitionFlags;
            this.verticalSpanType = verticalSpanType;
            this.partitionsBlockedFlags = partitionsBlockedFlags;
            this.adjacentPartitionsBlockedFlags = adjacentPartitionsBlockedFlags;
            this.partitionToolMode = partitionToolMode;
            this.toolUsageFlags = toolUsageFlags;
            this.defaultPatternIndex = defaultPatternIndex;
            this.wallThicknessType = wallThicknessType;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);

            this.unknown2 = r.ReadUInt32();
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.wallType = (Wall)r.ReadUInt32();
            this.partitionType = (Partition)r.ReadUInt32();
            this.partitionFlags = (PartitionFlagsType)r.ReadUInt32();
            this.verticalSpanType = (VerticalSpan)r.ReadUInt32();
            this.partitionsBlockedFlags = (PartitionsBlockedFlagsType)r.ReadUInt32();
            this.adjacentPartitionsBlockedFlags = (PartitionsBlockedFlagsType)r.ReadUInt32();
            this.partitionToolMode = (PartitionTool)r.ReadUInt32();
            this.toolUsageFlags = (ToolUsageFlagsType)r.ReadUInt32();
            this.defaultPatternIndex = r.ReadUInt32();
            this.wallThicknessType = (WallThickness)r.ReadUInt32();

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(WallCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(unknown2);
            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);
            w.Write((uint)wallType);
            w.Write((uint)partitionType);
            w.Write((uint)partitionFlags);
            w.Write((uint)verticalSpanType);
            w.Write((uint)partitionsBlockedFlags);
            w.Write((uint)adjacentPartitionsBlockedFlags);
            w.Write((uint)partitionToolMode);
            w.Write((uint)toolUsageFlags);
            w.Write(defaultPatternIndex);
            w.Write((uint)wallThicknessType);

            base.UnParse(s);

            w.Flush();

            return s;
        }
        #endregion

        #region Sub-classes
        public enum Wall : uint
        {
            None = 0x00000000,
            Normal = 0x00000001,
            Attic = 0x00000002,
            Fence = 0x00000003,
            DeckSkirt = 0x00000004,
            DeckRailing = 0x00000005,
            Foundation = 0x00000006,
            Pool = 0x00000007,
            Frieze = 0x00000008,
            Platform = 0x00000009,
            Foyer = 0x0000000A,
            HalfNormal = 0x0000000B,
            UnderConstruction = 0x000000FF,
        }

        public enum Partition : uint
        {
            Wall = 0x00000000,
            Fence = 0x00000001,
            FenceArch = 0x00000002,
            HalfWall = 0x00000003,
            PlatformWall = 0x00000004,
            Unspecified = 0xFFFFFFFF,
        }

        [Flags]
        public enum PartitionFlagsType : uint
        {
            MayChangeSurface = 0x00000001,
            MayAttachObjects = 0x00000002,
            MayCutAway = 0x00000004,
            RequiresFlatBottom = 0x00000008,

            RequiresFlatTop = 0x00000010,
            Submersible = 0x00000020,
            MayPlaceDiagonally = 0x00000040,
            BlocksLocomotion = 0x00000080,

            BlocksPlacement = 0x00000100,
            BearsLoad = 0x00000200,
            BlocksLight = 0x00000400,
            ShouldRender = 0x00000800,

            RequiresSupportUnderneath = 0x00001000,
            LitIndoors = 0x00002000,
            LitOutdoors = 0x00004000,
            AOMapped = 0x00008000,

            MayCutAwayBelowGround = 0x00010000,
        }

        public enum VerticalSpan : uint
        {
            FixedHeight = 0x00000000,
            FloorToFloor = 0x00000001,
            FloorToRoof = 0x00000002,
            FloorToStairs = 0x00000003,
            FloorToFloorBasedCeiling = 0x00000004,
            DoubleFixedHeight = 0x00000005,
            PlatformHeight = 0x00000006,
            HalfFixedHeight = 0x00000007,
            Unspecified = 0xFFFFFFFF,
        }

        [Flags]
        public enum PartitionsBlockedFlagsType : uint
        {
            BlocksWalls = 0x00000001,
            BlocksFences = 0x00000002,
            BlocksArches = 0x00000004,
            BlocksPlatformWalls = 0x00000010,
        }

        public enum PartitionTool : uint
        {
            Line = 0x00000001,
            Rectangle = 0x00000002,
            All = 0x00000003,
            Unspecified = 0xFFFFFFFF,
        }

        [Flags]
        public enum ToolUsageFlagsType : uint
        {
            Partition = 0x00000001,
            Roof = 0x00000002,
            LevelRoom = 0x00000004,
            ModularStairs = 0x00000008,
            WallDrag = 0x00000010,
        }

        public enum WallThickness : uint
        {
            Standard = 0x00000000,
            None = 0x00000001,
            Thin = 0x00000002,
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        [ElementPriority(12)]
        public uint Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21)]
        public Wall WallType { get { return wallType; } set { if (wallType != value) { wallType = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22)]
        public Partition PartitionType { get { return partitionType; } set { if (partitionType != value) { partitionType = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23)]
        public PartitionFlagsType PartitionFlags { get { return partitionFlags; } set { if (partitionFlags != value) { partitionFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(24)]
        public VerticalSpan VerticalSpanType { get { return verticalSpanType; } set { if (verticalSpanType != value) { verticalSpanType = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(25)]
        public PartitionsBlockedFlagsType PartitionsBlockedFlags { get { return partitionsBlockedFlags; } set { if (partitionsBlockedFlags != value) { partitionsBlockedFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(26)]
        public PartitionsBlockedFlagsType AdjacentPartitionsBlockedFlags { get { return adjacentPartitionsBlockedFlags; } set { if (adjacentPartitionsBlockedFlags != value) { adjacentPartitionsBlockedFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(27)]
        public PartitionTool PartitionToolMode { get { return partitionToolMode; } set { if (partitionToolMode != value) { partitionToolMode = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(28)]
        public ToolUsageFlagsType ToolUsageFlags { get { return toolUsageFlags; } set { if (toolUsageFlags != value) { toolUsageFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(29), TGIBlockListContentField("TGIBlocks")]
        public uint DefaultPatternIndex { get { return defaultPatternIndex; } set { if (defaultPatternIndex != value) { defaultPatternIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(30)]
        public WallThickness WallThicknessType { get { return wallThicknessType; } set { if (wallThicknessType != value) { wallThicknessType = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert TGIBlockList: no ElementPriority
        #endregion
    }
}
