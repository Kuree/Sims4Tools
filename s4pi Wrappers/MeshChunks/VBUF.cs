using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using s4pi.GenericRCOLResource;
using s4pi.Interfaces;
using s4pi.Settings;
namespace meshExpImp.ModelBlocks
{

    public struct Vertex
    {
        public float[] Position;
        public float[] Normal;
        public float[][] UV;
        public byte[] BlendIndices;
        public float[] BlendWeights;
        public float[] Tangents;
        public float[] Color;

    }
    public class VBUF2 : VBUF
    {
        public VBUF2(int apiVersion, EventHandler handler, VBUF basis)
            : base(apiVersion, handler, basis)
        {
        }

        public VBUF2(int apiVersion, EventHandler handler)
            : base(apiVersion, handler)
        {
        }

        public VBUF2(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler, s)
        {
        }

        public override uint ResourceType
        {
            get
            {
                return 0x0229684B;
            }
        }
    }
    public class VBUF : ARCOLBlock
    {
        static IDictionary<int, int> kColorUByte4Map =
        new Dictionary<int, int>
        {
            {0, 2},
            {1, 1},
            {2, 0},
            {3, 3}
        };
        [Flags]
        public enum FormatFlags : uint
        {
            Collapsed = 0x4,
            DifferencedVertices = 0x2,
            Dynamic = 0x1,
            None = 0x0
        }

        private UInt32 mVersion = 0x00000101;
        private FormatFlags mFlags;
        private GenericRCOLResource.ChunkReference mSwizzleInfo;
        private Byte[] mBuffer;
        public VBUF(int apiVersion, EventHandler handler, VBUF basis) : this(apiVersion, handler, basis.Version, basis.Flags, basis.SwizzleInfo, (Byte[])basis.Buffer.Clone()) { }
        public VBUF(int apiVersion, EventHandler handler) : base(apiVersion, handler, null) { }
        public VBUF(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler, s) { }
        public VBUF(int APIversion, EventHandler handler, uint version, FormatFlags flags, GenericRCOLResource.ChunkReference swizzleInfo, byte[] buffer)
            : this(APIversion, handler)
        {
            mVersion = version;
            mFlags = flags;
            mSwizzleInfo = swizzleInfo;
            mBuffer = buffer;
        }

