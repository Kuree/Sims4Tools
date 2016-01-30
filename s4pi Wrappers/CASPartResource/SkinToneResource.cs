/***************************************************************************
 *  Copyright (C) 2009, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Peter L Jones (pljones@users.sf.net)                                   *
 *  Keyi Zhang                                                             *
 *  Buzzler                                                                *
 *  Cmar                                                                   *
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

namespace CASPartResource
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using global::CASPartResource.Lists;
    using s4pi.Interfaces;

    public class SkinToneResource : AResource
    {
        private const int recommendedApiVersion = 1;

        public override int RecommendedApiVersion
        {
            get { return SkinToneResource.recommendedApiVersion; }
        }

        public override List<string> ContentFields
        {
            get { return AApiVersionedFields.GetContentFields(this.requestedApiVersion, this.GetType()); }
        }

        private uint version;
        private ulong rleInstance;
        private OverlayReferenceList overlayList;
        private ushort colorizeSaturation;
        private ushort colorizeHue;
        private uint colorizeOpacity;
        private FlagList flagList; //Same as CASP flags
        private float makeupOpacity;
        private SwatchColorList swatchList;
        private float sortOrder;
        private float makeupOpacity2;

        public SkinToneResource(int APIversion, Stream s) : base(APIversion, s)
        {
            if (this.stream == null || this.stream.Length == 0)
            {
                this.stream = this.UnParse();
                this.OnResourceChanged(this, EventArgs.Empty);
            }
            this.stream.Position = 0;
            this.Parse(this.stream);
        }

        #region Data I/O

        private void Parse(Stream s)
        {
            BinaryReader reader = new BinaryReader(s);
            s.Position = 0;
            this.version = reader.ReadUInt32();
            this.rleInstance = reader.ReadUInt64();
            this.overlayList = new OverlayReferenceList(this.OnResourceChanged, s);
            this.colorizeSaturation = reader.ReadUInt16();
            this.colorizeHue = reader.ReadUInt16();
            this.colorizeOpacity = reader.ReadUInt32();
            if (this.version > 6)
            {
                this.flagList = new FlagList(this.OnResourceChanged, s);
            }
            else
            {
                this.flagList = FlagList.CreateWithUInt16Flags(this.OnResourceChanged, s, SkinToneResource.recommendedApiVersion);
            }
            this.makeupOpacity = reader.ReadSingle();
            this.swatchList = new SwatchColorList(this.OnResourceChanged, s);
            this.sortOrder = reader.ReadSingle();
            this.makeupOpacity2 = reader.ReadSingle();
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(this.version);
            w.Write(this.rleInstance);
            if (this.overlayList == null)
            {
                this.overlayList = new OverlayReferenceList(this.OnResourceChanged);
            }
            this.overlayList.UnParse(ms);
            w.Write(this.colorizeSaturation);
            w.Write(this.colorizeHue);
            w.Write(this.colorizeOpacity);

            this.flagList = this.flagList ?? new FlagList(this.OnResourceChanged);
            if (this.version > 6)
            {
                this.flagList.UnParse(ms);
            }
            else
            {
                this.flagList.WriteUInt16Flags(ms);
            }
            w.Write(this.makeupOpacity);
            if (this.swatchList == null)
            {
                this.swatchList = new SwatchColorList(this.OnResourceChanged);
            }
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

            public OverlayReference(int apiVersion, EventHandler handler) : base(apiVersion, handler)
            {
            }

            public OverlayReference(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler)
            {
                BinaryReader r = new BinaryReader(s);
                this.ageGender = (AgeGenderFlags)r.ReadUInt32();
                this.textureReference = r.ReadUInt64();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)this.ageGender);
                w.Write(this.textureReference);
            }

            #region AHandlerElement Members

            public override int RecommendedApiVersion
            {
                get { return SkinToneResource.recommendedApiVersion; }
            }

            public override List<string> ContentFields
            {
                get { return AApiVersionedFields.GetContentFields(this.requestedApiVersion, this.GetType()); }
            }

            #endregion

            public bool Equals(OverlayReference other)
            {
                return this.textureReference == other.textureReference && this.ageGender == other.ageGender;
            }

            public string Value
            {
                get { return this.ValueBuilder; }
            }

            [ElementPriority(0)]
            public AgeGenderFlags AgeGender
            {
                get { return this.ageGender; }
                set
                {
                    if (this.ageGender != value)
                    {
                        this.OnElementChanged();
                        this.ageGender = value;
                    }
                }
            }

            [ElementPriority(1)]
            public ulong TextureReference
            {
                get { return this.textureReference; }
                set
                {
                    if (this.textureReference != value)
                    {
                        this.OnElementChanged();
                        this.textureReference = value;
                    }
                }
            }
        }

        public class OverlayReferenceList : DependentList<OverlayReference>
        {
            public OverlayReferenceList(EventHandler handler) : base(handler)
            {
            }

            public OverlayReferenceList(EventHandler handler, Stream s) : base(handler)
            {
                this.Parse(s);
            }

            #region Data I/O

            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                int count = r.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    this.Add(new OverlayReference(1, this.handler, s));
                }
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.Count);
                foreach (var reference in this)
                {
                    reference.UnParse(s);
                }
            }

            #endregion

            protected override OverlayReference CreateElement(Stream s)
            {
                return new OverlayReference(1, this.handler, s);
            }

            protected override void WriteElement(Stream s, OverlayReference element)
            {
                element.UnParse(s);
            }
        }

        #endregion

        #region Content Fields

        [ElementPriority(0)]
        public uint Version
        {
            get { return this.version; }
            set
            {
                if (!this.version.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.version = value;
                }
            }
        }

        [ElementPriority(1)]
        public ulong TextureInstance
        {
            get { return this.rleInstance; }
            set
            {
                if (!this.rleInstance.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.rleInstance = value;
                }
            }
        }

        [ElementPriority(2)]
        public OverlayReferenceList OverlayList
        {
            get { return this.overlayList; }
            set
            {
                if (!this.overlayList.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.overlayList = value;
                }
            }
        }

        [ElementPriority(3)]
        public ushort ColorizeSaturation
        {
            get { return this.colorizeSaturation; }
            set
            {
                if (!this.colorizeSaturation.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.colorizeSaturation = value;
                }
            }
        }

        [ElementPriority(4)]
        public ushort ColorizeHue
        {
            get { return this.colorizeHue; }
            set
            {
                if (!this.colorizeHue.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.colorizeHue = value;
                }
            }
        }

        [ElementPriority(5)]
        public uint ColorizeOpacity
        {
            get { return this.colorizeOpacity; }
            set
            {
                if (!this.colorizeOpacity.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.colorizeOpacity = value;
                }
            }
        }

        [ElementPriority(6)]
        public FlagList TONEFlagList
        {
            get { return this.flagList; }
            set
            {
                if (!value.Equals(this.flagList))
                {
                    this.flagList = value;
                }
                this.OnResourceChanged(this, EventArgs.Empty);
            }
        }

        [ElementPriority(7)]
        public float MakeupOpacity
        {
            get { return this.makeupOpacity; }
            set
            {
                if (!this.makeupOpacity.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.makeupOpacity = value;
                }
            }
        }

        [ElementPriority(8)]
        public SwatchColorList SwatchList
        {
            get { return this.swatchList; }
            set
            {
                if (!this.swatchList.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.swatchList = value;
                }
            }
        }

        [ElementPriority(9)]
        public float SortOrder
        {
            get { return this.sortOrder; }
            set
            {
                if (!this.sortOrder.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.sortOrder = value;
                }
            }
        }

        [ElementPriority(10)]
        public float MakeupOpacity2
        {
            get { return this.makeupOpacity2; }
            set
            {
                if (!this.makeupOpacity2.Equals(value))
                {
                    this.OnResourceChanged(this, EventArgs.Empty);
                    this.makeupOpacity2 = value;
                }
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }

        #endregion
    }

    public class SkinToneResourceHandler : AResourceHandler
    {
        public SkinToneResourceHandler()
        {
            this.Add(typeof (SkinToneResource), new List<string>(new string[] { "0x0354796A", }));
        }
    }
}