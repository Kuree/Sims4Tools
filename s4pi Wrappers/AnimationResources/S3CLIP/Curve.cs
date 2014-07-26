using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.Animation.S3CLIP
{
    public class Curve : AHandlerElement,
        IEquatable<Curve>
    {
        protected FrameList mFrames;
        protected CurveType mType;

        protected Curve(int apiVersion, EventHandler handler, CurveType type, CurveDataType dataType)
            : base(apiVersion, handler)
        {
            mType = type;
            mFrames = new FrameList(handler, dataType);
        }

        protected Curve(int apiVersion, EventHandler handler, Curve basis)
            : this(apiVersion, handler, basis.mType, basis.Frames.DataType)
        {
            mFrames = new FrameList(handler, basis.Frames.DataType, basis.Frames);
        }

        public Curve(int apiVersion, EventHandler handler, CurveType type, CurveDataType dataType, Stream s, CurveDataInfo info, IList<float> indexedFloats)
            : this(apiVersion, handler, type, dataType)
        {
            Parse(s, info, indexedFloats);
        }

        [ElementPriority(2)]
        public CurveType Type
        {
            get { return mType; }
        }

        [ElementPriority(3)]
        public FrameList Frames
        {
            get { return mFrames; }
            set
            {
                if (mFrames != value)
                {
                    mFrames = value;
                    OnElementChanged();
                }
            }
        }

        protected override List<string> ValueBuilderFields
        {
            get
            {
                List<string> f = base.ValueBuilderFields;
                f.Remove("Frames");
                return f;
            }
        }

        public override List<string> ContentFields
        {
            get { return GetContentFields(0, GetType()); }
        }

        public override int RecommendedApiVersion
        {
            get { return 1; }
        }

        public string Value
        {
            get { return ValueBuilder; }
        }

        #region IEquatable<Curve> Members

        public bool Equals(Curve other)
        {
            return base.Equals(other);
        }

        #endregion

        public static Int32 GetBitsPerFloat(CurveDataType curveType)
        {
            switch (curveType)
            {
                case CurveDataType.Float3:
                    return 10;
                case CurveDataType.Float4:
                    return 12;
                default:
                    throw new NotSupportedException();
            }
        }

        public static Int32 GetFloatCount(CurveDataType curveType)
        {
            switch (curveType)
            {
                case CurveDataType.Float3:
                    return 3;
                case CurveDataType.Float4:
                    return 4;
                default:
                    Debug.WriteLine("Testing unknown data type: " + curveType);
                    return 1;
            }
        }

        public static Int32 GetPackedCount(CurveDataType curveType)
        {
            switch (curveType)
            {
                case CurveDataType.Float3:
                    return 1;
                case CurveDataType.Float4:
                    return 4;
                default:
                    throw new NotSupportedException();
            }
        }



        protected virtual void Parse(Stream s, CurveDataInfo info, IList<float> indexedFloats)
        {
            mFrames = new FrameList(handler, info.Flags.Type, s, info, indexedFloats);
        }

        public virtual void UnParse(Stream s, CurveDataInfo info, IList<float> indexedFloats)
        {
            mFrames.UnParse(s, info, indexedFloats);
        }

        public override AHandlerElement Clone(EventHandler handler)
        {
            return (AHandlerElement)new Curve(0, handler, this);
        }

        public virtual IEnumerable<float> SelectFloats()
        {
            var floats = new List<float>();
            foreach (Frame f in mFrames) floats.AddRange(f.GetFloatValues());
            return floats;
        }
    }
}