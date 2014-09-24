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
using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CASPartResource
{
    public class GEOMListResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        private DataBlobHandler unknownHeader { get; set; }
        private TGIBlock currentInstance { get; set; }
        private uint unknown1 { get; set; }
        private uint unknown2 { get; set; }
        private uint unknown3 { get; set; }
        private ReferenceBlockList referenceBlockList { get; set; }

        #endregion


        public GEOMListResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }
        
        #region Data I/O
        private void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.unknownHeader = new DataBlobHandler(recommendedApiVersion, OnResourceChanged, 20, s);
            this.currentInstance = new TGIBlock(recommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();

            this.referenceBlockList = new ReferenceBlockList(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            this.unknownHeader.UnParse(ms);
            this.currentInstance.UnParse(ms);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            this.referenceBlockList.UnParse(ms);

            ms.Position = 0;
            return ms;
        }

        #endregion

        #region Sub-Class

        public class ReferenceBlock : AHandlerElement, IEquatable<ReferenceBlock>
        {
            public uint unknown1 { get; set; }
            public DataBlobHandler unknownBytes { get; set; }
            public TGIBlockList tgiList { get; set; }

            public ReferenceBlock(int APIversion, EventHandler handler, Stream s) :base(APIversion, handler)
            {
                BinaryReader r = new BinaryReader(s);
                this.unknown1 = r.ReadUInt32();
                this.unknownBytes = new DataBlobHandler(recommendedApiVersion, null, r.ReadBytes(5));

                int count = r.ReadInt32();

                this.tgiList = new TGIBlockList(null);
                for(int i = 0; i < count; i++)
                {
                    this.tgiList.Add(new TGIBlock(recommendedApiVersion, null, "ITG", s));
                }
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.unknown1);
                unknownBytes.UnParse(s);
                w.Write(this.tgiList.Count);
                foreach (var tgi in this.tgiList)
                    tgi.UnParse(s);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public bool Equals(ReferenceBlock other)
            {
                return this.unknown1 == other.unknown1 && this.unknownBytes.Equals(other.unknownBytes) && this.tgiList.Equals(other.tgiList);
            }

            public string Value { get { return ValueBuilder; } }
        }

        public class ReferenceBlockList :DependentList<ReferenceBlock>
        {
            #region Constructor
            public ReferenceBlockList(EventHandler handler, Stream s) : base(handler, s) { }
            #endregion

            #region Data I/O
            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                int count = r.ReadInt32();
                for(int i  = 0; i < count; i++)
                {
                    base.Add(new ReferenceBlock(recommendedApiVersion, handler, s));
                }
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(base.Count);
                foreach(var block in this)
                {
                    block.UnParse(s);
                }
            }
            #endregion

            protected override ReferenceBlock CreateElement(Stream s) { return new ReferenceBlock(recommendedApiVersion, handler, s); }
            protected override void WriteElement(Stream s, ReferenceBlock element) { element.UnParse(s); }

        }

        #endregion

        #region Content-Field
        [ElementPriority(0)]
        public DataBlobHandler UnknownHeader { get { return this.unknownHeader; } set { if (!value.Equals(this.unknownHeader)) { OnResourceChanged(this, EventArgs.Empty); this.unknownHeader = value; } } }
        [ElementPriority(1)]
        public TGIBlock CurrentInstance { get { return this.currentInstance; } set { if (!value.Equals(this.currentInstance)) { OnResourceChanged(this, EventArgs.Empty); this.currentInstance = value; } } }
        [ElementPriority(2)]
        public uint Unknown1 { get { return this.unknown1; } set { if (!value.Equals(this.unknown1)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(3)]
        public uint Unknown2 { get { return this.unknown2; } set { if (!value.Equals(this.unknown2)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(4)]
        public uint Unknown3 { get { return this.unknown3; } set { if (!value.Equals(this.unknown3)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(5)]
        public ReferenceBlockList GEOMReferenceBlockList { get { return this.referenceBlockList; } set { if (!value.Equals(this.referenceBlockList)) { OnResourceChanged(this, EventArgs.Empty); this.referenceBlockList = value; } } }
        #endregion

        public string Value { get { return ValueBuilder; } }
    }

    /// <summary>
    /// ResourceHandler for GEOMListResource wrapper
    /// </summary>
    public class GEOMListResourceHandler : AResourceHandler
    {
        public GEOMListResourceHandler()
        {
            this.Add(typeof(GEOMListResource), new List<string>(new string[] { "0xAC16FBEC", }));
        }
    }
}
