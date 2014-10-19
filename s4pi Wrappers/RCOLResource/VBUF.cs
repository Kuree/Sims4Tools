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
    public class VBUF : RCOLChunk
    {
        [ElementPriority(0)]
        public static RCOL.RCOLChunkType RCOLType { get { return RCOL.RCOLChunkType.VBUF; } }

        #region Attributes
        private UInt32 mVersion;
        private FormatFlags mFlags;
        private uint mSwizzleInfo;
        private Byte[] mBuffer;
        #endregion



        public VBUF(int APIversion, EventHandler handler, Stream s, TGIBlock currentTGI) : base(APIversion, handler, s, currentTGI) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            uint tag = r.ReadUInt32();
            if (tag != (uint)FOURCC(RCOLType.ToString())) throw new InvalidDataException(string.Format("Except to read 0x{0:8X}, read 0x{1:8X}", RCOLType, tag));
            mVersion = r.ReadUInt32();
            mFlags = (FormatFlags)r.ReadUInt32();
            mSwizzleInfo = r.ReadUInt32();
            mBuffer = new Byte[s.Length - s.Position];
            s.Read(mBuffer, 0, mBuffer.Length);
        }

        protected internal override void UnParse(Stream s)
        {
            BinaryWriter bw = new BinaryWriter(s);
            bw.Write((UInt32)RCOLType);
            bw.Write(mVersion);
            bw.Write((UInt32)mFlags);
            bw.Write(mSwizzleInfo);
            if (mBuffer == null) mBuffer = new byte[0];
            bw.Write(mBuffer);
        }
        #endregion

        #region Sub Types
        [Flags]
        public enum FormatFlags : uint
        {
            Collapsed = 0x4,
            DifferencedVertices = 0x2,
            Dynamic = 0x1,
            None = 0x0
        }
        #endregion

        #region Content Fields
        [ElementPriority(2)]
        public UInt32 Version { get { return mVersion; } set { if (mVersion != value) { mVersion = value; OnElementChanged(); } } }
        [ElementPriority(3)]
        public FormatFlags Flags { get { return mFlags; } set { if (mFlags != value) { mFlags = value; OnElementChanged(); } } }
        [ElementPriority(4)]
        public uint SwizzleInfo { get { return mSwizzleInfo; } set { if (mSwizzleInfo != value) { mSwizzleInfo = value; OnElementChanged(); } } }
        [ElementPriority(5)]
        public Byte[] Buffer { get { return mBuffer; } set { if (mBuffer != value) { mBuffer = value; OnElementChanged(); } } }
        public string Value { get { return ValueBuilder; } }
        protected override List<string> ValueBuilderFields { get { var fields = base.ValueBuilderFields; fields.Remove("Buffer"); return fields; } }
        #endregion
    }

    public class VBUF2 : VBUF
    {
        [ElementPriority(0)]
        public static new RCOL.RCOLChunkType RCOLType { get { return RCOL.RCOLChunkType.VBUF2; } }

        public VBUF2(int APIversion, EventHandler handler, Stream s, TGIBlock currentTGI)
            : base(APIversion, handler, s, currentTGI)
        {
        }

    }
}