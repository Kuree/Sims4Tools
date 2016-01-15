/***************************************************************************
 *  Copyright (C) 2009, 2015 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Peter L Jones (pljones@users.sf.net)                                   *
 *  Keyi Zhang                                                             *
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

namespace S4PIDemoFE.Settings
{
    using System.Reflection;
    using s4pi.Interfaces;

    public static class Version
    {
        static readonly string timestamp;
        public static string CurrentVersion { get { return timestamp; } }

        static readonly string libraryTimestamp;
        public static string LibraryVersion { get { return libraryTimestamp; } }

        static Version()
        {
            timestamp = VersionFor(Assembly.GetEntryAssembly());
            libraryTimestamp = VersionFor(typeof(AApiVersionedFields).Assembly);
        }

        private static string VersionFor(Assembly a)
        {
            return "";
        }
    }
}