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
    public class FTPT : RCOLChunk
    {
        public static RCOL.RCOLChunkType RCOLType { get { return RCOL.RCOLChunkType.FTPT; } }

        #region Attributes
        uint version;
        static bool checking = s4pi.Settings.Settings.Checking;
        #endregion

        public FTPT(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }

        //protected override void Parse(System.IO.Stream s)
        //{
        //    BinaryReader r = new BinaryReader(s);
        //    version = r.ReadUInt32();
        //}

        //protected override void UnParse(Stream s)
        //{
        //    BinaryWriter w = new BinaryWriter(s);

        //    w.Write(version);
        //}


        #region Sub-types
        
        #endregion

        #region Content Fields
        public string Value { get { return ValueBuilder; } }
        #endregion
    }
}

