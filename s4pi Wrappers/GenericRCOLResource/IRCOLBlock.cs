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
    /// Defines the interface exposed by an RCOL block.
    /// </summary>
    public interface IRCOLBlock : IResource
    {
        /// <summary>
        /// The four byte tag for the RCOL block, may be null if none present
        /// </summary>
        [ElementPriority(2)]
        string Tag { get; }

        /// <summary>
        /// The ResourceType for the RCOL block, used to determine which specific RCOL handlers are available
        /// </summary>
        [ElementPriority(3)]
        uint ResourceType { get; }

        /// <summary>
        /// Writes the content of the RCOL block to a stream
        /// </summary>
        /// <returns>A stream containing the current content of the RCOL block</returns>
        Stream UnParse();
    }
}
