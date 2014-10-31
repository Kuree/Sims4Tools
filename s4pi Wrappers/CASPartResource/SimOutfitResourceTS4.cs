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
    public class SimOutfitResourceTS4 : AResource
    {
        private uint version;
        private float unknown3 { get; set; }
        private float unknown4 { get; set; }
        private float unknown5 { get; set; }
        private float unknown6 { get; set; }
        private float unknown7 { get; set; }
        private float unknown8 { get; set; }
        private float unknown9 { get; set; }
        private float unknown10 { get; set; }
        private uint unknown11 { get; set; }
        private uint unknown12 { get; set; }
        private ulong unknown14 { get; set; }
        private ByteIndexList unknown19 { get; set; }


        private CountedTGIBlockList tgiList;
        public SliderRederence[] sliderReferences1 { get; set; }
        public SliderRederence[] sliderReferences2 { get; set; }
        public DataBlobHandler Unknown23 { get; set; }
        public UnknownRederence[] Unknown24 { get; set; }
        public byte unknown25 { get; set; }
        public byte unknown26 { get; set; }
        public byte unknown27 { get; set; }
        public DataBlobHandler unknown28 { get; set; }
        public byte unknown29;
        public SliderRederence[] sliderReferences3 { get; set; }
        public SliderRederence[] sliderReferences4 { get; set; }
        public DataBlobHandler unknown30 { get; set; }


        public SimOutfitResourceTS4(int APIversion, Stream s) : base(APIversion, s) { if (s == null) { OnResourceChanged(this, EventArgs.Empty); } else { Parse(s); } }

        public void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);

            this.version = r.ReadUInt32();
            uint tgiOffset = r.ReadUInt32() + 8;

            // get TGI list
            long tempPosition = s.Position;
            s.Position = tgiOffset;
            TGIBlock[] _tgilist = new TGIBlock[r.ReadByte()];
            for (int i = 0; i < _tgilist.Length; i++) _tgilist[i] = new TGIBlock(1, OnResourceChanged, "IGT", s);
            this.tgiList = new CountedTGIBlockList(OnResourceChanged, _tgilist);
            s.Position = tempPosition;

            this.unknown3 = r.ReadSingle();
            this.unknown4 = r.ReadSingle();
            this.unknown5 = r.ReadSingle();
            this.unknown6 = r.ReadSingle();
            this.unknown7 = r.ReadSingle();
            this.unknown8 = r.ReadSingle();
            this.unknown9 = r.ReadSingle();
            this.unknown10 = r.ReadSingle();

            this.unknown11 = r.ReadUInt32();
            this.unknown12 = r.ReadUInt32();
            this.unknown14 = r.ReadUInt64();

            byte[] unknown18 = new byte[r.ReadByte()];
            for (int i = 0; i < unknown18.Length; i++) unknown18[i] = r.ReadByte();
            this.unknown19 = new ByteIndexList(OnResourceChanged, unknown18, this.tgiList);

            byte count2 = r.ReadByte();
            sliderReferences1 = new SliderRederence[count2];
            for (int i = 0; i < count2; i++) sliderReferences1[i] = new SliderRederence(RecommendedApiVersion, OnResourceChanged, s, this.tgiList);
            count2 = r.ReadByte();
            sliderReferences2 = new SliderRederence[count2];
            for (int i = 0; i < count2; i++) sliderReferences2[i] = new SliderRederence(RecommendedApiVersion, OnResourceChanged, s, this.tgiList);

            this.Unknown23 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(54));

            int count3 = r.ReadInt32();
            this.Unknown24 = new UnknownRederence[count3];
            for (int i = 0; i < count3; i++) this.Unknown24[i] = new UnknownRederence(RecommendedApiVersion, OnResourceChanged, s, tgiList);
            this.unknown25 = r.ReadByte();
            this.unknown26 = r.ReadByte();
            this.unknown27 = r.ReadByte();
            this.unknown28 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(9 * 9));
            this.unknown29 = r.ReadByte();

            byte count4 = r.ReadByte();
            sliderReferences3 = new SliderRederence[count4];
            for (int i = 0; i < count4; i++) sliderReferences3[i] = new SliderRederence(RecommendedApiVersion, OnResourceChanged, s, this.tgiList);
            int count5 = r.ReadByte();
            sliderReferences4 = new SliderRederence[count5];
            for (int i = 0; i < count5; i++) sliderReferences4[i] = new SliderRederence(RecommendedApiVersion, OnResourceChanged, s, this.tgiList);

            this.unknown30 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes((int)(tgiOffset - s.Position)));

        }

        protected override Stream UnParse()
        {
            throw new NotImplementedException();
        }


        #region Sub-Type
        public class SliderRederence : AHandlerElement
        {
            private CountedTGIBlockList tgiList;
            public byte index { get; set; }
            public float sliderValue { get; set; }

            public SliderRederence(int APIversion, EventHandler handler, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; }
            public SliderRederence(int APIversion, EventHandler handler, Stream s, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; Parse(s); }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.index = r.ReadByte();
                this.sliderValue = r.ReadSingle();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.index);
                w.Write(this.sliderValue);
            }

            public TGIBlock TGIReference { get { return this.tgiList[this.index]; } }

            const int recommendedApiVersion = 1;
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        }

        public class UnknownRederence : AHandlerElement
        {
            private CountedTGIBlockList tgiList;
            public byte index { get; set; }
            public int value { get; set; }

            public UnknownRederence(int APIversion, EventHandler handler, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; }
            public UnknownRederence(int APIversion, EventHandler handler, Stream s, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; Parse(s); }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.value = r.ReadInt32();
                this.index = r.ReadByte();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.value);
                w.Write(this.index);
            }

            public TGIBlock TGIReference { get { return this.tgiList[this.index]; } }

            const int recommendedApiVersion = 1;
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        }
        #endregion

        public string Value { get { return ValueBuilder; } }

        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
    }


    public class SimOutfitTS4Handler : AResourceHandler
    {
        public SimOutfitTS4Handler()
        {
            if (s4pi.Settings.Settings.IsTS4)
                this.Add(typeof(SimOutfitResourceTS4), new List<string>(new string[] { "0x025ED6F4", }));
        }
    }
}

