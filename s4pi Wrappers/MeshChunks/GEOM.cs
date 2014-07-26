using System.IO;
using s4pi.Interfaces;
using s4pi.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using s4pi.GenericRCOLResource;
using System.Linq;
using System.Collections;

namespace meshExpImp.ModelBlocks
{
    public class GEOM : ARCOLBlock
    {
        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint tag = (uint)FOURCC("GEOM");
        uint version = 0x0000000C;
        ShaderType shader;
        MTNF mtnf = null;
        uint mergeGroup;
        uint sortOrder;
        VertexFormatList vertexFormats;
        VertexDataList vertexData;
        FaceList faces;
        int skinIndex;
        UnknownThingList unknownThings;
        UnknownThing2List unknownThings2;
        UIntList boneHashes;

        TGIBlockList tgiBlockList;
        #endregion

        #region Constructors
        public GEOM(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public GEOM(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public GEOM(int APIversion, EventHandler handler, GEOM basis)
            : this(APIversion, handler,
            basis.version, basis.shader, basis.mtnf, basis.mergeGroup, basis.sortOrder,
            basis.vertexFormats, basis.vertexData,
            basis.faces, basis.skinIndex, basis.unknownThings, basis.unknownThings2, basis.boneHashes,
            basis.tgiBlockList) { }
        public GEOM(int APIversion, EventHandler handler,
            uint version, ShaderType shader, MTNF mtnf, uint mergeGroup, uint sortOrder,
            IEnumerable<VertexFormat> vertexFormats, IEnumerable<VertexDataElement> vertexData,
            IEnumerable<Face> facePoints, int skinIndex, 
            UnknownThingList unknownThings, UnknownThing2List unknownThings2, IEnumerable<uint> boneHashes,
            IEnumerable<TGIBlock> tgiBlockList)
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.shader = shader;
            if (shader != 0 && mtnf == null)
                throw new ArgumentException("Must supply MTNF when applying a Shader.");
            this.mtnf = shader == 0 ? null : new MTNF(requestedApiVersion, handler, mtnf) { RCOLTag = "GEOM", };
            this.mergeGroup = mergeGroup;
            this.sortOrder = sortOrder;
            this.vertexFormats = vertexFormats == null ? null : new VertexFormatList(handler, version, vertexFormats);
            this.vertexData = vertexData == null ? null : new VertexDataList(handler, version, vertexData, this.vertexFormats);
            this.faces = facePoints == null ? null : new FaceList(handler, facePoints);
            this.skinIndex = skinIndex;
            this.unknownThings = unknownThings == null ? null : new UnknownThingList(handler, unknownThings);
            this.unknownThings2 = unknownThings2 == null ? null : new UnknownThing2List(handler, unknownThings2);
            this.boneHashes = boneHashes == null ? null : new UIntList(handler, boneHashes);
            this.tgiBlockList = tgiBlockList == null ? null : new TGIBlockList(handler, tgiBlockList);

            if (mtnf != null)
                mtnf.ParentTGIBlocks = this.tgiBlockList;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return "GEOM"; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x015A1849; } }

