/***************************************************************************
 *  Copyright (C) 2015, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Peter L. Jones (pljones@users.sf.net)                                  *
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

namespace s4pi.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using s4pi.Extensions.Properties;

    /// <summary>
    /// Provides a look-up from resource type to resource &quot;tag&quot; and file extension.
    /// The collection is read-only.
    /// </summary>
    /// <seealso cref="TGIN"/>
    public class ExtList : Dictionary<string, List<string>>
    {
        private static readonly ExtList e;

        static ExtList()
        {
            e = new ExtList();
        }

        /// <summary>
        /// A look-up from resource type to resource &quot;tag&quot; and file extension.
        /// The collection is read-only.
        /// </summary>
        public static ExtList Ext
        {
            get { return e; }
        }

        private ExtList()
        {
            StringReader reader = new StringReader(Resources.Extensions);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith(";", StringComparison.Ordinal))
                {
                    continue;
                }

                List<string> parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (parts.Count < 2)
                {
                    continue;
                }

                string key = parts[0];
                if (this.ContainsKey(key))
                {
                    string message = string.Format("ExtList already contains entry for key '{0}'", key);
                    throw new InvalidOperationException(message);
                }

                parts.RemoveAt(0);
                this.Add(key, parts);
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.
        /// If the specified key is not found, returns a default value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        public List<string> this[uint key]
        {
            get { return this["0x" + key.ToString("X8")]; }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key.
        /// If the specified key is not found, returns a default value.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <exception cref="InvalidOperationException">An attempt was made to set a value.</exception>
        public new List<string> this[string key]
        {
            get { return this.ContainsKey(key) ? base[key] : base["*"]; }
            set { throw new InvalidOperationException(); }
        }
    }
}