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

using CatalogResource.Common;

using s4pi.Interfaces;
using s4pi.Resource.Commons.CatalogTags;

namespace CatalogResource
{
    /// <summary>
    /// for all catalog resources
    /// </summary>
    public class CatalogCommon : AHandlerElement
    {
	    internal const int kRecommendedApiVersion = 1;

        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        #region Attributes

        private uint commonBlockVersion = 0x9;
        private uint nameHash;
        private uint descriptionHash;
        private uint price;
        private uint unkCommon01;
        private uint unkCommon02;
        private uint unkCommon03;
        private CountedTGIBlockList productStyles;
        private UInt16 unkCommon04;
        private CatalogTagList tagList;
        private SellingPointList sellingPoints;
        private uint unlockByHash;
        private uint unlockedByHash;
        private UInt16 unkCommon06;
        private ulong unkCommon07;

        #endregion

        #region Content Fields

        [ElementPriority(1)]
        public uint CommonBlockVersion
        {
            get { return commonBlockVersion; }
            set { if (commonBlockVersion != value) { commonBlockVersion = value; this.OnElementChanged(); } }
        }
        [ElementPriority(2)]
        public uint NameHash
        {
            get { return nameHash; }
            set { if (nameHash != value) { nameHash = value; this.OnElementChanged(); } }
        }
        [ElementPriority(4)]
        public uint DescriptionHash
        {
            get { return descriptionHash; }
            set { if (descriptionHash != value) { descriptionHash = value; this.OnElementChanged(); } }
        }
        [ElementPriority(5)]
        public uint Price
        {
            get { return price; }
            set { if (price != value) { price = value; this.OnElementChanged(); } }
        }
        [ElementPriority(6)]
        public uint UnkCommon01
        {
            get { return unkCommon01; }
            set { if (unkCommon01 != value) { unkCommon01 = value; this.OnElementChanged(); } }
        }
        [ElementPriority(7)]
        public uint UnkCommon02
        {
            get { return unkCommon02; }
            set { if (unkCommon02 != value) { unkCommon02 = value; this.OnElementChanged(); } }
        }
        [ElementPriority(8)]
        public uint UnkCommon03
        {
            get { return unkCommon03; }
            set { if (unkCommon03 != value) { unkCommon03 = value; this.OnElementChanged(); } }
        }
        [ElementPriority(10)]
        public CountedTGIBlockList ProductStyles
        {
            get { return productStyles; }
            set { if (!productStyles.Equals(value)) { productStyles = new CountedTGIBlockList(handler, TGIBlock.Order.ITG, value); this.OnElementChanged(); } }
        }
        [ElementPriority(11)]
        public UInt16 UnkCommon04
        {
            get { return unkCommon04; }
            set { if (this.unkCommon04 != value) { unkCommon04 = value; this.OnElementChanged(); } }
        }
        [ElementPriority(12)]
        public CatalogTagList Tags
        {
            get { return tagList; }
            set { if (!this.tagList.Equals(value)) { tagList = new CatalogTagList(handler, value); this.OnElementChanged(); } }
        }
        [ElementPriority(13)]
        public SellingPointList SellingPoints
        {
            get { return sellingPoints; }
            set { if (!sellingPoints.Equals(value)) { sellingPoints = new SellingPointList(handler, value); this.OnElementChanged(); } }
        }
        [ElementPriority(15)]
        public uint UnlockByHash
        {
            get { return unlockByHash; }
            set { if (unlockByHash != value) { unlockByHash = value; this.OnElementChanged(); } }
        }
        [ElementPriority(16)]
        public uint UnlockedByHash
        {
            get { return unlockedByHash; }
            set { if (unlockedByHash != value) { unlockedByHash = value; this.OnElementChanged(); } }
        }
        [ElementPriority(17)]
        public UInt16 SwatchSubsort
        {
            get { return unkCommon06; }
            set { if (unkCommon06 != value) { unkCommon06 = value; this.OnElementChanged(); } }
        }
        [ElementPriority(18)]
        public ulong UnkCommon07
        {
            get { return unkCommon07; }
            set { if (unkCommon07 != value) { unkCommon07 = value; this.OnElementChanged(); } }
        }

        public string Value { get { return ValueBuilder; } }


