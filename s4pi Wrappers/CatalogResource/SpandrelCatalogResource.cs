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
    class SpandrelCatalogResource : ObjectCatalogResource
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
        uint unknown;
        ulong catalogGroupID;
        SwatchColorList colorList;
        #endregion

        public SpandrelCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

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
            this.unknown = r.ReadUInt32();
            this.catalogGroupID = r.ReadUInt64();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
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
            w.Write(this.unknown);
            w.Write(this.catalogGroupID);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
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
        public uint Unknown { get { return unknown; } set { if (!unknown.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown = value; } } }
        [ElementPriority(27)]
        public ulong CatalogGroupID { get { return catalogGroupID; } set { if (!catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(28)]
        public SwatchColorList ColorList { get { return colorList; } set { if (!colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        #endregion

        #region Clone
        public override TGIBlock[] NestedTGIBlockList
        {
            get
            {
                List<TGIBlock> tgiList = new List<TGIBlock>();
                tgiList.Add(modlTGIReference1);
                tgiList.Add(modlTGIReference2);
                tgiList.Add(modlTGIReference3);
                tgiList.Add(modlTGIReference4);
                tgiList.Add(modlTGIReference5);
                tgiList.Add(modlTGIReference6);
                tgiList.Add(modlTGIReference7);
                if (this.modlEntryList1 != null)
                    foreach (var entry in this.modlEntryList1)
                        tgiList.Add(entry.MODLReference);
                if (this.modlEntryList2 != null)
                    foreach (var entry in this.modlEntryList1)
                        tgiList.Add(entry.MODLReference);
                if (this.modlEntryList3 != null)
                    foreach (var entry in this.modlEntryList1)
                        tgiList.Add(entry.MODLReference);
                if (this.modlEntryList4 != null)
                    foreach (var entry in this.modlEntryList1)
                        tgiList.Add(entry.MODLReference);
                return tgiList.ToArray();
            }
            set
            {
                base.SetTGIList(value);
            }
        }

        protected override object GroupingID { get { return this.CatalogGroupID; } set { this.CatalogGroupID = (ulong)value; } }
        #endregion
    }
}