/***************************************************************************
 *  Copyright (C) 2009, 2015 by the Sims 4 Tools development team          *
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

namespace s4pi.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Utility class to convert file names or paths in the TS4 resource name
    /// convention to the s4pe convention.
    /// </summary>
    public static class FileNameConverter
    {
        private static readonly IDictionary<string, string> resourceMap = new Dictionary<string, string>
		{
			{ "trayitem", "0x2a8a5e22" },
			{ "blueprint", "0x3924de26" },
			{ "bpi", "0xd33c281e" },
			{ "householdbinary", "0xb3c438f0" },
			{ "hhi", "0x3bd45407" },
			{ "sgi", "0x56278554" },
			{ "room", "0x370efd6e" },
			{ "rmi", "0x00de5ac5" }
		};

        /// <summary>
        /// Returns a value indicating whether the given <paramref name="path"/> is
        /// a valid TS4 resource file name.
        /// </summary>
        public static bool IsValidFileName(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            const string matchPattern = "0x[0-9a-f]{8}!0x[0-9a-f]{16}\\.(trayitem|blueprint|bpi|householdbinary|hhi|sgi|room|rmi)";

            bool isValid = Regex.IsMatch(path, matchPattern, RegexOptions.IgnoreCase);

            return isValid;
        }

        /// <summary>
        /// Converts the specified TS4 file name or path to its s4pe equivalent.
        /// </summary>
        public static string ConvertToPackageManagerConvention(string ts4Path)
        {
            string path = Path.GetDirectoryName(ts4Path) ?? string.Empty;
            string ts4Name = Path.GetFileName(ts4Path) ?? string.Empty;
            var parts = ts4Name.Split('!', '.');

            FileNameConverter.GuardInputIsValid(parts);

            string group = parts[0];
            string id = parts[1];
            string type = FileNameConverter.resourceMap[parts[2]];
            string extension = parts[2];

            return Path.Combine(path, string.Format("S4_{0}_{1}_{2}.{3}", type, group, id, extension));
        }

        private static void GuardInputIsValid(IList<string> parts)
        {
            if (parts.Count != 3)
            {
                throw new ArgumentException("fileName must be in format 'Group!ID.Type'");
            }

            if (!FileNameConverter.resourceMap.ContainsKey(parts[2]))
            {
                throw new NotSupportedException(string.Format("type '{0}' is not supported", parts[2]));
            }
        }
    }
}