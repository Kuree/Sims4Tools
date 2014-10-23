/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
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
using System.Linq;
using System.Text;
using s4pi.Interfaces;
using System.IO;

namespace CatalogResource.TS4
{
    public class ObjectCatalogResource : AResource
    {
        const int recommendedApiVersion = 1;
        private uint version;
        private uint catalogVersion; // 0x00000009
        private uint catalogNameHash;
        private uint catalogDescHash;
        private uint catalogPrice;

        private uint catalogUnknown1;
        private uint catalogUnknown2;
        private uint catalogUnknown3;

        private CountedTGIBlockList catalogStyleTGIList;

        private ushort catalogUnknown4;
        private SimpleList<ushort> catalogTagList;
        private SellingPointList catalogSellingPointList;
        private ulong catalogUnknown5;
        private ushort catalogUnknown6;
        private ulong catalogUnknown7;

        public ObjectCatalogResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        protected virtual void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.catalogVersion = r.ReadUInt32();
            this.catalogNameHash = r.ReadUInt32();
            this.catalogDescHash = r.ReadUInt32();
            this.catalogPrice = r.ReadUInt32();

            this.catalogUnknown1 = r.ReadUInt32();
            this.catalogUnknown2 = r.ReadUInt32();
            this.catalogUnknown3 = r.ReadUInt32();
            byte count = r.ReadByte();
            TGIBlock[] tgiLIst = new TGIBlock[count];
            for (int i = 0; i < count; i++) tgiLIst[i] = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.catalogStyleTGIList = new CountedTGIBlockList(OnResourceChanged, tgiLIst);

            this.catalogUnknown4 = r.ReadUInt16();
            var count2 = r.ReadInt32();
            this.catalogTagList = new SimpleList<ushort>(OnResourceChanged);
            for (var i = 0; i < count2; i++) this.catalogTagList.Add(r.ReadUInt16());
            this.catalogSellingPointList = new SellingPointList(OnResourceChanged, s);
            this.catalogUnknown5 = r.ReadUInt64();
            this.catalogUnknown6 = r.ReadUInt16();
            this.catalogUnknown7 = r.ReadUInt64();

        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(this.version);
            w.Write(this.catalogVersion);
            w.Write(this.catalogNameHash);
            w.Write(this.catalogDescHash);
            w.Write(this.catalogPrice);
            w.Write(this.catalogUnknown1);
            w.Write(this.catalogUnknown2);
            w.Write(this.catalogUnknown3);
            if (this.catalogStyleTGIList == null) this.catalogStyleTGIList = new CountedTGIBlockList(OnResourceChanged);
            w.Write((byte)this.catalogStyleTGIList.Count);
            foreach (var tgi in this.catalogStyleTGIList) tgi.UnParse(ms);

            w.Write(this.catalogUnknown4);
            if (this.catalogTagList == null) this.catalogTagList = new SimpleList<ushort>(OnResourceChanged);
            w.Write(this.catalogTagList.Count);
            foreach (var i in this.catalogTagList) w.Write(i);
            if (this.catalogSellingPointList == null) this.catalogSellingPointList = new SellingPointList(OnResourceChanged);
            this.catalogSellingPointList.UnParse(ms);
            w.Write(this.catalogUnknown5);
            w.Write(this.catalogUnknown6);
            w.Write(this.catalogUnknown7);
            return ms;
        }
        #endregion

        #region Sub-Class
        public class SellingPoint : AHandlerElement, IEquatable<SellingPoint>
        {
            #region Attributes
            const int recommendedApiVersion = 1;
            private ushort commodity;
            private uint amount;
            #endregion
            public SellingPoint(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { }


            #region Data I/O
            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.commodity = r.ReadUInt16();
                this.amount = r.ReadUInt32();
            }

            protected internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.commodity);
                w.Write(this.amount);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable
            public bool Equals(SellingPoint other)
            {
                return this.commodity == other.commodity && this.amount == other.amount;
            }
            #endregion

