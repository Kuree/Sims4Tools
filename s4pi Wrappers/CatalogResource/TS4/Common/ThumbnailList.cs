using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s4pi.Interfaces;
using System.IO;

namespace CatalogResource.TS4
{
    public class ThumbnailEntry : AHandlerElement, IEquatable<ThumbnailEntry>
    {
        #region Attributes
        private byte imgGroupLabel;
        private Tuple<TGIBlock, TGIBlock, TGIBlock> thumGroupTGI;
        const int recommendedApiVersion = 1;
        #endregion
        public ThumbnailEntry(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }


        #region Data I/O
        private void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.imgGroupLabel = r.ReadByte();
            this.thumGroupTGI = Tuple.Create(new TGIBlock(recommendedApiVersion, handler, "ITG", s), new TGIBlock(recommendedApiVersion, handler, "ITG", s), new TGIBlock(recommendedApiVersion, handler, "ITG", s));
        }

        protected internal void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(this.imgGroupLabel);
            this.thumGroupTGI.Item1.UnParse(s);
            this.thumGroupTGI.Item2.UnParse(s);
            this.thumGroupTGI.Item3.UnParse(s);
        }
        #endregion

        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        #region IEquatable
        public bool Equals(ThumbnailEntry other)
        {
            return this.imgGroupLabel == other.imgGroupLabel && this.thumGroupTGI.Equals(other.thumGroupTGI);
        }
        #endregion

        #region Content Fields
        [ElementPriority(0)]
        public byte MATDLabel { get { return this.imgGroupLabel; } set { if (!this.imgGroupLabel.Equals(value)) { OnElementChanged(); this.imgGroupLabel = value; } } }
        [ElementPriority(1)]
        public Tuple<TGIBlock, TGIBlock, TGIBlock> ThumGroupTGI { get { return this.thumGroupTGI; } set { if (!this.thumGroupTGI.Equals(value)) { OnElementChanged(); this.thumGroupTGI = value; } } }
        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class ThumbnailList : DependentList<ThumbnailEntry>
    {
        #region Constructors
        public ThumbnailList(EventHandler handler) : base(handler) { }
        public ThumbnailList(EventHandler handler, Stream s) : base(handler) { Parse(s); }
        #endregion


        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            var count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                base.Add(new ThumbnailEntry(1, handler, s));
            }
        }

        public override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(base.Count);
            foreach (var thum in this)
            {
                thum.UnParse(s);
            }
        }
        #endregion

        protected override ThumbnailEntry CreateElement(Stream s) { return new ThumbnailEntry(1, handler, s); }
        protected override void WriteElement(Stream s, ThumbnailEntry element) { element.UnParse(s); }
    }
}
