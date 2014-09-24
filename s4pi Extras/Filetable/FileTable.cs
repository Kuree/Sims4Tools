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
using s4pi.Interfaces;

namespace s4pi.Filetable
{
    /// <summary>
    /// Support providing a view over user ("custom") content, vendor content and the current package.
    /// </summary>
    public static class FileTable
    {
        /// <summary>
        /// Property containing the root folder for user content; searched recursively for
        /// files with a ".package" extension.
        /// </summary>
        public static string CustomContentPath { get; set; }

        /// <summary>
        /// When true, packages in <see cref="CustomContentPath"/> will be included in the FileTable view.
        /// </summary>
        public static bool CustomContentEnabled { get; set; }

        static List<PathPackageTuple> customContent = null;
        /// <summary>
        /// A list of all the <see cref="PathPackageTuple"/>s in <see cref="CustomContentPath"/>.
        /// </summary>
        public static List<PathPackageTuple> CustomContent
        {
            get
            {
                if (customContent == null)
                    customContent = ccGetList(CustomContentPath).ToList();
                return customContent;
            }
        }

        /* ----------------------------------------------- */

        /// <summary>
        /// When true, vendor packages will be included in the FileTable view (subject to <see cref="GameFolders.EPsDisabled"/>).
        /// </summary>
        public static bool FileTableEnabled { get; set; }

        /* ----------------------------------------------- */

        /// <summary>
        /// The current <see cref="PathPackageTuple"/>.  When not <c>null</c>, include in the FileTable view.
        /// </summary>
        public static PathPackageTuple Current { get; set; }

        /* ----------------------------------------------- */

        static FT gameContent = new GameContent();
        /// <summary>
        /// List of primary game content <see cref="PathPackageTuple"/>s comprising
        /// the current package (if set), custom content (if enabled) and vendor content;
        /// excludes vendor DDS images (see <see cref="DDSImages"/>)
        /// and thumbnail images (see <see cref="Thumbnails"/>).
        /// </summary>
        /// <seealso cref="DDSImages"/>
        /// <seealso cref="Thumbnails"/>
        public static List<PathPackageTuple> GameContent { get { return getList(gameContent); } }

        static FT ddsImages = new DDSImages();
        /// <summary>
        /// List of DDS image <see cref="PathPackageTuple"/>s comprising
        /// the current package (if set), custom content (if enabled) and vendor content.
        /// </summary>
        /// <seealso cref="GameContent"/>
        /// <seealso cref="Thumbnails"/>
        public static List<PathPackageTuple> DDSImages { get { return getList(ddsImages); } }

        static FT thumbnails = new Thumbnails();
        /// <summary>
        /// List of thumnail image <see cref="PathPackageTuple"/>s, comprising
        /// the current package (if set), custom content (if enabled) and vendor content.
        /// </summary>
        /// <seealso cref="GameContent"/>
        /// <seealso cref="DDSImages"/>
        public static List<PathPackageTuple> Thumbnails { get { return getList(thumbnails); } }

        static List<PathPackageTuple> getList(FT which)
        {
            List<PathPackageTuple> res = new List<PathPackageTuple>();
            if (Current != null) { res.Add(Current); }
            if (CustomContentEnabled && CustomContent != null) { res.AddRange(CustomContent); }
            if (FileTableEnabled && which.VendorContent != null) { res.AddRange(which.VendorContent); }
            return res.Count == 0 ? null : res;
        }

        /* ----------------------------------------------- */

        /// <summary>
        /// Clears file table references to packages.<br/>
        /// Note that this does not close the current package if it had been referenced.
        /// </summary>
        public static void Reset()
        {
            Current = null;
            CustomContentEnabled = false;
            customContent = null;
            FileTableEnabled = false;
            gameContent.Reset();
            ddsImages.Reset();
            thumbnails.Reset();
        }

        /// <summary>
        /// Indicates that there is data available in the FileTable
        /// </summary>
        public static bool IsOK { get { return GameContent != null && DDSImages != null && Thumbnails != null && GameContent.Count > 0; } }

        /// <summary>
        /// Determine the equality of two lists of <see cref="PathPackageTuple"/>s.
        /// </summary>
        /// <param name="ppt1">The first list of <see cref="PathPackageTuple"/>s.</param>
        /// <param name="ppt2">The second list of <see cref="PathPackageTuple"/>s.</param>
        /// <returns>The equality of <paramref name="ppt1"/> and <paramref name="ppt2"/>.</returns>
        public static bool IsEqual(List<PathPackageTuple> ppt1, List<PathPackageTuple> ppt2)
        {
            if (ppt1 == null)
                return ppt2 == null;
            else if (ppt2 == null)
                return ppt1 == null;
            if (ppt1.Count != ppt2.Count) return false;
            for (int i = 0; i < ppt1.Count; i++) if (!ppt1[i].Equals(ppt2[i])) return false;
            return true;
        }


        /* ----------------------------------------------- */

        //Inge (15-01-2011): "Only ever look in *.package for custom content"
        //static List<string> pkgPatterns = new List<string>(new string[] { "*.package", "*.dbc", "*.world", "*.nhd", });
        static List<string> CCpkgPatterns = new List<string>(new string[] { "*.package", });
        static IEnumerable<PathPackageTuple> ccGetList(string ccPath)
        {
            List<PathPackageTuple> ppts = new List<PathPackageTuple>();

            if (ccPath != null && Directory.Exists(ccPath))
            {
                //Depth-first search
                foreach (var dir in Directory.GetDirectories(ccPath))
                {
                    foreach (var ppt in ccGetList(dir))
                        yield return ppt;
                }
                foreach (string pattern in CCpkgPatterns)
                    foreach (var path in Directory.GetFiles(ccPath, pattern))
                    {
                        PathPackageTuple ppt = null;
                        try
                        {
                            ppt = new PathPackageTuple(path);
                        }
                        catch (InvalidDataException) { continue; }
                        yield return ppt;
                    }
            }
        }
    }

    abstract class FT
    {
        List<PathPackageTuple> vendorContent = null;
        public List<PathPackageTuple> VendorContent
        {
            get
            {
                if (vendorContent == null)
                {
                    IEnumerable<PathPackageTuple> ppts = GamePackages.Where(p => File.Exists(p.Path)).Select(p => new PathPackageTuple(p.Path, false));

                    vendorContent = ppts.ToList();
                }
                return vendorContent;
            }
        }

        public IEnumerable<PackageTag> GamePackages
        {
            get
            {
                IEnumerable<Game> games = GameFolders.Games.OrderByDescending(x => x.Priority);
                IEnumerable<Game> enabledGames = games.Where(game => game.Enabled);
                IEnumerable<IEnumerable<PackageTag>> ftPackageTags = enabledGames.Select(game => this.FTPaths(game));
                IEnumerable<PackageTag> allPackageTags = ftPackageTags.SelectMany(x => x).Distinct(Game.PackageComparer);
                return allPackageTags;
            }
        }

        protected abstract IEnumerable<PackageTag> FTPaths(Game game);

        public void Reset()
        {
            if (vendorContent == null) return;
            foreach (var ppt in vendorContent)
                if (ppt.Package != null)
                    Package.Package.ClosePackage(0, ppt.Package);
            vendorContent = null;
        }
    }

    class GameContent : FT { protected override IEnumerable<PackageTag> FTPaths(Game game) { return game.GameContent; } }

    class DDSImages : FT { protected override IEnumerable<PackageTag> FTPaths(Game game) { return game.DDSImages; } }

    class Thumbnails : FT { protected override IEnumerable<PackageTag> FTPaths(Game game) { return game.Thumbnails; } }
}
