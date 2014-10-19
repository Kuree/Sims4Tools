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
using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RCOLResource
{
    public class MTST : RCOLChunk
    {
        [ElementPriority(0)]
        public static RCOL.RCOLChunkType RCOLType { get { return RCOL.RCOLChunkType.MTST; } }

        public MTST(int APIversion, EventHandler handler, Stream s, TGIBlock currentTGI) : base(APIversion, handler, s, currentTGI) { }

        #region Attributes
        uint version;
        uint namehash;
        uint defaultMaterial;
        MaterialList materialList;
        static bool checking = s4pi.Settings.Settings.Checking;
        const int recommendedApiVersion = 1;
        #endregion

        #region Data I/O
        protected override void Parse(System.IO.Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            uint tag = r.ReadUInt32();
            if (tag != (uint)FOURCC(RCOLType.ToString())) throw new InvalidDataException(string.Format("Except to read 0x{0:8X}, read 0x{1:8X}", RCOLType, tag));
            this.version = r.ReadUInt32();
            this.namehash = r.ReadUInt32();
            this.defaultMaterial = r.ReadUInt32();
            this.materialList = new MaterialList(handler, s);
        }

        protected internal override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write((uint)RCOLType);
            w.Write(this.version);
            w.Write(this.namehash);
            w.Write(this.defaultMaterial);
            this.materialList.UnParse(s);
        }
        #endregion


        #region Sub Class
        public enum State : uint
        {
            Default = 0x2EA8FB98,
            Dirty = 0xEEAB4327,
            VeryDirty = 0x2E5DF9BB,
            Burnt = 0xC3867C32,
            Clogged = 0x257FB026,
            carLightsOff = 0xE4AF52C1,
        }

        public class Material : AHandlerElement, IEquatable<Material>
        {
            public Material(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s);}

            #region Attributes
            uint mat;
            State stateID;
            uint varientID;
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region Data I/O
            protected void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.mat = r.ReadUInt32();
                this.stateID = (State)r.ReadUInt32();
                this.varientID = r.ReadUInt32();
            }

            protected internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.mat);
                w.Write((uint)this.stateID);
                w.Write(this.varientID);
            }
            #endregion

            #region IEquatable
            public bool Equals(Material other)
            {
                return this.mat == other.mat && this.stateID == other.stateID && this.varientID == other.varientID;
            }
            #endregion

            #region Content Fields
            [ElementPriority(0)]
            public uint Mat { get { return this.mat; } set { if (!this.mat.Equals(value)) { OnElementChanged(); this.mat = value; } } }
            [ElementPriority(1)]
            public State StateID { get { return this.stateID; } set { if (!this.stateID.Equals(value)) { OnElementChanged(); this.stateID = value; } } }
            [ElementPriority(2)]
            public uint VarientID { get { return this.varientID; } set { if (!this.varientID.Equals(value)) { OnElementChanged(); this.varientID = value; } } }
            public string Value { get { return ValueBuilder; } }
            #endregion
        }

        public class MaterialList : DependentList<Material>
        {
            public MaterialList(EventHandler handler, Stream s) : base(handler) { Parse(s); }
            #region Data I/O
            protected internal void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                uint count = r.ReadUInt32();
                for (uint i = 0; i < count; i++) this.Add(new Material(1, handler, s));
            }

            protected internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.Count);
                foreach (var m in this) m.UnParse(s);
            }
            #endregion
            protected override Material CreateElement(Stream s) { throw new NotSupportedException(); }
            protected override void WriteElement(Stream s, Material element) { throw new NotSupportedException(); }
        }

        #endregion

        #region Content Fields
        [ElementPriority(2)]
        public uint Version { get { return this.version; } set { if (!this.version.Equals(value)) { OnElementChanged(); this.version = value; } } }
        [ElementPriority(3)]
        public uint Namehash { get { return this.namehash; } set { if (!this.namehash.Equals(value)) { OnElementChanged(); this.namehash = value; } } }
        [ElementPriority(4)]
        public uint DefaultMaterial { get { return this.defaultMaterial; } set { if (!this.defaultMaterial.Equals(value)) { OnElementChanged(); this.defaultMaterial = value; } } }
        [ElementPriority(5)]
        public MaterialList Materialist { get { return this.materialList; } set { if (!this.materialList.Equals(value)) { OnElementChanged(); this.materialList = value; } } }
        public override string RCOLTag { get { return RCOLType.ToString(); } }
        #endregion
    }
}
