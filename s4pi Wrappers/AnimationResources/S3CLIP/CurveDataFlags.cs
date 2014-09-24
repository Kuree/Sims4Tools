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