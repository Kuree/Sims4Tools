using System;
using System.Collections.Generic;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Used by <see cref="AResourceHandler"/>, which is used by <c>WrapperDealer</c>
    /// to identify &quot;interesting&quot; classes within assemblies
    /// </summary>
    interface IResourceHandler : IDictionary<Type, List<string>>
    {
    }
}
