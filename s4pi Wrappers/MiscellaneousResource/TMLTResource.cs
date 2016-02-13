/***************************************************************************
 *  Copyright (C) 2016 by the Sims 4 Tools development team                *
 *                                                                         *
 *  Contributors:                                                          *
 *  Inge Jones                                                             *
 *  pbox                                                                   *
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
using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s4pi.Miscellaneous
{
    public class TMLTResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes

        uint version;
        uint uint01;
        ulong uintlong02;

        uint uint03;
        TGIBlock myself;

        uint uint04;
        uint uint05;
        //then THUM
        float float01;
        float float02;
        float float03;
        float colorRed;
        float colorGreen;
        float colorBlue;
        float float07;
        float float08;
        float float09;
        float float10;
        float float11;
        float float12;
        byte byte01;
        byte byte02;
        byte byte03;

        #endregion

        public TMLTResource(int APIversion, Stream s) : base(APIversion, s)
        {
            if (stream == null)
            {
                version = 4;
                stream = UnParse(); OnResourceChanged(this, EventArgs.Empty);
            }
            stream.Position = 0; Parse(stream);
        }

        #region Data I/O
        void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);

            version = r.ReadUInt32();
            uint01 = r.ReadUInt32();
            uintlong02 = r.ReadUInt64();
            uint03 = r.ReadUInt32();
            this.myself = new TGIBlock(recommendedApiVersion, null, TGIBlock.Order.ITG, s);
            uint04 = r.ReadUInt32();
            uint05 = r.ReadUInt32();

            uint magic = r.ReadUInt32();
            if (checking)
            {
                if (magic != FOURCC("TMLT"))
                {
                    throw new InvalidDataException(String.Format("Expected magic tag 0x{0:X8}; read 0x{1:X8}; position 0x{2:X8}",
                        FOURCC("TMLT"), magic, s.Position));
                }
            }
            float01 = r.ReadSingle();
            float02 = r.ReadSingle();
            float03 = r.ReadSingle();
            colorRed = r.ReadSingle();
            colorGreen = r.ReadSingle();
            colorBlue = r.ReadSingle();
            float07 = r.ReadSingle();
            float08 = r.ReadSingle();
            float09 = r.ReadSingle();
            float10 = r.ReadSingle();
            float11 = r.ReadSingle();
            float12 = r.ReadSingle();
            byte01 = r.ReadByte();
            byte02 = r.ReadByte();
            byte03 = r.ReadByte();

        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            w.Write(uint01);
            w.Write(uintlong02);
            w.Write(uint03);
            if (this.myself== null)
            {
                this.myself = new TGIBlock(recommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.ITG);
            }
            this.myself.UnParse(ms);
            w.Write(uint04);
            w.Write(uint05);
            w.Write((uint)FOURCC("TMLT"));
            w.Write(float01);
            w.Write(float02);
            w.Write(float03);
            w.Write(colorRed);
            w.Write(colorGreen);
            w.Write(colorBlue);
            w.Write(float07);
            w.Write(float08);
            w.Write(float09);
            w.Write(float10);
            w.Write(float11);
            w.Write(float12);
            w.Write(byte01);
            w.Write(byte02);
            w.Write(byte03);

            return ms;
        }


        #endregion



        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public uint Uint01 { get { return uint01; } set { if (uint01 != value) { uint01 = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(3)]
        public ulong Uintlong02 { get { return uintlong02; } set { if (uintlong02 != value) { uintlong02 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public uint Uint03 { get { return uint03; } set { if (uint03 != value) { uint03 = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(5)]
        public TGIBlock SelfReference
        {
            get { return myself;  }
            set { myself = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(6)]
        public uint Uint04 { get { return uint04; } set { if (uint04 != value) { uint04 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(7)]
        public uint Uint05 { get { return uint05; } set { if (uint05 != value) { uint05 = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(8)]
        public float Float01 { get { return float01; } set { if (float01 != value) { float01 = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(9)]
        public float Float02 { get { return float02; } set { if (float02 != value) { float02 = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(10)]
        public float Float03 { get { return float03; } set { if (float03 != value) { float03 = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(11)]
        public float ColorRed { get { return colorRed; } set { if (colorRed != value) { colorRed = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(12)]
        public float ColorGreen { get { return colorGreen; } set { if (colorGreen != value) { colorGreen = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(13)]
        public float ColorBlue { get { return colorBlue; } set { if (colorBlue != value) { colorBlue = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(14)]
        public float Float07 { get { return float07; } set { if (float07 != value) { float07 = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(15)]
        public float Float08 { get { return float08; } set { if (float08 != value) { float08 = value; OnResourceChanged(this, EventArgs.Empty); } } }

        [ElementPriority(16)]
        public float Float09 { get { return float09; } set { if (float09 != value) { float09 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17)]
        public float Float10 { get { return float10; } set { if (float10 != value) { float10 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(18)]
        public float Float11 { get { return float11; } set { if (float11 != value) { float11 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(19)]
        public float Float12 { get { return float08; } set { if (float08 != value) { float08 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(20)]
        public byte Byte01 { get { return byte01; } set { if (byte01 != value) { byte01 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(21)]
        public byte Byte02 { get { return byte02; } set { if (byte02 != value) { byte02 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22)]
        public byte Byte03 { get { return byte03; } set { if (byte03 != value) { byte03 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        public string Value { get { return ValueBuilder; } }


        #endregion
    }

    /// <summary>
    /// ResourceHandler for TRIMResource wrapper
    /// </summary>
    public class TMLTResourceHandler : AResourceHandler
    {
        public TMLTResourceHandler()
        {
            this.Add(typeof(TMLTResource), new List<string>(new string[] { "0xB0118C15", }));
        }
    }
}
