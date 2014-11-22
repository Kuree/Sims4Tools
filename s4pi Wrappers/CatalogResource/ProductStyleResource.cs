/***************************************************************************
 *  Copyright (C) 2014 by Snaitf                                           *
 *  http://modthesims.info/member/Snaitf                                   *
 *  Keyi Zhang kz005@bucknell.edu                                          *
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
using System.IO;
using s4pi.Interfaces;
using System.Collections.Generic;

namespace CatalogResource
{
    class ProductStyleResource : ObjectCatalogResource
    {
        #region Attributes
        TGIBlock cwalTGIReference1;
        TGIBlock cwalTGIReference2;
        TGIBlock cfenTGIReference;
        TGIBlock cspnTGIReference;
        TGIBlock cflrTGIReference1;
        TGIBlock cflrTGIReference2;
        TGIBlock cobjTGIReference1;
        TGIBlock cobjTGIReference2;
        TGIBlock unknTGIReference;
        TGIBlock cfndTGIReference;
        TGIBlock ccolTGIReference;
        TGIBlock cstrTGIReference;
        TGIBlock crmtTGIReference;
        TGIBlock cobjTGIReference3;
        TGIBlock cxtrTGIReference;
        TGIBlock nullTGIReference1;
        TGIBlock crtrTGIReference;
        TGIBlock cfrzTGIReference;
        TGIBlock cwalTGIReference3;
        TGIBlock cflrTGIReference3;
        TGIBlock cflrTGIReference4;

        TGIBlock nullTGIReference2;
        TGIBlock nullTGIReference3;
        TGIBlock cwalTGIReference4;
        TGIBlock cflrTGIReference5;

        UnknownList unknownList1;

        uint unknown1;
        uint unknown2;
        uint unknown3;
        uint unknown4;
        byte unknown5;
        ulong imgIIDReference;
        #endregion

        public ProductStyleResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.cwalTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cwalTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cfenTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cspnTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cflrTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cflrTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cobjTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cobjTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cfndTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.ccolTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cstrTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.crmtTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cobjTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cxtrTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.nullTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.crtrTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cfrzTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cwalTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cflrTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.cflrTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);

            if (base.Version >= 0x0E)
            {
                this.nullTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.nullTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.cwalTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.cflrTGIReference5 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            }

            this.unknownList1 = new UnknownList(OnResourceChanged, s);

            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.unknown4 = r.ReadUInt32();
            this.unknown5 = r.ReadByte();
            this.imgIIDReference = r.ReadUInt64();
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (this.cwalTGIReference1 == null) this.cwalTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cwalTGIReference1.UnParse(s);
            if (this.cwalTGIReference2 == null) this.cwalTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cwalTGIReference2.UnParse(s);
            if (this.cfenTGIReference == null) this.cfenTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cfenTGIReference.UnParse(s);
            if (this.cspnTGIReference == null) this.cspnTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cspnTGIReference.UnParse(s);
            if (this.cflrTGIReference1 == null) this.cflrTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cflrTGIReference1.UnParse(s);
            if (this.cflrTGIReference2 == null) this.cflrTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cflrTGIReference2.UnParse(s);
            if (this.cobjTGIReference1 == null) this.cobjTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cobjTGIReference1.UnParse(s);
            if (this.cobjTGIReference2 == null) this.cobjTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cobjTGIReference2.UnParse(s);
            if (this.unknTGIReference == null) this.unknTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.unknTGIReference.UnParse(s);
            if (this.cfndTGIReference == null) this.cfndTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cfndTGIReference.UnParse(s);
            if (this.ccolTGIReference == null) this.ccolTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.ccolTGIReference.UnParse(s);
            if (this.cstrTGIReference == null) this.cstrTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cstrTGIReference.UnParse(s);
            if (this.crmtTGIReference == null) this.crmtTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.crmtTGIReference.UnParse(s);
            if (this.cobjTGIReference3 == null) this.cobjTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cobjTGIReference3.UnParse(s);
            if (this.cxtrTGIReference == null) this.cxtrTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cxtrTGIReference.UnParse(s);
            if (this.nullTGIReference1 == null) this.nullTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.nullTGIReference1.UnParse(s);
            if (this.crtrTGIReference == null) this.crtrTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.crtrTGIReference.UnParse(s);
            if (this.cfrzTGIReference == null) this.cfrzTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cfrzTGIReference.UnParse(s);
            if (this.cwalTGIReference3 == null) this.cwalTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cwalTGIReference3.UnParse(s);
            if (this.cflrTGIReference3 == null) this.cflrTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cflrTGIReference3.UnParse(s);
            if (this.cflrTGIReference4 == null) this.cflrTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.cflrTGIReference4.UnParse(s);

            if (base.Version >= 0x0E)
            {
                if (this.nullTGIReference2 == null) this.nullTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.nullTGIReference2.UnParse(s);
                if (this.nullTGIReference3 == null) this.nullTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.nullTGIReference3.UnParse(s);
                if (this.cwalTGIReference4 == null) this.cwalTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.cwalTGIReference4.UnParse(s);
                if (this.cflrTGIReference5 == null) this.cflrTGIReference5 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.cflrTGIReference5.UnParse(s);
            }

            if (this.unknownList1 == null) this.unknownList1 = new UnknownList(OnResourceChanged, s);
            this.unknownList1.UnParse(s);
            
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            w.Write(this.unknown4);
            w.Write(this.unknown5);
            w.Write(this.imgIIDReference);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public TGIBlock CwalTGIReference1 { get { return cwalTGIReference1; } set { if (!cwalTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cwalTGIReference1 = value; } } }
        [ElementPriority(16)]
        public TGIBlock CwalTGIReference2 { get { return cwalTGIReference2; } set { if (!cwalTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cwalTGIReference2 = value; } } }
        [ElementPriority(17)]
        public TGIBlock CfenTGIReference { get { return cfenTGIReference; } set { if (!cfenTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cfenTGIReference = value; } } }
        [ElementPriority(18)]
        public TGIBlock CspnTGIReference { get { return cspnTGIReference; } set { if (!cspnTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cspnTGIReference = value; } } }
        [ElementPriority(19)]
        public TGIBlock CflrTGIReference1 { get { return cflrTGIReference1; } set { if (!cflrTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cflrTGIReference1 = value; } } }
        [ElementPriority(20)]
        public TGIBlock CflrTGIReference2 { get { return cflrTGIReference2; } set { if (!cflrTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cflrTGIReference2 = value; } } }
        [ElementPriority(21)]
        public TGIBlock CobjTGIReference1 { get { return cobjTGIReference1; } set { if (!cobjTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cobjTGIReference1 = value; } } }
        [ElementPriority(22)]
        public TGIBlock CobjTGIReference2 { get { return cobjTGIReference2; } set { if (!cobjTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cobjTGIReference2 = value; } } }
        [ElementPriority(23)]
        public TGIBlock UnknTGIReference { get { return unknTGIReference; } set { if (!unknTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknTGIReference = value; } } }
        [ElementPriority(24)]
        public TGIBlock CfndTGIReference { get { return cfndTGIReference; } set { if (!cfndTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cfndTGIReference = value; } } }
        [ElementPriority(25)]
        public TGIBlock CcolTGIReference { get { return ccolTGIReference; } set { if (!ccolTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ccolTGIReference = value; } } }
        [ElementPriority(26)]
        public TGIBlock CstrTGIReference { get { return cstrTGIReference; } set { if (!cstrTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cstrTGIReference = value; } } }
        [ElementPriority(27)]
        public TGIBlock CrmtTGIReference { get { return crmtTGIReference; } set { if (!crmtTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.crmtTGIReference = value; } } }
        [ElementPriority(28)]
        public TGIBlock CobjTGIReference3 { get { return cobjTGIReference3; } set { if (!cobjTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cobjTGIReference3 = value; } } }
        [ElementPriority(29)]
        public TGIBlock CxtrTGIReference { get { return cxtrTGIReference; } set { if (!cxtrTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cxtrTGIReference = value; } } }
        [ElementPriority(30)]
        public TGIBlock NullTGIReference1 { get { return nullTGIReference1; } set { if (!nullTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.nullTGIReference1 = value; } } }
        [ElementPriority(31)]
        public TGIBlock CrtrTGIReference { get { return crtrTGIReference; } set { if (!crtrTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.crtrTGIReference = value; } } }
        [ElementPriority(32)]
        public TGIBlock CfrzTGIReference { get { return cfrzTGIReference; } set { if (!cfrzTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cfrzTGIReference = value; } } }
        [ElementPriority(33)]
        public TGIBlock CwalTGIReference3 { get { return cwalTGIReference3; } set { if (!cwalTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cwalTGIReference3 = value; } } }
        [ElementPriority(34)]
        public TGIBlock CflrTGIReference3 { get { return cflrTGIReference3; } set { if (!cflrTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cflrTGIReference3 = value; } } }
        [ElementPriority(35)]
        public TGIBlock CflrTGIReference4 { get { return cflrTGIReference4; } set { if (!cflrTGIReference4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cflrTGIReference4 = value; } } }

        [ElementPriority(36)]
        public TGIBlock NullTGIReference2 { get { return nullTGIReference2; } set { if (!nullTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.nullTGIReference2 = value; } } }
        [ElementPriority(37)]
        public TGIBlock NullTGIReference3 { get { return nullTGIReference3; } set { if (!nullTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.nullTGIReference3 = value; } } }
        [ElementPriority(38)]
        public TGIBlock CwalTGIReference4 { get { return cwalTGIReference4; } set { if (!cwalTGIReference4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cwalTGIReference4 = value; } } }
        [ElementPriority(39)]
        public TGIBlock CflrTGIReference5 { get { return cflrTGIReference5; } set { if (!cflrTGIReference5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.cflrTGIReference5 = value; } } }

        [ElementPriority(40)]
        public UnknownList UnknownList1 { get { return unknownList1; } set { if (!unknownList1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownList1 = value; } } }

        [ElementPriority(41)]
        public uint Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(42)]
        public uint Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(43)]
        public uint Unknown3 { get { return unknown3; } set { if (!unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(44)]
        public uint Unknown4 { get { return unknown4; } set { if (!unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown4 = value; } } }
        [ElementPriority(45)]
        public byte Unknown5 { get { return unknown5; } set { if (!unknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown5 = value; } } }
        [ElementPriority(46)]
        public ulong ImgIIDReference { get { return imgIIDReference; } set { if (!imgIIDReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.imgIIDReference = value; } } }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;

                if (base.Version <= 0x0D)
                {
                    res.Remove("NullTGIReference2");
                    res.Remove("NullTGIReference3");
                    res.Remove("CwalTGIReference4");
                    res.Remove("CflrTGIReference5");
                }
                return res;
            }
        }
        #endregion

        #region Sub Class
        public class UnknownListEntry : AHandlerElement, IEquatable<UnknownListEntry>
        {
            private UInt32 unknown;
            const int recommendedApiVersion = 1;

            public UnknownListEntry(int APIversion, EventHandler handler) : base(APIversion, handler) { this.UnParse(new MemoryStream()); }
            public UnknownListEntry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.unknown = r.ReadUInt32();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.unknown);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public bool Equals(UnknownListEntry other)
            {
                return this.unknown == other.unknown;
            }

            [ElementPriority(0)]
            public UInt32 Unknown { get { return this.unknown; } set { if (!value.Equals(this.unknown)) { OnElementChanged(); this.unknown = value; } } }
            public string Value { get { return ValueBuilder; } }
        }

        public class UnknownList : DependentList<UnknownListEntry>
        {
            #region Constructors
            public UnknownList(EventHandler handler) : base(handler) { }
            public UnknownList(EventHandler handler, Stream s) : base(handler) { Parse(s); }
            #endregion

            #region Data I/O
            protected override void Parse(Stream s)
            {
                int count = s.ReadByte();
                for (byte i = 0; i < count; i++)
                    this.Add(new UnknownListEntry(1, handler, s));
            }

            public override void UnParse(Stream s)
            {
                s.WriteByte((byte)this.Count);
                foreach (var unknown in this)
                    unknown.UnParse(s);
            }

            protected override UnknownListEntry CreateElement(Stream s) { return new UnknownListEntry(1, handler, s); }
            protected override void WriteElement(Stream s, UnknownListEntry element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region Clone
        // Clone Code
        #endregion
    }
}
