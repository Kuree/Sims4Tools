using System;
using System.Collections;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Classes implementing this interface can have elements added with
    /// an empty parameter list or
    /// the list of arguments to a generic class&apos; constructor.
    /// </summary>
    /// <seealso cref="DependentList{T}"/>
    public interface IGenericAdd : IList
    {
        /// <summary>
        /// Add a default element to an <see cref="IList"/> that implements this interface.
        /// </summary>
        /// <exception cref="NotImplementedException">Lists of abstract classes will fail
        /// with a NotImplementedException.</exception>
        /// <seealso cref="DependentList{T}"/>
        void Add();

        ///// <summary>
        ///// Adds an entry to an <see cref="IList"/> that implements this interface.
        ///// </summary>
        ///// <param name="fields">
        ///// Either the object to add or the generic type&apos;s constructor arguments.
        ///// </param>
        ///// <returns>True on success</returns>
        ///// <seealso cref="DependentList{T}"/>
        //bool Add(params object[] fields);

        /// <summary>
        /// Adds an entry to an <see cref="IList"/> that implements this interface.
        /// </summary>
        /// <param name="instanceType">Type of the instance to create and add to the <see cref="IList"/>.</param>
        void Add(Type instanceType);
    }
}
