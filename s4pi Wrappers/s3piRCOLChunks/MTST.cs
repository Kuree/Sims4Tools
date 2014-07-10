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
        EntryList list = null;
        #endregion

        #region Constructors
        public MTST(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public MTST(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public MTST(int APIversion, EventHandler handler, MTST basis)
            : this(APIversion, handler, basis.nameHash, basis.index, basis.list) { }
        public MTST(int APIversion, EventHandler handler, uint nameHash, GenericRCOLResource.ChunkReference index, IEnumerable<Entry> list)
            : base(APIversion, handler, null)
        {
            this.nameHash = nameHash;
            this.index = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, index);
            this.list = list == null ? null : new EntryList(OnRCOLChanged, list);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return "MTST"; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x02019972; } }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            tag = r.ReadUInt32();
            if (checking) if (tag != (uint)FOURCC("MTST"))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: 'MTST'; at 0x{1:X8}", FOURCC(tag), s.Position));
            version = r.ReadUInt32();
            if (checking) if (version != 0x00000200)
                    throw new InvalidDataException(String.Format("Invalid Version read: 0x{0:X8}; expected 0x00000200; at 0x{1:X8}", version, s.Position));

            nameHash = r.ReadUInt32();
            index = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s);
            list = new EntryList(OnRCOLChanged, s);
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
            if (list == null) this.list = new EntryList(OnRCOLChanged);
            list.UnParse(ms);

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

        public class Entry : AHandlerElement, IEquatable<Entry>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            GenericRCOLResource.ChunkReference index;
            State materialState = 0;
            #endregion

            #region Constructors
            public Entry(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Entry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Entry(int APIversion, EventHandler handler, Entry basis) : this(APIversion, handler, basis.index, basis.materialState) { }
            public Entry(int APIversion, EventHandler handler, GenericRCOLResource.ChunkReference index, State materialSet)
                : base(APIversion, handler)
            {
                this.index = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, index);
                this.materialState = materialSet;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s) { index = new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, s); materialState = (State)new BinaryReader(s).ReadUInt32(); }

            internal void UnParse(Stream s) { index.UnParse(s); new BinaryWriter(s).Write((uint)materialState); }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Entry> Members

            public bool Equals(Entry other) { return this.index == other.index && this.materialState == other.materialState; }
            public override bool Equals(object obj)
            {
                return obj as Entry != null && this.Equals(obj as Entry);
            }
            public override int GetHashCode()
            {
                return index.GetHashCode() ^ materialState.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public GenericRCOLResource.ChunkReference Index { get { return index; } set { if (index != value) { new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(2)]
            public State MaterialState { get { return materialState; } set { if (materialState != value) { materialState = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder.Replace("\n", "; "); } }
            #endregion
        }
        public class EntryList : DependentList<Entry>
        {
            #region Constructors
            public EntryList(EventHandler handler) : base(handler) { }
            public EntryList(EventHandler handler, Stream s) : base(handler, s) { }
            public EntryList(EventHandler handler, IEnumerable<Entry> le) : base(handler, le) { }
            #endregion

            #region Data I/O
            protected override Entry CreateElement(Stream s) { return new Entry(0, elementHandler, s); }
            protected override void WriteElement(Stream s, Entry element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public GenericRCOLResource.ChunkReference Index { get { return index; } set { if (index != value) { new GenericRCOLResource.ChunkReference(requestedApiVersion, handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public EntryList Entries { get { return list; } set { if (list != value) { list = value == null ? null : new EntryList(OnRCOLChanged, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }
}