        public override List<string> ContentFields
        {
            get { return GetContentFields(0, GetType()); }
        }

        #endregion

        #region Data I/O

        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.commonBlockVersion = br.ReadUInt32();
            this.nameHash = br.ReadUInt32();
            this.descriptionHash = br.ReadUInt32();
            this.price = br.ReadUInt32();
            this.unkCommon01 = br.ReadUInt32();
            this.unkCommon02 = br.ReadUInt32();
            this.unkCommon03 = br.ReadUInt32();
            int tgi_count = br.ReadByte();
            this.productStyles = new CountedTGIBlockList(handler, TGIBlock.Order.ITG, tgi_count, s);
            this.unkCommon04 = br.ReadUInt16();
            this.tagList = new CatalogTagList(handler, s);
            this.sellingPoints = new SellingPointList(handler, s);
            this.unlockByHash = br.ReadUInt32();
            this.unlockedByHash = br.ReadUInt32();
            this.unkCommon06 = br.ReadUInt16();
            this.unkCommon07 = br.ReadUInt64();
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            bw.Write(this.commonBlockVersion);
            bw.Write(this.nameHash);
            bw.Write(this.descriptionHash);
            bw.Write(this.price);
            bw.Write(this.unkCommon01);
            bw.Write(this.unkCommon02);
            bw.Write(this.unkCommon03);
            byte ncount = Convert.ToByte(productStyles.Count);
            bw.Write(ncount);
            this.productStyles.UnParse(s);
            bw.Write(this.unkCommon04);
            this.tagList.UnParse(s);
            this.sellingPoints.UnParse(s);
            bw.Write(this.unlockByHash);
            bw.Write(this.unlockedByHash);
            bw.Write(this.unkCommon06);
            bw.Write(this.unkCommon07);
        }

        public void MakeNew()
        {
            this.commonBlockVersion = 0x9;
            this.nameHash = 0;
            this.descriptionHash = 0;
            this.price = 0;
            this.unkCommon01 = 0;
            this.unkCommon02 = 0;
            this.unkCommon03 = 0;
            int tgi_count = 0;
            this.productStyles = new CountedTGIBlockList(handler, TGIBlock.Order.ITG, tgi_count);
            this.unkCommon04 = 0;
            this.tagList = new CatalogTagList(handler);
            this.sellingPoints = new SellingPointList(handler);
            this.unlockByHash = 0;
            this.unlockedByHash = 0;
            this.unkCommon06 = 0xffff;
            this.unkCommon07 = 0;
        }


        #endregion

        #region Constructors

        public CatalogCommon(int APIversion, EventHandler handler, uint commonBlockVersion, uint nameHash, uint descriptionHash,
            uint price, uint unkCommon01, uint unkCommon02, uint unkCommon03, CountedTGIBlockList productStyles, UInt16 unkCommon04,
            CatalogTagList tagList, SellingPointList sellingPoints, uint unlockByHash, uint unlockedByHash, UInt16 unkCommon06, ulong unkCommon07)
            : base(APIversion, handler)
        {
            this.handler = handler;
            this.commonBlockVersion = commonBlockVersion;
            this.nameHash = nameHash;
            this.descriptionHash = descriptionHash;
            this.price = price;
            this.unkCommon01 = unkCommon01;
            this.unkCommon02 = unkCommon02;
            this.unkCommon03 = unkCommon03;
            this.productStyles = new CountedTGIBlockList (handler,TGIBlock.Order.ITG, productStyles);
            this.unkCommon04 = unkCommon04;
            this.tagList =  new CatalogTagList(handler, tagList);
            this.sellingPoints = new SellingPointList(handler,  sellingPoints);
            this.unlockByHash = unlockByHash;
            this.unlockedByHash = unlockedByHash;
            this.unkCommon06 = unkCommon06;
            this.unkCommon07 = unkCommon07;
        }


