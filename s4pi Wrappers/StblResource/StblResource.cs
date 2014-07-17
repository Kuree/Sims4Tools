using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace StblResource
{
    /// <summary>
    /// A resource wrapper that understands String Table resources
    /// Currently not compatible with TS3
    /// </summary>
    public class StblResource : AResource, IDictionary<uint, string>, System.Collections.IDictionary
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        ushort unknown1;
        byte[] unknown2;
        uint size;
        Dictionary<uint, string> entries;
        #endregion

        public StblResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);

            uint magic = r.ReadUInt32();
            if (checking) if (magic != FOURCC("STBL"))
                    throw new InvalidDataException(String.Format("Expected magic tag 0x{0:X8}; read 0x{1:X8}; position 0x{2:X8}",
                        FOURCC("STBL"), magic, s.Position));
            byte version = r.ReadByte();
            if (checking) if (version != 0x05)
                    throw new InvalidDataException(String.Format("Expected version 0x02; read 0x{0:X2}; position 0x{1:X8}",
                        version, s.Position));
            
            unknown1 = r.ReadUInt16();

            uint count = r.ReadUInt32();

            unknown2 = r.ReadBytes(6);
            size = r.ReadUInt32();

            uint sizeCount = 0;

            entries = new Dictionary<uint, string>(); 
            for (int i = 0; i < count; i++)
            {
                uint key = r.ReadUInt32();
                r.ReadByte();
                Int16 length = r.ReadInt16();
                string value = System.Text.Encoding.UTF8.GetString(r.ReadBytes(length));
                sizeCount += (uint)length + 1;
                if (entries.ContainsKey(key)) continue; 
                entries.Add(key, value);
            }

            if (sizeCount != size) { throw new InvalidCastException(String.Format("Expected size 0x{0}; read 0x{1}", size, sizeCount)); }
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();

            BinaryWriter w = new BinaryWriter(ms);

            w.Write((uint)FOURCC("STBL"));
            w.Write((byte)0x05);

            w.Write(unknown1);

            if (entries == null) entries = new Dictionary<uint, string>();
            w.Write(entries.Count);

            w.Write(unknown2);
            long sizePosition = w.BaseStream.Position;
            w.Write(0x00000000); //w.Write(size);
            int actualSize = 0;
            foreach (var kvp in entries)
            {
                w.Write(kvp.Key);
                w.Write((byte)0);
                w.Write((ushort)kvp.Value.Length);
                w.Write(System.Text.Encoding.UTF8.GetBytes(kvp.Value));
                actualSize += kvp.Value.Length + 1;
            }

            w.BaseStream.Position = sizePosition;
            w.Write(actualSize);

            return ms;
        }
        #endregion

        #region IDictionary<uint,string> Members

        public void Add(uint key, string value) { entries.Add(key, value); OnResourceChanged(this, EventArgs.Empty); }

        public bool ContainsKey(uint key) { return entries.ContainsKey(key); }

        public ICollection<uint> Keys { get { return entries.Keys; } }

        public bool Remove(uint key) { try { return entries.Remove(key); } finally { OnResourceChanged(this, EventArgs.Empty); } }

        public bool TryGetValue(uint key, out string value) { return entries.TryGetValue(key, out value); }

        public ICollection<string> Values { get { return entries.Values; } }

        public string this[uint key]
        {
            get { return entries[key]; }
            set { if (entries[key] != value) { entries[key] = value; OnResourceChanged(this, EventArgs.Empty); } }
        }

        #endregion

        #region ICollection<KeyValuePair<uint,string>> Members

        public void Add(KeyValuePair<uint, string> item) { entries.Add(item.Key, item.Value); }

        public void Clear() { entries.Clear(); OnResourceChanged(this, EventArgs.Empty); }

        public bool Contains(KeyValuePair<uint, string> item) { return entries.ContainsKey(item.Key) && entries[item.Key].Equals(item.Value); }

        public void CopyTo(KeyValuePair<uint, string>[] array, int arrayIndex) { foreach (var kvp in entries) array[arrayIndex++] = kvp; }

        public int Count { get { return entries.Count; } }

        public bool IsReadOnly { get { return false; } }

        public bool Remove(KeyValuePair<uint, string> item) { try { return Contains(item) ? entries.Remove(item.Key) : false; } finally { OnResourceChanged(this, EventArgs.Empty); } }

        #endregion

        #region IEnumerable<KeyValuePair<uint,string>> Members

        public IEnumerator<KeyValuePair<uint, string>> GetEnumerator() { return entries.GetEnumerator(); }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return entries.GetEnumerator(); }

        #endregion

        #region IDictionary Members

        public void Add(object key, object value) { this.Add((uint)key, (string)value); }

        public bool Contains(object key) { return ContainsKey((uint)key); }

        System.Collections.IDictionaryEnumerator System.Collections.IDictionary.GetEnumerator() { return entries.GetEnumerator(); }

        public bool IsFixedSize { get { return false; } }

        System.Collections.ICollection System.Collections.IDictionary.Keys { get { return entries.Keys; } }

        public void Remove(object key) { Remove((uint)key); }

        System.Collections.ICollection System.Collections.IDictionary.Values { get { return entries.Values; } }

        public object this[object key] { get { return this[(uint)key]; } set { this[(uint)key] = (string)value; } }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index) { CopyTo((KeyValuePair<uint, string>[])array, index); }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot { get { return null; } }

        #endregion

        /// <summary>
        /// Return the default dictionary entry for this <c>IDictionary{TKey, TValue}</c>.
        /// </summary>
        /// <returns>The default dictionary entry for this <c>IDictionary{TKey, TValue}</c>.</returns>
        public static System.Collections.DictionaryEntry GetDefault() { return new System.Collections.DictionaryEntry((uint)0, ""); }

        #region Content Fields
        public ushort Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        public byte[] Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        public uint ByteSize { get { return size; } set { if (size != value) { size = value; OnResourceChanged(this, EventArgs.Empty); } } }

        public String Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for StblResource wrapper
    /// </summary>
    public class StblResourceHandler : AResourceHandler
    {
        public StblResourceHandler()
        {
            this.Add(typeof(StblResource), new List<string>(new string[] { "0x220557DA", }));
        }
    }
}
