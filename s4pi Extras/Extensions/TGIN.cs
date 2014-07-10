using System;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.Extensions
{
    /// <summary>
    /// A structure to manage conversion from <see cref="AResourceKey"/> to
    /// the standardised Sims3 resource export file name format.
    /// </summary>
    /// <seealso cref="ExtList"/>
    [Serializable]
    public struct TGIN
    {
        /// <summary>
        /// The Resource Type represented by this instance.
        /// </summary>
        public uint ResType;
        /// <summary>
        /// The Resource Group represented by this instance.
        /// </summary>
        public uint ResGroup;
        /// <summary>
        /// The Resource Instance ID represented by this instance.
        /// </summary>
        public ulong ResInstance;
        /// <summary>
        /// The Resource Name (from the package name map, based on the IID) represented by this instance.
        /// </summary>
        public string ResName;

        /// <summary>
        /// Instantiate a new <see cref="TGIN"/> based on the <see cref="IResourceKey"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="rk">An <see cref="IResourceKey"/>.</param>
        /// <param name="name">A <see cref="String"/>, the name of the resource.</param>
        public TGIN(IResourceKey rk, string name) { ResType = rk.ResourceType; ResGroup = rk.ResourceGroup; ResInstance = rk.Instance; ResName = name; }

        /// <summary>
        /// Cast an <see cref="AResourceKey"/> value to a <see cref="TGIN"/>.
        /// </summary>
        /// <param name="value">An <see cref="AResourceKey"/> value.</param>
        /// <returns>The equivalent <see cref="TGIN"/> (with no <see cref="ResName"/>).</returns>
        public static implicit operator TGIN(AResourceKey value)
        {
            TGIN res = new TGIN();
            res.ResType = value.ResourceType;
            res.ResGroup = value.ResourceGroup;
            res.ResInstance = value.Instance;
            return res;
        }
        /// <summary>
        /// Cast a <see cref="TGIN"/> to an <see cref="AResourceKey"/> value.
        /// </summary>
        /// <param name="value">A <see cref="TGIN"/>.</param>
        /// <returns>The equivalent <see cref="AResourceKey"/> value.</returns>
        public static implicit operator AResourceKey(TGIN value) { return new TGIBlock(0, null, value.ResType, value.ResGroup, value.ResInstance); }

        /// <summary>
        /// Casts a <see cref="string"/> to a <see cref="TGIN"/>.
        /// <para>The string value is presumed to be in the standardised
        /// Sims3 resource export file name format.</para>
        /// </summary>
        /// <param name="value">A string value is presumed to be in the standardised
        /// Sims3 resource export file name format.</param>
        /// <returns>The equivalent <see cref="TGIN"/> value.</returns>
        public static implicit operator TGIN(string value)
        {
            TGIN res = new TGIN();

            value = System.IO.Path.GetFileNameWithoutExtension(value);

            int i = value.ToLower().IndexOf("s3_");
            if (i == 0) value = value.Substring(3);
            i = value.IndexOf("%%+");
            if (i >= 0) value = value.Substring(0, i);

            string[] fnParts = value.Split(new char[] { '_', '-' }, 4);
            if (fnParts.Length >= 3)
            {
                try
                {
                    res.ResType = Convert.ToUInt32(fnParts[0], 16);
                    res.ResGroup = Convert.ToUInt32(fnParts[1], 16);
                    res.ResInstance = Convert.ToUInt64(fnParts[2], 16);
                }
                catch { }
                if (fnParts.Length == 4)
                    res.ResName = unescapeString(fnParts[3]);
            }

            return res;
        }
        /// <summary>
        /// Casts a <see cref="TGIN"/> to a <see cref="string"/> 
        /// in the standardised Sims3 resource export file name format.
        /// </summary>
        /// <param name="value">A <see cref="TGIN"/>.</param>
        /// <returns>A <see cref="string"/> in the standardised Sims3 resource export file name format.</returns>
        public static implicit operator string(TGIN value)
        {
            string extn = ".dat";
            if (ExtList.Ext.ContainsKey("0x" + value.ResType.ToString("X8")))
                extn = String.Join("", ExtList.Ext["0x" + value.ResType.ToString("X8")].ToArray());
            else if (ExtList.Ext.ContainsKey("*"))
                extn = String.Join("", ExtList.Ext["*"].ToArray());

            return String.Format((value.ResName != null && value.ResName.Length > 0)
                    ? "S3_{0:X8}_{1:X8}_{2:X16}_{3}%%+{4}"
                    : "S3_{0:X8}_{1:X8}_{2:X16}%%+{4}"
                    , value.ResType, value.ResGroup, value.ResInstance, value.ResName == null ? "" : escapeString(value.ResName), extn);
        }

        /// <summary>
        /// Returns a <see cref="string"/> in the standardised Sims3 resource export file name format
        /// equivalent to this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> in the standardised Sims3 resource export file name format.</returns>
        public override string ToString() { return this; }

        private static string unescapeString(string value)
        {
            for (int i = value.IndexOf("%"); i >= 0 && i + 2 < value.Length; i = value.IndexOf("%"))
            {
                try
                {
                    string bad = value.Substring(i + 1, 2);
                    string rep = new string(new char[] { (char)Convert.ToByte(bad, 16) });
                    value = value.Replace("%" + bad, rep);
                }
                catch { break; }
            }
            return value;
        }

        private static string escapeString(string value)
        {
            foreach (char[] ary in new char[][] { Path.GetInvalidFileNameChars(), Path.GetInvalidPathChars(), new char[] { '-' } })
            {
                foreach (char c in ary)
                {
                    string bad = new string(new char[] { c });
                    string rep = String.Format("%{0:x2}", (uint)c);
                    value = value.Replace(bad, rep);
                }
            }
            return value;
        }
    }
}
