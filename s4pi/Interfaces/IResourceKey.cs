using System;
using System.IO;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Exposes a standard set of properties to identify a resource
    /// </summary>
    public interface IResourceKey : System.Collections.Generic.IEqualityComparer<IResourceKey>, IEquatable<IResourceKey>, IComparable<IResourceKey>
    {
        /// <summary>
        /// The "type" of the resource
        /// </summary>
        UInt32 ResourceType { get; set; }
        /// <summary>
        /// The "group" the resource is part of
        /// </summary>
        UInt32 ResourceGroup { get; set; }
        /// <summary>
        /// The "instance" number of the resource
        /// </summary>
        UInt64 Instance { get; set; }
    }
}
