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
 *  s4pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

// I don't want to start from the old version any more. Any way, it's TS4, not TS3

// You can do whatever you want with my code, but at least you need to give me credit for decoding the resource and follow GPL rules.

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
        private float unknown1;
        private float unknown2;
        private float unknown3;
        private float unknown4;
        private float unknown5;
        private float unknown6;
        private float unknown7;
        private float unknown8;
        private uint unknown9;
        private uint unknown10;
        private ulong unknown11;
        private ByteIndexList unknown12;

        

        private CountedTGIBlockList tgiList;
        private SliderReferenceList sliderReferences1;
        private SliderReferenceList sliderReferences2;
        private DataBlobHandler unknown13;
        private UnknownReferenceList unknown14;
        private byte unknown15;
        private byte unknown16;
        private byte unknown17;
        private DataBlobHandler unknown18;
        private byte unknown19;
        private SliderReferenceList sliderReferences3;
        private SliderReferenceList sliderReferences4;
        private DataBlobHandler unknown20;





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

            this.unknown1 = r.ReadSingle();
            this.unknown2 = r.ReadSingle();
            this.unknown3 = r.ReadSingle();
            this.unknown4 = r.ReadSingle();
            this.unknown5 = r.ReadSingle();
            this.unknown6 = r.ReadSingle();
            this.unknown7 = r.ReadSingle();
            this.unknown8 = r.ReadSingle();

            this.unknown9 = r.ReadUInt32();
            this.unknown10 = r.ReadUInt32();
            this.unknown11 = r.ReadUInt64();

            // Thus 8 * 4 + 4 * 2 + 8 = 48 bytes

            byte[] unknown18 = new byte[r.ReadByte()];
            for (int i = 0; i < unknown18.Length; i++) unknown18[i] = r.ReadByte();
            this.unknown12 = new ByteIndexList(OnResourceChanged, unknown18, this.tgiList);


            sliderReferences1 = new SliderReferenceList(OnResourceChanged, s, tgiList);
            sliderReferences2 = new SliderReferenceList(OnResourceChanged, s, tgiList);
            
            this.unknown13 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(54));


            this.unknown14 = new UnknownReferenceList(OnResourceChanged, s, tgiList);
            this.unknown15 = r.ReadByte();
            this.unknown16 = r.ReadByte();
            this.unknown17 = r.ReadByte();
            this.unknown18 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes(9 * 9));
            this.unknown19 = r.ReadByte();


            sliderReferences3 = new SliderReferenceList(OnResourceChanged, s, tgiList);

            sliderReferences4 = new SliderReferenceList(OnResourceChanged, s, tgiList);


            // most of the time it's 80 bytes
            this.unknown20 = new DataBlobHandler(RecommendedApiVersion, OnResourceChanged, r.ReadBytes((int)(tgiOffset - s.Position)));

        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(this.version);
            long tgiOffsetPosition = ms.Position;
            w.Write(0);
            w.Write(unknown1);
            w.Write(unknown2);
            w.Write(unknown3);
            w.Write(unknown4);
            w.Write(unknown5);
            w.Write(unknown6);
            w.Write(unknown7);
            w.Write(unknown8);
            w.Write(unknown9);
            w.Write(unknown10);
            w.Write(unknown11);
            w.Write((byte)this.unknown12.Count);
            foreach (var value in this.unknown12) w.Write(value);
            sliderReferences1.UnParse(ms);
            sliderReferences2.UnParse(ms);
            unknown13.UnParse(ms);
            unknown14.UnParse(ms);
            w.Write(unknown15);
            w.Write(unknown16);
            w.Write(unknown17);
            unknown18.UnParse(ms);
            w.Write(unknown19);
            sliderReferences3.UnParse(ms);
            sliderReferences4.UnParse(ms);
            unknown20.UnParse(ms);

            long tmpPostion = ms.Position;
            ms.Position = tgiOffsetPosition;
            w.Write((uint)tmpPostion);
            ms.Position = tmpPostion;
            w.Write((byte)tgiList.Count);
            tgiList.UnParse(ms);

            ms.Position = 0;
            return ms;
        }


        #region Sub-Type
        public class SliderReference : AHandlerElement, IEquatable<SliderReference>
        {
            private CountedTGIBlockList tgiList;
            private byte index;
            private float sliderValue;

            public SliderReference(int APIversion, EventHandler handler, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; }
            public SliderReference(int APIversion, EventHandler handler, Stream s, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; Parse(s); }

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
            public string Value { get { return ValueBuilder; } }

            public bool Equals(SliderReference other)
            {
                return this.index == other.index && this.sliderValue == other.sliderValue;
            }
            [ElementPriority(0)]
            public byte Index { get { return this.index; } set { if (!this.index.Equals(value)) { OnElementChanged(); this.index = value; } } }
            [ElementPriority(1)]
            public float SliderValue { get { return this.sliderValue; } set { if (!this.sliderValue.Equals(value)) { OnElementChanged(); this.sliderValue = value; } } }
        }


        public class SliderReferenceList : DependentList<SliderReference>
        {
            private CountedTGIBlockList tgiList;
            public SliderReferenceList(EventHandler handler, CountedTGIBlockList tgiList) : base(handler) { this.tgiList = tgiList; }
            public SliderReferenceList(EventHandler handler, Stream s, CountedTGIBlockList tgiList) : this(handler, tgiList) { Parse(s, tgiList); }

            #region Data I/O
            protected void Parse(Stream s, CountedTGIBlockList tgiList)
            {
                int count = s.ReadByte();
                for (int i = 0; i < count; i++) this.Add(new SliderReference(recommendedApiVersion, handler, s, this.tgiList));
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((byte)this.Count);
                foreach (var entry in this) entry.UnParse(s);
            }

            protected override SliderReference CreateElement(Stream s) { throw new NotImplementedException(); }
            protected override void WriteElement(Stream s, SliderReference element) { throw new NotImplementedException(); }
            #endregion
        }

        public class UnknownRederence : AHandlerElement, IEquatable<UnknownRederence>
        {
            private CountedTGIBlockList tgiList;
            private byte unknown1;
            private byte unknown2;
            private byte unknown3;
            private byte index;
            private byte unknown4;

            public UnknownRederence(int APIversion, EventHandler handler, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; }
            public UnknownRederence(int APIversion, EventHandler handler, Stream s, CountedTGIBlockList tgiList) : base(APIversion, handler) { this.tgiList = tgiList; Parse(s); }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.unknown1 = r.ReadByte();
                this.unknown2 = r.ReadByte();
                this.unknown3 = r.ReadByte();
                this.index = r.ReadByte();
                this.unknown4 = r.ReadByte();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.unknown1);
                w.Write(this.unknown2);
                w.Write(this.unknown3);
                w.Write(index);
                w.Write(this.unknown4);
            }

            public TGIBlock TGIReference { get { return this.tgiList[this.index]; } }

            #region Content Fields
            [ElementPriority(0)]
            public byte Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnElementChanged(); this.unknown1 = value; } } }
            [ElementPriority(1)]
            public byte Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnElementChanged(); this.unknown2 = value; } } }
            [ElementPriority(2)]
            public byte Unknown3 { get { return this.unknown3; } set { if (!this.unknown3.Equals(value)) { OnElementChanged(); this.unknown3 = value; } } }
            [ElementPriority(3)]
            public byte Index { get { return this.index; } set { if (!this.index.Equals(value)) { OnElementChanged(); this.index = value; } } }
            [ElementPriority(4)]
            public byte Unknown4 { get { return this.unknown4; } set { if (!this.unknown4.Equals(value)) { OnElementChanged(); this.unknown4 = value; } } }
            #endregion
            const int recommendedApiVersion = 1;
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            public string Value { get { return ValueBuilder; } }


            public bool Equals(UnknownRederence other)
            {
                return this.index == other.index && this.unknown1 == other.unknown1 && this.unknown2 == other.unknown2 && this.unknown3 == other.unknown3 && this.unknown4 == other.unknown4;
            }
        }


        public class UnknownReferenceList : DependentList<UnknownRederence>
        {
            private CountedTGIBlockList tgiList;
            public UnknownReferenceList(EventHandler handler, CountedTGIBlockList tgiList) : base(handler) { this.tgiList = tgiList; }
            public UnknownReferenceList(EventHandler handler, Stream s, CountedTGIBlockList tgiList) : this(handler, tgiList) { Parse(s, tgiList); }

            #region Data I/O
            protected void Parse(Stream s, CountedTGIBlockList tgiList)
            {
                int count = s.ReadByte();
                for (int i = 0; i < count; i++) this.Add(new UnknownRederence(recommendedApiVersion, handler, s, this.tgiList));
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((byte)this.Count);
                foreach (var entry in this) entry.UnParse(s);
            }

            protected override UnknownRederence CreateElement(Stream s) { throw new NotImplementedException(); }
            protected override void WriteElement(Stream s, UnknownRederence element) { throw new NotImplementedException(); }
            #endregion
        }
        #endregion

        public string Value { get { return ValueBuilder; } }

        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }


        #region Content Fields
        [ElementPriority(0)]
        public uint Version { get { return this.version; } set { if (!this.version.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.version = value; } } }
        [ElementPriority(1)]
        public float Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(2)]
        public float Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(3)]
        public float Unknown3 { get { return this.unknown3; } set { if (!this.unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(4)]
        public float Unknown4 { get { return this.unknown4; } set { if (!this.unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown4 = value; } } }
        [ElementPriority(5)]
        public float Unknown5 { get { return this.unknown5; } set { if (!this.unknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown5 = value; } } }
        [ElementPriority(6)]
        public float Unknown6 { get { return this.unknown6; } set { if (!this.unknown6.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown6 = value; } } }
        [ElementPriority(7)]
        public float Unknown7 { get { return this.unknown7; } set { if (!this.unknown7.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown7 = value; } } }
        [ElementPriority(8)]
        public float Unknown8 { get { return this.unknown8; } set { if (!this.unknown8.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown8 = value; } } }
        [ElementPriority(9)]
        public uint Unknown9 { get { return this.unknown9; } set { if (!this.unknown9.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown9 = value; } } }
        [ElementPriority(10)]
        public uint Unknown10 { get { return this.unknown10; } set { if (!this.unknown10.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown10 = value; } } }
        [ElementPriority(11)]
        public ulong Unknown11 { get { return this.unknown11; } set { if (!this.unknown11.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown11 = value; } } }
        [ElementPriority(12)]
        public ByteIndexList Unknown12 { get { return this.unknown12; } set { if (!this.unknown12.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown12 = value; } } }
        [ElementPriority(14)]
        public SliderReferenceList SliderReferences1 { get { return this.sliderReferences1; } set { if (!this.sliderReferences1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.sliderReferences1 = value; } } }
        [ElementPriority(15)]
        public SliderReferenceList SliderReferences2 { get { return this.sliderReferences2; } set { if (!this.sliderReferences2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.sliderReferences2 = value; } } }
        [ElementPriority(16)]
        public DataBlobHandler Unknown13 { get { return this.unknown13; } set { if (!this.unknown13.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown13 = value; } } }
        [ElementPriority(17)]
        public UnknownReferenceList Unknown14 { get { return this.unknown14; } set { if (!this.unknown14.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown14 = value; } } }
        [ElementPriority(18)]
        public byte Unknown15 { get { return this.unknown15; } set { if (!this.unknown15.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown15 = value; } } }
        [ElementPriority(19)]
        public byte Unknown16 { get { return this.unknown16; } set { if (!this.unknown16.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown16 = value; } } }
        [ElementPriority(20)]
        public byte Unknown17 { get { return this.unknown17; } set { if (!this.unknown17.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown17 = value; } } }
        [ElementPriority(21)]
        public DataBlobHandler Unknown18 { get { return this.unknown18; } set { if (!this.unknown18.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown18 = value; } } }
        [ElementPriority(22)]
        public byte Unknown19 { get { return this.unknown19; } set { if (!this.unknown19.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown19 = value; } } }
        [ElementPriority(23)]
        public SliderReferenceList SliderReferences3 { get { return this.sliderReferences3; } set { if (!this.sliderReferences3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.sliderReferences3 = value; } } }
        [ElementPriority(24)]
        public SliderReferenceList SliderReferences4 { get { return this.sliderReferences4; } set { if (!this.sliderReferences4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.sliderReferences4 = value; } } }
        [ElementPriority(25)]
        public DataBlobHandler Unknown20 { get { return this.unknown20; } set { if (!this.unknown20.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown20 = value; } } }
        [ElementPriority(26)]
        public CountedTGIBlockList TGIList { get { return this.tgiList; } set { if (!this.tgiList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.tgiList = value; } } }
        #endregion
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

