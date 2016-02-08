/***************************************************************************
 *  Copyright (C) 2016 by Inge Jones                                       *
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
    public class TRIMResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        string sig = "TRIM";
        uint version;
        TRIMpt3EntryList pt3entryList;
        TRIMpt4EntryList pt4entryList;
        TGIBlock materialSetKey;
        byte hasFootprint;

        #endregion

        public TRIMResource(int APIversion, Stream s) : base(APIversion, s)
        {
            if (stream == null)
            {
                version = 4;
                stream = UnParse(); OnResourceChanged(this, EventArgs.Empty);
            }
            stream.Position = 0; Parse(stream);
        }

        public class TRIMpt3EntryList : DependentList<TRIMpt3Entry>
        {
            public TRIMpt3EntryList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public TRIMpt3EntryList(EventHandler handler, IEnumerable<TRIMpt3Entry> ilt, long maxSize = -1)
                : base(handler, ilt, maxSize)
            {
            }


            public TRIMpt3EntryList(EventHandler handler, Stream s, long maxSize = -1)
                : base(handler, s, maxSize)
            {
            }

            protected override TRIMpt3Entry CreateElement(Stream s)
            {
                return new TRIMpt3Entry(recommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, TRIMpt3Entry element)
            {
                element.UnParse(s);
            }

            public TRIMpt4EntryList ToTRIMpt4EntryList()
            //public IEnumerable<TRIMpt4Entry> ToTRIMpt4EntryList()
            {
                TRIMpt4EntryList result = new TRIMpt4EntryList(null);
                for (int i = 0; i < this.Count; i++)
                {
                    result.Add(new TRIMpt4Entry(recommendedApiVersion, null, this[i].X, this[i].Y, this[i].V, 0));
                }
                return result;
            }

        }

        public class TRIMpt4EntryList : DependentList<TRIMpt4Entry>
        {
            public TRIMpt4EntryList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public TRIMpt4EntryList(EventHandler handler, IEnumerable<TRIMpt4Entry> ilt, long maxSize = -1)
                : base(handler, ilt, maxSize)
            {
            }

            public TRIMpt4EntryList(EventHandler handler, Stream s, long maxSize = -1)
                : base(handler, s, maxSize)
            {
            }

            protected override TRIMpt4Entry CreateElement(Stream s)
            {
                return new TRIMpt4Entry(recommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, TRIMpt4Entry element)
            {
                element.UnParse(s);
            }

            public TRIMpt3EntryList ToTRIMpt3EntryList()
            //public IEnumerable<TRIMpt3Entry> ToTRIMpt3EntryList()

            {
                TRIMpt3EntryList result = new TRIMpt3EntryList(null);
                for (int i = 0; i < this.Count; i++)
                {
                    result.Add(new TRIMpt3Entry(recommendedApiVersion, null, this[i].X, this[i].Y, this[i].V));
                }
                return result;
            }
        }

        public class TRIMpt3Entry : AHandlerElement, IEquatable<TRIMpt3Entry>
        {
            float x;
            float y;
            float v;


            public TRIMpt3Entry(int apiVersion, EventHandler handler, TRIMpt3Entry other)
                : this(apiVersion, handler, other.x, other.y, other.v)
            {
            }
            public TRIMpt3Entry(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
            }
            public TRIMpt3Entry(int apiVersion, EventHandler handler, Stream s)
                : this(apiVersion, handler)
            {
                this.Parse(s);
            }
            public TRIMpt3Entry(int apiVersion, EventHandler handler, float x, float y, float v)
                : base(apiVersion, handler)
            {
                this.x = x;
                this.y = y;
                this.v = v;
            }

            [ElementPriority(0)]
            public float X
            {
                get { return x; }
                set { this.x = value; OnElementChanged(); }
            }

            [ElementPriority(1)]
            public float Y
            {
                get { return y; }
                set { this.y = value; OnElementChanged(); }
            }
            [ElementPriority(2)]
            public float V
            {
                get { return v; }
                set { this.v = value; OnElementChanged(); }
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
                this.x = br.ReadSingle();
                this.y = br.ReadSingle();
                this.v = br.ReadSingle();
            }
            public string Value { get { return ValueBuilder; } }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);

                bw.Write(this.x);
                bw.Write(this.y);
                bw.Write(this.v);
            }

            public bool Equals(TRIMpt3Entry other)
            {
                return this.x == other.x
                    && this.y == other.y
                    && this.v == other.v;
            }

        }

        public class TRIMpt4Entry : AHandlerElement, IEquatable<TRIMpt4Entry>
        {
            float x;
            float y;
            float v;
            float mappingMode;

                        
            public TRIMpt4Entry(int apiVersion, EventHandler handler, TRIMpt4Entry other)
                : this(apiVersion, handler, other.x, other.y, other.v, other.mappingMode)
            {
            }
            public TRIMpt4Entry(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
            }
            public TRIMpt4Entry(int apiVersion, EventHandler handler, Stream s)
                : this(apiVersion, handler)
            {
                this.Parse(s);
            }
            public TRIMpt4Entry(int apiVersion, EventHandler handler, float x, float y, float v, float mappingMode)
                : base(apiVersion, handler)
            {
                this.x = x;
                this.y = y;
                this.v = v;
                this.mappingMode = mappingMode;
            }

            [ElementPriority(0)]
            public float X
            {
                get { return x; }
                set { this.x = value; OnElementChanged(); }
            }

            [ElementPriority(1)]
            public float Y
            {
                get { return y; }
                set { this.y = value; OnElementChanged(); }
            }
            [ElementPriority(2)]
            public float V
            {
                get { return v; }
                set { this.v = value; OnElementChanged(); }
            }
            [ElementPriority(3)]
            public float MappingMode
            {
                get { return mappingMode; }
                set { this.mappingMode = value; OnElementChanged(); }
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
                this.x = br.ReadSingle();
                this.y = br.ReadSingle();
                this.v = br.ReadSingle();
                this.mappingMode = br.ReadSingle();
            }
            public string Value { get { return ValueBuilder; } }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);

                bw.Write(this.x);
                bw.Write(this.y);
                bw.Write(this.v);
                bw.Write(this.mappingMode);
            }

            public bool Equals(TRIMpt4Entry other)
            {
                return this.x == other.x
                    && this.y == other.y
                    && this.v == other.v
                    && this.mappingMode == other.mappingMode;
            }

        }


        #region Data I/O
        void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);

            uint magic = r.ReadUInt32();
            if (checking)
                if (magic != FOURCC("TRIM"))
                    throw new InvalidDataException(String.Format("Expected magic tag 0x{0:X8}; read 0x{1:X8}; position 0x{2:X8}",
                        FOURCC("TRIM"), magic, s.Position));

            version = r.ReadUInt32();
            if (version == 3)
            {
                this.pt3entryList = new TRIMpt3EntryList(this.OnResourceChanged, s);
            }
            else
            {
                this.pt4entryList = new TRIMpt4EntryList(this.OnResourceChanged, s);
            }
            this.materialSetKey = new TGIBlock(recommendedApiVersion, null, s); // this is default order TGI
            this.hasFootprint = r.ReadByte();
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write((uint)FOURCC("TRIM"));
            w.Write(version);
            if (version == 3)
            {
                if (this.pt3entryList == null)
                {
                    if (this.pt4entryList == null)
                    {
                        this.pt3entryList = new TRIMpt3EntryList(this.OnResourceChanged);
                    }
                    else
                    {
                        this.pt3entryList = this.pt4entryList.ToTRIMpt3EntryList();
                    }
                }
                this.pt3entryList.UnParse(ms);
            }
            else
            {
                if (this.pt4entryList == null)
                {
                    if (this.pt3entryList == null)
                    {
                        this.pt4entryList = new TRIMpt4EntryList(this.OnResourceChanged);
                    }
                    else
                    {
                        this.pt4entryList = this.pt3entryList.ToTRIMpt4EntryList();
                    }
                }

                this.pt4entryList.UnParse(ms);
            }
            if (this.materialSetKey== null) { this.materialSetKey = new TGIBlock(recommendedApiVersion, this.OnResourceChanged, TGIBlock.Order.TGI); }
            this.materialSetKey.UnParse(ms);
            w.Write(hasFootprint);

            return ms;
        }


        #endregion



        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public TRIMpt3EntryList Point3EntryList
        {
            get { return pt3entryList; }
            set { pt3entryList = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(3)]
        public TRIMpt4EntryList Point4EntryList
        {
            get { return pt4entryList; }
            set { pt4entryList = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(4)]
        public TGIBlock MaterialSetKey
        {
            get { return materialSetKey;  }
            set { materialSetKey = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(5)]
        public byte HasFootprint
        {
            get { return hasFootprint; }
            set { hasFootprint = value; this.OnResourceChanged(this, EventArgs.Empty); }
        }

        public string Value { get { return ValueBuilder; } }

        public override List<string> ContentFields
        {
            get
            {
                var res = GetContentFields(RecommendedApiVersion, this.GetType());
                if (this.Version == 3)
                {
                    res.Remove("Point4EntryList");
                }
                else
                {
                    res.Remove("Point3EntryList");
                }
                return res;
            }
        }


        #endregion
    }

    /// <summary>
    /// ResourceHandler for TRIMResource wrapper
    /// </summary>
    public class TRIMResourceHandler : AResourceHandler
    {
        public TRIMResourceHandler()
        {
            this.Add(typeof(TRIMResource), new List<string>(new string[] { "0x76BCF80C", }));
        }
    }
}
