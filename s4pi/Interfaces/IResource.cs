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
using System.IO;

namespace s4pi.Interfaces
{
    /// <summary>
    /// Minimal resource interface
    /// </summary>
    public interface IResource : IApiVersion, IContentFields
    {
        /// <summary>
        /// The resource content as a <see cref="System.IO.Stream"/>.
        /// </summary>
        Stream Stream { get; }
        /// <summary>
        /// The resource content as a <see cref="byte"/> array
        /// </summary>
        byte[] AsBytes { get; }

        /// <summary>
        /// Raised if the resource is changed
        /// </summary>
        event EventHandler ResourceChanged;
    }
}
