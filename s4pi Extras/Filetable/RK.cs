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
using s4pi.Interfaces;

namespace s4pi.Filetable
{
    /// <summary>
    /// A minimal implementation of <see cref="AResourceKey"/> providing some basic assistance methods.
    /// The <see cref="CompareTo"/> routine is geared to how one resource finds another to the best of
    /// our understanding...
    /// </summary>
    public class RK : AResourceKey
    {
        static RK() { NULL = new RK(); }
        RK() : base(0, null) { }
        /// <summary>
        /// Instantiate the <see cref="IResourceKey"/> as a concrete object.
        /// </summary>
        /// <param name="rk">An <see cref="IResourceKey"/>.</param>
        public RK(IResourceKey rk) : base(0, null, rk) { }

        /// <summary>
        /// Return a null valued resource key.
        /// </summary>
        public static RK NULL { get; private set; }

        /// <summary>
        /// Throw <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="handler">Not used.</param>
        /// <returns>Does not return.</returns>
        public override AHandlerElement Clone(EventHandler handler) { throw new NotImplementedException(); }
        /// <summary>
        /// Throw <see cref="NotImplementedException"/>.
        /// </summary>
        public override List<string> ContentFields { get { throw new NotImplementedException(); } }
        /// <summary>
        /// Throw <see cref="NotImplementedException"/>.
        /// </summary>
        public override int RecommendedApiVersion { get { throw new NotImplementedException(); } }

        /// <summary>
        /// Take into account some of the weird ways in which one resource refers to another, based on resource type.
        /// </summary>
        /// <param name="rk">An <c>IResourceKey</c> to compare with.</param>
        /// <returns></returns>
        public new int CompareTo(IResourceKey rk)
        {
            int res = this.ResourceType.CompareTo(rk.ResourceType); if (res != 0) return res;
            res = (this.ResourceGroup & 0x07FFFFFF).CompareTo(rk.ResourceGroup & 0x07FFFFFF); if (res != 0) return res;
            return ((this.ResourceType == 0x736884F1 && this.Instance >> 32 == 0) ? this.Instance & 0x07FFFFFF : this.Instance)
                .CompareTo(((rk.ResourceType == 0x736884F1 && rk.Instance >> 32 == 0) ? rk.Instance & 0x07FFFFFF : rk.Instance));
        }
        /// <summary>
        /// Take into account whether this resource key is equivalent to <paramref name="rk"/>.
        /// </summary>
        /// <param name="rk">The <see cref="IResourceKey"/> to test.</param>
        /// <returns>True if <paramref name="rk"/> refers to the same resource as this.</returns>
        public new bool Equals(IResourceKey rk) { return this.CompareTo(rk) == 0; }

        /// <summary>
        /// Convert a string to an <see cref="IResourceKey"/> value.
        /// </summary>
        /// <param name="value">A valid string representation of an <see cref="IResourceKey"/>.</param>
        /// <returns>The eqivalent <see cref="IResourceKey"/> value for <paramref name="value"/>.</returns>
        public static IResourceKey Parse(string value)
        {
            IResourceKey result;
            if (!TryParse(value, out result)) throw new ArgumentException();
            return result;
        }

        /// <summary>
        /// Tests whether a string can be converted to an <see cref="IResourceKey"/> and,
        /// if so, sets <paramref name="result"/> to the converted value.
        /// </summary>
        /// <param name="value">A potential string representation of an <see cref="IResourceKey"/>.</param>
        /// <param name="result">The eqivalent <see cref="IResourceKey"/> value for <paramref name="value"/>.</param>
        /// <returns>True if <paramref name="value"/> has the format
        /// "0xHHHHHHHH-0xHHHHHHHH-0xHHHHHHHHHHHHHHHH"; otherwise false.
        /// </returns>
        public static bool TryParse(string value, out IResourceKey result)
        {
            result = new RK();
            return AResourceKey.TryParse(value, result);
        }
    }
}