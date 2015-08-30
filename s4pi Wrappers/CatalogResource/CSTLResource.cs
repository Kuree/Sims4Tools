/***************************************************************************
 *  Copyright (C) 2014 by Inge Jones                                       *
 *                                                                         *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s4pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;

using s4pi.Interfaces;

namespace CatalogResource
{
    public class CSTLResource : AResource
    {
        #region Attributes
        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        private uint version = 0x0E;
        private int cstlRefsSize = 25;
        TGIBlock[] cstlRefs;

        private CatalogCommon commonA;
        private UnkCSTLUintList unkList1;
        private uint unk01;
        private uint unk02;
        private uint unk03;
        private uint unk04;
        private byte unk05;
        private ulong unkIID01;
        
        #endregion Attributes =============================================

        #region Content Fields

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "CTProductStyle"; }
        }
        [ElementPriority(1)]
        public uint Version
        {
            get { return version; }
            set { if (version != value) { version = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(2)]
        public CatalogCommon CommonBlock
        {
            get { return commonA; }
            set { if (!commonA.Equals(value)) { commonA = new CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(30)]
        public TGIBlock[] CSTLRefs
        {
            get { return cstlRefs; }
            set
            {
                if (value.Length != this.cstlRefs.Length) throw new ArgumentLengthException("CSTLRefs", this.cstlRefs.Length);
                if (!cstlRefs.Equals<TGIBlock>(value)) { cstlRefs = value == null ? null : (TGIBlock[])value.Clone(); OnResourceChanged(this, EventArgs.Empty); }
            }
        }
        [ElementPriority(30)]
        public UnkCSTLUintList UnkList1
        {
            get { return unkList1; }
            set { if (!unkList1.Equals(value)) { unkList1 = new UnkCSTLUintList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(31)]
        public uint Unk01
        {
            get { return unk01; }
            set { if (unk01 != value) { unk01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(32)]
        public uint Unk02
        {
            get { return unk02; }
            set { if (unk02 != value) { unk02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(33)]
        public uint Unk03
        {
            get { return unk03; }
            set { if (unk03 != value) { unk03 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(34)]
        public uint Unk04
        {
            get { return unk04; }
            set { if (unk04 != value) { unk04 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(35)]
        public byte Unk05
        {
            get { return unk05; }
            set { if (unk05 != value) { unk05 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(36)]
        public ulong UnkIID01
        {
            get { return unkIID01; }
            set { if (unkIID01 != value) { unkIID01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }

        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                return res;
            }
        }

        #endregion ContentFields

        #region Data I/O

        void Parse(Stream s)
        {

            var br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.commonA = new CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged, s);
            if (this.version == 0x0D)
            { 
                cstlRefsSize = 21;
            }
            cstlRefs = new TGIBlock[cstlRefsSize];
            for (int i = 0; i < cstlRefs.Length; i++)
                cstlRefs[i] = new TGIBlock(requestedApiVersion, OnResourceChanged, TGIBlock.Order.ITG, s);
            this.unkList1 = new UnkCSTLUintList(OnResourceChanged, s);
            this.unk01 = br.ReadUInt32();
            this.unk02 = br.ReadUInt32();
            this.unk03 = br.ReadUInt32();
            this.unk04 = br.ReadUInt32();
            this.unk05 = br.ReadByte();
            this.unkIID01 = br.ReadUInt64();
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            if (this.commonA == null) { this.commonA = new CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged); }
            this.commonA.UnParse(s);

            cstlRefsSize = 25; // default version 0x0E
            if (this.version == 0x0D) // has smaller array
            {
                cstlRefsSize = 21;
            }
            if (cstlRefs == null) // making new resource
            {
                cstlRefs = new TGIBlock[cstlRefsSize];
            }
            if (cstlRefs.Length < cstlRefsSize) // may have changed the version and need larger array
            {
                TGIBlock[] newcstlRefs = new TGIBlock[cstlRefsSize];
                for (int i = 0; i < cstlRefs.Length; i++)
                {
                    newcstlRefs[i] = cstlRefs[i];
                }
                cstlRefs = newcstlRefs;
            }
            for (int i = 0; i < cstlRefsSize; i++) // don't write out more than we need even if we have more
            {
                if (cstlRefs[i] == null) cstlRefs[i] = new TGIBlock(kRecommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG);
                cstlRefs[i].UnParse(s);
            }

            if (this.unkList1 == null) { this.unkList1 = new UnkCSTLUintList(this.OnResourceChanged); }
            this.unkList1.UnParse(s);
            bw.Write(this.unk01);
            bw.Write(this.unk02);
            bw.Write(this.unk03);
            bw.Write(this.unk04);
            bw.Write(this.unk05);
            bw.Write(this.unkIID01);
            return s;
        }
        #endregion DataIO ============================================================================

        #region Constructors
        public CSTLResource(int APIversion, Stream s)
            : base(APIversion, s)
        {

            if (s == null || s.Length == 0)
            {
                s = UnParse();
                OnResourceChanged(this, EventArgs.Empty);
            }
            s.Position = 0;
            this.Parse(s);
        }
        #endregion Constructors

        #region subclasses
        public class UnkCSTLUintList : SimpleList<uint>
        {
            // BYTE count
            // DWORD value
            public UnkCSTLUintList(EventHandler handler, Stream s) : base(handler, s, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }
            public UnkCSTLUintList(EventHandler handler) : base(handler, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }
            public UnkCSTLUintList(EventHandler handler, IEnumerable<UInt32> le) : base(handler, le, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }

            static uint ReadItem(Stream s)
            {
                return new BinaryReader(s).ReadUInt32();
            }
            static void WriteItem(Stream s, uint value)
            {
                new BinaryWriter(s).Write(value);
            }
            static int ReadListCount(Stream s)
            {
                return new BinaryReader(s).ReadByte();
            }
            static void WriteListCount(Stream s, int count)
            {
                byte ncount = Convert.ToByte(count);
                new BinaryWriter(s).Write(ncount);
            }
        }

        #endregion subclasses
    }

    /// <summary>
    /// ResourceHandler for CSTRResource wrapper
    /// </summary>
    public class CSTLResourceHandler : AResourceHandler
    {
        public CSTLResourceHandler()
        {
            this.Add(typeof(CSTLResource), new List<string>(new[] { "0x9F5CFF10" }));
        }
    }
}