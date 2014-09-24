/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
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
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

// I don't want to start from the old version any more. Any way, it's TS4, not TS3

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using s4pi.Interfaces;

namespace CASPartResource
{
    //public class SimOutfitResourceTS4 : AResource
    //{
    //    private uint version;
    //    private float unknown3;
    //    private float unknown4;
    //    private float unknown5;
    //    private float unknown6;
    //    private float unknown7;
    //    private float unknown8;
    //    private float unknown9;
    //    private float unknown10;
    //    private uint unknown11;
    //    private uint unknown12;
    //    private ulong unknown14;
    //    private ByteIndexList unknown19;
    //    private ByteIndexList unknown20;
    //    private float unknown21;
    //    private ByteIndexList unknown22;

    //    private TGIBlockList tgiList;

    //    public void Parse(Stream s)
    //    {
    //        BinaryReader r = new BinaryReader(s);

    //        this.version = r.ReadUInt32();
    //        uint tgiOffset = r.ReadUInt32() + 8;

    //        // get TGI list
    //        long tempPosition = s.Position;
    //        s.Position = tgiOffset;
    //        TGIBlock[] _tgilist = new TGIBlock[r.ReadByte()];
    //        for (int i = 0; i < _tgilist.Length; i++) _tgilist[i] = new TGIBlock(1, OnResourceChanged, "IGT", s);
    //        this.tgiList = new TGIBlockList(OnResourceChanged, _tgilist);
    //        s.Position = tempPosition;


    //        this.unknown3 = r.ReadSingle();
    //        this.unknown4 = r.ReadSingle();
    //        this.unknown5 = r.ReadSingle();
    //        this.unknown6 = r.ReadSingle();
    //        this.unknown7 = r.ReadSingle();
    //        this.unknown8 = r.ReadSingle();
    //        this.unknown9 = r.ReadSingle();
    //        this.unknown10 = r.ReadSingle();

    //        this.unknown11 = r.ReadUInt32();
    //        this.unknown12 = r.ReadUInt32();
    //        this.unknown14 = r.ReadUInt64();

    //        byte[] unknown18 = new byte[r.ReadByte()];
    //        for (int i = 0; i < unknown18.Length; i++) unknown18[i] = r.ReadByte();
    //        this.unknown19 = new ByteIndexList(OnResourceChanged, unknown18, this.tgiList);



    //    }


    //    #region Sub-Type
    //    #endregion
    //}
}