        [ElementPriority(1)]
        public UInt32 Version { get { return mVersion; } set { if (mVersion != value) { mVersion = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public FormatFlags Flags { get { return mFlags; } set { if (mFlags != value) { mFlags = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public GenericRCOLResource.ChunkReference SwizzleInfo { get { return mSwizzleInfo; } set { if (mSwizzleInfo != value) { mSwizzleInfo = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public Byte[] Buffer { get { return mBuffer; } set { if (mBuffer != value) { mBuffer = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        public string Value { get { return ValueBuilder; } }
        protected override List<string> ValueBuilderFields
        {
            get
            {
                var fields = base.ValueBuilderFields;
                fields.Remove("Buffer");
                return fields;
            }
        }
        protected override void Parse(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            string tag = FOURCC(br.ReadUInt32());
            if (checking && tag != Tag)
            {
                throw new InvalidDataException(string.Format("Invalid Tag read: '{0}'; expected: '{1}'; at 0x{2:X8}", tag, Tag, s.Position));
            }
            mVersion = br.ReadUInt32();
            mFlags = (FormatFlags)br.ReadUInt32();
            mSwizzleInfo = new GenericRCOLResource.ChunkReference(0, handler, s);
            mBuffer = new Byte[s.Length - s.Position];
            s.Read(mBuffer, 0, mBuffer.Length);
        }

        public override Stream UnParse()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(s);
            bw.Write((UInt32)FOURCC(Tag));
            bw.Write(mVersion);
            bw.Write((UInt32)mFlags);
            if (mSwizzleInfo == null) mSwizzleInfo = new GenericRCOLResource.ChunkReference(0, handler, 0);
            mSwizzleInfo.UnParse(s);
            if (mBuffer == null) mBuffer = new byte[0];
            bw.Write(mBuffer);
            return s;

        }
        public override uint ResourceType
        {
            get { return 0x01D0E6FB; }
        }

        public override string Tag
        {
            get { return "VBUF"; }
        }
        static bool checking = Settings.Checking;

        public BoundingBox GetBoundingBox(MLOD.Mesh mesh, VRTF vrtf)
        {
            BoundingBox bbox = null;

            GetBoundingBox(GetVertices(mesh, vrtf, null), ref bbox);
            foreach (var geos in mesh.GeometryStates)
                GetBoundingBox(GetVertices(mesh, vrtf, geos, null), ref bbox);

            return bbox ?? new BoundingBox(0, null);
        }
        private void GetBoundingBox(IEnumerable<Vertex> vertices, ref BoundingBox bbox)
        {
            foreach (float[] fs in vertices.Select(x => x.Position))
            {
                if (fs.Length != 3)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} floats in vertex position", fs.Length));
                    continue;
                }
                if (bbox == null)
                {
                    bbox = new BoundingBox(0, null, new s4pi.Interfaces.Vertex(0, null, fs[0], fs[1], fs[2]), new s4pi.Interfaces.Vertex(0, null, fs[0], fs[1], fs[2]));
                }
                else
                {
                    if (bbox.Min.X > fs[0]) bbox.Min.X = fs[0];
                    if (bbox.Min.Y > fs[1]) bbox.Min.Y = fs[1];
                    if (bbox.Min.Z > fs[2]) bbox.Min.Z = fs[2];
                    if (bbox.Max.X < fs[0]) bbox.Max.X = fs[0];
                    if (bbox.Max.Y < fs[1]) bbox.Max.Y = fs[1];
                    if (bbox.Max.Z < fs[2]) bbox.Max.Z = fs[2];
                }
            }
        }

        public Vertex[] GetVertices(MLOD.Mesh mesh, VRTF vrtf, float[] uvscales)
        {
            return GetVertices(vrtf, mesh.StreamOffset, mesh.VertexCount, uvscales);
        }
        public Vertex[] GetVertices(MLOD.Mesh mesh, VRTF vrtf, MLOD.GeometryState geo, float[] uvscales)
        {
            return GetVertices(vrtf, mesh.StreamOffset + (geo.MinVertexIndex * vrtf.Stride), geo.VertexCount, uvscales);
        }

        public Vertex[] GetVertices(VRTF vrtf, long offset, int count, float[] uvscales)
        {
            long streamOffset = offset;
            Stream s = new MemoryStream(mBuffer);
            s.Seek(streamOffset, SeekOrigin.Begin);

            var position = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.Position);
            var normal = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.Normal);
            var uv = vrtf.Layouts
                .Where(x => x.Usage == VRTF.ElementUsage.UV)
                .ToArray();
            var blendIndices = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.BlendIndex);
            var blendWeights = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.BlendWeight);
            var tangents = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.Tangent);
            var color = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.Colour);

            Vertex[] verts = new Vertex[count];
            if (uvscales == null) uvscales = new float[3];
            for (int i = 0; i < count; i++)
            {
                Vertex v = new Vertex();
                byte[] data = new byte[vrtf.Stride];
                s.Read(data, 0, vrtf.Stride);
                if (position != null)
                {
                    float[] posPoints = new float[VRTF.FloatCountFromFormat(position.Format)];
                    ReadFloatData(data, position, ref posPoints);
                    v.Position = posPoints;
                }
                if (normal != null)
                {
                    float[] normPoints = new float[VRTF.FloatCountFromFormat(normal.Format)];
                    ReadFloatData(data, normal, ref normPoints);
                    v.Normal = normPoints;
                }
                v.UV = new float[uv.Length][];
                for (int j = 0; j < uv.Length; j++)
                {
                    var u = uv[j];
                    float[] uvPoints = new float[VRTF.FloatCountFromFormat(u.Format)];
                    var scale = j < uvscales.Length && uvscales[j] != 0 ? uvscales[j] : uvscales[0];
                    ReadUVData(data, u, ref uvPoints, scale);
                    v.UV[j] = uvPoints;
                }
                if (blendIndices != null)
                {
                    byte[] blendIPoints = new byte[VRTF.ByteSizeFromFormat(blendIndices.Format)];
                    Array.Copy(data, blendIndices.Offset, blendIPoints, 0, blendIPoints.Length);
                    v.BlendIndices = blendIPoints;
                }
                if (blendWeights != null)
                {
                    float[] blendWPoints = new float[VRTF.FloatCountFromFormat(blendWeights.Format)];
                    ReadFloatData(data, blendWeights, ref blendWPoints);
                    v.BlendWeights = blendWPoints;
                }
                if (tangents != null)
                {
                    float[] tangentPoints = new float[VRTF.FloatCountFromFormat(tangents.Format)];
                    ReadFloatData(data, tangents, ref tangentPoints);
                    v.Tangents = tangentPoints;
                }
                if (color != null)
                {
                    float[] colorPoints = new float[VRTF.FloatCountFromFormat(color.Format)];
                    ReadFloatData(data, color, ref colorPoints);
                    v.Color = colorPoints;
                }
                verts[i] = v;
            }
            return verts;
        }
        public static void ReadUVData(byte[] data, VRTF.ElementLayout layout, ref float[] output, float scale)
        {
            byte[] element = new byte[VRTF.ByteSizeFromFormat(layout.Format)];
            Array.Copy(data, layout.Offset, element, 0, element.Length);

            switch (layout.Format)
            {
                case VRTF.ElementFormat.Short2:
                    for (int i = 0; i < output.Length; i++)
                    {
                        output[i] += (float)BitConverter.ToInt16(element, i * sizeof(short)) * scale;
                    }
                    break;
                case VRTF.ElementFormat.Short4:
                    for (int i = 0; i < output.Length; i++)
                        output[i] += (float)BitConverter.ToInt16(element, i * sizeof(short)) / short.MaxValue;
                    break;
                case VRTF.ElementFormat.Short4_DropShadow:
                    for (int i = 0; i < output.Length - 1; i++)
                        output[i] += (float)BitConverter.ToInt16(element, i * sizeof(short)) / short.MaxValue;
                    output[output.Length - 1] += (float)BitConverter.ToInt16(element, (output.Length - 1) * sizeof(short)) / 511;
                    break;
                default:
                    ReadFloatData(data, layout, ref output);
                    break;
            }
        }
        //Currently not supported:
        //UByte4N,
        //Short2N, Short4N, UShort2N,
        //Dec3N, UDec3N,
        //Float16_2, Float16_4
        public static void ReadFloatData(byte[] data, VRTF.ElementLayout layout, ref float[] output)
        {
            byte[] element = new byte[VRTF.ByteSizeFromFormat(layout.Format)];
            Array.Copy(data, layout.Offset, element, 0, element.Length);
            float scalar;

            switch (layout.Format)
            {
                case VRTF.ElementFormat.Float1:
                case VRTF.ElementFormat.Float2:
                case VRTF.ElementFormat.Float3:
                case VRTF.ElementFormat.Float4:
                    for (int i = 0; i < output.Length; i++)
                        output[i] += BitConverter.ToSingle(element, i * sizeof(float));
                    break;
                case VRTF.ElementFormat.ColorUByte4:
                    switch (layout.Usage)
                    {
                        case VRTF.ElementUsage.Colour:
                            for (int i = 0; i < output.Length; i++)
                                output[i] += element[i] / (float)byte.MaxValue;
                            break;
                        case VRTF.ElementUsage.BlendWeight:
                            for (int i = 0; i < output.Length; i++)
                                output[i] += element[kColorUByte4Map[i]] / (float)byte.MaxValue;
                            break;
                        case VRTF.ElementUsage.Normal:
                        case VRTF.ElementUsage.Tangent:
                            for (int i = 0; i < output.Length - 1; i++)
                                output[i] += element[2 - i] == 0 ? -1 : (((element[2 - i] + 1) / 128f) - 1);
                            //--
                            // Wes: (signed char) bytes[0]=vert[2+vrtfp.offset[j]];
                            //      (float)       norms[0]=(float)bytes[0];
                            //                    norms[0]=(norms[0]<(float)0.0)?(norms[0]+(float)128.0):(norms[0]-(float)128.0);
                            //???
                            //--
                            // input: 255 > 128 > 127 > 0
                            // map step1: +2 > 0
                            // map step2: +1 > -1
                            //for (int i = 0; i < output.Length - 1; i++)
                            //    output[i] += (element[2 - i] / 127.5f) - 1f;

                            switch (element[3])
                            {
                                case 0: output[output.Length - 1] = -1f; break; // -1 determinant
                                case 127: output[output.Length - 1] = 0f; break; // There is no determinant
                                case 255: output[output.Length - 1] = +1f; break; // +1 determinant
                                default: System.Diagnostics.Debug.WriteLine(String.Format("Unexpected handedness {0}.", element[3])); break;
                            }
                            break;
                    }
                    break;
                case VRTF.ElementFormat.Short2:
                    for (int i = 0; i < output.Length; i++)
                        output[i] += BitConverter.ToInt16(element, i * sizeof(short)) / (float)short.MaxValue;
                    break;
                case VRTF.ElementFormat.Short4:
                    scalar = BitConverter.ToUInt16(element, 3 * sizeof(short));
                    if (scalar == 0) scalar = short.MaxValue;
                    //scalar++;
                    for (int i = 0; i < output.Length; i++)
                        output[i] += BitConverter.ToInt16(element, i * sizeof(short)) / scalar;
                    break;
                case VRTF.ElementFormat.UShort4N:
                    scalar = BitConverter.ToUInt16(element, 3 * sizeof(ushort));
                    if (scalar == 0) scalar = 511;
                    //scalar++;
                    for (int i = 0; i < output.Length; i++)
                        output[i] += BitConverter.ToInt16(element, i * sizeof(short)) / scalar;
                    break;
            }
        }

        public bool SetVertices(MLOD mlod, int meshIndex, VRTF vrtf, Vertex[] vertices, float[] uvscales)
        {
            return SetVertices(mlod, mlod.Meshes[meshIndex], vrtf, vertices, uvscales);
        }
        public bool SetVertices(MLOD mlod, MLOD.Mesh mesh, VRTF vrtf, Vertex[] vertices, float[] uvscales)
        {
            bool okay = SetVertices(mlod, mesh.VertexBufferIndex, mesh.StreamOffset, mesh.VertexCount, vrtf, vertices, uvscales);
            mesh.VertexCount = vertices.Length;
            return okay;
        }

        public bool SetVertices(MLOD mlod, MLOD.Mesh mesh, int geoIndex, VRTF vrtf, Vertex[] vertices, float[] uvscales)
        {
            return SetVertices(mlod, mesh, mesh.GeometryStates[geoIndex], vrtf, vertices, uvscales);
        }
        public bool SetVertices(MLOD mlod, MLOD.Mesh mesh, MLOD.GeometryState geo, VRTF vrtf, Vertex[] vertices, float[] uvscales)
        {
            long beforeLength = mesh.StreamOffset + (geo.MinVertexIndex * vrtf.Stride);
            bool okay = SetVertices(mlod, mesh.VertexBufferIndex, beforeLength, geo.VertexCount, vrtf, vertices, uvscales);
            geo.VertexCount = vertices.Length;
            return okay;
        }

        private bool SetVertices(MLOD mlod, s4pi.GenericRCOLResource.GenericRCOLResource.ChunkReference myVBI, long beforeLength, int count, VRTF vrtf, IEnumerable<Vertex> vertices, float[] uvscales)
        {
            bool okay = true;
            byte[] before = new byte[beforeLength];
            Array.Copy(mBuffer, before, before.Length);

            long afterPos = Math.Min(mBuffer.Length, beforeLength + (count * vrtf.Stride));
            byte[] after = new byte[mBuffer.Length - afterPos];
            Array.Copy(mBuffer, afterPos, after, 0, after.Length);

            long offset = 0;
            using (MemoryStream mg = new MemoryStream())
            {
                if (!SetVertices(mg, vrtf, vertices, uvscales)) okay = false;
                offset = beforeLength + mg.Length - afterPos;

                mBuffer = new byte[before.Length + mg.Length + after.Length];
                Array.Copy(before, mBuffer, before.Length);
                Array.Copy(mg.ToArray(), 0, mBuffer, before.Length, mg.Length);
                Array.Copy(after, 0, mBuffer, before.Length + mg.Length, after.Length);

                mg.Close();
            }

            int voffset = (int)offset / vrtf.Stride;
            if (offset != 0)
                foreach (var m in mlod.Meshes.Where(m => m.VertexBufferIndex.Equals(myVBI) && m.StreamOffset > beforeLength))
                {
                    m.StreamOffset = (uint)(m.StreamOffset + offset);
                    foreach (var g in m.GeometryStates)
                        if (g.MinVertexIndex * vrtf.Stride > beforeLength)
                            g.MinVertexIndex += voffset;
                }
            return okay;
        }

        static float positionMin;
        static float positionMax;
        private static void PositionMinMax(VRTF vrtf, IEnumerable<Vertex> vertices)
        {
            positionMin = 0f;
            positionMax = 0f;
            var layout = vrtf.Layouts.FirstOrDefault(x => x.Format == VRTF.ElementFormat.UShort4N && x.Usage == VRTF.ElementUsage.Position);
            if (layout == null) return;

            List<float> positionFloats = new List<float>();
            foreach (float[] fs in vertices.Select(x => x.Position))
                if (fs != null)
                    positionFloats.AddRange(fs);

            if (positionFloats.Count > 0)
            {
                positionMax = positionMin = Math.Abs(positionFloats[0]);
                foreach (float f in positionFloats)
                {
                    float x = Math.Abs(f);
                    if (positionMin > x) positionMin = x;
                    if (positionMax < x) positionMax = x;
                }
            }

            //positionMin = positionFloats.Count > 0 ? positionFloats.Min(x => Math.Abs(x)) : 0;
            //positionMax = positionFloats.Count > 0 ? positionFloats.Max(x => Math.Abs(x)) : 0;
        }
        static float[] defaultUVScales = new float[] { 1f / 32767f, 1f / 32767f, 1f / 32767f, };
        private static bool SetVertices(MemoryStream s, VRTF vrtf, IEnumerable<Vertex> vertices, float[] uvscales)
        {
            bool okay = true;
            PositionMinMax(vrtf, vertices);

            byte[] output = new byte[vrtf.Stride];
            var position = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.Position);
            var normal = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.Normal);
            var uv = vrtf.Layouts
                .Where(x => x.Usage == VRTF.ElementUsage.UV)
                .ToArray();
            var blendIndices = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.BlendIndex);
            var blendWeights = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.BlendWeight);
            var tangents = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.Tangent);
            var color = vrtf.Layouts
                .FirstOrDefault(x => x.Usage == VRTF.ElementUsage.Colour);
            if (uvscales == null) uvscales = defaultUVScales;
            foreach (var v in vertices)
            {
                if (v.Position != null) WritePositionData(v.Position, position, output, positionMax);
                if (v.Normal != null) WriteFloatData(v.Normal, normal, output);
                for (int u = 0; u < uv.Length; u++)
                {
                    var scale = u < uvscales.Length && uvscales[u] != 0 ? uvscales[u] : uvscales[0];
                    if (v.UV[u] != null)
                        if (!WriteUVData(v.UV[u], uv[u], output, scale))
                            okay = false;
                }
                if (v.BlendIndices != null) Array.Copy(v.BlendIndices, 0, output, blendIndices.Offset, VRTF.ByteSizeFromFormat(blendIndices.Format));
                if (v.BlendWeights != null) WriteFloatData(v.BlendWeights, blendWeights, output);
                if (v.Tangents != null) WriteFloatData(v.Tangents, tangents, output);
                if (v.Color != null) WriteFloatData(v.Color, color, output);
                s.Write(output, 0, output.Length);
            }
            s.Flush();
            return okay;
        }
        private static void WritePositionData(float[] input, VRTF.ElementLayout layout, byte[] output, float max)
        {
            switch (layout.Format)
            {
                case VRTF.ElementFormat.UShort4N:
                    //scalar = (max >= 1) ? 511 : (ulong)short.MaxValue;
                    //-- could try "max < 1.0 / 512.0"?
                    //ulong scalar = (ulong)(max < 1 ? short.MaxValue : 511);
                    //scalar++;
                    //2011-08-30 changed to fixed 512 as LoveseatDanishModern had problems
                    ulong scalar = 512;
                    for (int i = 0; i < input.Length; i++)
                        Array.Copy(BitConverter.GetBytes((short)Math.Round(input[i] * scalar)), 0, output, layout.Offset + i * sizeof(short), sizeof(short));
                    Array.Copy(BitConverter.GetBytes((short)(scalar == 512 ? 0 : scalar - 1)), 0, output, layout.Offset + 3 * sizeof(short), sizeof(short));
                    break;
                default:
                    WriteFloatData(input, layout, output);
                    break;
            }
        }
        private static bool WriteUVData(float[] input, VRTF.ElementLayout layout, byte[] output, float scale)
        {
            bool okay = true;
            switch (layout.Format)
            {
                case VRTF.ElementFormat.Short2:
                    for (int i = 0; i < input.Length; i++)
                    {
                        if ((short)Math.Round(input[i] / scale) != (long)Math.Round(input[i] / scale)) okay = false;
                        Array.Copy(BitConverter.GetBytes((short)Math.Round(input[i] / scale)), 0, output, layout.Offset + i * sizeof(short), sizeof(short));
                    }
                    break;

                case VRTF.ElementFormat.Short4:
                    for (int i = 0; i < input.Length; i++)
                        Array.Copy(BitConverter.GetBytes((short)Math.Round(input[i] * short.MaxValue)), 0, output, layout.Offset + i * sizeof(short), sizeof(short));
                    break;
                case VRTF.ElementFormat.Short4_DropShadow:
                    for (int i = 0; i < input.Length - 1; i++)
                        Array.Copy(BitConverter.GetBytes((short)Math.Round(input[i] * short.MaxValue)), 0, output, layout.Offset + i * sizeof(short), sizeof(short));
                    Array.Copy(BitConverter.GetBytes((short)Math.Round(input[input.Length - 1] * 511)), 0, output, layout.Offset + (input.Length - 1) * sizeof(short), sizeof(short));
                    break;
                default:
                    WriteFloatData(input, layout, output);
                    break;
            }
            return okay;
        }
        private static void WriteFloatData(float[] input, VRTF.ElementLayout layout, byte[] output)
        {
            ulong scalar;
            double max;

            switch (layout.Format)
            {
                case VRTF.ElementFormat.Float1:
                case VRTF.ElementFormat.Float2:
                case VRTF.ElementFormat.Float3:
                case VRTF.ElementFormat.Float4:
                    for (int i = 0; i < input.Length; i++)
                        Array.Copy(BitConverter.GetBytes(input[i]), 0, output, layout.Offset + i * sizeof(float), sizeof(float));
                    break;
                case VRTF.ElementFormat.ColorUByte4:
                    switch (layout.Usage)
                    {
                        case VRTF.ElementUsage.Colour:
                            for (int i = 0; i < input.Length; i++)
                                output[layout.Offset + i] = (byte)Math.Round(input[i] * byte.MaxValue);
                            break;
                        case VRTF.ElementUsage.BlendWeight:
                            for (int i = 0; i < input.Length; i++)
                                output[layout.Offset + kColorUByte4Map[i]] = (byte)Math.Round(input[i] * byte.MaxValue);
                            break;
                        case VRTF.ElementUsage.Normal:
                        case VRTF.ElementUsage.Tangent:
                            // -0.98828125 == (((0.5 + 1) / 128f) - 1)
                            for (int i = 0; i < input.Length - 1; i++)
                                output[layout.Offset + 2 - i] = (byte)(input[i] < -0.98828125 ? 0 : (Math.Round((input[i] + 1) * 128) - 1));
                            //--
                            // Wes: (signed char) bytes[0]=vert[2+vrtfp.offset[j]];
                            //      (float)       norms[0]=(float)bytes[0];
                            //                    norms[0]=(norms[0]<(float)0.0)?(norms[0]+(float)128.0):(norms[0]-(float)128.0);
                            //??
                            //--
                            // input: +1 > 0 > -1
                            // map step1: +2 > +1 > 0
                            // map step2: 255 > 127.5 > 0
                            //for (int i = 0; i < input.Length - 1; i++)
                            //    output[layout.Offset + 2 - i] = (byte)Math.Round((input[i] + 1) * 127.5);

                            if (input[input.Length - 1] == -1) output[layout.Offset + 3] = 0;
                            else if (input[input.Length - 1] == 0) output[layout.Offset + 3] = 127;
                            else if (input[input.Length - 1] == 1) output[layout.Offset + 3] = 255;
                            else
                                System.Diagnostics.Debug.WriteLine(String.Format("Unexpected handedness {0}.", input[input.Length - 1]));
                            break;
                    }
                    break;
                case VRTF.ElementFormat.Short2:
                    for (int i = 0; i < input.Length; i++)
                        Array.Copy(BitConverter.GetBytes((short)Math.Round(input[i] * short.MaxValue)), 0, output, layout.Offset + i * sizeof(short), sizeof(short));
                    break;
                case VRTF.ElementFormat.Short4:
                    max = Math.Ceiling(input.Max(x => Math.Abs(x)));
                    scalar = (ulong)(max < 1 ? short.MaxValue : short.MaxValue / max);
                    //scalar++;
                    for (int i = 0; i < input.Length; i++)
                        Array.Copy(BitConverter.GetBytes((short)Math.Round(input[i] * scalar)), 0, output, layout.Offset + i * sizeof(short), sizeof(short));
                    Array.Copy(BitConverter.GetBytes((short)(scalar /*- 1/**/)), 0, output, layout.Offset + 3 * sizeof(short), sizeof(short));
                    break;
                case VRTF.ElementFormat.UShort4N:
                    max = Math.Ceiling(input.Max(x => Math.Abs(x)));
                    scalar = (ulong)(max < 1.0 ? short.MaxValue : 512);
                    //scalar++;
                    for (int i = 0; i < input.Length; i++)
                        Array.Copy(BitConverter.GetBytes((short)Math.Round(input[i] * scalar)), 0, output, layout.Offset + i * sizeof(short), sizeof(short));
                    Array.Copy(BitConverter.GetBytes((short)(scalar == 512 ? 0 : scalar /*- 1/**/)), 0, output, layout.Offset + 3 * sizeof(short), sizeof(short));
                    break;
            }
        }
    }
}
