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
    public class RSLT : ARCOLBlock
    {
        static bool checking = s4pi.Settings.Settings.Checking;
        const string TAG = "RSLT";

        #region Attributes
        uint tag = (uint)FOURCC(TAG);
        uint version = 4;
        PartList routes;
        SlotOffsetList routeOffsets;

        SlottedPartList containers;
        SlotOffsetList containerOffsets;

        PartList effects;
        SlotOffsetList effectOffsets;

        PartList inverseKineticsTargets;
        SlotOffsetList inverseKineticsTargetOffsets;

        ConePartList cones;
        SlotOffsetList coneOffsets;
        #endregion

        #region Constructors
        public RSLT(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public RSLT(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public RSLT(int APIversion, EventHandler handler, RSLT basis)
            : base(APIversion, handler, null)
        {
            this.version = basis.version;
            this.routes = new PartList(handler, basis.routes);
            this.routeOffsets = basis.routeOffsets == null ? null : new SlotOffsetList(handler, basis.routeOffsets);
            this.containers = basis.containers == null ? null : new SlottedPartList(handler, basis.containers);
            this.containerOffsets = basis.containerOffsets == null ? null : new SlotOffsetList(handler, basis.containerOffsets);
            this.effects = basis.effects == null ? null : new PartList(handler, basis.effects);
            this.effectOffsets = basis.effectOffsets == null ? null : new SlotOffsetList(handler, basis.effectOffsets);
            this.inverseKineticsTargets = basis.inverseKineticsTargets == null ? null : new PartList(handler, basis.inverseKineticsTargets);
            this.inverseKineticsTargetOffsets = basis.inverseKineticsTargetOffsets == null ? null : new SlotOffsetList(handler, basis.inverseKineticsTargetOffsets);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0xD3044521; } }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            tag = r.ReadUInt32();
            if (checking) if (tag != (uint)FOURCC(TAG))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: '{1}'; at 0x{2:X8}", FOURCC(tag), TAG, s.Position));
            version = r.ReadUInt32();

            int nRouteSlots = r.ReadInt32();
            int nContainers = r.ReadInt32();
            int nEffects = r.ReadInt32();
            int nInverseKineticsTargets = r.ReadInt32();
            int nConeSlots = r.ReadInt32();
            if (checking) if (nConeSlots != 0)
                    throw new InvalidDataException(string.Format("Expected zero, read 0x{0:X8} at 0x{1:X8}", nConeSlots, s.Position));

            routes = new PartList(handler, s, nRouteSlots);
            if (nRouteSlots == 0)
                routeOffsets = new SlotOffsetList(handler);
            else
                routeOffsets = new SlotOffsetList(handler, s);

            containers = new SlottedPartList(handler, s, nContainers);
            if (nContainers == 0)
                containerOffsets = new SlotOffsetList(handler);
            else
                containerOffsets = new SlotOffsetList(handler, s);
            
            effects = new PartList(handler, s, nEffects);
            if (nEffects == 0)
                effectOffsets = new SlotOffsetList(handler);
            else
                effectOffsets = new SlotOffsetList(handler, s);
            
            inverseKineticsTargets = new PartList(handler, s, nInverseKineticsTargets);
            if (nInverseKineticsTargets == 0)
                inverseKineticsTargetOffsets = new SlotOffsetList(handler);
            else
                inverseKineticsTargetOffsets = new SlotOffsetList(handler, s);

            cones = new ConePartList(handler, s, nConeSlots);
            if (nConeSlots == 0)
                coneOffsets = new SlotOffsetList(handler);
            else
                coneOffsets = new SlotOffsetList(handler, s);
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(tag);
            w.Write(version);

            if (routes == null) routes = new PartList(handler);
            w.Write(routes.Count);
            if (containers == null) containers = new SlottedPartList(handler);
            w.Write(containers.Count);
            if (effects == null) effects = new PartList(handler);
            w.Write(effects.Count);
            if (inverseKineticsTargets == null) inverseKineticsTargets = new PartList(handler);
            w.Write(inverseKineticsTargets.Count);
            if (cones == null) cones = new ConePartList(handler);
            w.Write(cones.Count);

            routes.UnParse(ms);
            if (routes.Count > 0) routeOffsets.UnParse(ms);
            containers.UnParse(ms);
            if (containers.Count > 0) containerOffsets.UnParse(ms);
            effects.UnParse(ms);
            if (effects.Count > 0) effectOffsets.UnParse(ms);
            inverseKineticsTargets.UnParse(ms);
            if (inverseKineticsTargets.Count > 0) inverseKineticsTargetOffsets.UnParse(ms);
            cones.UnParse(ms);
            if (cones.Count > 0) coneOffsets.UnParse(ms);

            return ms;
        }
        #endregion

        #region Sub-types
        public class SlotOffset : AHandlerElement, IEquatable<SlotOffset>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            int slotIndex = -1;
            Vertex position = null;
            Vertex rotation = null;
            #endregion

            #region Constructors
            public SlotOffset(int APIversion, EventHandler handler)
                : this(APIversion, handler, -1, new Vertex(0, null), new Vertex(0, null)) { }
            public SlotOffset(int APIversion, EventHandler handler, SlotOffset basis)
                : this(APIversion, handler, basis.slotIndex, basis.position, basis.rotation) { }
            public SlotOffset(int APIversion, EventHandler handler,
                int slotIndex, Vertex position, Vertex rotation)
                : base(APIversion, handler)
            {
                this.slotIndex = slotIndex;
                this.position = new Vertex(requestedApiVersion, handler, position);
                this.rotation = new Vertex(requestedApiVersion, handler, rotation);
            }
            public SlotOffset(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                slotIndex = r.ReadInt32();
                position = new Vertex(requestedApiVersion, handler, s);
                rotation = new Vertex(requestedApiVersion, handler, s);
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(slotIndex);
                if (position == null) position = new Vertex(requestedApiVersion, handler);
                position.UnParse(s);
                if (rotation == null) rotation = new Vertex(requestedApiVersion, handler);
                rotation.UnParse(s);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<SlotOffset>
            public bool Equals(SlotOffset other)
            {
                return slotIndex == other.slotIndex
                    && position == other.position
                    && rotation == other.rotation
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as SlotOffset != null ? this.Equals(obj as SlotOffset) : false;
            }

            public override int GetHashCode()
            {
                return slotIndex.GetHashCode()
                    ^ position.GetHashCode()
                    ^ rotation.GetHashCode()
                    ;
            }
            #endregion

            #region Content Fields
            public int SlotIndex { get { return slotIndex; } set { if (slotIndex != value) { slotIndex = value; OnElementChanged(); } } }
            public Vertex Position { get { return position; } set { if (position != value) { position = value; OnElementChanged(); } } }
            public Vertex Rotation { get { return rotation; } set { if (rotation != value) { rotation = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class SlotOffsetList : DependentList<SlotOffset>
        {
            #region Constructors
            public SlotOffsetList(EventHandler handler) : base(handler) { }
            public SlotOffsetList(EventHandler handler, Stream s) : base(null) { elementHandler = handler; Parse(s); this.handler = handler; }
            public SlotOffsetList(EventHandler handler, IEnumerable<SlotOffset> lsf) : base(handler, lsf) { }
            #endregion

            #region Data I/O
            protected override SlotOffset CreateElement(Stream s) { return new SlotOffset(0, elementHandler, s); }
            protected override void WriteElement(Stream s, SlotOffset element) { element.UnParse(s); }
            #endregion

            public override void Add() { if (Count < MaxSize) base.Add(); } // fail silently if the list is full??!
        }

        public class TransformElement : AHandlerElement, IEquatable<TransformElement>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            float rot1 = 0f;
            float rot2 = 0f;
            float rot3 = 0f;
            float pos = 0f;
            #endregion

            #region Constructors
            public TransformElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public TransformElement(int APIversion, EventHandler handler, TransformElement basis)
                : this(APIversion, handler, basis.rot1, basis.rot2, basis.rot3, basis.pos) { }
            public TransformElement(int APIversion, EventHandler handler, float rot1, float rot2, float rot3, float pos)
                : base(APIversion, handler) { this.rot1 = rot1; this.rot2 = rot2; this.rot3 = rot3; this.pos = pos; }
            public TransformElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                rot1 = r.ReadSingle();
                rot2 = r.ReadSingle();
                rot3 = r.ReadSingle();
                pos = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(rot1);
                w.Write(rot2);
                w.Write(rot3);
                w.Write(pos);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<TransformElement>
            public bool Equals(TransformElement other) { return rot1 == other.rot1 && rot2 == other.rot2 && rot3 == other.rot3 && pos == other.pos; }
            public override bool Equals(object obj)
            {
                return obj as TransformElement != null ? this.Equals(obj as TransformElement) : false;
            }
            public override int GetHashCode()
            {
                return rot1.GetHashCode() ^ rot2.GetHashCode() ^ rot3.GetHashCode() ^ pos.GetHashCode(); 
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public float Rot1 { get { return rot1; } set { if (rot1 != value) { rot1 = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float Rot2 { get { return rot2; } set { if (rot2 != value) { rot2 = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public float Rot3 { get { return rot3; } set { if (rot3 != value) { rot3 = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public float Pos { get { return pos; } set { if (pos != value) { pos = value; OnElementChanged(); } } }

            public virtual string Value { get { return String.Format("Rot1: {0}; Rot2: {1}; Rot3: {2}; Pos: {3}", rot1, rot2, rot3, pos); } }
            #endregion
        }

        public class Part : AHandlerElement, IEquatable<Part>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            protected uint slotName;
            protected uint boneName;
            TransformElement tX;
            TransformElement tY;
            TransformElement tZ;
            #endregion

            #region Constructors
            public Part(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
                tX = new TransformElement(requestedApiVersion, handler);
                tY = new TransformElement(requestedApiVersion, handler);
                tZ = new TransformElement(requestedApiVersion, handler);
            }
            public Part(int APIversion, EventHandler handler, Part basis)
                : this(APIversion, handler, basis.slotName, basis.boneName, basis.tX, basis.tY, basis.tZ) { }
            public Part(int APIversion, EventHandler handler,
                uint slotName, uint boneName, TransformElement tX, TransformElement tY, TransformElement tZ)
                : base(APIversion, handler)
            {
                this.slotName = slotName;
                this.boneName = boneName;
                this.tX = new TransformElement(requestedApiVersion, handler, tX);
                this.tY = new TransformElement(requestedApiVersion, handler, tY);
                this.tZ = new TransformElement(requestedApiVersion, handler, tZ);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Part>
            public bool Equals(Part other)
            {
                return slotName.Equals(other.slotName)
                    && boneName.Equals(other.boneName)
                    && tX.Equals(other.tX)
                    && tY.Equals(other.tY)
                    && tZ.Equals(other.tZ)
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as Part != null ? this.Equals(obj as Part) : false;
            }

            public override int GetHashCode()
            {
                return slotName.GetHashCode()
                    ^ boneName.GetHashCode()
                    ^ tX.GetHashCode()
                    ^ tY.GetHashCode()
                    ^ tZ.GetHashCode()
                    ;
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint SlotName { get { return slotName; } set { if (slotName != value) { slotName = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public uint BoneName { get { return boneName; } set { if (boneName != value) { boneName = value; OnElementChanged(); } } }
            //[ElementPriority(3)] reserved for SlotPlacementFlags
            [ElementPriority(4)]
            public TransformElement X { get { return tX; } set { if (tX != value) { tX = new TransformElement(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(5)]
            public TransformElement Y { get { return tY; } set { if (tY != value) { tY = new TransformElement(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(6)]
            public TransformElement Z { get { return tZ; } set { if (tZ != value) { tZ = new TransformElement(requestedApiVersion, handler, value); OnElementChanged(); } } }

            public virtual string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class PartList : DependentList<Part>
        {
            #region Constructors
            public PartList(EventHandler handler) : base(handler) { }
            public PartList(EventHandler handler, Stream s, int count) : base(null) { elementHandler = handler; Parse(s, count); this.handler = handler; }
            public PartList(EventHandler handler, IEnumerable<Part> lsb) : base(handler, lsb) { }
            #endregion

            #region Data I/O
            protected void Parse(Stream s, int count)
            {
                uint[] slotNames = new uint[count];
                uint[] boneNames = new uint[count];
                BinaryReader r = new BinaryReader(s);
                for (int i = 0; i < slotNames.Length; i++) slotNames[i] = r.ReadUInt32();
                for (int i = 0; i < boneNames.Length; i++) boneNames[i] = r.ReadUInt32();
                for (int i = 0; i < count; i++) this.Add(new Part(0, elementHandler, slotNames[i], boneNames[i],
                    new TransformElement(0, elementHandler, s), new TransformElement(0, elementHandler, s), new TransformElement(0, elementHandler, s)));
            }
            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                for (int i = 0; i < Count; i++) w.Write(this[i].SlotName);
                for (int i = 0; i < Count; i++) w.Write(this[i].BoneName);
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].X == null) this[i].X = new TransformElement(0, handler);
                    this[i].X.UnParse(s);
                    if (this[i].Y == null) this[i].Y = new TransformElement(0, handler);
                    this[i].Y.UnParse(s);
                    if (this[i].Z == null) this[i].Z = new TransformElement(0, handler);
                    this[i].Z.UnParse(s);
                }
            }

            protected override Part CreateElement(Stream s) { throw new NotImplementedException(); }
            protected override void WriteElement(Stream s, Part element) { throw new NotImplementedException(); }
            #endregion
        }

        //SlotPlacement flags taken from ObjectCatalogResource.cs
        [Flags]
        public enum SlotPlacement : uint
        {
            //CheckFlags = 0xc3f38, 

            None = 0x01,
            //
            //
            Small = 0x08,

            Medium = 0x10,
            Large = 0x20,
            //
            //

            Sim = 0x0100,
            Chair = 0x0200,
            CounterSink = 0x0400,
            EndTable = 0x0800,

            Stool = 0x1000,
            CounterAppliance = 0x2000,
            //
            //

            //
            //
            Functional = 0x40000,
            Decorative = 0x80000,

            Upgrade = 0x1000000,
            //MatchFlags    = 0x2000000,
            Vertical = 0x2000000,
            PlacementOnly = 0x4000000,
            //

            //RotationFlags = 0x30000000,
            CardinalRotation = 0x10000000,
            FullRotation = 0x20000000,
            AlwaysUp = 0x40000000,
            //
        }
        public class SlottedPart : Part, IEquatable<SlottedPart>
        {
            SlotPlacement slotPlacementFlags;
            public SlottedPart(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public SlottedPart(int APIversion, EventHandler handler, SlottedPart basis) : base(APIversion, handler, basis) { slotPlacementFlags = basis.slotPlacementFlags; }
            public SlottedPart(int APIversion, EventHandler handler, uint slotName, uint boneName, SlotPlacement slotPlacementFlags,
                TransformElement tX, TransformElement tY, TransformElement tZ)
                : base(APIversion, handler, slotName, boneName, tX, tY, tZ) { this.slotPlacementFlags = slotPlacementFlags; }

            public bool Equals(SlottedPart other) { return ((Part)this).Equals((Part)other) && slotPlacementFlags.Equals(other.slotPlacementFlags); }
            public override bool Equals(object obj)
            {
                return obj as SlottedPart != null ? this.Equals(obj as SlottedPart) : false;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode() ^ slotPlacementFlags.GetHashCode();
            }

            [ElementPriority(3)]
            public SlotPlacement SlotPlacementFlags { get { return slotPlacementFlags; } set { if (slotPlacementFlags != value) { slotPlacementFlags = value; OnElementChanged(); } } }
        }
        public class SlottedPartList : DependentList<SlottedPart>
        {
            #region Constructors
            public SlottedPartList(EventHandler handler) : base(handler) { }
            public SlottedPartList(EventHandler handler, Stream s, int count) : base(null) { elementHandler = handler; Parse(s, count); this.handler = handler; }
            public SlottedPartList(EventHandler handler, IEnumerable<SlottedPart> lsbp) : base(handler, lsbp) { }
            #endregion

            #region Data I/O
            protected void Parse(Stream s, int count)
            {
                uint[] slotNames = new uint[count];
                uint[] boneNames = new uint[count];
                uint[] flags = new uint[count];
                BinaryReader r = new BinaryReader(s);
                for (int i = 0; i < slotNames.Length; i++) slotNames[i] = r.ReadUInt32();
                for (int i = 0; i < boneNames.Length; i++) boneNames[i] = r.ReadUInt32();
                for (int i = 0; i < flags.Length; i++) flags[i] = r.ReadUInt32();
                for (int i = 0; i < count; i++) this.Add(new SlottedPart(0, elementHandler, slotNames[i], boneNames[i], (SlotPlacement)flags[i],
                    new TransformElement(0, elementHandler, s), new TransformElement(0, elementHandler, s), new TransformElement(0, elementHandler, s)));
            }
            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                for (int i = 0; i < Count; i++) w.Write(this[i].SlotName);
                for (int i = 0; i < Count; i++) w.Write(this[i].BoneName);
                for (int i = 0; i < Count; i++) w.Write((uint)this[i].SlotPlacementFlags);
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].X == null) this[i].X = new TransformElement(0, handler);
                    this[i].X.UnParse(s);
                    if (this[i].Y == null) this[i].Y = new TransformElement(0, handler);
                    this[i].Y.UnParse(s);
                    if (this[i].Z == null) this[i].Z = new TransformElement(0, handler);
                    this[i].Z.UnParse(s);
                }
            }

            protected override SlottedPart CreateElement(Stream s) { throw new NotImplementedException(); }
            protected override void WriteElement(Stream s, SlottedPart element) { throw new NotImplementedException(); }
            #endregion
        }

        public class ConeElement : AHandlerElement, IEquatable<ConeElement>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            float radius;
            float angle;
            #endregion

            #region Constructors
            public ConeElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public ConeElement(int APIversion, EventHandler handler, ConeElement basis)
                : this(APIversion, handler, basis.radius, basis.angle) { }
            public ConeElement(int APIversion, EventHandler handler, float radius, float angle)
                : base(APIversion, handler)
            {
                this.radius = radius;
                this.angle = angle;
            }
            public ConeElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                radius = r.ReadSingle();
                angle = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(radius);
                w.Write(angle);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Part>
            public bool Equals(ConeElement other) { return angle.Equals(other.angle) && radius.Equals(other.radius); }
            public override bool Equals(object obj) { return obj as Part != null ? this.Equals(obj as Part) : false; }
            public override int GetHashCode() { return angle.GetHashCode() ^ radius.GetHashCode(); }
            #endregion
        }

        public class ConePart : AHandlerElement, IEquatable<ConePart>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            protected uint slotName;
            protected uint boneName;
            TransformElement tX;
            TransformElement tY;
            TransformElement tZ;
            ConeElement cone;
            #endregion

            #region Constructors
            public ConePart(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
                tX = new TransformElement(requestedApiVersion, handler);
                tY = new TransformElement(requestedApiVersion, handler);
                tZ = new TransformElement(requestedApiVersion, handler);
            }
            public ConePart(int APIversion, EventHandler handler, ConePart basis)
                : this(APIversion, handler, basis.slotName, basis.boneName, basis.tX, basis.tY, basis.tZ, basis.cone) { }
            public ConePart(int APIversion, EventHandler handler,
                uint slotName, uint boneName, TransformElement tX, TransformElement tY, TransformElement tZ, ConeElement cone)
                : base(APIversion, handler)
            {
                this.slotName = slotName;
                this.boneName = boneName;
                this.tX = new TransformElement(requestedApiVersion, handler, tX);
                this.tY = new TransformElement(requestedApiVersion, handler, tY);
                this.tZ = new TransformElement(requestedApiVersion, handler, tZ);
                this.cone = new ConeElement(requestedApiVersion, handler, cone);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Part>
            public bool Equals(ConePart other)
            {
                return slotName.Equals(other.slotName)
                    && boneName.Equals(other.boneName)
                    && tX.Equals(other.tX)
                    && tY.Equals(other.tY)
                    && tZ.Equals(other.tZ)
                    && cone.Equals(other.cone)
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as Part != null ? this.Equals(obj as Part) : false;
            }

            public override int GetHashCode()
            {
                return slotName.GetHashCode()
                    ^ boneName.GetHashCode()
                    ^ tX.GetHashCode()
                    ^ tY.GetHashCode()
                    ^ tZ.GetHashCode()
                    ^ cone.GetHashCode()
                    ;
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint SlotName { get { return slotName; } set { if (slotName != value) { slotName = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public uint BoneName { get { return boneName; } set { if (boneName != value) { boneName = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public TransformElement X { get { return tX; } set { if (tX != value) { tX = new TransformElement(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(5)]
            public TransformElement Y { get { return tY; } set { if (tY != value) { tY = new TransformElement(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(6)]
            public TransformElement Z { get { return tZ; } set { if (tZ != value) { tZ = new TransformElement(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(7)]
            public ConeElement Cone { get { return cone; } set { if (cone != value) { cone = new ConeElement(requestedApiVersion, handler, value); OnElementChanged(); } } }

            public virtual string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class ConePartList : DependentList<ConePart>
        {
            #region Constructors
            public ConePartList(EventHandler handler) : base(handler) { }
            public ConePartList(EventHandler handler, Stream s, int count) : base(null) { elementHandler = handler; Parse(s, count); this.handler = handler; }
            public ConePartList(EventHandler handler, IEnumerable<ConePart> lsb) : base(handler, lsb) { }
            #endregion

            #region Data I/O
            protected void Parse(Stream s, int count)
            {
                uint[] slotNames = new uint[count];
                uint[] boneNames = new uint[count];
                TransformElement[][] transformElements = new TransformElement[count][];
                ConeElement[] coneElements = new ConeElement[count];
                BinaryReader r = new BinaryReader(s);

                for (int i = 0; i < slotNames.Length; i++) slotNames[i] = r.ReadUInt32();
                for (int i = 0; i < boneNames.Length; i++) boneNames[i] = r.ReadUInt32();
                for (int i = 0; i < transformElements.Length; i++)
                {
                    transformElements[i] = new TransformElement[3];
                    transformElements[i][0] = new TransformElement(0, elementHandler, s);//X
                    transformElements[i][1] = new TransformElement(0, elementHandler, s);//Y
                    transformElements[i][2] = new TransformElement(0, elementHandler, s);//Z
                }
                for (int i = 0; i < coneElements.Length; i++) coneElements[i] = new ConeElement(0, elementHandler, s);

                for (int i = 0; i < count; i++) this.Add(new ConePart(0, elementHandler, slotNames[i], boneNames[i],
                    transformElements[i][0], transformElements[i][1], transformElements[i][2], coneElements[i]));
            }
            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                for (int i = 0; i < Count; i++) w.Write(this[i].SlotName);
                for (int i = 0; i < Count; i++) w.Write(this[i].BoneName);
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].X == null) this[i].X = new TransformElement(0, handler);
                    this[i].X.UnParse(s);
                    if (this[i].Y == null) this[i].Y = new TransformElement(0, handler);
                    this[i].Y.UnParse(s);
                    if (this[i].Z == null) this[i].Z = new TransformElement(0, handler);
                    this[i].Z.UnParse(s);
                }
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].Cone == null) this[i].Cone = new ConeElement(0, handler);
                    this[i].Cone.UnParse(s);
                }
            }

            protected override ConePart CreateElement(Stream s) { throw new NotImplementedException(); }
            protected override void WriteElement(Stream s, ConePart element) { throw new NotImplementedException(); }
            #endregion
        }
        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public PartList Routes { get { return routes; } set { if (routes != value) { routes = new PartList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public SlotOffsetList RouteOffsets { get { return routeOffsets; } set { if (routeOffsets != value) { routeOffsets = value == null ? null : new SlotOffsetList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public SlottedPartList Containers { get { return containers; } set { if (containers != value) { containers = value == null ? null : new SlottedPartList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public SlotOffsetList ContainerOffsets { get { return containerOffsets; } set { if (containerOffsets != value) { containerOffsets = value == null ? null : new SlotOffsetList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public PartList Effects { get { return effects; } set { if (effects != value) { effects = value == null ? null : new PartList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17)]
        public SlotOffsetList EffectOffsets { get { return effectOffsets; } set { if (effectOffsets != value) { effectOffsets = value == null ? null : new SlotOffsetList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(18)]
        public PartList InverseKineticsTargets { get { return inverseKineticsTargets; } set { if (inverseKineticsTargets != value) { inverseKineticsTargets = value == null ? null : new PartList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(19)]
        public SlotOffsetList InverseKineticsTargetOffsets { get { return inverseKineticsTargetOffsets; } set { if (inverseKineticsTargetOffsets != value) { inverseKineticsTargetOffsets = value == null ? null : new SlotOffsetList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(20)]
        public ConePartList Cones { get { return cones; } set { if (cones != value) { cones = value == null ? null : new ConePartList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(21)]
        public SlotOffsetList ConeOffsets { get { return coneOffsets; } set { if (coneOffsets != value) { coneOffsets = value == null ? null : new SlotOffsetList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }
}
