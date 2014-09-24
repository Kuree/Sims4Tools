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
    /// Defines a vertex - a point in 3d space defined by three coordinates.
    /// </summary>
    public class Vertex : AHandlerElement, IEquatable<Vertex>
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        float x = 0f;
        float y = 0f;
        float z = 0f;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a vertex at { 0, 0, 0 }.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        public Vertex(int APIversion, EventHandler handler) : base(APIversion, handler) { }
        /// <summary>
        /// Create a vertex from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="s"><see cref="Stream"/> containing coordinates.</param>
        public Vertex(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
        /// <summary>
        /// Create a vertex from a given value.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="basis"><see cref="Vertex"/> to copy.</param>
        public Vertex(int APIversion, EventHandler handler, Vertex basis)
            : this(APIversion, handler, basis.x, basis.y, basis.z) { }
        /// <summary>
        /// Create a vertex at { x, y, z }.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="z">Z coordinate.</param>
        public Vertex(int APIversion, EventHandler handler, float x, float y, float z)
            : base(APIversion, handler)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.x = r.ReadSingle();
            this.y = r.ReadSingle();
            this.z = r.ReadSingle();
        }

        /// <summary>
        /// Write the vertex to the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> to contain coordinates.</param>
        public void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(x);
            w.Write(y);
            w.Write(z);
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
        // /// Get a copy of the <see cref="Vertex"/> but with a new change <see cref="EventHandler"/>.
        // /// </summary>
        // /// <param name="handler">The replacement <see cref="EventHandler"/> delegate.</param>
        // /// <returns>Return a copy of the <see cref="Vertex"/> but with a new change <see cref="EventHandler"/>.</returns>
        // public override AHandlerElement Clone(EventHandler handler) { return new Vertex(requestedApiVersion, handler, this); }
        #endregion

        #region IEquatable<BoxPoint> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(Vertex other)
        {
            return this.x == other.x && this.y == other.y && this.z == other.z;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Vertex"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Vertex"/>.</param>
        /// <returns>true if the specified <see cref="System.Object"/> is equal to the current <see cref="Vertex"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj as Vertex != null ? this.Equals(obj as Vertex) : false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        #endregion

        #region Content Fields
        /// <summary>
        /// X coordinate
        /// </summary>
        [ElementPriority(1)]
        public float X { get { return x; } set { if (x != value) { x = value; OnElementChanged(); } } }
        /// <summary>
        /// Y coordinate
        /// </summary>
        [ElementPriority(2)]
        public float Y { get { return y; } set { if (y != value) { y = value; OnElementChanged(); } } }
        /// <summary>
        /// Z coordinate
        /// </summary>
        [ElementPriority(3)]
        public float Z { get { return z; } set { if (z != value) { z = value; OnElementChanged(); } } }

        /// <summary>
        /// A displayable representation of the object
        /// </summary>
        public string Value { get { return "{ " + String.Format("{0:F4}; {1:F4}; {2:F4}", x, y, z) + " }"; } }
        #endregion
    }

    /// <summary>
    /// Defines a bounding box - a imaginary box large enough to completely contain an object
    /// - by its minimum and maximum vertices.
    /// </summary>
    public class BoundingBox : AHandlerElement, IEquatable<BoundingBox>
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        Vertex min;
        Vertex max;
        #endregion

        #region Constructors
        /// <summary>
        /// Create an zero-sized bounding box.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        public BoundingBox(int APIversion, EventHandler handler) : base(APIversion, handler)
        {
            min = new Vertex(requestedApiVersion, handler);
            max = new Vertex(requestedApiVersion, handler);
        }
        /// <summary>
        /// Create a bounding box from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="s"><see cref="Stream"/> containing vertices.</param>
        public BoundingBox(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
        /// <summary>
        /// Create a bounding box from a given value.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="basis"><see cref="BoundingBox"/> to copy.</param>
        public BoundingBox(int APIversion, EventHandler handler, BoundingBox basis)
            : this(APIversion, handler, basis.min, basis.max) { }
        /// <summary>
        /// Create a bounding box with the specified minimum and maximum vertices.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="min">Minimum vertex.</param>
        /// <param name="max">Maximum vertex.</param>
        public BoundingBox(int APIversion, EventHandler handler, Vertex min, Vertex max)
            : base(APIversion, handler)
        {
            this.min = new Vertex(requestedApiVersion, handler, min);
            this.max = new Vertex(requestedApiVersion, handler, max);
        }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            min = new Vertex(requestedApiVersion, handler, s);
            max = new Vertex(requestedApiVersion, handler, s);
        }

        /// <summary>
        /// Write the bounding box to the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> to contain vertices.</param>
        public void UnParse(Stream s)
        {
            if (min == null) min = new Vertex(requestedApiVersion, handler);
            min.UnParse(s);

            if (max == null) max = new Vertex(requestedApiVersion, handler);
            max.UnParse(s);
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
        // /// Get a copy of the <see cref="BoundingBox"/> but with a new change <see cref="EventHandler"/>.
        // /// </summary>
        // /// <param name="handler">The replacement <see cref="EventHandler"/> delegate.</param>
        // /// <returns>Return a copy of the <see cref="BoundingBox"/> but with a new change <see cref="EventHandler"/>.</returns>
        // public override AHandlerElement Clone(EventHandler handler) { return new BoundingBox(requestedApiVersion, handler, this); }
        #endregion

        #region IEquatable<BoundingBox> Members

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(BoundingBox other)
        {
            return min.Equals(other.min) && max.Equals(other.max);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="BoundingBox"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="BoundingBox"/>.</param>
        /// <returns>true if the specified <see cref="System.Object"/> is equal to the current <see cref="BoundingBox"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj as BoundingBox != null ? this.Equals(obj as BoundingBox) : false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return min.GetHashCode() ^ max.GetHashCode();
        }

        #endregion

        #region Content Fields
        /// <summary>
        /// Minimum vertex
        /// </summary>
        [ElementPriority(1)]
        public Vertex Min { get { return min; } set { if (min != value) { min = new Vertex(requestedApiVersion, handler, value); OnElementChanged(); } } }
        /// <summary>
        /// Maximum vertex
        /// </summary>
        [ElementPriority(2)]
        public Vertex Max { get { return max; } set { if (max != value) { max = new Vertex(requestedApiVersion, handler, value); OnElementChanged(); } } }

        /// <summary>
        /// A displayable representation of the object
        /// </summary>
        public string Value { get { return String.Format("[ Min: {0} | Max: {1} ]", min.Value, max.Value); } }
        #endregion
    }
}