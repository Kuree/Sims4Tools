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
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    class ColumnCatalogResource : ObjectCatalogResource
    {
        #region Attributes
        uint unknown1;
        uint unknown2;
        uint unknown3;
        uint unknown4;
        uint unknown5;
        uint unknown6;
        uint unknown7;
        DataBlobHandler dataBlob1;
        uint unknown8;
        ulong unknown9;
        ulong catalogGroupID;
        byte unknown10;
        SwatchColorList colorList;
        uint unknown11;
        ushort unknown12;
        DataBlobHandler dataBlob2;
        uint unknown13;
        uint tgiReferenceFlag;

        TGIBlock modlTGIReference1;
        TGIBlock modlTGIReference2;
        TGIBlock modlTGIReference3;
        TGIBlock modlTGIReference4;
        TGIBlock modlTGIReference5;
        TGIBlock modlTGIReference6;
        TGIBlock modlTGIReference7;
        TGIBlock modlTGIReference8;
        TGIBlock modlTGIReference9;

        TGIBlock ftptTGIReference1;
        TGIBlock ftptTGIReference2;
        TGIBlock ftptTGIReference3;
        TGIBlock ftptTGIReference4;
        TGIBlock ftptTGIReference5;
        TGIBlock ftptTGIReference6;
        TGIBlock ftptTGIReference7;
        TGIBlock ftptTGIReference8;
        TGIBlock ftptTGIReference9;

        TGIBlock nullTGIReference1;
        TGIBlock nullTGIReference2;
        TGIBlock nullTGIReference3;
        TGIBlock nullTGIReference4;
        #endregion

        public ColumnCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.unknown4 = r.ReadUInt32();
            this.unknown5 = r.ReadUInt32();
            this.unknown6 = r.ReadUInt32();
            this.unknown7 = r.ReadUInt32();
            this.dataBlob1 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(17));
            this.unknown8 = r.ReadUInt32();
            this.unknown9 = r.ReadUInt64();
            this.catalogGroupID = r.ReadUInt64();
            this.unknown10 = r.ReadByte();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.unknown11 = r.ReadUInt32();
            this.unknown12 = r.ReadUInt16();
            if (base.Version >= 0x19) this.dataBlob2 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(16));
            this.unknown13 = r.ReadUInt32();
            this.tgiReferenceFlag = r.ReadUInt32();

            if (this.tgiReferenceFlag == 0x01)
            {
                this.modlTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.modlTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.modlTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.modlTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.modlTGIReference5 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.modlTGIReference6 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.modlTGIReference7 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.modlTGIReference8 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.modlTGIReference9 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);

                this.ftptTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.ftptTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.ftptTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.ftptTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.ftptTGIReference5 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.ftptTGIReference6 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.ftptTGIReference7 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.ftptTGIReference8 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.ftptTGIReference9 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            }
            else
            {
                this.nullTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.nullTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.nullTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
                this.nullTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG", s);
            }
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            w.Write(this.unknown4);
            w.Write(this.unknown5);
            w.Write(this.unknown6);
            w.Write(this.unknown7);
            if (this.dataBlob1 == null) this.dataBlob1 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, new byte[17]);
            this.dataBlob1.UnParse(s);
            w.Write(this.unknown8);
            w.Write(this.unknown9);
            w.Write(this.catalogGroupID);
            w.Write(this.unknown10);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            w.Write(this.unknown11);
            w.Write(this.unknown12);
            if (base.Version >= 0x19)
            {
                if (this.dataBlob2 == null) this.dataBlob2 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, new byte[16]);
                this.dataBlob2.UnParse(s);
            }
            w.Write(this.unknown13);
            w.Write(this.tgiReferenceFlag);

            if (this.tgiReferenceFlag == 0x01)
            {
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
                if (this.modlTGIReference8 == null) this.modlTGIReference8 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.modlTGIReference8.UnParse(s);
                if (this.modlTGIReference9 == null) this.modlTGIReference9 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.modlTGIReference9.UnParse(s);

                if (this.ftptTGIReference1 == null) this.ftptTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.ftptTGIReference1.UnParse(s);
                if (this.ftptTGIReference2 == null) this.ftptTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.ftptTGIReference2.UnParse(s);
                if (this.ftptTGIReference3 == null) this.ftptTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.ftptTGIReference3.UnParse(s);
                if (this.ftptTGIReference4 == null) this.ftptTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.ftptTGIReference4.UnParse(s);
                if (this.ftptTGIReference5 == null) this.ftptTGIReference5 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.ftptTGIReference5.UnParse(s);
                if (this.ftptTGIReference6 == null) this.ftptTGIReference6 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.ftptTGIReference6.UnParse(s);
                if (this.ftptTGIReference7 == null) this.ftptTGIReference7 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.ftptTGIReference7.UnParse(s);
                if (this.ftptTGIReference8 == null) this.ftptTGIReference8 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.ftptTGIReference8.UnParse(s);
                if (this.ftptTGIReference9 == null) this.ftptTGIReference9 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.ftptTGIReference9.UnParse(s);
            }
            else
            {
                if (this.nullTGIReference1 == null) this.nullTGIReference1 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.nullTGIReference1.UnParse(s);
                if (this.nullTGIReference2 == null) this.nullTGIReference2 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.nullTGIReference2.UnParse(s);
                if (this.nullTGIReference3 == null) this.nullTGIReference3 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.nullTGIReference3.UnParse(s);
                if (this.nullTGIReference4 == null) this.nullTGIReference4 = new TGIBlock(RecommendedApiVersion, OnResourceChanged, "ITG");
                this.nullTGIReference4.UnParse(s);
            }
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public uint Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(16)]
        public uint Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(17)]
        public uint Unknown3 { get { return unknown3; } set { if (!unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(18)]
        public uint Unknown4 { get { return unknown4; } set { if (!unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown4 = value; } } }
        [ElementPriority(19)]
        public uint Unknown5 { get { return unknown5; } set { if (!unknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown5 = value; } } }
        [ElementPriority(20)]
        public uint Unknown6 { get { return unknown6; } set { if (!unknown6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown6 = value; } } }
        [ElementPriority(21)]
        public uint Unknown7 { get { return unknown7; } set { if (!unknown7.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown7 = value; } } }
        [ElementPriority(22)]
        public DataBlobHandler DataBlob1 { get { return this.dataBlob1; } set { if (!this.dataBlob1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.dataBlob1 = value; } } }
        [ElementPriority(23)]
        public uint Unknown8 { get { return unknown8; } set { if (!unknown8.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown8 = value; } } }
        [ElementPriority(24)]
        public ulong Unknown9 { get { return unknown9; } set { if (!unknown9.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown9 = value; } } }
        [ElementPriority(25)]
        public ulong CatalogGroupID { get { return catalogGroupID; } set { if (!catalogGroupID.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.catalogGroupID = value; } } }
        [ElementPriority(26)]
        public byte Unknown10 { get { return unknown10; } set { if (!unknown10.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown10 = value; } } }
        [ElementPriority(27)]
        public SwatchColorList ColorList { get { return colorList; } set { if (!colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(28)]
        public uint Unknown11 { get { return unknown11; } set { if (!unknown11.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown11 = value; } } }
        [ElementPriority(29)]
        public ushort Unknown12 { get { return unknown12; } set { if (!unknown12.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown12 = value; } } }
        [ElementPriority(30)]
        public DataBlobHandler DataBlob2 { get { return this.dataBlob2; } set { if (!this.dataBlob2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.dataBlob2 = value; } } }
        [ElementPriority(31)]
        public uint Unknown13 { get { return unknown13; } set { if (!unknown13.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown13 = value; } } }
        [ElementPriority(32)]
        public uint TgiReferenceFlag { get { return tgiReferenceFlag; } set { if (!tgiReferenceFlag.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.tgiReferenceFlag = value; } } }

        [ElementPriority(33)]
        public TGIBlock ModlTGIReference1 { get { return modlTGIReference1; } set { if (!modlTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference1 = value; } } }
        [ElementPriority(34)]
        public TGIBlock ModlTGIReference2 { get { return modlTGIReference2; } set { if (!modlTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference2 = value; } } }
        [ElementPriority(35)]
        public TGIBlock ModlTGIReference3 { get { return modlTGIReference3; } set { if (!modlTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference3 = value; } } }
        [ElementPriority(36)]
        public TGIBlock ModlTGIReference4 { get { return modlTGIReference4; } set { if (!modlTGIReference4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference4 = value; } } }
        [ElementPriority(37)]
        public TGIBlock ModlTGIReference5 { get { return modlTGIReference5; } set { if (!modlTGIReference5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference5 = value; } } }
        [ElementPriority(38)]
        public TGIBlock ModlTGIReference6 { get { return modlTGIReference6; } set { if (!modlTGIReference6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference6 = value; } } }
        [ElementPriority(39)]
        public TGIBlock ModlTGIReference7 { get { return modlTGIReference7; } set { if (!modlTGIReference7.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference7 = value; } } }
        [ElementPriority(40)]
        public TGIBlock ModlTGIReference8 { get { return modlTGIReference8; } set { if (!modlTGIReference8.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference8 = value; } } }
        [ElementPriority(41)]
        public TGIBlock ModlTGIReference9 { get { return modlTGIReference9; } set { if (!modlTGIReference9.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.modlTGIReference9 = value; } } }

        [ElementPriority(42)]
        public TGIBlock FtptTGIReference1 { get { return ftptTGIReference1; } set { if (!ftptTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ftptTGIReference1 = value; } } }
        [ElementPriority(43)]
        public TGIBlock FtptTGIReference2 { get { return ftptTGIReference2; } set { if (!ftptTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ftptTGIReference2 = value; } } }
        [ElementPriority(44)]
        public TGIBlock FtptTGIReference3 { get { return ftptTGIReference3; } set { if (!ftptTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ftptTGIReference3 = value; } } }
        [ElementPriority(45)]
        public TGIBlock FtptTGIReference4 { get { return ftptTGIReference4; } set { if (!ftptTGIReference4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ftptTGIReference4 = value; } } }
        [ElementPriority(46)]
        public TGIBlock FtptTGIReference5 { get { return ftptTGIReference5; } set { if (!ftptTGIReference5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ftptTGIReference5 = value; } } }
        [ElementPriority(47)]
        public TGIBlock FtptTGIReference6 { get { return ftptTGIReference6; } set { if (!ftptTGIReference6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ftptTGIReference6 = value; } } }
        [ElementPriority(48)]
        public TGIBlock FtptTGIReference7 { get { return ftptTGIReference7; } set { if (!ftptTGIReference7.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ftptTGIReference7 = value; } } }
        [ElementPriority(49)]
        public TGIBlock FtptTGIReference8 { get { return ftptTGIReference8; } set { if (!ftptTGIReference8.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ftptTGIReference8 = value; } } }
        [ElementPriority(50)]
        public TGIBlock FtptTGIReference9 { get { return ftptTGIReference9; } set { if (!ftptTGIReference9.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ftptTGIReference9 = value; } } }

        [ElementPriority(51)]
        public TGIBlock NullTGIReference1 { get { return nullTGIReference1; } set { if (!nullTGIReference1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.nullTGIReference1 = value; } } }
        [ElementPriority(52)]
        public TGIBlock NullTGIReference2 { get { return nullTGIReference2; } set { if (!nullTGIReference2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.nullTGIReference2 = value; } } }
        [ElementPriority(53)]
        public TGIBlock NullTGIReference3 { get { return nullTGIReference3; } set { if (!nullTGIReference3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.nullTGIReference3 = value; } } }
        [ElementPriority(54)]
        public TGIBlock NullTGIReference4 { get { return nullTGIReference4; } set { if (!nullTGIReference4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.nullTGIReference4 = value; } } }        
       
        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;

                if (base.Version <= 0x18) res.Remove("DataBlob2");

                if (this.tgiReferenceFlag == 0x01)
                {
                    res.Remove("NullTGIReference1");
                    res.Remove("NullTGIReference2");
                    res.Remove("NullTGIReference3");
                    res.Remove("NullTGIReference4");
                }
                else
                {
                    res.Remove("ModlTGIReference1");
                    res.Remove("ModlTGIReference2");
                    res.Remove("ModlTGIReference3");
                    res.Remove("ModlTGIReference4");
                    res.Remove("ModlTGIReference5");
                    res.Remove("ModlTGIReference6");
                    res.Remove("ModlTGIReference7");
                    res.Remove("ModlTGIReference8");
                    res.Remove("ModlTGIReference9");

                    res.Remove("FtptTGIReference1");
                    res.Remove("FtptTGIReference2");
                    res.Remove("FtptTGIReference3");
                    res.Remove("FtptTGIReference4");
                    res.Remove("FtptTGIReference5");
                    res.Remove("FtptTGIReference6");
                    res.Remove("FtptTGIReference7");
                    res.Remove("FtptTGIReference8");
                    res.Remove("FtptTGIReference9");
                }
                return res;
            }
        }
        #endregion

        #region Clone
        // Clone Code
        #endregion
    }
}