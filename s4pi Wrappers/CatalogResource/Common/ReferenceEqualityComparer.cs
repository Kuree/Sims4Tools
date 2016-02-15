using System;
using System.Collections.Generic;

namespace CatalogResource.Common
{
    /// <summary>
    /// Implementation of <see cref="EqualityComparer{T}"/> to compare objects by reference.
    /// <remarks>Code from http://stackoverflow.com/a/11308879. All credits to Alex Burtsev.</remarks>
    /// </summary>
    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }
}
