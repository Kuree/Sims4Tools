using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CatalogResource
{
    public class ModlEntry : AHandlerElement, IEquatable<ModlEntry>
    {
        private UInt16 unknown;
        private TGIBlock modlReference;
        const int recommendedApiVersion = 1;

        public ModlEntry(int apiVersion, EventHandler handler) : base(apiVersion, handler) { this.UnParse(new MemoryStream()); }
        public ModlEntry(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }

        private void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.unknown = r.ReadUInt16();
            this.modlReference = new TGIBlock(RecommendedApiVersion, handler, "ITG", s);
        }

        public void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.unknown);
            if (this.modlReference == null) this.modlReference = new TGIBlock(RecommendedApiVersion, handler, "ITG");
            this.modlReference.UnParse(s);
        }

        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        public bool Equals(ModlEntry other)
        {
            return this.unknown == other.unknown && this.modlReference.Equals(other.modlReference);
        }

        [ElementPriority(0)]
        public UInt16 Unknown { get { return this.unknown; } set { if (!value.Equals(this.unknown)) { OnElementChanged(); this.unknown = value; } } }
        [ElementPriority(1)]
        public TGIBlock MODLReference { get { return this.modlReference; } set { if (!value.Equals(this.modlReference)) { OnElementChanged(); this.modlReference = value; } } }
        public string Value { get { return ValueBuilder; } }
    }

    public class ModlEntryList : DependentList<ModlEntry>
    {
        #region Constructors
        public ModlEntryList(EventHandler handler) : base(handler) { }
        public ModlEntryList(EventHandler handler, Stream s) : base(handler) { Parse(s); }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            int count = s.ReadByte();
            for (byte i = 0; i < count; i++)
                this.Add(new ModlEntry(1, handler, s));
        }

        public override void UnParse(Stream s)
        {
            s.WriteByte((byte)this.Count);
            foreach (var modl in this)
                modl.UnParse(s);
        }

        protected override ModlEntry CreateElement(Stream s) { return new ModlEntry(1, handler, s); }
        protected override void WriteElement(Stream s, ModlEntry element) { element.UnParse(s); }
        #endregion
    }
}