        public CatalogCommon(int APIversion, EventHandler handler, CatalogCommon other)
            : this(APIversion, handler, other.commonBlockVersion, other.nameHash, other.descriptionHash, other.price,
                other.unkCommon01, other.unkCommon02, other.unkCommon03, other.productStyles, other.unkCommon04, other.tagList,
                other.sellingPoints, other.unlockByHash, other.unlockedByHash, other.unkCommon06, other.unkCommon07)
        {
        }
        public CatalogCommon(int APIversion, EventHandler handler)
            : base(APIversion, handler)
        {
            this.MakeNew();
        }
        public CatalogCommon(int APIversion, EventHandler handler, Stream s)
            : base(APIversion, handler)
        {
            this.Parse(s);
        }

        public bool Equals(CatalogCommon other)
        {
            return
            this.commonBlockVersion == other.commonBlockVersion &&
            this.nameHash == other.nameHash &&
            this.descriptionHash == other.descriptionHash &&
            this.price == other.price &&
            this.unkCommon01 == other.unkCommon01 &&
            this.unkCommon02 == other.unkCommon02 &&
            this.unkCommon03 == other.unkCommon03 &&
            this.productStyles == other.productStyles &&
            this.unkCommon04 == other.unkCommon04 &&
            this.tagList == other.tagList &&
            this.sellingPoints == other.sellingPoints &&
            this.unlockByHash == other.unlockByHash &&
            this.unlockedByHash == other.unlockedByHash &&
            this.unkCommon06 == other.unkCommon06 &&
            this.unkCommon07 == other.unkCommon07;
        }


        #endregion

        #region Sub-classes

        public class SellingPointList : DependentList<SellingPoint>
        {
            public SellingPointList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public SellingPointList(EventHandler handler, IEnumerable<SellingPoint> ilt, long maxSize = -1)
                : base(handler, ilt, maxSize)
            {
            }

            public SellingPointList(EventHandler handler, Stream s, long maxSize = -1)
                : base(handler, s, maxSize)
            {
            }

            protected override SellingPoint CreateElement(Stream s)
            {
                return new SellingPoint(kRecommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, SellingPoint element)
            {
                element.UnParse(s);
            }
        }

        public class SellingPoint : AHandlerElement, IEquatable<SellingPoint>
        {
            private Tag commodity;

            private int amount;

            #region Constructors

            public SellingPoint(int APIversion, EventHandler handler, SellingPoint other)
                : this(APIversion, handler, other.commodity, other.amount)
            {
            }

            public SellingPoint(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
                this.MakeNew();
            }

            public SellingPoint(int APIversion, EventHandler handler, Stream s)
                : base(APIversion, handler)
            {
                this.Parse(s);
            }

            public SellingPoint(int APIversion, EventHandler handler, Tag commodity, int value)
                : base(APIversion, handler)
            {
                this.commodity = commodity;
                this.amount = value;
            }

            public bool Equals(SellingPoint other)
            {
                return this.commodity == other.commodity && this.amount == other.amount;
            }

            #endregion Constructors =========================================

            #region ContentFields

            [ElementPriority(1)]
            public Tag Commodity
            {
                get { return commodity; }
                set { if (this.commodity != value) { commodity = value; OnElementChanged(); } }
            }
            [ElementPriority(2)]
            public int Amount
            {
                get { return amount; }
                set { if (this.amount != value) { this.amount = value; OnElementChanged(); } }
            }

            public override int RecommendedApiVersion
            {
                get { return kRecommendedApiVersion; }
            }

            public string Value { get { return ValueBuilder; } }

            public override List<string> ContentFields
            {
                get { return GetContentFields(0, GetType()); }
            }

            #endregion ContentFields ========================================

            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
	            this.commodity = CatalogTagRegistry.FetchTag(br.ReadUInt16());
                this.amount = br.ReadInt32();
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(this.commodity);
                bw.Write(this.amount);
            }
            private void MakeNew()
            {
                commodity = new Tag();
                amount = 0;
            }
        }

