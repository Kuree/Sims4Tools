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

namespace System
{
    /// <summary>
    /// Represents an error in the length of an argument to a method
    /// </summary>
    public class ArgumentLengthException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the System.ArgumentLengthException class.
        /// </summary>
        public ArgumentLengthException() : base() { }
        /// <summary>
        /// Initializes a new instance of the System.ArgumentLengthException class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ArgumentLengthException(string message) : base(message) { }
        /// <summary>
        /// Initializes a new instance of the System.ArgumentLengthException class with a predefined message based on
        /// <paramref name="argument"/> and <paramref name="length"/>.
        /// </summary>
        /// <param name="argument">Name of the method argument in error</param>
        /// <param name="length">Valid length of the argument</param>
        public ArgumentLengthException(string argument, int length) : base(String.Format("{0} length must be {1}.", argument, length)) { }
        /// <summary>
        /// Initializes a new instance of the System.ArgumentLengthException class with a formatted error message.
        /// See <see cref="String.Format(string, object[])"/>.
        /// </summary>
        /// <param name="format">format string</param>
        /// <param name="formatparams">format string substitutions</param>
        public ArgumentLengthException(string format, params object[] formatparams) : base(String.Format(format, formatparams)) { }
    }
}
