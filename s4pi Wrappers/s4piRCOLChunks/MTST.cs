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

namespace s4pi.GenericRCOLResource
{
    public class MTST : ARCOLBlock
    {
        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint tag = (uint)FOURCC("MTST");
        uint version = 0x00000200;

        uint nameHash = 0;
        GenericRCOLResource.ChunkReference index;
        Type200EntryList matdList200 = null;
        Type300EntryList matdList300 = null;
        #endregion

        #region Constructors
        public MTST(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public MTST(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public MTST(int APIversion, EventHandler handler, MTST basis)
            : base(APIversion, handler, null)
        {
            this.version = basis.version;
            this.nameHash = basis.nameHash;
            this.index = new GenericRCOLResource.ChunkReference(requestedApiVersion,
            handler, basis.index);
            this.matdList200 = basis.matdList200 == null ? null : new Type200EntryList(OnRCOLChanged, basis.matdList200);
            this.matdList300 = basis.matdList300 == null ? null : new Type300EntryList(OnRCOLChanged, basis.matdList300);
        }
      //  public MTST(int APIversion, EventHandler handler, uint nameHash, GenericRCOLResource.ChunkReference index, IEnumerable<Type300Entry> list)
      //      : base(APIversion, handler, null)
      //  {
      //      this.nameHash = nameHash;
      //      this.index = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, index);
      //      this.matdList200 = list == null ? null : new Type200EntryList(OnRCOLChanged, list);
      //  }
        #endregion

        #region ARCOLBlock
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            tag = r.ReadUInt32();
            if (checking) if (tag != (uint)FOURCC("MTST"))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: 'MTST'; at 0x{1:X8}", FOURCC(tag), s.Position));
            version = r.ReadUInt32();
            //if (checking) if (version != 0x00000200)
            //        throw new InvalidDataException(String.Format("Invalid Version read: 0x{0:X8}; expected 0x00000200; at 0x{1:X8}", version, s.Position));

            nameHash = r.ReadUInt32();
            index = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
            if (this.version < 768U)
            {
                matdList200 = new Type200EntryList(OnRCOLChanged, s);
            }
            else
            {
                matdList300 = new Type300EntryList(OnRCOLChanged, s);
            }
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(tag);
            w.Write(version);

            w.Write(nameHash);
            if (index == null) this.index = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, 0);
            index.UnParse(ms);
            if (matdList200 == null) this.matdList200 = new Type200EntryList(OnRCOLChanged);
            if (matdList300 == null) this.matdList300 = new Type300EntryList(OnRCOLChanged);
            if (this.version < 768U)
                this.matdList200.UnParse(ms);
            else
                this.matdList300.UnParse(ms);
            return ms;
        }
        #endregion

        #region Sub-types
        public enum State : uint
        {
            Default = 0x2EA8FB98,
            Dirty = 0xEEAB4327,
            VeryDirty = 0x2E5DF9BB,
            Burnt = 0xC3867C32,
            Clogged = 0x257FB026,
            carLightsOff = 0xE4AF52C1,
        }

        public class Type300Entry : AHandlerElement, IEquatable<Type300Entry>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            GenericRCOLResource.ChunkReference matdIndex;
            State materialState = 0;
            uint materialVariant = 0;
            #endregion

            #region Constructors
            public Type300Entry(int APIversion, EventHandler handler)
                : base(APIversion, handler) {}
 
