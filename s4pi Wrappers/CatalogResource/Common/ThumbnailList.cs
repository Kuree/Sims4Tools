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

namespace CatalogResource
{
    public class ThumbnailEntry : AHandlerElement, IEquatable<ThumbnailEntry>
    {
        #region Attributes
        private byte imgGroupLabel;
        private Tuple<TGIBlock, TGIBlock, TGIBlock> thumGroupTGI;
        const int recommendedApiVersion = 1;
        #endregion

        public ThumbnailEntry(int apiVersion, EventHandler handler) : base(apiVersion, handler) { this.UnParse(new MemoryStream()); }
        public ThumbnailEntry(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }


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
            if (this.thumGroupTGI == null) this.thumGroupTGI = new Tuple<TGIBlock, TGIBlock, TGIBlock>(new TGIBlock(recommendedApiVersion, handler), new TGIBlock(recommendedApiVersion, handler), new TGIBlock(recommendedApiVersion, handler));
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
