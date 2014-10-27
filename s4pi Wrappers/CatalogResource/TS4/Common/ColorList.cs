using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CatalogResource.TS4
{
    public class SwatchColor : AHandlerElement, IEquatable<SwatchColor>
    {
        private Color color;
        const int recommendedApiVersion = 1;

        public SwatchColor(int APIversion, EventHandler handler, Stream s)
            : base(APIversion, handler)
        {
            BinaryReader r = new BinaryReader(s);
            this.color = Color.FromArgb(r.ReadInt32());
        }
        public SwatchColor(int APIversion, EventHandler handler, Color color) : base(APIversion, handler) { this.color = color; }
        public void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(this.color.ToArgb()); }

        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        public bool Equals(SwatchColor other) { return other.Equals(this.color); }

        public Color Color { get { return this.color; } set { if (!color.Equals(value)) { this.color = value; OnElementChanged(); } } }
        public string Value { get { { return this.color.IsKnownColor ? this.color.ToKnownColor().ToString() : this.color.Name; } } }
    }

    public class SwatchColorList : DependentList<SwatchColor>
    {
        public SwatchColorList(EventHandler handler) : base(handler) { }
        public SwatchColorList(EventHandler handler, Stream s) : base(handler) { Parse(s); }

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            byte count = r.ReadByte();
            for (int i = 0; i < count; i++)
                base.Add(new SwatchColor(1, handler, s));
        }

        public override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write((byte)base.Count);
            foreach (var color in this)
                color.UnParse(s);
        }

        protected override SwatchColor CreateElement(Stream s) { return new SwatchColor(1, handler, Color.Black); }
        protected override void WriteElement(Stream s, SwatchColor element) { element.UnParse(s); }
        #endregion
    }
}
