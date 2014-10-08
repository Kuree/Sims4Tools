
/***************************************************************************
 *  Copyright (C) 2014 by Inge Jones                                       *
 *                                                   *
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

namespace s4pi.Miscellaneous
{
    /// <summary>
    /// A resource wrapper that understands 0xC5F6763E resources
    /// </summary>
    public class _0xC5F6763E_Resource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Contentfields
        [ElementPriority(1)]
        public uint Unk01
        {
            get { return unk01; }
            set { this.unk01 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(2)]
        public uint Unk02
        {
            get { return unk02; }
            set { this.unk02 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(3)]
        public uint Unk03
        {
            get { return unk03; }
            set { this.unk03 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(5)]
        public uint Unk04
        {
            get { return unk04; }
            set { this.unk04 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(6)] //self
        public TGIBlock Self
        {
            get { return self; }
            set { this.self = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(8)]
        public TGIBlockList UnkITGList
        {
            get { return this.unkITGList; }
            set { this.unkITGList = value; OnResourceChanged(this, EventArgs.Empty); } 
        }
        [ElementPriority(10)]
        public uint Unk05
        {
            get { return unk05; }
            set { this.unk05 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(11)]
        public uint Unk06
        {
            get { return unk06; }
            set { this.unk06 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(12)]
        public uint Unk07
        {
            get { return unk07; }
            set { this.unk07 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(13)]
        public uint Unk08
        {
            get { return unk08; }
            set { this.unk08 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(14)]
        public uint Unk09
        {
            get { return unk09; }
            set { this.unk09 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(15)]
        public uint Unk10
        {
            get { return unk10; }
            set { this.unk10 = value; OnResourceChanged(this, EventArgs.Empty); }
        }

        [ElementPriority(17)]
        public TGIBlock UnkITG_01
        {
            get { return unkITG_01; }
            set { this.unkITG_01 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(18)]
        public TGIBlock UnkITG_02
        {
            get { return unkITG_02; }
            set { this.unkITG_02 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(19)]
        public TGIBlock UnkITG_03
        {
            get { return unkITG_03; }
            set { this.unkITG_03 = value; OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(21)]
        public uint Unk11
        {
            get { return unk11; }
            set { this.unk11 = value; OnResourceChanged(this, EventArgs.Empty); }
        }


        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        public string Value { get { return ValueBuilder; } }

        #endregion contentfields

        #region Attributes
        List<String> sstring;
        String s;
        uint unk01 = 0x00000003;
        uint unk02;
        uint unk03;
        uint unk04;
        TGIBlock self;
        TGIBlockList unkITGList;
        uint unk05;
        uint unk06;
        uint unk07;
        uint unk08;
        uint unk09;
        uint unk10;
        uint unk11;
        TGIBlock unkITG_01;
        TGIBlock unkITG_02;
        TGIBlock unkITG_03;
        #endregion

        public _0xC5F6763E_Resource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
 
        void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            this.unk01 = br.ReadUInt32();
            this.unk02 = br.ReadUInt32();
            this.unk03 = br.ReadUInt32();
            uint count1 = br.ReadUInt32();
            this.unk04 = br.ReadUInt32();
            this.self = new TGIBlock(recommendedApiVersion, null, TGIBlock.Order.ITG, s);
            unkITGList = new TGIBlockList(null);
            for (int i = 0; i < count1; i++)
            {
               unkITGList.Add(new TGIBlock(recommendedApiVersion,null,TGIBlock.Order.ITG,s));
            }
            this.unk05 = br.ReadUInt32();
            this.unk06 = br.ReadUInt32();
            this.unk07 = br.ReadUInt32();
            this.unk08 = br.ReadUInt32();
            this.unk09 = br.ReadUInt32();
            this.unk10 = br.ReadUInt32();
            this.unkITG_01 = new TGIBlock(recommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.unkITG_02 = new TGIBlock(recommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.unkITG_03 = new TGIBlock(recommendedApiVersion, null, TGIBlock.Order.ITG, s);
            this.unk11 = br.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            if (unkITGList == null)
            { 
                unkITGList = new TGIBlockList(null);
            }

            bw.Write(this.unk01);
            bw.Write(this.unk02);
            bw.Write(this.unk03);
            bw.Write(this.unkITGList.Count);
            bw.Write(this.unk04);
            UnParseITG(bw, this.self);
            bw.Write(this.self);
            for (int i = 0; i < this.unkITGList.Count; i++)
            {
                UnParseITG(bw, this.unkITGList[i]);
            }
            bw.Write(this.unk05);
            bw.Write(this.unk06);
            bw.Write(this.unk07);
            bw.Write(this.unk08);
            bw.Write(this.unk09);
            bw.Write(this.unk10);
            UnParseITG(bw, this.unkITG_01);
            UnParseITG(bw, this.unkITG_01);
            UnParseITG(bw, this.unkITG_01);

            bw.Write(this.unkITG_01);
            bw.Write(this.unkITG_02);
            bw.Write(this.unkITG_03);
            bw.Write(this.unk11);
            return ms;
        }

        void UnParseITG(BinaryWriter bw, TGIBlock t)
        {
            if (t == null)
            {
                t = new TGIBlock(recommendedApiVersion, null, TGIBlock.Order.ITG);
            }
            bw.Write(t);
        }

        
        #endregion


    }

    /// <summary>
    /// ResourceHandler for _0xC5F6763EResource wrapper
    /// </summary>
    public class _0xC5F6763E_ResourceHandler : AResourceHandler
    {
        public _0xC5F6763E_ResourceHandler()
        {
            this.Add(typeof(_0xC5F6763E_Resource), new List<string>(new string[] { "0xC5F6763E", }));
        }
    }
}
