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
using s4pi.Interfaces;

namespace s4pi.Filetable
{
    /// <summary>
    /// <see cref="SpecificResource"/> extends <see cref="SpecificIndexEntry"/> to provide methods relating to
    /// <see cref="IResource"/> manipulation.
    /// </summary>
    public class SpecificResource : SpecificIndexEntry
    {
        /// <summary>
        /// When true, this <see cref="SpecificResource"/> was created specifying that the default wrapper should always be used.
        /// </summary>
        public bool DefaultWrapper { get; private set; }

        /// <summary>
        /// Contains any exception thrown as a result of attempting to retrieve the <see cref="Resource"/>.
        /// </summary>
        public Exception Exception { get; private set; }

        IResource resource = null;
        /// <summary>
        /// Returns the <see cref="IResource"/> represented by this <see cref="SpecificResource"/>, or null if the resource could
        /// not be retrieved due to an exception.
        /// </summary>
        /// <seealso cref="SpecificResource.Exception"/>
        public IResource Resource
        {
            get
            {
                try
                {
                    if (resource == null && ResourceIndexEntry != null) resource = s4pi.WrapperDealer.WrapperDealer.GetResource(0, PathPackage.Package, ResourceIndexEntry, DefaultWrapper);
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                    return null;
                }
                return resource;
            }
        }

        /// <summary>
        /// Extends the <see cref="SpecificIndexEntry"/> constructor by allowing an optional flag indicating
        /// whether to always resolve the request for <see cref="Resource"/> using the default wrapper.
        /// </summary>
        /// <param name="ppt">The <see cref="PathPackageTuple"/> to use to resolve the request.</param>
        /// <param name="specificRK">The requested <see cref="IResourceIndexEntry"/>.</param>
        /// <param name="defaultWrapper">If true, always resolve the request for <see cref="Resource"/> using the default wrapper;
        /// otherwise <see cref="s4pi.WrapperDealer.WrapperDealer.GetResource(int, IPackage, IResourceIndexEntry, bool)"/> will determine the correct resource wrapper.</param>
        public SpecificResource(PathPackageTuple ppt, IResourceIndexEntry specificRK, bool defaultWrapper = false) : base(ppt, specificRK) { DefaultWrapper = defaultWrapper; }

        /// <summary>
        /// Extends the <see cref="SpecificIndexEntry"/> constructor by allowing an optional flag indicating
        /// whether to always resolve the request for <see cref="Resource"/> using the default wrapper.
        /// </summary>
        /// <param name="searchList">One of the <see cref="FileTable"/> vendor search lists.</param>
        /// <param name="requestedRK">The <see cref="IResourceKey"/> to look up.</param>
        /// <param name="defaultWrapper">If true, always resolve the request for <see cref="Resource"/> using the default wrapper;
        /// otherwise <see cref="s4pi.WrapperDealer.WrapperDealer.GetResource(int, IPackage, IResourceIndexEntry, bool)"/> will determine the correct resource wrapper.</param>
        public SpecificResource(List<PathPackageTuple> searchList, IResourceKey requestedRK, bool defaultWrapper = false) : base(searchList, requestedRK) { DefaultWrapper = defaultWrapper; }

        /// <summary>
        /// Replaces the resource in the package with the current value.
        /// </summary>
        public void Commit() { PathPackage.Package.ReplaceResource(ResourceIndexEntry, Resource); }
    }

    /// <summary>
    /// This class provides additional helper methods extending the s3pi <see cref="IResourceIndexEntry"/>
    /// class primarily.  It binds an <see cref="IResourceIndexEntry"/> to the package it came from, using
    /// <see cref="PathPackageTuple"/>.
    /// </summary>
    public class SpecificIndexEntry : IEquatable<SpecificIndexEntry>
    {
        /// <summary>
        /// A short string indicating which file table was the source of this item.
        /// </summary>
        /// <remarks>
        /// <para>Values are:</para>
        /// <list type="table">
        /// <listheader><term>Value</term><description>Source used to resolve the request for an item.</description></listheader>
        /// <item><term>current</term><description>The current package.</description></item>
        /// <item><term>cc</term><description>A custom content package.</description></item>
        /// <item><term>fb0</term><description>A primary game content package, excluding DDS images and thumbnail images.</description></item>
        /// <item><term>dds</term><description>A DDS image package.</description></item>
        /// <item><term>tmb</term><description>A thumbnail image package.</description></item>
        /// </list>
        /// </remarks>
        public string PPSource { get; private set; }

        /// <summary>
        /// The <see cref="PathPackageTuple"/> from which this item was resolved.
        /// </summary>
        public PathPackageTuple PathPackage { get; private set; }

        /// <summary>
        /// The <see cref="IResourceIndexEntry"/> in the package that refers to the requested resource key.
        /// This is dynamically resolved when first referenced.
        /// </summary>
        public IResourceIndexEntry ResourceIndexEntry
        {
            get
            {
                if (specificIndexEntry == null && searchList != null) specificIndexEntry = findRK();
                return specificIndexEntry;
            }
        }

