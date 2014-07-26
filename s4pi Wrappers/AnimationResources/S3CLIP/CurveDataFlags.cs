using System;

namespace s4pi.Animation.S3CLIP
{
    public struct CurveDataFlags
    {
        private Byte mRaw;

        public CurveDataFlags(byte raw)
        {
            mRaw = raw;
        }


        public CurveDataType Type
        {
            get { return (CurveDataType)((mRaw & 0x07) >> 0); }
            set
            {
                mRaw &= 0x1F;
                mRaw |= (byte)((byte)value << 0);
            }
        }

        public Boolean Static
        {
            get { return ((mRaw & 0x08) >> 3) == 1 ? true : false; }
            set
            {
                mRaw &= 0xF7;
                mRaw |= (byte)((value ? 1 : 0) << 3);
            }
        }

        public CurveDataFormat Format
        {
            get { return (CurveDataFormat)((mRaw & 0xF0) >> 4); }
            set
            {
                mRaw &= 0x0F;
                mRaw |= (byte)(((byte)value) << 4);
            }
        }

        public Byte Raw
        {
            get { return mRaw; }
            set { mRaw = value; }
        }
    }
}