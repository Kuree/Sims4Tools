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
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace s4pi.Filetable
{
    /// <summary>
    /// Provides access to the XML definition of game folder default locations and
    /// the resultant list of known games.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When using this class - which is depended upon by <see cref="Filetable"/> - ensure
    /// the file <c>GameFolders.xml</c> is present in the same folder as the assembly containing this class as it
    /// is used to specify a number of configuration values.
    /// These are as follows:
    /// </para>
    /// <list type="table">
    /// <listheader><term><c>GameFolders.xml</c> tag</term><description>Description</description></listheader>
    /// <item><term><c>&lt;gamefolders&gt;</c></term><description>Root element of the file, defines the XML namespace used.</description></item>
    /// <item><term><c>&lt;defaultrootfolder&gt;</c></term><description>The folder to use when no <c>&lt;rootfolder&gt;</c> tag applies.
    /// If this value is also not found, a value of <c>/</c> is used.
    /// </description></item>
    /// <item><term><c>&lt;rootfolder&gt;</c></term>
    /// <description>A search is performed for a matching entry based on the <c>vendor</c> and <c>os</c> attributes:
    /// <list type="table">
    /// <listheader><term>attribute</term><description>values</description></listheader>
    /// <item><term>vendor</term><description>
    /// <see cref="Environment"/><c cref="OperatingSystem">.OSVersion</c> (see <see cref="OperatingSystem"/>) <c cref="PlatformID">.Platform</c> (see <see cref="PlatformID"/>)
    /// is used to determine whether the vendor is <c>MacOSX</c>, <c>Unix</c> or (otherwise) <c>Microsoft</c>.
    /// </description></item>
    /// <item><term>os</term>
    /// <description><c>64bit</c>, or <c>32bit</c> when the size of <see cref="T:System.IntPtr"/> is 4 bytes.</description>
    /// </item>
    /// </list>
    /// </description>
    /// </item>
    /// <item><term><c>&lt;game&gt;</c></term><description>
    /// <para>This is a container for details about a specific Sims3 game.  It has two attributes:</para>
    /// <list type="table">
    /// <listheader><term>attribute</term><description>values</description></listheader>
    /// <item><term>class</term><description><c>baseGame</c> (the base game itself), <c>EP</c> (a full expansion pack) or <c>SP</c> (a stuff pack).
    /// The value is not used.</description></item>
    /// <item><term>rgversion</term><description>Exposed as <see cref="Game.RGVersion"/> and used internally (e.g. to determine where delta build packages are found)
    /// as well as by the <see cref="Filetable"/>.  The value is expected to match the top five "version" bits of the
    /// <see cref="s4pi.Interfaces.IResourceKey.ResourceGroup"/> (hence RGVersion) of certain resources.</description></item>
    /// </list>
    /// <para>The following tags appear for each game:</para>
    /// <list type="table">
    /// <listheader><term><c>&lt;game&gt;</c> section tag</term><term><see cref="Game"/> property</term><description>Description</description></listheader>
    /// <item><term><c>&lt;Name&gt;</c></term><term><see cref="Game.Name"/></term><description>The identity of the game, used for the list of EPs Disabled (see <see cref="EPsDisabled"/>).</description></item>
    /// <item><term><c>&lt;Longname&gt;</c></term><term><see cref="Game.Longname"/></term><description>A descriptive name for the section.</description></item>
    /// <item><term><c>&lt;DefaultInstallDir&gt;</c></term><term><see cref="Game.DefaultInstallDir"/></term><description>(English-language) default install folder for the game.</description></item>
    /// <item><term><c>&lt;Suppressed&gt;</c></term><term><see cref="Game.Suppressed"/></term>
    /// <description>Whether the game is disabled by default; the value <c>not-allowed</c> means the game will always be enabled.</description></item>
    /// <item><term><c>&lt;ExtraPackage&gt;</c></term><term><c> </c></term>
    /// <description>Optional tag to allow extra an EA content package to be added for this <see cref="Game"/> that is otherwise not found.
    /// Multiple occurrences are supported.
    /// It is expected only to be required if EA change the naming scheme and a library update has not been issued to cater for the change.
    /// This is not directly exposed but all values are prepended to <see cref="Game.GameContent"/>, <see cref="Game.DDSImages"/> and <see cref="Game.Thumbnails"/>
    /// - see <see cref="Game"/> remarks for more details.
    /// </description></item>
    /// </list>
    /// </description></item>
    /// </list>
    /// <para>
    /// The "root folder" for the installation is determined by:
    /// </para>
    /// <list type="bullet">
    /// <item>finding a matching &lt;rootfolder&gt; tag; or</item>
    /// <item>finding the &lt;defaultrootfolder&gt; tag exists and is a valid folder; or</item>
    /// <item>using "/".</item>
    /// </list>
    /// <para>
    /// The EA packages for a <see cref="Game"/> are found relative to the folder either specified in <see cref="InstallDirs"/>,
    /// or the &lt;DefaultInstallDir&gt; tag value for the &lt;game&gt; - i.e. the value returned by <see cref="Game.UserInstallDir"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="Game"/>
    /// <seealso cref="GameFoldersForm"/>
    public static class GameFolders
    {
        #region The first level element/attribute names...
        static readonly string ns = "{http://sims3.drealm.info/terms/gamefolders/1.0}";
        static readonly string docXName = ns + "gamefolders";
        static readonly string defaultrootfolderXName = ns + "defaultrootfolder";
        static readonly string rootfolderXName = ns + "rootfolder";
        static readonly string vendorAttributeName = "vendor";
        static readonly string osAttributeName = "os";
        static readonly string gameXName = ns + "game";
        #endregion

        static XDocument _gameFoldersXml;
        static XDocument GameFoldersXML
        {
            get
            {
                if (_gameFoldersXml == null)
                {
                    XDocument gameFoldersXml;
                    string iniFile = Path.Combine(Path.GetDirectoryName(typeof(GameFolders).Assembly.Location), "GameFolders.xml");
                    gameFoldersXml = XDocument.Load(iniFile);

                    _gameFoldersXml = gameFoldersXml;
                }
                return _gameFoldersXml;
            }
        }
        static XElement XElementGameFolders { get { return GameFoldersXML.Element(docXName); } }

        static string _rootFolder = null;
        internal static string RootFolder
        {
            get
            {
                if (_rootFolder == null)
                {
                    string rootFolder = "/";
                    string vendor = "Microsoft";
                    string os = "64bit";

                    if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
                        vendor = Environment.OSVersion.Platform.ToString();

                    if (System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)) == 4)
                        os = "32bit";

                    XElement root = XElementGameFolders.Elements(rootfolderXName)
                        .Where(x => x.Attributes(vendorAttributeName).Any(y => y.Value == vendor))
                        .Where(x => x.Attributes(osAttributeName).Any(y => y.Value == os))
                        .FirstOrDefault();

                    if (root == null)
                        root = XElementGameFolders.Elements(defaultrootfolderXName).FirstOrDefault();

                    if (root != null && root.Value != null)
                        rootFolder = root.Value;

                    _rootFolder = rootFolder;
                }
                return _rootFolder;
            }
        }

        static List<Game> _games = null;
        /// <summary>
        /// The list of <see cref="Game"/>s defined in the <c>GameFolders.xml</c> file.
        /// </summary>
        public static List<Game> Games
        {
            get
            {
                if (_games == null)
                    _games = XElementGameFolders.Elements(gameXName).Select(g => new Game(g)).ToList();
                return _games;
            }
        }

        /// <summary>
        /// Return the <see cref="Game"/> with a <c>Name</c> element with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">String to match again <see cref="Game"/>s' <c>Name</c> elements.</param>
        /// <returns>The (first) <see cref="Game"/> with a <c>Name</c> element with the specified <paramref name="value"/>
        /// or <c>null</c> if none found.</returns>
        public static Game byName(string value) { return Games.Where(x => x.Name == value).FirstOrDefault(); }

        /// <summary>
        /// Return the <see cref="Game"/> with a <c>RGVersion</c> element with the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Number to match again <see cref="Game"/>s' <c>RGVersion</c> elements.</param>
        /// <returns>The (first) <see cref="Game"/> with a <c>RGVersion</c> element with the specified <paramref name="value"/>
        /// or <c>null</c> if none found.</returns>
        public static Game byRGVersion(int value) { return Games.Where(x => x.RGVersion == value).FirstOrDefault(); }

        private static Dictionary<Game, string> installDirs = new Dictionary<Game,string>();
        /// <summary>
        /// A semi-colon delimited string of game name / install folder pairs, internally delimited by an equals sign.
        /// </summary>
        public static string InstallDirs
        {
            get { return string.Join(";", installDirs.Select(kvp => kvp.Key.Name + "=" + kvp.Value).ToArray()); }
            set
            {
                if (value == null) installDirs = new Dictionary<Game, string>();
                else installDirs = value.Split(';')
                    .Select(xy => xy.Split('='))
                    .Where(xy =>
                        xy.Length == 2 &&
                        GameFolders.byName(xy[0]) != null &&
                        Directory.Exists(xy[1]) &&
                        Path.GetFullPath(xy[1]) != GameFolders.byName(xy[0]).InstallDir
                        )
                    .ToDictionary(xy => GameFolders.byName(xy[0]), xy => xy[1]);
            }
        }

        /// <summary>
        /// Return the folder where the given <see cref="Game"/> is installed.
        /// </summary>
        /// <param name="game"><see cref="Game"/> for which to determine install folder.</param>
        /// <returns>The install folder for <paramref name="game"/>.</returns>
        /// <remarks>
        /// This will either be a user-specified location or the <see cref="Game.DefaultInstallDir"/>
        /// relative to the determined root folder (see <see cref="GameFolders"/> Remarks section).
        /// </remarks>
        public static string InstallDir(Game game)
        {
            return installDirs.ContainsKey(game) && Directory.Exists(installDirs[game]) ? installDirs[game] : game.InstallDir;
        }

        static List<Game> gamesEnabled = null;
        /// <summary>
        /// A semi-colon delimited string of EP and SP names that the user has stated should not be referenced by the vendor part of the
        /// <see cref="FileTable"/> view.
        /// </summary>
        /// <seealso cref="FileTable"/>
        public static string EPsDisabled
        {
            get { return string.Join(";", GameFolders.Games.Where(g => !gamesEnabled.Contains(g)).Select(g => g.Name).ToArray()); }
            set
            {
                string[] split = value.Split(';').Distinct().ToArray();
                gamesEnabled = new List<Game>(GameFolders.Games.Where(g => !g.Suppressed.HasValue || !split.Contains(g.Name)));
            }
        }

        /// <summary>
        /// Returns whether a specified <see cref="Game"/> is enabled.
        /// </summary>
        /// <param name="game">The <see cref="Game"/> to query.</param>
        /// <returns>Whether a specified <see cref="Game"/> is enabled.</returns>
        /// <remarks>A game is enabled if<br/>
        /// (a) Disabling it is disallowed;<br/>
        /// (b) Or no games are user-disabled (see <see cref="EPsDisabled"/>) and it is not suppressed;<br/>
        /// (c) Or it is not one of the user-disabled games (see <see cref="EPsDisabled"/>).</remarks>
        public static bool IsEnabled(Game game) { return !game.Suppressed.HasValue || (gamesEnabled != null ? gamesEnabled.Contains(game) : !game.Suppressed.Value); }
    }

    /// <summary>
    /// Represents a known game object.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="GameContent"/>, <see cref="DDSImages"/> and <see cref="Thumbnails"/> all return enumerations of paths based on
    /// EA's known behaviour for distributing EPs and SPs.  If this changes to a method not supported (see below), the
    /// &lt;ExtraPackage&gt; tag for the &lt;game&gt; can be used - see <see cref="GameFolders"/> remarks for details.
    /// </para>
    /// <para>
    /// For <see cref="GameContent"/> and <see cref="DDSImages"/>, one of the following values is returned:
    /// </para>
    /// <list type="bullet">
    /// <item>If no <see cref="UserInstallDir"/> is known, return only the &lt;ExtraPackage&gt; paths.</item>
    /// <item>Otherwise, return the &lt;ExtraPackage&gt; paths, delta packages and the appropriate fullbuild (0 for <see cref="GameContent"/>; 2 for <see cref="DDSImages"/>).</item>
    /// </list>
    /// <para>
    /// Delta packages are found under the <see cref="UserInstallDir"/> + "GameData\Shared\DeltaPackages".
    /// This is searched for each sub-folder named "pXX" (where "XX" is the deltabuild number - assumed for the timebeing to have 32 as the highest value).
    /// Within each sub-folder, the package named "Deltabuild_pXX.package" is added to the list, if found.
    /// </para>
    /// <para>
    /// Finally, under the <see cref="UserInstallDir"/> + "GameData\Shared\Packages" folder:
    /// </para>
    /// <list type="bullet">
    /// <item>For games other than the basegame, any packages matching "FullBuild*.package" are added to the list.</item>
    /// <item>For the basegame, DeltaBuildX.package and FullBuildX.package (where X is 0 for <see cref="GameContent"/>; 2 for <see cref="DDSImages"/>) are added to the list.</item>
    /// </list>
    /// <para>
    /// For <see cref="Thumbnails"/>, one of the following values is returned:
    /// </para>
    /// <list type="bullet">
    /// <item>If no <see cref="UserInstallDir"/> is known, return only the &lt;ExtraPackage&gt; paths.</item>
    /// <item>Otherwise, return the &lt;ExtraPackage&gt; paths and any packages matching "*Thumbnails.package" under <see cref="UserInstallDir"/> + "Thumbnails".</item>
    /// </list>
    /// </remarks>
    public class Game
    {
        XElement _game;
        string ns = "";

        /// <summary>
        /// Create a game object from the supplied <see cref="XElement"/>.
        /// </summary>
        /// <param name="game">An <see cref="XElement"/> describing the game.</param>
        public Game(XElement game)
        {
            _game = game;
            if (_game.GetDefaultNamespace().NamespaceName.Length > 0)
                ns = "{" + _game.GetDefaultNamespace().NamespaceName + "}";
        }

        int _rgversion = -1;
        /// <summary>
        /// The ResourceGroup Version number for the <see cref="Game"/>.
        /// </summary>
        public int RGVersion
        {
            get
            {
                if (_rgversion == -1)
                {
                    int rgversion = 0;
                    XAttribute XArgversion = _game.Attribute("rgversion");
                    if (XArgversion == null || XArgversion.Value == null) rgversion = 0;
                    else if (!int.TryParse(XArgversion.Value, out rgversion)) rgversion = 0;
                    if (rgversion < 0) rgversion = 0;
                    _rgversion = rgversion;
                }
                return _rgversion;
            }
        }

        int _priority = -1;
        /// <summary>
        /// The Priority for the <see cref="Game"/>.  Defaults to RGVersion if not specified.
        /// </summary>
        public int Priority
        {
            get
            {
                if (_priority == -1)
                {
                    int priority = 0;
                    XAttribute XArgversion = _game.Attribute("priority");
                    if (XArgversion == null || XArgversion.Value == null) priority = RGVersion;
                    else if (!int.TryParse(XArgversion.Value, out priority)) priority = RGVersion;
                    if (priority < 0) priority = RGVersion;
                    _priority = priority;
                }
                return _priority;
            }
        }

        string getElement(string elementName, string defaultValue)
        {
            XElement xe = _game.Element(ns + elementName);
            if (xe != null && xe.Value != null && xe.Value.Length > 0)
                defaultValue = xe.Value;
            return defaultValue;
        }

        string _name = null;
        /// <summary>
        /// The name by which the game is known for settings purposes.
        /// </summary>
        public string Name
        {
            get
            {
                if (_name == null) _name = getElement("Name", "Unk");
                return _name;
            }
        }

        string _longname = null;
        /// <summary>
        /// The full name for the game.
        /// </summary>
        public string Longname
        {
            get
            {
                if (_longname == null) _longname = getElement("Longname", "Unknown");
                return _longname;
            }
        }

        string _guid = null;
        /// <summary>
        /// Where known, the GUID for an EP/SP, used in the Windows Registry.
        /// </summary>
        /// <example>
        /// The following registry key is The Sims™ 3 High-End Loft Stuff:
        /// <c>HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\GameUX\Games\{71828142-5A24-4BD0-97E7-976DA08CE6CF}</c>
        /// </example>
        public string GUID
        {
            get
            {
                if (_guid == null) _guid = getElement("GUID", "");
                return _guid;
            }
        }

        /// <summary>
        /// Returns the ConfigApplicationPath Windows registry value for this EP/SP (or null).
        /// </summary>
        public string ConfigApplicationPath
        {
            get
            {
                if (GUID == "") return null;
                string key = "SOFTWARE" + ((System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr)) != 4) ? @"\Wow6432Node" : "") + @"\Microsoft\Windows\CurrentVersion\Uninstall\" + GUID;
                using (var rkGame = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(key))
                {
                    if (rkGame == null) return null;
                    return rkGame.GetValue("InstallLocation") as string;
                }
            }
        }

        string _defaultInstallDir = null;
        /// <summary>
        /// The default installation location, relative to the determined root folder.
        /// </summary>
        /// <remarks>See <see cref="GameFolders"/> remarks for details of how "root folder" is determined.</remarks>
        public string DefaultInstallDir
        {
            get
            {
                if (_defaultInstallDir == null) _defaultInstallDir = getElement("DefaultInstallDir", "/");
                return _defaultInstallDir;
            }
        }

        string _installDir = null;
        /// <summary>
        /// The full path to where we think the EP/SP is installed unless overriden.
        /// </summary>
        public string InstallDir
        {
            get
            {
                if (_installDir == null)
                {
                    _installDir = ConfigApplicationPath;
                    if (_installDir == null)
                    {
                        _installDir = Path.Combine(GameFolders.RootFolder, DefaultInstallDir);
                    }
                    _installDir = Path.GetFullPath(_installDir);
                }
                return _installDir;
            }
        }

        /// <summary>
        /// The <see cref="GameFolders.InstallDir"/> for this <see cref="Game"/>.
        /// </summary>
        public string UserInstallDir { get { return GameFolders.InstallDir(this); } }

        int? _suppresed = null;
        /// <summary>
        /// Whether the supplied XML indicates this game is suppressed.
        /// </summary>
        /// <remarks>The value is null if the user should not be able to disable this game.</remarks>
        public bool? Suppressed
        {
            get
            {
                if (!_suppresed.HasValue)
                {
                    string suppresed = getElement("Suppressed", "false");
                    switch (suppresed)
                    {
                        case "true": _suppresed = 1; break;
                        case "not-allowed": _suppresed = -1; break;
                        default: _suppresed = 0; break;
                    }
                }
                return _suppresed == -1 ? (bool?)null : _suppresed != 0;
            }
        }

        /// <summary>
        /// The <see cref="GameFolders.IsEnabled"/> state for this <see cref="Game"/>.
        /// </summary>
        public bool Enabled { get { return GameFolders.IsEnabled(this); } }

        /// <summary>
        /// All <see cref="PackageTag"/>s for this <see cref="Game"/> used to contain game content
        /// (i.e. excluding DDS images and thumbnail images).
        /// </summary>
        /// <seealso cref="DDSImages"/>
        /// <seealso cref="Thumbnails"/>
        public IEnumerable<PackageTag> GameContent { get { return GetPackages("GameContent").Distinct(PackageComparer).OrderByDescending(p => p.Priority); } }

        /// <summary>
        /// All <see cref="PackageTag"/>s for this <see cref="Game"/> used to contain DDS images.
        /// </summary>
        /// <seealso cref="DDSImages"/>
        /// <seealso cref="Thumbnails"/>
        public IEnumerable<PackageTag> DDSImages { get { return GetPackages("DDSImages").Distinct(PackageComparer).OrderByDescending(p => p.Priority); } }

        /// <summary>
        /// All <see cref="PackageTag"/>s for this <see cref="Game"/> used to contain Thumbnail images.
        /// </summary>
        /// <seealso cref="GameContent"/>
        /// <seealso cref="DDSImages"/>
        public IEnumerable<PackageTag> Thumbnails { get { return GetPackages("Thumbnails").Distinct(PackageComparer).OrderByDescending(p => p.Priority); } }

        internal class PackageEqual : IEqualityComparer<PackageTag>
        {
            public bool Equals(PackageTag x, PackageTag y) { return Path.GetFullPath(x.Path) == Path.GetFullPath(y.Path); }
            public int GetHashCode(PackageTag obj) { return obj.Path.GetHashCode(); }
        }
        internal static PackageEqual PackageComparer = new PackageEqual();

        IEnumerable<PackageTag> GetPackages(string root)
        {
            XElement xRoot = _game.Element(ns + root);
            string path = "";
            foreach (XElement xFolder in xRoot.Elements(ns + "Folder"))
            {
                XAttribute xPath = xFolder.Attribute("path");
                if (xPath != null && xPath.Value != null)
                    path = Path.Combine(UserInstallDir, xPath.Value);
                else
                    path = UserInstallDir;
                if (Directory.Exists(path))
                    foreach (PackageTag p in GetPackages(xFolder, path).Where(p => p.Priority >= 0))
                        yield return p;
            }
            if (Directory.Exists(UserInstallDir))
                foreach (PackageTag p in GetPackages(xRoot, UserInstallDir))
                    yield return p;
        }

        IEnumerable<PackageTag> GetPackages(XElement e, string path)
        {
            foreach (XElement xPackage in e.Elements(ns + "Package"))
            {
                if (xPackage.Value != null && File.Exists(Path.Combine(path, xPackage.Value)))
                {
                    bool isPatch = false;
                    XAttribute xIsPatch = xPackage.Attribute("IsPatch");
                    if (xIsPatch != null && xIsPatch.Value != null)
                        bool.TryParse(xIsPatch.Value, out isPatch);

                    int priority = 0;
                    XAttribute xPriority = xPackage.Attribute("Priority");
                    if (xPriority != null && xPriority.Value != null)
                        int.TryParse(xPriority.Value, out priority);

                    yield return new PackageTag(Path.Combine(path, xPackage.Value), isPatch, priority);
                }
            }
        }
    }

    /// <summary>
    /// Package represents the <c>&lt;Package/&gt;</c> tag.
    /// </summary>
    public class PackageTag
    {
        /// <summary>
        /// The full path of package specified by the <c>&lt;Package&gt;</c> tag.
        /// </summary>
        /// <value>The full path of package specified by the &lt;Package&gt; tag.</value>
        public string Path { get; private set; }

        /// <summary>
        /// True if this <see cref="PackageTag"/> belongs to a patch; otherwise false.
        /// </summary>
        /// <value>True if this <see cref="PackageTag"/> belongs to a patch; otherwise false.</value>
        /// <remarks>Patch packages may be missing for a particular game.</remarks>
        public bool IsPatch { get; private set; }

        /// <summary>
        /// Used to sort packages; higher priority packages override the content of lower priority packages.
        /// </summary>
        /// <value>Used to sort packages; higher priority packages override the content of lower priority packages.</value>
        public int Priority { get; private set; }

        /// <summary>
        /// Create a new <see cref="PackageTag"/> with the given values.
        /// </summary>
        /// <param name="path">The full path of package specified by the <c>&lt;Package&gt;</c> tag.</param>
        /// <param name="isPatch">True if this <see cref="PackageTag"/> belongs to a patch; otherwise false.</param>
        /// <param name="priority">Used to sort packages; higher priority packages override the content of lower priority packages.</param>
        public PackageTag(string path, bool isPatch, int priority) { Path = path; IsPatch = isPatch; Priority = priority; }
    }
}