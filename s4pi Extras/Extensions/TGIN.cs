/***************************************************************************
 *  Copyright (C) 2009, 2015 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Peter L Jones (pljones@users.sf.net)                                   *
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
        public TGIN(IResourceKey rk, string name)
        {
            this.ResType = rk.ResourceType;
            this.ResGroup = rk.ResourceGroup;
            this.ResInstance = rk.Instance;
            this.ResName = name;
        }

        /// <summary>
        /// Cast an <see cref="AResourceKey"/> value to a <see cref="TGIN"/>.
        /// </summary>
        /// <param name="value">An <see cref="AResourceKey"/> value.</param>
        /// <returns>The equivalent <see cref="TGIN"/> (with no <see cref="ResName"/>).</returns>
        public static implicit operator TGIN(AResourceKey value)
        {
            TGIN res = new TGIN
                       {
                           ResType = value.ResourceType,
                           ResGroup = value.ResourceGroup,
                           ResInstance = value.Instance
                       };
            return res;
        }

        /// <summary>
        /// Cast a <see cref="TGIN"/> to an <see cref="AResourceKey"/> value.
        /// </summary>
        /// <param name="value">A <see cref="TGIN"/>.</param>
        /// <returns>The equivalent <see cref="AResourceKey"/> value.</returns>
        public static implicit operator AResourceKey(TGIN value)
        {
            return new TGIBlock(0, null, value.ResType, value.ResGroup, value.ResInstance);
        }

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
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            TGIN res = new TGIN();

            value = Path.GetFileName(value);
            if (FileNameConverter.IsValidFileName(value))
            {
                value = FileNameConverter.ConvertToPackageManagerConvention(value);
            }

            value = Path.GetFileNameWithoutExtension(value);

            int i = value.ToLower().IndexOf("s4_", StringComparison.Ordinal);
            if (i == 0)
            {
                value = value.Substring(3);
            }
            i = value.IndexOf("%%+", StringComparison.Ordinal);
            if (i >= 0)
            {
                value = value.Substring(0, i);
            }

            string[] fnParts = value.Split(new[] { '_', '-' }, 4);
            if (fnParts.Length >= 3)
            {
                try
                {
                    res.ResType = Convert.ToUInt32(fnParts[0], 16);
                    res.ResGroup = Convert.ToUInt32(fnParts[1], 16);
                    res.ResInstance = Convert.ToUInt64(fnParts[2], 16);
                }
                catch
                {
                }
                if (fnParts.Length == 4)
                {
                    res.ResName = TGIN.UnescapeString(fnParts[3]);
                }
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
            {
                extn = string.Join("", ExtList.Ext["0x" + value.ResType.ToString("X8")].ToArray());
            }
            else if (ExtList.Ext.ContainsKey("*"))
            {
                extn = string.Join("", ExtList.Ext["*"].ToArray());
            }

            return string.Format(!string.IsNullOrEmpty(value.ResName)
                ? "S4_{0:X8}_{1:X8}_{2:X16}_{3}%%+{4}"
                : "S4_{0:X8}_{1:X8}_{2:X16}%%+{4}"
                ,
                value.ResType,
                value.ResGroup,
                value.ResInstance,
                value.ResName == null ? "" : TGIN.EscapeString(value.ResName),
                extn);
        }

        /// <summary>
        /// Returns a <see cref="string"/> in the standardised Sims3 resource export file name format
        /// equivalent to this instance.
        /// </summary>
        /// <returns>A <see cref="string"/> in the standardised Sims3 resource export file name format.</returns>
        public override string ToString()
        {
            return this;
        }

        private static string UnescapeString(string value)
        {
            for (int i = value.IndexOf("%", StringComparison.Ordinal); i >= 0 && i + 2 < value.Length; i = value.IndexOf("%", StringComparison.Ordinal))
            {
                try
                {
                    string bad = value.Substring(i + 1, 2);
                    string rep = new string(new[] { (char)Convert.ToByte(bad, 16) });
                    value = value.Replace("%" + bad, rep);
                }
                catch
                {
                    break;
                }
            }
            return value;
        }

        private static string EscapeString(string value)
        {
            foreach (
                char[] ary in
                    new[] { Path.GetInvalidFileNameChars(), Path.GetInvalidPathChars(), new[] { '-' } })
            {
                foreach (char c in ary)
                {
                    string bad = new string(new[] { c });
                    string rep = string.Format("%{0:x2}", (uint)c);
                    value = value.Replace(bad, rep);
                }
            }
            return value;
        }
    }
}