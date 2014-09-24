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
    /// Defines a quaternion - the quotient of two vectors.
    /// </summary>
    public class Quaternion : AHandlerElement, IEquatable<Quaternion>
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        float a = 0f;
        float b = 0f;
        float c = 0f;
        float d = 0f;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a Quaternion { 0, 0, 0, 0 }.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        public Quaternion(int APIversion, EventHandler handler) : base(APIversion, handler) { }
        /// <summary>
        /// Create a Quaternion from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="s"><see cref="Stream"/> containing coordinates.</param>
        public Quaternion(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
        /// <summary>
        /// Create a Quaternion from a given value.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="basis"><see cref="Quaternion"/> to copy.</param>
        public Quaternion(int APIversion, EventHandler handler, Quaternion basis)
            : this(APIversion, handler, basis.a, basis.b, basis.c, basis.d) { }
        /// <summary>
        /// Create a Quaternion { a, b, c, d }.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="a">Q 'a' value.</param>
        /// <param name="b">Q 'b' value.</param>
        /// <param name="c">Q 'c' value.</param>
        /// <param name="d">Q 'd' value.</param>
        public Quaternion(int APIversion, EventHandler handler, float a, float b, float c, float d)
            : base(APIversion, handler)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.a = r.ReadSingle();
            this.b = r.ReadSingle();
            this.c = r.ReadSingle();
            this.d = r.ReadSingle();
        }

        /// <summary>
        /// Write the Quaternion to the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> to contain coordinates.</param>
        public void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(a);
            w.Write(b);
            w.Write(c);
            w.Write(d);
        }
        #endregion

        #region AHandlerElement Members
        /// <summary>
        /// The best supported version of the API available
        /// </summary>
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        /// <summary>
        /// The list of available field names on this API object.
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        // /// <summary>
        // /// Get a copy of the <see cref="Quaternion"/> but with a new change <see cref="EventHandler"/>.
        // /// </summary>
        // /// <param name="handler">The replacement <see cref="EventHandler"/> delegate.</param>
        // /// <returns>Return a copy of the <see cref="Quaternion"/> but with a new change <see cref="EventHandler"/>.</returns>
        // public override AHandlerElement Clone(EventHandler handler) { return new Quaternion(requestedApiVersion, handler, this); }
        #endregion

        #region IEquatable<Quaternion> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(Quaternion other)
        {
            return this.a == other.a && this.b == other.b && this.c == other.c && this.d == other.d;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Quaternion"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Quaternion"/>.</param>
        /// <returns>true if the specified <see cref="System.Object"/> is equal to the current <see cref="Quaternion"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj as Quaternion != null ? this.Equals(obj as Quaternion) : false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return a.GetHashCode() ^ b.GetHashCode() ^ c.GetHashCode() ^ d.GetHashCode();
        }

        #endregion

        #region Content Fields
        /// <summary>
        /// 'a' value
        /// </summary>
        [ElementPriority(1)]
        public float A { get { return a; } set { if (a != value) { a = value; OnElementChanged(); } } }
        /// <summary>
        /// 'b' value
        /// </summary>
        [ElementPriority(2)]
        public float B { get { return b; } set { if (b != value) { b = value; OnElementChanged(); } } }
        /// <summary>
        /// 'c' value
        /// </summary>
        [ElementPriority(3)]
        public float C { get { return c; } set { if (c != value) { c = value; OnElementChanged(); } } }
        /// <summary>
        /// 'd' value
        /// </summary>
        [ElementPriority(3)]
        public float D { get { return d; } set { if (d != value) { d = value; OnElementChanged(); } } }

        /// <summary>
        /// A displayable representation of the object
        /// </summary>
        public string Value { get { return "{ " + String.Format("{0:F4}; {1:F4}; {2:F4}; {3:F4}", a, b, c, d) + " }"; } }
        #endregion
    }
}