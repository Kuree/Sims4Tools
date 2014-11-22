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
    class ToolCatalogResource : ObjectCatalogResource
    {
        #region Attributes
        byte unknown1;
        byte unknown2;

        bool listFlag = false;
        UnknownList unknownList1;
        #endregion

        public ToolCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            this.unknown1 = r.ReadByte();
            this.unknown2 = r.ReadByte();

            if (r.BaseStream.Position != r.BaseStream.Length) this.listFlag = true;

            if (this.listFlag) this.unknownList1 = new UnknownList(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            var s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown1);
            w.Write(this.unknown2);

            if (this.listFlag)
            {
                if (this.unknownList1 == null) this.unknownList1 = new UnknownList(OnResourceChanged, s);
                this.unknownList1.UnParse(s);
            }
            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(15)]
        public byte Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(16)]
        public byte Unknown2 { get { return unknown2; } set { if (!unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(17)]
        public bool ListFlag { get { return listFlag; } set { if (!listFlag.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.listFlag = value; } } }
        [ElementPriority(18)]
        public UnknownList UnknownList1 { get { return unknownList1; } set { if (!unknownList1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownList1 = value; } } }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;

                if (!this.listFlag) res.Remove("UnknownList1");
                return res;
            }
        }
        #endregion

        #region Sub Class
        public class UnknownListEntry : AHandlerElement, IEquatable<UnknownListEntry>
        {
            private Byte unknown1;
            private Byte unknown2;
            private Byte unknown3;
            const int recommendedApiVersion = 1;

            public UnknownListEntry(int APIversion, EventHandler handler) : base(APIversion, handler) { this.UnParse(new MemoryStream()); }
            public UnknownListEntry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.unknown1 = r.ReadByte();
                this.unknown2 = r.ReadByte();
                this.unknown3 = r.ReadByte();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.unknown1);
                w.Write(this.unknown2);
                w.Write(this.unknown3);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public bool Equals(UnknownListEntry other)
            {
                return this.unknown1 == other.unknown1 && this.unknown2 == other.unknown2 && this.unknown3 == other.unknown3;
            }

            [ElementPriority(0)]
            public Byte Unknown1 { get { return this.unknown1; } set { if (!value.Equals(this.unknown1)) { OnElementChanged(); this.unknown1 = value; } } }
            public Byte Unknown2 { get { return this.unknown2; } set { if (!value.Equals(this.unknown2)) { OnElementChanged(); this.unknown2 = value; } } }
            public Byte Unknown3 { get { return this.unknown3; } set { if (!value.Equals(this.unknown3)) { OnElementChanged(); this.unknown3 = value; } } }
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
