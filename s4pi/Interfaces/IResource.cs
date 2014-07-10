using System;
using System.Collections.Generic;
using System.IO;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Minimal resource interface
    /// </summary>
    public interface IResource : IApiVersion, IContentFields
    {
        /// <summary>
        /// The resource content as a <see cref="System.IO.Stream"/>.
        /// </summary>
        Stream Stream { get; }
        /// <summary>
        /// The resource content as a <see cref="byte"/> array
        /// </summary>
        byte[] AsBytes { get; }

        /// <summary>
        /// Raised if the resource is changed
        /// </summary>
        event EventHandler ResourceChanged;
    }
}
