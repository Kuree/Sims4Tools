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
