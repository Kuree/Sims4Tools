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
using System.IO;
using s4pi.Interfaces;

namespace CASPartResource
{
    public class BlendGeometryResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint magic = (uint)FOURCC("BGEO");
        uint version;
        uint x04;
        uint x08;
        uint x0C;
        Section1EntryList section1Entries;
        Section2EntryList section2Entries;
        Section3EntryList section3Entries;
        #endregion

        public BlendGeometryResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        private void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);

            magic = r.ReadUInt32();
            if (magic != (uint)FOURCC("BGEO"))
                throw new InvalidDataException("Invalid magic number.  Read 0x" + magic.ToString("X8") + "; expected 0x" + FOURCC("BGEO").ToString("X8"));

            version = r.ReadUInt32();
            if (version != 0x00000300)
                throw new InvalidDataException("Invalid version.  Read 0x" + version.ToString("X4") + "; expected 0x00000300");

            int s1count = r.ReadInt32();

            x04 = r.ReadUInt32();
            if (x04 != 0x04)
                throw new InvalidDataException("Invalid x04.  Read 0x" + version.ToString("X0") + "; expected 0x00000004");

            int s2count = r.ReadInt32();
            int s3count = r.ReadInt32();

            x08 = r.ReadUInt32();
            if (x08 != 0x08)
                throw new InvalidDataException("Invalid x08.  Read 0x" + version.ToString("X0") + "; expected 0x00000008");

            x0C = r.ReadUInt32();
            if (x0C != 0x0C)
                throw new InvalidDataException("Invalid x0C.  Read 0x" + version.ToString("X0") + "; expected 0x0000000C");

            uint s1posn = r.ReadUInt32();
            uint s2posn = r.ReadUInt32();
            uint s3posn = r.ReadUInt32();

            if (s1posn != s.Position)
                throw new InvalidDataException("Invalid section 1 position.  Read 0x" + s1posn.ToString("X8") + "; expected 0x" + s.Position.ToString("X8"));

            section1Entries = new Section1EntryList(OnResourceChanged, s1count, s);

            if (s2posn != s.Position)
                throw new InvalidDataException("Invalid section 2 position.  Read 0x" + s2posn.ToString("X8") + "; expected 0x" + s.Position.ToString("X8"));

            section2Entries = new Section2EntryList(OnResourceChanged, s2count, s);

            if (s3posn != s.Position)
                throw new InvalidDataException("Invalid section 3 position.  Read 0x" + s3posn.ToString("X8") + "; expected 0x" + s.Position.ToString("X8"));

            section3Entries = new Section3EntryList(OnResourceChanged, s3count, s);
        }

        protected override Stream UnParse()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(magic);
            w.Write(version);
            w.Write(section1Entries.Count);
            w.Write(x04);
            w.Write(section2Entries.Count);
            w.Write(section3Entries.Count);
            w.Write(x08);
            w.Write(x0C);

            long posn = s.Position;
            w.Write((uint)0);
            w.Write((uint)0);
            w.Write((uint)0);

            long s1posn = s.Position;
            section1Entries.UnParse(s);

            long s2posn = s.Position;
            section2Entries.UnParse(s);

            long s3posn = s.Position;
            section3Entries.UnParse(s);

            long endPosn = s.Position;

            s.Position = posn;
            w.Write((uint)s1posn);
            w.Write((uint)s2posn);
            w.Write((uint)s3posn);
            s.Position = endPosn;

            return s;
        }
        #endregion

        #region Sub-Types
        public class LODSection : AHandlerElement, IEquatable<LODSection>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            int lastIndex;
            int section2Index;//4-7
            int section3Index;//8-B
            #endregion

            #region Constructors
            public LODSection(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public LODSection(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public LODSection(int APIversion, EventHandler handler, LODSection basis) : this(APIversion, handler, basis.lastIndex, basis.section2Index, basis.section3Index) { }
            public LODSection(int APIversion, EventHandler handler, int lastIndex, int section2Index, int section3Index) : base(APIversion, handler) { this.lastIndex = lastIndex; this.section2Index = section2Index; this.section3Index = section3Index; }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                lastIndex = r.ReadInt32();
                section2Index = r.ReadInt32();
                section3Index = r.ReadInt32();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(lastIndex);
                w.Write(section2Index);
                w.Write(section3Index);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Section1Entry> Members

            public bool Equals(LODSection other)
            {
                return
                    lastIndex.Equals(other.lastIndex)
                    && section2Index.Equals(other.section2Index)
                    && section3Index.Equals(other.section3Index)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as Section2Entry != null ? this.Equals(obj as Section2Entry) : false;
            }
            public override int GetHashCode()
            {
                return
                    lastIndex.GetHashCode()
                    ^ section2Index.GetHashCode()
                    ^ section3Index.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public int LastIndex { get { return lastIndex; } set { if (!lastIndex.Equals(value)) { lastIndex = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public int Section2Index { get { return section2Index; } set { if (!section2Index.Equals(value)) { section2Index = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public int Section3Index { get { return section3Index; } set { if (!section3Index.Equals(value)) { section3Index = value; OnElementChanged(); } } }

            public string Value { get { return "{ " + ValueBuilder.Replace("\n", "; ") + " }"; } }
            #endregion
        }
        public class Section1Entry : AHandlerElement, IEquatable<Section1Entry>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            AgeGenderFlags ageGender;
            FacialRegionFlags facialRegion;
            LODSection lod1;
            LODSection lod2;
            LODSection lod3;
            LODSection lod4;
            #endregion
            
            #region Constructors
            public Section1Entry(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
                ageGender = new AgeGenderFlags(requestedApiVersion, handler);
                lod1 = new LODSection(requestedApiVersion, handler);
                lod2 = new LODSection(requestedApiVersion, handler);
                lod3 = new LODSection(requestedApiVersion, handler);
                lod4 = new LODSection(requestedApiVersion, handler);
            }
            public Section1Entry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Section1Entry(int APIversion, EventHandler handler, Section1Entry basis) : this(APIversion, handler, basis.lod1, basis.lod2, basis.lod3, basis.lod4) { }
            public Section1Entry(int APIversion, EventHandler handler, LODSection lod1, LODSection lod2, LODSection lod3, LODSection lod4)
                : base(APIversion, handler)
            {
                lod1 = new LODSection(0, handler, lod1);
                lod2 = new LODSection(0, handler, lod2);
                lod3 = new LODSection(0, handler, lod3);
                lod4 = new LODSection(0, handler, lod4);
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                ageGender = new AgeGenderFlags(0, handler, s);
                facialRegion = (FacialRegionFlags)new BinaryReader(s).ReadUInt32();
                lod1 = new LODSection(0, handler, s);
                lod2 = new LODSection(0, handler, s);
                lod3 = new LODSection(0, handler, s);
                lod4 = new LODSection(0, handler, s);
            }

            internal void UnParse(Stream s)
            {
                if (ageGender == null) ageGender = new AgeGenderFlags(0, handler);
                ageGender.UnParse(s);
                new BinaryWriter(s).Write((uint)facialRegion);
                if (lod1 == null) lod1 = new LODSection(0, handler);
                lod1.UnParse(s);
                if (lod2 == null) lod2 = new LODSection(0, handler);
                lod2.UnParse(s);
                if (lod3 == null) lod3 = new LODSection(0, handler);
                lod3.UnParse(s);
                if (lod4 == null) lod4 = new LODSection(0, handler);
                lod4.UnParse(s);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Section1Entry> Members

            public bool Equals(Section1Entry other)
            {
                return
                    ageGender.Equals(other.ageGender)
                    && facialRegion.Equals(other.facialRegion)
                    && lod1.Equals(other.lod1)
                    && lod2.Equals(other.lod2)
                    && lod3.Equals(other.lod3)
                    && lod4.Equals(other.lod4)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as Section1Entry != null ? this.Equals(obj as Section1Entry) : false;
            }
            public override int GetHashCode()
            {
                return
                    ageGender.GetHashCode()
                    & facialRegion.GetHashCode()
                    & lod1.GetHashCode()
                    & lod2.GetHashCode()
                    & lod3.GetHashCode()
                    & lod4.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public AgeGenderFlags AgeGender { get { return ageGender; } set { if (!ageGender.Equals(value)) { ageGender = new AgeGenderFlags(0, handler, value); OnElementChanged(); } } }
            [ElementPriority(2)]
            public FacialRegionFlags FacialRegion { get { return facialRegion; } set { if (!facialRegion.Equals(value)) { facialRegion = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public LODSection LOD1 { get { return lod1; } set { if (!lod1.Equals(value)) { lod1 = new LODSection(0, handler, value); OnElementChanged(); } } }
            [ElementPriority(4)]
            public LODSection LOD2 { get { return lod2; } set { if (!lod2.Equals(value)) { lod2 = new LODSection(0, handler, value); OnElementChanged(); } } }
            [ElementPriority(5)]
            public LODSection LOD3 { get { return lod3; } set { if (!lod3.Equals(value)) { lod3 = new LODSection(0, handler, value); OnElementChanged(); } } }
            [ElementPriority(6)]
            public LODSection LOD4 { get { return lod4; } set { if (!lod4.Equals(value)) { lod4 = new LODSection(0, handler, value); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class Section1EntryList : DependentList<Section1Entry>
        {
            int initialCount;

            #region Constructors
            public Section1EntryList(EventHandler handler) : base(handler) { }
            public Section1EntryList(EventHandler handler, int initialCount, Stream s) : base(null) { elementHandler = handler; this.initialCount = initialCount; Parse(s); this.handler = handler; }
            public Section1EntryList(EventHandler handler, IEnumerable<Section1Entry> le) : base(handler, le) { }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return initialCount; }
            protected override Section1Entry CreateElement(Stream s) { return new Section1Entry(0, elementHandler, s); }
            protected override void WriteCount(Stream s, int count) { return; }
            protected override void WriteElement(Stream s, Section1Entry element) { element.UnParse(s); }
            #endregion
        }

        public class Section2Entry : AHandlerElement, IEquatable<Section2Entry>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            ushort data;
            #endregion

            #region Constructors
            public Section2Entry(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Section2Entry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Section2Entry(int APIversion, EventHandler handler, Section2Entry basis) : this(APIversion, handler, basis.data) { }
            public Section2Entry(int APIversion, EventHandler handler, ushort data) : base(APIversion, handler) { this.data = data; }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                data = r.ReadUInt16();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(data);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Section1Entry> Members

            public bool Equals(Section2Entry other)
            {
                return
                    data.Equals(other.data)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as Section2Entry != null ? this.Equals(obj as Section2Entry) : false;
            }
            public override int GetHashCode()
            {
                return
                    data.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public ushort Data { get { return data; } set { if (!data.Equals(value)) { data = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class Section2EntryList : DependentList<Section2Entry>
        {
            int initialCount;

            #region Constructors
            public Section2EntryList(EventHandler handler) : base(handler) { }
            public Section2EntryList(EventHandler handler, int initialCount, Stream s) : base(null) { elementHandler = handler; this.initialCount = initialCount; Parse(s); this.handler = handler; }
            public Section2EntryList(EventHandler handler, IEnumerable<Section2Entry> le) : base(handler, le) { }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return initialCount; }
            protected override Section2Entry CreateElement(Stream s) { return new Section2Entry(0, elementHandler, s); }
            protected override void WriteCount(Stream s, int count) { return; }
            protected override void WriteElement(Stream s, Section2Entry element) { element.UnParse(s); }
            #endregion
        }

        public class Section3Entry : AHandlerElement, IEquatable<Section3Entry>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            ushort data1;
            ushort data2;
            ushort data3;
            #endregion

            #region Constructors
            public Section3Entry(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Section3Entry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Section3Entry(int APIversion, EventHandler handler, Section3Entry basis) : this(APIversion, handler, basis.data1, basis.data2, basis.data3) { }
            public Section3Entry(int APIversion, EventHandler handler, ushort data1, ushort data2, ushort data3) : base(APIversion, handler) { this.data1 = data1; this.data2 = data2; this.data3 = data3; }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                data1 = r.ReadUInt16();
                data2 = r.ReadUInt16();
                data3 = r.ReadUInt16();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(data1);
                w.Write(data2);
                w.Write(data3);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Section1Entry> Members

            public bool Equals(Section3Entry other)
            {
                return
                    data1.Equals(other.data1)
                    && data2.Equals(other.data2)
                    && data3.Equals(other.data3)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as Section2Entry != null ? this.Equals(obj as Section3Entry) : false;
            }
            public override int GetHashCode()
            {
                return
                    data1.GetHashCode()
                    ^ data2.GetHashCode()
                    ^ data3.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public ushort Data1 { get { return data1; } set { if (!data1.Equals(value)) { data1 = value; OnElementChanged(); } } }
            public ushort Data2 { get { return data2; } set { if (!data2.Equals(value)) { data2 = value; OnElementChanged(); } } }
            public ushort Data3 { get { return data3; } set { if (!data3.Equals(value)) { data3 = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class Section3EntryList : DependentList<Section3Entry>
        {
            int initialCount;

            #region Constructors
            public Section3EntryList(EventHandler handler) : base(handler) { }
            public Section3EntryList(EventHandler handler, int initialCount, Stream s) : base(null) { elementHandler = handler; this.initialCount = initialCount; Parse(s); this.handler = handler; }
            public Section3EntryList(EventHandler handler, IEnumerable<Section3Entry> le) : base(handler, le) { }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return initialCount; }
            protected override Section3Entry CreateElement(Stream s) { return new Section3Entry(0, elementHandler, s); }
            protected override void WriteCount(Stream s, int count) { return; }
            protected override void WriteElement(Stream s, Section3Entry element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Magic { get { return magic; } set { if (value != (uint)FOURCC("BGEO")) throw new ArgumentException(); if (!magic.Equals(value)) { magic = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public uint Version { get { return version; } set { if (value != 0x00000300) throw new ArgumentException(); if (!version.Equals(value)) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public uint X04 { get { return x04; } set { if (value != 0x04) throw new ArgumentException(); if (!x04.Equals(value)) { x04 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public uint X08 { get { return x08; } set { if (value != 0x08) throw new ArgumentException(); if (!x08.Equals(value)) { x08 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(5)]
        public uint X0C { get { return x0C; } set { if (value != 0x0C) throw new ArgumentException(); if (!x0C.Equals(value)) { x0C = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(6)]
        public Section1EntryList Section1Entries { get { return section1Entries; } set { if (!section1Entries.Equals(value)) { section1Entries = value == null ? null : new Section1EntryList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(7)]
        public Section2EntryList Section2Entries { get { return section2Entries; } set { if (!section2Entries.Equals(value)) { section2Entries = value == null ? null : new Section2EntryList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(8)]
        public Section3EntryList Section3Entries { get { return section3Entries; } set { if (!section3Entries.Equals(value)) { section3Entries = value == null ? null : new Section3EntryList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for BlendGeometryResource wrapper
    /// </summary>
    public class BlendGeometryResourceHandler : AResourceHandler
    {
        public BlendGeometryResourceHandler()
        {
            this.Add(typeof(BlendGeometryResource), new List<string>(new string[] { "0x067CAA11" }));
        }
    }
}