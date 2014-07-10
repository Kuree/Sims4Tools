using System;
using System.Collections.Generic;
using System.IO;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Defines the interface exposed by an RCOL block.
    /// </summary>
    public interface IRCOLBlock : IResource
    {
        /// <summary>
        /// The four byte tag for the RCOL block, may be null if none present
        /// </summary>
        [ElementPriority(2)]
        string Tag { get; }

        /// <summary>
        /// The ResourceType for the RCOL block, used to determine which specific RCOL handlers are available
        /// </summary>
        [ElementPriority(3)]
        uint ResourceType { get; }

        /// <summary>
        /// Writes the content of the RCOL block to a stream
        /// </summary>
        /// <returns>A stream containing the current content of the RCOL block</returns>
        Stream UnParse();
    }
}
