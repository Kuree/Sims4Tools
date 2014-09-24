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
using System.IO;

namespace s4pi.Interfaces
{
    /// <summary>
    /// An index entry within a package
    /// </summary>
    public interface IResourceIndexEntry : IApiVersion, IContentFields, IResourceKey, IEquatable<IResourceIndexEntry>
    {
        /// <summary>
        /// If the resource was read from a package, the location in the package the resource was read from
        /// </summary>
        UInt32 Chunkoffset { get; set; }
        /// <summary>
        /// The number of bytes the resource uses within the package
        /// </summary>
        UInt32 Filesize { get; set; }
        /// <summary>
        /// The number of bytes the resource uses in memory
        /// </summary>
        UInt32 Memsize { get; set; }
        /// <summary>
        /// 0xFFFF if Filesize != Memsize, else 0x0000
        /// </summary>
        UInt16 Compressed { get; set; }
        /// <summary>
        /// Always 0x0001
        /// </summary>
        UInt16 Unknown2 { get; set; }

        /// <summary>
        /// A <see cref="MemoryStream"/> covering the index entry bytes
        /// </summary>
        Stream Stream { get; }

        /// <summary>
        /// True if the index entry has been deleted from the package index
        /// </summary>
        bool IsDeleted { get; set; }
    }
}
