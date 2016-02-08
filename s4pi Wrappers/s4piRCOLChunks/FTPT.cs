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

namespace s4pi.GenericRCOLResource
{
    public class FTPT : ARCOLBlock
    {
        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint tag = (uint)FOURCC("FTPT");
        uint version = 12;
        ulong instance;
        uint type, group;
        PolygonHeightOverrideList minHeightOverrides;
        PolygonHeightOverrideList maxHeightOverrides;
        AreaList footprintAreas;
        AreaList slotAreas;
        float maxHeight;
        float minHeight;
        #endregion

        #region Constructors
        public FTPT(int apiVersion, EventHandler handler) : base(apiVersion, handler, null) { }
        public FTPT(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler, s) { }
        public FTPT(int apiVersion, EventHandler handler, FTPT basis)
            : base(apiVersion, handler, null)
        {
            this.version = basis.version;
            this.instance = basis.instance;
            this.type = basis.type;
            this.group = basis.group;
            this.minHeightOverrides = basis.minHeightOverrides == null ? null : new PolygonHeightOverrideList(OnRCOLChanged, basis.minHeightOverrides);
            this.maxHeightOverrides = basis.maxHeightOverrides == null ? null : new PolygonHeightOverrideList(OnRCOLChanged, basis.maxHeightOverrides);
            this.footprintAreas = basis.footprintAreas == null ? null : new AreaList(OnRCOLChanged, basis.footprintAreas, version);
            this.slotAreas = basis.slotAreas == null ? null : new AreaList(OnRCOLChanged, basis.slotAreas, version);
            this.maxHeight = basis.maxHeight;
            this.minHeight = basis.minHeight;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return "FTPT"; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0xD382BF57; } }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            tag = r.ReadUInt32();
            if (checking) if (tag != (uint)FOURCC("FTPT"))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: 'FTPT'; at 0x{1:X8}", FOURCC(tag), s.Position));
            version = r.ReadUInt32();
            instance = r.ReadUInt64();
            type = r.ReadUInt32();
            group = r.ReadUInt32();
            if (type != 0)
            {
                minHeightOverrides = new PolygonHeightOverrideList(OnRCOLChanged, s);
                maxHeightOverrides = new PolygonHeightOverrideList(OnRCOLChanged, s);
                footprintAreas = new AreaList(OnRCOLChanged, version);
                slotAreas = new AreaList(OnRCOLChanged, version);
            }
            else
            {
                minHeightOverrides = new PolygonHeightOverrideList(OnRCOLChanged);
                maxHeightOverrides = new PolygonHeightOverrideList(OnRCOLChanged);
                footprintAreas = new AreaList(OnRCOLChanged, s, version);
                slotAreas = new AreaList(OnRCOLChanged, s, version);
                maxHeight = r.ReadSingle();
                minHeight = r.ReadSingle();
            }
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(tag);
            w.Write(version);
            w.Write(instance);
            w.Write(type);
            w.Write(group);
            if (type != 0)
            {
                if (minHeightOverrides == null) minHeightOverrides = new PolygonHeightOverrideList(OnRCOLChanged);
                minHeightOverrides.UnParse(ms);
                if (maxHeightOverrides == null) maxHeightOverrides = new PolygonHeightOverrideList(OnRCOLChanged);
                maxHeightOverrides.UnParse(ms);
            }
            else
            {
                if (footprintAreas == null) footprintAreas = new AreaList(OnRCOLChanged, version);
                footprintAreas.UnParse(ms);
                if (slotAreas == null) slotAreas = new AreaList(OnRCOLChanged, version);
                slotAreas.UnParse(ms);
                w.Write(maxHeight);
                w.Write(minHeight);
            }

