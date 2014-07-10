/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
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

namespace s3pi.Interfaces
{
    /// <summary>
    /// Specify the constructor parameters for a descendant of an abstract class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    [Obsolete("Use DependentList<T>.Add(T instance) or DependendList<T>.Add(Type concrete-of-T).")]
    public class ConstructorParametersAttribute : Attribute
    {
        /// <summary>
        /// The constructor parameters
        /// </summary>
        public readonly object[] parameters;
        /// <summary>
        /// Specify the constructor parameters for a descendant of an abstract class
        /// </summary>
        /// <param name="parameters">The constructor parameters</param>
        public ConstructorParametersAttribute(object[] parameters) { this.parameters = parameters;}
    }
}
