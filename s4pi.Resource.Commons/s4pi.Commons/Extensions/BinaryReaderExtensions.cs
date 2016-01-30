/***************************************************************************
 *  Copyright (C) 2009, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Buzzler                                                                *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s4pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

namespace s4pi.Commons.Extensions
{
    using System.IO;

    /// <summary>
	/// Extensions for the <see cref="BinaryReader"/> class.
	/// </summary>
	public static class BinaryReaderExtensions
	{
		/// <summary>
		/// Reads the specified number of bytes using the <see cref="BinaryReader"/>.
		/// </summary>
		public static byte[] ReadBytes(this BinaryReader reader, int count)
		{
            byte[] bytes = new byte[count];

		    reader.Read(bytes, 0, count);

		    return bytes;
		}
	}
}
