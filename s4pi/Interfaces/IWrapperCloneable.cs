using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Interface for self-clone wrapper
    /// </summary>
    /// <typeparam name="T">Wrapper Type</typeparam>
    public interface IWrapperCloneable<T> where T : AResource
    {
        /// <summary>
        /// Deep clone the wrapper object with built-in renumbering function
        /// </summary>
        /// <param name="hashsalt">Hash salt to renumber the fields</param>
        /// <param name="renumber">If true, preset fields will be renumbered</param>
        /// <returns>T</returns>
        T CloneWrapper(string hashsalt, bool renumber = true, bool isStandAlone = true);
    }
}
