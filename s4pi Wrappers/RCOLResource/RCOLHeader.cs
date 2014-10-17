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
    public class RCOLHeader : AHandlerElement
    {
        #region Attributes
        const int recommendedApiVersion = 1;
        public uint version { get; set; }
        public uint internalPublicChunkCount { get; private set; }
        public uint index3 { get; set; }
        public int externalCount { get { return this.externalTGIList.Count; } }
        public int internalCount { get { return this.internalTGIList.Count; } }
        CountedTGIBlockList externalTGIList { get; set; }
        public CountedTGIBlockList internalTGIList { get; set; }
        #endregion

        #region Constructor
        public RCOLHeader(int APIversion, EventHandler handler) : base(APIversion, handler) { }
        public RCOLHeader(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt32();
            this.internalPublicChunkCount = r.ReadUInt32();
            this.index3 = r.ReadUInt32();
            int externalCount = r.ReadInt32();
            int internalCount = r.ReadInt32();
            this.externalTGIList = new CountedTGIBlockList(null, "ITG", externalCount, s);
            this.internalTGIList = new CountedTGIBlockList(null, "ITG", internalCount, s);
        }
        #endregion

        public void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.version);
            w.Write(this.internalPublicChunkCount);
            w.Write(this.index3);
            w.Write(this.externalCount);
            w.Write(this.internalCount);
            this.externalTGIList.UnParse(s);
            this.internalTGIList.UnParse(s);
        }

        protected internal void OnChunkListChanged(object sender, RCOL.RCOLListChangeEventArg e)
        {

        }

        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        #region Content Fields
        public string Value { get { return ValueBuilder; } }
        #endregion
    }
}
