/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *  2014 by Keyi Zhang                                                     *
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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;

namespace AutoUpdate
{
    public class Version
    {
        static String timestamp;
        public static String CurrentVersion { get { return timestamp; } }

        static String libraryTimestamp;
        public static String LibraryVersion { get { return libraryTimestamp; } }

        static Version()
        {
            timestamp = VersionFor(System.Reflection.Assembly.GetEntryAssembly());
            libraryTimestamp = VersionFor(typeof(s4pi.Interfaces.AApiVersionedFields).Assembly);
        }

        public static String VersionFor(System.Reflection.Assembly a)
        {

            return "";
        }
    }
    public class UpdateInfo
    {
        Dictionary<string, string> pgmUpdate = new Dictionary<string, string>();
        public String AvailableVersion { get { return pgmUpdate["Version"]; } }
        public String UpdateURL { get { return pgmUpdate["UpdateURL"]; } }
        public String Message { get { return pgmUpdate["Message"]; } }
        public bool Reset { get { bool res = false; return pgmUpdate.ContainsKey("Reset") && bool.TryParse(pgmUpdate["Reset"], out res) ? res : false; } }

        public UpdateInfo(String url)
        {
            TextReader tr = new StreamReader(new System.Net.WebClient().OpenRead(url));
            string line1 = tr.ReadLine().Trim();
            bool reset = false;
            if (bool.TryParse(line1, out reset))
            {
                pgmUpdate.Add("Reset", reset ? Boolean.TrueString : Boolean.FalseString);
                pgmUpdate.Add("Version", tr.ReadLine().Trim());
            }
            else
            {
                pgmUpdate.Add("Version", line1);
            }
            pgmUpdate.Add("UpdateURL", tr.ReadLine().Trim());
            pgmUpdate.Add("Message", tr.ReadToEnd().Trim());
            tr.Close();
        }
    }

    public class GithubVersion : IComparable<GithubVersion>
    {
        public int MajorVersion { get; private set; }
        public char MinorVersion { get; private set; }

        public GithubVersion(string version)
        {
            this.AssignVersionFromString(version);
        }

        public GithubVersion()
        {
            string updateURL = "https://api.github.com/repos/Kuree/Sims4Tools/releases";
            var client = new System.Net.WebClient();
            client.Headers.Add("user-agent", "Only a test!");
            client.Credentials = CredentialCache.DefaultCredentials;
            TextReader tr = new StreamReader(client.OpenRead(updateURL));
            string rawJSON = tr.ReadToEnd();
            Regex r = new Regex("tag_name\\\":\\\"([0-9].[0-9][a-z])");
            var match = r.Matches(rawJSON);
            this.AssignVersionFromString(match[0].Groups[1].Value);
        }

        private void AssignVersionFromString(string version)
        {
            Regex r = new Regex("([0-9][0-9])([a-z])");
            var match = r.Match(version.Replace(".", ""));
            if (match.Groups.Count < 2)
            {
                MajorVersion = 0;
                MinorVersion = 'a';
            }
            else
            {
                try
                {
                    var test = match.Groups[1].Value;
                    MajorVersion = int.Parse(match.Groups[1].Value);
                    MinorVersion = char.Parse(match.Groups[2].Value);
                }
                catch
                {
                    MajorVersion = 0;
                    MinorVersion = 'a';
                }
            }
        }

        public int CompareTo(GithubVersion other)
        {
            if(other.MajorVersion != this.MajorVersion)
            {
                return this.MajorVersion.CompareTo(other.MajorVersion);
            }
            else
            {
                return this.MinorVersion.CompareTo(other.MinorVersion);
            }
        }
    }

    public class Checker
    {
        static S4PIDemoFE.Properties.Settings pgmSettings = S4PIDemoFE.Properties.Settings.Default;

