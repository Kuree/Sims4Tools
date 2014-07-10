using System;
using System.Collections.Generic;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Standardised interface to API objects (hiding the reflection)
    /// </summary>
    public interface IContentFields
    {
#if UNDEF
        /// <summary>
        /// The list of method names available on this object
        /// </summary>
        List<string> Methods { get; }

        /// <summary>
        /// Invoke a method on this object by name
        /// </summary>
        /// <param name="method">The method name</param>
        /// <param name="parms">The array of TypedValue parameters</param>
        /// <returns>The TypedValue result of invoking the method (or null if the method is void)</returns>
        TypedValue Invoke(string method, params TypedValue[] parms);
#endif
        
        /// <summary>
        /// A <c>List&lt;string&gt;</c> of available field names on object
        /// </summary>
        List<string> ContentFields { get; }

        /// <summary>
        /// A <see cref="TypedValue"/> on this object
        /// </summary>
        /// <param name="index">The <see cref="string"/> representing the name of the field
        /// (i.e. one of the values from <see cref="ContentFields"/>)</param>
        /// <returns>The <see cref="TypedValue"/> of field <paramref name="index"/> on this API object.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown index name is requested</exception>
        TypedValue this[string index] { get; set; }
    }
}
