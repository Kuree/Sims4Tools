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
using System.IO;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Representation of a Sims 3 Package
    /// </summary>
    public interface IPackage : IApiVersion, IContentFields, IDisposable
    {
        #region Whole package
        /// <summary>
        /// Tell the package to save itself to wherever it believes it came from
        /// </summary>
        void SavePackage();
        /// <summary>
        /// Tell the package to save itself to the <see cref="System.IO.Stream"/> <paramref name="s"/>
        /// </summary>
        /// <param name="s">A <see cref="System.IO.Stream"/> to which the package should be saved</param>
        void SaveAs(Stream s);
        /// <summary>
        /// Tell the package to save itself to a file with the name in <paramref name="path"/>
        /// </summary>
        /// <param name="path">A fully-qualified file name</param>
        void SaveAs(string path);
        #endregion

        #region Package header
        /// <summary>
        /// Package header: "DBPF" bytes
        /// </summary>
        [ElementPriority(1)]
        byte[] Magic { get; }
        /// <summary>
        /// Package header: 0x00000002
        /// </summary>
        [ElementPriority(2)]
        Int32 Major { get; }
        /// <summary>
        /// Package header: 0x00000000
        /// </summary>
        [ElementPriority(3)]
        Int32 Minor { get; }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(4)]
        byte[] Unknown1 { get; }
        /// <summary>
        /// Package header: number of entries in the package index
        /// </summary>
        [ElementPriority(5)]
        Int32 Indexcount { get; }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(6)]
        byte[] Unknown2 { get; }
        /// <summary>
        /// Package header: index size on disk in bytes
        /// </summary>
        [ElementPriority(7)]
        Int32 Indexsize { get; }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(8)]
        byte[] Unknown3 { get; }
        /// <summary>
        /// Package header: always 3?
        /// </summary>
        [ElementPriority(9)]
        Int32 Indexversion { get; }
        /// <summary>
        /// Package header: index position in file
        /// </summary>
        [ElementPriority(10)]
        Int32 Indexposition { get; }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(11)]
        byte[] Unknown4 { get; }

        /// <summary>
        /// A <see cref="MemoryStream"/> covering the package header bytes
        /// </summary>
        [ElementPriority(12)]
        Stream HeaderStream { get; }
        #endregion

        #region Package index
        /// <summary>
        /// Package index: raised when the result of a previous call to GetResourceList becomes invalid 
        /// </summary>
        event EventHandler ResourceIndexInvalidated;

        /// <summary>
        /// Package index: the index format in use
        /// </summary>
        [ElementPriority(13)]
        UInt32 Indextype { get; }

        /// <summary>
        /// Package index: the index as a <see cref="IResourceIndexEntry"/> list
        /// </summary>
        [ElementPriority(14)]
        List<IResourceIndexEntry> GetResourceList { get; }

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for the first <see cref="IResourceIndexEntry"/> that matches the conditions defined by
        /// <paramref name="flags"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="flags">True bits enable matching against numerically equivalent <paramref name="values"/> entry.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>The first matching <see cref="IResourceIndexEntry"/>, if any; otherwise null.</returns>
        [Obsolete("Please use Find(Predicate<IResourceIndexEntry> Match)")]
        IResourceIndexEntry Find(uint flags, IResourceIndexEntry values);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for the first <see cref="IResourceIndexEntry"/> that matches the conditions defined by
        /// <paramref name="names"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="names">Names of <see cref="IResourceIndexEntry"/> fields to compare.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>The first matching <see cref="IResourceIndexEntry"/>, if any; otherwise null.</returns>
        [Obsolete("Please use Find(Predicate<IResourceIndexEntry> Match)")]
        IResourceIndexEntry Find(string[] names, TypedValue[] values);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for the first <see cref="IResourceIndexEntry"/> that matches the conditions defined by
        /// the <c>Predicate&lt;IResourceIndexEntry&gt;</c> <paramref name="Match"/>.
        /// </summary>
        /// <param name="Match"><c>Predicate&lt;IResourceIndexEntry&gt;</c> defining matching conditions.</param>
        /// <returns>The first matching <see cref="IResourceIndexEntry"/>, if any; otherwise null.</returns>
        /// <remarks>Note that entries marked as deleted will not be returned.</remarks>
        IResourceIndexEntry Find(Predicate<IResourceIndexEntry> Match);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for all <see cref="IResourceIndexEntry"/>s that matches the conditions defined by
        /// <paramref name="flags"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="flags">True bits enable matching against numerically equivalent <paramref name="values"/> entry.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>An <c>IList&lt;IResourceIndexEntry&gt;</c> of zero or more matches.</returns>
        [Obsolete("Please use FindAll(Predicate<IResourceIndexEntry> Match)")]
        List<IResourceIndexEntry> FindAll(uint flags, IResourceIndexEntry values);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for all <see cref="IResourceIndexEntry"/>s that matches the conditions defined by
        /// <paramref name="names"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="names">Names of <see cref="IResourceIndexEntry"/> fields to compare.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>An <c>IList&lt;IResourceIndexEntry&gt;</c> of zero or more matches.</returns>
        [Obsolete("Please use FindAll(Predicate<IResourceIndexEntry> Match)")]
        List<IResourceIndexEntry> FindAll(string[] names, TypedValue[] values);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for all <see cref="IResourceIndexEntry"/>s that matches the conditions defined by
        /// the <c>Predicate&lt;IResourceIndexEntry&gt;</c> <paramref name="Match"/>.
        /// </summary>
        /// <param name="Match"><c>Predicate&lt;IResourceIndexEntry&gt;</c> defining matching conditions.</param>
        /// <returns>Zero or more matches.</returns>
        /// <remarks>Note that entries marked as deleted will not be returned.</remarks>
        List<IResourceIndexEntry> FindAll(Predicate<IResourceIndexEntry> Match);
        #endregion

        #region Package content
        /// <summary>
        /// Add a resource to the <see cref="IPackage"/>.
        /// </summary>
        /// <param name="rk">The resource&apos;s <see cref="IResourceKey"/></param>
        /// <param name="stream">The <see cref="System.IO.Stream"/> that contains the resource&apos;s data</param>
        /// <param name="rejectDups">If true, fail if the <see cref="IResourceKey"/> already exists</param>
        /// <returns>Null if rejectDups and the <see cref="IResourceKey"/> exists; else the new <see cref="IResourceIndexEntry"/></returns>
        IResourceIndexEntry AddResource(IResourceKey rk, Stream stream, bool rejectDups);
        /// <summary>
        /// Tell the <see cref="IPackage"/> to replace the data for the resource indexed by <see cref="IResourceIndexEntry"/> <paramref name="rc"/>
        /// with the data from the <see cref="IResource"/> <paramref name="res"/>.
        /// </summary>
        /// <param name="rc">Target <see cref="IResourceIndexEntry"/>.</param>
        /// <param name="res">Source <see cref="IResource"/>.</param>
        void ReplaceResource(IResourceIndexEntry rc, IResource res);
        /// <summary>
        /// Tell the package to delete the resource indexed by <see cref="IResourceIndexEntry"/> <paramref name="rc"/>.
        /// </summary>
        /// <param name="rc">Target <see cref="IResourceIndexEntry"/>.</param>
        void DeleteResource(IResourceIndexEntry rc);
        #endregion

    }
}
