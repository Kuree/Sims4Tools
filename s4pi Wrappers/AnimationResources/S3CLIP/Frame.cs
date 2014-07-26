using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using s4pi.Interfaces;

namespace s4pi.Animation.S3CLIP
{
    public class Frame : AHandlerElement,
        IEquatable<Frame>
    {
        protected float[] mData;
        protected UInt16 mFlags;
        protected UInt16 mFrameIndex;

        protected Frame(int apiVersion, EventHandler handler, CurveDataType dataType)
            : base(apiVersion, handler)
        {
            this.DataType = dataType;
            mData = new float[Curve.GetFloatCount(DataType)];
        }

        protected Frame(int apiVersion, EventHandler handler, Frame basis)
            : this(apiVersion, handler, basis.DataType)
        {
            mData = basis.mData;
            mFrameIndex = basis.mFrameIndex;
            mFlags = basis.mFlags;
        }

        public Frame(int apiVersion, EventHandler handler, Stream s, CurveDataInfo info, IList<float> indexedFloats)
            : this(apiVersion, handler, info.Flags.Type)
        {
            Parse(s, info, indexedFloats);
        }

        public CurveDataType DataType { get; set; }

        [ElementPriority(1)]
        public ushort FrameIndex
        {
            get { return mFrameIndex; }
            set
            {
                if (mFrameIndex != value)
                {
                    mFrameIndex = value;
                    OnElementChanged();
                }
            }
        }

        [ElementPriority(2)]
        public UInt16 Flags
        {
            get { return mFlags; }
            set
            {
                if (mFlags != value)
                {
                    mFlags = value;
                    OnElementChanged();
                }
            }
        }

        [ElementPriority(3)]
        public float[] Data
        {
            get { return mData; }
            set { mData = value; }
        }

        public string Value
        {
            get { return ValueBuilder; }
        }

        public override List<string> ContentFields
        {
            get { return GetContentFields(0, GetType()); }
        }

        public override int RecommendedApiVersion
        {
            get { return 1; }
        }

        #region IEquatable<Frame> Members

        public bool Equals(Frame other)
        {
            IEnumerable<float> a = GetFloatValues();
            IEnumerable<float> b = other.GetFloatValues();
            if (a.Count() != b.Count()) return false;
            return !(from af in a from bf in b where af != bf select af).Any();
        }

        #endregion


        public void Parse(Stream s, CurveDataInfo info, IList<float> indexedFloats)
        {
            var br = new BinaryReader(s);
            mFrameIndex = br.ReadUInt16();
            ushort flags = br.ReadUInt16();
            mFlags = (UInt16)(flags >> 4);

            switch (info.Flags.Format)
            {
                case CurveDataFormat.Indexed:

                    for (int floatsRead = 0; floatsRead < Curve.GetFloatCount(DataType); floatsRead++)
                    {
                        float val = indexedFloats[br.ReadUInt16()];
                        if ((flags & 1 << floatsRead) != 0 ? true : false) val *= -1;
                        mData[floatsRead] = Unpack(val, info.Offset, info.Scale);
                    }
                    break;
                case CurveDataFormat.Packed:
                    for (int packedRead = 0; packedRead < Curve.GetPackedCount(DataType); packedRead++)
                    {
                        ulong packed;
                        switch (DataType)
                        {
                            case CurveDataType.Float4:
                            case CurveDataType.Float1:
                                packed = br.ReadUInt16();
                                break;
                            case CurveDataType.Float3:
                                packed = br.ReadUInt32();
                                break;
                            default:
                                packed = br.ReadUInt16();
                                break;
                        }
                        for (int packedIndex = 0; packedIndex < Curve.GetFloatCount(DataType) / Curve.GetPackedCount(DataType); packedIndex++)
                        {

                            int floatIndex = packedIndex + packedRead;
                            int bitsPerFloat = Curve.GetBitsPerFloat(DataType);
                            var maxPackedVal = (ulong)(Math.Pow(2, bitsPerFloat) - 1);
                            ulong mask = (maxPackedVal << (packedIndex * bitsPerFloat));
                            float val = ((packed & mask) >> (packedIndex * bitsPerFloat)) / (float)maxPackedVal;
                            if ((flags & 1 << floatIndex) != 0 ? true : false) val *= -1;
                            mData[floatIndex] = Unpack(val, info.Offset, info.Scale);
                        }
                    }
                    break;
            }
        }

        public static float Unpack(float packed, float offset, float scale)
        {
            return (packed * scale) + offset;
        }

        public static float Pack(float unpacked, float offset, float scale)
        {
            return (unpacked - offset) / scale;
        }

        public virtual void UnParse(Stream s, CurveDataInfo info, IList<float> indexedFloats)
        {
            var bw = new BinaryWriter(s);
            bw.Write(mFrameIndex);
            int flags = mFlags << 4;
            UInt16[] indices = null;
            ulong[] packedVals = null;
            switch (info.Flags.Format)
            {
                case CurveDataFormat.Indexed:
                    indices = new ushort[Curve.GetFloatCount(DataType)];
                    for (int i = 0; i < Curve.GetFloatCount(DataType); i++)
                    {
                        float packedIndex = Pack(mData[i], info.Offset, info.Scale);
                        if (packedIndex < 0) flags |= (1 << i);
                        packedIndex = Math.Abs(packedIndex);
                        if (!indexedFloats.Contains(packedIndex)) indexedFloats.Add(packedIndex);
                        indices[i] = (UInt16)indexedFloats.IndexOf(packedIndex);
                    }
                    break;
                case CurveDataFormat.Packed:
                    packedVals = new ulong[Curve.GetPackedCount(DataType)];
                    for (int packedWritten = 0; packedWritten < packedVals.Length; packedWritten++)
                    {
                        ulong packed = 0;
                        for (int packedIndex = 0; packedIndex < Curve.GetFloatCount(DataType) / packedVals.Length; packedIndex++)
                        {
                            int floatIndex = packedWritten + packedIndex;
                            double maxPackedVal = Math.Pow(2, Curve.GetBitsPerFloat(DataType)) - 1;
                            float val = (mData[floatIndex] - info.Offset) / info.Scale;
                            if (val < 0) flags |= (1 << floatIndex);
                            val = Math.Abs(val);
                            packed |= (ulong)Math.Floor(val * maxPackedVal) << (packedIndex * Curve.GetBitsPerFloat(DataType));
                        }
                        packedVals[packedWritten] = packed;
                    }
                    break;
            }
            bw.Write((UInt16)flags);
            switch (info.Flags.Format)
            {
                case CurveDataFormat.Indexed:
                    for (int i = 0; i < indices.Length; i++)
                        bw.Write(indices[i]);
                    break;
                case CurveDataFormat.Packed:
                    for (int i = 0; i < packedVals.Length; i++)
                        Debug.WriteLine("broken");//WritePacked(s, packedVals[i]);

                    break;
            }
        }

        public virtual IEnumerable<float> GetFloatValues()
        {
            return mData;
        }

        public override AHandlerElement Clone(EventHandler handler)
        {
            return new Frame(0, handler, this);
        }
    }
}