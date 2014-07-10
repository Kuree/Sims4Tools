using System;
using System.Collections.Generic;
using System.IO;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Abstract definition of a package
    /// </summary>
    public abstract class APackage : AApiVersionedFields, IPackage
    {
        #region AApiVersionedFields
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        #region IPackage Members

        #region Whole package
        /// <summary>
        /// Tell the package to save itself to wherever it believes it came from
        /// </summary>
        public abstract void SavePackage();
        /// <summary>
        /// Tell the package to save itself to the stream <paramref name="s"/>
        /// </summary>
        /// <param name="s">A stream to which the package should be saved</param>
        public abstract void SaveAs(Stream s);
        /// <summary>
        /// Tell the package to save itself to a file with the name in <paramref name="path"/>
        /// </summary>
        /// <param name="path">A fully-qualified file name</param>
        public abstract void SaveAs(string path);
        #endregion

        #region Package header
        /// <summary>
        /// Package header: "DBPF" bytes
        /// </summary>
        [ElementPriority(1)]
        public abstract byte[] Magic { get; }
        /// <summary>
        /// Package header: 0x00000002
        /// </summary>
        [ElementPriority(2)]
        public abstract int Major { get; }
        /// <summary>
        /// Package header: 0x00000000
        /// </summary>
        [ElementPriority(3)]
        public abstract int Minor { get; }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(4)]
        public abstract byte[] Unknown1 { get; }
        /// <summary>
        /// Package header: number of entries in the package index
        /// </summary>
        [ElementPriority(5)]
        public abstract int Indexcount { get; }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(6)]
        public abstract byte[] Unknown2 { get; }
        /// <summary>
        /// Package header: index size on disk in bytes
        /// </summary>
        [ElementPriority(7)]
        public abstract int Indexsize { get; }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(8)]
        public abstract byte[] Unknown3 { get; }
        /// <summary>
        /// Package header: always 3?
        /// </summary>
        [ElementPriority(9)]
        public abstract int Indexversion { get; }
        /// <summary>
        /// Package header: index position in file
        /// </summary>
        [ElementPriority(10)]
        public abstract int Indexposition { get; }
        /// <summary>
        /// Package header: unused
        /// </summary>
        [ElementPriority(11)]
        public abstract byte[] Unknown4 { get; }

        /// <summary>
        /// A MemoryStream covering the package header bytes
        /// </summary>
        [ElementPriority(12)]
        public abstract Stream HeaderStream { get; }
        #endregion

        #region Package index
        /// <summary>
        /// Package index: raised when the result of a previous call to GetResourceList becomes invalid 
        /// </summary>
        public event EventHandler ResourceIndexInvalidated;

        /// <summary>
        /// Package index: the index format in use
        /// </summary>
        [ElementPriority(13)]
        public abstract uint Indextype { get; }

        /// <summary>
        /// Package index: the index
        /// </summary>
        [ElementPriority(14)]
        public abstract List<IResourceIndexEntry> GetResourceList { get; }

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for the first <see cref="IResourceIndexEntry"/> that matches the conditions defined by
        /// <paramref name="flags"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="flags">True bits enable matching against numerically equivalent <paramref name="values"/> entry.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>The first matching <see cref="IResourceIndexEntry"/>, if any; otherwise null.</returns>
        [Obsolete("Please use Find(Predicate<IResourceIndexEntry> Match)")]
        public abstract IResourceIndexEntry Find(uint flags, IResourceIndexEntry values);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for the first <see cref="IResourceIndexEntry"/> that matches the conditions defined by
        /// <paramref name="names"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="names">Names of <see cref="IResourceIndexEntry"/> fields to compare.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>The first matching <see cref="IResourceIndexEntry"/>, if any; otherwise null.</returns>
        [Obsolete("Please use Find(Predicate<IResourceIndexEntry> Match)")]
        public abstract IResourceIndexEntry Find(string[] names, TypedValue[] values);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for the first <see cref="IResourceIndexEntry"/> that matches the conditions defined by
        /// the <c>Predicate&lt;IResourceIndexEntry&gt;</c> <paramref name="Match"/>.
        /// </summary>
        /// <param name="Match"><c>Predicate&lt;IResourceIndexEntry&gt;</c> defining matching conditions.</param>
        /// <returns>The first matching <see cref="IResourceIndexEntry"/>, if any; otherwise null.</returns>
        /// <remarks>Note that entries marked as deleted will not be returned.</remarks>
        public abstract IResourceIndexEntry Find(Predicate<IResourceIndexEntry> Match);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for all <see cref="IResourceIndexEntry"/>s that matches the conditions defined by
        /// <paramref name="flags"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="flags">True bits enable matching against numerically equivalent <paramref name="values"/> entry.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>An <c>IList&lt;IResourceIndexEntry&gt;</c> of zero or more matches.</returns>
        [Obsolete("Please use FindAll(Predicate<IResourceIndexEntry> Match)")]
        public abstract List<IResourceIndexEntry> FindAll(uint flags, IResourceIndexEntry values);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for all <see cref="IResourceIndexEntry"/>s that matches the conditions defined by
        /// <paramref name="names"/> and <paramref name="values"/>.
        /// </summary>
        /// <param name="names">Names of <see cref="IResourceIndexEntry"/> fields to compare.</param>
        /// <param name="values">Field values to compare against.</param>
        /// <returns>An <c>IList&lt;IResourceIndexEntry&gt;</c> of zero or more matches.</returns>
        [Obsolete("Please use FindAll(Predicate<IResourceIndexEntry> Match)")]
        public abstract List<IResourceIndexEntry> FindAll(string[] names, TypedValue[] values);

        /// <summary>
        /// Searches the entire <see cref="IPackage"/>
        /// for all <see cref="IResourceIndexEntry"/>s that matches the conditions defined by
        /// the <c>Predicate&lt;IResourceIndexEntry&gt;</c> <paramref name="Match"/>.
        /// </summary>
        /// <param name="Match"><c>Predicate&lt;IResourceIndexEntry&gt;</c> defining matching conditions.</param>
        /// <returns>Zero or more matches.</returns>
        /// <remarks>Note that entries marked as deleted will not be returned.</remarks>
        public abstract List<IResourceIndexEntry> FindAll(Predicate<IResourceIndexEntry> Match);
        #endregion

        #region Package content
        /// <summary>
        /// Add a resource to the package
        /// </summary>
        /// <param name="rk">The resource key</param>
        /// <param name="stream">The stream that contains the resource data</param>
        /// <param name="rejectDups">If true, fail if the resource key already exists</param>
        /// <returns>Null if rejectDups and the resource key exists; else the new IResourceIndexEntry</returns>
        public abstract IResourceIndexEntry AddResource(IResourceKey rk, Stream stream, bool rejectDups);
        /// <summary>
        /// Tell the package to replace the data for the resource indexed by <paramref name="rc"/>
        /// with the data from the resource <paramref name="res"/>
        /// </summary>
        /// <param name="rc">Target resource index</param>
        /// <param name="res">Source resource</param>
        public abstract void ReplaceResource(IResourceIndexEntry rc, IResource res);
        /// <summary>
        /// Tell the package to delete the resource indexed by <paramref name="rc"/>
        /// </summary>
        /// <param name="rc">Target resource index</param>
        public abstract void DeleteResource(IResourceIndexEntry rc);
        #endregion

        #endregion

        // Static so cannot be defined on the interface

        /// <summary>
        /// Initialise a new, empty package and return the IPackage reference
        /// </summary>
        /// <param name="APIversion">(unused)</param>
        /// <returns>IPackage reference to an empty package</returns>
        public static IPackage NewPackage(int APIversion) { throw new NotImplementedException(); }
        /// <summary>
        /// Initialise a new, empty package and return the IPackage reference
        /// </summary>
        /// <param name="APIversion">(unused)</param>
        /// <param name="major">Major version for the DBPF package.</param>
        /// <returns>IPackage reference to an empty package</returns>
        public static IPackage NewPackage(int APIversion, int major) { throw new NotImplementedException(); }
        /// <summary>
        /// Open an existing package by filename, read only
        /// </summary>
        /// <param name="APIversion">(unused)</param>
        /// <param name="packagePath">Fully qualified filename of the package</param>
        /// <returns>IPackage reference to an existing package on disk</returns>
        public static IPackage OpenPackage(int APIversion, string packagePath) { throw new NotImplementedException(); }
        /// <summary>
        /// Open an existing package by filename, optionally readwrite
        /// </summary>
        /// <param name="APIversion">(unused)</param>
        /// <param name="packagePath">Fully qualified filename of the package</param>
        /// <param name="readwrite">True to open for update</param>
        /// <returns>IPackage reference to an existing package on disk</returns>
        public static IPackage OpenPackage(int APIversion, string packagePath, bool readwrite) { throw new NotImplementedException(); }
        /// <summary>
        /// Releases any internal references associated with the given package
        /// </summary>
        /// <param name="APIversion">(unused)</param>
        /// <param name="pkg">IPackage reference to close</param>
        public static void ClosePackage(int APIversion, IPackage pkg) { throw new NotImplementedException(); }

        // Required by API, not user tools

        /// <summary>
        /// Required internally by s3pi - <b>not</b> for use in user tools.
        /// Please use <c>WrapperDealer.GetResource(int, IPackage, IResourceIndexEntry)</c> instead.
        /// </summary>
        /// <param name="rie">IResourceIndexEntry of resource</param>
        /// <returns>The resource data (uncompressed, if necessary)</returns>
        /// <remarks>Used by WrapperDealer to get the data for a resource.</remarks>
        public abstract Stream GetResource(IResourceIndexEntry rie);

        /// <summary>
        /// Used to indicate a resource index returned by GetResourceList is no longer valid (as a whole)
        /// </summary>
        /// <param name="sender">Object causing the list to become invalid</param>
        /// <param name="e">(not used)</param>
        protected virtual void OnResourceIndexInvalidated(object sender, EventArgs e) { if (ResourceIndexInvalidated != null) ResourceIndexInvalidated(sender, e); }
    }
}
