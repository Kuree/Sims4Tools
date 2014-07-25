using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.Animation.S3CLIP
{
    public class FrameList : DependentList<Frame>
    {
        private readonly CurveDataType mDataType;

        public FrameList(EventHandler handler, CurveDataType type)
            : base(handler)
        {
            mDataType = type;
        }

        public FrameList(EventHandler handler, CurveDataType type, IEnumerable<Frame> ilt)
            : base(handler, ilt)
        {
            mDataType = type;
        }

        public FrameList(EventHandler handler, CurveDataType type, Stream s, CurveDataInfo info, IList<float> floats)
            : base(handler)
        {
            mDataType = type;
            Parse(s, info, floats);
        }

        public CurveDataType DataType
        {
            get { return mDataType; }
        }

        private void Parse(Stream s, CurveDataInfo info, IList<float> floats)
        {
            for (int i = 0; i < info.FrameCount; i++)
            {
                ((IList<Frame>)this).Add(new Frame(0, this.handler, s, info, floats));
            }
        }

        public void UnParse(Stream s, CurveDataInfo info, IList<float> floats)
        {
            info.FrameDataOffset = (uint)s.Position;
            info.FrameCount = Count;
            for (int i = 0; i < Count; i++)
            {
                this[i].UnParse(s, info, floats);
            }
        }

        public override void Add(Type t)
        {
            base.Add((Frame)Activator.CreateInstance(t, mDataType));
        }

        protected virtual Frame CreateElement(Stream s, CurveDataInfo info, IList<float> floats)
        {
            return new Frame(0, handler, s, info, floats);
        }

        protected virtual void WriteElement(Stream s, CurveDataInfo info, IList<float> floats, Frame element)
        {
            element.UnParse(s, info, floats);
        }

        #region Unused

        protected override Frame CreateElement(Stream s)
        {
            throw new NotSupportedException();
        }

        protected override void WriteElement(Stream s, Frame element)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}