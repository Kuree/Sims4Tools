/***************************************************************************
 *  Copyright (C) 2009, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Peter Jones                                                            *
 *  Keyi Zhang                                                             *
 *  Cmar                                                                   *
 *  Buzzler                                                                *
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

using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace StblResource
{
    using System.Text;

    /// <summary>
    /// A resource wrapper that understands String Table resources
    /// Currently not compatible with TS3
    /// </summary>
    public class StblResource : AResource
    {
        private const int recommendedApiVersion = 1;

        public override int RecommendedApiVersion
        {
            get { return recommendedApiVersion; }
        }

        public override List<string> ContentFields
        {
            get { return GetContentFields(this.requestedApiVersion, this.GetType()); }
        }

        private static readonly bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes

        private ushort version;
        private byte isCompressed;
        private ulong numEntries;
        private byte[] reserved; //2 bytes
        private uint stringLength;
        private StringEntryList entries;

        #endregion

        public StblResource(int APIversion, Stream s) : base(APIversion, s)
        {
            if (this.stream == null)
            {
                this.version = 5;
                this.stream = this.UnParse();
                this.OnResourceChanged(this, EventArgs.Empty);
            }
            this.stream.Position = 0;
            this.Parse(this.stream);
        }

        #region Data I/O

        private void Parse(Stream s)
        {
            if (s == null)
            {
                s = this.UnParse();
            }
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);

            uint magic = r.ReadUInt32();
            if (checking)
            {
                if (magic != FOURCC("STBL"))
                {
                    throw new InvalidDataException(
                        string.Format("Expected magic tag 0x{0:X8}; read 0x{1:X8}; position 0x{2:X8}",
                            FOURCC("STBL"),
                            magic,
                            s.Position));
                }
            }
            this.version = r.ReadUInt16();
            if (checking)
            {
                if (this.version != 0x05)
                {
                    throw new InvalidDataException(
                        string.Format("Expected version 0x05; read 0x{0:X2}; position 0x{1:X8}", this.version,
                            s.Position));
                }
            }

            this.isCompressed = r.ReadByte();
            this.numEntries = r.ReadUInt64();
            this.reserved = r.ReadBytes(2);
            this.stringLength = r.ReadUInt32();

            this.entries = new StringEntryList(this.OnResourceChanged, s, this.numEntries);
        }

        protected override Stream UnParse()
        {
            Stream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);

            writer.Write((uint)FOURCC("STBL"));
            writer.Write(this.version);

            writer.Write(this.isCompressed);

            if (this.entries == null)
            {
                this.entries = new StringEntryList(this.OnResourceChanged);
            }
            writer.Write((ulong)this.entries.Count);

            if (this.reserved == null)
            {
                this.reserved = new byte[2];
            }
            writer.Write(this.reserved);

            long sizePosition = writer.BaseStream.Position;
            writer.Write(0x00000000);
            uint actualSize = 0;
            foreach (StringEntry entry in this.entries)
            {
                entry.UnParse(memoryStream);
                actualSize += entry.EntrySize;
            }

            writer.BaseStream.Position = sizePosition;
            writer.Write(actualSize);

            memoryStream.Position = 0;
            return memoryStream;
        }

        #endregion

        #region Sub Class

        public class StringEntry : AHandlerElement, IEquatable<StringEntry>
        {
            private uint keyHash;
            private byte flags;
            private string stringValue;

            internal uint EntrySize
            {
                get { return (uint)(this.StringValue.Length + 1); }
            }

            public StringEntry(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
            }

            public StringEntry(int apiVersion, EventHandler handler, Stream s)
                : base(apiVersion, handler)
            {
                this.Parse(s);
            }

            #region AHandlerElement Members

            public override int RecommendedApiVersion
            {
                get { return recommendedApiVersion; }
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(this.requestedApiVersion, this.GetType()); }
            }

            public override AHandlerElement Clone(EventHandler handler)
            {
                StringEntry clone = new StringEntry(this.RecommendedApiVersion, handler)
                                    {
                                        keyHash = this.keyHash,
                                        flags = this.flags,
                                        stringValue = this.stringValue
                                    };
                return clone;
            }

            #endregion

            #region Data I/O

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.keyHash = r.ReadUInt32();
                this.flags = r.ReadByte();
                ushort length = r.ReadUInt16();
                this.stringValue = Encoding.UTF8.GetString(r.ReadBytes(length));
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.keyHash);
                w.Write(this.flags);
                byte[] str = Encoding.UTF8.GetBytes(this.StringValue);
                w.Write((ushort)str.Length);
                w.Write(str);
            }

            #endregion

            public bool Equals(StringEntry other)
            {
                return this.keyHash == other.keyHash && this.flags == other.flags
                       && string.CompareOrdinal(this.StringValue, other.StringValue) == 0;
            }

            [ElementPriority(0)]
            public uint KeyHash
            {
                get { return this.keyHash; }
                set
                {
                    if (this.keyHash != value)
                    {
                        this.OnElementChanged();
                        this.keyHash = value;
                    }
                }
            }

            [ElementPriority(1)]
            public byte Flags
            {
                get { return this.flags; }
                set
                {
                    if (this.flags != value)
                    {
                        this.OnElementChanged();
                        this.flags = value;
                    }
                }
            }

            [ElementPriority(2)]
            public string StringValue
            {
                get { return this.stringValue ?? string.Empty; }
                set
                {
                    if (string.CompareOrdinal(this.StringValue, value) != 0)
                    {
                        this.OnElementChanged();
                        this.stringValue = value;
                    }
                }
            }

            public string Value
            {
                get
                {
                    return string.Format("Key 0x{0:X8}, Flags 0x{1:X2} : {2}",
                        this.keyHash,
                        this.flags,
                        this.StringValue);
                }
            }
        }

        public class StringEntryList : DependentList<StringEntry>
        {
            private readonly ulong numberEntries;

            public StringEntryList(EventHandler handler) : base(handler)
            {
            }

            public StringEntryList(EventHandler handler, Stream s, ulong numEntries) : base(handler)
            {
                this.numberEntries = numEntries;
                this.Parse(s);
            }

            #region Data I/O

            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                for (ulong i = 0; i < this.numberEntries; i++)
                {
                    this.Add(new StringEntry(1, this.handler, s));
                }
            }

            public override void UnParse(Stream s)
            {
                foreach (StringEntry entry in this)
                {
                    entry.UnParse(s);
                }
            }

            #endregion

            protected override StringEntry CreateElement(Stream s)
            {
                return new StringEntry(1, this.handler, s);
            }

            protected override void WriteElement(Stream s, StringEntry element)
            {
                element.UnParse(s);
            }
        }

        #endregion

        #region Content Fields

        [ElementPriority(0)]
        public ushort Version
        {
            get { return this.version; }
            set
            {
                if (this.version != value)
                {
                    this.version = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(1)]
        public byte IsCompressed
        {
            get { return this.isCompressed; }
            set
            {
                if (this.isCompressed != value)
                {
                    this.isCompressed = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(2)]
        public byte[] Reserved
        {
            get { return this.reserved; }
            set
            {
                if (this.reserved != value)
                {
                    this.reserved = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(3)]
        public ulong NumberEntries
        {
            get { return this.numEntries; }
            set { }
        }

        [ElementPriority(4)]
        public uint StringDataLength
        {
            get { return this.stringLength; }
            set { }
        }

        [ElementPriority(5)]
        public StringEntryList Entries
        {
            get { return this.entries; }
            set
            {
                if (this.entries != value)
                {
                    this.entries = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }

        #endregion
    }

    /// <summary>
    /// ResourceHandler for StblResource wrapper
    /// </summary>
    public class StblResourceHandler : AResourceHandler
    {
        public StblResourceHandler()
        {
            this.Add(typeof (StblResource), new List<string>(new[] { "0x220557DA", }));
        }
    }
}