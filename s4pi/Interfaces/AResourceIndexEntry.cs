/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
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
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;

namespace s4pi.Interfaces
{
    /// <summary>
    /// An abstract class, descended from <see cref="AResourceKey"/>, providing an abstract implemention of <see cref="IResourceIndexEntry"/>,
    /// representing an index entry within a package.
    /// </summary>
    public abstract class AResourceIndexEntry : AResourceKey, IResourceIndexEntry
    {
        /// <summary>
        /// Initialize a new instance with the default API version and no change <see cref="EventHandler"/>.
        /// </summary>
        public AResourceIndexEntry() : base(0, null) { handler += OnResourceIndexEntryChanged; }

        private void OnResourceIndexEntryChanged(object sender, EventArgs e) { if (ResourceIndexEntryChanged != null) ResourceIndexEntryChanged(sender, e); }

        #region AApiVersionedFields
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        #region IResourceIndexEntry Members
        /// <summary>
        /// If the resource was read from a package, the location in the package the resource was read from
        /// </summary>
        [ElementPriority(5)]
        public abstract uint Chunkoffset { get; set; }
        /// <summary>
        /// The number of bytes the resource uses within the package
        /// </summary>
        [ElementPriority(6)]
        public abstract uint Filesize { get; set; }
        /// <summary>
        /// The number of bytes the resource uses in memory
        /// </summary>
        [ElementPriority(7)]
        public abstract uint Memsize { get; set; }
        /// <summary>
        /// 0xFFFF if Filesize != Memsize, else 0x0000
        /// </summary>
        [ElementPriority(8)]
        public abstract ushort Compressed { get; set; }
        /// <summary>
        /// Always 0x0001
        /// </summary>
        [ElementPriority(9)]
        public abstract ushort Unknown2 { get; set; }

        /// <summary>
        /// A MemoryStream covering the index entry bytes
        /// </summary>
        public abstract System.IO.Stream Stream { get; }

        /// <summary>
        /// True if the index entry has been deleted from the package index
        /// </summary>
        public abstract bool IsDeleted { get; set; }

        /// <summary>
        /// Raised when the AResourceIndexEntry changes
        /// </summary>
        public EventHandler ResourceIndexEntryChanged;
        #endregion

        #region IEquatable<IResourceIndexEntry>
        /// <summary>
        /// Indicates whether the current <see cref="IResourceIndexEntry"/> instance is equal to another <see cref="IResourceIndexEntry"/> instance.
        /// </summary>
        /// <param name="other">An <see cref="IResourceIndexEntry"/> instance to compare with this instance.</param>
        /// <returns>true if the current instance is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        public abstract bool Equals(IResourceIndexEntry other);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="AResourceIndexEntry"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="AResourceIndexEntry"/>.</param>
        /// <returns>true if the specified <see cref="System.Object"/> is equal to the current <see cref="AResourceIndexEntry"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj as AResourceIndexEntry != null ? this.Equals(obj as AResourceIndexEntry) : false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override abstract int GetHashCode();
        #endregion
    }
}
