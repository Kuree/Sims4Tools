using System;
using System.Collections.Generic;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Used by WrapperDealer to identify "interesting" classes and assemblies.
    /// The class maps implementers of AResource to string representations of ResourceType.
    /// Hence each "wrapper" assembly can contain multiple wrapper types (Type key) each of which
    /// supports one or more ResourceTypes (List&lt;string&gt; value).  The single
    /// AResourceHandler implementation summarises what the assembly provides.
    /// </summary>
    public abstract class AResourceHandler : Dictionary<Type, List<string>>, IResourceHandler
    {
        /// <summary>
        /// Create the content of the Dictionary
        /// </summary>
        public AResourceHandler() { }
    }
}
