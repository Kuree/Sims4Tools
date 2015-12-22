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

using CASPartResource.Lists;
using CASPartResource.Handlers;

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
        private ushort colorizeSaturation;
        private ushort colorizeHue;
        private uint colorizeOpacity;
        FlagList flagList;                  //Same as CASP flags
        private float makeupOpacity;
        private SwatchColorList swatchList;
        private float sortOrder;
        private float makeupOpacity2;

        public SkinToneResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null || stream.Length == 0) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            s.Position = 0;
            this.version = r.ReadUInt32();
            this.rleInstance = r.ReadUInt64();
            this.overlayList = new OverlayReferenceList(OnResourceChanged, s);
            this.colorizeSaturation = r.ReadUInt16();
            this.colorizeHue = r.ReadUInt16();
            this.colorizeOpacity = r.ReadUInt32();
            if (this.version > 6) flagList = new FlagList(OnResourceChanged, s);
            else
            {
                uint flagCount = r.ReadUInt32();
                flagList = new FlagList(OnResourceChanged);
                for (int i = 0; i < flagCount; i++)
                {
                    ushort cat = r.ReadUInt16();
                    ushort val = r.ReadUInt16();
                    flagList.Add(new Flag(recommendedApiVersion, OnResourceChanged, cat, val));
                }
            }
            this.makeupOpacity = r.ReadSingle();
            this.swatchList = new SwatchColorList(OnResourceChanged, s);
            this.sortOrder = r.ReadSingle();
            this.makeupOpacity2 = r.ReadSingle();
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(this.version);
            w.Write(this.rleInstance);
            if (this.overlayList == null) this.overlayList = new OverlayReferenceList(OnResourceChanged);
            this.overlayList.UnParse(ms);
            w.Write(this.colorizeSaturation);
            w.Write(this.colorizeHue);
            w.Write(this.colorizeOpacity);
            if (this.version > 6)
            {
                if (this.flagList == null) this.flagList = new FlagList(OnResourceChanged);
                this.flagList.UnParse(ms);
            }
            else
            {
                w.Write(this.flagList.Count);
                foreach (Flag f in this.flagList)
                {
                    w.Write((ushort)f.CompoundTag.Category);
                    w.Write((ushort)f.CompoundTag.Value);
                }
            }
            w.Write(this.makeupOpacity);
            if (this.swatchList == null) this.swatchList = new SwatchColorList(OnResourceChanged);
            this.swatchList.UnParse(ms);
            w.Write(this.sortOrder);
            w.Write(this.makeupOpacity2);
            return ms;
        }

        #endregion

        #region Sub Class
        public class OverlayReference : AHandlerElement, IEquatable<OverlayReference>
        {
            private AgeGenderFlags ageGender;
            private ulong textureReference;
            public OverlayReference(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public OverlayReference(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { BinaryReader r = new BinaryReader(s); this.ageGender = (AgeGenderFlags)r.ReadUInt32(); this.textureReference = r.ReadUInt64(); }
            public void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write((uint)this.ageGender); w.Write(this.textureReference); }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public bool Equals(OverlayReference other) { return this.textureReference == other.textureReference && this.ageGender == other.ageGender; }
            public string Value { get { return ValueBuilder; } }
            [ElementPriority(0)]
            public AgeGenderFlags AgeGender { get { return this.ageGender; } set { if (this.ageGender != value) { OnElementChanged(); this.ageGender = value; } } }
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
        public ulong TextureInstance { get { return this.rleInstance; } set { if (!this.rleInstance.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.rleInstance = value; } } }
        [ElementPriority(2)]
        public OverlayReferenceList OverlayList { get { return this.overlayList; } set { if (!this.overlayList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.overlayList = value; } } }
        [ElementPriority(3)]
        public ushort ColorizeSaturation { get { return this.colorizeSaturation; } set { if (!this.colorizeSaturation.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorizeSaturation = value; } } }
        [ElementPriority(4)]
        public ushort ColorizeHue { get { return this.colorizeHue; } set { if (!this.colorizeHue.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorizeHue = value; } } }
        [ElementPriority(5)]
        public uint ColorizeOpacity { get { return this.colorizeOpacity; } set { if (!this.colorizeOpacity.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.colorizeOpacity = value; } } }
        [ElementPriority(6)]
        public FlagList TONEFlagList { get { return flagList; } set { if (!value.Equals(flagList)) flagList = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(7)]
        public float MakeupOpacity { get { return this.makeupOpacity; } set { if (!this.makeupOpacity.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.makeupOpacity = value; } } }
        [ElementPriority(8)]
        public SwatchColorList SwatchList { get { return this.swatchList; } set { if (!this.swatchList.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.swatchList = value; } } }
        [ElementPriority(9)]
        public float SortOrder { get { return this.sortOrder; } set { if (!this.sortOrder.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.sortOrder = value; } } }
        [ElementPriority(10)]
        public float MakeupOpacity2 { get { return this.makeupOpacity2; } set { if (!this.makeupOpacity2.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.makeupOpacity2 = value; } } }
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
