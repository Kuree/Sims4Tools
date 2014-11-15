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

// This code is based on Snaitf's analyze

using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace CASPartResource
{
    public class SkinToneResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        static bool checking = s4pi.Settings.Settings.Checking;

        private uint version;
        private ulong rleInstance;
        private OverlayReferenceList overlayList;
        private uint unknown1;
        private uint unknown2;
        private SimpleList<uint> unknownList;
        private uint unknown3;
        private CASPartResource.SwatchColorList swatchList;
        private uint unknown4;
        private uint unknown5;

        public SkinToneResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null || stream.Length == 0) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            s.Position = 0;
            this.version = r.ReadUInt32();
            this.rleInstance = r.ReadUInt64();
            this.overlayList = new OverlayReferenceList(OnResourceChanged, s);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            int count = r.ReadInt32();
            this.unknownList = new SimpleList<uint>(OnResourceChanged);
            for (int i = 0; i < count; i++) this.unknownList.Add(r.ReadUInt32());
            this.unknown3 = r.ReadUInt32();
            this.swatchList = new CASPartResource.SwatchColorList(OnResourceChanged, s);
            this.unknown4 = r.ReadUInt32();
            this.unknown5 = r.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(this.version);
            w.Write(this.rleInstance);
            if (this.overlayList == null) this.overlayList = new OverlayReferenceList(OnResourceChanged);
            this.overlayList.UnParse(ms);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            if (this.unknownList == null) this.unknownList = new SimpleList<uint>(OnResourceChanged);
            w.Write(this.unknownList.Count);
            foreach (var i in this.unknownList) w.Write(i);
            w.Write(this.unknown3);
            if (this.swatchList == null) this.swatchList = new CASPartResource.SwatchColorList(OnResourceChanged);
            this.swatchList.UnParse(ms);
            w.Write(this.unknown4);
            w.Write(this.unknown5);
            return ms;
        }

        #endregion

        #region Sub Class
        public class OverlayReference : AHandlerElement, IEquatable<OverlayReference>
        {
            private uint flags;
            private ulong textureReference;
            public OverlayReference(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public OverlayReference(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { BinaryReader r = new BinaryReader(s); this.flags = r.ReadUInt32(); this.textureReference = r.ReadUInt64(); }
            public void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(this.flags); w.Write(this.textureReference); }
            
            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public bool Equals(OverlayReference other) { return this.textureReference == other.textureReference && this.flags == other.flags; }
            public string Value { get { return ValueBuilder; } }
            [ElementPriority(0)]
            public uint Flags { get { return this.flags; } set { if (this.flags != value) { OnElementChanged(); this.flags = value; } } }
            [ElementPriority(1)]
            public ulong TextureReference { get { return this.textureReference; } set { if (this.textureReference != value) { OnElementChanged(); this.textureReference = value; } } }
        }

        public class OverlayReferenceList : DependentList<OverlayReference>
        {
            public OverlayReferenceList(EventHandler handler) : base(handler) { }
            public OverlayReferenceList(EventHandler handler, Stream s) : base(handler) { Parse(s); }

            #region Data I/O
            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                int count = r.ReadInt32();
                for (int i = 0; i < count; i++)
                    base.Add(new OverlayReference(1, handler, s));
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(base.Count);
                foreach (var reference in this)
                    reference.UnParse(s);
            }
            #endregion

            protected override OverlayReference CreateElement(Stream s) { return new OverlayReference(1, handler, s); }
            protected override void WriteElement(Stream s, OverlayReference element) { element.UnParse(s); }
        }
        #endregion

        #region Content Fields
        [ElementPriority(0)]
        public uint Version { get { return this.version; } set { if (!this.version.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.version = value; } } }
        [ElementPriority(1)]
        public ulong RleInstance { get { return this.rleInstance; } set { if (!this.rleInstance.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.rleInstance = value; } } }
        [ElementPriority(2)]
        public OverlayReferenceList OverlayList { get { return this.overlayList; } set { if (!this.overlayList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.overlayList = value; } } }
        [ElementPriority(3)]
        public uint Unknown1 { get { return this.unknown1; } set { if (!this.unknown1.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown1 = value; } } }
        [ElementPriority(4)]
        public uint Unknown2 { get { return this.unknown2; } set { if (!this.unknown2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown2 = value; } } }
        [ElementPriority(5)]
        public SimpleList<uint> UnknownList { get { return this.unknownList; } set { if (!this.unknownList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknownList = value; } } }
        [ElementPriority(6)]
        public uint Unknown3 { get { return this.unknown3; } set { if (!this.unknown3.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown3 = value; } } }
        [ElementPriority(7)]
        public CASPartResource.SwatchColorList SwatchList { get { return this.swatchList; } set { if (!this.swatchList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.swatchList = value; } } }
        [ElementPriority(8)]
        public uint Unknown4 { get { return this.unknown4; } set { if (!this.unknown4.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown4 = value; } } }
        [ElementPriority(9)]
        public uint Unknown5 { get { return this.unknown5; } set { if (!this.unknown5.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.unknown5 = value; } } }
        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class SkinToneResourceHandler : AResourceHandler
    {
        public SkinToneResourceHandler()
        {
            this.Add(typeof(SkinToneResource), new List<string>(new string[] { "0x0354796A", }));
        }
    }
}
