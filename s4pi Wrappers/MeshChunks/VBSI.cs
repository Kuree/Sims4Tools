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
using System.IO;
using s4pi.GenericRCOLResource;
using s4pi.Interfaces;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using s4pi.Settings;
namespace meshExpImp.ModelBlocks
{
    public enum SwizzleCmd : uint
    {
        None = 0x0,
        Swizzle32 = 0x1,
        Swizzle16x2 = 0x2
    }

    public class VBSI : ARCOLBlock
    {
        public class SwizzleEntry : AHandlerElement, IEquatable<SwizzleEntry>
        {
            public SwizzleEntry(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public SwizzleEntry(int apiVersion, EventHandler handler, SwizzleCmd command) : base(apiVersion, handler) { mCommand = command; }
            public SwizzleEntry(int apiVersion, EventHandler handler, SwizzleEntry basis) : this(apiVersion, handler, basis.Command) { }
            public SwizzleEntry(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
            [ElementPriority(1)]
            public SwizzleCmd Command
            {
                get { return mCommand; }
                set { if (mCommand != value) { if(mCommand!=value){mCommand = value; OnElementChanged();} } }
            }

            private SwizzleCmd mCommand;
            private void Parse(Stream s)
            {
                BinaryReader br = new BinaryReader(s);
                mCommand = (SwizzleCmd)br.ReadUInt32();
            }
            public void UnParse(Stream s)
            {
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write((UInt32)mCommand);
            }
            public override List<string> ContentFields
            {
                get { return GetContentFields(requestedApiVersion, GetType()); }
            }

            public override int RecommendedApiVersion
            {
                get { return kRecommendedApiVersion; }
            }

            public bool Equals(SwizzleEntry other)
            {
                return mCommand.Equals(other.mCommand);
            }
            public override bool Equals(object obj)
            {
                return obj as SwizzleEntry != null ? this.Equals(obj as SwizzleEntry) : false;
            }
            public override int GetHashCode()
            {
                return mCommand.GetHashCode();
            }
            public override string ToString()
            {
                return mCommand.ToString();
            }
            public string Value { get { return ToString(); } }
        }
        public class SwizzleList : DependentList<SwizzleEntry>
        {
            public SwizzleList(EventHandler handler)
                : base(handler)
            {
            }

            public SwizzleList(EventHandler handler, Stream s, int count)
                : base(handler)
            {
                Parse(s, count);
            }

            public SwizzleList(EventHandler handler, IEnumerable<SwizzleEntry> ilt) : base(handler, ilt) { }

            private void Parse(Stream s, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    ((IList<SwizzleEntry>)this).Add(CreateElement(s));
                }
            }
            protected override void WriteCount(Stream s, int count) { }
            protected override SwizzleEntry CreateElement(Stream s)
            {
                return new SwizzleEntry(0, handler, s);
            }

            protected override void WriteElement(Stream s, SwizzleEntry element)
            {
                element.UnParse(s);
            }
        }
        public class SegmentList : DependentList<SegmentInfo>
        {
            public SegmentList(EventHandler handler) : base(handler) { }
            public SegmentList(EventHandler handler, Stream s) : base(handler, s) { }
            public SegmentList(EventHandler handler, IEnumerable<SegmentInfo> ilt) : base(handler, ilt) { }
            protected override SegmentInfo CreateElement(Stream s)
            {
                return new SegmentInfo(0, handler, s);
            }
            protected override void WriteElement(Stream s, SegmentInfo element)
            {
                element.UnParse(s);
            }
        }
        public class SegmentInfo : AHandlerElement, IEquatable<SegmentInfo>
        {
            private Int32 mVertexSize;
            private Int32 mVertexCount;
            private UInt32 mByteOffset;
            private SwizzleList mSwizzles;

