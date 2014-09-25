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
    public class COBJResource : AResource
    {
        const int kRecommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return kRecommendedApiVersion; } }

        #region Data I/O

        void Parse(Stream s)
        {
            s.Position = 0;
            var br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.unknown1 = br.ReadUInt32();
            this.name_hash = br.ReadUInt32();
            this.description_hash = br.ReadUInt32();
            this.price = br.ReadInt32();
            this.unknown2 = br.ReadUInt64();
            this.unknown3 = br.ReadUInt32();
            int tgi_count = br.ReadByte();
            this.ct_prduct_styles = new StyleList(this.OnResourceChanged, TGIBlock.Order.ITG, tgi_count, s );
            this.unknown4 = br.ReadUInt16();
            this.unknown5 = new UnknownShortList(this.OnResourceChanged, s);
            this.selling_points = new SellingPointList(this.OnResourceChanged, s);
            this.unknown6 = br.ReadUInt64();
            this.unknown7 = br.ReadUInt16();
            this.unknown8 = br.ReadUInt64();
            this.unknown9 = br.ReadUInt32();
            this.unknown10 = br.ReadUInt32();
            this.unknown11 = br.ReadUInt32();
            this.unknown12 = br.ReadUInt32();
            this.unknown13 = br.ReadUInt32();
            this.unknown14 = br.ReadUInt32();
            this.unknown15 = br.ReadUInt32();
            this.unknown16_flags = br.ReadUInt32();
            this.category_flags = br.ReadUInt32();
            this.unknown17_flags = br.ReadUInt32();
            this.unknown18_flags = br.ReadUInt32();
            this.placement_flags = br.ReadUInt32();
            this.unknown19_iid = br.ReadUInt64();
            this.unknown20 = br.ReadByte();
            this.unknown21_iid = br.ReadUInt64();
            this.unknown27 = br.ReadByte();
            this.colors = new ColorList(this.OnResourceChanged, s);
            this.unknown22 = br.ReadByte();
            this.unknown23 = br.ReadByte();
            this.unknown24 = br.ReadByte();
            this.unknown25 = br.ReadByte();
            this.unknown26 = br.ReadByte();
            this.build_buy = br.ReadByte();
            /*
            */

        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write(this.version);
            bw.Write(this.unknown1);
            bw.Write(this.name_hash);
            bw.Write(this.description_hash);
            bw.Write(this.price);
            bw.Write(this.unknown2);
            bw.Write(this.unknown3);
            this.ct_prduct_styles.UnParse(s);
            bw.Write(this.unknown4);
            this.unknown5.UnParse(s);
            this.selling_points.UnParse(s);
            bw.Write(this.unknown6);
            bw.Write(this.unknown7);
            bw.Write(this.unknown8);
            bw.Write(this.unknown9);
            bw.Write(this.unknown10);
            bw.Write(this.unknown11);
            bw.Write(this.unknown12);
            bw.Write(this.unknown13);
            bw.Write(this.unknown14);
            bw.Write(this.unknown15);
            bw.Write(this.unknown16_flags);
            bw.Write(this.category_flags);
            bw.Write(this.unknown17_flags);
            bw.Write(this.unknown18_flags);
            bw.Write(this.placement_flags);
            bw.Write(this.unknown19_iid);
            bw.Write(this.unknown20);
            bw.Write(this.unknown21_iid);
            bw.Write(this.unknown27);
            this.colors.UnParse(s);
            bw.Write(this.unknown22);
            bw.Write(this.unknown23);
            bw.Write(this.unknown24);
            bw.Write(this.unknown25);
            bw.Write(this.unknown26);
            bw.Write(this.build_buy);
            /*
            */
            return s;
        }
        #endregion


 
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
 
        #region Attributes
 
        private uint version;
        private uint unknown1;
        private uint name_hash;
        private uint description_hash;
        private Int32 price;
        private ulong unknown2;
        private uint unknown3;
        private StyleList ct_prduct_styles;
        private ushort unknown4;
        private SimpleList<ushort> unknown5;
 
        private SellingPointList selling_points;
 
        private ulong unknown6;
        private ushort unknown7;
        private ulong unknown8;
 
        private uint unknown9;
        private uint unknown10;
        private uint unknown11;
        private uint unknown12;
        private uint unknown13;
        private uint unknown14;
        private uint unknown15;
 
        private uint unknown16_flags;
        private uint category_flags;
        private uint unknown17_flags;
        private uint unknown18_flags;
        private uint placement_flags;
 
        private ulong unknown19_iid;
        private byte unknown20;
        private ulong unknown21_iid;
        private byte unknown27;
 
        private ColorList colors;
 
        private byte unknown22;
        private byte unknown23;
        private byte unknown24;
        private byte unknown25;
        private byte unknown26;
        private byte build_buy;
 
        #endregion
 
        #region Constructors
        public COBJResource(int APIversion, Stream s)
            : base(APIversion, s)
        {
            this.Parse(s);
        }
        #endregion


        #region Content Fields
        /*
*/
        [ElementPriority(37)]
        public byte BuildBuy
        {
            get { return build_buy; }
            set { build_buy = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(36)]
        public byte Unknown26
        {
            get { return unknown26; }
            set { unknown26 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(35)]
        public byte Unknown25
        {
            get { return unknown25; }
            set { unknown25 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(34)]
        public byte Unknown24
        {
            get { return unknown24; }
            set { unknown24 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(33)]
        public byte Unknown23
        {
            get { return unknown23; }
            set { unknown23 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(32)]
        public byte Unknown22
        {
            get { return unknown22; }
            set { unknown22 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(31)]
        public ColorList Colors
        {
            get { return colors; }
            set { colors = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(30)]
        public byte Unknown27
        {
            get { return unknown27; }
            set { unknown27 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(29)]
        public ulong Unknown21Iid
        {
            get { return unknown21_iid; }
            set { unknown21_iid = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(28)]
        public byte Unknown20
        {
            get { return unknown20; }
            set { unknown20 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(27)]
        public ulong Unknown19Iid
        {
            get { return unknown19_iid; }
            set { unknown19_iid = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(26)]
        public uint PlacementFlags
        {
            get { return placement_flags; }
            set { placement_flags = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(25)]
        public uint Unknown18Flags
        {
            get { return unknown18_flags; }
            set { unknown18_flags = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(24)]
        public uint Unknown17Flags
        {
            get { return unknown17_flags; }
            set { unknown17_flags = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(23)]
        public uint CategoryFlags
        {
            get { return category_flags; }
            set { category_flags = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(22)]
        public uint Unknown16Flags
        {
            get { return unknown16_flags; }
            set { unknown16_flags = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(21)]
        public uint Unknown15
        {
            get { return unknown15; }
            set { unknown15 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(20)]
        public uint Unknown14
        {
            get { return unknown14; }
            set { unknown14 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(19)]
        public uint Unknown13
        {
            get { return unknown13; }
            set { unknown13 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(18)]
        public uint Unknown12
        {
            get { return unknown12; }
            set { unknown12 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(17)]
        public uint Unknown11
        {
            get { return unknown11; }
            set { unknown11 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(16)]
        public uint Unknown10
        {
            get { return unknown10; }
            set { unknown10 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(15)]
        public uint Unknown9
        {
            get { return unknown9; }
            set { unknown9 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(14)]
        public ulong Unknown8
        {
            get { return unknown8; }
            set { unknown8 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(13)]
        public ushort Unknown7
        {
            get { return unknown7; }
            set { unknown7 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(12)]
        public ulong Unknown6
        {
            get { return unknown6; }
            set { unknown6 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(11)]
        public SellingPointList SellingPoints
        {
            get { return selling_points; }
            set { selling_points = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(10)]
        public SimpleList<ushort> Unknown5
        {
            get { return unknown5; }
            set { unknown5 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(9)]
        public ushort Unknown4
        {
            get { return unknown4; }
            set { unknown4 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(8)]
        public StyleList CtPrductStyles
        {
            get { return ct_prduct_styles; }
            set { ct_prduct_styles = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(7)]
        public uint Unknown3
        {
            get { return unknown3; }
            set { unknown3 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(6)]
        public ulong Unknown2
        {
            get { return unknown2; }
            set { unknown2 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(5)]
        public Int32 Price
        {
            get { return price; }
            set { price = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(4)]
        public uint DescriptionHash
        {
            get { return description_hash; }
            set { description_hash = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(3)]
        public uint NameHash
        {
            get { return name_hash; }
            set { name_hash = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(2)]
        public uint Unknown1
        {
            get { return unknown1; }
            set { unknown1 = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(1)]
        public uint Version
        {
            get { return version; }
            set { version = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        public string Value { get { return ValueBuilder; } }

        #endregion


    }
}