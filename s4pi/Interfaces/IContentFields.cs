/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
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
