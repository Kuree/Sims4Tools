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
 *  s3pi is distributed in the hope that it will be useful,                *
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
using System.Linq;
using System.Text;
using s4pi.Interfaces;

namespace CatalogResource
{
    public class WallCatalogResourceTS4 : AResource
    {
        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }


        #region Attributes

        private uint version;
        private uint unk02;
        private uint name_hash;
        private uint description_hash;
        private Int32 price;
        private uint unk05;
        private uint unk06;
        private uint unk07;
        private StyleList ct_prduct_styles;
        private UInt16 unk08;
        private SimpleList<ushort> tagList;
        //private SellingPointList selling_points;
        private uint unk09;
        private uint unk10;
        private uint unk11;
        private uint unk12;
        private uint unk13;
        private UInt16 unk14;
        private MATDEntryList matdList;
        private ImgGroupList imgGroupList;
        private uint unk15;
        private ColorList colors;
        private ulong unkIID01;

        #endregion


        #region Content Fields


        [ElementPriority(1)]
        public uint Version
        {
            get { return version; }
            set { version = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(2)]
        public uint Unk02
        {
            get { return unk02; }
            set { unk02 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(3)]
        public uint NameHash
        {
            get { return name_hash; }
            set { name_hash = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(4)]
        public uint DescriptionHash
        {
            get { return description_hash; }
            set { description_hash = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(5)]
        public Int32 Price
        {
            get { return price; }
            set { price = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(6)]
        public uint Unk05
        {
            get { return unk05; }
            set { unk05 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(7)]
        public uint Unk06
        {
            get { return unk06; }
            set { unk06 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(8)]
        public uint Unk07
        {
            get { return unk07; }
            set { unk07 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(9)]
        public StyleList CtPrductStyles
        {
            get { return ct_prduct_styles; }
            set { ct_prduct_styles = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(10)]
        public ushort Unk08
        {
            get { return unk08; }
            set { unk08 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(11)]
        public SimpleList<ushort> TagList
        {
            get { return tagList; }
            set { tagList = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(12)]
        public uint Unk09
        {
            get { return unk09; }
            set { unk09 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(13)]
        public uint Unk10
        {
            get { return unk10; }
            set { unk10 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(14)]
        public uint Unk11
        {
            get { return unk11; }
            set { unk11 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(17)]
        public uint Unk12
        {
            get { return unk12; }
            set { unk12 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(18)]
        public uint Unk13
        {
            get { return unk13; }
            set { unk13 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(19)]
        public ushort Unk14
        {
            get { return unk14; }
            set { unk14 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(20)]
        public MATDEntryList MATDList
        {
            get { return matdList; }
            set { matdList = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(22)]
        public ImgGroupList ImgList
        {
            get { return imgGroupList; }
            set { imgGroupList = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(23)]
        public uint Unk15
        {
            get { return unk15; }
            set { unk15 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(24)]
        public ColorList Colors
        {
            get { return colors; }
            set { colors = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(25)]
        public ulong UnkIID01
        {
            get { return unkIID01; }
            set { unkIID01 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }


        public string Value { get { return ValueBuilder; } }

        #endregion

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                /*
                if (this.version < 0x00000019)
                {
                    res.Remove("Unknown28");
                    res.Remove("Unknown29");
                    res.Remove("Unknown30");
                    res.Remove("Unknown31");
                }
                 * */
                return res;
            }
        }


        #region Data I/O

        void Parse(Stream s)
        {
            s.Position = 0;
            var br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.unk02 = br.ReadUInt32();
            this.name_hash = br.ReadUInt32();
            this.description_hash = br.ReadUInt32();
            this.price = br.ReadInt32();
            this.unk05 = br.ReadUInt32();
            this.unk06 = br.ReadUInt32();
            this.unk07 = br.ReadUInt32();
            int tgi_count = br.ReadByte();
            this.ct_prduct_styles = new StyleList(this.OnResourceChanged, TGIBlock.Order.ITG, tgi_count, s);
            this.unk08 = br.ReadUInt16();
            this.tagList = new UnknownShortList(this.OnResourceChanged, s);
            this.unk09 = br.ReadUInt32();
            this.unk10 = br.ReadUInt32();
            this.unk11 = br.ReadUInt32();
            this.unk12 = br.ReadUInt32();
            this.unk13 = br.ReadUInt32();
            this.unk14 = br.ReadUInt16();
            this.matdList = new MATDEntryList(this.OnResourceChanged, s);
            this.imgGroupList = new ImgGroupList(this.OnResourceChanged, s);
            this.unk15 = br.ReadUInt32();
            this.colors = new ColorList(this.OnResourceChanged, s);
            this.unkIID01 = br.ReadUInt64();
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            bw.Write(this.unk02);
            bw.Write(this.name_hash);
            bw.Write(this.description_hash);
            bw.Write(this.price);
            bw.Write(this.unk05);
            bw.Write(this.unk06);
            bw.Write(this.unk07);
            this.ct_prduct_styles.UnParse(s);
            bw.Write(this.unk08);
            this.tagList.UnParse(s);
            bw.Write(this.unk09);
            bw.Write(this.unk10);
            bw.Write(this.unk11);
            bw.Write(this.unk12);
            bw.Write(this.unk13);
            bw.Write(this.unk14);
            this.matdList.UnParse(s);
            this.imgGroupList.UnParse(s);
            bw.Write(this.unk15);
            this.colors.UnParse(s);
            bw.Write(this.unkIID01);
            return s;
        }
        #endregion


        /// 


        #region Sub-classes


        public class StyleList : CountedTGIBlockList
        {
            // BYTE count
            // ITG resourcekey

            public StyleList(EventHandler handler, TGIBlock.Order o, int count, Stream s)
                : base(handler, TGIBlock.Order.ITG, count, s, 255)
            {
            }

            public StyleList(EventHandler handler)
                : base(handler, TGIBlock.Order.ITG)
            {
            }

            public override void UnParse(Stream s)
            {
                byte ncount = Convert.ToByte(Count);
                new BinaryWriter(s).Write(ncount);
                foreach (TGIBlock rk in this)
                {
                    WriteElement(s, rk);
                }
            }

        }

        public class MATDEntryList : DependentList<MATDEntry>
        {
            public MATDEntryList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public MATDEntryList(EventHandler handler, IEnumerable<MATDEntry> ilt, long maxSize = -1)
                : base(handler, ilt, maxSize)
            {
            }

            public MATDEntryList(EventHandler handler, Stream s)
                : base(handler, s)
            {
            }

            protected override MATDEntry CreateElement(Stream s)
            {
                return new MATDEntry(kRecommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, MATDEntry element)
            {
                element.UnParse(s);
            }
        }

        public class MATDEntry : AHandlerElement, IEquatable<MATDEntry>
        {
            byte mlodLabel = 0;
            TGIBlock mlodRef;

            public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

            [ElementPriority(1)]
            public byte MLODLabel
            {
                get { return mlodLabel; }
                set { mlodLabel = value; OnElementChanged(); }
            }
            [ElementPriority(2)]
            public TGIBlock MLODRef
            {
                get { return mlodRef; }
                set { mlodRef = value; OnElementChanged(); }
            }

            public MATDEntry(int APIversion, EventHandler handler, MATDEntry other)
                : this(APIversion, handler, other.mlodLabel, other.mlodRef)
            {
            }
            public MATDEntry(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
            }
            public MATDEntry(int APIversion, EventHandler handler, Stream s)
                : this(APIversion, handler)
            {
                this.Parse(s);
            }
            public MATDEntry(int APIversion, EventHandler handler, byte mlodLabel, TGIBlock mlodRef)
                : base(APIversion, handler)
            {
                this.mlodLabel = mlodLabel;
                this.mlodRef = mlodRef;
            }

            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.mlodLabel = br.ReadByte();
                this.mlodRef = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(mlodLabel);
                mlodRef.UnParse(s);
            }
            public bool Equals(MATDEntry other)
            {
                return this.mlodLabel == other.mlodLabel && this.mlodRef == other.mlodRef;
            }

            public string Value { get { return ValueBuilder; } }

            public override List<string> ContentFields
            {
                get { return GetContentFields(0, GetType()); }
            }

        }

        public class ImgGroupList : DependentList<ImgGroupEntry>
        {
            public ImgGroupList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public ImgGroupList(EventHandler handler, IEnumerable<ImgGroupEntry> ilt, long maxSize = -1)
                : base(handler, ilt, maxSize)
            {
            }

            public ImgGroupList(EventHandler handler, Stream s)
                : base(handler, s)
            {
            }

            protected override ImgGroupEntry CreateElement(Stream s)
            {
                return new ImgGroupEntry(kRecommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, ImgGroupEntry element)
            {
                element.UnParse(s);
            }
        }

        public class ImgGroupEntry : AHandlerElement, IEquatable<ImgGroupEntry>
        {
            byte imgGroupLabel = 0;
            TGIBlock imgRef01;
            TGIBlock imgRef02;
            TGIBlock imgRef03;

            public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }


            [ElementPriority(1)]
            public byte ImgGroupLabel
            {
                get { return imgGroupLabel; }
                set { imgGroupLabel = value; OnElementChanged(); }
            }
            [ElementPriority(2)]
            public TGIBlock ImgRef01
            {
                get { return imgRef01; }
                set { imgRef01 = value; OnElementChanged(); }
            }
            [ElementPriority(3)]
            public TGIBlock ImgRef02
            {
                get { return imgRef02; }
                set { imgRef02 = value; OnElementChanged(); }
            }
            [ElementPriority(4)]
            public TGIBlock ImgRef03
            {
                get { return imgRef03; }
                set { imgRef03 = value; OnElementChanged(); }
            }

            public ImgGroupEntry(int APIversion, EventHandler handler, ImgGroupEntry other)
                : this(APIversion, handler, other.imgGroupLabel, other.imgRef01, other.imgRef02, other.imgRef03)
            {
            }
            public ImgGroupEntry(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
            }
            public ImgGroupEntry(int APIversion, EventHandler handler, Stream s)
                : this(APIversion, handler)
            {
                this.Parse(s);
            }
            public ImgGroupEntry(int APIversion, EventHandler handler, byte imgGroupLabel, TGIBlock imgRef01, TGIBlock imgRef02, TGIBlock imgRef03)
                : base(APIversion, handler)
            {
                this.imgGroupLabel = imgGroupLabel;
                this.imgRef01 = imgRef01;
                this.imgRef02 = imgRef02;
                this.imgRef02 = imgRef02;
            }

            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.imgGroupLabel = br.ReadByte();
                this.imgRef01 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
                this.imgRef02 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
                this.imgRef03 = new TGIBlock(kRecommendedApiVersion, null, TGIBlock.Order.ITG, s);
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(imgGroupLabel);
                imgRef01.UnParse(s);
                imgRef02.UnParse(s);
                imgRef03.UnParse(s);
            }
            public bool Equals(ImgGroupEntry other)
            {
                return this.imgGroupLabel == other.imgGroupLabel &&
                    this.imgRef01 == other.imgRef01 &&
                    this.imgRef02 == other.imgRef02 &&
                    this.imgRef03 == other.imgRef03;
            }

            public string Value { get { return ValueBuilder; } }

            public override List<string> ContentFields
            {
                get { return GetContentFields(0, GetType()); }
            }

        }


        public class ColorList : SimpleList<uint>
        {
            // BYTE count
            // DWORD value
            public ColorList(EventHandler handler, Stream s)
                : base(handler, s, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount)
            {
            }
            public ColorList(EventHandler handler) : base(handler, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }
            public ColorList(EventHandler handler, IList<UInt32> le) : base(handler, le, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }

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

        public class UnknownShortList : SimpleList<ushort>
        {
            // DWORD count
            // WORD value

            public UnknownShortList(EventHandler handler, Stream s)
                : base(handler, s, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount)
            {
            }
            public UnknownShortList(EventHandler handler) : base(handler, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }
            public UnknownShortList(EventHandler handler, IList<ushort> le) : base(handler, le, ReadItem, WriteItem, UInt32.MaxValue, ReadListCount, WriteListCount) { }

            static ushort ReadItem(Stream s)
            {
                ushort foo = new BinaryReader(s).ReadUInt16();
                return foo;
            }
            static void WriteItem(Stream s, ushort value)
            {
                new BinaryWriter(s).Write(value);
            }
            static int ReadListCount(Stream s)
            {
                UInt32 count = new BinaryReader(s).ReadUInt32();
                return (int)count;
            }
            static void WriteListCount(Stream s, int count)
            {
                new BinaryWriter(s).Write((UInt32)count);
            }
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
                return new SellingPoint(kRecommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, SellingPoint element)
            {
                element.UnParse(s);
            }
        }



        public class SellingPoint : AHandlerElement, IEquatable<SellingPoint>
        {
            private ushort commodity;
            private int amount;

            public SellingPoint(int APIversion, EventHandler handler, SellingPoint other)
                : this(APIversion, handler, other.commodity, other.amount)
            {
            }
            public SellingPoint(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
            }

            public SellingPoint(int APIversion, EventHandler handler, Stream s)
                : this(APIversion, handler)
            {
                this.Parse(s);
            }
            public SellingPoint(int APIversion, EventHandler handler, ushort commodity, int value)
                : base(APIversion, handler)
            {
                this.commodity = commodity;
                this.amount = value;
            }

            [ElementPriority(1)]
            public ushort Commodity
            {
                get { return commodity; }
                set { commodity = value; OnElementChanged(); }
            }
            [ElementPriority(2)]
            public int Amount
            {
                get { return amount; }
                set { this.amount = value; OnElementChanged(); }
            }

            public override int RecommendedApiVersion
            {
                get { return kRecommendedApiVersion; }
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(0, GetType()); }
            }

            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.commodity = br.ReadUInt16();
                this.amount = br.ReadInt32();
            }
            public string Value { get { return ValueBuilder; } }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(this.commodity);
                bw.Write(this.amount);
            }
            public bool Equals(SellingPoint other)
            {
                return this.commodity == other.commodity && this.amount == other.amount;
            }
        }
        #endregion



        #region Constructors
        public WallCatalogResourceTS4(int APIversion, Stream s)
            : base(APIversion, s)
        {
            this.Parse(s);
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
            if (s4pi.Settings.Settings.IsTS4)
            {
                this.Add(typeof(WallCatalogResourceTS4), new List<string>(new string[] { "0xD5F0F921", }));
            }
        }
    }
}