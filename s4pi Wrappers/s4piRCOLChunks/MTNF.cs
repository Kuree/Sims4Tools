/***************************************************************************
 *  Copyright (C) 2012 by Peter L Jones                                    *
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
    public class MTNF : AHandlerElement, IEquatable<MTNF>, IResource
    {
        static bool checking = s4pi.Settings.Settings.Checking;

        const int recommendedApiVersion = 1;

        DependentList<TGIBlock> _ParentTGIBlocks;
        public DependentList<TGIBlock> ParentTGIBlocks
        {
            get { return _ParentTGIBlocks; }
            set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; if (sdList != null) sdList.ParentTGIBlocks = _ParentTGIBlocks; } }
        }

        protected string _RCOLTag = "MATD"; // Unless otherwise set by GEOM
        public string RCOLTag
        {
            get { return _RCOLTag; }
            set { if (_RCOLTag != value) { _RCOLTag = value; if (sdList != null) sdList.RCOLTag = _RCOLTag; } }
        }

        #region Attributes
        uint mtnfUnknown1;
        ShaderDataList sdList = null;
        #endregion

        public MTNF(int APIversion, EventHandler handler, MTNF basis)
            : base(APIversion, handler)
        {
            this.mtnfUnknown1 = basis.mtnfUnknown1;
            this.sdList = basis.sdList == null ? null : new ShaderDataList(handler, basis.sdList, basis._ParentTGIBlocks, basis._RCOLTag);
        }
        public MTNF(int APIversion, EventHandler handler, Stream s, string _RCOLTag = "MATD") : base(APIversion, handler) { this._RCOLTag = _RCOLTag; Parse(s); }
        public MTNF(int APIversion, EventHandler handler, string _RCOLTag = "MATD") : base(APIversion, handler) { this._RCOLTag = _RCOLTag; }

        #region Data I/O
        private void Parse(Stream s)
        {
            long start = s.Position;
            BinaryReader r = new BinaryReader(s);
            uint mtnfTag = r.ReadUInt32();
            if (checking) if (mtnfTag != (uint)FOURCC("MTNF") && mtnfTag != (uint)FOURCC("MTRL"))
                    throw new InvalidDataException(String.Format("Invalid mtnfTag read: '{0}'; expected: 'MTNF' or 'MTRL'; at 0x{1:X8}", FOURCC(mtnfTag), s.Position));
            mtnfUnknown1 = r.ReadUInt32();
            this.sdList = new ShaderDataList(handler, s, start, r.ReadInt32(), _ParentTGIBlocks, _RCOLTag);
        }

        internal void UnParse(Stream s)
        {
            long start = s.Position;
            BinaryWriter w = new BinaryWriter(s);
            w.Write((uint)FOURCC("MTNF"));
            w.Write(mtnfUnknown1);
            long dlPos = s.Position;
            w.Write((uint)0);//data length
            if (sdList == null) sdList = new ShaderDataList(handler, _ParentTGIBlocks, _RCOLTag);
            sdList.UnParse(s, start);

            long dlEnd = s.Position;
            s.Position = dlPos;
            w.Write((uint)(dlEnd - sdList.dataPos));
            s.Position = dlEnd;
        }
        #endregion

        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields
        {
            get
            {
                List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                res.Remove("ParentTGIBlocks");
                res.Remove("RCOLTag");
                return res;
            }
        }
        #endregion

        #region IEquatable<MTNF> Members

        public bool Equals(MTNF other) { return mtnfUnknown1 == other.mtnfUnknown1 && sdList == other.sdList; }
        public override bool Equals(object obj)
        {
            return obj as MTNF != null ? this.Equals(obj as MTNF) : false;
        }
        public override int GetHashCode()
        {
            return mtnfUnknown1.GetHashCode() ^ sdList.GetHashCode();
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
        public uint MTNFUnknown1 { get { return mtnfUnknown1; } set { if (mtnfUnknown1 != value) { mtnfUnknown1 = value; OnElementChanged(); } } }
        [ElementPriority(2)]
        public ShaderDataList SData
        {
            get { return sdList; }
            set
            {
                if (sdList != value)
                {
                    sdList = value == null ? value : new ShaderDataList(handler, value, _ParentTGIBlocks, _RCOLTag);
                    OnElementChanged();
                }
            }
        }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }
}