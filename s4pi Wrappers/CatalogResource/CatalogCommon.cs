/***************************************************************************
 *  Copyright (C) 2009, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Inge Jones                                                             *
 *  Buzzler                                                                *
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

        public override int RecommendedApiVersion
        {
            get { return CatalogCommon.kRecommendedApiVersion; }
        }

        #region Attributes

        private uint commonBlockVersion = 0x9;
        private uint nameHash;
        private uint descriptionHash;
        private uint price;
        private ulong thumbnailHash;
        private uint unkCommon02;
        private uint devCategoryFlags;
        private CountedTGIBlockList productStyles;
        private short packId;
        private PackFlag packFlags;
        private byte[] reservedBytes; // 9 bytes always 0
        private byte unused2 = 1, unused3;
        private CatalogTagList tagList;
        private SellingPointList sellingPoints;
        private uint unlockByHash;
        private uint unlockedByHash;
        private ushort swatchColorsSortPriority;
        private ulong varientThumbImageHash;

        #endregion

        #region Content Fields

        [ElementPriority(1)]
        public uint CommonBlockVersion
        {
            get { return this.commonBlockVersion; }
            set
            {
                if (this.commonBlockVersion != value)
                {
                    this.commonBlockVersion = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(2)]
        public uint NameHash
        {
            get { return this.nameHash; }
            set
            {
                if (this.nameHash != value)
                {
                    this.nameHash = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(4)]
        public uint DescriptionHash
        {
            get { return this.descriptionHash; }
            set
            {
                if (this.descriptionHash != value)
                {
                    this.descriptionHash = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(5)]
        public uint Price
        {
            get { return this.price; }
            set
            {
                if (this.price != value)
                {
                    this.price = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(6)]
        public ulong ThumbnailHash
        {
            get { return this.thumbnailHash; }
            set
            {
                if (this.thumbnailHash != value)
                {
                    this.thumbnailHash = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(8)]
        public uint DevCategoryFlags
        {
            get { return this.devCategoryFlags; }
            set
            {
                if (this.devCategoryFlags != value)
                {
                    this.devCategoryFlags = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(10)]
        public CountedTGIBlockList ProductStyles
        {
            get { return this.productStyles; }
            set
            {
                if (!this.productStyles.Equals(value))
                {
                    this.productStyles = new CountedTGIBlockList(this.handler, TGIBlock.Order.ITG, value);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(11)]
        public short PackId
        {
            get { return this.packId; }
            set
            {
                if (this.packId != value)
                {
                    this.packId = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(12)]
        public PackFlag PackFlags
        {
            get { return this.packFlags; }
            set
            {
                if (this.packFlags != value)
                {
                    this.packFlags = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(13)]
        public byte[] ReservedBytes
        {
            get { return this.reservedBytes; }
            set
            {
                this.reservedBytes = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(14)]
        public byte Unused2
        {
            get { return this.unused2; }
            set
            {
                this.unused2 = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(15)]
        public byte Unused3
        {
            get { return this.unused3; }
            set
            {
                this.unused3 = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(16)]
        public CatalogTagList Tags
        {
            get { return this.tagList; }
            set
            {
                if (!this.tagList.Equals(value))
                {
                    this.tagList = new CatalogTagList(this.handler, value);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(17)]
        public SellingPointList SellingPoints
        {
            get { return this.sellingPoints; }
            set
            {
                if (!this.sellingPoints.Equals(value))
                {
                    this.sellingPoints = new SellingPointList(this.handler, value);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(18)]
        public uint UnlockByHash
        {
            get { return this.unlockByHash; }
            set
            {
                if (this.unlockByHash != value)
                {
                    this.unlockByHash = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(19)]
        public uint UnlockedByHash
        {
            get { return this.unlockedByHash; }
            set
            {
                if (this.unlockedByHash != value)
                {
                    this.unlockedByHash = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(20)]
        public ushort SwatchColorsSortPriority
        {
            get { return this.swatchColorsSortPriority; }
            set
            {
                if (this.swatchColorsSortPriority != value)
                {
                    this.swatchColorsSortPriority = value;
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(21)]
        public ulong VarientThumbImageHash
        {
            get { return this.varientThumbImageHash; }
            set
            {
                if (this.varientThumbImageHash != value)
                {
                    this.varientThumbImageHash = value;
                    this.OnElementChanged();
                }
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }

        public override List<string> ContentFields
        {
            get
            {
                var res = GetContentFields(this.RecommendedApiVersion, this.GetType());
                if (this.CommonBlockVersion < 10)
                {
                    res.Remove("PackId");
                    res.Remove("PackFlags");
                    res.Remove("ReservedBytes");
                }
                else
                {
                    res.Remove("Unused2");
                    res.Remove("Unused3");
                }
                return res;
            }
        }

        #endregion

        #region Data I/O

        private void Parse(Stream s)
        {
            var reader = new BinaryReader(s);
            this.commonBlockVersion = reader.ReadUInt32();
            this.nameHash = reader.ReadUInt32();
            this.descriptionHash = reader.ReadUInt32();
            this.price = reader.ReadUInt32();
            this.thumbnailHash = reader.ReadUInt64();
            this.devCategoryFlags = reader.ReadUInt32();
            int tgi_count = reader.ReadByte();
            this.productStyles = new CountedTGIBlockList(this.handler, TGIBlock.Order.ITG, tgi_count, s);

            if (this.CommonBlockVersion >= 10)
            {
                this.packId = reader.ReadInt16();
                this.packFlags = (PackFlag)reader.ReadByte();
                this.reservedBytes = reader.ReadBytes(9);
            }
            else
            {
                this.unused2 = reader.ReadByte();
                if (this.unused2 > 0)
                {
                    this.unused3 = reader.ReadByte();
                }
            }

            if (this.commonBlockVersion >= 11)
            {
                this.tagList = new CatalogTagList(this.handler, s);
            }
            else
            {
                this.tagList = new CatalogTagList(this.handler);
                uint count = reader.ReadUInt32();
                for (int i = 0; i < count; i++)
                {
                    uint tagval = reader.ReadUInt16();
                    this.tagList.Add(new CatalogTag(this.RecommendedApiVersion, this.handler, tagval));
                }
            }

            this.sellingPoints = new SellingPointList(this.handler, s);
            this.unlockByHash = reader.ReadUInt32();
            this.unlockedByHash = reader.ReadUInt32();
            this.swatchColorsSortPriority = reader.ReadUInt16();
            this.varientThumbImageHash = reader.ReadUInt64();
        }

        public void UnParse(Stream s)
        {
            var writer = new BinaryWriter(s);
            writer.Write(this.commonBlockVersion);
            writer.Write(this.nameHash);
            writer.Write(this.descriptionHash);
            writer.Write(this.price);
            writer.Write(this.thumbnailHash);
            writer.Write(this.devCategoryFlags);
            byte ncount = Convert.ToByte(this.productStyles.Count);
            writer.Write(ncount);
            this.productStyles.UnParse(s);

            if (this.CommonBlockVersion >= 10)
            {
                writer.Write(this.packId);
                writer.Write((byte)this.packFlags);
                if (this.reservedBytes == null)
                {
                    this.reservedBytes = new byte[9];
                }
                writer.Write(this.reservedBytes);
            }
            else
            {
                writer.Write(this.unused2);
                if (this.unused2 > 0)
                {
                    writer.Write(this.unused3);
                }
            }

            if (this.commonBlockVersion >= 11)
            {
                this.tagList.UnParse(s);
            }
            else
            {
                writer.Write(this.tagList.Count);
                for (int i = 0; i < this.tagList.Count; i++)
                {
                    writer.Write((ushort)this.tagList[i].Tag.Index);
                }
            }

            this.sellingPoints.UnParse(s);
            writer.Write(this.unlockByHash);
            writer.Write(this.unlockedByHash);
            writer.Write(this.swatchColorsSortPriority);
            writer.Write(this.varientThumbImageHash);
        }

        public void MakeNew()
        {
            this.commonBlockVersion = 0x9;
            this.nameHash = 0;
            this.descriptionHash = 0;
            this.price = 0;
            this.thumbnailHash = 0;
            this.unkCommon02 = 0;
            this.devCategoryFlags = 0;
            int tgi_count = 0;
            this.productStyles = new CountedTGIBlockList(this.handler, TGIBlock.Order.ITG, tgi_count);
            this.packId = 0;
            this.tagList = new CatalogTagList(this.handler);
            this.sellingPoints = new SellingPointList(this.handler);
            this.unlockByHash = 0;
            this.unlockedByHash = 0;
            this.swatchColorsSortPriority = 0xffff;
            this.varientThumbImageHash = 0;
        }

        #endregion

        #region Constructors

        public CatalogCommon(int apiVersion,
                             EventHandler handler,
                             uint commonBlockVersion,
                             uint nameHash,
                             uint descriptionHash,
                             uint price,
                             ulong thumbnailHash,
                             uint unkCommon02,
                             uint devCategoryFlags,
                             CountedTGIBlockList productStyles,
                             short packId,
                             CatalogTagList tagList,
                             SellingPointList sellingPoints,
                             uint unlockByHash,
                             uint unlockedByHash,
                             ushort swatchColorsSortPriority,
                             ulong varientThumbImageHash)
            : base(apiVersion, handler)
        {
            this.handler = handler;
            this.commonBlockVersion = commonBlockVersion;
            this.nameHash = nameHash;
            this.descriptionHash = descriptionHash;
            this.price = price;
            this.thumbnailHash = thumbnailHash;
            this.unkCommon02 = unkCommon02;
            this.devCategoryFlags = devCategoryFlags;
            this.productStyles = new CountedTGIBlockList(handler, TGIBlock.Order.ITG, productStyles);
            this.packId = packId;
            this.tagList = new CatalogTagList(handler, tagList);
            this.sellingPoints = new SellingPointList(handler, sellingPoints);
            this.unlockByHash = unlockByHash;
            this.unlockedByHash = unlockedByHash;
            this.swatchColorsSortPriority = swatchColorsSortPriority;
            this.varientThumbImageHash = varientThumbImageHash;
        }


        public CatalogCommon(int apiVersion, EventHandler handler, CatalogCommon other)
            : this(apiVersion,
                handler,
                other.commonBlockVersion,
                other.nameHash,
                other.descriptionHash,
                other.price,
                other.thumbnailHash,
                other.unkCommon02,
                other.devCategoryFlags,
                other.productStyles,
                other.packId,
                other.tagList,
                other.sellingPoints,
                other.unlockByHash,
                other.unlockedByHash,
                other.swatchColorsSortPriority,
                other.varientThumbImageHash)
        {
        }

        public CatalogCommon(int apiVersion, EventHandler handler)
            : base(apiVersion, handler)
        {
            this.MakeNew();
        }

        public CatalogCommon(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler)
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
                this.thumbnailHash == other.thumbnailHash &&
                this.unkCommon02 == other.unkCommon02 &&
                this.devCategoryFlags == other.devCategoryFlags &&
                this.productStyles == other.productStyles &&
                this.packId == other.packId &&
                this.tagList == other.tagList &&
                this.sellingPoints == other.sellingPoints &&
                this.unlockByHash == other.unlockByHash &&
                this.unlockedByHash == other.unlockedByHash &&
                this.swatchColorsSortPriority == other.swatchColorsSortPriority &&
                this.varientThumbImageHash == other.varientThumbImageHash;
        }

        #endregion

        #region Sub-classes

        [Flags]
        public enum PackFlag : byte
        {
            HidePackIcon = 1
        }

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
                return new SellingPoint(CatalogCommon.kRecommendedApiVersion, this.elementHandler, s);
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

            public SellingPoint(int apiVersion, EventHandler handler, SellingPoint other)
                : this(apiVersion, handler, other.commodity, other.amount)
            {
            }

            public SellingPoint(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                this.MakeNew();
            }

            public SellingPoint(int apiVersion, EventHandler handler, Stream s)
                : base(apiVersion, handler)
            {
                this.Parse(s);
            }

            public SellingPoint(int apiVersion, EventHandler handler, Tag commodity, int value)
                : base(apiVersion, handler)
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
                get { return this.commodity; }
                set
                {
                    if (this.commodity != value)
                    {
                        this.commodity = value;
                        this.OnElementChanged();
                    }
                }
            }

            [ElementPriority(2)]
            public int Amount
            {
                get { return this.amount; }
                set
                {
                    if (this.amount != value)
                    {
                        this.amount = value;
                        this.OnElementChanged();
                    }
                }
            }

            public override int RecommendedApiVersion
            {
                get { return CatalogCommon.kRecommendedApiVersion; }
            }

            public string Value
            {
                get { return this.ValueBuilder; }
            }

            public override List<string> ContentFields
            {
                get { return AApiVersionedFields.GetContentFields(0, this.GetType()); }
            }

            #endregion ContentFields ========================================

            private void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.commodity = CatalogTagRegistry.FetchTag(br.ReadUInt16());
                this.amount = br.ReadInt32();
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(this.commodity.ToUInt16());
                bw.Write(this.amount);
            }

            private void MakeNew()
            {
                this.commodity = new Tag();
                this.amount = 0;
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
        public ColorList(EventHandler handler, Stream s)
            : base(
                handler,
                s,
                ColorList.ReadItem,
                ColorList.WriteItem,
                uint.MaxValue,
                ColorList.ReadListCount,
                ColorList.WriteListCount)
        {
        }

        public ColorList(EventHandler handler)
            : base(
                handler,
                ColorList.ReadItem,
                ColorList.WriteItem,
                uint.MaxValue,
                ColorList.ReadListCount,
                ColorList.WriteListCount)
        {
        }

        public ColorList(EventHandler handler, IEnumerable<uint> le)
            : base(
                handler,
                le,
                ColorList.ReadItem,
                ColorList.WriteItem,
                uint.MaxValue,
                ColorList.ReadListCount,
                ColorList.WriteListCount)
        {
        }

        private static uint ReadItem(Stream s)
        {
            return new BinaryReader(s).ReadUInt32();
        }

        private static void WriteItem(Stream s, uint value)
        {
            new BinaryWriter(s).Write(value);
        }

        private static int ReadListCount(Stream s)
        {
            return new BinaryReader(s).ReadByte();
        }

        private static void WriteListCount(Stream s, int count)
        {
            byte ncount = Convert.ToByte(count);
            new BinaryWriter(s).Write(ncount);
        }
    }

    // used by cfen and cspn
    public class SpnFenMODLEntryList : DependentList<SpnFenMODLEntry>
    {
        private int kRecommendedApiVersion = 1;

        public int RecommendedApiVersion
        {
            get { return this.kRecommendedApiVersion; }
        }

        public SpnFenMODLEntryList(EventHandler handler, Stream s)
            : base(handler, s, uint.MaxValue)
        {
        }

        public SpnFenMODLEntryList(EventHandler handler)
            : base(handler, uint.MaxValue)
        {
        }

        public SpnFenMODLEntryList(EventHandler handler, IList<SpnFenMODLEntry> le)
            : base(handler, le, uint.MaxValue)
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
            return new SpnFenMODLEntry(this.kRecommendedApiVersion, this.elementHandler, s);
        }
    }

    // used by cfen and cspn
    public class SpnFenMODLEntry : AHandlerElement, IEquatable<SpnFenMODLEntry>
    {
        #region Attributes

        private ushort modlLabel;
        private TGIBlock modlRef;
        private int kRecommendedApiVersion = 1;

        public override int RecommendedApiVersion
        {
            get { return this.kRecommendedApiVersion; }
        }

        #endregion Attributes

        #region ContentFields

        [ElementPriority(1)]
        public ushort MODLLabel
        {
            get { return this.modlLabel; }
            set
            {
                this.modlLabel = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(2)]
        public TGIBlock MODLRef
        {
            get { return this.modlRef; }
            set
            {
                if (this.modlRef != value)
                {
                    this.modlRef = new TGIBlock(this.kRecommendedApiVersion, this.handler, TGIBlock.Order.ITG, value);
                    this.OnElementChanged();
                }
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }

        public override List<string> ContentFields
        {
            get { return AApiVersionedFields.GetContentFields(0, this.GetType()); }
        }

        #endregion ContentFields ========================================================

        #region Constructors

        public SpnFenMODLEntry(int apiVersion, EventHandler handler, SpnFenMODLEntry other)
            : this(apiVersion, handler, other.modlLabel, other.modlRef)
        {
        }

        public SpnFenMODLEntry(int apiVersion, EventHandler handler)
            : base(apiVersion, handler)
        {
            this.MakeNew();
        }

        public SpnFenMODLEntry(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler)
        {
            this.Parse(s);
        }

        public SpnFenMODLEntry(int apiVersion, EventHandler handler, ushort modlLabel, TGIBlock modlRef)
            : base(apiVersion, handler)
        {
            this.modlLabel = modlLabel;
            this.modlRef = modlRef = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, modlRef);
        }

        public bool Equals(SpnFenMODLEntry other)
        {
            return (this.modlLabel == other.modlLabel) && (this.modlRef == other.modlRef);
        }

        #endregion Constructors ====================================================================

        #region DataIO

        private void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.modlLabel = br.ReadUInt16();
            this.modlRef = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            bw.Write(this.modlLabel);
            this.modlRef.UnParse(s);
        }

        private void MakeNew()
        {
            this.modlLabel = 0x0;
            this.modlRef = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }

        #endregion DataIO ===============================================================
    }

    // used by ccol
    public class Gp4references : AHandlerElement, IEquatable<Gp4references>
    {
        private TGIBlock ref01;
        private TGIBlock ref02;
        private TGIBlock ref03;
        private TGIBlock ref04;
        private int kRecommendedApiVersion = 1;

        public override int RecommendedApiVersion
        {
            get { return this.kRecommendedApiVersion; }
        }

        #region ContentFields

        [ElementPriority(2)]
        public TGIBlock Ref01
        {
            get { return this.ref01; }
            set
            {
                if (this.ref01 != value)
                {
                    this.ref01 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(3)]
        public TGIBlock Ref02
        {
            get { return this.ref02; }
            set
            {
                if (this.ref02 != value)
                {
                    this.ref02 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(4)]
        public TGIBlock Ref03
        {
            get { return this.ref03; }
            set
            {
                if (this.ref03 != value)
                {
                    this.ref03 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(5)]
        public TGIBlock Ref04
        {
            get { return this.ref04; }
            set
            {
                if (this.ref04 != value)
                {
                    this.ref04 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }

        public override List<string> ContentFields
        {
            get { return AApiVersionedFields.GetContentFields(0, this.GetType()); }
        }

        #endregion ContentFields

        #region Constructors

        public Gp4references(int apiVersion, EventHandler handler, Gp4references other)
            : this(apiVersion, handler, other.ref01, other.ref02, other.ref03, other.ref04)
        {
        }

        public Gp4references(int apiVersion, EventHandler handler)
            : base(apiVersion, handler)
        {
            this.MakeNew();
        }

        public Gp4references(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler)
        {
            this.Parse(s);
        }

        public Gp4references(int apiVersion,
                             EventHandler handler,
                             TGIBlock ref01,
                             TGIBlock ref02,
                             TGIBlock ref03,
                             TGIBlock ref04)
            : base(apiVersion, handler)
        {
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref01);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref02);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref03);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref04);
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

        private void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref03 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            this.ref01.UnParse(s);
            this.ref02.UnParse(s);
            this.ref03.UnParse(s);
            this.ref04.UnParse(s);
        }

        public void MakeNew()
        {
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref03 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }

        #endregion DataIO
    }

    // used by cfen and cspn
    public class Gp7references : AHandlerElement, IEquatable<Gp7references>
    {
        private TGIBlock ref01;
        private TGIBlock ref02;
        private TGIBlock ref03;
        private TGIBlock ref04;
        private TGIBlock ref05;
        private TGIBlock ref06;
        private TGIBlock ref07;
        private int kRecommendedApiVersion = 1;

        public override int RecommendedApiVersion
        {
            get { return this.kRecommendedApiVersion; }
        }

        #region ContentFields

        [ElementPriority(2)]
        public TGIBlock Ref01
        {
            get { return this.ref01; }
            set
            {
                if (this.ref01 != value)
                {
                    this.ref01 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(3)]
        public TGIBlock Ref02
        {
            get { return this.ref02; }
            set
            {
                if (this.ref02 != value)
                {
                    this.ref02 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(4)]
        public TGIBlock Ref03
        {
            get { return this.ref03; }
            set
            {
                if (this.ref03 != value)
                {
                    this.ref03 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(5)]
        public TGIBlock Ref04
        {
            get { return this.ref04; }
            set
            {
                if (this.ref04 != value)
                {
                    this.ref04 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(6)]
        public TGIBlock Ref05
        {
            get { return this.ref05; }
            set
            {
                if (this.ref05 != value)
                {
                    this.ref05 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(7)]
        public TGIBlock Ref06
        {
            get { return this.ref06; }
            set
            {
                if (this.ref06 != value)
                {
                    this.ref06 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(8)]
        public TGIBlock Ref07
        {
            get { return this.ref07; }
            set
            {
                if (this.ref07 != value)
                {
                    this.ref07 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }

        public override List<string> ContentFields
        {
            get { return AApiVersionedFields.GetContentFields(0, this.GetType()); }
        }

        #endregion ContentFields

        #region Constructors

        public Gp7references(int apiVersion, EventHandler handler, Gp7references other)
            : this(
                apiVersion,
                handler,
                other.ref01,
                other.ref02,
                other.ref03,
                other.ref04,
                other.ref05,
                other.ref06,
                other.ref07)
        {
        }

        public Gp7references(int apiVersion, EventHandler handler)
            : base(apiVersion, handler)
        {
            this.MakeNew();
        }

        public Gp7references(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler)
        {
            this.Parse(s);
        }

        public Gp7references(int apiVersion,
                             EventHandler handler,
                             TGIBlock ref01,
                             TGIBlock ref02,
                             TGIBlock ref03,
                             TGIBlock ref04,
                             TGIBlock ref05,
                             TGIBlock ref06,
                             TGIBlock ref07)
            : base(apiVersion, handler)
        {
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref01);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref02);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref03);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref04);
            this.ref05 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref05);
            this.ref06 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref06);
            this.ref07 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref07);
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

        private void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref03 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref05 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref06 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref07 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            this.ref01.UnParse(s);
            this.ref02.UnParse(s);
            this.ref03.UnParse(s);
            this.ref04.UnParse(s);
            this.ref05.UnParse(s);
            this.ref06.UnParse(s);
            this.ref07.UnParse(s);
        }

        public void MakeNew()
        {
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref03 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref05 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref06 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref07 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }

        #endregion DataIO
    }

    // used by cral
    public class Gp8references : AHandlerElement, IEquatable<Gp8references>
    {
        private TGIBlock ref01;
        private TGIBlock ref02;
        private TGIBlock ref03;
        private TGIBlock ref04;
        private TGIBlock ref05;
        private TGIBlock ref06;
        private TGIBlock ref07;
        private TGIBlock ref08;
        private int kRecommendedApiVersion = 1;
        private bool parsed = false;

        public override int RecommendedApiVersion
        {
            get { return this.kRecommendedApiVersion; }
        }

        #region ContentFields

        [ElementPriority(2)]
        public TGIBlock Ref01
        {
            get { return this.ref01; }
            set
            {
                if (this.ref01 != value)
                {
                    this.ref01 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(3)]
        public TGIBlock Ref02
        {
            get { return this.ref02; }
            set
            {
                if (this.ref01 != value)
                {
                    this.ref01 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(4)]
        public TGIBlock Ref03
        {
            get { return this.ref03; }
            set
            {
                if (this.ref03 != value)
                {
                    this.ref03 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(5)]
        public TGIBlock Ref04
        {
            get { return this.ref04; }
            set
            {
                if (this.ref04 != value)
                {
                    this.ref04 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(6)]
        public TGIBlock Ref05
        {
            get { return this.ref05; }
            set
            {
                if (this.ref05 != value)
                {
                    this.ref05 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(7)]
        public TGIBlock Ref06
        {
            get { return this.ref06; }
            set
            {
                if (this.ref06 != value)
                {
                    this.ref06 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(7)]
        public TGIBlock Ref07
        {
            get { return this.ref07; }
            set
            {
                if (this.ref07 != value)
                {
                    this.ref07 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(7)]
        public TGIBlock Ref08
        {
            get { return this.ref08; }
            set
            {
                if (this.ref08 != value)
                {
                    this.ref08 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }

        public override List<string> ContentFields
        {
            get { return AApiVersionedFields.GetContentFields(0, this.GetType()); }
        }

        #endregion ContentFields ==================================================================

        #region Constructors

        public Gp8references(int apiVersion, EventHandler handler, Gp8references other)
            : this(
                apiVersion,
                handler,
                other.ref01,
                other.ref02,
                other.ref03,
                other.ref04,
                other.ref05,
                other.ref06,
                other.ref07,
                other.ref08)
        {
        }

        public Gp8references(int apiVersion, EventHandler handler)
            : base(apiVersion, handler)
        {
            this.MakeNew();
        }

        public Gp8references(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler)
        {
            this.Parse(s);
        }

        public Gp8references(int apiVersion,
                             EventHandler handler,
                             TGIBlock ref01,
                             TGIBlock ref02,
                             TGIBlock ref03,
                             TGIBlock ref04,
                             TGIBlock ref05,
                             TGIBlock ref06,
                             TGIBlock ref07,
                             TGIBlock ref08)
            : base(apiVersion, handler)
        {
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref01);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref02);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref03);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref04);
            this.ref05 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref05);
            this.ref06 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref06);
            this.ref07 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref07);
            this.ref08 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref08);
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

        private void Parse(Stream s)
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
            this.parsed = true;
        }

        private void MakeNew()
        {
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref03 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref05 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref06 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref07 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref08 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }

        public void UnParse(Stream s)
        {
            if (this.parsed == false)
            {
                this.MakeNew();
            }
            var bw = new BinaryWriter(s);
            this.ref01.UnParse(s);
            this.ref02.UnParse(s);
            this.ref03.UnParse(s);
            this.ref04.UnParse(s);
            this.ref05.UnParse(s);
            this.ref06.UnParse(s);
            this.ref07.UnParse(s);
            this.ref08.UnParse(s);
        }

        #endregion DataIO
    }

    // used by ccol
    public class Gp9references : AHandlerElement, IEquatable<Gp9references>
    {
        private TGIBlock ref01;
        private TGIBlock ref02;
        private TGIBlock ref03;
        private TGIBlock ref04;
        private TGIBlock ref05;
        private TGIBlock ref06;
        private TGIBlock ref07;
        private TGIBlock ref08;
        private TGIBlock ref09;
        private int kRecommendedApiVersion = 1;

        public override int RecommendedApiVersion
        {
            get { return this.kRecommendedApiVersion; }
        }

        #region ContentFields

        [ElementPriority(2)]
        public TGIBlock Ref01
        {
            get { return this.ref01; }
            set
            {
                if (this.ref01 != value)
                {
                    this.ref01 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(3)]
        public TGIBlock Ref02
        {
            get { return this.ref02; }
            set
            {
                if (this.ref02 != value)
                {
                    this.ref02 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(4)]
        public TGIBlock Ref03
        {
            get { return this.ref03; }
            set
            {
                if (this.ref03 != value)
                {
                    this.ref03 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(5)]
        public TGIBlock Ref04
        {
            get { return this.ref04; }
            set
            {
                if (this.ref04 != value)
                {
                    this.ref04 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(6)]
        public TGIBlock Ref05
        {
            get { return this.ref05; }
            set
            {
                if (this.ref05 != value)
                {
                    this.ref05 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(7)]
        public TGIBlock Ref06
        {
            get { return this.ref06; }
            set
            {
                if (this.ref06 != value)
                {
                    this.ref06 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(8)]
        public TGIBlock Ref07
        {
            get { return this.ref07; }
            set
            {
                if (this.ref07 != value)
                {
                    this.ref07 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(9)]
        public TGIBlock Ref08
        {
            get { return this.ref08; }
            set
            {
                if (this.ref08 != value)
                {
                    this.ref08 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        [ElementPriority(10)]
        public TGIBlock Ref09
        {
            get { return this.ref09; }
            set
            {
                if (this.ref09 != value)
                {
                    this.ref09 = new TGIBlock(this.kRecommendedApiVersion,
                        this.handler,
                        TGIBlock.Order.ITG,
                        value.ResourceType,
                        value.ResourceGroup,
                        value.Instance);
                    this.OnElementChanged();
                }
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }

        public override List<string> ContentFields
        {
            get { return AApiVersionedFields.GetContentFields(0, this.GetType()); }
        }

        #endregion ContentFields

        #region Constructors

        public Gp9references(int apiVersion, EventHandler handler, Gp9references other)
            : this(
                apiVersion,
                handler,
                other.ref01,
                other.ref02,
                other.ref03,
                other.ref04,
                other.ref05,
                other.ref06,
                other.ref07,
                other.ref08,
                other.ref09)
        {
        }

        public Gp9references(int apiVersion, EventHandler handler)
            : base(apiVersion, handler)
        {
            this.MakeNew();
        }

        public Gp9references(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler)
        {
            this.Parse(s);
        }

        public Gp9references(int apiVersion,
                             EventHandler handler,
                             TGIBlock ref01,
                             TGIBlock ref02,
                             TGIBlock ref03,
                             TGIBlock ref04,
                             TGIBlock ref05,
                             TGIBlock ref06,
                             TGIBlock ref07,
                             TGIBlock ref08,
                             TGIBlock ref09)
            : base(apiVersion, handler)
        {
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref01);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref02);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref03);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref04);
            this.ref05 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref05);
            this.ref06 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref06);
            this.ref07 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref07);
            this.ref08 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref08);
            this.ref09 = new TGIBlock(this.kRecommendedApiVersion, handler, TGIBlock.Order.ITG, ref09);
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

        private void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref03 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref05 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref06 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref07 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref08 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.ref09 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            this.ref01.UnParse(s);
            this.ref02.UnParse(s);
            this.ref03.UnParse(s);
            this.ref04.UnParse(s);
            this.ref05.UnParse(s);
            this.ref06.UnParse(s);
            this.ref07.UnParse(s);
            this.ref08.UnParse(s);
            this.ref09.UnParse(s);
        }

        public void MakeNew()
        {
            this.ref01 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref02 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref03 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref04 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref05 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref06 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref07 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref08 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
            this.ref09 = new TGIBlock(this.kRecommendedApiVersion, null, TGIBlock.Order.ITG);
        }

        #endregion DataIO
    }
}