        // public override AHandlerElement Clone(EventHandler handler) { return new GEOM(requestedApiVersion, handler, this); }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                if (shader == 0)
                    res.Remove("Mtnf");
                if (version == 0x00000005)
                {
                    res.Remove("UnknownThings");
                    res.Remove("UnknownThings2");
                }
                else if (version == 0x0000000C)
                {
                    res.Remove("SkinIndex");
                }
                return res;
            }
        }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            tag = r.ReadUInt32();
            if (checking) if (tag != (uint)FOURCC("GEOM"))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: 'GEOM'; at 0x{1:X8}", FOURCC(tag), s.Position));
            version = r.ReadUInt32();
            if (checking) if (version != 0x00000005 && version != 0x0000000C)
                    throw new InvalidDataException(String.Format("Invalid Version read: '{0}'; expected: '0x00000005' or '0x0000000C'; at 0x{1:X8}", version, s.Position));

            long tgiPosn = r.ReadUInt32() + s.Position;
            long tgiSize = r.ReadUInt32();

            shader = (ShaderType)r.ReadUInt32();
            if (shader != 0)
            {
                uint size = r.ReadUInt32();
                long posn = s.Position;
                mtnf = new MTNF(requestedApiVersion, handler, s, "GEOM");
                if (checking) if (s.Position != posn + size)
                    throw new InvalidDataException(String.Format("MTNF chunk size invalid; expected 0x{0:X8} bytes, read 0x{1:X8} bytes; at 0x{2:X8}",
                        size, s.Position - posn, s.Position));
            }
            else mtnf = null;

            mergeGroup = r.ReadUInt32();
            sortOrder = r.ReadUInt32();

            int numVertices = r.ReadInt32();//now write that down...
            vertexFormats = new VertexFormatList(handler, version, s);
            vertexData = new VertexDataList(handler, version, s, numVertices, vertexFormats);//...as you'll be needing it

            int numFacePointSizes = r.ReadInt32();
            if (checking) if (numFacePointSizes != 1)
                    throw new InvalidDataException(String.Format("Expected number of face point sizes to be 1, read {0}, at 0x{1:X8}", numFacePointSizes, s.Position));

            byte facePointSize = r.ReadByte();
            if (checking) if (facePointSize != 2)
                    throw new InvalidDataException(String.Format("Expected face point size to be 2, read {0}, at 0x{1:X8}", facePointSize, s.Position));

            faces = new FaceList(handler, s);
            if (version == 0x00000005)
            {
                skinIndex = r.ReadInt32();
            }
            else if (version == 0x0000000C)
            {
                unknownThings = new UnknownThingList(handler, s);
                unknownThings2 = new UnknownThing2List(handler, s);
            }
            boneHashes = new UIntList(handler, s);

            tgiBlockList = new TGIBlockList(OnRCOLChanged, s, tgiPosn, tgiSize);
            if (mtnf != null)
                mtnf.ParentTGIBlocks = tgiBlockList;
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(tag);
            w.Write(version);

            long pos = ms.Position;
            w.Write((uint)0); // tgiOffset
            w.Write((uint)0); // tgiSize

            w.Write((uint)shader);
            if (shader != 0)
            {
                if (mtnf == null) mtnf = new MTNF(requestedApiVersion, handler, "GEOM") { };
                byte[] mtnfData = mtnf.AsBytes;
                w.Write(mtnfData.Length);
                w.Write(mtnfData);
            }

            w.Write(mergeGroup);
            w.Write(sortOrder);

            if (vertexData == null) w.Write(0);
            else w.Write(vertexData.Count);
            if (vertexFormats == null) vertexFormats = new VertexFormatList(handler, version);
            vertexFormats.UnParse(ms);
            if (vertexData == null) vertexData = new VertexDataList(handler, version, vertexFormats);
            vertexData.UnParse(ms);
            w.Write((int)1);
            w.Write((byte)2);
            if (faces == null) faces = new FaceList(handler);
            faces.UnParse(ms);
            if (version == 0x00000005)
            {
                w.Write(skinIndex);
            }
            else if (version == 0x0000000C)
            {
                if (unknownThings == null) unknownThings = new UnknownThingList(handler);
                unknownThings.UnParse(ms);
                if (unknownThings2 == null) unknownThings2 = new UnknownThing2List(handler);
                unknownThings2.UnParse(ms);
            }
            if (boneHashes == null) boneHashes = new UIntList(handler);
            boneHashes.UnParse(ms);

            if (tgiBlockList == null)
            {
                tgiBlockList = new TGIBlockList(OnRCOLChanged);
                if (mtnf != null)
                    mtnf.ParentTGIBlocks = tgiBlockList;
            }
            tgiBlockList.UnParse(ms, pos);

            return ms;
        }

        private byte ReadByte(Stream s) { return new BinaryReader(s).ReadByte(); }
        private void WriteByte(Stream s, byte element) { new BinaryWriter(s).Write(element); }
        #endregion

        #region Sub-Types

        #region VertexFormat
        public enum UsageType : uint
        {
            Position = 0x01,
            Normal = 0x02,
            UV = 0x03,
            BoneAssignment = 0x04,
            Weights = 0x05,
            TangentNormal = 0x06,
            Color = 0x07,
            VertexID = 0x0A,
        }
        static uint[] expectedDataType05 = new uint[] {
            /*Unknown*/ 0,
            /*Position*/ 1,
            /*Normal*/ 1,
            /*UV*/ 1,
            /*BoneAssignment*/ 2,
            /*Weights*/ 1, 
            /*TangentNormal*/ 1,
            /*Color*/ 3,
            /*Unknown*/ 0,
            /*Unknown*/ 0,
            /*VertexID*/ 4,
            /**/
        };
        static byte[] expectedElementSize05 = new byte[] {
            /*Unknown*/ 0,
            /*Position*/ 12,
            /*Normal*/ 12,
            /*UV*/ 8,
            /*BoneAssignment*/ 4,
            /*Weights*/ 16, 
            /*TangentNormal*/ 12,
            /*Color*/ 4,
            /*Unknown*/ 0,
            /*Unknown*/ 0,
            /*VertexID*/ 4,
            /**/
        };
        static uint[] expectedDataType0C = new uint[] {
            /*Unknown*/ 0,
            /*Position*/ 1,
            /*Normal*/ 1,
            /*UV*/ 1,
            /*BoneAssignment*/ 2,
            /*Unknown*/ 2, 
            /*TangentNormal*/ 1,
            /*Color*/ 3,
            /*Unknown*/ 0,
            /*Unknown*/ 0,
            /*VertexID*/ 4,
            /**/
        };
        static byte[] expectedElementSize0C = new byte[] {
            /*Unknown*/ 0,
            /*Position*/ 12,
            /*Normal*/ 12,
            /*UV*/ 8,
            /*BoneAssignment*/ 4,
            /*Unknown*/ 4, 
            /*TangentNormal*/ 12,
            /*Color*/ 4,
            /*Unknown*/ 0,
            /*Unknown*/ 0,
            /*VertexID*/ 4,
            /**/
        };
        public class VertexFormat : AHandlerElement, IEquatable<VertexFormat>
        {
            const int recommendedApiVersion = 1;

            uint version;
            UsageType usage;

            public VertexFormat(int APIversion, EventHandler handler, uint version) : base(APIversion, handler) { this.version = version; }
            public VertexFormat(int APIversion, EventHandler handler, uint version, Stream s) : base(APIversion, handler) { this.version = version; Parse(s); }
            public VertexFormat(int APIversion, EventHandler handler, VertexFormat basis)
                : this(APIversion, handler, basis.version, basis.usage) { }
            public VertexFormat(int APIversion, EventHandler handler, uint version, UsageType usage)
                : base(APIversion, handler)
            {
                this.version = version;
                this.usage = usage;
            }

            private void Parse(Stream s)
            {
                uint[] expectedDataType;
                byte[] expectedElementSize;

                switch (version)
                {
                    case 0x00000005:
                        expectedDataType = expectedDataType0C;
                        expectedElementSize = expectedElementSize0C;
                        break;
                    case 0x0000000C:
                        expectedDataType = expectedDataType0C;
                        expectedElementSize = expectedElementSize0C;
                        break;
                    default:
                        expectedDataType = null;
                        expectedElementSize = null;
                        break;
                }

                BinaryReader r = new BinaryReader(s);
                usage = (UsageType)r.ReadUInt32();
                if (checking) if (usage == 0 || (uint)usage >= expectedDataType.Length)
                    throw new InvalidDataException(string.Format("Unexpected usage code 0x{0:X8} at 0x{1:X8}", (uint)usage, s.Position));

                uint dataType = r.ReadUInt32();
                if (checking) if (dataType != expectedDataType[(uint)usage])
                    throw new InvalidDataException(string.Format("Expected data type 0x{0:X8}, read 0x{1:X8}, at 0x{2:X8}", expectedDataType[(uint)usage], dataType, s.Position));

                byte elementSize = r.ReadByte();
                if (checking) if (elementSize != expectedElementSize[(uint)usage])
                    throw new InvalidDataException(String.Format("Expected element size 0x{0:X2}, read 0x{1:X2}, at {2:X8}", expectedElementSize[(uint)usage], elementSize, s.Position));
            }

            internal void UnParse(Stream s)
            {
                uint[] expectedDataType;
                byte[] expectedElementSize;

                switch (version)
                {
                    case 0x00000005:
                        expectedDataType = expectedDataType0C;
                        expectedElementSize = expectedElementSize0C;
                        break;
                    case 0x0000000C:
                        expectedDataType = expectedDataType0C;
                        expectedElementSize = expectedElementSize0C;
                        break;
                    default:
                        expectedDataType = null;
                        expectedElementSize = null;
                        break;
                }

                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)usage);
                w.Write(expectedDataType[(uint)usage]);
                w.Write(expectedElementSize[(uint)usage]);
            }

            #region AHandlerElement
            // public override AHandlerElement Clone(EventHandler handler) { return new VertexFormat(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<VertexFormat>
            public bool Equals(VertexFormat other)
            {
                return this.usage.Equals(other.usage);
            }

            public override bool Equals(object obj) { return obj is VertexFormat && Equals(obj as VertexFormat); }

            public override int GetHashCode() { return usage.GetHashCode(); }
            #endregion

            [ElementPriority(1)]
            public UsageType Usage { get { return usage; } set { if (!usage.Equals(value)) { usage = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
        }
        public class VertexFormatList : DependentList<VertexFormat>
        {
            uint version;

            #region Constructors
            public VertexFormatList(EventHandler handler, uint version) : base(handler) { this.version = version; }
            public VertexFormatList(EventHandler handler, uint version, Stream s) : base(null) { this.version = version; elementHandler = handler; Parse(s); this.handler = handler; }
            public VertexFormatList(EventHandler handler, uint version, IEnumerable<VertexFormat> le) 
                : base(null) 
            {
                elementHandler = handler; 
                this.version = version; 
                foreach (VertexFormat ve in le) 
                    this.Add(new VertexFormat(0, elementHandler, version, ve.Usage)); 
                this.handler = handler; 
            }
            #endregion

            protected override VertexFormat CreateElement(Stream s) { return new VertexFormat(0, elementHandler, this.version, s); }
            protected override void WriteElement(Stream s, VertexFormat element) { element.UnParse(s); }

            //public override void Add() { this.Add(new VertexFormat(0, elementHandler)); }
        }
        #endregion

        #region VertexElement
        public abstract class VertexElement : AHandlerElement, IEquatable<VertexElement>
        {
            const int recommendedApiVersion = 1;

            protected VertexElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            protected VertexElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

            protected abstract void Parse(Stream s);
            internal abstract void UnParse(Stream s);

            #region AHandlerElement
            //public abstract AHandlerElement Clone(EventHandler handler);
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public abstract bool Equals(VertexElement other);

            public virtual string Value { get { return string.Join("; ", ValueBuilder.Split('\n')); } }
        }
        public class PositionElement : VertexElement
        {
            protected float x, y, z;

            public PositionElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public PositionElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public PositionElement(int APIversion, EventHandler handler, PositionElement basis) : this(APIversion, handler, basis.x, basis.y, basis.z) { }
            public PositionElement(int APIversion, EventHandler handler, float x, float y, float z) : base(APIversion, handler) { this.x = x; this.y = y; this.z = z; }

            protected override void Parse(Stream s) { BinaryReader r = new BinaryReader(s); x = r.ReadSingle(); y = r.ReadSingle(); z = r.ReadSingle(); }
            internal override void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(x); w.Write(y); w.Write(z); }

            // public override AHandlerElement Clone(EventHandler handler) { return new PositionElement(requestedApiVersion, handler, this); }
            public override bool Equals(VertexElement other) { PositionElement o = other as PositionElement; return o != null && x.Equals(o.x) && y.Equals(o.y) && z.Equals(o.z); }
            public override bool Equals(object obj) { return obj is PositionElement && this.Equals(obj as PositionElement); }
            public override int GetHashCode() { return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode(); }

            [ElementPriority(1)]
            public float X { get { return x; } set { if (!x.Equals(value)) { x = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float Y { get { return y; } set { if (!y.Equals(value)) { y = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public float Z { get { return z; } set { if (!z.Equals(value)) { z = value; OnElementChanged(); } } }
        }
        public class NormalElement : PositionElement
        {
            public NormalElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public NormalElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public NormalElement(int APIversion, EventHandler handler, NormalElement basis) : this(APIversion, handler, basis.x, basis.y, basis.z) { }
            public NormalElement(int APIversion, EventHandler handler, float x, float y, float z) : base(APIversion, handler) { this.x = x; this.y = y; this.z = z; }

            // public override AHandlerElement Clone(EventHandler handler) { return new NormalElement(requestedApiVersion, handler, this); }
            public override bool Equals(VertexElement other) { NormalElement o = other as NormalElement; return o != null && x.Equals(o.x) && y.Equals(o.y) && z.Equals(o.z); }
            public override bool Equals(object obj) { return obj is NormalElement && this.Equals(obj as NormalElement); }
            public override int GetHashCode() { return base.GetHashCode(); }
        }
        public class UVElement : VertexElement
        {
            protected float u, v;

            public UVElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public UVElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public UVElement(int APIversion, EventHandler handler, UVElement basis) : this(APIversion, handler, basis.u, basis.v) { }
            public UVElement(int APIversion, EventHandler handler, float u, float v) : base(APIversion, handler) { this.u = u; this.v = v;  }

            protected override void Parse(Stream s) { BinaryReader r = new BinaryReader(s); u = r.ReadSingle(); v = r.ReadSingle(); }
            internal override void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(u); w.Write(v);  }

            // public override AHandlerElement Clone(EventHandler handler) { return new UVElement(requestedApiVersion, handler, this); }
            public override bool Equals(VertexElement other) { UVElement o = other as UVElement; return o != null && u.Equals(o.u) && v.Equals(o.v); }
            public override bool Equals(object obj) { return obj is UVElement && this.Equals(obj as UVElement); }
            public override int GetHashCode() { return u.GetHashCode() ^ v.GetHashCode(); }

            [ElementPriority(1)]
            public float U { get { return u; } set { if (!u.Equals(value)) { u = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float V { get { return v; } set { if (!v.Equals(value)) { v = value; OnElementChanged(); } } }
        }
        public class BoneAssignmentElement : VertexElement
        {
            protected uint id;

            public BoneAssignmentElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public BoneAssignmentElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public BoneAssignmentElement(int APIversion, EventHandler handler, BoneAssignmentElement basis) : this(APIversion, handler, basis.id) { }
            public BoneAssignmentElement(int APIversion, EventHandler handler, uint id) : base(APIversion, handler) { this.id = id; }

            protected override void Parse(Stream s) { BinaryReader r = new BinaryReader(s); id = r.ReadUInt32(); }
            internal override void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(id); }

            // public override AHandlerElement Clone(EventHandler handler) { return new BoneAssignmentElement(requestedApiVersion, handler, this); }
            public override bool Equals(VertexElement other) { BoneAssignmentElement o = other as BoneAssignmentElement; return o != null && id.Equals(o.id); }
            public override bool Equals(object obj) { return obj is BoneAssignmentElement && this.Equals(obj as BoneAssignmentElement); }
            public override int GetHashCode() { return id.GetHashCode(); }

            [ElementPriority(1)]
            public uint ID { get { return id; } set { if (!id.Equals(value)) { id = value; OnElementChanged(); } } }
        }
        public class WeightsElement : VertexElement
        {
            protected float w1, w2, w3, w4;

            public WeightsElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public WeightsElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public WeightsElement(int APIversion, EventHandler handler, WeightsElement basis) : this(APIversion, handler, basis.w1, basis.w2, basis.w3, basis.w4) { }
            public WeightsElement(int APIversion, EventHandler handler, float w1, float w2, float w3, float w4) : base(APIversion, handler) { this.w1 = w1; this.w2 = w2; this.w3 = w3; this.w4 = w4; }

            protected override void Parse(Stream s) { BinaryReader r = new BinaryReader(s); w1 = r.ReadSingle(); w2 = r.ReadSingle(); w3 = r.ReadSingle(); w4 = r.ReadSingle(); }
            internal override void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(w1); w.Write(w2); w.Write(w3); w.Write(w4); }

            // public override AHandlerElement Clone(EventHandler handler) { return new WeightsElement(requestedApiVersion, handler, this); }
            public override bool Equals(VertexElement other) { WeightsElement o = other as WeightsElement; return o != null && w1.Equals(o.w1) && w2.Equals(o.w2) && w3.Equals(o.w3) && w3.Equals(o.w4); }
            public override bool Equals(object obj) { return obj is WeightsElement && this.Equals(obj as WeightsElement); }
            public override int GetHashCode() { return w1.GetHashCode() ^ w2.GetHashCode() ^ w3.GetHashCode() ^ w4.GetHashCode(); }

            [ElementPriority(1)]
            public float W1 { get { return w1; } set { if (!w1.Equals(value)) { w1 = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float W2 { get { return w2; } set { if (!w2.Equals(value)) { w2 = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public float W3 { get { return w3; } set { if (!w3.Equals(value)) { w3 = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public float W4 { get { return w4; } set { if (!w4.Equals(value)) { w4 = value; OnElementChanged(); } } }
        }
        public class WeightBytesElement : VertexElement
        {
            protected byte w1, w2, w3, w4;

            public WeightBytesElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public WeightBytesElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public WeightBytesElement(int APIversion, EventHandler handler, WeightBytesElement basis) : this(APIversion, handler, basis.w1, basis.w2, basis.w3, basis.w4) { }
            public WeightBytesElement(int APIversion, EventHandler handler, byte w1, byte w2, byte w3, byte w4) : base(APIversion, handler) { this.w1 = w1; this.w2 = w2; this.w3 = w3; this.w4 = w4; }

            protected override void Parse(Stream s) { BinaryReader r = new BinaryReader(s); w1 = r.ReadByte(); w2 = r.ReadByte(); w3 = r.ReadByte(); w4 = r.ReadByte(); }
            internal override void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(w1); w.Write(w2); w.Write(w3); w.Write(w4); }

            // public override AHandlerElement Clone(EventHandler handler) { return new WeightBytesElement(requestedApiVersion, handler, this); }
            public override bool Equals(VertexElement other) { WeightBytesElement o = other as WeightBytesElement; return o != null && w1.Equals(o.w1) && w2.Equals(o.w2) && w3.Equals(o.w3) && w3.Equals(o.w4); }
            public override bool Equals(object obj) { return obj is WeightBytesElement && this.Equals(obj as WeightBytesElement); }
            public override int GetHashCode() { return w1.GetHashCode() ^ w2.GetHashCode() ^ w3.GetHashCode() ^ w4.GetHashCode(); }

            [ElementPriority(1)]
            public byte W1 { get { return w1; } set { if (!w1.Equals(value)) { w1 = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public byte W2 { get { return w2; } set { if (!w2.Equals(value)) { w2 = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public byte W3 { get { return w3; } set { if (!w3.Equals(value)) { w3 = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public byte W4 { get { return w4; } set { if (!w4.Equals(value)) { w4 = value; OnElementChanged(); } } }
        }
        public class TangentNormalElement : PositionElement
        {
            public TangentNormalElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public TangentNormalElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public TangentNormalElement(int APIversion, EventHandler handler, TangentNormalElement basis) : this(APIversion, handler, basis.x, basis.y, basis.z) { }
            public TangentNormalElement(int APIversion, EventHandler handler, float x, float y, float z) : base(APIversion, handler) { this.x = x; this.y = y; this.z = z; }

            // public override AHandlerElement Clone(EventHandler handler) { return new TangentNormalElement(requestedApiVersion, handler, this); }
            public override bool Equals(VertexElement other) { TangentNormalElement o = other as TangentNormalElement; return o != null && x.Equals(o.x) && y.Equals(o.y) && z.Equals(o.z); }
            public override bool Equals(object obj) { return obj is TangentNormalElement && this.Equals(obj as TangentNormalElement); }
            public override int GetHashCode() { return base.GetHashCode(); }
        }
        public class ColorElement : VertexElement
        {
            int argb;

            public ColorElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public ColorElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public ColorElement(int APIversion, EventHandler handler, ColorElement basis) : this(APIversion, handler, basis.argb) { }
            public ColorElement(int APIversion, EventHandler handler, int argb) : base(APIversion, handler) { this.argb = argb; }

            protected override void Parse(Stream s) { BinaryReader r = new BinaryReader(s); argb = r.ReadInt32(); }
            internal override void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(argb); }

            // public override AHandlerElement Clone(EventHandler handler) { return new ColorElement(requestedApiVersion, handler, this); }
            public override bool Equals(VertexElement other) { ColorElement o = other as ColorElement; return o != null && argb.Equals(o.argb); }
            public override bool Equals(object obj) { return obj is PositionElement && this.Equals(obj as PositionElement); }
            public override int GetHashCode() { return argb.GetHashCode(); }

            public override string Value { get { return Color.FromArgb(argb).ToString(); } }
        }
        public class VertexIDElement : BoneAssignmentElement
        {
            public VertexIDElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public VertexIDElement(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
            public VertexIDElement(int APIversion, EventHandler handler, VertexIDElement basis) : this(APIversion, handler, basis.id) { }
            public VertexIDElement(int APIversion, EventHandler handler, uint id) : base(APIversion, handler) { this.id = id; }

            // public override AHandlerElement Clone(EventHandler handler) { return new VertexIDElement(requestedApiVersion, handler, this); }
            public override bool Equals(VertexElement other) { VertexIDElement o = other as VertexIDElement; return o != null && id.Equals(o.id); }
            public override bool Equals(object obj) { return obj is VertexIDElement && this.Equals(obj as VertexIDElement); }
            public override int GetHashCode() { return base.GetHashCode(); }
        }
        public class ElementList : DependentList<VertexElement>
        {
            private uint version;

            public DependentList<VertexFormat> ParentVertexFormats { get; private set; }

            #region Constructors
            public ElementList(EventHandler handler, uint version) : base(handler) { }
            public ElementList(EventHandler handler, uint version, Stream s, DependentList<VertexFormat> parentVertexFormats)
                : base(null)
            {
                this.version = version;
                this.ParentVertexFormats = parentVertexFormats;
                elementHandler = handler;
                foreach (var fmt in parentVertexFormats)
                {
                    switch (fmt.Usage)
                    {
                        case UsageType.Position: this.Add(new PositionElement(0, handler, s)); break;
                        case UsageType.Normal: this.Add(new NormalElement(0, handler, s)); break;
                        case UsageType.UV: this.Add(new UVElement(0, handler, s)); break;
                        case UsageType.BoneAssignment: this.Add(new BoneAssignmentElement(0, handler, s)); break;
                        case UsageType.Weights:
                            switch (this.version)
                            {
                                case 0x00000005: this.Add(new WeightsElement(0, handler, s)); break;
                                case 0x0000000C: this.Add(new WeightBytesElement(0, handler, s)); break;
                            }
                            break;
                        case UsageType.TangentNormal: this.Add(new TangentNormalElement(0, handler, s)); break;
                        case UsageType.Color: this.Add(new ColorElement(0, handler, s)); break;
                        case UsageType.VertexID: this.Add(new VertexIDElement(0, handler, s)); break;
                    }
                }
                this.handler = handler;
            }
            public ElementList(EventHandler handler, uint version, IEnumerable<VertexElement> ilt, DependentList<VertexFormat> parentVertexFormats)
                : base(null)
            {
                this.version = version;
                this.ParentVertexFormats = parentVertexFormats;
                elementHandler = handler;
                foreach (var fmt in parentVertexFormats)
                {
                    switch (fmt.Usage)
                    {
                        case UsageType.Position: this.Add(ilt.FirstOrDefault(t => t is PositionElement) ?? new PositionElement(0, handler)); break;
                        case UsageType.Normal: this.Add(ilt.FirstOrDefault(t => t is NormalElement) ?? new NormalElement(0, handler)); break;
                        case UsageType.UV: this.Add(ilt.FirstOrDefault(t => t is UVElement) ?? new UVElement(0, handler)); break;
                        case UsageType.BoneAssignment: this.Add(ilt.FirstOrDefault(t => t is BoneAssignmentElement) ?? new BoneAssignmentElement(0, handler)); break;
                        case UsageType.Weights:
                            switch (this.version)
                            {
                                case 0x00000005: this.Add(ilt.FirstOrDefault(t => t is WeightsElement) ?? new WeightsElement(0, handler)); break;
                                case 0x0000000C: this.Add(ilt.FirstOrDefault(t => t is WeightBytesElement) ?? new WeightBytesElement(0, handler)); break;
                            }
                            break;
                        case UsageType.TangentNormal: this.Add(ilt.FirstOrDefault(t => t is TangentNormalElement) ?? new TangentNormalElement(0, handler)); break;
                        case UsageType.Color: this.Add(ilt.FirstOrDefault(t => t is ColorElement) ?? new ColorElement(0, handler)); break;
                        case UsageType.VertexID: this.Add(ilt.FirstOrDefault(t => t is VertexIDElement) ?? new VertexIDElement(0, handler)); break;
                    }
                }
                this.handler = handler;
            }
            #endregion

            protected override VertexElement CreateElement(Stream s) { throw new NotImplementedException(); }

            public override void UnParse(Stream s)
            {
                foreach (var fmt in ParentVertexFormats)
                {
                    VertexElement vtx = null;
                    switch (fmt.Usage)
                    {
                        case UsageType.Position: vtx = this.Find(e => e is PositionElement); break;
                        case UsageType.Normal: vtx = this.Find(e => e is NormalElement); break;
                        case UsageType.UV: vtx = this.Find(e => e is UVElement); break;
                        case UsageType.BoneAssignment: vtx = this.Find(e => e is BoneAssignmentElement); break;
                        case UsageType.Weights:
                            switch (this.version)
                            {
                                case 0x00000005: vtx = this.Find(e => e is WeightsElement); break;
                                case 0x0000000C: vtx = this.Find(e => e is WeightBytesElement); break;
                            }
                            break;
                        case UsageType.TangentNormal: vtx = this.Find(e => e is TangentNormalElement); break;
                        case UsageType.Color: vtx = this.Find(e => e is ColorElement); break;
                        case UsageType.VertexID: vtx = this.Find(e => e is VertexIDElement); break;
                    }
                    if (vtx == null)
                        throw new InvalidOperationException();
                    vtx.UnParse(s);
                }
            }

            protected override void WriteElement(Stream s, VertexElement element) { throw new NotImplementedException(); }

            public override void Add() { throw new NotImplementedException(); }

            public VertexElement this[UsageType usage]
            {
                get
                {
                    if (!ParentVertexFormats.Exists(x => x.Usage.Equals(usage)))
                        throw new IndexOutOfRangeException();
                    switch (usage)
                    {
                        case UsageType.Position: return this.Find(x => x is PositionElement);
                        case UsageType.Normal: return this.Find(x => x is NormalElement);
                        case UsageType.UV: return this.Find(x => x is UVElement);
                        case UsageType.BoneAssignment: return this.Find(x => x is BoneAssignmentElement);
                        case UsageType.Weights:
                            switch (this.version)
                            {
                                case 0x00000005: return this.Find(x => x is WeightsElement);
                                case 0x0000000C: return this.Find(x => x is WeightBytesElement);
                            }
                            break;
                        case UsageType.TangentNormal: return this.Find(x => x is TangentNormalElement);
                        case UsageType.Color: return this.Find(x => x is ColorElement);
                        case UsageType.VertexID: return this.Find(x => x is VertexIDElement);
                    }
                    throw new ArgumentException();
                }
                set
                {
                    VertexElement vtx = this[usage];
                    if (vtx != null && vtx.Equals(value)) return;

                    int index = this.IndexOf(vtx);
                    if (value.GetType().Equals(vtx.GetType()))
                        this[index] = vtx.Clone(handler) as VertexElement;
                    else
                        throw new ArgumentException();
                }
            }
        }
        public class VertexDataElement : AHandlerElement, IEquatable<VertexDataElement>
        {
            const int recommendedApiVersion = 1;

            private uint version;
            private ElementList elementList;

            public DependentList<VertexFormat> ParentVertexFormats { get; set; }
            public override List<string> ContentFields { get { var res = GetContentFields(requestedApiVersion, this.GetType()); res.Remove("ParentVertexFormats"); return res; } }

            public VertexDataElement(int APIversion, EventHandler handler, uint version, DependentList<VertexFormat> parentVertexFormats) : base(APIversion, handler) { this.version = version;  this.ParentVertexFormats = parentVertexFormats; }
            public VertexDataElement(int APIversion, EventHandler handler, uint version, Stream s, DependentList<VertexFormat> parentVertexFormats) : base(APIversion, handler) { this.version = version;  this.ParentVertexFormats = parentVertexFormats; Parse(s); }
            public VertexDataElement(int APIversion, EventHandler handler, uint version, VertexDataElement basis) : this(APIversion, handler, basis.version, basis.elementList, basis.ParentVertexFormats) { }
            public VertexDataElement(int APIversion, EventHandler handler, uint version, DependentList<VertexElement> elementList, DependentList<VertexFormat> parentVertexFormats)
                : base(APIversion, handler)
            {
                this.version = version;
                this.ParentVertexFormats = parentVertexFormats;//reference!
                this.elementList = new ElementList(handler, version, elementList, ParentVertexFormats);
            }

            private void Parse(Stream s) { elementList = new ElementList(handler, version, s, ParentVertexFormats); }
            internal void UnParse(Stream s) { elementList.UnParse(s); }

            #region AHandlerElement
            // public override AHandlerElement Clone(EventHandler handler) { return new VertexDataElement(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            #endregion

            public bool Equals(VertexDataElement other) { return elementList.Equals(other.elementList); }
            public override bool Equals(object obj) { return obj is VertexDataElement && this.Equals(obj as VertexDataElement); }
            public override int GetHashCode() { return elementList.GetHashCode(); }

            public ElementList Vertex
            {
                get { return elementList; }
                set { if (!elementList.Equals(value)) { elementList = new ElementList(handler, version, value, ParentVertexFormats); OnElementChanged(); } }
            }

            public string Value
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var fmt in ParentVertexFormats)
                    {
                        sb.AppendLine(fmt.Usage.ToString() + ": " + elementList[fmt.Usage].Value);
                    }
                    return sb.ToString();
                }
            }
        }

        public class VertexDataList : DependentList<VertexDataElement>
        {
            int origCount;
            uint version;
            DependentList<VertexFormat> parentVertexFormats;

            #region Constructors
            public VertexDataList(EventHandler handler, uint version, DependentList<VertexFormat> parentVertexFormats) : base(handler) { this.version = version; this.parentVertexFormats = parentVertexFormats; }
            public VertexDataList(EventHandler handler, uint version, Stream s, int origCount, DependentList<VertexFormat> parentVertexFormats) : base(null) { this.origCount = origCount; this.version = version; this.parentVertexFormats = parentVertexFormats; elementHandler = handler; Parse(s); this.handler = handler; }
            public VertexDataList(EventHandler handler, uint version, IEnumerable<VertexDataElement> ilt, DependentList<VertexFormat> parentVertexFormats)
                : base(null)
            {
                this.version = version;
                this.parentVertexFormats = parentVertexFormats;
                elementHandler = handler;
                foreach (var t in ilt)
                    this.Add(t);
                this.handler = handler;
            }
            #endregion

            protected override int ReadCount(Stream s) { return origCount; }
            protected override VertexDataElement CreateElement(Stream s) { return new VertexDataElement(0, elementHandler, version, s, parentVertexFormats); }

            protected override void WriteCount(Stream s, int count) { }
            protected override void WriteElement(Stream s, VertexDataElement element) { element.UnParse(s); }

            public override void Add() { this.Add(new VertexDataElement(0, elementHandler, version, parentVertexFormats)); }
            public override void Add(VertexDataElement item) { item.ParentVertexFormats = parentVertexFormats; base.Add(item); }
        }
        #endregion

        public class Face : AHandlerElement, IEquatable<Face>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            ushort vertexDataIndex0;
            ushort vertexDataIndex1;
            ushort vertexDataIndex2;
            #endregion

            public Face(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Face(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Face(int APIversion, EventHandler handler, Face basis)
                : this(APIversion, handler, basis.vertexDataIndex0, basis.vertexDataIndex1, basis.vertexDataIndex2) { }
            public Face(int APIversion, EventHandler handler, ushort vertexDataIndex0, ushort vertexDataIndex1, ushort vertexDataIndex2)
                : base(APIversion, handler)
            {
                this.vertexDataIndex0 = vertexDataIndex0;
                this.vertexDataIndex1 = vertexDataIndex1;
                this.vertexDataIndex2 = vertexDataIndex2;
            }

            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                vertexDataIndex0 = r.ReadUInt16();
                vertexDataIndex1 = r.ReadUInt16();
                vertexDataIndex2 = r.ReadUInt16();
            }
            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(vertexDataIndex0);
                w.Write(vertexDataIndex1);
                w.Write(vertexDataIndex2);
            }

            #region AHandlerElement
            // public override AHandlerElement Clone(EventHandler handler) { return new Face(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Face>
            public bool Equals(Face other)
            {
                return this.vertexDataIndex0.Equals(other.vertexDataIndex0)
                    && this.vertexDataIndex1.Equals(other.vertexDataIndex1)
                    && this.vertexDataIndex2.Equals(other.vertexDataIndex2);
            }

            public override bool Equals(object obj) { return obj is VertexFormat && Equals(obj as VertexFormat); }

            public override int GetHashCode() { return vertexDataIndex0.GetHashCode() ^ vertexDataIndex1.GetHashCode() ^ vertexDataIndex2.GetHashCode(); }
            #endregion

            [ElementPriority(1)]
            public ushort VertexDataIndex0 { get { return vertexDataIndex0; } set { if (vertexDataIndex0 != value) { vertexDataIndex0 = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public ushort VertexDataIndex1 { get { return vertexDataIndex1; } set { if (vertexDataIndex1 != value) { vertexDataIndex1 = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public ushort VertexDataIndex2 { get { return vertexDataIndex2; } set { if (vertexDataIndex2 != value) { vertexDataIndex2 = value; OnElementChanged(); } } }

            public string Value { get { return string.Join("; ", ValueBuilder.Split('\n')); } }
        }
        public class FaceList : DependentList<Face>
        {
            #region Constructors
            public FaceList(EventHandler handler) : base(handler) { }
            public FaceList(EventHandler handler, Stream s) : base(handler, s) {}
            public FaceList(EventHandler handler, IEnumerable<Face> le) : base(handler, le) { }
            #endregion

            protected override int ReadCount(Stream s) { return base.ReadCount(s) / 3; }
            protected override Face CreateElement(Stream s) { return new Face(0, elementHandler, s); }
            protected override void WriteCount(Stream s, int count) { base.WriteCount(s, (int)(count * 3)); }
            protected override void WriteElement(Stream s, Face element) { element.UnParse(s); }

            //public override void Add() { this.Add(new Face(0, elementHandler)); }
        }
        public class UnknownThing : AHandlerElement, IEquatable<UnknownThing>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            uint unknown1;
            Vector2List unknown2;
            #endregion

            public UnknownThing(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public UnknownThing(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public UnknownThing(int APIversion, EventHandler handler, UnknownThing basis) : this(APIversion, handler, basis.unknown1, basis.unknown2) { }
            public UnknownThing(int APIversion, EventHandler handler, uint unknown1, IEnumerable<Vector2> unknown2)
                : base(APIversion, handler)
            {
                this.unknown1 = unknown1;
                this.unknown2 = unknown2 == null ? null : new Vector2List(handler, unknown2);
            }

            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                unknown1 = r.ReadUInt32();
                unknown2 = new Vector2List(handler, s);
            }
            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(unknown1);
                if (unknown2 == null) unknown2 = new Vector2List(handler);
                unknown2.UnParse(s);
            }

            #region AHandlerElement
            // public override UnknownThing Clone(EventHandler handler) { return new UnknownThing(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<UnknownThing>
            public bool Equals(UnknownThing other)
            {
                if (this.unknown1 != other.unknown1)
                    return false;

                if (this.unknown2 == null && other.unknown2 != null)
                    return false;

                if (this.unknown2 != null)
                {
                    if (other.unknown2 == null || this.unknown2.Count != other.unknown2.Count)
                        return false;

                    for (int i = this.unknown2.Count - 1; i >= 0; i--)
                    {
                        if (!this.unknown2[i].Equals(other.unknown2[i]))
                            return false;
                    }
                    return true;
                }

                return false;
            }

            public override bool Equals(object obj) { return obj is UnknownThing && Equals(obj as UnknownThing); }

            public override int GetHashCode() { return unknown1.GetHashCode() ^ unknown2.GetHashCode(); }
            #endregion
            [ElementPriority(1)]
            public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public Vector2List Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { unknown2 = value == null ? null : new Vector2List(handler, value); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
        }
        public class UnknownThingList : DependentList<UnknownThing>
        {
            #region Constructors
            public UnknownThingList(EventHandler handler) : base(handler) { }
            public UnknownThingList(EventHandler handler, Stream s) : base(handler, s) { }
            public UnknownThingList(EventHandler handler, IEnumerable<UnknownThing> le) : base(handler, le) { }
            #endregion

            protected override UnknownThing CreateElement(Stream s) { return new UnknownThing(0, elementHandler, s); }
            protected override void WriteElement(Stream s, UnknownThing element) { element.UnParse(s); }
        }


        public class UnknownThing2 : AHandlerElement, IEquatable<UnknownThing2>
        {
            const int recommendedApiVersion = 1;
            const int unk5size = 53;//bytes;
            // Sizes found: 53 (common), 245

            #region Attributes
            uint unknown1;
            ushort unknown2;
            ushort unknown3;
            ushort unknown4;
            float unknown5;
            float unknown6;
            float unknown7;
            float unknown8;
            float unknown9;
            float unknown10;
            float unknown11;
            float unknown12;
            float unknown13;
            float unknown14;
            float unknown15;
            float unknown16;
            float unknown17;
            byte unknown18;
            #endregion

            public UnknownThing2(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public UnknownThing2(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public UnknownThing2(int APIversion, EventHandler handler, UnknownThing2 basis)
                : this(APIversion, handler, basis.unknown1,
                basis.unknown2, basis.unknown3, basis.unknown4, basis.unknown5,
                basis.unknown6, basis.unknown7, basis.unknown8, basis.unknown9,
                basis.unknown10, basis.unknown11, basis.unknown12, basis.unknown13,
                basis.unknown14, basis.unknown15, basis.unknown16, basis.unknown17, basis.unknown18) { }
            public UnknownThing2(int APIversion, EventHandler handler, uint unknown1,
                ushort unknown2, ushort unknown3, ushort unknown4, float unknown5,
                float unknown6, float unknown7, float unknown8, float unknown9,
                float unknown10, float unknown11, float unknown12, float unknown13, 
                float unknown14, float unknown15, float unknown16, float unknown17, byte unknown18)
                : base(APIversion, handler)
            {
                this.unknown1 = unknown1;
                this.unknown2 = unknown2;
                this.unknown3 = unknown3;
                this.unknown4 = unknown4;
                this.unknown5 = unknown5;
                this.unknown6 = unknown6;
                this.unknown7 = unknown7;
                this.unknown8 = unknown8;
                this.unknown9 = unknown9;
                this.unknown10 = unknown10;
                this.unknown11 = unknown11;
                this.unknown12 = unknown12;
                this.unknown13 = unknown13;
                this.unknown14 = unknown14;
                this.unknown15 = unknown15;
                this.unknown16 = unknown16;
                this.unknown17 = unknown17;
                this.unknown18 = unknown18;
            }

            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                unknown1 = r.ReadUInt32();
                unknown2 = r.ReadUInt16();
                unknown3 = r.ReadUInt16();
                unknown4 = r.ReadUInt16();
                unknown5 = r.ReadSingle();
                unknown6 = r.ReadSingle();
                unknown7 = r.ReadSingle();
                unknown8 = r.ReadSingle();
                unknown9 = r.ReadSingle();
                unknown10 = r.ReadSingle();
                unknown11 = r.ReadSingle();
                unknown12 = r.ReadSingle();
                unknown13 = r.ReadSingle();
                unknown14 = r.ReadSingle();
                unknown15 = r.ReadSingle();
                unknown16 = r.ReadSingle();
                unknown17 = r.ReadSingle();
                unknown18 = r.ReadByte();
            }
            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(unknown1);
                w.Write(unknown2);
                w.Write(unknown3);
                w.Write(unknown4);
                w.Write(unknown5);
                w.Write(unknown6);
                w.Write(unknown7);
                w.Write(unknown8);
                w.Write(unknown9);
                w.Write(unknown10);
                w.Write(unknown11);
                w.Write(unknown12);
                w.Write(unknown13);
                w.Write(unknown14);
                w.Write(unknown15);
                w.Write(unknown16);
                w.Write(unknown17);
                w.Write(unknown18);
            }

            #region AHandlerElement
            // public override UnknownThing Clone(EventHandler handler) { return new UnknownThing(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<UnknownThing>
            public bool Equals(UnknownThing2 other)
            {
                return this.unknown1.Equals(other.unknown1)
                    && this.unknown2.Equals(other.unknown2)
                    && this.unknown3.Equals(other.unknown3)
                    && this.unknown4.Equals(other.unknown4)
                    && this.unknown5.Equals(other.unknown5)
                    && this.unknown6.Equals(other.unknown6)
                    && this.unknown7.Equals(other.unknown7)
                    && this.unknown8.Equals(other.unknown8)
                    && this.unknown9.Equals(other.unknown9)
                    && this.unknown10.Equals(other.unknown10)
                    && this.unknown11.Equals(other.unknown11)
                    && this.unknown12.Equals(other.unknown12)
                    && this.unknown13.Equals(other.unknown13)
                    && this.unknown14.Equals(other.unknown14)
                    && this.unknown15.Equals(other.unknown15)
                    && this.unknown16.Equals(other.unknown16)
                    && this.unknown17.Equals(other.unknown17)
                    && this.unknown18.Equals(other.unknown18);
            }

            public override bool Equals(object obj) { return obj is UnknownThing2 && Equals(obj as UnknownThing2); }

            public override int GetHashCode() { return unknown1.GetHashCode() ^ unknown2.GetHashCode() ^ unknown3.GetHashCode() ^ unknown4.GetHashCode() ^ unknown5.GetHashCode(); }
            #endregion
            [ElementPriority(1)]
            public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public ushort Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public ushort Unknown3 { get { return unknown3; } set { if (unknown3 != value) { unknown3 = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public ushort Unknown4 { get { return unknown4; } set { if (unknown4 != value) { unknown4 = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public float Unknown5 { get { return unknown5; } set { if (unknown5 != value) { unknown5 = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public float Unknown6 { get { return unknown6; } set { if (unknown6 != value) { unknown6 = value; OnElementChanged(); } } }
            [ElementPriority(7)]
            public float Unknown7 { get { return unknown7; } set { if (unknown7 != value) { unknown7 = value; OnElementChanged(); } } }
            [ElementPriority(8)]
            public float Unknown8 { get { return unknown8; } set { if (unknown8 != value) { unknown8 = value; OnElementChanged(); } } }
            [ElementPriority(9)]
            public float Unknown9 { get { return unknown9; } set { if (unknown9 != value) { unknown9 = value; OnElementChanged(); } } }
            [ElementPriority(10)]
            public float Unknown10 { get { return unknown10; } set { if (unknown10 != value) { unknown10 = value; OnElementChanged(); } } }
            [ElementPriority(11)]
            public float Unknown11 { get { return unknown11; } set { if (unknown11 != value) { unknown11 = value; OnElementChanged(); } } }
            [ElementPriority(12)]
            public float Unknown12 { get { return unknown12; } set { if (unknown12 != value) { unknown12 = value; OnElementChanged(); } } }
            [ElementPriority(13)]
            public float Unknown13 { get { return unknown13; } set { if (unknown13 != value) { unknown13 = value; OnElementChanged(); } } }
            [ElementPriority(14)]
            public float Unknown14 { get { return unknown14; } set { if (unknown14 != value) { unknown14 = value; OnElementChanged(); } } }
            [ElementPriority(15)]
            public float Unknown15 { get { return unknown15; } set { if (unknown15 != value) { unknown15 = value; OnElementChanged(); } } }
            [ElementPriority(16)]
            public float Unknown16 { get { return unknown16; } set { if (unknown16 != value) { unknown16 = value; OnElementChanged(); } } }
            [ElementPriority(17)]
            public float Unknown17 { get { return unknown17; } set { if (unknown17 != value) { unknown17 = value; OnElementChanged(); } } }
            [ElementPriority(18)]
            public byte Unknown18 { get { return unknown18; } set { if (unknown18 != value) { unknown18 = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
        }
        public class UnknownThing2List : DependentList<UnknownThing2>
        {
            #region Constructors
            public UnknownThing2List(EventHandler handler) : base(handler) { }
            public UnknownThing2List(EventHandler handler, Stream s) : base(handler, s) {}
            public UnknownThing2List(EventHandler handler, IEnumerable<UnknownThing2> le) : base(handler, le) { }
            #endregion

            protected override UnknownThing2 CreateElement(Stream s) { return new UnknownThing2(0, elementHandler, s); }
            protected override void WriteElement(Stream s, UnknownThing2 element) { element.UnParse(s); }
        }
        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public ShaderType Shader { get { return shader; } set { if (shader != value) { shader = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public MTNF Mtnf
        {
            get { return mtnf; }
            set
            {
                if ((shader == 0 && value != null) || (shader != 0 && value == null)) throw new ArgumentException();
                if (!mtnf.Equals(value))
                {
                    mtnf = new MTNF(requestedApiVersion, OnRCOLChanged, value) { ParentTGIBlocks = tgiBlockList, RCOLTag = "GEOM", };
                    OnRCOLChanged(this, EventArgs.Empty);
                }
            }
        }
        [ElementPriority(14)]
        public uint MergeGroup { get { return mergeGroup; } set { if (mergeGroup != value) { mergeGroup = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public uint SortOrder { get { return sortOrder; } set { if (sortOrder != value) { sortOrder = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public VertexFormatList VertexFormats
        {
            get { return vertexFormats; }
            set { if (!vertexFormats.Equals(value)) { vertexFormats = value == null ? null : new VertexFormatList(OnRCOLChanged, version, value); OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(17)]
        public VertexDataList VertexData
        {
            get { return vertexData; }
            set { if (!vertexData.Equals(value)) { vertexData = value == null ? null : new VertexDataList(OnRCOLChanged, version, value, this.vertexFormats); OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(18)]
        public FaceList Faces
        {
            get { return faces; }
            set { if (!faces.Equals(value)) { faces = value == null ? null : new FaceList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(19), TGIBlockListContentField("TGIBlocks")]
        public int SkinIndex { get { return skinIndex; } set { if (skinIndex != value) { skinIndex = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(19)]
        public UnknownThingList UnknownThings { get { return unknownThings; } set { if (!unknownThings.Equals(value)) { unknownThings = value == null ? null : new UnknownThingList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(20)]
        public UnknownThing2List UnknownThings2 { get { return unknownThings2; } set { if (!unknownThings2.Equals(value)) { unknownThings2 = value == null ? null : new UnknownThing2List(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(21)]
        public UIntList BoneHashes
        {
            get { return boneHashes; }
            set { if (!boneHashes.Equals(value)) { boneHashes = value == null ? null : new UIntList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(22)]
        public TGIBlockList TGIBlocks
        {
            get { return tgiBlockList; }
            set
            {
                if (!tgiBlockList.Equals(value))
                {
                    tgiBlockList = value == null ? null : new TGIBlockList(OnRCOLChanged, value);
                    if (mtnf != null)
                        mtnf.ParentTGIBlocks = tgiBlockList;
                    OnRCOLChanged(this, EventArgs.Empty);
                }
            }
        }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }
}