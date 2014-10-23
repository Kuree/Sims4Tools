using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s4pi.Interfaces;
using System.IO;

namespace CatalogResource.TS4
{
    public class MATDEntry : AHandlerElement, IEquatable<MATDEntry>
    {
        #region Attributes
        private byte matdLabel;
        private TGIBlock matdTGI;
        const int recommendedApiVersion = 1;
        #endregion
        public MATDEntry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }


        #region Data I/O
        private void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.matdLabel = r.ReadByte();
            this.matdTGI = new TGIBlock(recommendedApiVersion, handler, "ITG", s);
        }

        protected internal void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.matdLabel);
            this.matdTGI.UnParse(s);
        }
        #endregion

        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        #region IEquatable
        public bool Equals(MATDEntry other)
        {
            return this.matdLabel == other.matdLabel && this.matdTGI.Equals(other.matdTGI);
        }
        #endregion

        #region Content Fields
        [ElementPriority(0)]
        public byte MATDLabel { get { return this.matdLabel; } set { if (!this.matdLabel.Equals(value)) { OnElementChanged(); this.matdLabel = value; } } }
        [ElementPriority(1)]
        public TGIBlock MATDTGI { get { return this.matdTGI; } set { if (!this.matdTGI.Equals(value)) { OnElementChanged(); this.matdTGI = value; } } }
        #endregion
    }

    public class MATDList : DependentList<MATDEntry>
    {
        #region Constructors
        public MATDList(EventHandler handler) : base(handler) { }
        public MATDList(EventHandler handler, Stream s) : base(handler) { Parse(s); }
        #endregion


        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            var count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                base.Add(new MATDEntry(1, handler, s));
            }
        }

        public override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(base.Count);
            foreach (var matd in this)
            {
                matd.UnParse(s);
            }
        }

        protected override MATDEntry CreateElement(Stream s) { return new MATDEntry(1, handler, s); }
        protected override void WriteElement(Stream s, MATDEntry element) { element.UnParse(s); }
        #endregion
    }
}
