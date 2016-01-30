/***************************************************************************
 *  Copyright (C) 2016 by Sims 4 Tools Development Team                    *
 *  Credits: Peter Jones, Keyi Zhang, Cmar                                 *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
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
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

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
    public class StblResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        ushort version;
        byte  isCompressed;
        ulong numEntries;
        byte[] reserved;        //2 bytes
        uint stringLength;
        StringEntryList entries;
        #endregion

        public StblResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            if (s == null) s = this.UnParse();
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);

            uint magic = r.ReadUInt32();
            if (checking) if (magic != FOURCC("STBL"))
                    throw new InvalidDataException(String.Format("Expected magic tag 0x{0:X8}; read 0x{1:X8}; position 0x{2:X8}",
                        FOURCC("STBL"), magic, s.Position));
            this.version = r.ReadUInt16();
            if (checking) if (version != 0x05)
                    throw new InvalidDataException(String.Format("Expected version 0x05; read 0x{0:X2}; position 0x{1:X8}",
                        version, s.Position));
            
            this.isCompressed = r.ReadByte();
            this.numEntries = r.ReadUInt64();
            this.reserved = r.ReadBytes(2);
            this.stringLength = r.ReadUInt32();

            this.entries = new StringEntryList(OnResourceChanged, s, this.numEntries);

           // if (sizeCount != stringLength) { throw new InvalidCastException(String.Format("Expected size 0x{0}; read 0x{1}", stringLength, sizeCount)); }
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write((uint)FOURCC("STBL"));
            w.Write(this.version);

            w.Write(this.isCompressed);

            if (entries == null) entries = new StringEntryList(OnResourceChanged); 
            w.Write((ulong)entries.Count);

            if (this.reserved == null) this.reserved = new byte[2];
            w.Write(reserved);

            long sizePosition = w.BaseStream.Position;
            w.Write(0x00000000); //w.Write(size);
            uint actualSize = 0;
            foreach (StringEntry e in entries)
            {
                e.UnParse(ms);
                actualSize += e.EntrySize;
            }

            w.BaseStream.Position = sizePosition;
            w.Write(actualSize);

            ms.Position = 0;
            return ms;
        }
        #endregion

        #region Sub Class
        public class StringEntry : AHandlerElement, IEquatable<StringEntry>
        {
            private uint keyHash;
            private byte flags;
            private string stringValue;
            internal uint EntrySize { get { return (uint)(stringValue.Length + 1); } }
            public StringEntry(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public StringEntry(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { this.Parse(s); }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region Data I/O
            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.keyHash = r.ReadUInt32();
                this.flags = r.ReadByte();
                UInt16 len = r.ReadUInt16();
                this.stringValue = System.Text.Encoding.UTF8.GetString(r.ReadBytes(len));
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.keyHash);
                w.Write(this.flags);
                byte[] str = System.Text.Encoding.UTF8.GetBytes(this.stringValue);
                w.Write((ushort)str.Length);
                w.Write(str);
            }
            #endregion

            public bool Equals(StringEntry other) { return this.keyHash == other.keyHash && this.flags == other.flags && string.Compare(this.stringValue, other.stringValue) == 0; }

            [ElementPriority(0)]
            public uint KeyHash { get { return this.keyHash; } set { if (this.keyHash != value) { OnElementChanged(); this.keyHash = value; } } }
            [ElementPriority(1)]
            public byte Flags { get { return this.flags; } set { if (this.flags != value) { OnElementChanged(); this.flags = value; } } }
            [ElementPriority(2)]
            public string StringValue { get { return this.stringValue; } set { if (String.Compare(this.stringValue, value) != 0) { OnElementChanged(); this.stringValue = value; } } }

            public string Value { get { return String.Format("Key 0x{0:X8}, Flags 0x{1:X2} : {2}", this.keyHash, this.flags, this.stringValue); } }
        }

        public class StringEntryList : DependentList<StringEntry>
        {
            private ulong numberEntries;
            public StringEntryList(EventHandler handler) : base(handler) { }
            public StringEntryList(EventHandler handler, Stream s, ulong numEntries) : base(handler) { this.numberEntries = numEntries; Parse(s); }

            #region Data I/O
            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                for (ulong i = 0; i < numberEntries; i++)
                    base.Add(new StringEntry(1, handler, s));
            }

            public override void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                foreach (var reference in this)
                    reference.UnParse(s);
            }
            #endregion

            protected override StringEntry CreateElement(Stream s) { return new StringEntry(1, handler, s); }
            protected override void WriteElement(Stream s, StringEntry element) { element.UnParse(s); }
        }

        #endregion
        
        #region Content Fields
        [ElementPriority(0)]
        public ushort Version { get { return this.version; } set { if (this.version != value) { this.version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(1)]
        public byte IsCompressed { get { return this.isCompressed; } set { if (this.isCompressed != value) { this.isCompressed = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public byte[] Reserved { get { return this.reserved; } set { if (this.reserved != value) { this.reserved = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public ulong NumberEntries { get { return this.numEntries; } set {  } }
        [ElementPriority(4)]
        public uint StringDataLength { get { return this.stringLength; } set { } }
        [ElementPriority(5)]
        public StringEntryList Entries { get { return this.entries; } set { if (this.entries != value) { this.entries = value; OnResourceChanged(this, EventArgs.Empty); } } }

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
