/***************************************************************************
 *  Copyright (C) 2014 by Inge Jones                                       *
 *  Modified by pbox 2015-04-25                                            *
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
using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s4pi.Miscellaneous
{
    public class MTBLResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint version;
        MTBLEntryList entryList;
        #endregion

        public MTBLResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        public class MTBLEntryList : DependentList<MTBLEntry>
        {
                public MTBLEntryList(EventHandler handler, long maxSize = -1)
                    : base(handler, maxSize)
                {
                }

                public MTBLEntryList(EventHandler handler, IEnumerable<MTBLEntry> ilt, long maxSize = -1)
                    : base(handler, ilt, maxSize)
                {
                }

                public MTBLEntryList(EventHandler handler, Stream s, long maxSize = -1)
                    : base(handler, s, maxSize)
                {
                }

                protected override MTBLEntry CreateElement(Stream s)
                {
                    return new MTBLEntry(recommendedApiVersion, this.elementHandler, s);
                }

                protected override void WriteElement(Stream s, MTBLEntry element)
                {
                    element.UnParse(s);
                }

        }

        public class MTBLEntry : AHandlerElement, IEquatable<MTBLEntry>
        {
            long offset = 0;
            ulong modelIID = 0x0000000000000000;
            ulong unkIID_1 = 0x0000000000000000;
            uint unk1;
            float unk2;
            float unk3;
            float unk4;
            float unk5;
            float unk6;
            float unk7;
            uint unk8;
            ulong unkIID_2 = 0x0000000000000000;
                        
            public MTBLEntry(int APIversion, EventHandler handler, MTBLEntry other)
                : this(APIversion, handler, other.modelIID, other.unkIID_1, other.unk1, other.unk2, other.unk3, other.unk4, other.unk5, other.unk6, other.unk7, other.unk8, other.unkIID_2)//,other.unk9, other.unk10)
            {
            }
            public MTBLEntry(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
            }
            public MTBLEntry(int APIversion, EventHandler handler, Stream s)
                : this(APIversion, handler)
            {
                this.Parse(s);
            }
            public MTBLEntry(int APIversion, EventHandler handler, ulong modelIID, ulong unkIID_1, uint unk1, float unk2, float unk3, float unk4, float unk5, float unk6, float unk7, uint unk8, ulong unkIID_2)//, float unk9, float unk10)
                : base(APIversion, handler)
            {
                this.modelIID = modelIID;
                this.unkIID_1 = unkIID_1;
                this.unk1 = unk1;
                this.unk2 = unk2;
                this.unk3 = unk3;
                this.unk4 = unk4;
                this.unk5 = unk5;
                this.unk6 = unk6;
                this.unk7 = unk7;
                this.unk8 = unk8;
                this.unkIID_2 = unkIID_2;
            }

            [ElementPriority(0)]
            public long Offset
            {
                get { return offset; }
            }
            [ElementPriority(1)]
            public ulong ModelIID
            {
                get { return modelIID; }
                set { modelIID = value; OnElementChanged(); }
            }
            [ElementPriority(2)]
            public ulong UnkIID_1
            {
                get { return unkIID_1; }
                set { this.unkIID_1 = value; OnElementChanged(); }
            }
            [ElementPriority(3)]
            public uint Unk1
            {
                get { return unk1; }
                set { this.unk1 = value; OnElementChanged(); }
            }
            [ElementPriority(4)]
            public float ThumbnailBoundsMinX
            {
                get { return unk2; }
                set { this.unk2 = value; OnElementChanged(); }
            }

            [ElementPriority(5)]
            public float ThumbnailBoundsMaxX
            {
                get { return unk5; }
                set { this.unk5 = value; OnElementChanged(); }
            }
            [ElementPriority(6)]
            public float ThumbnailBoundsMinY
            {
                get { return unk4; }
                set { this.unk4 = value; OnElementChanged(); }
            }
            [ElementPriority(7)]
            public float ThumbnailBoundsMaxY
            {
                get { return unk7; }
                set { this.unk7 = value; OnElementChanged(); }
            }
            [ElementPriority(8)]
            public float ThumbnailBoundsMinZ
            {
                get { return unk3; }
                set { this.unk3 = value; OnElementChanged(); }
            }
            [ElementPriority(9)]
            public float ThumbnailBoundsMaxZ
            {
                get { return unk6; }
                set { this.unk6 = value; OnElementChanged(); }
            }

            [ElementPriority(10)]
            public uint Unk8
            {
                get { return unk8; }
                set { this.unk8 = value; OnElementChanged(); }
            }
            /*
            [ElementPriority(11)]
            public float Unk9
            {
                get { return unk9; }
                set { this.unk9 = value; OnElementChanged(); }
            }

            [ElementPriority(12)]
            public float Unk10
            {
                get { return unk10; }
                set { this.unk10 = value; OnElementChanged(); }
            }
            */
            [ElementPriority(13)]
            public ulong UnkIID_2
            {
                get { return unkIID_2; }
                set { this.unkIID_2 = value; OnElementChanged(); }
            }

            public override int RecommendedApiVersion
            {
                get { return recommendedApiVersion; }
            }
 
            public override List<string> ContentFields
            {
                get { return GetContentFields(0, GetType()); }
            }
 
            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.offset = s.Position;
                this.modelIID = br.ReadUInt64();
                this.unkIID_1 = br.ReadUInt64();
                this.unk1 = br.ReadUInt32();
                this.unk2 = br.ReadSingle();
                this.unk3 = br.ReadSingle();
                this.unk4 = br.ReadSingle();
                this.unk5 = br.ReadSingle();
                this.unk6 = br.ReadSingle();
                this.unk7 = br.ReadSingle();
                this.unk8 = br.ReadUInt32();
                //this.unk9 = br.ReadSingle();
                //this.unk10 = br.ReadSingle();
                this.unkIID_2 = br.ReadUInt64();

            }
            public string Value { get { return ValueBuilder; } }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(this.modelIID);
                bw.Write(this.unkIID_1);
                bw.Write(this.unk1);
                bw.Write(this.unk2);
                bw.Write(this.unk3);
                bw.Write(this.unk4);
                bw.Write(this.unk5);
                bw.Write(this.unk6);
                bw.Write(this.unk7);
                bw.Write(this.unk8);
                //bw.Write(this.unk9);
                //bw.Write(this.unk10);
                bw.Write(this.unkIID_2);

            }
            public bool Equals(MTBLEntry other)
            {
                return this.modelIID == other.modelIID 
                    && this.unkIID_1 == other.unkIID_1
                    && this.unk1 == other.unk1
                    && this.unk2 == other.unk2
                    && this.unk3 == other.unk3
                    && this.unk4 == other.unk4
                    && this.unk5 == other.unk5
                    && this.unk6 == other.unk6
                    && this.unk7 == other.unk7
                    && this.unk8 == other.unk8
                    && this.unkIID_2 == other.unkIID_2;

                    //&& this.unk9 == other.unk9
                    //&& this.unk10 == other.unk10;
            }

        }

        #region Data I/O
        void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);

            uint magic = r.ReadUInt32();
            if (checking) if (magic != FOURCC("MTBL"))
                    throw new InvalidDataException(String.Format("Expected magic tag 0x{0:X8}; read 0x{1:X8}; position 0x{2:X8}",
                        FOURCC("MTBL"), magic, s.Position));

            version = r.ReadUInt32();
            this.EntryList= new MTBLEntryList(this.OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();

            BinaryWriter w = new BinaryWriter(ms);

            w.Write((uint)FOURCC("MTBL"));

            w.Write(version);

            this.entryList.UnParse(ms);

            return ms;
        }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public MTBLEntryList EntryList
        {
            get { return entryList; }
            set { entryList = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        public string Value { get { return ValueBuilder; } }

        #endregion
    }

    /// <summary>
    /// ResourceHandler for MTBLResource wrapper
    /// </summary>
    public class MTBLResourceHandler : AResourceHandler
    {
        public MTBLResourceHandler()
        {
            this.Add(typeof(MTBLResource), new List<string>(new string[] { "0x81CA1A10", }));
        }
    }
}
