/***************************************************************************
 *  Copyright (C) 2009, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Peter L Jones (pljones@users.sf.net)                                   *
 *  Keyi Zhang                                                             *
 *  Buzzler                                                                *
 *  Cmar                                                                   *
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
using System.IO;

using s4pi.Interfaces;
using s4pi.Resource.Commons.CatalogTags;

namespace CASPartResource.Handlers
{
    public class Flag : AHandlerElement, IEquatable<Flag>
    {
        private CompoundTag compoundTag;

        public Flag(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler)
        {
            var reader = new BinaryReader(s);
            var category = CatalogTagRegistry.FetchCategory(reader.ReadUInt16());
            var value = CatalogTagRegistry.FetchTag(reader.ReadUInt32());
            this.compoundTag = new CompoundTag { Category = category, Value = value };
        }

        public Flag(int apiVersion, EventHandler handler, ushort flagCategory, uint flagValue)
            : base(apiVersion, handler)
        {
            var category = CatalogTagRegistry.FetchCategory(flagCategory);
            var value = CatalogTagRegistry.FetchTag(flagValue);
            this.compoundTag = new CompoundTag { Category = category, Value = value };
        }

        public Flag(int apiVersion, EventHandler handler)
            : base(apiVersion, handler)
        {
            var category = CatalogTagRegistry.FetchCategory(0);
            var value = CatalogTagRegistry.FetchTag(0);
            this.compoundTag = new CompoundTag { Category = category, Value = value };
        }

        public void UnParse(Stream stream)
        {
            var w = new BinaryWriter(stream);
            w.Write(this.compoundTag.Category.ToUInt16());
            w.Write(this.compoundTag.Value.ToUInt32());
        }

        #region AHandlerElement Members

        public void WriteUInt16(Stream stream)
        {
            var w = new BinaryWriter(stream);
            w.Write(this.compoundTag.Category.ToUInt16());
            w.Write(this.compoundTag.Value.ToUInt16());
        }

        public override int RecommendedApiVersion
        {
            get { return CASPartResource.recommendedApiVersion; }
        }

        public override List<string> ContentFields
        {
            get { return GetContentFields(this.requestedApiVersion, this.GetType()); }
        }

        #endregion

        public bool Equals(Flag other)
        {
            if (other != null)
            {
                return this.compoundTag == other.compoundTag;
            }
            return false;
        }

        [ElementPriority(0)]
        public CompoundTag CompoundTag
        {
            get { return this.compoundTag; }
            set
            {
                if (value != this.compoundTag)
                {
                    this.OnElementChanged();
                    this.compoundTag = value;
                }
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }
    }
}
