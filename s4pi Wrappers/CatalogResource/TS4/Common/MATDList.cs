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
        private bool hasLabel;
        const int recommendedApiVersion = 1;
        #endregion

        public MATDEntry(int APIversion, EventHandler handler) : base(APIversion, handler) { }
        public MATDEntry(int APIversion, EventHandler handler, Stream s, bool hasLabel = true) : base(APIversion, handler) { this.hasLabel = hasLabel; Parse(s, hasLabel); }


        #region Data I/O
        private void Parse(Stream s, bool hasLabel = true)
        {
            BinaryReader r = new BinaryReader(s);
            if (hasLabel) this.matdLabel = r.ReadByte();
            this.matdTGI = new TGIBlock(recommendedApiVersion, handler, "ITG", s);
        }

        protected internal void UnParse(Stream s) { this.UnParse(s, this.hasLabel); }

        protected internal void UnParse(Stream s, bool hasLabel)
        {
            BinaryWriter w = new BinaryWriter(s);
            if (hasLabel) w.Write(this.matdLabel);
            this.matdTGI.UnParse(s);
        }
        #endregion

        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { var res = GetContentFields(requestedApiVersion, this.GetType()); if (!this.hasLabel) { res.Remove("MATDLabel"); } return res; } }
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
        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class MATDList : DependentList<MATDEntry>
    {
        private bool hasLabel;
        #region Constructors
        public MATDList(EventHandler handler, bool hasLabel = true) : base(handler) { this.hasLabel = hasLabel; }
        public MATDList(EventHandler handler, Stream s, bool hasLabel = true) : base(handler) { this.hasLabel = hasLabel; Parse(s, hasLabel); }
        #endregion


        #region Data I/O
        protected override void Parse(Stream s)
        {
            this.Parse(s, true);
        }

        protected void Parse(Stream s, bool hasLabel)
        {
            BinaryReader r = new BinaryReader(s);
            var count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                base.Add(new MATDEntry(1, handler, s, hasLabel));
            }
        }

        public override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(base.Count);
            foreach (var matd in this)
            {
                matd.UnParse(s, this.hasLabel);
            }
        }

        protected override MATDEntry CreateElement(Stream s) { return new MATDEntry(1, handler, s, hasLabel); }
        protected override void WriteElement(Stream s, MATDEntry element) { element.UnParse(s); }
        #endregion
    }
}
