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
using System;
using System.Collections.Generic;
using System.Text;
using s4pi.Interfaces;
using System.IO;
using s4pi.Settings;
using s4pi.GenericRCOLResource;

namespace meshExpImp.ModelBlocks
{
    [Flags]
    public enum LODInfoFlags : uint
    {
        Portal = 0x00000001,
        Door = 0x00000002
    }
    public enum LODId : uint
    {
        HighDetail = 0x00000000,
        MediumDetail = 0x00000001,
        LowDetail = 0x00000002,
        HighDetailShadow = 0x00010000,
        MediumDetailShadow = 0x00010001,
        LowDetailShadow = 0x00010002
    }
    public class MODL : ARCOLBlock
    {
        public class BoundingBoxList : DependentList<BoundingBox>
        {
            public BoundingBoxList(EventHandler handler) : base(handler) { }
            public BoundingBoxList(EventHandler handler, Stream s) : base(handler, s) { }
            public BoundingBoxList(EventHandler handler, IEnumerable<BoundingBox> ilt) : base(handler, ilt) { }
            protected override BoundingBox CreateElement(Stream s) { return new BoundingBox(0, this.handler, s); }
            protected override void WriteElement(Stream s, BoundingBox element) { element.UnParse(s); }
        }
        public class LODEntryList : DependentList<LODEntry>
        {
            public LODEntryList(EventHandler handler) : base(handler) { }
            public LODEntryList(EventHandler handler, Stream s, int count)
                : base(handler)
            {
                Parse(s, count);
            }

            public LODEntryList(EventHandler handler, IEnumerable<LODEntry> ilt) : base(handler, ilt) { }

            private void Parse(Stream s, int count)
            {
                for (uint i = 0; i < count; i++)
                {
                    ((IList<LODEntry>)this).Add(CreateElement(s));
                }
            }
            public override void UnParse(Stream s)
            {
                foreach (var element in this)
                {
                    WriteElement(s, element);
                }
            }

            protected override LODEntry CreateElement(Stream s) { return new LODEntry(0, handler, s); }

            protected override void WriteElement(Stream s, LODEntry element) { element.UnParse(s); }
        }

        public class LODEntry : AHandlerElement, IEquatable<LODEntry>
        {
            private GenericRCOLResource.ChunkReference mModelLodIndex;
            private LODInfoFlags mFlags;
            private LODId mId;
            private float mMinZValue;
            private float mMaxZValue;

            public LODEntry(int APIversion, EventHandler handler) : this(APIversion, handler, new GenericRCOLResource.ChunkReference(APIversion, handler, 0), (LODInfoFlags)0, LODId.LowDetail, float.MinValue, float.MaxValue) { }
            public LODEntry(int APIversion, EventHandler handler, LODEntry basis) : this(APIversion, handler, basis.ModelLodIndex, basis.Flags, basis.Id, basis.MinZValue, basis.MaxZValue) { }
            public LODEntry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public LODEntry(int APIversion, EventHandler handler, GenericRCOLResource.ChunkReference modelLodIndex, LODInfoFlags flags, LODId id, float minZValue, float maxZValue)
                : base(APIversion, handler)
            {
                mModelLodIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, modelLodIndex);
                mFlags = flags;
                mId = id;
                mMinZValue = minZValue;
                mMaxZValue = maxZValue;
            }

            public string Value { get { return ValueBuilder; } }
            [ElementPriority(1)]
            public GenericRCOLResource.ChunkReference ModelLodIndex
            {
                get { return mModelLodIndex; }
                set { if (mModelLodIndex != value) { mModelLodIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnElementChanged(); } }
            }
            [ElementPriority(2)]
            public LODInfoFlags Flags
            {
                get { return mFlags; }
                set { if (mFlags != value) { mFlags = value; OnElementChanged(); } }
            }
            [ElementPriority(3)]
            public LODId Id
            {
                get { return mId; }
                set { if (mId != value) { mId = value; OnElementChanged(); } }
            }
            [ElementPriority(4)]
            public float MinZValue
            {
                get { return mMinZValue; }
                set { if (mMinZValue != value) { mMinZValue = value; OnElementChanged(); } }
            }
            [ElementPriority(5)]
            public float MaxZValue
            {
                get { return mMaxZValue; }
                set { if (mMaxZValue != value) { mMaxZValue = value; OnElementChanged(); } }
            }

            private void Parse(Stream s)
            {

                BinaryReader br = new BinaryReader(s);
                mModelLodIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
                mFlags = (LODInfoFlags)br.ReadUInt32();
                mId = (LODId)br.ReadUInt32();
                mMinZValue = br.ReadSingle();
                mMaxZValue = br.ReadSingle();
            }

            public void UnParse(Stream s)
            {
                BinaryWriter bw = new BinaryWriter(s);
                mModelLodIndex.UnParse(s);
                bw.Write((UInt32)mFlags);
                bw.Write((UInt32)mId);
                bw.Write(mMinZValue);
                bw.Write(mMaxZValue);
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(base.requestedApiVersion, GetType()); }
            }

            public override int RecommendedApiVersion
            {
                get { return kRecommendedApiVersion; }
            }

