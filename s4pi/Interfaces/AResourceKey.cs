using System;
using System.IO;

namespace s4pi.Interfaces
{
    /// <summary>
    /// An abstract class, extending <see cref="AHandlerElement"/> and implementing <see cref="IResourceKey"/>.
    /// </summary>
    public abstract class AResourceKey : AHandlerElement, IResourceKey
    {
        #region Attributes
        /// <summary>
        /// The "type" of the resource
        /// </summary>
        protected uint resourceType;
        /// <summary>
        /// The "group" the resource is part of
        /// </summary>
        protected uint resourceGroup;
        /// <summary>
        /// The "instance" number of the resource
        /// </summary>
        protected ulong instance;
        #endregion

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AResourceKey"/> changes.</param>
        public AResourceKey(int APIversion, EventHandler handler) : base(APIversion, handler) { }
        /// <summary>
        /// Initialize a new instance
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AResourceKey"/> changes.</param>
        /// <param name="basis">The <see cref="IResourceKey"/> values to use to initialise the instance.</param>
        public AResourceKey(int APIversion, EventHandler handler, IResourceKey basis)
            : this(APIversion, handler, basis.ResourceType, basis.ResourceGroup, basis.Instance) { }
        /// <summary>
        /// Initialize a new instance
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AResourceKey"/> changes.</param>
        /// <param name="resourceType">The type of the resource.</param>
        /// <param name="resourceGroup">The group of the resource.</param>
        /// <param name="instance">The instance of the resource.</param>
        public AResourceKey(int APIversion, EventHandler handler, uint resourceType, uint resourceGroup, ulong instance)
            : base(APIversion, handler)
        {
            this.resourceType = resourceType;
            this.resourceGroup = resourceGroup;
            this.instance = instance;
        }

        #region IResourceKey Members
        /// <summary>
        /// The "type" of the resource
        /// </summary>
        [ElementPriority(1)]
        public virtual uint ResourceType { get { return resourceType; } set { if (resourceType != value) { resourceType = value; OnElementChanged(); } } }
        /// <summary>
        /// The "group" the resource is part of
        /// </summary>
        [ElementPriority(2)]
        public virtual uint ResourceGroup { get { return resourceGroup; } set { if (resourceGroup != value) { resourceGroup = value; OnElementChanged(); } } }
        /// <summary>
        /// The "instance" number of the resource
        /// </summary>
        [ElementPriority(3)]
        public virtual ulong Instance { get { return instance; } set { if (instance != value) { instance = value; OnElementChanged(); } } }
        #endregion

        #region IEqualityComparer<IResourceKey> Members

        /// <summary>
        /// Determines whether the specified <see cref="IResourceKey"/> instances are equal.
        /// </summary>
        /// <param name="x">The first <see cref="IResourceKey"/> to compare.</param>
        /// <param name="y">The second <see cref="IResourceKey"/> to compare.</param>
        /// <returns>true if the specified <see cref="IResourceKey"/> instances are equal; otherwise, false.</returns>
        public bool Equals(IResourceKey x, IResourceKey y) { return x.Equals(y); }

        /// <summary>
        /// Returns a hash code for the specified <see cref="IResourceKey"/>.
        /// </summary>
        /// <param name="obj">The <see cref="IResourceKey"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="ArgumentNullException">The type of <paramref name="obj"/> is a reference type and
        /// <paramref name="obj"/> is null.</exception>
        /// <seealso cref="GetHashCode()"/>
        public int GetHashCode(IResourceKey obj) { return obj.GetHashCode(); }

        /// <summary>
        /// Serves as a hash function for an <see cref="AResourceKey"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="AResourceKey"/>.</returns>
        public override int GetHashCode() { return ResourceType.GetHashCode() ^ ResourceGroup.GetHashCode() ^ Instance.GetHashCode(); }

        #endregion

        #region IEquatable<IResourceKey> Members

        /// <summary>
        /// Indicates whether the current <see cref="AResourceKey"/> instance is equal to another <see cref="IResourceKey"/> instance.
        /// </summary>
        /// <param name="other">An <see cref="IResourceKey"/> instance to compare with this instance.</param>
        /// <returns>true if the current instance is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        public bool Equals(IResourceKey other) { return this.CompareTo(other) == 0; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="AResourceKey"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="AResourceKey"/>.</param>
        /// <returns>true if the specified <see cref="System.Object"/> is equal to the current <see cref="AResourceKey"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return obj as AResourceKey != null ? this.Equals(obj as AResourceKey) : false;
        }

        #endregion

        #region IComparable<IResourceKey> Members

