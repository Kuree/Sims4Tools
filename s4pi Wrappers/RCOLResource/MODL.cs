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
    public class MODL : RCOLChunk
    {
        [ElementPriority(0)]
        public static RCOL.RCOLChunkType RCOLType { get { return RCOL.RCOLChunkType.MODL; } }

        public MODL(int APIversion, EventHandler handler, Stream s, TGIBlock currentTGI) : base(APIversion, handler, s, currentTGI) { }

        public uint version { get; set; }
        public MLODEntry[] mlodList { get; set; }
        public float minX { get; set; }
        public float minY { get; set; }
        public float minZ { get; set; }
        public float maxX { get; set; }
        public float maxY { get; set; }
        public float maxZ { get; set; }
        public uint unknown1 { get; set; }
        public uint unknown2 { get; set; }
        public uint unknown3 { get; set; }
        public uint unknown4 { get; set; }
        public uint unknown5 { get; set; }

        #region Data I/O
        protected override void Parse(System.IO.Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            uint tag = r.ReadUInt32();
            if (tag != (uint)FOURCC(RCOLType.ToString())) throw new InvalidDataException(string.Format("Except to read 0x{0:8X}, read 0x{1:8X}", RCOLType, tag));
            this.version = r.ReadUInt32();
            uint count = r.ReadUInt32();
            this.minX = r.ReadSingle();
            this.minY = r.ReadSingle();
            this.minZ = r.ReadSingle();
            this.maxX = r.ReadSingle();
            this.maxY = r.ReadSingle();
            this.maxZ = r.ReadSingle();
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();
            this.unknown4 = r.ReadUInt32();
            this.unknown5 = r.ReadUInt32();
            this.mlodList = new MLODEntry[count];
            for (uint i = 0; i < count; i++) this.mlodList[i] = new MLODEntry(RecommendedApiVersion, handler, s);
        }

        protected internal override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write((uint)RCOLType);
            w.Write(this.version);
            w.Write(this.mlodList.Length);
            w.Write(this.minX);
            w.Write(this.minY);
            w.Write(this.minZ);
            w.Write(this.maxX);
            w.Write(this.maxY);
            w.Write(this.maxZ);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            w.Write(this.unknown4);
            w.Write(this.unknown5);
            foreach (var mlod in this.mlodList) mlod.UnParse(s);
        }

        protected internal override void UpdateChunkVisibility()
        {
            foreach(var mlod in this.mlodList)
            {
                uint index = mlod.lodModel;
                ChunkVisibilityType type = (ChunkVisibilityType)(index & 0xF0000000U);
                index = (index & 0x0FFFFFFF) - 1;
                base.ChangeVisibility((int)index, type);
            }
        }

        #endregion

        #region Sub-Types
        public class MLODEntry : AHandlerElement
        {
            const int recommendedApiVersion = 1;
            public MLODEntry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public uint lodModel { get; set; }
            public LODInfoFlags lodInfoFlags { get; set; }
            public LODId lodID { get; set; }
            public float minZValue { get; set; }
            public float maxZValue { get; set; }

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.lodModel = r.ReadUInt32();
                this.lodInfoFlags = (LODInfoFlags)r.ReadUInt32();
                this.lodID = (LODId)r.ReadUInt32();
                this.minZValue = r.ReadSingle();
                this.maxZValue = r.ReadSingle();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.lodModel);
                w.Write((uint)this.lodInfoFlags);
                w.Write((uint)this.lodID);
                w.Write(this.minZValue);
                w.Write(this.maxZValue);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region Content Fields
            public string Value { get { return ValueBuilder; } }
            #endregion
        }


        [Flags]
        public enum LODInfoFlags : uint
        {
            Portal = 0x00000001,
            Door = 0x00000002
        }

        public enum LODId : uint
        {
            HighDetail = 0x00000000,
            MediumDetail = 0x00000001,
            LowDetail = 0x00000002,
            HighDetailShadow = 0x00010000,
            MediumDetailShadow = 0x00010001,
            LowDetailShadow = 0x00010002
        }
        #endregion

    }
}