            public Type300Entry(int APIversion, EventHandler handler, Stream s)
                : base(APIversion, handler)
            {
                Parse(s);
            }
            public Type300Entry(int APIversion, EventHandler handler, Type300Entry basis)
                : this(APIversion, handler, basis.matdIndex, basis.materialState, basis.materialVariant) { }
            public Type300Entry(int APIversion, EventHandler handler, GenericRCOLResource.ChunkReference index, State materialSet, uint materialVariant)
                : base(APIversion, handler)
            {
                this.matdIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, index);
                this.materialState = materialSet;
                this.materialVariant = materialVariant;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s) 
            { 
                BinaryReader r = new BinaryReader(s); 
                matdIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s); 
                materialState = (State)r.ReadUInt32(); 
                materialVariant = r.ReadUInt32(); 
            }

            internal void UnParse(Stream s) 
            { 
                BinaryWriter w = new BinaryWriter(s);
                if (this.matdIndex == null)
                    this.matdIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, 0U);
                matdIndex.UnParse(s); 
                w.Write((uint)materialState); 
                w.Write(materialVariant);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields
            {
                get
                {
                    var res = GetContentFields(requestedApiVersion, this.GetType());
                    return res;
                }
            }

            #endregion

            #region IEquatable<Entry> Members

            public bool Equals(Type300Entry other) { return this.matdIndex == other.matdIndex && this.materialState == other.materialState && this.materialVariant == other.materialVariant; }
            public override bool Equals(object obj)
            {
                return obj as Type300Entry != null && this.Equals(obj as Type300Entry);
            }
            public override int GetHashCode()
            {
                return matdIndex.GetHashCode() ^ materialState.GetHashCode() ^ materialVariant.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public GenericRCOLResource.ChunkReference Index { get { return matdIndex; } set { if (matdIndex != value) { new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(2)]
            public State MaterialState { get { return materialState; } set { if (materialState != value) { materialState = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public uint MaterialVariant { get { return this.materialVariant; } set { if (this.materialVariant != value) { this.materialVariant = value; this.OnElementChanged(); } } }
            public string Value { get { return ValueBuilder.Replace("\n", "; "); } }
            #endregion
        }
        public class Type300EntryList : DependentList<Type300Entry>
        {
            #region Constructors
            public Type300EntryList(EventHandler handler) : base(handler) { }
            public Type300EntryList(EventHandler handler, Stream s) : base(handler) { this.Parse(s); }
            public Type300EntryList(EventHandler handler, IEnumerable<Type300Entry> le) : base(handler, le) { }
            #endregion

            #region Data I/O
            protected override Type300Entry CreateElement(Stream s) { return new Type300Entry(0, elementHandler, s); }
            protected override void WriteElement(Stream s, Type300Entry element) { element.UnParse(s); }
            #endregion
        }
        public class Type200Entry : AHandlerElement, IEquatable<Type200Entry>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            GenericRCOLResource.ChunkReference matdIndex;
            State materialState = 0;
            #endregion

            #region Constructors
            public Type200Entry(int APIversion, EventHandler handler)
                : base(APIversion, handler) { }

            public Type200Entry(int APIversion, EventHandler handler, Stream s)
                : base(APIversion, handler)
            {
                Parse(s);
            }
            public Type200Entry(int APIversion, EventHandler handler, Type200Entry basis)
                : this(APIversion, handler, basis.matdIndex, basis.materialState) { }
            public Type200Entry(int APIversion, EventHandler handler, GenericRCOLResource.ChunkReference index, State materialSet)
                : base(APIversion, handler)
            {
                this.matdIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, index);
                this.materialState = materialSet;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                matdIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
                materialState = (State)r.ReadUInt32();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                if (this.matdIndex == null)
                    this.matdIndex = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, 0U);
                matdIndex.UnParse(s);
                w.Write((uint)materialState);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields
            {
                get
                {
                    var res = GetContentFields(requestedApiVersion, this.GetType());
                    return res;
                }
            }

            #endregion

            #region IEquatable<Entry> Members

            public bool Equals(Type200Entry other) { return this.matdIndex == other.matdIndex && this.materialState == other.materialState; }
            public override bool Equals(object obj)
            {
                return obj as Type200Entry != null && this.Equals(obj as Type200Entry);
            }
            public override int GetHashCode()
            {
                return matdIndex.GetHashCode() ^ materialState.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public GenericRCOLResource.ChunkReference Index { get { return matdIndex; } set { if (matdIndex != value) { new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(2)]
            public State MaterialState { get { return materialState; } set { if (materialState != value) { materialState = value; OnElementChanged(); } } }
            public string Value { get { return ValueBuilder.Replace("\n", "; "); } }
            #endregion
        }
        public class Type200EntryList : DependentList<Type200Entry>
        {
            #region Constructors
            public Type200EntryList(EventHandler handler) : base(handler) { }
            public Type200EntryList(EventHandler handler, Stream s) : base(handler) { this.Parse(s); }
            public Type200EntryList(EventHandler handler, IEnumerable<Type200Entry> le) : base(handler, le) { }
            #endregion

            #region Data I/O
            protected override Type200Entry CreateElement(Stream s) { return new Type200Entry(0, elementHandler, s); }
            protected override void WriteElement(Stream s, Type200Entry element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region Content Fields
        [ElementPriority(2)]
        public override string Tag { get { return "MTST"; } }
        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02019972; } }
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public GenericRCOLResource.ChunkReference Index { get { return index; } set { if (index != value) { new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public Type200EntryList Type200Entries { get { return matdList200; } set { if (matdList200 != value) { matdList200 = value == null ? null : new Type200EntryList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public Type300EntryList Type300Entries { get { return matdList300; } set { if (matdList300 != value) { matdList300 = value == null ? null : new Type300EntryList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get
            {
                List<string> contentFields = base.ContentFields;
                if (this.version < 768U)
                    contentFields.Remove("Type300Entries");
                else
                    contentFields.Remove("Type200Entries");
                return contentFields;
            }
        }
        #endregion
    }

}
