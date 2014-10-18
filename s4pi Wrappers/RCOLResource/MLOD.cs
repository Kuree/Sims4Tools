/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
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
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RCOLResource
{
    public class MLOD : RCOLChunk
    {
        [ElementPriority(0)]
        public static RCOL.RCOLChunkType RCOLType { get { return RCOL.RCOLChunkType.MLOD; } }

        public MLOD(int APIversion, EventHandler handler, Stream s, TGIBlock currentTGI) : base(APIversion, handler, s, currentTGI) { }

        #region Attributes
        public uint version { get; set; }
        public Mesh[] meshes { get; set; }

        static bool checking = s4pi.Settings.Settings.Checking;
        const int recommendedApiVersion = 1;
        #endregion

        #region Data I/O
        protected override void Parse(System.IO.Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            uint tag = r.ReadUInt32();
            if (tag != (uint)RCOLType) throw new InvalidDataException(string.Format("Except to read 0x{0:8X}, read 0x{1:8X}", RCOLType, tag));
            this.version = r.ReadUInt32();
            uint count = r.ReadUInt32();
            this.meshes = new Mesh[count];
            for (uint i = 0; i < count; i++) this.meshes[i] = new Mesh(recommendedApiVersion, handler, s);
        }

        protected internal override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write((uint)RCOLType);
            w.Write(this.version);
            w.Write(this.meshes.Length);
            foreach (var mesh in this.meshes) mesh.UnParse(s);
        }
        #endregion

        #region Sub Types
        public class Mesh : AHandlerElement
        {
            public Mesh(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

            const int recommendedApiVersion = 1;
            public uint name { get; set; }
            public uint materialIndex { get; set; }
            public uint vertexFormatIndex { get; set; }
            public uint vertexBufferIndex { get; set; }
            public uint indexBufferIndex { get; set; }
            public ModelPrimitiveType primitiveType { get; set; }
            public MeshFlags flags { get; set; }
            public UInt32 streamOffset { get; set; }
            public Int32 startVertex { get; set; }
            public Int32 startIndex { get; set; }
            public Int32 minVertexIndex { get; set; }
            public Int32 vertexCount { get; set; }
            public Int32 primitiveCount { get; set; }
            public uint skinControllerIndex { get; set; }
            public uint scaleOffsetIndex { get; set; }
            public UIntList jointReferences { get; set; }
            public BoundingBox bounds { get; set; }
            public GeometryState[] geometryStates { get; set; }
            public UInt32 parentName { get; set; }
            public Vector4 mirrorPlane { get; set; }
            public UInt32 unknown1 { get; set; }

            #region Data I/O
            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                long expectedSize = r.ReadUInt32();
                long start = s.Position;
                this.name = r.ReadUInt32();
                this.materialIndex = r.ReadUInt32();
                this.vertexFormatIndex = r.ReadUInt32();
                this.vertexBufferIndex = r.ReadUInt32();
                this.indexBufferIndex = r.ReadUInt32();
                uint flag = r.ReadUInt32();
                this.primitiveType = (ModelPrimitiveType)(flag & 0x000000FF);
                this.flags = (MeshFlags)(flag >> 8);
                this.streamOffset = r.ReadUInt32();
                this.startVertex = r.ReadInt32();
                this.startIndex = r.ReadInt32();
                this.minVertexIndex = r.ReadInt32();
                this.vertexCount = r.ReadInt32();
                this.primitiveCount = r.ReadInt32();
                this.bounds = new BoundingBox(recommendedApiVersion, handler, s);
                this.skinControllerIndex = r.ReadUInt32();
                this.jointReferences = new UIntList(handler, s);
                this.scaleOffsetIndex = r.ReadUInt32();
                uint count = r.ReadUInt32();
                this.geometryStates = new GeometryState[count];
                for (uint i = 0; i < count; i++) this.geometryStates[i] = new GeometryState(recommendedApiVersion, handler, s);
                this.parentName = r.ReadUInt32();
                this.mirrorPlane = new Vector4(recommendedApiVersion, handler, s);
                this.unknown1 = r.ReadUInt32();
                long actualSize = s.Position - start;
                if (checking && actualSize != expectedSize) throw new Exception(String.Format("Expected end at {0}, actual end was {1}", expectedSize, actualSize));
            }
            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                long offsetPos = s.Position;
                w.Write(0);
                long startPos = s.Position;
                w.Write(this.name);
                w.Write(this.materialIndex);
                w.Write(this.vertexFormatIndex);
                w.Write(this.vertexBufferIndex);
                w.Write(this.indexBufferIndex);
                uint flag = ((uint)this.primitiveType) & ((uint)this.flags << 8);
                w.Write(flag);
                w.Write(this.streamOffset);
                w.Write(this.startVertex);
                w.Write(this.startIndex);
                w.Write(this.minVertexIndex);
                w.Write(this.vertexCount);
                w.Write(this.primitiveCount);
                this.bounds.UnParse(s);
                w.Write(this.skinControllerIndex);
                this.jointReferences.UnParse(s);
                w.Write(this.scaleOffsetIndex);
                w.Write(this.geometryStates.Length);
                foreach (var state in this.geometryStates) state.UnParse(s);
                w.Write(this.parentName);
                this.mirrorPlane.UnParse(s);
                w.Write(this.unknown1);
                uint size = (uint)(s.Position - startPos);
                long endPos = s.Position;
                s.Position = offsetPos;
                w.Write(size);
                s.Position = endPos;
            }
            #endregion

            #region Sub-types
            public class GeometryState : AHandlerElement, IEquatable<GeometryState>
            {
                private const int kRecommendedApiVersion = 1;

                private UInt32 mName;
                private Int32 mStartIndex;
                private Int32 mMinVertexIndex;
                private Int32 mVertexCount;
                private Int32 mPrimitiveCount;

                public GeometryState(int APIversion, EventHandler handler) : base(APIversion, handler) { }
                public GeometryState(int APIversion, EventHandler handler, GeometryState basis) : this(APIversion, handler, basis.Name, basis.StartIndex, basis.MinVertexIndex, basis.VertexCount, basis.PrimitiveCount) { }
                public GeometryState(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
                public GeometryState(int APIversion, EventHandler handler, uint name, int startIndex, int minVertexIndex, int vertexCount, int primitiveCount)
                    : base(APIversion, handler)
                {
                    mName = name;
                    mStartIndex = startIndex;
                    mMinVertexIndex = minVertexIndex;
                    mVertexCount = vertexCount;
                    mPrimitiveCount = primitiveCount;
                }

                [ElementPriority(1)]
                public UInt32 Name
                {
                    get { return mName; }
                    set { if (mName != value) { mName = value; OnElementChanged(); } }
                }
                [ElementPriority(2)]
                public Int32 StartIndex
                {
                    get { return mStartIndex; }
                    set { if (mStartIndex != value) { mStartIndex = value; OnElementChanged(); } }
                }
                [ElementPriority(3)]
                public Int32 MinVertexIndex
                {
                    get { return mMinVertexIndex; }
                    set { if (mMinVertexIndex != value) { mMinVertexIndex = value; OnElementChanged(); } }
                }
                [ElementPriority(4)]
                public Int32 VertexCount
                {
                    get { return mVertexCount; }
                    set { if (mVertexCount != value) { mVertexCount = value; OnElementChanged(); } }
                }
                [ElementPriority(5)]
                public Int32 PrimitiveCount
                {
                    get { return mPrimitiveCount; }
                    set { if (mPrimitiveCount != value) { mPrimitiveCount = value; OnElementChanged(); } }
                }

                private void Parse(Stream s)
                {
                    BinaryReader br = new BinaryReader(s);
                    mName = br.ReadUInt32();
                    mStartIndex = br.ReadInt32();
                    mMinVertexIndex = br.ReadInt32();
                    mVertexCount = br.ReadInt32();
                    mPrimitiveCount = br.ReadInt32();
                }
                public void UnParse(Stream s)
                {
                    BinaryWriter bw = new BinaryWriter(s);
                    bw.Write(mName);
                    bw.Write(mStartIndex);
                    bw.Write(mMinVertexIndex);
                    bw.Write(mVertexCount);
                    bw.Write(mPrimitiveCount);
                }

                public override List<string> ContentFields
                {
                    get { return GetContentFields(base.requestedApiVersion, GetType()); }
                }

                public override int RecommendedApiVersion
                {
                    get { return kRecommendedApiVersion; }
                }

                public bool Equals(GeometryState other)
                {
                    return
                        mName.Equals(other.mName)
                        && mStartIndex.Equals(other.mStartIndex)
                        && mMinVertexIndex.Equals(other.mMinVertexIndex)
                        && mVertexCount.Equals(other.mVertexCount)
                        && mPrimitiveCount.Equals(other.mPrimitiveCount)
                        ;
                }
                public override bool Equals(object obj)
                {
                    return obj as GeometryState != null ? this.Equals(obj as GeometryState) : false;
                }
                public override int GetHashCode()
                {
                    return
                        mName.GetHashCode()
                        ^ mStartIndex.GetHashCode()
                        ^ mMinVertexIndex.GetHashCode()
                        ^ mVertexCount.GetHashCode()
                        ^ mPrimitiveCount.GetHashCode()
                        ;
                }

                public string Value { get { return ValueBuilder; } }
            }
            #endregion


            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region Content Fields

            public string Value { get { return ValueBuilder; } }
            #endregion

        }

        [Flags]
        public enum MeshFlags : uint
        {
            BasinInterior = 0x00000001,
            HDExteriorLit = 0x00000002,
            PortalSide = 0x00000004,
            DropShadow = 0x00000008,
            ShadowCaster = 0x00000010,
            Foundation = 0x00000020,
            Pickable = 0x00000040

        }
        public enum ModelPrimitiveType : uint
        {
            PointList = 1,
            LineList = 2,
            LineStrip = 3,
            TriangleList = 4,
            TriangleFan = 5,
            TriangleStrip = 6,
            RectList = 6,
            QuadList = 7,
            DisplayList = 8
        }
        #endregion

        public string Value { get { return ValueBuilder; } }
    }
}
