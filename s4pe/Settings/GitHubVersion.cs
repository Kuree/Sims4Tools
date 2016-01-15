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
    using System;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;

    public class GithubVersion : IComparable<GithubVersion>
    {
        private const string VersionMatchPattern = @"(?<=""tag_name"":"")(\d+\.?)+([_a-zA-Z]*)(?="")";
        private const string GitHubRepoUrl = @"https://api.github.com/repos/s4ptacle/Sims4Tools/releases";

        private readonly string version;

        public GithubVersion(string version)
        {
            this.version = version;
        }

        public GithubVersion()
        {
            var client = new WebClient();
            client.Headers.Add("user-agent", "Only a test!");
            client.Credentials = CredentialCache.DefaultCredentials;
            TextReader tr = new StreamReader(client.OpenRead(GitHubRepoUrl));

            string json = tr.ReadToEnd();
            Match match = Regex.Match(json, VersionMatchPattern, RegexOptions.IgnoreCase);

            this.version = match.Success
                         ? match.Groups[0].Value
                         : "0";
        }

        public int CompareTo(GithubVersion other)
        {
            return string.Compare(this.version, other.version, StringComparison.Ordinal);
        }
    }
}
