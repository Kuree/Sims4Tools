/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace CatalogResource
{
    public class SwatchColor : AHandlerElement, IEquatable<SwatchColor>
    {
        private Color color;
        const int recommendedApiVersion = 1;
        public SwatchColor(int APIversion, EventHandler handler) : base(APIversion, handler) { }
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
