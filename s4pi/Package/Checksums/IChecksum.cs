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

namespace ICSharpCode.SharpZipLib.Checksums 
{
	
	/// <summary>
	/// Interface to compute a data checksum used by checked input/output streams.
	/// A data checksum can be updated by one byte or with a byte array. After each
	/// update the value of the current checksum can be returned by calling
	/// <code>getValue</code>. The complete checksum object can also be reset
	/// so it can be used again with new data.
	/// </summary>
	public interface IChecksum
	{
		/// <summary>
		/// Returns the data checksum computed so far.
		/// </summary>
		long Value 
		{
			get;
		}
		
		/// <summary>
		/// Resets the data checksum as if no update was ever called.
		/// </summary>
		void Reset();
		
		/// <summary>
		/// Adds one byte to the data checksum.
		/// </summary>
		/// <param name = "value">
		/// the data value to add. The high byte of the int is ignored.
		/// </param>
		void Update(int value);
		
		/// <summary>
		/// Updates the data checksum with the bytes taken from the array.
		/// </summary>
		/// <param name="buffer">
		/// buffer an array of bytes
		/// </param>
		void Update(byte[] buffer);
		
		/// <summary>
		/// Adds the byte array to the data checksum.
		/// </summary>
		/// <param name = "buffer">
		/// The buffer which contains the data
		/// </param>
		/// <param name = "offset">
		/// The offset in the buffer where the data starts
		/// </param>
		/// <param name = "count">
		/// the number of data bytes to add.
		/// </param>
		void Update(byte[] buffer, int offset, int count);
	}
}