	    #endregion
    }

	/// <summary>
    /// for any catalogresource
    /// </summary>
    public class ColorList : SimpleList<uint>
    {
        // BYTE count
        // DWORD value
        public ColorList(EventHandler handler, Stream s) : base(handler, s, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }
        public ColorList(EventHandler handler) : base(handler, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }
        public ColorList(EventHandler handler, IEnumerable<UInt32> le) : base(handler, le, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }

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

    // used by cfen and cspn
    public class SpnFenMODLEntryList : DependentList<SpnFenMODLEntry>
    {
        int kRecommendedApiVersion = 1;
        public int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        public SpnFenMODLEntryList(EventHandler handler, Stream s)
            : base(handler, s, UInt32.MaxValue)
        {
        }
        public SpnFenMODLEntryList(EventHandler handler)
            : base(handler, UInt32.MaxValue)
        {
        }
        public SpnFenMODLEntryList(EventHandler handler, IList<SpnFenMODLEntry> le)
            : base(handler, le, UInt32.MaxValue)
        {
        }
        protected override int ReadCount(Stream s)
        {
            return new BinaryReader(s).ReadByte();
        }
        protected override void WriteCount(Stream s, int count)
        {
            byte ncount = Convert.ToByte(count);
            new BinaryWriter(s).Write(ncount);
        }
        protected override void WriteElement(Stream s, SpnFenMODLEntry element)
        {
            element.UnParse(s);
        }
        protected override SpnFenMODLEntry CreateElement(Stream s)
        {
            return new SpnFenMODLEntry(kRecommendedApiVersion, this.elementHandler, s);
        }
    }

    // used by cfen and cspn
    public class SpnFenMODLEntry : AHandlerElement, IEquatable<SpnFenMODLEntry>
    {
        #region Attributes
        UInt16 modlLabel;
        TGIBlock modlRef;
        int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        #endregion Attributes

        #region ContentFields
        [ElementPriority(1)]
        public UInt16 MODLLabel
        {
            get { return modlLabel; }
            set { modlLabel = value; OnElementChanged(); }
        }
        [ElementPriority(2)]
        public TGIBlock MODLRef
        {
            get { return modlRef; }
            set { if (modlRef != value) { modlRef = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value); OnElementChanged(); } }
        }
        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get { return GetContentFields(0, GetType()); }
        }
        #endregion ContentFields ========================================================

        #region Constructors
        public SpnFenMODLEntry(int APIversion, EventHandler handler, SpnFenMODLEntry other)
            : this(APIversion, handler, other.modlLabel, other.modlRef)
        {
        }
        public SpnFenMODLEntry(int APIversion, EventHandler handler)
            : base(APIversion, handler)
        {
            this.MakeNew();
        }
        public SpnFenMODLEntry(int APIversion, EventHandler handler, Stream s)
            : base(APIversion, handler)
        {
            this.Parse(s);
        }
        public SpnFenMODLEntry(int APIversion, EventHandler handler, UInt16 modlLabel, TGIBlock modlRef)
            : base(APIversion, handler)
        {
            this.modlLabel = modlLabel;
            this.modlRef = modlRef = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, modlRef);
        }
        public bool Equals(SpnFenMODLEntry other)
        {
            return (this.modlLabel == other.modlLabel) && (this.modlRef == other.modlRef);
        }
        #endregion Constructors ====================================================================

        #region DataIO
        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.modlLabel = br.ReadUInt16();
            this.modlRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            bw.Write(modlLabel);
            modlRef.UnParse(s);
        }

        private void MakeNew()
        {
            this.modlLabel = 0x0;
            this.modlRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }

        #endregion DataIO ===============================================================

    }

    // used by ccol
    public class Gp4references : AHandlerElement, IEquatable<Gp4references>
    {
        TGIBlock ref01;
        TGIBlock ref02;
        TGIBlock ref03;
        TGIBlock ref04;
        int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        #region ContentFields

        [ElementPriority(2)]
        public TGIBlock Ref01
        {
            get { return ref01; }
            set { if (ref01 != value) { ref01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(3)]
        public TGIBlock Ref02
        {
            get { return ref02; }
            set { if (ref02 != value) { ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(4)]
        public TGIBlock Ref03
        {
            get { return ref03; }
            set { if (ref03 != value) { ref03 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(5)]
        public TGIBlock Ref04
        {
            get { return ref04; }
            set { if (ref04 != value) { ref04 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }

        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get { return GetContentFields(0, GetType()); }
        }

        #endregion ContentFields

        #region Constructors

        public Gp4references(int APIversion, EventHandler handler, Gp4references other)
            : this(APIversion, handler, other.ref01, other.ref02, other.ref03, other.ref04)
        {
        }
        public Gp4references(int APIversion, EventHandler handler)
            : base(APIversion, handler)
        {
            this.MakeNew();
        }
        public Gp4references(int APIversion, EventHandler handler, Stream s)
            : base(APIversion, handler)
        {
            this.Parse(s);
        }
        public Gp4references(int APIversion, EventHandler handler, TGIBlock ref01, TGIBlock ref02, TGIBlock ref03, TGIBlock ref04)
            : base(APIversion, handler)
        {
            this.ref01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref01);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref02);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref03);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref04);
        }
        public bool Equals(Gp4references other)
        {
            return
                this.ref01 == other.ref01 &&
                this.ref02 == other.ref02 &&
                this.ref03 == other.ref03 &&
                this.ref04 == other.ref04;
        }

        #endregion Constructors

        #region DataIO
        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.ref01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            ref01.UnParse(s);
            ref02.UnParse(s);
            ref03.UnParse(s);
            ref04.UnParse(s);
        }

        public void MakeNew()
        {
            this.ref01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }
        #endregion DataIO

    }

    // used by cfen and cspn
    public class Gp7references : AHandlerElement, IEquatable<Gp7references>
    {
        TGIBlock ref01;
        TGIBlock ref02;
        TGIBlock ref03;
        TGIBlock ref04;
        TGIBlock ref05;
        TGIBlock ref06;
        TGIBlock ref07;
        int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        #region ContentFields

        [ElementPriority(2)]
        public TGIBlock Ref01
        {
            get { return ref01; }
            set { if (ref01 != value) { ref01 = new TGIBlock(kRecommendedApiVersion,handler,TGIBlock.Order.ITG,value.ResourceType,value.ResourceGroup,value.Instance) ; OnElementChanged(); } }
        }
        [ElementPriority(3)]
        public TGIBlock Ref02
        {
            get { return ref02; }
            set { if (ref02 != value) { ref02 = new TGIBlock(kRecommendedApiVersion,handler,TGIBlock.Order.ITG,value.ResourceType,value.ResourceGroup,value.Instance) ; OnElementChanged(); } }
        }
        [ElementPriority(4)]
        public TGIBlock Ref03
        {
            get { return ref03; }
            set { if (ref03 != value) { ref03 = new TGIBlock(kRecommendedApiVersion,handler,TGIBlock.Order.ITG,value.ResourceType,value.ResourceGroup,value.Instance) ; OnElementChanged(); } }
        }
        [ElementPriority(5)]
        public TGIBlock Ref04
        {
            get { return ref04; }
            set { if (ref04 != value) { ref04 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(6)]
        public TGIBlock Ref05
        {
            get { return ref05; }
            set { if (ref05 != value) { ref05 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(7)]
        public TGIBlock Ref06
        {
            get { return ref06; }
            set { if (ref06 != value) { ref06 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(8)]
        public TGIBlock Ref07
        {
            get { return ref07; }
            set { if (ref07 != value) { ref07 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }

        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get { return GetContentFields(0, GetType()); }
        }

        #endregion ContentFields

        #region Constructors

        public Gp7references(int APIversion, EventHandler handler, Gp7references other)
            : this(APIversion, handler, other.ref01, other.ref02, other.ref03, other.ref04, other.ref05, other.ref06, other.ref07)
        {
        }
        public Gp7references(int APIversion, EventHandler handler)
            : base(APIversion, handler)
        {
            this.MakeNew();
        }
        public Gp7references(int APIversion, EventHandler handler, Stream s)
            : base(APIversion, handler)
        {
            this.Parse(s);
        }
        public Gp7references(int APIversion, EventHandler handler, TGIBlock ref01, TGIBlock ref02, TGIBlock ref03, TGIBlock ref04, TGIBlock ref05, TGIBlock ref06, TGIBlock ref07)
            : base(APIversion, handler)
        {
            this.ref01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref01);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref02);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref03);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref04);
            this.ref05 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref05);
            this.ref06 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref06);
            this.ref07 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref07);
        }
        public bool Equals(Gp7references other)
        {
            return
                this.ref01 == other.ref01 &&
                this.ref02 == other.ref02 &&
                this.ref03 == other.ref03 &&
                this.ref04 == other.ref04 &&
                this.ref05 == other.ref05 &&
                this.ref06 == other.ref06 &&
                this.ref07 == other.ref07;
        }

        #endregion Constructors

        #region DataIO
        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.ref01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref05 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref06 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref07 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            ref01.UnParse(s);
            ref02.UnParse(s);
            ref03.UnParse(s);
            ref04.UnParse(s);
            ref05.UnParse(s);
            ref06.UnParse(s);
            ref07.UnParse(s);
        }

        public void MakeNew()
        {
            this.ref01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref05 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref06 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref07 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }
        #endregion DataIO

    }

    // used by cral
    public class Gp8references : AHandlerElement, IEquatable<Gp8references>
    {
        TGIBlock ref01;
        TGIBlock ref02;
        TGIBlock ref03;
        TGIBlock ref04;
        TGIBlock ref05;
        TGIBlock ref06;
        TGIBlock ref07;
        TGIBlock ref08;
        int kRecommendedApiVersion = 1;
        bool parsed = false;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        #region ContentFields

        [ElementPriority(2)]
        public TGIBlock Ref01
        {
            get { return ref01; }
            set { if (ref01 != value) { ref01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(3)]
        public TGIBlock Ref02
        {
            get { return ref02; }
            set { if (ref01 != value) { ref01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(4)]
        public TGIBlock Ref03
        {
            get { return ref03; }
            set { if (ref03 != value) { ref03 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(5)]
        public TGIBlock Ref04
        {
            get { return ref04; }
            set { if (ref04 != value) { ref04 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(6)]
        public TGIBlock Ref05
        {
            get { return ref05; }
            set { if (ref05 != value) { ref05 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(7)]
        public TGIBlock Ref06
        {
            get { return ref06; }
            set { if (ref06 != value) { ref06 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(7)]
        public TGIBlock Ref07
        {
            get { return ref07; }
            set { if (ref07 != value) { ref07 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(7)]
        public TGIBlock Ref08
        {
            get { return ref08; }
            set { if (ref08 != value) { ref08 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }

        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get { return GetContentFields(0, GetType()); }
        }

        #endregion ContentFields ==================================================================

        #region Constructors

        public Gp8references(int APIversion, EventHandler handler, Gp8references other)
            : this(APIversion, handler, other.ref01, other.ref02, other.ref03, other.ref04, other.ref05, other.ref06, other.ref07, other.ref08)
        {
        }
        public Gp8references(int APIversion, EventHandler handler)
            : base(APIversion, handler)
        {
            this.MakeNew();
        }
        public Gp8references(int APIversion, EventHandler handler, Stream s)
            : base(APIversion, handler)
        {
            this.Parse(s);
        }
        public Gp8references(int APIversion, EventHandler handler, TGIBlock ref01, TGIBlock ref02, TGIBlock ref03, TGIBlock ref04, TGIBlock ref05, TGIBlock ref06, TGIBlock ref07, TGIBlock ref08)
            : base(APIversion, handler)
        {
            this.ref01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref01);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref02);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref03);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref04);
            this.ref05 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref05);
            this.ref06 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref06);
            this.ref07 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref07);
            this.ref08 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref08);
        }
        public bool Equals(Gp8references other)
        {
            return
                this.ref01 == other.ref01 &&
                this.ref02 == other.ref02 &&
                this.ref03 == other.ref03 &&
                this.ref04 == other.ref04 &&
                this.ref05 == other.ref05 &&
                this.ref06 == other.ref06 &&
                this.ref07 == other.ref07 &&
                this.ref08 == other.ref08;
        }

        #endregion Constructors ===========================================================

        #region DataIO

        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.ref01 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
            this.ref02 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
            this.ref03 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
            this.ref04 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
            this.ref05 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
            this.ref06 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
            this.ref07 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
            this.ref08 = new TGIBlock(1, null, TGIBlock.Order.ITG, s);
            parsed = true;
        }

        private void MakeNew()
        {
            this.ref01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref05 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref06 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref07 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref08 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }

        public void UnParse(Stream s)
        {
            if (parsed == false) { MakeNew(); }
            var bw = new BinaryWriter(s);
            ref01.UnParse(s);
            ref02.UnParse(s);
            ref03.UnParse(s);
            ref04.UnParse(s);
            ref05.UnParse(s);
            ref06.UnParse(s);
            ref07.UnParse(s);
            ref08.UnParse(s);
        }

        #endregion DataIO

    }

    // used by ccol
    public class Gp9references : AHandlerElement, IEquatable<Gp9references>
    {
        TGIBlock ref01;
        TGIBlock ref02;
        TGIBlock ref03;
        TGIBlock ref04;
        TGIBlock ref05;
        TGIBlock ref06;
        TGIBlock ref07;
        TGIBlock ref08;
        TGIBlock ref09;
        int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        #region ContentFields

        [ElementPriority(2)]
        public TGIBlock Ref01
        {
            get { return ref01; }
            set { if (ref01 != value) { ref01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(3)]
        public TGIBlock Ref02
        {
            get { return ref02; }
            set { if (ref02 != value) { ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(4)]
        public TGIBlock Ref03
        {
            get { return ref03; }
            set { if (ref03 != value) { ref03 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(5)]
        public TGIBlock Ref04
        {
            get { return ref04; }
            set { if (ref04 != value) { ref04 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(6)]
        public TGIBlock Ref05
        {
            get { return ref05; }
            set { if (ref05 != value) { ref05 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(7)]
        public TGIBlock Ref06
        {
            get { return ref06; }
            set { if (ref06 != value) { ref06 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(8)]
        public TGIBlock Ref07
        {
            get { return ref07; }
            set { if (ref07 != value) { ref07 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(9)]
        public TGIBlock Ref08
        {
            get { return ref08; }
            set { if (ref08 != value) { ref08 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }
        [ElementPriority(10)]
        public TGIBlock Ref09
        {
            get { return ref09; }
            set { if (ref09 != value) { ref09 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, value.ResourceType, value.ResourceGroup, value.Instance); OnElementChanged(); } }
        }

        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get { return GetContentFields(0, GetType()); }
        }

        #endregion ContentFields

        #region Constructors

        public Gp9references(int APIversion, EventHandler handler, Gp9references other)
            : this(APIversion, handler, other.ref01, other.ref02, other.ref03, other.ref04, other.ref05, other.ref06, other.ref07, other.ref08, other.ref09)
        {
        }
        public Gp9references(int APIversion, EventHandler handler)
            : base(APIversion, handler)
        {
            this.MakeNew();
        }
        public Gp9references(int APIversion, EventHandler handler, Stream s)
            : base(APIversion, handler)
        {
            this.Parse(s);
        }
        public Gp9references(int APIversion, EventHandler handler, TGIBlock ref01, TGIBlock ref02, TGIBlock ref03, TGIBlock ref04, TGIBlock ref05, TGIBlock ref06, TGIBlock ref07, TGIBlock ref08, TGIBlock ref09)
            : base(APIversion, handler)
        {
            this.ref01 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref01);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref02);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref03);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref04);
            this.ref05 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref05);
            this.ref06 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref06);
            this.ref07 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref07); 
            this.ref08 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref08);
            this.ref09 = new TGIBlock(kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref09);
        }
        public bool Equals(Gp9references other)
        {
            return
                this.ref01 == other.ref01 &&
                this.ref02 == other.ref02 &&
                this.ref03 == other.ref03 &&
                this.ref04 == other.ref04 &&
                this.ref05 == other.ref05 &&
                this.ref06 == other.ref06 &&
                this.ref07 == other.ref07 &&
                this.ref08 == other.ref08 &&
                this.ref09 == other.ref09;
        }

        #endregion Constructors

        #region DataIO
        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.ref01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref05 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref06 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref07 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref08 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref09 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            ref01.UnParse(s);
            ref02.UnParse(s);
            ref03.UnParse(s);
            ref04.UnParse(s);
            ref05.UnParse(s);
            ref06.UnParse(s);
            ref07.UnParse(s);
            ref08.UnParse(s);
            ref09.UnParse(s);
        }

        public void MakeNew()
        {
            this.ref01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref04 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref05 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref06 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref07 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref08 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref09 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }

        #endregion DataIO

    }
}