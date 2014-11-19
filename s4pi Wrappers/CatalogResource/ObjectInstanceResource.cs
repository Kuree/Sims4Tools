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
using System.Linq;
using System.Text;
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    public class ObjectInstanceResource : ObjectCatalogResource
    {
        #region Attributes
        private DataBlobHandler unknown1;
        private uint unknownFlags1;
        private uint unknownFlags2;
        private uint unknownFlags3;
        private uint unknownFlags4;
        private uint unknownFlags5;
        private ulong unknownInstance1;
        private byte unknown2;
        private ulong unknownInstance2;
        private byte unknown3;
        private SwatchColorList colorList;
        private DataBlobHandler unknownFlags;
        private bool buildBuyMode;
        private uint unknown4;
        private uint unknown5;
        private uint unknown6;
        private uint unknown7;
        #endregion

        public ObjectInstanceResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.unknown1 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(7 * 4));
            this.unknownFlags1 = r.ReadUInt32();
            this.unknownFlags2 = r.ReadUInt32();
            this.unknownFlags3 = r.ReadUInt32();
            this.unknownFlags4 = r.ReadUInt32();
            this.unknownFlags5 = r.ReadUInt32();
            this.unknownInstance1 = r.ReadUInt64();
            this.unknown2 = r.ReadByte();
            this.unknownInstance2 = r.ReadUInt64();
            this.unknown3 = r.ReadByte();
            this.colorList = new SwatchColorList(OnResourceChanged, s);
            this.unknownFlags = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(5));
            this.buildBuyMode = r.ReadBoolean();
            if (base.Version >= 0x19)
            {
                this.unknown4 = r.ReadUInt32();
                this.unknown5 = r.ReadUInt32();
                this.unknown6 = r.ReadUInt32();
                this.unknown7 = r.ReadUInt32();
            }
        }

        protected override Stream UnParse()
        {
            var s =  base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            if (this.unknown1 == null) this.unknown1 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, new byte[7 * 4]);
            this.unknown1.UnParse(s);
            w.Write(this.unknownFlags1);
            w.Write(this.unknownFlags2);
            w.Write(this.unknownFlags3);
            w.Write(this.unknownFlags4);
            w.Write(this.unknownFlags5);
            w.Write(this.unknownInstance1);
            w.Write(this.unknown2);
            w.Write(this.unknownInstance2);
            w.Write(this.unknown3);
            if (this.colorList == null) this.colorList = new SwatchColorList(OnResourceChanged);
            this.colorList.UnParse(s);
            if (this.unknownFlags == null) this.unknownFlags = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, new byte[5]);
            this.unknownFlags.UnParse(s);
            w.Write(this.buildBuyMode);
            if (base.Version >= 0x19)
            {
                w.Write(this.unknown4);
                w.Write(this.unknown5);
                w.Write(this.unknown6);
                w.Write(this.unknown7);
            }
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public DataBlobHandler Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(16)]
        public uint UnknownFlag1 { get { return this.unknownFlags1; } set { if (!this.unknownFlags1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags1 = value; } } }
        [ElementPriority(17)]
        public uint UnknownFlag2 { get { return this.unknownFlags2; } set { if (!this.unknownFlags2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags2 = value; } } }
        [ElementPriority(18)]
        public uint UnknownFlag3 { get { return this.unknownFlags3; } set { if (!this.unknownFlags3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags3 = value; } } }
        [ElementPriority(19)]
        public uint UnknownFlag4 { get { return this.unknownFlags4; } set { if (!this.unknownFlags4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags4 = value; } } }
        [ElementPriority(20)]
        public uint UnknownFlag5 { get { return this.unknownFlags5; } set { if (!this.unknownFlags5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags5 = value; } } }
        [ElementPriority(21)]
        public ulong UnknownInstance1 { get { return this.unknownInstance1; } set { if (!this.unknownInstance1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownInstance1 = value; } } }
        [ElementPriority(22)]
        public byte Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(23)]
        public ulong UnknownInstance2 { get { return this.unknownInstance2; } set { if (!this.unknownInstance2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownInstance2 = value; } } }
        [ElementPriority(24)]
        public byte Unknown3 { get { return this.unknown3; } set { if (!this.unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(25)]
        public SwatchColorList SwatchColors { get { return this.colorList; } set { if (!this.colorList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorList = value; } } }
        [ElementPriority(26)]
        public DataBlobHandler UnknownFlags { get { return this.unknownFlags; } set { if (!this.unknownFlags.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownFlags = value; } } }
        [ElementPriority(27)]
        public bool BuildBuyMode { get { return this.buildBuyMode; } set { if (!this.buildBuyMode.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.buildBuyMode = value; } } }
        [ElementPriority(28)]
        public uint Unknown4 { get { return this.unknown4; } set { if (!this.unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown4 = value; } } }
        [ElementPriority(29)]
        public uint Unknown5 { get { return this.unknown5; } set { if (!this.unknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown5 = value; } } }
        [ElementPriority(30)]
        public uint Unknown6 { get { return this.unknown6; } set { if (!this.unknown6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown6 = value; } } }
        [ElementPriority(31)]
        public uint Unknown7 { get { return this.unknown7; } set { if (!this.unknown7.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown7 = value; } } }
        public override List<string> ContentFields { get { var res = base.ContentFields; if (base.Version >= 0x13) { res.Remove("Unknown3"); } return res; } }
        #endregion

        #region Clone Code
        #endregion   
    }
}
