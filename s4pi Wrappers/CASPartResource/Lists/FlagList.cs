/***************************************************************************
 *  Copyright (C) 2016 by Sims 4 Tools Development Team                    *
 *  Credits: Peter Jones, Keyi Zhang, Buzzler, Cmar                        *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using System;
using System.IO;

using CASPartResource.Handlers;

using s4pi.Interfaces;

namespace CASPartResource.Lists
{
    public class FlagList : DependentList<Flag>
    {
        public FlagList(EventHandler handler)
            : base(handler)
        {
        }

        public FlagList(EventHandler handler, Stream s)
            : base(handler, s)
        {
        }

        #region Data I/O

        protected override void Parse(Stream s)
        {
            var r = new BinaryReader(s);
            var count = r.ReadUInt32();
            for (var i = 0; i < count; i++)
            {
                this.Add(new Flag(CASPartResource.recommendedApiVersion, this.handler, s));
            }
        }

        public override void UnParse(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write((uint)this.Count);
            foreach (Flag flag in this)
            {
                flag.UnParse(stream);
            }
        }

        #endregion

        public void WriteUInt16Flags(Stream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write((uint)this.Count);
            foreach (Flag flag in this)
            {
                flag.WriteUInt16(stream);
            }
        }

        protected override Flag CreateElement(Stream s)
        {
            return new Flag(CASPartResource.recommendedApiVersion, this.handler, s);
        }

        protected override void WriteElement(Stream s, Flag element)
        {
            element.UnParse(s);
        }

        /// <summary>
        /// Creates a new <see cref="FlagList"/> based on 16Bit tag values.
        /// </summary>
        public static FlagList CreateWithUInt16Flags(EventHandler onResourceChanged, Stream stream, int apiVersion)
        {
            BinaryReader reader = new BinaryReader(stream);

            uint flagCount = reader.ReadUInt32();
            FlagList list = new FlagList(onResourceChanged);
            for (int i = 0; i < flagCount; i++)
            {
                ushort cat = reader.ReadUInt16();
                uint val = reader.ReadUInt16();
                list.Add(new Flag(apiVersion, onResourceChanged, cat, val));
            }
            return list;
        }
    }
}