            public SegmentInfo(int apiVersion, EventHandler handler) : this(apiVersion, handler, 0, 0, 0, new SwizzleList(handler)) { }
            public SegmentInfo(int apiVersion, EventHandler handler, SegmentInfo basis) : this(apiVersion, handler, basis.VertexSize, basis.VertexCount, basis.ByteOffset, basis.Swizzles) { }
            public SegmentInfo(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
            public SegmentInfo(int apiVersion, EventHandler handler, Int32 vertexSize, Int32 vertexCount, UInt32 byteOffset, SwizzleList swizzles)
                : base(apiVersion, handler)
            {
                mVertexSize = vertexSize;
                mVertexCount = vertexCount;
                mByteOffset = byteOffset;
                mSwizzles = swizzles == null ? null : new SwizzleList(handler, swizzles);
            }
            public static SegmentInfo FromMesh(MLOD.Mesh mesh,VRTF vrtf)
            {
                var segment = new SegmentInfo(0, null);
                segment.VertexSize = vrtf.Stride;
                segment.VertexCount = mesh.VertexCount;
                segment.ByteOffset = mesh.StreamOffset;
                
                foreach(var layout in vrtf.Layouts)
                {
                    switch(layout.Format)
                    {
                        case VRTF.ElementFormat.Float1:
                        case VRTF.ElementFormat.UByte4:
                        case VRTF.ElementFormat.UByte4N:
                        case VRTF.ElementFormat.ColorUByte4:
                        case VRTF.ElementFormat.Dec3N:
                        case VRTF.ElementFormat.UDec3N:
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            break;
                        case VRTF.ElementFormat.Float2:
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            break;
                        case VRTF.ElementFormat.Float3:
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            break;
                        case VRTF.ElementFormat.Float4:
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle32));
                            break;
                        case VRTF.ElementFormat.Short2:
                        case VRTF.ElementFormat.Short2N:
                        case VRTF.ElementFormat.UShort2N:
                        case VRTF.ElementFormat.Float16_2:
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle16x2));
                            break;
                        case VRTF.ElementFormat.Short4:
                        case VRTF.ElementFormat.Short4N:
                        case VRTF.ElementFormat.UShort4N:
                        case VRTF.ElementFormat.Float16_4:
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle16x2));
                            segment.Swizzles.Add(new SwizzleEntry(0, null, SwizzleCmd.Swizzle16x2));
                            break;
                    }
                }
                return segment;
            }
            [ElementPriority(1)]
            public Int32 VertexSize
            {
                get { return mVertexSize; }
                set { if (mVertexSize != value) { mVertexSize = value; OnElementChanged(); } }
            }
            [ElementPriority(2)]
            public Int32 VertexCount
            {
                get { return mVertexCount; }
                set { if (mVertexCount != value) { mVertexCount = value; OnElementChanged(); } }
            }
            [ElementPriority(3)]
            public UInt32 ByteOffset
            {
                get { return mByteOffset; }
                set { if (mByteOffset != value) { mByteOffset = value; OnElementChanged(); } }
            }
            [ElementPriority(4)]
            public SwizzleList Swizzles
            {
                get { return mSwizzles; }
                set { if (mSwizzles != value) { mSwizzles = value == null ? null : new SwizzleList(handler, value); OnElementChanged(); } }
            }

            public string Value { get { return ValueBuilder; } }
            private void Parse(Stream s)
            {
                BinaryReader br = new BinaryReader(s);
                mVertexSize = br.ReadInt32();
                mVertexCount = br.ReadInt32();
                mByteOffset = br.ReadUInt32();
                mSwizzles = new SwizzleList(handler, s, (int)mVertexSize / 4);
            }
            public void UnParse(Stream s)
            {
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write(mVertexSize);
                bw.Write(mVertexCount);
                bw.Write(mByteOffset);
                if (mSwizzles == null) mSwizzles = new SwizzleList(handler);
                mSwizzles.UnParse(s);

            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(requestedApiVersion, GetType()); }
            }

            public override int RecommendedApiVersion
            {
                get { return kRecommendedApiVersion; }
            }

            public bool Equals(SegmentInfo other)
            {
                return
                    mVertexSize.Equals(other.mVertexSize)
                    && mVertexCount.Equals(other.mVertexCount)
                    && mByteOffset.Equals(other.mByteOffset)
                    && mSwizzles.Equals(other.mSwizzles)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as SegmentInfo != null ? this.Equals(obj as SegmentInfo) : false;
            }
            public override int GetHashCode()
            {
                return
                    mVertexSize.GetHashCode()
                    ^ mVertexCount.GetHashCode()
                    ^ mByteOffset.GetHashCode()
                    ^ mSwizzles.GetHashCode()
                    ;
            }
        }
        public VBSI(int apiVersion, EventHandler handler) : base(apiVersion, handler, null) { }
        public VBSI(int apiVersion, EventHandler handler, VBSI basis) : this(apiVersion, handler, new SegmentList(handler, basis.Segments)) { }
        public VBSI(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler, s) { }
        public VBSI(int apiVersion, EventHandler handler, SegmentList segments)
            : base(apiVersion, handler, null)
        {
            mSegments = segments;
        }

        [ElementPriority(1)]
        public SegmentList Segments
        {
            get { return mSegments; }
            set { if (mSegments != value) { mSegments = value; OnRCOLChanged(this, EventArgs.Empty); } }
        }

        public string Value { get { return ValueBuilder; } }
        public static VBSI FromMLOD(MLOD mlod, GenericRCOLResource container)
        {
            var vbsi = new VBSI(0, null);
            foreach (var mesh in mlod.Meshes)
            {
                var vrtf = (VRTF)GenericRCOLResource.ChunkReference.GetBlock(container, mesh.VertexFormatIndex);
                vbsi.Segments.Add(SegmentInfo.FromMesh(mesh,vrtf));
            }
            return vbsi;
        }

        private SegmentList mSegments;
        protected override void Parse(Stream s)
        {
            mSegments = new SegmentList(handler, s);
        }
        public override Stream UnParse()
        {
            if (mSegments == null) mSegments = new SegmentList(handler);
            MemoryStream s = new MemoryStream();
            mSegments.UnParse(s);
            return s;
        }

        public override string Tag
        {
            get { return "VBSI"; }
        }

        public override uint ResourceType
        {
            get { return 0xFFFFFFFF; }
        }

        private static bool checking = Settings.Checking;
        private const int kRecommendedApiVersion = 1;
    }
}