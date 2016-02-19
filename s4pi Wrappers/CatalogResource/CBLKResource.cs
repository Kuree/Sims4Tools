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
    public class CBLKResource : AResource
    {
        #region Attributes

        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }
        private uint version = 0x5;
        private CatalogCommon commonA;
        private byte unk01;
        private byte unk02;
        private CBLKEntryList cblkEntries;
        private bool hasCBLKEntries = true;

        #endregion Attributes =============================================================

        #region Content Fields
        
        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "CTProductBlock"; }
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
            set { if (commonA != value) { commonA = new CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(3)]
        public byte Unk01
        {
            get { return unk01; }
            set { if (unk01 != value) { unk01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(4)]
        public byte Unk02
        {
            get { return unk02; }
            set { if (unk02 != value) { unk02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(5)]
        public CBLKEntryList CBLKEntries
        {
            get { return cblkEntries; }
            set { if (cblkEntries != value) { cblkEntries = new CBLKEntryList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }

        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                if (hasCBLKEntries == false) { res.Remove("CBLKEntries"); }
                return res;
            }
        }

        #endregion ContentFields ==========================

        #region Data I/O

        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.commonA = new CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged, s);
            this.unk01 = br.ReadByte();
            this.unk02 = br.ReadByte();
            if (s.Position < (s.Length))
            {
                this.cblkEntries = new CBLKEntryList(this.OnResourceChanged, s); 
                hasCBLKEntries = true;
            }
            else 
            { 
                hasCBLKEntries = false; 
            }
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            if (this.commonA == null) { this.commonA = new CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged); }
            this.commonA.UnParse(s);
            bw.Write(this.unk01);
            bw.Write(this.unk02);
            if (hasCBLKEntries == true)
            {
                if (this.cblkEntries == null) 
                { 
                    this.cblkEntries = new CBLKEntryList(this.OnResourceChanged); 
                }
                this.cblkEntries.UnParse(s);
            }
            return s;
        }
        #endregion  DataIO

        #region Constructors

        public CBLKResource(int APIversion, Stream s)
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

        #endregion

        #region Subclasses

        public class CBLKEntryList : DependentList<CBLKEntry>
        {
            public CBLKEntryList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public CBLKEntryList(EventHandler handler, IEnumerable<CBLKEntry> ilt, long maxSize = -1)
                : base(handler, ilt, maxSize)
            {
            }

            public CBLKEntryList(EventHandler handler, Stream s)
                : base(handler, s)
            {
            }

            protected override CBLKEntry CreateElement(Stream s)
            {
                return new CBLKEntry(kRecommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, CBLKEntry element)
            {
                element.UnParse(s);
            }
        }

        public class CBLKEntry : AHandlerElement, IEquatable<CBLKEntry>
        {
            #region Attributes
            byte byte01;
            byte byte02;
            byte byte03;
            
            public override int RecommendedApiVersion
            {
                get { return kRecommendedApiVersion; }
            }
            #endregion Attributes =========================

            #region ContentFields
            [ElementPriority(1)]
            public byte Byte01
            {
                get { return byte01; }
                set { if (byte01 != value) {byte01 = value; OnElementChanged(); }}
            }
            [ElementPriority(1)]
            public byte Byte02
            {
                get { return byte02; }
                set { if (byte02 != value) {byte02 = value; OnElementChanged(); }}
            }
            [ElementPriority(1)]
            public byte Byte03
            {
                get { return byte03; }
                set { if (byte03 != value) { byte03 = value; OnElementChanged(); } }
            }

            public string Value { get { return ValueBuilder; } }

            public override List<string> ContentFields
            {
                get { return GetContentFields(0, GetType()); }
            }
            #endregion ContentFields

            #region Constructors

            public CBLKEntry(int apiVersion, EventHandler handler, CBLKEntry other)
                : this(apiVersion, handler, other.byte01, other.byte02, other.byte03)
            {
            }
            public CBLKEntry(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                this.MakeNew();
            }
            public CBLKEntry(int apiVersion, EventHandler handler, Stream s)
                : base(apiVersion, handler)
            {
                this.Parse(s);
            }
            public CBLKEntry(int apiVersion, EventHandler handler, byte value01, byte value02, byte value03)
                : base(apiVersion, handler)
            {
                this.byte01 = value01;
                this.byte02 = value02;
                this.byte03 = value03;
            }
            public bool Equals(CBLKEntry other)
            {
                return (
                    this.byte01 == other.byte01)
                    && (this.byte02 == other.byte02)
                    && (this.byte03 == other.byte03);
            }
            #endregion Constructors

            #region DataIO
            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.byte01 = br.ReadByte();
                this.byte02 = br.ReadByte();
                this.byte03 = br.ReadByte();
            }
            void MakeNew()
            {
                this.byte01 = 0;
                this.byte02 = 0;
                this.byte03 = 0;
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(this.byte01);
                bw.Write(this.byte02);
                bw.Write(this.byte03);
            }

            #endregion DataIO

        
        }

        #endregion Subclasses =========================

    }
    /// <summary>
    /// ResourceHandler for MTBLResource wrapper
    /// </summary>
    public class CBLKResourceHandler : AResourceHandler
    {
        public CBLKResourceHandler()
        {
            this.Add(typeof(CBLKResource), new List<string>(new[] { "0x07936CE0", }));
        }
    }
}