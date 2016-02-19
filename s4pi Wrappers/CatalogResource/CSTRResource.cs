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
    public class CSTRResource : AResource
    {
        #region Attributes
        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        private uint version = 0x0A;
        private CatalogCommon commonA;
        private uint hashIndicator = 0x1;
        private uint hash01 = 0x811C9DC5;
        private uint hash02 = 0x811C9DC5;
        private uint hash03 = 0x811C9DC5;
        private CSTR_references refList;
        private byte unk01;
        private byte unk02;
        private byte unk03;
        private uint materialVariant;
        private ulong swatchGrouping;
        private ColorList colors;
        private byte unk05;
        
        #endregion Attributes =============================================

        #region Content Fields

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "CTProductStair"; }
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
        [ElementPriority(4)]
        public uint HashIndicator
        {
            get { return hashIndicator; }
            set { if (hashIndicator != value) { hashIndicator = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(5)]
        public uint Hash01
        {
            get { return hash01; }
            set { if (hash01 != value) { hash01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public uint Hash02
        {
            get { return hash02; }
            set { if (hash02 != value) { hash02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(7)]
        public uint Hash03
        {
            get { return hash03; }
            set { if (hash03 != value) { hash03 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(8)]
        public CSTR_references ReferenceList
        {
            get { return refList; }
            set { if (!refList.Equals(value)) { refList = new CSTR_references(kRecommendedApiVersion, this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(10)]
        public byte Unk01
        {
            get { return unk01; }
            set { if (unk01 != value) { unk01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(11)]
        public byte Unk02
        {
            get { return unk02; }
            set { if (unk02 != value) { unk02 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(12)]
        public byte Unk03
        {
            get { return unk03; }
            set { if (unk03 != value) { unk03 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(14)]
        public uint MaterialVariant
        {
            get { return materialVariant; }
            set { if (materialVariant != value) { materialVariant = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(15)]
        public ulong SwatchGrouping//UnkIID01
        {
            get { return swatchGrouping; }
            set { if (swatchGrouping != value) { swatchGrouping = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(16)]
        public ColorList Colors
        {
            get { return colors; }
            set { if (!colors.Equals(value)) { colors = new ColorList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(17)]
        public byte Unk05
        {
            get { return unk05; }
            set { if (unk05 != value) { unk05 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
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
            this.hashIndicator = br.ReadUInt32();
            this.hash01 = br.ReadUInt32();
            this.hash02 = br.ReadUInt32();
            this.hash03 = br.ReadUInt32();
            this.refList = new CSTR_references(kRecommendedApiVersion,this.OnResourceChanged,s);
            this.unk01 = br.ReadByte();
            this.unk02 = br.ReadByte();
            this.unk03 = br.ReadByte();
            this.materialVariant = br.ReadUInt32();
            this.swatchGrouping = br.ReadUInt64();
            this.colors = new ColorList(this.OnResourceChanged, s);
            this.unk05 = br.ReadByte();
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            if (this.commonA == null) { this.commonA = new CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged); }
            this.commonA.UnParse(s);
            bw.Write(this.hashIndicator);
            bw.Write(this.hash01);
            bw.Write(this.hash02);
            bw.Write(this.hash03);
            if (this.refList == null) { this.refList = new CSTR_references(kRecommendedApiVersion, this.OnResourceChanged); }
            this.refList.UnParse(s);
            bw.Write(this.unk01);
            bw.Write(this.unk02);
            bw.Write(this.unk03);
            bw.Write(this.materialVariant);
            bw.Write(this.swatchGrouping);
            if (this.colors == null) { this.colors = new ColorList(this.OnResourceChanged); }
            this.colors.UnParse(s);
            bw.Write(this.unk05);
            return s;
        }
        #endregion DataIO ============================================================================

        #region Constructors
        public CSTRResource(int APIversion, Stream s)
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

        public class CSTR_references : AHandlerElement, IEquatable<CSTR_references>
        {
            #region Attributes
            TGIBlock modlRef01;
            TGIBlock modlRef02;
            TGIBlock modlRef03;
            TGIBlock unkRef01;
            TGIBlock wallRef;
            TGIBlock objRef;
            private int kRecommendedApiVersion = 1;
            public override int RecommendedApiVersion { get { return 1; } }
            #endregion Attributes ==================================================

            #region ContentFields
            [ElementPriority(2)]
            public TGIBlock ModlRef01
            {
                get { return modlRef01; }
                set { modlRef01 = value; OnElementChanged(); }
            }
            [ElementPriority(3)]
            public TGIBlock ModlRef02
            {
                get { return modlRef02; }
                set { modlRef02 = value; OnElementChanged(); }
            }
            [ElementPriority(4)]
            public TGIBlock ModlRef03
            {
                get { return modlRef03; }
                set { modlRef03 = value; OnElementChanged(); }
            }
            [ElementPriority(5)]
            public TGIBlock UnkRef01
            {
                get { return unkRef01; }
                set { unkRef01 = value; OnElementChanged(); }
            }
            [ElementPriority(6)]
            public TGIBlock WallRef
            {
                get { return wallRef; }
                set { wallRef = value; OnElementChanged(); }
            }
            [ElementPriority(7)]
            public TGIBlock ObjRef
            {
                get { return objRef; }
                set { objRef = value; OnElementChanged(); }
            }

            public string Value { get { return ValueBuilder; } }

            public override List<string> ContentFields
            {
                get { return GetContentFields(0, GetType()); }
            }

            #endregion ContentFields ==========================================================

            #region Constructors

            public CSTR_references(int apiVersion, EventHandler handler, CSTR_references other)
                : this(apiVersion, handler, other.modlRef01, other.modlRef02, other.modlRef03, other.unkRef01, other.wallRef, other.objRef)
            {
            }
            public CSTR_references(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                this.MakeNew();
            }
            public CSTR_references(int apiVersion, EventHandler handler, Stream s)
                : base(apiVersion, handler)
            {
                this.Parse(s);
            }
            public CSTR_references(int apiVersion, EventHandler handler, TGIBlock modlRef01, TGIBlock modlRef02, TGIBlock modlRef03, TGIBlock unkRef01, TGIBlock wallRef, TGIBlock objRef)
                : base(apiVersion, handler)
            {
                this.modlRef01 = new TGIBlock(kRecommendedApiVersion,handler,TGIBlock.Order.ITG,modlRef01);
                this.modlRef02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, modlRef02);
                this.modlRef03 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, modlRef03);
                this.unkRef01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, unkRef01);
                this.wallRef = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, wallRef);
                this.objRef = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, objRef);
            }
            public bool Equals(CSTR_references other)
            {
                return
                    this.modlRef01 == other.modlRef01 &&
                    this.modlRef02 == other.modlRef02 &&
                    this.modlRef03 == other.modlRef03 &&
                    this.unkRef01 == other.unkRef01 &&
                    this.wallRef == other.wallRef &&
                    this.objRef == other.objRef;
            }

            #endregion Constructors =======================================================

            #region DataIO

            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.modlRef01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
                this.modlRef02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
                this.modlRef03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
                this.unkRef01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
                this.wallRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
                this.objRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            }
            void MakeNew()
            {
                this.modlRef01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
                this.modlRef02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
                this.modlRef03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
                this.unkRef01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
                this.wallRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
                this.objRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                modlRef01.UnParse(s);
                modlRef02.UnParse(s);
                modlRef03.UnParse(s);
                unkRef01.UnParse(s);
                wallRef.UnParse(s);
                objRef.UnParse(s);
            }
            #endregion DataIO

        }
        #endregion subclasses
    }

    /// <summary>
    /// ResourceHandler for CSTRResource wrapper
    /// </summary>
    public class CSTRResourceHandler : AResourceHandler
    {
        public CSTRResourceHandler()
        {
            this.Add(typeof(CSTRResource), new List<string>(new[] { "0x9A20CD1C", }));
        }
    }
}