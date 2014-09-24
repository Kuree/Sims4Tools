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
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.GenericRCOLResource
{
    public class MATD : ARCOLBlock
    {
        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint tag = (uint)FOURCC("MATD");
        uint version = 0x00000103;
        uint materialNameHash;
        ShaderType shader = 0;
        MTRL mtrl = null;// < 0x00000103
        bool isVideoSurface;// >= 0x00000103
        bool isPaintingSurface;// >= 0x00000103
        MTNF mtnf = null;// >= 0x00000103
        #endregion

        #region Constructors
        public MATD(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public MATD(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public MATD(int APIversion, EventHandler handler, MATD basis)
            : base(APIversion, handler, null)
        {
            this.version = basis.version;
            this.materialNameHash = basis.materialNameHash;
            this.shader = basis.shader;
            if (version < 0x00000103)
            {
                this.mtrl = basis.mtrl != null
                    ? new MTRL(requestedApiVersion, OnRCOLChanged, basis.mtrl)
                    : new MTRL(requestedApiVersion, OnRCOLChanged);
            }
            else
            {
                this.isVideoSurface = basis.isVideoSurface;
                this.isPaintingSurface = basis.isPaintingSurface;
                this.mtnf = basis.mtnf != null
                    ? new MTNF(requestedApiVersion, OnRCOLChanged, basis.mtnf)
                    : new MTNF(requestedApiVersion, OnRCOLChanged);
            }
        }

        public MATD(int APIversion, EventHandler handler,
            uint version, uint materialNameHash, ShaderType shader, MTRL mtrl)
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.materialNameHash = materialNameHash;
            this.shader = shader;
            if (checking) if (version >= 0x00000103)
                    throw new ArgumentException("version must be < 0x0103 for MTRLs");
            this.mtrl = mtrl != null
                ? new MTRL(requestedApiVersion, OnRCOLChanged, mtrl)
                : new MTRL(requestedApiVersion, OnRCOLChanged);
        }

        public MATD(int APIversion, EventHandler handler,
            uint version, uint materialNameHash, ShaderType shader, bool isVideoSurface, bool isPaintingSurface, MTNF mtnf)
            : base(APIversion, handler, null)
        {
            this.version = version;
            this.materialNameHash = materialNameHash;
            this.shader = shader;
            if (checking) if (version < 0x00000103)
                    throw new ArgumentException("version must be >= 0x0103 for MTNFs");
            this.isVideoSurface = isVideoSurface;
            this.isPaintingSurface = isPaintingSurface;
            this.mtnf = mtnf != null
                ? new MTNF(requestedApiVersion, OnRCOLChanged, mtnf)
                : new MTNF(requestedApiVersion, OnRCOLChanged);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return "MATD"; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x01D0E75D; } }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            tag = r.ReadUInt32();
            if (checking) if (tag != (uint)FOURCC("MATD"))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: 'MATD'; at 0x{1:X8}", FOURCC(tag), s.Position));
            version = r.ReadUInt32();
            materialNameHash = r.ReadUInt32();
            shader = (ShaderType)r.ReadUInt32();
            uint length = r.ReadUInt32();
            long start;
            if (version < 0x00000103)
            {
                start = s.Position;
                mtrl = new MTRL(requestedApiVersion, OnRCOLChanged, s);
            }
            else
            {
                isVideoSurface = r.ReadInt32() != 0;
                isPaintingSurface = r.ReadInt32() != 0;
                start = s.Position;
                mtnf = new MTNF(requestedApiVersion, OnRCOLChanged, s);
            }

            if (checking) if (start + length != s.Position)
                    throw new InvalidDataException(string.Format("Invalid length 0x{0:X8} at 0x{1:X8}", length, s.Position));
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(tag);
            w.Write(version);
            w.Write(materialNameHash);
            w.Write((uint)shader);
            long lenPos = ms.Position;
            w.Write((uint)0);//length
            long pos;
            if (version < 0x00000103)
            {
                pos = ms.Position;
                if (mtrl == null) mtrl = new MTRL(requestedApiVersion, OnRCOLChanged);
                mtrl.UnParse(ms);
            }
            else
            {
                w.Write(isVideoSurface ? 1 : 0);
                w.Write(isPaintingSurface ? 1 : 0);
                pos = ms.Position;
                if (mtnf == null) mtnf = new MTNF(requestedApiVersion, OnRCOLChanged);
                mtnf.UnParse(ms);
            }

            long endPos = ms.Position;
            ms.Position = lenPos;
            w.Write((uint)(endPos - pos));
            ms.Position = endPos;

            return ms;
        }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                if (version < 0x00000103)
                {
                    res.Remove("IsVideoSurface");
                    res.Remove("IsPaintingSurface");
                    res.Remove("Mtnf");
                }
                else
                {
                    res.Remove("Mtrl");
                }
                return res;
            }
        }
        #endregion

        #region Sub-types
        // At some point AHandlerElement will gain IResource...
        public class MTRL : AHandlerElement, IEquatable<MTRL>, IResource
        {
            const int recommendedApiVersion = 1;

            uint mtrlUnknown1;
            ushort mtrlUnknown2;
            ushort mtrlUnknown3;
            ShaderDataList sdList = null;

            public MTRL(int APIversion, EventHandler handler, MTRL basis)
                : base(APIversion, handler)
            {
                this.mtrlUnknown1 = basis.mtrlUnknown1;
                this.mtrlUnknown2 = basis.mtrlUnknown2;
                this.mtrlUnknown3 = basis.mtrlUnknown3;
                this.sdList = basis.sdList == null ? null : new ShaderDataList(handler, basis.sdList);
            }
            public MTRL(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public MTRL(int APIversion, EventHandler handler) : base(APIversion, handler) { }

            #region Data I/O
            private void Parse(Stream s)
            {
                long start = s.Position;
                BinaryReader r = new BinaryReader(s);
                uint mtrlTag = r.ReadUInt32();
                if (checking) if (mtrlTag != (uint)FOURCC("MTRL"))
                        throw new InvalidDataException(String.Format("Invalid mtrlTag read: '{0}'; expected: 'MTRL'; at 0x{1:X8}", FOURCC(mtrlTag), s.Position));
                mtrlUnknown1 = r.ReadUInt32();
                mtrlUnknown2 = r.ReadUInt16();
                mtrlUnknown3 = r.ReadUInt16();
                this.sdList = new ShaderDataList(handler, s, start, null);
            }

            internal void UnParse(Stream s)
            {
                long start = s.Position;
                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)FOURCC("MTRL"));
                w.Write(mtrlUnknown1);
                w.Write(mtrlUnknown2);
                w.Write(mtrlUnknown3);
                if (sdList == null) sdList = new ShaderDataList(handler);
                sdList.UnParse(s, start);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<MTRL> Members

            public bool Equals(MTRL other) { return mtrlUnknown1 == other.mtrlUnknown1 && mtrlUnknown2 == other.mtrlUnknown2 && mtrlUnknown3 == other.mtrlUnknown3 && sdList == other.sdList; }

            public override bool Equals(object obj)
            {
                return obj as MTRL != null ? this.Equals(obj as MTRL) : false;
            }

            public override int GetHashCode()
            {
                return mtrlUnknown1.GetHashCode() ^ mtrlUnknown2.GetHashCode() ^ mtrlUnknown3.GetHashCode() ^ sdList.GetHashCode();
            }

            #endregion

            #region IResource
            public Stream Stream
            {
                get
                {
                    MemoryStream ms = new MemoryStream();
                    UnParse(ms);
                    ms.Position = 0;
                    return ms;
                }
            }

            public byte[] AsBytes
            {
                get { return ((MemoryStream)Stream).ToArray(); }
                set { MemoryStream ms = new MemoryStream(value); Parse(ms); OnElementChanged(); }
            }

            public event EventHandler ResourceChanged;
            protected override void OnElementChanged()
            {
                dirty = true;
                if (handler != null) handler(this, EventArgs.Empty);
                if (ResourceChanged != null) ResourceChanged(this, EventArgs.Empty);
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint MTRLUnknown1 { get { return mtrlUnknown1; } set { if (mtrlUnknown1 != value) { mtrlUnknown1 = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public ushort MTRLUnknown2 { get { return mtrlUnknown2; } set { if (mtrlUnknown2 != value) { mtrlUnknown2 = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public ushort MTRLUnknown3 { get { return mtrlUnknown3; } set { if (mtrlUnknown3 != value) { mtrlUnknown3 = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public ShaderDataList SData { get { return sdList; } set { if (sdList != value) { sdList = value == null ? null : new ShaderDataList(handler, value); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public uint MaterialNameHash { get { return materialNameHash; } set { if (materialNameHash != value) { materialNameHash = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public ShaderType Shader { get { return shader; } set { if (shader != value) { shader = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public MTRL Mtrl
        {
            get { if (version >= 0x00000103) throw new InvalidOperationException(); return mtrl; }
            set { if (version >= 0x00000103) throw new InvalidOperationException(); if (mtrl != value) { mtrl = new MTRL(requestedApiVersion, handler, mtrl); OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(15)]
        public bool IsVideoSurface
        {
            get { if (version < 0x00000103) throw new InvalidOperationException(); return isVideoSurface; }
            set { if (version < 0x00000103) throw new InvalidOperationException(); if (isVideoSurface != value) { isVideoSurface = value; OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(16)]
        public bool IsPaintingSurface
        {
            get { if (version < 0x00000103) throw new InvalidOperationException(); return isPaintingSurface; }
            set { if (version < 0x00000103) throw new InvalidOperationException(); if (isPaintingSurface != value) { isPaintingSurface = value; OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(17)]
        public MTNF Mtnf
        {
            get { if (version < 0x00000103) throw new InvalidOperationException(); return mtnf; }
            set { if (version < 0x00000103) throw new InvalidOperationException(); if (mtnf != value) { mtnf = new MTNF(requestedApiVersion, handler, mtnf) { RCOLTag = "MATD", }; OnRCOLChanged(this, EventArgs.Empty); } }
        }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }
}
