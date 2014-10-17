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

namespace RCOLResource
{
    public class MTRL : AHandlerElement
    {
        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        const int recommendedApiVersion = 1;
        uint mtnfUnknown1;
        ShaderDataList sdList;
        RCOL.RCOLChunkType type;
        private uint chunkTag;
        #endregion

        public MTRL(int APIversion, EventHandler handler, Stream s, RCOL.RCOLChunkType type) : base(APIversion, handler) { this.type = type; Parse(s); }

        private void Parse(Stream s)
        {
            long start = s.Position;
            BinaryReader r = new BinaryReader(s);
            chunkTag = r.ReadUInt32();
            if (checking) if (chunkTag != (uint)FOURCC("MTNF") && chunkTag != (uint)FOURCC("MTRL"))
                    throw new InvalidDataException(String.Format("Invalid mtnfTag read: '{0}'; expected: 'MTNF' or 'MTRL'; at 0x{1:X8}", FOURCC(chunkTag), s.Position));
            mtnfUnknown1 = r.ReadUInt32();
            //r.ReadInt32();
            this.sdList = new ShaderDataList(handler, s, this.type, start );
        }

        public void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            int start = (int)s.Position;
            w.Write(chunkTag);
            w.Write(mtnfUnknown1);
            this.sdList.UnParse(s, start);

        }

        public uint MtnfUnknown1 { get { return this.mtnfUnknown1; } set { if (!this.mtnfUnknown1.Equals(value)) { OnElementChanged(); this.mtnfUnknown1 = value; } } }
        public ShaderDataList SdList { get { return this.sdList; } set { if (!this.sdList.Equals(value)) { OnElementChanged(); this.sdList = value; } } }
        public string Value { get { return ValueBuilder; } }


        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion
    }
}
