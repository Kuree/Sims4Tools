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
    public class CWALResource : AResource
    {
        #region Attributes
        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        private uint version = 0x7;
        private CatalogCommon commonA;
        private WallMATDEntryList matdList;
        private WallImgGroupList imgGroupList;
        private uint unk01;
        private ColorList colors;
        private ulong unkIID01;

        #endregion Attributes =============================================================

        #region Content Fields

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "CTProductWallPattern"; }
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

        [ElementPriority(20)]
        public WallMATDEntryList MaterialList
        {
            get { return matdList; }
            set { if (matdList != value) { matdList = new WallMATDEntryList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(22)]
        public WallImgGroupList CornerTextures
        {
            get { return imgGroupList; }
            set { if (imgGroupList != value) { imgGroupList = new WallImgGroupList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(23)]
        public uint CorneringFactor
        {
            get { return unk01; }
            set { if (unk01 != value) { unk01 = value; this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(24)]
        public ColorList Colors
        {
            get { return colors; }
            set { if (colors != value) { colors = new ColorList(this.OnResourceChanged, value); this.OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(25)]
        public ulong SwatchGrouping//UnkIID01
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
            this.matdList = new WallMATDEntryList(this.OnResourceChanged, s);
            this.imgGroupList = new WallImgGroupList(this.OnResourceChanged, s);
            this.unk01 = br.ReadUInt32();
            this.colors = new ColorList(this.OnResourceChanged, s);
            this.unkIID01 = br.ReadUInt64();
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            if (this.commonA == null) { this.commonA = new CatalogCommon(kRecommendedApiVersion, this.OnResourceChanged); }
            this.commonA.UnParse(s);
            if (this.matdList == null) { this.matdList = new WallMATDEntryList(this.OnResourceChanged); }
            this.matdList.UnParse(s);
            if (this.imgGroupList == null) { this.imgGroupList = new WallImgGroupList(this.OnResourceChanged); }
            this.imgGroupList.UnParse(s);
            bw.Write(this.unk01);
            if (this.colors == null) { this.colors = new ColorList(this.OnResourceChanged); }
            this.colors.UnParse(s);
            bw.Write(this.unkIID01);
            return s;
        }
        #endregion  DataIO

        #region Constructors
        public CWALResource(int APIversion, Stream s)
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

        public class WallMATDEntryList : DependentList<WallMATDEntry>
        {
            public WallMATDEntryList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public WallMATDEntryList(EventHandler handler, IEnumerable<WallMATDEntry> ilt, long maxSize = -1)
                : base(handler, ilt, maxSize)
            {
            }

            public WallMATDEntryList(EventHandler handler, Stream s)
                : base(handler, s)
            {
            }

            protected override WallMATDEntry CreateElement(Stream s)
            {
                return new WallMATDEntry(kRecommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, WallMATDEntry element)
            {
                element.UnParse(s);
            }
        }

        public class WallMATDEntry : AHandlerElement, IEquatable<WallMATDEntry>
        {
            #region Attributes
            MainWallHeight matdLabel;
            TGIBlock matdRef;
            
            public override int RecommendedApiVersion
            {
                get { return kRecommendedApiVersion; }
            }
            #endregion Attributes =========================

            #region ContentFields
            [ElementPriority(1)]
            public MainWallHeight MATDLabel
            {
                get { return matdLabel; }
                set { matdLabel = value; OnElementChanged(); }
            }
            [ElementPriority(2)]
            public TGIBlock MATDRef
            {
                get { return matdRef; }
                set { matdRef = value; OnElementChanged(); }
            }
            public string Value { get { return ValueBuilder; } }

            public override List<string> ContentFields
            {
                get { return GetContentFields(0, GetType()); }
            }
            #endregion ContentFields

            #region Constructors

            public WallMATDEntry(int apiVersion, EventHandler handler, WallMATDEntry other)
                : this(apiVersion, handler, other.matdLabel, other.matdRef)
            {
            }
            public WallMATDEntry(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                this.MakeNew();
            }
            public WallMATDEntry(int apiVersion, EventHandler handler, Stream s)
                : base(apiVersion, handler)
            {
                this.Parse(s);
            }
            public WallMATDEntry(int apiVersion, EventHandler handler, MainWallHeight matdLabel, TGIBlock matdRef)
                : base(apiVersion, handler)
            {
                this.matdLabel = matdLabel;
                this.matdRef = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, matdRef);
            }
            public bool Equals(WallMATDEntry other)
            {
                return (this.matdLabel == other.matdLabel) && (this.matdRef == other.matdRef);
            }
            #endregion Constructors

            #region DataIO
            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.matdLabel = (MainWallHeight)br.ReadByte();
                this.matdRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            }
            void MakeNew()
            {
                this.matdLabel = (MainWallHeight)0x0;
                this.matdRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(Convert.ToByte(this.matdLabel));
                if (this.matdRef == null) { this.matdRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG); }
                this.matdRef.UnParse(s);
            }
            #endregion DataIO

            #region subclasses

            public enum MainWallHeight : byte
            {
                ShortWall = 0x03,
                MediumWall = 0x04,
                TallWall = 0x05,
            }

            #endregion subclasses
        
        }

        public class WallImgGroupList : DependentList<WallImgGroupEntry>
        {
            public WallImgGroupList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public WallImgGroupList(EventHandler handler, IEnumerable<WallImgGroupEntry> ilt, long maxSize = -1)
                : base(handler, ilt, maxSize)
            {
            }

            public WallImgGroupList(EventHandler handler, Stream s)
                : base(handler, s)
            {
            }

            protected override WallImgGroupEntry CreateElement(Stream s)
            {
                return new WallImgGroupEntry(kRecommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, WallImgGroupEntry element)
            {
                element.UnParse(s);
            }
        }

        public class WallImgGroupEntry : AHandlerElement, IEquatable<WallImgGroupEntry>
        {
            #region Attributes
            CornerWallHeight imgGroupType;
            TGIBlock imgRef01;
            TGIBlock imgRef02;
            TGIBlock imgRef03;

            public override int RecommendedApiVersion { get { return 1; } }
            #endregion Attributes

            #region ContentFields

            [ElementPriority(1)]
            public CornerWallHeight ImgGroupType
            {
                get { return imgGroupType; }
                set { if (imgGroupType != value) { imgGroupType = value; OnElementChanged(); } }
            }
            [ElementPriority(3)]
            public TGIBlock DiffuseMap //ImgRef01
            {
                get { return imgRef01; }
                set { if (imgRef01 != value) { imgRef01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value); OnElementChanged(); } }
            }

            [ElementPriority(4)]
            public TGIBlock BumpMap //ImgRef02
            {
                get { return imgRef02; }
                set { if (imgRef02 != value) { imgRef02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value); OnElementChanged(); } }
            }

            [ElementPriority(5)]
            public TGIBlock SpecularMap //ImgRef03
            {
                get { return imgRef03; }
                set { if (imgRef03 != value) { imgRef03 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value); OnElementChanged(); } }
            }


            public string Value { get { return ValueBuilder; } }

            public override List<string> ContentFields
            {
                get { return GetContentFields(0, GetType()); }
            }

            #endregion ContentFields

            #region Constructors
            public WallImgGroupEntry(int apiVersion, EventHandler handler, WallImgGroupEntry other)
                : this(apiVersion, handler, other.imgGroupType, other.imgRef01, other.imgRef02, other.imgRef03)
            {
            }
            public WallImgGroupEntry(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                this.MakeNew();
            }
            public WallImgGroupEntry(int apiVersion, EventHandler handler, Stream s)
                : base(apiVersion, handler)
            {
                this.Parse(s);
            }
            public WallImgGroupEntry(int apiVersion, EventHandler handler, CornerWallHeight imgGroupType, TGIBlock imgRef01, TGIBlock imgRef02, TGIBlock imgRef03)
                : base(apiVersion, handler)
            {
                this.imgGroupType = imgGroupType;
                this.imgRef01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, imgRef01);
                this.imgRef02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, imgRef02);
                this.imgRef03 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, imgRef03);
            }
            public bool Equals(WallImgGroupEntry other)
            {
                return this.imgGroupType == other.imgGroupType &&
                    this.imgRef01 == other.imgRef01 &&
                    this.imgRef02 == other.imgRef02 &&
                    this.imgRef03 == other.imgRef03;
            }

            #endregion Constructors ============================================

            #region DataIO
            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.imgGroupType = (CornerWallHeight)br.ReadByte();
                this.imgRef01 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
                this.imgRef02 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
                this.imgRef03 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
            }
            void MakeNew()
            {
                this.imgGroupType = (CornerWallHeight)0x0;
                this.imgRef01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
                this.imgRef02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
                this.imgRef03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(Convert.ToByte(imgGroupType));
                if (imgRef01 == null) { this.imgRef01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG); }
                imgRef01.UnParse(s);
                if (imgRef02 == null) { this.imgRef02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG); }
                imgRef02.UnParse(s);
                if (imgRef03 == null) { this.imgRef03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG); }
                imgRef03.UnParse(s);
            }
            #endregion DataIO ===============================

            #region subclasses

            public enum CornerWallHeight : byte
            {
                ShortWall = 0xC3,
                MediumWall = 0xC4,
                TallWall = 0xC5,
            }

            #endregion subclasses
        }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for MTBLResource wrapper
    /// </summary>
    public class CWALResourceHandler : AResourceHandler
    {
        public CWALResourceHandler()
        {
            this.Add(typeof(CWALResource), new List<string>(new[] { "0xD5F0F921" }));
        }
    }
}