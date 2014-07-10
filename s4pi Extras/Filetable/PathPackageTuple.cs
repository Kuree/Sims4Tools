using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.Filetable
{
    /// <summary>
    /// Contains the path of a <see cref="IPackage"/> and the <see cref="IPackage"/> reference itself.
    /// </summary>
    public class PathPackageTuple : IEquatable<PathPackageTuple>
    {
        /// <summary>
        /// The path containing this <see cref="IPackage"/>.
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// The <see cref="IPackage"/> stored in <see cref="Path"/>.
        /// </summary>
        public IPackage Package { get; private set; }

        /// <summary>
        /// Store the full pathname of <paramref name="path"/> and open
        /// the referenced <see cref="IPackage"/>, read-only by default.
        /// </summary>
        /// <param name="path">The path of the <see cref="IPackage"/> to open.</param>
        /// <param name="readwrite">Set to true to open the package read-write; otherwise set to false for read-only (default is false).</param>
        /// <remarks>See <see cref="s4pi.Package.Package.OpenPackage(int, string, bool)"/> for potential exceptions.</remarks>
        public PathPackageTuple(string path, bool readwrite = false) { Path = System.IO.Path.GetFullPath(path); Package = s4pi.Package.Package.OpenPackage(0, Path, readwrite); }

        /// <summary>
        /// Add a resource, identified by <paramref name="rk"/> with the content from <paramref name="stream"/>
        /// the package referenced by this <see cref="PathPackageTuple"/>.  The new <see cref="SpecificResource"/>
        /// value is returned.
        /// </summary>
        /// <param name="rk">The <see cref="IResourceKey"/> to use to identify the new resource.</param>
        /// <param name="stream">A <see cref="Stream"/> of data to use to populate the new resource.</param>
        /// <returns>The <see cref="SpecificResource"/> representing the newly added <see cref="IResourceIndexEntry"/>.</returns>
        public SpecificResource AddResource(IResourceKey rk, Stream stream = null)
        {
            IResourceIndexEntry rie = Package.AddResource(rk, stream, true);
            if (rie == null) return null;
            return new SpecificResource(this, rie);
        }

        /// <summary>
        /// Searches the entire <see cref="IPackage"/> for the first <see cref="IResourceIndexEntry"/>
        /// that matches the conditions defined by <paramref name="match"/>.
        /// </summary>
        /// <param name="match">The matching critera.</param>
        /// <returns>The first matching resource.</returns>
        public SpecificResource Find(Predicate<IResourceIndexEntry> match)
        {
            IResourceIndexEntry rie = Package.Find(match);
            return rie == null ? null : new SpecificResource(this, rie);
        }

        /// <summary>
        /// Searches the entire <see cref="IPackage"/> for all <see cref="IResourceIndexEntry"/>
        /// that match the conditions defined by <paramref name="match"/>.
        /// </summary>
        /// <param name="match">The matching critera.</param>
        /// <returns>All matching resources.</returns>
        public IEnumerable<SpecificResource> FindAll(Predicate<IResourceIndexEntry> match)
        {
            foreach (var rie in Package.FindAll(match))
                yield return new SpecificResource(this, rie);
        }

        /// <summary>
        /// Determine whether <paramref name="other"/> is the same package as this.
        /// </summary>
        /// <param name="other">The <see cref="PathPackageTuple"/> under inspection.</param>
        /// <returns>True if this and <paramref name="other"/> have the same path.</returns>
        public bool Equals(PathPackageTuple other) { return Path.Equals(other.Path); }
    }
}
