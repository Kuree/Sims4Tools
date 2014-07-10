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
        uint version = 7;
        AreaList footprintAreas;
        AreaList slotAreas;
        #endregion

        #region Constructors
        public FTPT(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public FTPT(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public FTPT(int APIversion, EventHandler handler, FTPT basis)
            : base(APIversion, null, null)
        {
            this.handler = handler;
            this.version = basis.version;
            this.footprintAreas = new AreaList(OnRCOLChanged, basis.footprintAreas, version);
            this.slotAreas = new AreaList(OnRCOLChanged, basis.slotAreas, version);
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
            footprintAreas = new AreaList(OnRCOLChanged, s, version);
            slotAreas = new AreaList(OnRCOLChanged, s, version);
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(tag);
            w.Write(version);
            if (footprintAreas == null) footprintAreas = new AreaList(OnRCOLChanged, version);
            footprintAreas.UnParse(ms);
            if (slotAreas == null) slotAreas = new AreaList(OnRCOLChanged, version);
            slotAreas.UnParse(ms);

            return ms;
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
        public enum AreaType : uint
        {
            Unknown00 = 0x00,

            ForPlacement = 0x01,
            ForPathing = 0x02,
            IsEnabled = 0x04,
            IsDiscouraged = 0x08,

            ForShell = 0x10,
            Unknown20 = 0x20,
            Unknown40 = 0x40,
            Unknown80 = 0x80,
        }
        [Flags]
        public enum AllowIntersection : uint
        {
            None = 0x00,

            Unknown01 = 0x01,
            Walls = 0x02,
            Objects = 0x04,
            Sims = 0x08,

            Roofs = 0x10,
            Fences = 0x20,
            ModularStairs = 0x40,
            ObjectsOfSameType = 0x80,
        }
        [Flags]
        public enum SurfaceType : uint
        {
            Unknown00 = 0x00,

            Terrain = 0x01,
            Floor = 0x02,
            Pool = 0x04,
            Pond = 0x08,

            Fence = 0x10,
            AnySurface = 0x20,
            Air = 0x40,
            Roof = 0x80,
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
            AreaType areaTypeFlags;
            PolygonPointList closedPolygon;
            AllowIntersection allowIntersectionFlags;
            SurfaceType surfaceTypeFlags;
            SurfaceAttribute surfaceAttributeFlags;
            byte levelOffset;
            float elevationOffset;//FTPT.Version>=0x07
            PolygonPoint lower;
            PolygonPoint upper;
            #endregion

            #region Constructors
            public Area(int APIversion, EventHandler handler, uint version) : base(APIversion, handler)
            {
                this.ParentVersion = version;
                Stream ms = new MemoryStream();
                UnParse(ms);
                ms.Position = 0;
                Parse(ms);
            }
            public Area(int APIversion, EventHandler handler, Stream s, uint version) : base(APIversion, handler) { this.ParentVersion = version; Parse(s); }
            public Area(int APIversion, EventHandler handler, Area basis)
                : this(APIversion, handler, basis.ParentVersion,
                basis.name, basis.priority, basis.areaTypeFlags, basis.closedPolygon,
                basis.allowIntersectionFlags, basis.surfaceTypeFlags, basis.surfaceAttributeFlags,
                basis.levelOffset,
                basis.elevationOffset,
                basis.lower, basis.upper) { }
            public Area(int APIversion, EventHandler handler, uint version,
                uint name, byte priority, AreaType areaTypeFlags, IEnumerable<PolygonPoint> closedPolygon,
                AllowIntersection allowIntersectionFlags, SurfaceType surfaceTypeFlags, SurfaceAttribute surfaceAttributeFlags,
                byte levelOffset,
                PolygonPoint lower, PolygonPoint upper)
                : this(APIversion, handler, version,
                name, priority, areaTypeFlags, closedPolygon,
                allowIntersectionFlags, surfaceTypeFlags, surfaceAttributeFlags,
                levelOffset,
                0,
                lower, upper)
            {
                if (checking) if (version >= 0x00000007)
                        throw new InvalidOperationException(String.Format("Constructor requires ElevationOffset for version {0}", version));
            }
            public Area(int APIversion, EventHandler handler, uint version,
                uint name, byte priority, AreaType areaTypeFlags, IEnumerable<PolygonPoint> closedPolygon,
                AllowIntersection allowIntersectionFlags, SurfaceType surfaceTypeFlags, SurfaceAttribute surfaceAttributeFlags,
                byte levelOffset,
                float elevationOffset,
                PolygonPoint lower, PolygonPoint upper)
                : base(APIversion, handler)
            {
                this.ParentVersion = version;

                this.name = name;
                this.priority = priority;
                this.areaTypeFlags = areaTypeFlags;
                this.closedPolygon = closedPolygon == null ? null : new PolygonPointList(handler, closedPolygon);
                this.allowIntersectionFlags = allowIntersectionFlags;
                this.surfaceTypeFlags = surfaceTypeFlags;
                this.surfaceAttributeFlags = surfaceAttributeFlags;
                this.levelOffset = levelOffset;
                this.elevationOffset = elevationOffset;
                this.lower = new PolygonPoint(APIversion, handler, lower);
                this.upper = new PolygonPoint(APIversion, handler, upper);
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.name = r.ReadUInt32();
                this.priority = r.ReadByte();
                this.areaTypeFlags = (AreaType)r.ReadUInt32();
                this.closedPolygon = new PolygonPointList(handler, s);
                this.allowIntersectionFlags = (AllowIntersection)r.ReadUInt32();
                this.surfaceTypeFlags = (SurfaceType)r.ReadUInt32();
                this.surfaceAttributeFlags = (SurfaceAttribute)r.ReadUInt32();
                this.levelOffset = r.ReadByte();
                this.elevationOffset = ParentVersion >= 0x00000007 ? r.ReadSingle() : 0;
                this.lower = new PolygonPoint(requestedApiVersion, handler, s);
                this.upper = new PolygonPoint(requestedApiVersion, handler, s);
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(name);
                w.Write(priority);
                w.Write((uint)areaTypeFlags);
                if (closedPolygon == null) closedPolygon = new PolygonPointList(handler);
                closedPolygon.UnParse(s);
                w.Write((uint)allowIntersectionFlags);
                w.Write((uint)surfaceTypeFlags);
                w.Write((uint)surfaceAttributeFlags);
                w.Write(levelOffset);
                if (ParentVersion >= 0x00000007) w.Write(elevationOffset);
                if (lower == null) lower = new PolygonPoint(requestedApiVersion, handler);
                lower.UnParse(s);
                if (upper == null) upper = new PolygonPoint(requestedApiVersion, handler);
                upper.UnParse(s);
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
                    closedPolygon.Equals(other.closedPolygon) &&
                    allowIntersectionFlags == other.allowIntersectionFlags &&
                    surfaceTypeFlags == other.surfaceTypeFlags &&
                    surfaceAttributeFlags == other.surfaceAttributeFlags &&
                    levelOffset == other.levelOffset &&
                    lower.Equals(other.lower) &&
                    upper.Equals(other.upper);
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
                    closedPolygon.GetHashCode() ^
                    allowIntersectionFlags.GetHashCode() ^
                    surfaceTypeFlags.GetHashCode() ^
                    surfaceAttributeFlags.GetHashCode() ^
                    levelOffset.GetHashCode() ^
                    lower.GetHashCode() ^
                    upper.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint Name { get { return name; } set { if (name != value) { name = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public byte Priority { get { return priority; } set { if (priority != value) { priority = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public AreaType AreaTypeFlags { get { return areaTypeFlags; } set { if (areaTypeFlags != value) { areaTypeFlags = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public PolygonPointList ClosedPolygon { get { return closedPolygon; } set { if (closedPolygon != value) { closedPolygon = value == null ? null : new PolygonPointList(handler, value); OnElementChanged(); } } }
            [ElementPriority(5)]
            public AllowIntersection AllowIntersectionFlags { get { return allowIntersectionFlags; } set { if (allowIntersectionFlags != value) { allowIntersectionFlags = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public SurfaceType SurfaceTypeFlags { get { return surfaceTypeFlags; } set { if (surfaceTypeFlags != value) { surfaceTypeFlags = value; OnElementChanged(); } } }
            [ElementPriority(7)]
            public SurfaceAttribute SurfaceAttributeFlags { get { return surfaceAttributeFlags; } set { if (surfaceAttributeFlags != value) { surfaceAttributeFlags = value; OnElementChanged(); } } }
            [ElementPriority(8)]
            public byte LevelOffset { get { return levelOffset; } set { if (levelOffset != value) { levelOffset = value; OnElementChanged(); } } }
            [ElementPriority(9)]
            public float ElevationOffset
            {
                get { if (ParentVersion < 0x00000007) throw new InvalidOperationException(); return elevationOffset; }
                set { if (ParentVersion < 0x00000007) throw new InvalidOperationException(); if (elevationOffset != value) { elevationOffset = value; OnElementChanged(); } }
            }
            [ElementPriority(10)]
            public PolygonPoint Lower { get { return lower; } set { if (lower != value) { lower = value; OnElementChanged(); } } }
            [ElementPriority(11)]
            public PolygonPoint Upper { get { return upper; } set { if (upper != value) { upper = value; OnElementChanged(); } } }

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
        public AreaList FootprintAreas { get { return footprintAreas; } set { if (footprintAreas != value) { footprintAreas = new AreaList(OnRCOLChanged, value, version); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public AreaList SlotAreas { get { return slotAreas; } set { if (slotAreas != value) { slotAreas = new AreaList(OnRCOLChanged, value, version); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value
        {
            get
            {
                return ValueBuilder;
                /*
                string fmt;
                string s = "";
                s += "Tag: 0x" + tag.ToString("X8");
                s += "\nVersion: 0x" + version.ToString("X8");

                s += String.Format("\nFootprintAreas ({0:X}):", footprintAreas.Count);
                fmt = "\n--[{0:X" + footprintAreas.Count.ToString("X").Length + "}]--\n{1}\n--";
                for (int i = 0; i < footprintAreas.Count; i++)
                    s += String.Format(fmt, i, footprintAreas[i].Value);
                s += "\n----";

                s += String.Format("\nSlotAreas ({0:X}):", slotAreas.Count);
                fmt = "\n--[{0:X" + slotAreas.Count.ToString("X").Length + "}]--\n{1}\n--";
                for (int i = 0; i < slotAreas.Count; i++)
                    s += String.Format(fmt, i, slotAreas[i].Value);
                return s;
                **/
            }
        }
        #endregion
    }
}