            public bool Equals(LODEntry other)
            {
                return
                    mModelLodIndex.Equals(other.mModelLodIndex)
                    && mFlags.Equals(other.mFlags)
                    && mId.Equals(other.mId)
                    && mMinZValue.Equals(other.mMinZValue)
                    && mMaxZValue.Equals(other.mMaxZValue)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as LODEntry != null ? this.Equals(obj as LODEntry) : false;
            }
            public override int GetHashCode()
            {
                return
                    mModelLodIndex.GetHashCode()
                    ^ mFlags.GetHashCode()
                    ^ mId.GetHashCode()
                    ^ mMinZValue.GetHashCode()
                    ^ mMaxZValue.GetHashCode()
                    ;
            }
        }

        private UInt32 mVersion;
        private BoundingBox mBounds;
        private UInt32 mFadeType;
        private float mCustomFadeDistance;
        private BoundingBoxList mExtraBounds;
        private LODEntryList mEntries;

        public MODL(int APIversion, EventHandler handler) : this(APIversion, handler, 256, new BoundingBox(0, handler), new BoundingBoxList(handler), 0, 0f, new LODEntryList(handler)) { }
        public MODL(int APIversion, EventHandler handler, MODL basis) : this(APIversion, handler, basis.Version, new BoundingBox(0, handler, basis.Bounds), new BoundingBoxList(handler, basis.ExtraBounds),basis.FadeType,basis.CustomFadeDistance, new LODEntryList(handler, basis.Entries)) { }
        public MODL(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public MODL(int APIversion, EventHandler handler, uint version, BoundingBox bounds, BoundingBoxList extraBounds, uint fadeType, float customFadeDistance, LODEntryList entries)
            : base(APIversion, handler, null)
        {
            mVersion = version;
            mBounds = bounds;
            mExtraBounds = extraBounds == null ? null : extraBounds;
            mFadeType = fadeType;
            mCustomFadeDistance = customFadeDistance;
            mEntries = entries == null ? null : entries;
        }
        [ElementPriority(1)]
        public uint Version
        {
            get { return mVersion; }
            set { if (mVersion != value) { mVersion = value; OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(2)]
        public BoundingBox Bounds
        {
            get { return mBounds; }
            set { if (mBounds != value) { mBounds = value; OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(3)]
        public BoundingBoxList ExtraBounds
        {
            get { return mExtraBounds; }
            set { if (mExtraBounds != value) { mExtraBounds = value == null ? null : new BoundingBoxList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(4)]
        public uint FadeType
        {
            get { return mFadeType; }
            set { if (mFadeType != value) { mFadeType = value; OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(5)]
        public float CustomFadeDistance
        {
            get { return mCustomFadeDistance; }
            set { if (mCustomFadeDistance != value) { mCustomFadeDistance = value; OnRCOLChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public LODEntryList Entries
        {
            get { return mEntries; }
            set { if (mEntries != value) { mEntries = value == null ? null : new LODEntryList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } }
        }

        protected override void Parse(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            string tag = FOURCC(br.ReadUInt32());
            if (Settings.Checking && tag != Tag)
            {
                throw new InvalidDataException(string.Format("Invalid Tag read: '{0}'; expected: '{1}'; at 0x{1:X8}", tag, Tag, s.Position));
            }
            mVersion = br.ReadUInt32();
            int count = br.ReadInt32();
            mBounds = new BoundingBox(0, handler, s);
            if (mVersion >= 258)
            {
                mExtraBounds = new BoundingBoxList(handler, s);
                mFadeType = br.ReadUInt32();
                mCustomFadeDistance = br.ReadSingle();
            }
            else
            {
                mExtraBounds = new BoundingBoxList(handler);
            }
            mEntries = new LODEntryList(handler, s, count);
        }

        public override Stream UnParse()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(s);
            bw.Write((UInt32)FOURCC(Tag));
            if (mExtraBounds == null) mExtraBounds = new BoundingBoxList(handler);
            if (mEntries == null) mEntries = new LODEntryList(handler);
            if (mBounds == null) mBounds = new BoundingBox(0, handler);
            if (mVersion < 258 && mExtraBounds.Count > 0) mVersion = 258;
            bw.Write(mVersion);
            bw.Write(mEntries.Count);
            mBounds.UnParse(s);
            if (mVersion >= 258)
            {
                mExtraBounds.UnParse(s);
                bw.Write(mFadeType);
                bw.Write(mCustomFadeDistance);
            }
            mEntries.UnParse(s);
            return s;
        }
        public override List<string> ContentFields
        {
            get
            {
                var fields = base.ContentFields;
                if (mVersion < 258)
                {
                    fields.Remove("ExtraBounds");
                    fields.Remove("FadeType");
                    fields.Remove("CustomFadeDistance");
                }
                return fields;
            }
        }

        public string Value { get { return ValueBuilder; } }
        public override uint ResourceType
        {
            get { return 0x01661233; }
        }

        public override string Tag
        {
            get { return "MODL"; }
        }

        private const int kRecommendedApiVersion = 1;
    }
}