        static Checker()
        {
            // Should only be set to "AskMe" the first time through (although it might have been reset by the user)
            if (pgmSettings.AutoUpdateChoice == 0) // AskMe
            {
                int dr = CopyableMessageBox.Show(
                    PortableSettingsProvider.ExecutableName + " is under development.\n"
                    + "It is recommended you allow automated update checking\n"
                    + "(no more than once per day, when the program is run).\n\n"
                    + "Do you want " + PortableSettingsProvider.ExecutableName + " to check for updates automatically?"
                    , PortableSettingsProvider.ExecutableName + " AutoUpdate Setting"
                    , CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, -1, 1
                );
                if (dr == 0)
                    AutoUpdateChoice = true; // Daily
                else
                {
                    AutoUpdateChoice = false; // Manual
                    CopyableMessageBox.Show("You can enable AutoUpdate checking under the Settings Menu.\n" +
                        "Manual update checking is under the Help Menu."
                        , PortableSettingsProvider.ExecutableName + " AutoUpdate Setting"
                        , CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information
                    );
                }
                pgmSettings.Save();
                OnAutoUpdateChoice_Changed();
            }
        }

        public static void Daily()
        {
#if DEBUGUPDATE
            GetUpdate(true);
#else
            // Since it is an early stage development
            // Forces user to update to latest version
            if (DateTime.UtcNow.Date != pgmSettings.AULastUpdateTS.Date)
            {
                GetUpdate(true);
                pgmSettings.AULastUpdateTS = DateTime.UtcNow; // Only the automated check updates this setting
                pgmSettings.Save();
            }
#endif
        }

        public static bool GetUpdate(bool autoCheck)
        {
            string versionIni = "version.ini";
            bool hasUpdate = false;

            if (!File.Exists(versionIni))
            {
                CopyableMessageBox.Show(
                    "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
                    + versionIni + " - not found"
                    , PortableSettingsProvider.ExecutableName + " AutoUpdate"
                    , CopyableMessageBoxButtons.OK
                    , CopyableMessageBoxIcon.Error);
                return true;
            }

            try
            {
                string currentVersion = new StreamReader(versionIni).ReadLine();
                currentVersion = currentVersion.Trim();
                GithubVersion s4peVersion = new GithubVersion(currentVersion);
                GithubVersion currentGithubVersion = new GithubVersion();
                hasUpdate = s4peVersion.CompareTo(currentGithubVersion) < 0;
            }
            catch (System.Net.WebException we)
            {
                if (we != null)
                {
                    CopyableMessageBox.Show(
                        "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
                        + (we.Response != null ? "\nURL: " + we.Response.ResponseUri : "")
                        + "\n" + we.Message
                        , PortableSettingsProvider.ExecutableName + " AutoUpdate"
                        , CopyableMessageBoxButtons.OK
                        , CopyableMessageBoxIcon.Error);
                    return true;
                }
            }
            catch (IOException ioe)
            {
                CopyableMessageBox.Show(
                    "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
                    + ioe.Message
                    , PortableSettingsProvider.ExecutableName + " AutoUpdate"
                    , CopyableMessageBoxButtons.OK
                    , CopyableMessageBoxIcon.Error);
                return true;
            }

            if (hasUpdate)
            {
                int dr = CopyableMessageBox.Show(
                        String.Format("New version available",
                        "Click Visit lick will open the default browser"
                        , CopyableMessageBoxIcon.Question
                        , new List<string>(new string[] { "&Visit link", "&Later", "Cancel"})
                        ));

                if(dr == 0)
                {
                    System.Diagnostics.Process.Start(@"https://github.com/Kuree/Sims4Tools/releases");
                }
                return true;
            }
            return false;

        }

        private static bool UpdateApplicable(UpdateInfo ui, bool autoCheck)
        {
            if (ui.AvailableVersion.CompareTo(Version.CurrentVersion) <= 0)
                return false;

            if (ui.Reset && ui.AvailableVersion.CompareTo(pgmSettings.AULastIgnoredVsn) != 0)
                pgmSettings.AULastIgnoredVsn = Version.CurrentVersion;

            if (autoCheck && ui.AvailableVersion.CompareTo(pgmSettings.AULastIgnoredVsn) <= 0)
                return false;

            return true;
        }

        public static event EventHandler AutoUpdateChoice_Changed;
        protected static void OnAutoUpdateChoice_Changed() { if (AutoUpdateChoice_Changed != null) AutoUpdateChoice_Changed(pgmSettings, EventArgs.Empty); }
        public static bool AutoUpdateChoice { get { return pgmSettings.AutoUpdateChoice == 1; } set { pgmSettings.AutoUpdateChoice = value ? 1 : 2; OnAutoUpdateChoice_Changed(); } }
    }
}
