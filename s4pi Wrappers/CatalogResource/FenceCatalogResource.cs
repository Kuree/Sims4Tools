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
using System.Collections.Generic;
using s4pi.Interfaces;
using System.IO;

namespace CatalogResource
{
    class FenceCatalogResource : ObjectCatalogResource
    {
        #region Attributes
        ModlEntryList modlEntryList1;
        ModlEntryList modlEntryList2;
        ModlEntryList modlEntryList3;
        ModlEntryList modlEntryList4;
        TGIBlock modlTGIReference1;
        TGIBlock modlTGIReference2;
        TGIBlock modlTGIReference3;
        TGIBlock modlTGIReference4;
        TGIBlock modlTGIReference5;
        TGIBlock modlTGIReference6;
        TGIBlock modlTGIReference7;
        uint unknown1;
        byte unknown2;
        uint unknown3;
        ulong catalogGroupID;
        TGIBlock rsltTGIReference;
        SimpleList<uint> unknownList1;
        SimpleList<uint> unknownList2;
        SimpleList<uint> unknownList3;
        SwatchColorList colorList;
        uint unknown4;
        #endregion

        public FenceCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.modlEntryList1 = new ModlEntryList(OnResourceChanged, s);
            this.modlEntryList2 = new ModlEntryList(OnResourceChanged, s);
            this.modlEntryList3 = new ModlEntryList(OnResourceChanged, s);
            this.modlEntryList4 = new ModlEntryList(OnResourceChanged, s);
            this.modlTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlTGIReference5 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlTGIReference6 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.modlTGIReference7 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadByte();
            this.unknown3 = r.ReadUInt32();
            this.catalogGroupID = r.ReadUInt64();
            this.rsltTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknownList1 = new SimpleList<uint>(OnResourceChanged);
            ushort count = r.ReadUInt16();
            for (int i = 0; i < count; i++) this.unknownList1.Add(r.ReadUInt32());
            this.unknownList2 = new SimpleList<uint>(OnResourceChanged);
            count = r.ReadUInt16();
            for (int i = 0; i < count; i++) this.unknownList2.Add(r.ReadUInt32());
            count = r.ReadUInt16();
            this.unknownList3 = new SimpleList<uint>(OnResourceChanged);
            for (int i = 0; i < count; i++) this.unknownList3.Add(r.ReadUInt32());
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.unknown4 = r.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            if (this.modlEntryList1 == null) this.modlEntryList1 = new ModlEntryList(OnResourceChanged, s);
            this.modlEntryList1.UnParse(s);
            if (this.modlEntryList2 == null) this.modlEntryList2 = new ModlEntryList(OnResourceChanged, s);
            this.modlEntryList2.UnParse(s);
            if (this.modlEntryList3 == null) this.modlEntryList3 = new ModlEntryList(OnResourceChanged, s);
            this.modlEntryList3.UnParse(s);
            if (this.modlEntryList4 == null) this.modlEntryList4 = new ModlEntryList(OnResourceChanged, s);
            this.modlEntryList4.UnParse(s);
            if (this.modlTGIReference1 == null) this.modlTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference1.UnParse(s);
            if (this.modlTGIReference2 == null) this.modlTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference2.UnParse(s);
            if (this.modlTGIReference3 == null) this.modlTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference3.UnParse(s);
            if (this.modlTGIReference4 == null) this.modlTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference4.UnParse(s);
            if (this.modlTGIReference5 == null) this.modlTGIReference5 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference5.UnParse(s);
            if (this.modlTGIReference6 == null) this.modlTGIReference6 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference6.UnParse(s);
            if (this.modlTGIReference7 == null) this.modlTGIReference7 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.modlTGIReference7.UnParse(s);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            w.Write(this.catalogGroupID);
            if (this.rsltTGIReference == null) this.rsltTGIReference = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
            this.rsltTGIReference.UnParse(s);
            if (this.unknownList1 == null) this.unknownList1 = new SimpleList<uint>(OnResourceChanged);
            w.Write((ushort)this.unknownList1.Count);
            this.unknownList1.UnParse(s);
            if (this.unknownList2 == null) this.unknownList2 = new SimpleList<uint>(OnResourceChanged);
            w.Write((ushort)this.unknownList2.Count);
            this.unknownList2.UnParse(s);
            if (this.unknownList3 == null) this.unknownList3 = new SimpleList<uint>(OnResourceChanged);
            w.Write((ushort)this.unknownList3.Count);
            this.unknownList3.UnParse(s);            
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            w.Write(this.unknown1);
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public ModlEntryList ModlEntryList1 { get { return modlEntryList1; } set { if (!modlEntryList1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlEntryList1 = value; } } }
        [ElementPriority(16)]
        public ModlEntryList ModlEntryList2 { get { return modlEntryList2; } set { if (!modlEntryList2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlEntryList2 = value; } } }
        [ElementPriority(17)]
        public ModlEntryList ModlEntryList3 { get { return modlEntryList3; } set { if (!modlEntryList3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlEntryList3 = value; } } }
        [ElementPriority(18)]
        public ModlEntryList ModlEntryList4 { get { return modlEntryList4; } set { if (!modlEntryList4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlEntryList4 = value; } } }
        [ElementPriority(19)]
        public TGIBlock ModlTGIReference1 { get { return modlTGIReference1; } set { if (!modlTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference1 = value; } } }
        [ElementPriority(20)]
        public TGIBlock ModlTGIReference2 { get { return modlTGIReference2; } set { if (!modlTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference2 = value; } } }
        [ElementPriority(21)]
        public TGIBlock ModlTGIReference3 { get { return modlTGIReference3; } set { if (!modlTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference3 = value; } } }
        [ElementPriority(22)]
        public TGIBlock ModlTGIReference4 { get { return modlTGIReference4; } set { if (!modlTGIReference4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference4 = value; } } }
        [ElementPriority(23)]
        public TGIBlock ModlTGIReference5 { get { return modlTGIReference5; } set { if (!modlTGIReference5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference5 = value; } } }
        [ElementPriority(24)]
        public TGIBlock ModlTGIReference6 { get { return modlTGIReference6; } set { if (!modlTGIReference6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference6 = value; } } }
        [ElementPriority(25)]
        public TGIBlock ModlTGIReference7 { get { return modlTGIReference7; } set { if (!modlTGIReference7.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference7 = value; } } }
        [ElementPriority(26)]
        public uint Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(27)]
        public byte Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(28)]
        public uint Unknown3 { get { return unknown3; } set { if (!unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(29)]
        public ulong CatalogGroupID { get { return catalogGroupID; } set { if (!catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(30)]
        public TGIBlock RsltTGIReference { get { return rsltTGIReference; } set { if (!rsltTGIReference.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.rsltTGIReference = value; } } }
        [ElementPriority(31)]
        public SimpleList<uint> UnknownList1 { get { return unknownList1; } set { if (!unknownList1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownList1 = value; } } }
        [ElementPriority(32)]
        public SimpleList<uint> UnknownList2 { get { return unknownList2; } set { if (!unknownList2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownList2 = value; } } }
        [ElementPriority(33)]
        public SimpleList<uint> UnknownList3 { get { return unknownList3; } set { if (!unknownList3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownList3 = value; } } }
        [ElementPriority(34)]
        public SwatchColorList ColorList { get { return colorList; } set { if (!colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(35)]
        public uint Unknown4 { get { return unknown4; } set { if (!unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown4 = value; } } }
        #endregion
        
        #region Clone
        #endregion
    }
}