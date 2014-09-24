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
using System.Text;

namespace CASPartResource
{
    /// <summary>
    /// A resource wrapper that understands Face and Clothing resources
    /// </summary>
    public class FaceClothingResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields
        {
            get
            {
                List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                if (version < 8)
                {
                    res.Remove("BlendGeometry");
                }
                return res;
            }
        }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint version = 8;
        string partName = "";
        uint unknown1 = 2;
        TGIBlock blendGeometry;
        CASEntryList casEntries;
        TGIBlockList tgiBlocks;
        #endregion

        public FaceClothingResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            long tgiPosn, tgiSize;
            BinaryReader r = new BinaryReader(s);

            version = r.ReadUInt32();
            tgiPosn = r.ReadUInt32() + s.Position;
            tgiSize = r.ReadUInt32();

            partName = BigEndianUnicodeString.Read(s);
            unknown1 = r.ReadUInt32();

            if (version >= 8)
            {
                blendGeometry = new TGIBlock(requestedApiVersion, OnResourceChanged, s);
            }

            casEntries = new CASEntryList(OnResourceChanged, s);

            tgiBlocks = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize, addEight: version >= 8);

            casEntries.ParentTGIBlocks = tgiBlocks;
        }

        protected override Stream UnParse()
        {
            long posn;
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(version);

            posn = s.Position;
            w.Write((uint)0);
            w.Write((uint)0);

            BigEndianUnicodeString.Write(s, partName);
            w.Write(unknown1);
            
            if (version >= 8)
            {
                if (blendGeometry == null) blendGeometry = new TGIBlock(requestedApiVersion, OnResourceChanged);
                blendGeometry.UnParse(s);
            }

            if (tgiBlocks == null) tgiBlocks = new TGIBlockList(OnResourceChanged, true);
            if (casEntries == null) casEntries = new CASEntryList(OnResourceChanged, tgiBlocks);
            casEntries.UnParse(s);

            tgiBlocks.UnParse(s, posn);

            s.Flush();

            return s;
        }
        #endregion

        #region Sub-types
        public class Entry : AHandlerElement, IEquatable<Entry>
        {
            const int recommendedApiVersion = 1;
            public DependentList<TGIBlock> ParentTGIBlocks { get; set; }
            public override List<string> ContentFields { get { List<string> res = GetContentFields(requestedApiVersion, this.GetType()); res.Remove("ParentTGIBlocks"); return res; } }

            #region Attributes
            AgeGenderFlags ageGender;
            float amount;
            int index;
            #endregion

            #region Constructors
            public Entry(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                this.ParentTGIBlocks = ParentTGIBlocks;
                ageGender = new AgeGenderFlags(requestedApiVersion, handler);
            }
            public Entry(int APIversion, EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                this.ParentTGIBlocks = ParentTGIBlocks;
                Parse(s);
            }
            public Entry(int APIversion, EventHandler handler, Entry basis, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, basis.ageGender, basis.amount, basis.index, ParentTGIBlocks ?? basis.ParentTGIBlocks) { }
            public Entry(int APIversion, EventHandler handler, AgeGenderFlags ageGender, float amount, int index, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                this.ParentTGIBlocks = ParentTGIBlocks;
                this.ageGender = new AgeGenderFlags(0, handler, ageGender);
                this.amount = amount;
                this.index = index;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                ageGender = new AgeGenderFlags(0, handler, s);
                amount = r.ReadSingle();
                index = r.ReadInt32();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                if (ageGender == null) ageGender = new AgeGenderFlags(0, handler);
                ageGender.UnParse(s);
                w.Write(amount);
                w.Write(index);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            #endregion

            #region IEquatable<Entry> Members

            public bool Equals(Entry other)
            {
                return
                    this.ageGender.Equals(other.ageGender)
                    && this.amount == other.amount
                    && this.index == other.index
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as Entry != null ? this.Equals(obj as Entry) : false;
            }

            public override int GetHashCode()
            {
                return
                    this.ageGender.GetHashCode()
                    ^ this.amount.GetHashCode()
                    ^ this.index.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public AgeGenderFlags AgeGender { get { return ageGender; } set { if (!ageGender.Equals(value)) { ageGender = new AgeGenderFlags(0, handler, value); OnElementChanged(); } } }
            [ElementPriority(2)]
            public float Amount { get { return amount; } set { if (amount != value) { amount = value; OnElementChanged(); } } }
            [ElementPriority(3), TGIBlockListContentField("ParentTGIBlocks")]
            public int Index { get { return index; } set { if (index != value) { index = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder/*.Replace("\n", "; ")/**/; } }
            #endregion
        }

        public class EntryList : DependentList<Entry>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (var i in this) i.ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            #region Constructors
            public EntryList(EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler) { _ParentTGIBlocks = ParentTGIBlocks; }
            public EntryList(EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; Parse(s); this.handler = handler; }
            public EntryList(EventHandler handler, IEnumerable<Entry> le, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; foreach (var t in le) this.Add((Entry)t.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override Entry CreateElement(Stream s) { return new Entry(0, elementHandler, s) { ParentTGIBlocks = ParentTGIBlocks }; }
            protected override void WriteElement(Stream s, Entry element) { element.UnParse(s); }
            #endregion

            public override void Add() { this.Add(new Entry(0, handler, _ParentTGIBlocks)); }
            public override void Add(Entry item) { item.ParentTGIBlocks = _ParentTGIBlocks; base.Add(item); }
        }

        public class CASEntry : AHandlerElement, IEquatable<CASEntry>
        {
            const int recommendedApiVersion = 1;
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set
                {
                    if (_ParentTGIBlocks != value)
                    {
                        _ParentTGIBlocks = value;
                        if (geomEntries != null) geomEntries.ParentTGIBlocks = _ParentTGIBlocks;
                        if (boneEntries != null) boneEntries.ParentTGIBlocks = _ParentTGIBlocks;
                    }
                }
            }

            #region Attributes
            FacialRegionFlags facialRegion;
            EntryList geomEntries;
            EntryList boneEntries;
            #endregion

            #region Constructors
            public CASEntry(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                geomEntries = new EntryList(handler, _ParentTGIBlocks);
                geomEntries = new EntryList(handler, _ParentTGIBlocks);
            }
            public CASEntry(int APIversion, EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                this._ParentTGIBlocks = ParentTGIBlocks;
                Parse(s);
            }
            public CASEntry(int APIversion, EventHandler handler, CASEntry basis, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, basis.facialRegion, basis.geomEntries, basis.boneEntries,
                ParentTGIBlocks ?? basis._ParentTGIBlocks) { }
            public CASEntry(int APIversion, EventHandler handler, FacialRegionFlags facialRegion, EntryList geomEntries, EntryList boneEntries,
                DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                this._ParentTGIBlocks = ParentTGIBlocks;
                this.facialRegion = facialRegion;
                this.geomEntries = new EntryList(handler, geomEntries, _ParentTGIBlocks);
                this.boneEntries = new EntryList(handler, boneEntries, _ParentTGIBlocks);
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                facialRegion = (FacialRegionFlags)(new BinaryReader(s).ReadUInt32());
                geomEntries = new EntryList(handler, s, _ParentTGIBlocks);
                boneEntries = new EntryList(handler, s, _ParentTGIBlocks);
            }

            internal void UnParse(Stream s)
            {
                new BinaryWriter(s).Write((uint)facialRegion);
                geomEntries.UnParse(s);
                boneEntries.UnParse(s);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields
            {
                get
                {
                    List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                    res.Remove("ParentTGIBlocks");
                    return res;
                }
            }
            #endregion

            #region IEquatable<CASEntry> Members

            public bool Equals(CASEntry other)
            {
                return
                    this.facialRegion == other.facialRegion
                    && this.geomEntries.Equals(other.geomEntries)
                    && this.boneEntries.Equals(other.boneEntries);
            }

            public override bool Equals(object obj)
            {
                return obj as CASEntry != null ? this.Equals(obj as CASEntry) : false;
            }

            public override int GetHashCode()
            {
                return
                    this.facialRegion.GetHashCode()
                    ^ this.geomEntries.GetHashCode()
                    ^ this.boneEntries.GetHashCode()
                ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public FacialRegionFlags FacialRegion { get { return facialRegion; } set { if (facialRegion != value) { facialRegion = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public EntryList Geoms { get { return geomEntries; } set { if (!geomEntries.Equals(value)) { geomEntries = new EntryList(handler, value, _ParentTGIBlocks); OnElementChanged(); } } }
            [ElementPriority(3)]
            public EntryList Bones { get { return boneEntries; } set { if (!boneEntries.Equals(value)) { boneEntries = new EntryList(handler, value, _ParentTGIBlocks); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class CASEntryList : DependentList<CASEntry>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (var i in this) i.ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            #region Constructors
            public CASEntryList(EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler) { _ParentTGIBlocks = ParentTGIBlocks; }
            public CASEntryList(EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; Parse(s); this.handler = handler; }
            public CASEntryList(EventHandler handler, IEnumerable<CASEntry> le, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; foreach (var t in le) this.Add((CASEntry)t.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override CASEntry CreateElement(Stream s) { return new CASEntry(0, elementHandler, s) { ParentTGIBlocks = ParentTGIBlocks }; }
            protected override void WriteElement(Stream s, CASEntry element) { element.UnParse(s); }
            #endregion

            public override void Add() { this.Add(new CASEntry(0, handler, _ParentTGIBlocks)); }
            public override void Add(CASEntry item) { item.ParentTGIBlocks = _ParentTGIBlocks; base.Add(item); }
        }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public string PartName { get { return partName; } set { if (partName != value) { partName = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public TGIBlock BlendGeometry
        {
            get { if (version < 8) throw new InvalidOperationException(); return blendGeometry; }
            set { if (version < 8) throw new InvalidOperationException(); if (!blendGeometry.Equals(value)) { blendGeometry = new TGIBlock(requestedApiVersion, OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(5)]
        public CASEntryList CASEntries { get { return casEntries; } set { if (!casEntries.Equals(value)) { casEntries = value == null ? null : new CASEntryList(OnResourceChanged, value, tgiBlocks); OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(6)]
        public TGIBlockList TGIBlocks { get { return tgiBlocks; } set { if (!tgiBlocks.Equals(value)) { tgiBlocks = value == null ? null : new TGIBlockList(OnResourceChanged, value, true); casEntries.ParentTGIBlocks = tgiBlocks; OnResourceChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for FaceClothingResource wrapper
    /// </summary>
    public class FaceClothingResourceHandler : AResourceHandler
    {
        public FaceClothingResourceHandler()
        {
            this.Add(typeof(FaceClothingResource), new List<string>(new string[] { "0x0358B08A", "0x062C8204", }));
        }
    }
}
