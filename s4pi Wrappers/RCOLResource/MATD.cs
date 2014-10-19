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
using System.IO;
using s4pi.Interfaces;

namespace RCOLResource
{
    public class MATD : RCOLChunk
    {
        [ElementPriority(0)]
        public static RCOL.RCOLChunkType RCOLType { get { return RCOL.RCOLChunkType.MATD; } }

        public MATD(int APIversion, EventHandler handler, Stream s, TGIBlock currentTGI) : base(APIversion, handler, s, currentTGI) { }

        #region Attributes
        uint version;
        static bool checking = s4pi.Settings.Settings.Checking;
        uint materialNameHash;
        ShaderType shaderNameHash;
        int unknown1;
        int unknown2;
        MTRL mtrl;
        #endregion

        #region Data I/O
        protected override void Parse(System.IO.Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            uint tag = r.ReadUInt32();
            if (tag != (uint)FOURCC(RCOLType.ToString())) throw new InvalidDataException(string.Format("Except to read 0x{0:8X}, read 0x{1:8X}", RCOLType, tag));
            this.version = r.ReadUInt32();
            this.materialNameHash = r.ReadUInt32();
            this.shaderNameHash = (ShaderType)r.ReadUInt32();
            uint length = r.ReadUInt32();


            this.unknown1 = r.ReadInt32();
            this.unknown2 = r.ReadInt32();
            this.mtrl = new MTRL(RecommendedApiVersion, null, s, RCOLType);

        }

        protected internal override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write((uint)RCOLType);
            w.Write(this.version);
            w.Write(this.materialNameHash);
            w.Write((uint)this.shaderNameHash);

            long lengthPosition = s.Position;
            w.Write(0);


            w.Write(this.unknown1);
            w.Write(this.unknown2);

            this.mtrl.UnParse(s);


            long position = s.Position;
            s.Position = lengthPosition;
            w.Write(position - lengthPosition - 12);
            s.Position = position;
        }
        #endregion

        #region Content Fields
        public override string RCOLTag { get { return RCOLType.ToString(); } }
        [ElementPriority(2)]
        public uint Version { get { return this.version; } set { if (!this.version.Equals(value)) { OnElementChanged(); this.version = value; } } }
        [ElementPriority(3)]
        public uint MaterialNameHash { get { return this.materialNameHash; } set { if (!this.materialNameHash.Equals(value)) { OnElementChanged(); this.materialNameHash = value; } } }
        [ElementPriority(4)]
        public ShaderType ShaderNameHash { get { return this.shaderNameHash; } set { if (!this.shaderNameHash.Equals(value)) { OnElementChanged(); this.shaderNameHash = value; } } }
        [ElementPriority(5)]
        public int Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnElementChanged(); this.unknown1 = value; } } }
        [ElementPriority(6)]
        public int Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnElementChanged(); this.unknown2 = value; } } }
        [ElementPriority(7)]
        public MTRL MTRL { get { return this.mtrl; } set { if (!this.Equals(value)) { OnElementChanged(); this.mtrl = value; } } }
        #endregion
    }
}
