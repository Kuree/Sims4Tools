using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s4pi.Miscellaneous
{
    public class DeformerMapResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        
        static bool checking = s4pi.Settings.Settings.Checking;

        public uint version { get; set; }
        public DataBlobHandler unknown { get; set; }
        public UnknownEntryLIst unknownEntryList { get; set; }
        public string Value { get { return ValueBuilder; } }

        public DeformerMapResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            s.Position = 0;
            this.version = r.ReadUInt32();
            this.unknown = new DataBlobHandler(recommendedApiVersion, OnResourceChanged, 31, s);
            uint size = r.ReadUInt32();
            this.unknownEntryList = new UnknownEntryLIst(OnResourceChanged, s, size);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(this.version);
            this.unknown.UnParse(ms);
            long tmpPos = ms.Position;
            w.Write(0);
            this.unknownEntryList.UnParse(ms);
            long endPos = ms.Position;
            ms.Position = tmpPos;
            w.Write((uint)(endPos - tmpPos));
            ms.Position = 0;
            return ms;
        }
        #endregion

        #region Sub-Class
        public class UnknownEntry : AHandlerElement, IEquatable<UnknownEntry>
        {
            public SimpleList<uint> data { get; set; }
            public byte Size { get; private set; }

            public UnknownEntry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.Size = r.ReadByte();
                int realSize = (this.Size - 1) / 4;
                this.data = new SimpleList<uint>(handler);
                for(int i = 0; i< realSize; i++) this.data.Add(r.ReadUInt32());

            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.Size);
                foreach (var i in this.data) w.Write(i);                
                
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable
            public bool Equals(UnknownEntry other)
            {
                return this.Size == other.Size && this.data.Equals(other.data);
            }
            #endregion

            public string Value { get { return ValueBuilder; } }
        }

        public class UnknownEntryLIst : DependentList<UnknownEntry>
        {
            public UnknownEntryLIst(EventHandler handler) : base(handler) { }
            public UnknownEntryLIst(EventHandler handler, Stream s, uint size) : base(handler) { Parse(s, size); }

            #region Data I/O
            protected void Parse(Stream s, uint size)
            {
                while (size != 0)
                {
                    var entry = new UnknownEntry(1, handler, s);
                    size -= entry.Size;
                    this.Add(entry);
                }

            }

            public override void UnParse(Stream s)
            {
                foreach (var bone in this) bone.UnParse(s);
            }

            protected override UnknownEntry CreateElement(Stream s) { return new UnknownEntry(1, handler, s); }
            protected override void WriteElement(Stream s, UnknownEntry element) { element.UnParse(s); }
            #endregion
        }
        #endregion
    }

    public class DeformerMapResourceHandler : AResourceHandler
    {
        public DeformerMapResourceHandler()
        {
            if (s4pi.Settings.Settings.IsTS4)
                this.Add(typeof(DeformerMapResource), new List<string>(new string[] { "0xDB43E069", }));
        }
    }
}
