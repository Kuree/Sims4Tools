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
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Windows.Forms;

    public static class UpdateChecker
    {
        private const string DownloadPage = @"https://github.com/pboxx/Sims4Tools/releases";
        private static readonly Properties.Settings settings = Properties.Settings.Default;

        static UpdateChecker()
        {
            // Should only be set to "AskMe" the first time through (although it might have been reset by the user)
            if (settings.AutoUpdateChoice == 0) // AskMe
            {
                int dialogResult = CopyableMessageBox.Show(
                    PortableSettingsProvider.ExecutableName + " is under development.\n"
                    + "It is recommended you allow automated update checking\n"
                    + "(no more than once per day, when the program is run).\n\n"
                    + "Do you want " + PortableSettingsProvider.ExecutableName + " to check for updates automatically?"
                    , PortableSettingsProvider.ExecutableName + " AutoUpdate Setting"
                    , CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, -1, 1
                    );
                if (dialogResult == 0)
                    AutoUpdateChoice = true; // DailyCheck
                else
                {
                    AutoUpdateChoice = false; // Manual
                    CopyableMessageBox.Show("You can enable AutoUpdate checking under the Settings Menu.\n" +
                                            "Manual update checking is under the Help Menu."
                        , PortableSettingsProvider.ExecutableName + " AutoUpdate Setting"
                        , CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information
                        );
                }
                settings.Save();
                OnAutoUpdateChoice_Changed();
            }
        }

        public static void DailyCheck()
        {
#if DEBUGUPDATE
            GetUpdate(true);
#else
            if (DateTime.UtcNow > settings.LastUpdateTimeStamp.AddDays(1))
            {
                GetUpdate(true);
                settings.LastUpdateTimeStamp = DateTime.UtcNow; // Only the automated check updates this setting
                settings.Save();
            }
#endif
        }

        public static bool GetUpdate(bool autoCheck)
        {
            string versionIni = "version.ini";
            bool hasUpdate;

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
                GithubVersion localVersion = new GithubVersion(currentVersion);
                GithubVersion githubVersion = new GithubVersion();
                hasUpdate = localVersion.CompareTo(githubVersion) < 0;
            }
            catch (WebException we)
            {
                ShowErrorMessage(autoCheck, we);
                return true;
            }
            catch (IOException ioe)
            {
                ShowErrorMessage(autoCheck, ioe);
                return true;
            }

            if (hasUpdate)
            {
                string message = "New version available\nDo you want to visit the download site now?";
                int dialogResult = CopyableMessageBox.Show(message, "", CopyableMessageBoxButtons.YesNo);

                if (dialogResult == 0)
                {
                    Process.Start(DownloadPage);
                }
                return true;
            }
            return false;
        }

        private static void ShowErrorMessage(bool autoCheck, Exception exception)
        {
            CopyableMessageBox.Show(
                "Problem checking for update" + (autoCheck ? " - will try again later" : "") + "\n"
                + exception.Message
                , PortableSettingsProvider.ExecutableName + " AutoUpdate"
                , CopyableMessageBoxButtons.OK
                , CopyableMessageBoxIcon.Error);
        }

        public static event EventHandler AutoUpdateChoiceChanged;

        private static void OnAutoUpdateChoice_Changed()
        {
            if (AutoUpdateChoiceChanged != null) AutoUpdateChoiceChanged(settings, EventArgs.Empty);
        }

        public static bool AutoUpdateChoice
        {
            get { return settings.AutoUpdateChoice == 1; }
            set
            {
                settings.AutoUpdateChoice = value ? 1 : 2;
                OnAutoUpdateChoice_Changed();
            }
        }
    }
}