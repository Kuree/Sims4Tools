using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.GenericRCOLResource
{
    public class VPXY : ARCOLBlock
    {
        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint tag = (uint)FOURCC("VPXY");
        uint version = 4;
        EntryList entryList;
        byte tc02 = 0x02;
        BoundingBox bounds;
        byte[] unused = new byte[4];
        byte modular;
        int ftptIndex;
        TGIBlockList tgiBlockList;
        #endregion

        #region Constructors
        public VPXY(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public VPXY(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public VPXY(int APIversion, EventHandler handler, VPXY basis)
            : this(APIversion, handler,
            basis.version, basis.entryList, basis.tc02, basis.bounds, basis.unused, basis.modular, basis.ftptIndex,
            basis.tgiBlockList) { }
        public VPXY(int APIversion, EventHandler handler,
            uint version, IEnumerable<Entry> entryList, byte tc02, BoundingBox bounds, byte[] unused, byte modular, int ftptIndex,
            IEnumerable<TGIBlock> tgiBlockList)
            : base(APIversion, handler, null)
        {
            this.version = version;
            if (checking) if (version != 4)
                    throw new ArgumentException(String.Format("Invalid Version: 0x{0:X8}; expected 0x00000004", version));
            this.entryList = entryList == null ? null : new EntryList(OnRCOLChanged, entryList);
            this.tc02 = tc02;
            if (checking) if (tc02 != 0x02)
                    throw new ArgumentException(String.Format("Invalid TC02: 0x{0:X2}; expected 0x02", tc02));
            this.bounds = new BoundingBox(requestedApiVersion, handler, bounds);
            this.unused = (byte[])unused.Clone();
            if (checking) if (unused.Length != 4)
                    throw new ArgumentLengthException("Unused", 4);
            this.modular = modular;
            if (modular != 0)
                this.ftptIndex = ftptIndex;
            this.tgiBlockList = tgiBlockList == null ? null : new TGIBlockList(OnRCOLChanged, tgiBlockList);

            if (this.entryList != null)
                this.entryList.ParentTGIBlocks = this.tgiBlockList;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return "VPXY"; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x736884F1; } }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            tag = r.ReadUInt32();
            if (checking) if (tag != (uint)FOURCC("VPXY"))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: 'VPXY'; at 0x{1:X8}", FOURCC(tag), s.Position));
            version = r.ReadUInt32();
            if (checking) if (version != 4)
                    throw new InvalidDataException(String.Format("Invalid Version read: 0x{0:X8}; expected 0x00000004; at 0x{1:X8}", version, s.Position));

            long tgiPosn = r.ReadUInt32() + s.Position;
            long tgiSize = r.ReadUInt32();

            entryList = new EntryList(OnRCOLChanged, s);
            tc02 = r.ReadByte();
            if (checking) if (tc02 != 2)
                    throw new InvalidDataException(String.Format("Invalid TC02 read: 0x{0:X2}; expected 0x02; at 0x{1:X8}", tc02, s.Position));
            bounds = new BoundingBox(requestedApiVersion, handler, s);
            unused = r.ReadBytes(4);
            if (checking) if (unused.Length != 4)
                    throw new EndOfStreamException(String.Format("Unused: expected 4 bytes, read {0}.", unused.Length));
            modular = r.ReadByte();
            if (modular != 0)
                ftptIndex = r.ReadInt32();
            else
                ftptIndex = 0;

            tgiBlockList = new TGIBlockList(OnRCOLChanged, s, tgiPosn, tgiSize, ignoreTgiSize: true);

            entryList.ParentTGIBlocks = tgiBlockList;
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(tag);
            w.Write(version);

            long pos = ms.Position;
            w.Write((uint)0); // tgiOffset
            w.Write((uint)0); // tgiSize

            if (entryList == null) entryList = new EntryList(OnRCOLChanged) { ParentTGIBlocks = tgiBlockList };
            entryList.UnParse(ms);

            w.Write(tc02);

            if (bounds == null) bounds = new BoundingBox(requestedApiVersion, handler);
            bounds.UnParse(ms);

            w.Write(unused);
            w.Write(modular);
            if (modular != 0)
                w.Write(ftptIndex);

            if (tgiBlockList == null) tgiBlockList = new TGIBlockList(OnRCOLChanged);
            tgiBlockList.UnParse(ms, pos);

            entryList.ParentTGIBlocks = tgiBlockList;

            return ms;
        }
        #endregion

        #region Sub-types
        public abstract class Entry : AHandlerElement, IEquatable<Entry>
        {
            const int recommendedApiVersion = 1;
            public abstract DependentList<TGIBlock> ParentTGIBlocks { get; set; }

            #region Constructors
            protected Entry(int APIversion, EventHandler handler) : base(APIversion, handler) { }

            public static Entry CreateEntry(int APIversion, EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks)
            {
                BinaryReader r = new BinaryReader(s);
                byte entryType = r.ReadByte();
                if (entryType == 0x00) return new Entry00(APIversion, handler, 0, r.ReadByte(), s, ParentTGIBlocks);
                if (entryType == 0x01) return new Entry01(APIversion, handler, 1, r.ReadInt32(), ParentTGIBlocks);
                throw new InvalidDataException(String.Format("Unknown EntryType 0x{0:X2} at 0x{1:X8}", entryType, s.Position));
            }
            #endregion

            #region Data I/O
            internal abstract void UnParse(Stream s);
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            /// <summary>
            /// The list of available field names on this API object
            /// </summary>
            public override List<string> ContentFields { get { List<string> res = GetContentFields(requestedApiVersion, this.GetType()); res.Remove("ParentTGIBlocks"); return res;  } }
            #endregion

            #region IEquatable<Entry> Members

            public abstract bool Equals(Entry other);
            public override bool Equals(object obj)
            {
                return obj as Entry != null ? this.Equals(obj as Entry) : false;
            }
            public abstract override int GetHashCode();

            #endregion

            public virtual string Value { get { return ValueBuilder; } }
        }
        public class Entry00 : Entry
        {
            DependentList<TGIBlock> _ParentTGIBlocks;
            public override DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; if (tgiIndexes != null) tgiIndexes.ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            byte entryID;
            Int32IndexList tgiIndexes;

            public Entry00(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, 0, 0, new Int32IndexList(null), ParentTGIBlocks) { }
            public Entry00(int APIversion, EventHandler handler, Entry00 basis, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, 0, basis.entryID, basis.tgiIndexes, ParentTGIBlocks ?? basis._ParentTGIBlocks) { }
            public Entry00(int APIversion, EventHandler handler, byte entryType, byte entryID, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler) { _ParentTGIBlocks = ParentTGIBlocks; this.entryID = entryID; this.tgiIndexes = new Int32IndexList(handler, s, byte.MaxValue, ReadByte, WriteByte, _ParentTGIBlocks); }
            public Entry00(int APIversion, EventHandler handler, byte entryType, byte entryID, IEnumerable<int> tgiIndexes, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler) { _ParentTGIBlocks = ParentTGIBlocks; this.entryID = entryID; this.tgiIndexes = new Int32IndexList(handler, tgiIndexes, byte.MaxValue, ReadByte, WriteByte, _ParentTGIBlocks); }

            internal override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((byte)0x00);
                w.Write(entryID);
                if (tgiIndexes == null) tgiIndexes = new Int32IndexList(handler, byte.MaxValue, ReadByte, WriteByte);
                tgiIndexes.UnParse(s);
                tgiIndexes.ParentTGIBlocks = ParentTGIBlocks;
            }
            static int ReadByte(Stream s) { return (new BinaryReader(s)).ReadByte(); }
            static void WriteByte(Stream s, int count) { (new BinaryWriter(s)).Write((byte)count); }

            public override bool Equals(Entry other)
            {
                return other.GetType() == this.GetType() &&
                    (other as Entry00).entryID == entryID && (other as Entry00).tgiIndexes == tgiIndexes;
            }
            public override int GetHashCode()
            {
                return entryID.GetHashCode() ^ tgiIndexes.GetHashCode();
            }

            #region Content Fields
            public byte EntryID { get { return entryID; } set { if (entryID != value) { entryID = value; OnElementChanged(); } } }
            public Int32IndexList TGIIndexes { get { return tgiIndexes; } set { if (tgiIndexes != value) { tgiIndexes = value == null ? value : new Int32IndexList(handler, value, byte.MaxValue, ReadByte, WriteByte) { ParentTGIBlocks = _ParentTGIBlocks, }; OnElementChanged(); } } }
            #endregion
        }
        public class Entry01 : Entry
        {
            public override DependentList<TGIBlock> ParentTGIBlocks { get; set; }

            int tgiIndex;
            public Entry01(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, 1, 0, ParentTGIBlocks) { }
            public Entry01(int APIversion, EventHandler handler, Entry01 basis, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, 1, basis.tgiIndex, ParentTGIBlocks ?? basis.ParentTGIBlocks) { }
            public Entry01(int APIversion, EventHandler handler, byte entryType, int tgiIndex, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler) { this.ParentTGIBlocks = ParentTGIBlocks; this.tgiIndex = tgiIndex; }
            internal override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((byte)0x01);
                w.Write(tgiIndex);
            }

            public override bool Equals(Entry other) { return other.GetType() == this.GetType() && (other as Entry01).tgiIndex == tgiIndex; }
            public override int GetHashCode()
            {
                return tgiIndex.GetHashCode();
            }

            #region Content Fields
            [TGIBlockListContentField("ParentTGIBlocks")]
            public Int32 TGIIndex { get { return tgiIndex; } set { if (tgiIndex != value) { tgiIndex = value; if (handler != null) handler(this, EventArgs.Empty); } } }
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
            public EntryList(EventHandler handler) : base(handler, Byte.MaxValue) { }
            public EntryList(EventHandler handler, Stream s) : base(handler, s, Byte.MaxValue) { }
            public EntryList(EventHandler handler, IEnumerable<Entry> le) : base(handler, le, Byte.MaxValue) { }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return (new BinaryReader(s)).ReadByte(); }
            protected override void WriteCount(Stream s, int count) { (new BinaryWriter(s)).Write((byte)count); }

            protected override Entry CreateElement(Stream s) { return Entry.CreateEntry(0, elementHandler, s, ParentTGIBlocks); }

            protected override void WriteElement(Stream s, Entry element) { element.UnParse(s); }
            #endregion

            //public override void Add() { throw new NotImplementedException(); }
            public override void Add(Entry item) { item.ParentTGIBlocks = _ParentTGIBlocks; base.Add(item); }
            public override void Add(Type elementType)
            {
                if (elementType.IsAbstract)
                    throw new ArgumentException("Must pass a concrete element type.", "elementType");

                if (!typeof(Entry).IsAssignableFrom(elementType))
                    throw new ArgumentException("The element type must belong to the generic type of the list.", "elementType");

                Entry newElement = Activator.CreateInstance(elementType, new object[] { (int)0, elementHandler, _ParentTGIBlocks, }) as Entry;
                base.Add(newElement);
            }
        }
        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public uint Version { get { return version; } /*set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } }/**/ }
        [ElementPriority(12)]
        public EntryList Entries { get { return entryList; } set { if (entryList != value) { entryList = value == null ? null : new EntryList(OnRCOLChanged, value) { ParentTGIBlocks = tgiBlockList }; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public byte TC02 { get { return tc02; } /*set { if (tc02 != value) { tc02 = value; OnRCOLChanged(this, EventArgs.Empty); } }/**/ }
        [ElementPriority(14)]
        public BoundingBox Bounds
        {
            get { return bounds; }
            set
            {
                if (bounds != value) { bounds = new BoundingBox(requestedApiVersion, handler, value); OnRCOLChanged(this, EventArgs.Empty); }
            }
        }
        [ElementPriority(15)]
        public byte[] Unused
        {
            get { return (byte[])unused.Clone(); }
            set
            {
                if (value.Length != this.unused.Length) throw new ArgumentLengthException("Unused", this.unused.Length);
                if (!unused.Equals<byte>(value)) { unused = value == null ? null : (byte[])value.Clone(); OnRCOLChanged(this, EventArgs.Empty); }
            }
        }
        [ElementPriority(16)]
        public bool Modular { get { return modular != 0; } set { if (Modular != value) { modular = (byte)(value ? 0x01 : 0x00); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17), TGIBlockListContentField("TGIBlocks")]
        public int FTPTIndex
        {
            get { return ftptIndex; }
            set { if (modular == 0) throw new InvalidOperationException(); if (ftptIndex != value) { ftptIndex = value; OnRCOLChanged(this, EventArgs.Empty); } }
        }
        public TGIBlockList TGIBlocks
        {
            get { return tgiBlockList; }
            set
            {
                if (!tgiBlockList.Equals(value))
                {
                    tgiBlockList = value == null ? null : new TGIBlockList(OnRCOLChanged, value);
                    if (entryList != null)
                        entryList.ParentTGIBlocks = tgiBlockList;
                    OnRCOLChanged(this, EventArgs.Empty);
                }
            }
        }

        public string Value
        {
            get
            {
                return ValueBuilder;
                /*
                string fmt;
                string s = "";
                s += "Tag: 0x" + tag.ToString("X8");
                s += "\nVersion: 0x" + version.ToString("X8");

                s += String.Format("\nEntry List ({0:X}):", entryList.Count);
                fmt = "\n  [{0:X" + entryList.Count.ToString("X").Length + "}]: {1}";
                for (int i = 0; i < entryList.Count; i++) s += String.Format(fmt, i, entryList[i].Value);
                s += "\n----";

                s += "\nTC02: 0x" + tc02.ToString("X2");
                s += "\nBounds: " + bounds.Value;
                s += "\nUnused: " + this["Unused"];
                s += "\nModular: " + modular;
                if (Modular)
                    s += "\n" + "FTPTIndex: 0x" + ftptIndex.ToString("X8");

                s += String.Format("\nTGI Blocks ({0:X}):", tgiBlockList.Count);
                fmt = "\n  [{0:X" + tgiBlockList.Count.ToString("X").Length + "}]: {1}";
                for (int i = 0; i < tgiBlockList.Count; i++) s += string.Format(fmt, i, tgiBlockList[i].Value);
                s += "\n----";

                return s;
                /**/
            }
        }
        #endregion
    }
}
