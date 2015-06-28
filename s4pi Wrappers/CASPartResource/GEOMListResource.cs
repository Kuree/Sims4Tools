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
using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace CASPartResource
{
    public class GEOMListResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        private uint contextVersion { get; set; }
        private uint publicKeyCount { get; set; }
        private uint externalKeyCount { get; set; }
        private uint delayLoadKeyCount { get; set; }
        private uint objectCount { get; set; }
        private TGIBlock[] publicKey { get; set; }
        private TGIBlock[] externalKey { get; set; }
        private TGIBlock[] delayLoadKey { get; set; }
        private uint objectPosition { get; set; }
        private uint objectLength { get; set; }
        private uint objectVersion { get; set; }
        private ReferenceBlockList referenceBlockList { get; set; }

        #endregion


        public GEOMListResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }
        
        #region Data I/O
        private void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.contextVersion = r.ReadUInt32();
            this.publicKeyCount = r.ReadUInt32();
            this.externalKeyCount = r.ReadUInt32();
            this.delayLoadKeyCount = r.ReadUInt32();
            this.objectCount = r.ReadUInt32();
            this.publicKey = new TGIBlock[publicKeyCount];
            for (int i = 0; i < publicKeyCount; i++)
            {
                this.publicKey[i] = new TGIBlock(recommendedApiVersion, OnResourceChanged, "ITG", s);
            }
            this.externalKey = new TGIBlock[externalKeyCount];
            for (int i = 0; i < externalKeyCount; i++)
            {
                this.externalKey[i] = new TGIBlock(recommendedApiVersion, OnResourceChanged, "ITG", s);
            }
            this.delayLoadKey = new TGIBlock[delayLoadKeyCount];
            for (int i = 0; i < delayLoadKeyCount; i++)
            {
                this.delayLoadKey[i] = new TGIBlock(recommendedApiVersion, OnResourceChanged, "ITG", s);
            }
            this.objectPosition = r.ReadUInt32();
            this.objectLength = r.ReadUInt32();
            this.objectVersion = r.ReadUInt32();
            this.referenceBlockList = new ReferenceBlockList(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(this.contextVersion);
            w.Write(this.publicKeyCount);
            w.Write(this.externalKeyCount);
            w.Write(this.delayLoadKeyCount);
            w.Write(this.objectCount);

            for (int i = 0; i < publicKeyCount; i++)
            {
                this.publicKey[i].UnParse(ms);
            }
            for (int i = 0; i < externalKeyCount; i++)
            {
                this.externalKey[i].UnParse(ms);
            }
            for (int i = 0; i < delayLoadKeyCount; i++)
            {
                this.delayLoadKey[i].UnParse(ms);
            }
            this.objectPosition = (uint)(ms.Position + 8);
            this.objectLength = referenceBlockList.Size + 4;
            w.Write(this.objectPosition);
            w.Write(this.objectLength);
            w.Write(this.objectVersion);
            this.referenceBlockList.UnParse(ms);
            
            ms.Position = 0;
            return ms;
        }

        #endregion

        #region Sub-Class

        public class ReferenceBlock : AHandlerElement, IEquatable<ReferenceBlock>
        {
            public CASPartRegion region { get; set; }
            public float layer { get; set; }
            public bool isReplacement { get; set; }
            public TGIBlockList tgiList { get; set; }

            public ReferenceBlock(int APIversion, EventHandler handler) : base(APIversion, handler) { this.UnParse(new MemoryStream()); }

            public ReferenceBlock(int APIversion, EventHandler handler, Stream s) :base(APIversion, handler)
            {
                BinaryReader r = new BinaryReader(s);
                this.region = (CASPartRegion)r.ReadUInt32();
                this.layer = r.ReadSingle();
                this.isReplacement = r.ReadBoolean();

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
                w.Write((uint)this.region);
                w.Write(this.layer);
                w.Write(this.isReplacement);
                if (this.tgiList == null) this.tgiList = new TGIBlockList(handler);
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
                return this.region == other.region && this.layer == other.layer && this.isReplacement == other.isReplacement && this.tgiList.Equals(other.tgiList);
            }

            public string Value { get { return ValueBuilder; } }
        }

        public class ReferenceBlockList :DependentList<ReferenceBlock>
        {
            #region Constructor
            public ReferenceBlockList(EventHandler handler, Stream s) : base(handler, s) { }
            #endregion

            public uint Size
            {
                get
                {
                    uint tmp = 4;
                    foreach (var Block in this)
                    {
                        tmp += (uint) (13 + (Block.tgiList.Count * 16));
                    }
                    return tmp;
                }
            }

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
        public uint ContextVersion { get { return this.contextVersion; } set { if (!value.Equals(this.contextVersion)) { OnResourceChanged(this, EventArgs.Empty); this.contextVersion = value; } } }
        [ElementPriority(1)]
        public TGIBlock[] PublicKey { get { return this.publicKey; } set { if (!value.Equals(this.publicKey)) { OnResourceChanged(this, EventArgs.Empty); this.publicKey = value; } } }
        [ElementPriority(2)]
        public TGIBlock[] ExternalKey { get { return this.externalKey; } set { if (!value.Equals(this.externalKey)) { OnResourceChanged(this, EventArgs.Empty); this.externalKey = value; } } }
        [ElementPriority(3)]
        public TGIBlock[] DelayLoadKey { get { return this.delayLoadKey; } set { if (!value.Equals(this.delayLoadKey)) { OnResourceChanged(this, EventArgs.Empty); this.delayLoadKey = value; } } }
        [ElementPriority(4)]
        public uint ObjectVersion { get { return this.objectVersion; } set { if (!value.Equals(this.objectVersion)) { OnResourceChanged(this, EventArgs.Empty); this.objectVersion = value; } } }
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
