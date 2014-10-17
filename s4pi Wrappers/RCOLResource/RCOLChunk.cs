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
    public class RCOLChunk : AHandlerElement
    {
        
        public RCOLChunk(int APIversion, EventHandler handler) : base(APIversion, handler) { }
        public RCOLChunk(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); this.CurrentTGI = new TGIBlock(recommendedApiVersion, handler); }
        public RCOLChunk(int APIversion, EventHandler handler, Stream s, TGIBlock currentTGI) : base(APIversion, handler) { Parse(s); this.CurrentTGI = currentTGI; }

        #region Attributes
        const int recommendedApiVersion = 1;
        private byte[] rawData;
        #endregion

        public virtual string RCOLTag { get; private set; }
        [ElementPriority(1)]
        public TGIBlock CurrentTGI { get; set; }

        #region Data I/O
        protected virtual void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            this.RCOLTag = FOURCC((ulong)r.ReadInt32());
            s.Position = 0;
            this.rawData = r.ReadBytes((int)s.Length);
        }

        protected internal virtual void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            if (this.rawData == null) this.rawData = new byte[0];
            w.Write(this.rawData);
        }

        protected internal virtual void OnRCOLListChanged(object sender, RCOL.RCOLListChangeEventArg e) { }
        #endregion

        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { var res = GetContentFields(requestedApiVersion, this.GetType()); res.Remove("RCOLTag"); return res; } }
        #endregion

        #region Content Fields
        public string Value { get { return ValueBuilder; } }
        //public static RCOL.RCOLChunkType RCOLType =  RCOL.RCOLChunkType.None;
        #endregion
    }
}