            #region Content Fields
            [ElementPriority(0)]
            public ushort Commodity { get { return this.commodity; } set { if (!this.commodity.Equals(value)) { OnElementChanged(); this.commodity = value; } } }
            [ElementPriority(1)]
            public uint Amount { get { return this.amount; } set { if (!this.amount.Equals(value)) { OnElementChanged(); this.amount = value; } } }
            #endregion
        }

        public class SellingPointList : DependentList<SellingPoint>
        {
            #region Constructors
            public SellingPointList(EventHandler handler) : base(handler) {  }
            public SellingPointList(EventHandler handler, Stream s) : base(handler) { Parse(s); }
            #endregion


            #region Data I/O
            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                var count = r.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    base.Add(new SellingPoint(1, handler, s));
                }
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(base.Count);
                foreach (var unknownClass in this)
                {
                    unknownClass.UnParse(s);
                }
            }

            protected override SellingPoint CreateElement(Stream s) { return new SellingPoint(1, handler, s); }
            protected override void WriteElement(Stream s, SellingPoint element) { element.UnParse(s); }
            #endregion

        }
        #endregion

        #region Content Fields
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        [ElementPriority(0)]
        public uint Version { get { return this.version; } set { if (!this.version.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.version = value; } } }
        [ElementPriority(1)]
        public uint CatalogVersion { get { return this.catalogVersion; } set { if (!this.catalogVersion.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogVersion = value; } } }
        [ElementPriority(2)]
        public uint CatalogNameHash { get { return this.catalogNameHash; } set { if (!this.catalogNameHash.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogNameHash = value; } } }
        [ElementPriority(3)]
        public uint CatalogDescHash { get { return this.catalogDescHash; } set { if (!this.catalogDescHash.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogDescHash = value; } } }
        [ElementPriority(4)]
        public uint CatalogPrice { get { return this.catalogPrice; } set { if (!this.catalogPrice.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogPrice = value; } } }
        [ElementPriority(5)]
        public uint CatalogUnknown1 { get { return this.catalogUnknown1; } set { if (!this.catalogUnknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogUnknown1 = value; } } }
        [ElementPriority(6)]
        public uint CatalogUnknown2 { get { return this.catalogUnknown2; } set { if (!this.catalogUnknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogUnknown2 = value; } } }
        [ElementPriority(7)]
        public uint CatalogUnknown3 { get { return this.catalogUnknown3; } set { if (!this.catalogUnknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogUnknown3 = value; } } }
        [ElementPriority(8)]
        public CountedTGIBlockList CatalogStyleTGIList { get { return this.catalogStyleTGIList; } set { if (!this.catalogStyleTGIList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogStyleTGIList = value; } } }
        [ElementPriority(9)]
        public ushort CatalogUnknown4 { get { return this.catalogUnknown4; } set { if (!this.catalogUnknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogUnknown4 = value; } } }
        [ElementPriority(10)]
        public SimpleList<ushort> CatalogTagList { get { return this.catalogTagList; } set { if (!this.catalogTagList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogTagList = value; } } }
        [ElementPriority(11)]
        public SellingPointList CatalogSellingPointsList { get { return this.catalogSellingPointList; } set { if (!this.catalogSellingPointList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogSellingPointList = value; } } }
        [ElementPriority(12)]
        public ulong CatalogUnknown5 { get { return this.catalogUnknown5; } set { if (!this.catalogUnknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogUnknown5 = value; } } }
        [ElementPriority(13)]
        public ushort CatalogUnknown6 { get { return this.catalogUnknown6; } set { if (!this.catalogUnknown6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogUnknown6 = value; } } }
        [ElementPriority(14)]
        public ulong CatalogUnknown7 { get { return this.catalogUnknown7; } set { if (!this.catalogUnknown7.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogUnknown7 = value; } } }
        public string Value { get { return ValueBuilder; } }
        #endregion
    }


    public class CatalogResourceHandler : AResourceHandler
    {
        public CatalogResourceHandler()
        {
            if (s4pi.Settings.Settings.IsTS4)
            {
                this.Add(typeof(WallCatalogResource), new List<string>(new string[] { "0xD5F0F921", }));
                this.Add(typeof(FloorCatalogResource), new List<string>(new string[] { "0xB4F762C9", }));
            }
        }
    }
}
