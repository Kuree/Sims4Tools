using System;
using System.Collections.Generic;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Base class for versioning support.  Not directly used by the API.
    /// </summary>
    public class VersionAttribute : Attribute
    {
        Int32 version;
        /// <summary>
        /// Version number attribute (base)
        /// </summary>
        /// <param name="Version">Version number</param>
        public VersionAttribute(Int32 Version) { version = Version; }
        /// <summary>
        /// Version number
        /// </summary>
        public Int32 Version { get { return version; } set { version = value; } }
    }

    /// <summary>
    /// Specify the Minumum version from which a field or method is supported
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false,  Inherited = true)]
    public class MinimumVersionAttribute : VersionAttribute
    {
        /// <summary>
        /// Specify the Minumum version from which a field or method is supported
        /// </summary>
        /// <param name="Version">Version number</param>
        public MinimumVersionAttribute(Int32 Version) : base(Version) { }
    }

    /// <summary>
    /// Specify the Maximum version up to which a field or method is supported
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MaximumVersionAttribute : VersionAttribute
    {
        /// <summary>
        /// Specify the Maximum version up to which a field or method is supported
        /// </summary>
        /// <param name="Version">Version number</param>
        public MaximumVersionAttribute(Int32 Version) : base(Version) { }
    }
}
