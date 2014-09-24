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
using System.Configuration;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

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
#if DEBUG
            string[] v = a.GetName().Version.ToString().Split('.');
            return String.Format("{0}-{1}{2}-{3}", v[0].Substring(0, 2), v[0].Substring(2), v[1], v[2]);
#else
            string version_txt = Path.Combine(Path.GetDirectoryName(PortableSettingsProvider.ExecutablePath), PortableSettingsProvider.ExecutableName + "-Version.txt");
            if (!File.Exists(version_txt))
                return "Unknown";
            using (System.IO.StreamReader sr = new StreamReader(version_txt))
            {
                String line1 = sr.ReadLine();
                sr.Close();
                return line1.Trim();
            }
#endif
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
            if ((pgmSettings.AutoUpdateChoice == 1)
                && (DateTime.UtcNow.Date != pgmSettings.AULastUpdateTS.Date))
            {
                GetUpdate(true);
                pgmSettings.AULastUpdateTS = DateTime.UtcNow; // Only the automated check updates this setting
                pgmSettings.Save();
            }
#endif
        }

        public static bool GetUpdate(bool autoCheck)
        {
            return true;
            //UpdateInfo ui = null;
            //string ini = PortableSettingsProvider.GetApplicationIniFile("Update");
            //if (!File.Exists(ini))
            //{
            //    CopyableMessageBox.Show(
            //        "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
            //        + ini + " - not found"
            //        , PortableSettingsProvider.ExecutableName + " AutoUpdate"
            //        , CopyableMessageBoxButtons.OK
            //        , CopyableMessageBoxIcon.Error);
            //    return true;
            //}
            //try
            //{
            //    string url = new StreamReader(ini).ReadLine();
            //    if (url == null)
            //        throw new IOException(ini + " - failed to read url");
            //    url = url.Trim();
            //    try
            //    {
            //        using (S4PIDemoFE.Splash splash = new S4PIDemoFE.Splash("Checking for updates..."))
            //        {
            //            splash.Show();
            //            Application.DoEvents();
            //            ui = new UpdateInfo(url);
            //        }
            //    }
            //    catch (System.Net.WebException we)
            //    {
            //        if (we != null)
            //        {
            //            CopyableMessageBox.Show(
            //                "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
            //                + (we.Response != null ? "\nURL: " + we.Response.ResponseUri : "")
            //                + "\n" + we.Message
            //                , PortableSettingsProvider.ExecutableName + " AutoUpdate"
            //                , CopyableMessageBoxButtons.OK
            //                , CopyableMessageBoxIcon.Error);
            //            return true;
            //        }
            //    }
            //}
            //catch (IOException ioe)
            //{
            //    CopyableMessageBox.Show(
            //        "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
            //        + ioe.Message
            //        , PortableSettingsProvider.ExecutableName + " AutoUpdate"
            //        , CopyableMessageBoxButtons.OK
            //        , CopyableMessageBoxIcon.Error);
            //    return true;
            //}

            //if (UpdateApplicable(ui, autoCheck))
            //{
            //    int dr = CopyableMessageBox.Show(
            //        String.Format("{0}\n{3}\n\nCurrent version: {1}\nAvailable version: {2}",
            //        ui.Message, Version.CurrentVersion, ui.AvailableVersion, ui.UpdateURL)
            //        , PortableSettingsProvider.ExecutableName + " update available"
            //        , CopyableMessageBoxIcon.Question
            //        , new List<string>(new string[] { "&Visit link", "&Later", "&Skip version", }), 1, 2
            //        );

            //    switch (dr)
            //    {
            //        case 0: System.Diagnostics.Process.Start(ui.UpdateURL); break;
            //        case 2: pgmSettings.AULastIgnoredVsn = ui.AvailableVersion; pgmSettings.Save(); break;
            //    }
            //    return true;
            //}
            //return false;
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