        /// <summary>
        /// Compare this <see cref="AResourceKey"/> to another <see cref="IResourceKey"/> for sort order purposes
        /// </summary>
        /// <param name="other">Target <see cref="IResourceKey"/></param>
        /// <returns>A 32-bit signed integer that indicates the relative order of the objects being compared.  The return value has these meanings:
        /// <table>
        /// <thead><tr><td><strong>Value</strong></td><td><strong>Meaning</strong></td></tr></thead>
        /// <tbody>
        /// <tr><td>Less than zero</td><td>This instance is less than <paramref name="other"/>.</td></tr>
        /// <tr><td>Zero</td><td>This instance is equal to <paramref name="other"/>.</td></tr>
        /// <tr><td>Greater than zero</td><td>This instance is greater than <paramref name="other"/>.</td></tr>
        /// </tbody>
        /// </table>
        /// </returns>
        /// <exception cref="NotImplementedException">Either this object's Type or the target's is not comparable</exception>
        /// <exception cref="ArgumentException">The target is not comparable with this object</exception>
        public int CompareTo(IResourceKey other)
        {
            int res = ResourceType.CompareTo(other.ResourceType); if (res != 0) return res;
            res = ResourceGroup.CompareTo(other.ResourceGroup); if (res != 0) return res;
            return Instance.CompareTo(other.Instance);
        }

        #endregion

        /// <summary>
        /// Converts the string representation of a resource key to its <see cref="AResourceKey"/>
        /// equivalent. A return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="value">A string containing a resource key to convert.</param>
        /// <param name="result">
        /// When this method returns, contains the <see cref="AResourceKey"/> equivalent
        /// to the resource key contained in <paramref name="value"/>, if the conversion succeeded, or
        /// unchanged if the conversion failed.  The conversion fails if the <paramref name="value"/>
        /// parameter is null or is not of the correct format.  This parameter must not be null.
        /// </param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        /// <exception cref="NullReferenceException">Thrown if <paramref name="result"/> is null.</exception>
        public static bool TryParse(String value, IResourceKey result)
        {
            if (value == null) return false;

            uint t, g;
            ulong i;

            // Two formats 0x{X8}-0x{X8}-0x{X16} and (key:){X8}:{X8}:{X16}
            if (value.Contains("-"))
            {
                string[] tgi = value.Trim().ToLower().Split('-');
                if (tgi.Length != 3) return false;
                foreach (var x in tgi) if (!x.StartsWith("0x")) return false;

                if (!uint.TryParse(tgi[0].Substring(2), System.Globalization.NumberStyles.HexNumber, null, out t)) return false;
                if (!uint.TryParse(tgi[1].Substring(2), System.Globalization.NumberStyles.HexNumber, null, out g)) return false;
                if (!ulong.TryParse(tgi[2].Substring(2), System.Globalization.NumberStyles.HexNumber, null, out i)) return false;
            }
            else
            {
                string[] tgi = value.Trim().ToLower().Split(':');
                if (tgi.Length == 4 && tgi[0].ToLower() == "key")
                    Array.Copy(tgi, 1, tgi, 0, 3);
                else if (tgi.Length != 3) return false;

                if (!uint.TryParse(tgi[0], System.Globalization.NumberStyles.HexNumber, null, out t)) return false;
                if (!uint.TryParse(tgi[1], System.Globalization.NumberStyles.HexNumber, null, out g)) return false;
                if (!ulong.TryParse(tgi[2], System.Globalization.NumberStyles.HexNumber, null, out i)) return false;
            }

            result.ResourceType = t;
            result.ResourceGroup = g;
            result.Instance = i;

            return true;
        }

        /// <summary>
        /// Converts an <see cref="AResourceKey"/> to a string representation.
        /// </summary>
        /// <param name="value">The <see cref="AResourceKey"/> to convert</param>
        /// <returns>The 42 character string representation of this resource key,
        /// of the form 0xXXXXXXXX-0xXXXXXXXX-0xXXXXXXXXXXXXXXXX.</returns>
        public static implicit operator String(AResourceKey value) { return String.Format("0x{0:X8}-0x{1:X8}-0x{2:X16}", value.ResourceType, value.ResourceGroup, value.Instance); }
        /// <summary>
        /// Returns a string representation of this <see cref="AResourceKey"/>.
        /// </summary>
        /// <returns>The 42 character string representation of this resource key,
        /// of the form 0xXXXXXXXX-0xXXXXXXXX-0xXXXXXXXXXXXXXXXX.</returns>
        public override string ToString() { return this; }
    }
}