        /// <summary>
        /// The resource key by which this item was requested.
        /// </summary>
        public RK RequestedRK { get; private set; }

        /// <summary>
        /// The resource Tag, the <see cref="IResourceIndexEntry"/> and the path from which the item was resolved.
        /// </summary>
        public string LongName
        {
            get
            {
                string key = "0x" + specificIndexEntry.ResourceType.ToString("X8");
                List<string> exts = s4pi.Extensions.ExtList.Ext.ContainsKey(key) ? s4pi.Extensions.ExtList.Ext[key] : new List<string>();
                return String.Format("{0}-{1} from {2}", exts.Count > 0 ? exts[0] : "UNKN", specificIndexEntry, PathPackage.Path);
            }
        }

        /// <summary>
        /// The <see cref="Game.Name"/> for the <see cref="Game"/> with the Resource Group Version
        /// of this item, or an empty string for the base game.  References <see cref="ResourceIndexEntry"/>.
        /// </summary>
        public string RGVsn
        {
            get
            {
                if (ResourceIndexEntry == null) return "";
                byte rgVersion = (byte)(ResourceIndexEntry.ResourceGroup >> 27);
                if (rgVersion == 0) return "";
                Game g = GameFolders.byRGVersion(rgVersion);
                return g == null ? "Unk" : g.Name.ToUpper();
            }
        }

        private IResourceIndexEntry specificIndexEntry;
        private List<PathPackageTuple> searchList;

        /// <summary>
        /// Request <paramref name="resourceIndexEntry"/> from <paramref name="ppt"/>.
        /// </summary>
        /// <param name="ppt">The <see cref="PathPackageTuple"/> to use to resolve the request.</param>
        /// <param name="resourceIndexEntry">The requested <see cref="IResourceIndexEntry"/>.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="ppt"/> does not contain <paramref name="resourceIndexEntry"/>.</exception>
        public SpecificIndexEntry(PathPackageTuple ppt, IResourceIndexEntry resourceIndexEntry)
        {
            if (FileTable.Current == ppt) PPSource = "current";
            else if (FileTable.CustomContentEnabled && FileTable.CustomContent.Contains(ppt)) PPSource = "cc";
            else if (FileTable.GameContent != null && FileTable.GameContent.Contains(ppt)) PPSource = "fb0";
            else if (FileTable.DDSImages != null && FileTable.DDSImages.Contains(ppt)) PPSource = "dds";
            else if (FileTable.Thumbnails != null && FileTable.Thumbnails.Contains(ppt)) PPSource = "tmb";
            else PPSource = "unk";

            if (ppt.Package.Find(rie => rie.Equals(resourceIndexEntry)) == null) // Avoid recursive call to ppt.Find()
                throw new ArgumentException("Package does not contain specified ResourceIndexEntry", "resourceIndexEntry");
            this.RequestedRK = new RK(resourceIndexEntry);
            this.PathPackage = ppt;

            this.specificIndexEntry = resourceIndexEntry;
            this.searchList = null;
        }

        /// <summary>
        /// Request <paramref name="requestedRK"/> from the given <paramref name="searchList"/>.
        /// </summary>
        /// <param name="searchList">One of the <see cref="FileTable"/> vendor search lists.</param>
        /// <param name="requestedRK">The <see cref="IResourceKey"/> to look up.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="searchList"/> is not one of <see cref="FileTable"/> vendor search list properties.</exception>
        public SpecificIndexEntry(List<PathPackageTuple> searchList, IResourceKey requestedRK)
        {
            if (FileTable.IsEqual(searchList, FileTable.GameContent)) PPSource = "fb0";
            else if (FileTable.IsEqual(searchList, FileTable.DDSImages)) PPSource = "dds";
            else if (FileTable.IsEqual(searchList, FileTable.Thumbnails)) PPSource = "tmb";
            else throw new ArgumentOutOfRangeException("searchList", "Search list must be one of FileTable vendor search list properties.");
            this.PathPackage = null;
            this.RequestedRK = new RK(requestedRK);

            this.specificIndexEntry = null;
            this.searchList = searchList;
        }

        IResourceIndexEntry findRK()
        {
            foreach (var ppt in searchList)
            {
                var rie = ppt.Package.Find(RequestedRK.Equals);
                if (rie != null)
                {
                    PathPackage = ppt;
                    return rie;
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if this item represents the same resource as <paramref name="other"/>.
        /// </summary>
        /// <param name="other">Another <see cref="SpecificIndexEntry"/> to test for equivalence.</param>
        /// <returns>True if this item represents the same resource as <paramref name="other"/>; otherwise false.</returns>
        public bool Equals(SpecificIndexEntry other)
        {
            return ResourceIndexEntry.Equals(other.ResourceIndexEntry) && PathPackage.Equals(other.PathPackage);
        }
    }
}
