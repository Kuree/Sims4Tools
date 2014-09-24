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
    /// Base class for versioning support.  Not directly used by the API.
    /// </summary>
    public class VersionAttribute : Attribute
    {
        Int32 version;
        /// <summary>
        /// Version number attribute (base)
        /// </summary>
        /// <param name="Version">Version number</param>
        public VersionAttribute(Int32 Version) { version = Version; }
        /// <summary>
        /// Version number
        /// </summary>
        public Int32 Version { get { return version; } set { version = value; } }
    }

    /// <summary>
    /// Specify the Minumum version from which a field or method is supported
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false,  Inherited = true)]
    public class MinimumVersionAttribute : VersionAttribute
    {
        /// <summary>
        /// Specify the Minumum version from which a field or method is supported
        /// </summary>
        /// <param name="Version">Version number</param>
        public MinimumVersionAttribute(Int32 Version) : base(Version) { }
    }

    /// <summary>
    /// Specify the Maximum version up to which a field or method is supported
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class MaximumVersionAttribute : VersionAttribute
    {
        /// <summary>
        /// Specify the Maximum version up to which a field or method is supported
        /// </summary>
        /// <param name="Version">Version number</param>
        public MaximumVersionAttribute(Int32 Version) : base(Version) { }
    }
}