            return ms;
        }
        #endregion

        #region Sub-types
        public class PolygonHeightOverride : AHandlerElement, IEquatable<PolygonHeightOverride>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            uint nameHash = 0u;
            float height = 0f;
            #endregion

            #region Constructors
            public PolygonHeightOverride(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public PolygonHeightOverride(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
            public PolygonHeightOverride(int apiVersion, EventHandler handler, PolygonHeightOverride basis)
                : this(apiVersion, handler, basis.nameHash, basis.height) { }
            public PolygonHeightOverride(int apiVersion, EventHandler handler, uint NameHash, float Height)
                : base(apiVersion, handler)
            {
                this.nameHash = NameHash;
                this.height = Height;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.nameHash = r.ReadUInt32();
                this.height = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(nameHash);
                w.Write(height);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            /// <summary>
            /// The list of available field names on this API object
            /// </summary>
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<PolygonHeightOverride> Members

            public bool Equals(PolygonHeightOverride other)
            {
                return this.nameHash == other.nameHash && this.height == other.height;
            }

            public override bool Equals(object obj)
            {
                return obj as PolygonHeightOverride != null ? this.Equals(obj as PolygonHeightOverride) : false;
            }

            public override int GetHashCode()
            {
                return nameHash.GetHashCode() ^ height.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(4)]
            public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public float Height { get { return height; } set { if (height != value) { height = value; OnElementChanged(); } } }

            public virtual string Value { get { return ValueBuilder; } }
           // public string Value { get { return String.Format("[NameHash: 0x{0}] [Height: {1}]", nameHash.ToString("X8"), height); } }
            #endregion
        }
        public class PolygonHeightOverrideList : DependentList<PolygonHeightOverride>
        {
            #region Constructors
            public PolygonHeightOverrideList(EventHandler handler) : base(handler, Byte.MaxValue) { }
            public PolygonHeightOverrideList(EventHandler handler, Stream s) : base(handler, s, Byte.MaxValue) { }
            public PolygonHeightOverrideList(EventHandler handler, IEnumerable<PolygonHeightOverride> lpho) : base(handler, lpho, Byte.MaxValue) { }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return (new BinaryReader(s)).ReadByte(); }
            protected override void WriteCount(Stream s, int count) { (new BinaryWriter(s)).Write((byte)count); }

            protected override PolygonHeightOverride CreateElement(Stream s) { return new PolygonHeightOverride(0, elementHandler, s); }

            protected override void WriteElement(Stream s, PolygonHeightOverride element) { element.UnParse(s); }
            #endregion
        }

        public class PolygonPoint : AHandlerElement, IEquatable<PolygonPoint>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            float x = 0f;
            float z = 0f;
            #endregion

            #region Constructors
            public PolygonPoint(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public PolygonPoint(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
            public PolygonPoint(int apiVersion, EventHandler handler, PolygonPoint basis)
                : this(apiVersion, handler, basis.x, basis.z) { }
            public PolygonPoint(int apiVersion, EventHandler handler, float X, float Z)
                : base(apiVersion, handler)
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
        public class PolygonPointList : DependentList<PolygonPoint>
        {
            #region Constructors
            public PolygonPointList(EventHandler handler) : base(handler, Byte.MaxValue) { }
            public PolygonPointList(EventHandler handler, Stream s) : base(handler, s, Byte.MaxValue) { }
            public PolygonPointList(EventHandler handler, IEnumerable<PolygonPoint> lpp) : base(handler, lpp, Byte.MaxValue) { }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return (new BinaryReader(s)).ReadByte(); }
            protected override void WriteCount(Stream s, int count) { (new BinaryWriter(s)).Write((byte)count); }

            protected override PolygonPoint CreateElement(Stream s) { return new PolygonPoint(0, elementHandler, s); }

            protected override void WriteElement(Stream s, PolygonPoint element) { element.UnParse(s); }
            #endregion
        }

        [Flags]
        public enum FootprintPolyFlags : uint
        {
            FOOTPRINTPOLYFLAG_PLACEMENT = 0x01,
            FOOTPRINTPOLYFLAG_PATHING = 0x02,
            FOOTPRINTPOLYFLAG_ENABLED = 0x04,
            FOOTPRINTPOLYFLAG_DISCOURAGED = 0x08,
            FOOTPRINTPOLYFLAG_LANDINGSTRIP = 0x10,
            FOOTPRINTPOLYFLAG_NO_RAYCAST = 0x20,
            FOOTPRINTPOLYFLAG_PLACEMENTSLOTTED = 0x40,
            FOOTPRINTPOLYFLAG_ENCOURAGED = 0x80,
            FOOTPRINTPOLYFLAG_TERRAIN_CUTOUT = 0x100,
        }
        [Flags]
        public enum IntersectionFlags : uint
        {
            INTERSECTIONFLAG_NONE = 0,
            INTERSECTIONFLAG_WALLS = (1 << 1),
            INTERSECTIONFLAG_OBJECTS = (1 << 2),
            INTERSECTIONFLAG_SIMS = (1 << 3),
            INTERSECTIONFLAG_ROOFS = (1 << 4),
            INTERSECTIONFLAG_FENCES = (1 << 5),
            INTERSECTIONFLAG_MODULARSTAIRS = (1 << 6),
            INTERSECTIONFLAG_OBJECTSOFSAMETYPE = (1 << 7),
            INTERSECTIONFLAG_COLUMNS = (1 << 8),
            INTERSECTIONFLAG_RESERVEDSPACE = (1 << 9),
            INTERSECTIONFLAG_FOUNDATIONS = (1 << 10),
            INTERSECTIONFLAG_FENESTRATION_NODE = (1 << 11),
            INTERSECTIONFLAG_TRIM = (1 << 12),
        }
        [Flags]
        public enum SurfaceTypeFlags : uint
        {
            SURFACETYPEFLAG_TERRAIN = (1 << 0),
            SURFACETYPEFLAG_FLOOR = (1 << 1),
            SURFACETYPEFLAG_POOL = (1 << 2),
            SURFACETYPEFLAG_POND = (1 << 3),
            SURFACETYPEFLAG_FENCEPOST = (1 << 4),
            SURFACETYPEFLAG_ANYSURFACE = (1 << 5),
            SURFACETYPEFLAG_AIR = (1 << 6),
            SURFACETYPEFLAG_ROOF = (1 << 7)
        }
        [Flags]
        public enum SurfaceAttribute : uint
        {
            Unknown00 = 0x00,
            Inside = 0x01,
            Outside = 0x02,
            Slope = 0x04,
            Unknown08 = 0x08,
        }
        public class Area : AHandlerElement, IEquatable<Area>
        {
            const int recommendedApiVersion = 1;

            public uint ParentVersion { get; set; }

            #region Attributes
            uint name;
            byte priority;
            FootprintPolyFlags areaTypeFlags;
            PolygonPointList pointList;
            IntersectionFlags intersectionObjectType;
            IntersectionFlags allowIntersectionTypes;
            SurfaceTypeFlags surfaceTypeFlags;
            SurfaceAttribute surfaceAttributeFlags;
            byte deprecatedLevelOffset;
            BoundingBox boundingBox3D;
            #endregion

            #region Constructors
            public Area(int apiVersion, EventHandler handler, uint version) : base(apiVersion, handler)
            {
                this.ParentVersion = version;
                Stream ms = new MemoryStream();
                UnParse(ms);
                ms.Position = 0;
                Parse(ms);
            }
            public Area(int apiVersion, EventHandler handler, Stream s, uint version) : base(apiVersion, handler) { this.ParentVersion = version; Parse(s); }
            public Area(int apiVersion, EventHandler handler, Area basis)
                : this(apiVersion, handler, basis.ParentVersion,
                basis.name, basis.priority, basis.areaTypeFlags, basis.pointList,
                basis.intersectionObjectType, basis.allowIntersectionTypes, basis.surfaceTypeFlags, basis.surfaceAttributeFlags,
                basis.deprecatedLevelOffset,
                basis.boundingBox3D) { }
            public Area(int apiVersion, EventHandler handler, uint version,
                uint name, byte priority, FootprintPolyFlags areaTypeFlags, IEnumerable<PolygonPoint> closedPolygon,
                IntersectionFlags intersectionObjectType, IntersectionFlags allowIntersectionTypes, 
                SurfaceTypeFlags surfaceTypeFlags, SurfaceAttribute surfaceAttributeFlags,
                byte deprecatedLevelOffset, BoundingBox boundingBox3D)
                : base(apiVersion, handler)
            {
                this.ParentVersion = version;
                this.name = name;
                this.priority = priority;
                this.areaTypeFlags = areaTypeFlags;
                this.pointList = closedPolygon == null ? null : new PolygonPointList(handler, closedPolygon);
                this.intersectionObjectType = intersectionObjectType;
                this.allowIntersectionTypes = allowIntersectionTypes;
                this.surfaceTypeFlags = surfaceTypeFlags;
                this.surfaceAttributeFlags = surfaceAttributeFlags;
                this.deprecatedLevelOffset = deprecatedLevelOffset;
                this.boundingBox3D = new BoundingBox(apiVersion, handler, boundingBox3D);
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.name = r.ReadUInt32();
                this.priority = r.ReadByte();
                this.areaTypeFlags = (FootprintPolyFlags)r.ReadUInt32();
                this.pointList = new PolygonPointList(handler, s);
                this.intersectionObjectType = (IntersectionFlags)r.ReadUInt32();
                this.allowIntersectionTypes = (IntersectionFlags)r.ReadUInt32();
                this.surfaceTypeFlags = (SurfaceTypeFlags)r.ReadUInt32();
                this.surfaceAttributeFlags = (SurfaceAttribute)r.ReadUInt32();
                this.deprecatedLevelOffset = r.ReadByte();
                float min3d_x = r.ReadSingle();
                float min3d_z = r.ReadSingle();
                float max3d_x = r.ReadSingle();
                float max3d_z = r.ReadSingle();
                float min3d_y = r.ReadSingle();
                float max3d_y = r.ReadSingle();
                boundingBox3D = new BoundingBox(1, handler, new Vertex(1, handler, min3d_x, min3d_y, min3d_z),
                    new Vertex(1, handler, max3d_x, max3d_y, max3d_z));
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(name);
                w.Write(priority);
                w.Write((uint)areaTypeFlags);
                if (pointList == null) pointList = new PolygonPointList(handler);
                pointList.UnParse(s);
                w.Write((uint)intersectionObjectType);
                w.Write((uint)allowIntersectionTypes);
                w.Write((uint)surfaceTypeFlags);
                w.Write((uint)surfaceAttributeFlags);
                w.Write(deprecatedLevelOffset);
                w.Write(boundingBox3D.Min.X);
                w.Write(boundingBox3D.Min.Z);
                w.Write(boundingBox3D.Max.X);
                w.Write(boundingBox3D.Max.Z);
                w.Write(boundingBox3D.Min.Y);
                w.Write(boundingBox3D.Max.Y);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            /// <summary>
            /// The list of available field names on this API object
            /// </summary>
            public override List<string> ContentFields
            {
                get
                {
                    List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                    res.Remove("ParentVersion");
                    if (ParentVersion < 0x00000007)
                    {
                        res.Remove("ElevationOffset");
                    }
                    return res;
                }
            }
            #endregion

            #region IEquatable<Area> Members

            public bool Equals(Area other)
            {
                return name == other.name &&
                    priority == other.priority &&
                    areaTypeFlags == other.areaTypeFlags &&
                    pointList.Equals(other.pointList) &&
                    intersectionObjectType == other.intersectionObjectType &&
                    allowIntersectionTypes == other.allowIntersectionTypes &&
                    surfaceTypeFlags == other.surfaceTypeFlags &&
                    surfaceAttributeFlags == other.surfaceAttributeFlags &&
                    deprecatedLevelOffset == other.deprecatedLevelOffset &&
                    boundingBox3D == other.boundingBox3D;
            }

            public override bool Equals(object obj)
            {
                return obj as Area != null ? this.Equals(obj as Area) : false;
            }

            public override int GetHashCode()
            {
                return name.GetHashCode() ^
                    priority.GetHashCode() ^
                    areaTypeFlags.GetHashCode() ^
                    pointList.GetHashCode() ^
                    intersectionObjectType.GetHashCode() ^
                    allowIntersectionTypes.GetHashCode() ^
                    surfaceTypeFlags.GetHashCode() ^
                    surfaceAttributeFlags.GetHashCode() ^
                    deprecatedLevelOffset.GetHashCode() ^
                    boundingBox3D.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint Name { get { return name; } set { if (name != value) { name = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public byte Priority { get { return priority; } set { if (priority != value) { priority = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public FootprintPolyFlags AreaTypeFlags { get { return areaTypeFlags; } set { if (areaTypeFlags != value) { areaTypeFlags = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public PolygonPointList PolygonPoints { get { return pointList; } set { if (pointList != value) { pointList = value == null ? null : new PolygonPointList(handler, value); OnElementChanged(); } } }
            [ElementPriority(5)]
            public IntersectionFlags IntersectionObjectType { get { return intersectionObjectType; } set { if (intersectionObjectType != value) { intersectionObjectType = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public IntersectionFlags AllowIntersectionTypes { get { return allowIntersectionTypes; } set { if (allowIntersectionTypes != value) { allowIntersectionTypes = value; OnElementChanged(); } } }
            [ElementPriority(7)]
            public SurfaceTypeFlags SurfaceTypeFlags { get { return surfaceTypeFlags; } set { if (surfaceTypeFlags != value) { surfaceTypeFlags = value; OnElementChanged(); } } }
            [ElementPriority(8)]
            public SurfaceAttribute SurfaceAttributeFlags { get { return surfaceAttributeFlags; } set { if (surfaceAttributeFlags != value) { surfaceAttributeFlags = value; OnElementChanged(); } } }
            [ElementPriority(9)]
            public byte DeprecatedLevelOffset { get { return deprecatedLevelOffset; } set { if (deprecatedLevelOffset != value) { deprecatedLevelOffset = value; OnElementChanged(); } } }
            [ElementPriority(10)]
            public BoundingBox BoundingBox3D { get { return boundingBox3D; } set { if (boundingBox3D != value) { boundingBox3D = new BoundingBox(1, handler, value); OnElementChanged(); } } }

            public string Value
            {
                get
                {
                    return ValueBuilder;
                    /*
                    string s = "";
                    s += "Name: 0x" + name.ToString("X8");
                    s += "\nPriority: 0x" + priority.ToString("X2");
                    s += "\nAreaTypeFlags: " + this["AreaTypeFlags"];
                    s += "\nClosedPolygon: "; foreach (PolygonPoint pp in closedPolygon) s += pp.Value + "; "; s = s.TrimEnd(';', ' ');
                    s += "\nAllowIntersectionFlags: " + this["AllowIntersectionFlags"];
                    s += "\nSurfaceTypeFlags: " + this["SurfaceTypeFlags"];
                    s += "\nSurfaceAttributeFlags: " + this["SurfaceAttributeFlags"];
                    s += "\nLevelOffset: 0x" + levelOffset.ToString("X8");
                    if (version >= 0x00000007) s += "\nElevationOffset: " + elevationOffset.ToString();
                    s += "\nLowerX: " + lowerX;
                    s += "\nLowerY: " + lowerY;
                    s += "\nUpperX: " + upperX;
                    s += "\nUpperY: " + upperY;
                    return s;
                    /**/
                }
            }
            #endregion
        }
        public class AreaList : DependentList<Area>
        {
            uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set { if (_ParentVersion != value) { _ParentVersion = value; foreach (var i in this) i.ParentVersion = _ParentVersion; } }
            }

            #region Constructors
            public AreaList(EventHandler handler, uint version) : base(handler, 255) { this._ParentVersion = version; }
            public AreaList(EventHandler handler, Stream s, uint version) : base(null, 255) { this._ParentVersion = version; elementHandler = handler; Parse(s); this.handler = handler; }
            public AreaList(EventHandler handler, IEnumerable<Area> lfpa, uint version) : base(null, 255) { this._ParentVersion = version; elementHandler = handler; this.AddRange(lfpa); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return (new BinaryReader(s)).ReadByte(); }
            protected override void WriteCount(Stream s, int count) { (new BinaryWriter(s)).Write((byte)count); }

            protected override Area CreateElement(Stream s) { return new Area(0, elementHandler, s, _ParentVersion); }

            protected override void WriteElement(Stream s, Area element) { element.UnParse(s); }
            #endregion

            public override void Add() { this.Add(new Area(0, handler, _ParentVersion)); }
            public override void Add(Area item) { item.ParentVersion = _ParentVersion; base.Add(item); }
        }
        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; footprintAreas.ParentVersion = version; slotAreas.ParentVersion = version; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public TGIBlock TemplateKey { get { return new TGIBlock(1, null, type, group, instance); } set { if (new TGIBlock(1, null, type, group, instance) != value) { type = value.ResourceType; group = value.ResourceGroup; instance = value.Instance ; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public PolygonHeightOverrideList MinimumHeightOverrides { get { return minHeightOverrides; } set { if (minHeightOverrides != value) { minHeightOverrides = value == null ? null : new PolygonHeightOverrideList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public PolygonHeightOverrideList MaximumHeightOverrides { get { return maxHeightOverrides; } set { if (maxHeightOverrides != value) { maxHeightOverrides = value == null ? null : new PolygonHeightOverrideList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public AreaList FootprintAreas { get { return footprintAreas; } set { if (footprintAreas != value) { footprintAreas = value == null ? null : new AreaList(OnRCOLChanged, value, version); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public AreaList SlotAreas { get { return slotAreas; } set { if (slotAreas != value) { slotAreas = value == null ? null : new AreaList(OnRCOLChanged, value, version); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17)]
        public float MaxHeight { get { return maxHeight; } set { if (maxHeight != value) { maxHeight = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(18)]
        public float MinHeight { get { return minHeight; } set { if (minHeight != value) { minHeight = value; OnRCOLChanged(this, EventArgs.Empty); } } }

        public String Value { get { return ValueBuilder; } }
        public override List<string> ContentFields 
        {
            get
            {
                List<string> res = base.ContentFields;
                if (this.type == 0)
                {
                    res.Remove("MinimumHeightOverrides");
                    res.Remove("MaximumHeightOverrides");
                }
                else
                {
                    res.Remove("FootprintAreas");
                    res.Remove("SlotAreas");
                    res.Remove("MaxHeight");
                    res.Remove("MinHeight");
                }
                return res;
            }
        }
        #endregion
    }
}
