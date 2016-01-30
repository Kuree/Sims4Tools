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
        public RSLT(int apiVersion, EventHandler handler) : base(apiVersion, handler, null) { }
        public RSLT(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler, s) { }
        public RSLT(int apiVersion, EventHandler handler, RSLT basis)
            : base(apiVersion, handler, null)
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
            public SlotOffset(int apiVersion, EventHandler handler)
                : this(apiVersion, handler, -1, new Vertex(0, null), new Vertex(0, null)) { }
            public SlotOffset(int apiVersion, EventHandler handler, SlotOffset basis)
                : this(apiVersion, handler, basis.slotIndex, basis.position, basis.rotation) { }
            public SlotOffset(int apiVersion, EventHandler handler,
                int slotIndex, Vertex position, Vertex rotation)
                : base(apiVersion, handler)
            {
                this.slotIndex = slotIndex;
                this.position = new Vertex(requestedApiVersion, handler, position);
                this.rotation = new Vertex(requestedApiVersion, handler, rotation);
            }
            public SlotOffset(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
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

        public class MatrixRow : AHandlerElement, IEquatable<MatrixRow>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            float rot1 = 0f;
            float rot2 = 0f;
            float rot3 = 0f;
            #endregion

            #region Constructors
            public MatrixRow(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public MatrixRow(int apiVersion, EventHandler handler, MatrixRow basis)
                : this(apiVersion, handler, basis.rot1, basis.rot2, basis.rot3) { }
            public MatrixRow(int apiVersion, EventHandler handler, float x, float y, float z)
                : base(apiVersion, handler) { this.rot1 = x; this.rot2 = y; this.rot3 = z; }
            public MatrixRow(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                rot1 = r.ReadSingle();
                rot2 = r.ReadSingle();
                rot3 = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(rot1);
                bw.Write(rot2);
                bw.Write(rot3);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Vector3>
            public bool Equals(MatrixRow other) { return rot1 == other.rot1 && rot2 == other.rot2 && rot3 == other.rot3; }
            public override bool Equals(object obj)
            {
                return obj as MatrixRow != null ? this.Equals(obj as MatrixRow) : false;
            }
           // public override int GetHashCode()
           // {
           //     return rot1.GetHashCode() ^ rot2.GetHashCode() ^ rot3.GetHashCode() ^ pos.GetHashCode(); 
           // }
            public override int GetHashCode()
            {
                return rot1.GetHashCode() ^ rot2.GetHashCode() ^ rot3.GetHashCode();
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

           // public virtual string Value { get { return String.Format("Rot1: {0}; Rot2: {1}; Rot3: {2}; Pos: {3}", rot1, rot2, rot3, pos); } }
            public virtual string Value { get { return String.Format(" {0}; {1}; {2};", rot1, rot2, rot3); } }
            #endregion
        }

        public class Vector3 : AHandlerElement, IEquatable<Vector3>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            float x = 0f;
            float y = 0f;
            float z = 0f;
            #endregion

            #region Constructors
            public Vector3(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public Vector3(int apiVersion, EventHandler handler, Vector3 basis)
                : this(apiVersion, handler, basis.x, basis.y, basis.z) { }
            public Vector3(int apiVersion, EventHandler handler, float x, float y, float z)
                : base(apiVersion, handler) { this.x = x; this.y = y; this.z = z; }
            public Vector3(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                x = r.ReadSingle();
                y = r.ReadSingle();
                z = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(x);
                bw.Write(y);
                bw.Write(z);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Vector3>
            public bool Equals(Vector3 other) { return x == other.x && y == other.y && z == other.z; }
            public override bool Equals(object obj)
            {
                return obj as Vector3 != null ? this.Equals(obj as Vector3) : false;
            }
            public override int GetHashCode()
            {
                return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public float X { get { return x; } set { if (x != value) { x = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float Y { get { return y; } set { if (y != value) { y = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public float Z { get { return z; } set { if (z != value) { z = value; OnElementChanged(); } } }
           // [ElementPriority(4)]

            public virtual string Value { get { return String.Format("X: {0}; Y: {1}; Z: {2};", x, y, z); } }
            #endregion
        }

        public class Part : AHandlerElement, IEquatable<Part>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            protected uint slotName;
            protected uint boneName;
            MatrixRow matrixX;
            MatrixRow matrixY;
            MatrixRow matrixZ;
            Vector3 coordinates;
            #endregion

            #region Constructors
            public Part(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                matrixX = new MatrixRow(requestedApiVersion, handler);
                matrixY = new MatrixRow(requestedApiVersion, handler);
                matrixZ = new MatrixRow(requestedApiVersion, handler);
                coordinates = new Vector3(requestedApiVersion, handler);
            }
            public Part(int apiVersion, EventHandler handler, Part basis)
                : this(apiVersion, handler, basis.slotName, basis.boneName, basis.matrixX, basis.matrixY, basis.matrixZ, basis.coordinates) { }
            public Part(int apiVersion, EventHandler handler,
                uint slotName, uint boneName, MatrixRow tX, MatrixRow tY, MatrixRow tZ, Vector3 coordinates)
                : base(apiVersion, handler)
            {
                this.slotName = slotName;
                this.boneName = boneName;
                this.matrixX = new MatrixRow(requestedApiVersion, handler, tX);
                this.matrixY = new MatrixRow(requestedApiVersion, handler, tY);
                this.matrixZ = new MatrixRow(requestedApiVersion, handler, tZ);
                this.coordinates = new Vector3(requestedApiVersion, handler, coordinates);
            }
            public Part(int apiVersion, EventHandler handler,
                uint slotName, uint boneName, MatrixRow[] matrix, Vector3 coordinates)
                : base(apiVersion, handler)
            {
                this.slotName = slotName;
                this.boneName = boneName;
                this.matrixX = new MatrixRow(requestedApiVersion, handler, matrix[0]);
                this.matrixY = new MatrixRow(requestedApiVersion, handler, matrix[1]);
                this.matrixZ = new MatrixRow(requestedApiVersion, handler, matrix[2]);
                this.coordinates = new Vector3(requestedApiVersion, handler, coordinates);
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
                    && matrixX.Equals(other.matrixX)
                    && matrixY.Equals(other.matrixY)
                    && matrixZ.Equals(other.matrixZ)
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
                    ^ matrixX.GetHashCode()
                    ^ matrixY.GetHashCode()
                    ^ matrixZ.GetHashCode()
                    ;
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint PointSlotNameHash { get { return slotName; } set { if (slotName != value) { slotName = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public uint TargetBoneNameHash { get { return boneName; } set { if (boneName != value) { boneName = value; OnElementChanged(); } } }
            [ElementPriority(7)]
            public MatrixRow TransformMatrixX { get { return matrixX; } set { if (matrixX != value) { matrixX = new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(8)]
            public MatrixRow TransformMatrixY { get { return matrixY; } set { if (matrixY != value) { matrixY = new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(9)]
            public MatrixRow TransformMatrixZ { get { return matrixZ; } set { if (matrixZ != value) { matrixZ = new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(10)]
            public Vector3 SlotCoordinates { get { return coordinates; } set { if (coordinates != value) { coordinates = new Vector3(requestedApiVersion, handler, value); OnElementChanged(); } } }

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
                MatrixRow[][] matrix = new MatrixRow[count][];
                Vector3[] coords = new Vector3[count];
                BinaryReader r = new BinaryReader(s);
                for (int i = 0; i < slotNames.Length; i++) slotNames[i] = r.ReadUInt32();
                for (int i = 0; i < boneNames.Length; i++) boneNames[i] = r.ReadUInt32();
                for (int i = 0; i < count; i++)
                {
                    matrix[i] = new MatrixRow[3];
                    float[] tmp = new float[3];
                    for (int j = 0; j < 3; j++)
                    {
                        matrix[i][j] = new MatrixRow(0, elementHandler, s);
                        tmp[j] = r.ReadSingle();
                    }
                    coords[i] = new Vector3(0, elementHandler, tmp[0], tmp[1], tmp[2]);
                }
              //  for (int i = 0; i < count; i++) this.Add(new Part(0, elementHandler, slotNames[i], boneNames[i],
              //      new Vector3(0, elementHandler, s), new Vector3(0, elementHandler, s), new Vector3(0, elementHandler, s)));
                for (int i = 0; i < count; i++) this.Add(new Part(0, elementHandler, slotNames[i], boneNames[i],
                    matrix[i], coords[i]));
            }
            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                for (int i = 0; i < Count; i++) w.Write(this[i].PointSlotNameHash);
                for (int i = 0; i < Count; i++) w.Write(this[i].TargetBoneNameHash);
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].TransformMatrixX == null) this[i].TransformMatrixX = new MatrixRow(0, handler);
                    this[i].TransformMatrixX.UnParse(s);
                    w.Write(this[i].SlotCoordinates.X);
                    if (this[i].TransformMatrixY == null) this[i].TransformMatrixY = new MatrixRow(0, handler);
                    this[i].TransformMatrixY.UnParse(s);
                    w.Write(this[i].SlotCoordinates.Y);
                    if (this[i].TransformMatrixZ == null) this[i].TransformMatrixZ = new MatrixRow(0, handler);
                    this[i].TransformMatrixZ.UnParse(s);
                    w.Write(this[i].SlotCoordinates.Z);
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
           // SlotPlacement slotPlacementFlags;

            byte slotSize;  
            ulong slotTypeSet; 
            bool slotDirectionLocked; 
            uint slotLegacyHash;

            public SlottedPart(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public SlottedPart(int apiVersion, EventHandler handler, SlottedPart basis) : base(apiVersion, handler, basis) {
                this.slotSize = basis.slotSize; this.slotTypeSet = basis.slotTypeSet; this.slotDirectionLocked = basis.slotDirectionLocked; this.slotLegacyHash = basis.slotLegacyHash;
            }
            public SlottedPart(int apiVersion, EventHandler handler, uint slotName, uint boneName,
                byte slotSize, ulong slotTypeSet, bool slotDirectionLocked, uint slotLegacyHash,
                MatrixRow[] matrix, Vector3 coordinates)
                : base(apiVersion, handler, slotName, boneName, matrix, coordinates)
            {
                this.slotSize = slotSize; this.slotTypeSet = slotTypeSet; this.slotDirectionLocked = slotDirectionLocked; this.slotLegacyHash = slotLegacyHash;
            }
            
            public bool Equals(SlottedPart other) { return ((Part)this).Equals((Part)other); }
            public override bool Equals(object obj)
            {
                return obj as SlottedPart != null ? this.Equals(obj as SlottedPart) : false;
            }
            public override int GetHashCode()
            {
               // return base.GetHashCode() ^ slotPlacementFlags.GetHashCode();
                return base.GetHashCode();
            }

            [ElementPriority(3)]
            public byte SlotSize { get { return slotSize; } set { if (slotSize != value) { slotSize = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public ulong SlotTypeSet { get { return slotTypeSet; } set { if (slotTypeSet != value) { slotTypeSet = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public bool SlotDirectionLocked { get { return slotDirectionLocked; } set { if (slotDirectionLocked != value) { slotDirectionLocked = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public uint SlotLegacyHash { get { return slotLegacyHash; } set { if (slotLegacyHash != value) { slotLegacyHash = value; OnElementChanged(); } } }
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
                byte[] slotSize = new byte[count];
                ulong[] slotTypeSet = new ulong[count];
                bool[] slotDirectionLocked = new bool[count];
                uint[] slotLegacyHash = new uint[count];
                BinaryReader r = new BinaryReader(s);
                for (int i = 0; i < slotNames.Length; i++) slotNames[i] = r.ReadUInt32();
                for (int i = 0; i < boneNames.Length; i++) boneNames[i] = r.ReadUInt32();
                for (int i = 0; i < count; i++) slotSize[i] = r.ReadByte(); 
                for (int i = 0; i < count; i++) slotTypeSet[i] = r.ReadUInt64();
                for (int i = 0; i < count; i++) slotDirectionLocked[i] = r.ReadBoolean();
                for (int i = 0; i < count; i++) slotLegacyHash[i] = r.ReadUInt32();
                MatrixRow[][] matrix = new MatrixRow[count][];
                Vector3[] coords = new Vector3[count];
                for (int i = 0; i < count; i++)
                {
                    matrix[i] = new MatrixRow[3];
                    float[] tmp = new float[3];
                    for (int j = 0; j < 3; j++)
                    {
                        matrix[i][j] = new MatrixRow(0, elementHandler, s);
                        tmp[j] = r.ReadSingle();
                    }
                    coords[i] = new Vector3(0, elementHandler, tmp[0], tmp[1], tmp[2]);
                }

                for (int i = 0; i < count; i++) this.Add(new SlottedPart(0, elementHandler, slotNames[i], boneNames[i],
                    slotSize[i], slotTypeSet[i], slotDirectionLocked[i], slotLegacyHash[i],
                    matrix[i], coords[i]));
            }
            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                for (int i = 0; i < Count; i++) w.Write(this[i].PointSlotNameHash);
                for (int i = 0; i < Count; i++) w.Write(this[i].TargetBoneNameHash);
              //  for (int i = 0; i < Count; i++) w.Write((uint)this[i].SlotPlacementFlags);
                for (int i = 0; i < Count; i++) w.Write(this[i].SlotSize);
                for (int i = 0; i < Count; i++) w.Write(this[i].SlotTypeSet);
                for (int i = 0; i < Count; i++) w.Write(this[i].SlotDirectionLocked);
                for (int i = 0; i < Count; i++) w.Write(this[i].SlotLegacyHash);
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].TransformMatrixX == null) this[i].TransformMatrixX = new MatrixRow(0, handler);
                    this[i].TransformMatrixX.UnParse(s);
                    w.Write(this[i].SlotCoordinates.X);
                    if (this[i].TransformMatrixY == null) this[i].TransformMatrixY = new MatrixRow(0, handler);
                    this[i].TransformMatrixY.UnParse(s);
                    w.Write(this[i].SlotCoordinates.Y);
                    if (this[i].TransformMatrixZ == null) this[i].TransformMatrixZ = new MatrixRow(0, handler);
                    this[i].TransformMatrixZ.UnParse(s);
                    w.Write(this[i].SlotCoordinates.Z);
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
            public ConeElement(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public ConeElement(int apiVersion, EventHandler handler, ConeElement basis)
                : this(apiVersion, handler, basis.radius, basis.angle) { }
            public ConeElement(int apiVersion, EventHandler handler, float radius, float angle)
                : base(apiVersion, handler)
            {
                this.radius = radius;
                this.angle = angle;
            }
            public ConeElement(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
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
            MatrixRow tX;
            MatrixRow tY;
            MatrixRow tZ;
            ConeElement cone;
            #endregion

            #region Constructors
            public ConePart(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                tX = new MatrixRow(requestedApiVersion, handler);
                tY = new MatrixRow(requestedApiVersion, handler);
                tZ = new MatrixRow(requestedApiVersion, handler);
            }
            public ConePart(int apiVersion, EventHandler handler, ConePart basis)
                : this(apiVersion, handler, basis.slotName, basis.boneName, basis.tX, basis.tY, basis.tZ, basis.cone) { }
            public ConePart(int apiVersion, EventHandler handler,
                uint slotName, uint boneName, MatrixRow tX, MatrixRow tY, MatrixRow tZ, ConeElement cone)
                : base(apiVersion, handler)
            {
                this.slotName = slotName;
                this.boneName = boneName;
                this.tX = new MatrixRow(requestedApiVersion, handler, tX);
                this.tY = new MatrixRow(requestedApiVersion, handler, tY);
                this.tZ = new MatrixRow(requestedApiVersion, handler, tZ);
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
            public MatrixRow X { get { return tX; } set { if (tX != value) { tX = new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(5)]
            public MatrixRow Y { get { return tY; } set { if (tY != value) { tY = new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(6)]
            public MatrixRow Z { get { return tZ; } set { if (tZ != value) { tZ = new MatrixRow(requestedApiVersion, handler, value); OnElementChanged(); } } }
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
                MatrixRow[][] transformElements = new MatrixRow[count][];
                ConeElement[] coneElements = new ConeElement[count];
                BinaryReader r = new BinaryReader(s);

                for (int i = 0; i < slotNames.Length; i++) slotNames[i] = r.ReadUInt32();
                for (int i = 0; i < boneNames.Length; i++) boneNames[i] = r.ReadUInt32();
                for (int i = 0; i < transformElements.Length; i++)
                {
                    transformElements[i] = new MatrixRow[3];
                    transformElements[i][0] = new MatrixRow(0, elementHandler, s);//X
                    transformElements[i][1] = new MatrixRow(0, elementHandler, s);//Y
                    transformElements[i][2] = new MatrixRow(0, elementHandler, s);//Z
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
                    if (this[i].X == null) this[i].X = new MatrixRow(0, handler);
                    this[i].X.UnParse(s);
                    if (this[i].Y == null) this[i].Y = new MatrixRow(0, handler);
                    this[i].Y.UnParse(s);
                    if (this[i].Z == null) this[i].Z = new MatrixRow(0, handler);
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
