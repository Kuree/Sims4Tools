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
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using s4pi.Helpers;
using S4PIDemoFE.Settings;

namespace S4PIDemoFE
{
    public partial class MenuBarWidget : UserControl
    {
        private List<ToolStripMenuItem> tsMD, tsMB, cmsTP, cmsBW;

        public MenuBarWidget()
        {
            InitializeComponent();
            //VS2010Express likes setting these to false:
            bwcmImport.Enabled = true;
            bwcmExport.Enabled = true;
            bwcmSelectAll.Enabled = true;

            tsMD = new List<ToolStripMenuItem>(new ToolStripMenuItem[]
            {
                fileToolStripMenuItem, editToolStripMenuItem, resourceToolStripMenuItem, helpToolStripMenuItem,
            });
            tsMB = new List<ToolStripMenuItem>(new ToolStripMenuItem[]
            {
                //File
                newToolStripMenuItem, openToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem,
                saveCopyAsToolStripMenuItem, closeToolStripMenuItem,
                setMaxRecentToolStripMenuItem, bookmarkCurrentToolStripMenuItem, setMaxBookmarksToolStripMenuItem,
                organiseBookmarksToolStripMenuItem,
                exitToolStripMenuItem,
                //Edit
                editCopyToolStripMenuItem, editSavePreviewToolStripMenuItem, editFloatToolStripMenuItem,
                editOTEToolStripMenuItem,
                //Resource
                addToolStripMenuItem, resCopyToolStripMenuItem1, resPasteToolStripMenuItem1, duplicateToolStripMenuItem,
                replaceToolStripMenuItem,
                compressedToolStripMenuItem, deletedToolStripMenuItem, detailsToolStripMenuItem,
                copyResourceKeyToolStripMenuItem, selectAllToolStripMenuItem,
                fromFileToolStripMenuItem, fromPackageToolStripMenuItem, replaceSelectedToolStripMenuItem,
                asDBCToolStripMenuItem, toFileToolStripMenuItem, toPackageToolStripMenuItem,
                hexEditorToolStripMenuItem, textEditorToolStripMenuItem,
                //Tools
                fNVHashToolStripMenuItem, searchToolStripMenuItem,
                //Settings
                automaticUpdateChecksToolStripMenuItem, enableDDSPreviewToolStripMenuItem,
                enableFallbackTextPreviewToolStripMenuItem, enableFallbackHexPreviewToolStripMenuItem,
                askToAutosaveDBCToolStripMenuItem,
                organiseBookmarksSettingsToolStripMenuItem, organiseFolderBookmarksToolStripMenuItem,
                externalProgramsToolStripMenuItem, manageWrappersToolStripMenuItem,
                saveSettingsToolStripMenuItem,
                //Help
                contentsToolStripMenuItem, aboutToolStripMenuItem, checkForUpdateToolStripMenuItem,
                warrantyToolStripMenuItem, licenceToolStripMenuItem,
                //Filter context menu
                flcmPasteResourceKey,
            });
            cmsTP = new List<ToolStripMenuItem>(new ToolStripMenuItem[]
            {
                //TextPreviewContextMenuStrip
                tpcmCopy, tpcmSavePreview, tpcmFloat, tpcmOTE,
            });
            cmsBW = new List<ToolStripMenuItem>(new ToolStripMenuItem[]
            {
                //BrowserWidgetContextMenuStrip
                bwcmAdd, bwcmCopy, bwcmPaste, bwcmDuplicate, bwcmReplace,
                bwcmCompressed, bwcmDeleted, bwcmDetails, bwcmCopyResourceKey, bwcmSelectAll,
                bwcmFromFile, bwcmFromPackage, bwcmReplaceSelected, bwcmAsDBC, bwcmToFile, bwcmToPackage,
                bwcmHexEditor, bwcmTextEditor,
            });
            UpdateMRUList();
            UpdateBookmarks();
            UpdateChecker.AutoUpdateChoiceChanged += new EventHandler(Checker_AutoUpdateChoice_Changed);
            Checker_AutoUpdateChoice_Changed(null, null);

            Checked(MB.MBS_previewDDS, Properties.Settings.Default.EnableDDSPreview);
            Checked(MB.MBS_fallbackTextPreview, Properties.Settings.Default.EnableFallbackTextPreview);
            Checked(MB.MBS_fallbackHexPreview, Properties.Settings.Default.EnableFallbackHexPreview);
        }

        private void Checker_AutoUpdateChoice_Changed(object sender, EventArgs e)
        {
            Checked(MB.MBS_updates, UpdateChecker.AutoUpdateChoice);
        }

        public enum MD
        {
            MBF,
            MBE,
            MBR,
            MBS,
            MBH,
            CMF,
        }

        public enum MB
        {
            MBF_new = 0,
            MBF_open,
            MBF_save,
            MBF_saveAs,
            MBF_saveCopyAs,
            MBF_close,
            MBF_setMaxRecent,
            MBF_bookmarkCurrent,
            MBF_setMaxBookmarks,
            MBF_organiseBookmarks,
            MBF_exit,
            MBE_copy,
            MBE_save,
            MBE_float,
            MBE_ote,
            MBR_add,
            MBR_copy,
            MBR_paste,
            MBR_duplicate,
            MBR_replace,
            MBR_compressed,
            MBR_isdeleted,
            MBR_details,
            MBR_copyRK,
            MBR_selectAll,
            MBR_importResources,
            MBR_importPackages,
            MBR_replaceFrom,
            MBR_importAsDBC,
            MBR_exportResources,
            MBR_exportToPackage,
            MBR_hexEditor,
            MBR_textEditor,
            MBT_fnvHash,
            MBT_search,
            MBS_updates,
            MBS_previewDDS,
            MBS_fallbackTextPreview,
            MBS_fallbackHexPreview,
            MBS_askAutoSaveDBC,
            MBS_bookmarks,
            MBS_customplaces,
            MBS_externals,
            MBS_wrappers,
            MBS_saveSettings,
            MBH_contents,
            MBH_about,
            MBH_update,
            MBH_warranty,
            MBH_licence,
            CMF_pasteRK,
        }

        public enum CMS_TP
        {
            MBE_copy = (int) MB.MBE_copy,
            MBE_save,
            MBE_float,
            MBE_ote,
        }

        public enum CMS_BW
        {
            MBR_add = (int) MB.MBR_add,
            MBR_copy,
            MBR_paste,
            MBR_duplicate,
            MBR_replace,
            MBR_compressed,
            MBR_isdeleted,
            MBR_details,
            MBR_copyRK,
            MBR_selectAll,
            MBR_importResources,
            MBR_importPackages,
            MBR_importAsDBC,
            MBR_exportResources,
            MBR_exportToPackage,
            MBR_hexEditor,
            MBR_textEditor,
        }

        private bool isCMSTP(MB mn)
        {
            return (mn >= MB.MBE_copy && mn < MB.MBR_add);
        }

        private bool isCMSBW(MB mn)
        {
            return (mn >= MB.MBR_add && mn < MB.MBT_fnvHash);
        }

        public void Enable(MD mn, bool state)
        {
            tsMD[(int) mn].Enabled = state;
            if (mn == MD.MBR) browserWidgetContextMenuStrip.Enabled = state;
        }

        public void Enable(MB mn, bool state)
        {
            tsMB[(int) mn].Enabled = state;
            if (isCMSTP(mn)) cmsTP[(int) mn - (int) CMS_TP.MBE_copy].Enabled = state;
            if (isCMSBW(mn)) cmsBW[(int) mn - (int) CMS_BW.MBR_add].Enabled = state;
        }

        public void Checked(MB mn, bool state)
        {
            tsMB[(int) mn].Checked = state;
            tsMB[(int) mn].CheckState = state ? CheckState.Checked : CheckState.Unchecked;
            if (isCMSTP(mn))
            {
                cmsTP[(int) mn - (int) CMS_TP.MBE_copy].Checked = state;
                cmsTP[(int) mn - (int) CMS_TP.MBE_copy].CheckState = state ? CheckState.Checked : CheckState.Unchecked;
            }
            if (isCMSBW(mn))
            {
                cmsBW[(int) mn - (int) CMS_BW.MBR_add].Checked = state;
                cmsBW[(int) mn - (int) CMS_BW.MBR_add].CheckState = state ? CheckState.Checked : CheckState.Unchecked;
            }
        }

        public void Indeterminate(MB mn)
        {
            tsMB[(int) mn].CheckState = CheckState.Indeterminate;
            if (isCMSTP(mn)) cmsTP[(int) mn - (int) CMS_TP.MBE_copy].CheckState = CheckState.Indeterminate;
            if (isCMSBW(mn)) cmsBW[(int) mn - (int) CMS_BW.MBR_add].CheckState = CheckState.Indeterminate;
        }

        public bool IsChecked(MB mn)
        {
            return tsMB[(int) mn].Checked;
        }

        public class MBDropDownOpeningEventArgs : EventArgs
        {
            public readonly MD mn;

            public MBDropDownOpeningEventArgs(MD mn)
            {
                this.mn = mn;
            }
        }

        public delegate void MBDropDownOpeningEventHandler(object sender, MBDropDownOpeningEventArgs mn);

        public event MBDropDownOpeningEventHandler MBDropDownOpening;

        protected void OnMBDropDownOpening(object sender, MD mn)
        {
            if (MBDropDownOpening != null) MBDropDownOpening(sender, new MBDropDownOpeningEventArgs(mn));
        }

        private void tsMD_DropDownOpening(object sender, EventArgs e)
        {
            OnMBDropDownOpening(sender, (MD) tsMD.IndexOf(sender as ToolStripMenuItem));
        }

        private void cmsTP_Opening(object sender, CancelEventArgs e)
        {
            OnMBDropDownOpening(sender, MD.MBE);
        }

        private void cmsBW_Opening(object sender, CancelEventArgs e)
        {
            OnMBDropDownOpening(sender, MD.MBR);
        }

        private void cmsFL_Opening(object sender, CancelEventArgs e)
        {
            OnMBDropDownOpening(sender, MD.CMF);
        }

        public class MBClickEventArgs : EventArgs
        {
            public readonly MB mn;

            public MBClickEventArgs(MB mn)
            {
                this.mn = mn;
            }
        }

        public delegate void MBClickEventHandler(object sender, MBClickEventArgs mn);

        public event MBClickEventHandler MBFile_Click;

        protected void OnMBFile_Click(object sender, MB mn)
        {
            if (MBFile_Click != null) MBFile_Click(sender, new MBClickEventArgs(mn));
        }

        private void tsMBF_Click(object sender, EventArgs e)
        {
            OnMBFile_Click(sender, (MB) tsMB.IndexOf(sender as ToolStripMenuItem));
        }

        public event MBClickEventHandler MBEdit_Click;

        protected void OnMBEdit_Click(object sender, MB mn)
        {
            if (MBEdit_Click != null) MBEdit_Click(sender, new MBClickEventArgs(mn));
        }

        private void tsMBE_Click(object sender, EventArgs e)
        {
            OnMBEdit_Click(sender, (MB) tsMB.IndexOf(sender as ToolStripMenuItem));
        }

        public event MBClickEventHandler MBResource_Click;

        protected void OnMBResource_Click(object sender, MB mn)
        {
            if (MBResource_Click != null) MBResource_Click(sender, new MBClickEventArgs(mn));
        }

        private void tsMBR_Click(object sender, EventArgs e)
        {
            OnMBResource_Click(sender, (MB) tsMB.IndexOf(sender as ToolStripMenuItem));
        }

        public event MBClickEventHandler MBTools_Click;

        protected void OnMBTools_Click(object sender, MB mn)
        {
            if (MBTools_Click != null) MBTools_Click(sender, new MBClickEventArgs(mn));
        }

        private void tsMBT_Click(object sender, EventArgs e)
        {
            OnMBTools_Click(sender, (MB) tsMB.IndexOf(sender as ToolStripMenuItem));
        }

        public event MBClickEventHandler MBSettings_Click;

        protected void OnMBSettings_Click(object sender, MB mn)
        {
            if (MBSettings_Click != null) MBSettings_Click(sender, new MBClickEventArgs(mn));
        }

        private void tsMBS_Click(object sender, EventArgs e)
        {
            OnMBSettings_Click(sender, (MB) tsMB.IndexOf(sender as ToolStripMenuItem));
        }

        public event MBClickEventHandler MBHelp_Click;

        protected void OnMBHelp_Click(object sender, MB mn)
        {
            if (MBHelp_Click != null) MBHelp_Click(sender, new MBClickEventArgs(mn));
        }

        private void tsMBH_Click(object sender, EventArgs e)
        {
            OnMBHelp_Click(sender, (MB) tsMB.IndexOf(sender as ToolStripMenuItem));
        }

        public event MBClickEventHandler CMFilter_Click;

        protected void OnCMFilter_Click(object sender, MB mn)
        {
            if (CMFilter_Click != null) CMFilter_Click(sender, new MBClickEventArgs(mn));
        }

        private void tsCMF_Click(object sender, EventArgs e)
        {
            OnCMFilter_Click(sender, (MB) tsMB.IndexOf(sender as ToolStripMenuItem));
        }

        private void tsCMSTP_Click(object sender, EventArgs e)
        {
            OnMBEdit_Click(sender, (MB) (cmsTP.IndexOf(sender as ToolStripMenuItem)) + (int) MB.MBE_copy);
        }

        private void tsCMSBW_Click(object sender, EventArgs e)
        {
            OnMBResource_Click(sender, (MB) (cmsBW.IndexOf(sender as ToolStripMenuItem)) + (int) MB.MBR_add);
        }


        private static string resourceHelperPrefix = "resourceHelper";
        private static string contentHelperPrefix = "bwcmHelper";

        private void ClearHelpers(ToolStripItemCollection dropdown, ToolStripItem tss, string prefix)
        {
            int i = dropdown.IndexOf(tss) + 1;
            while (true)
            {
                if (i >= dropdown.Count) break;
                if (!dropdown[i].Name.StartsWith(prefix)) break;
                dropdown.RemoveAt(i);
            }
        }

        public void ClearHelpers()
        {
            ClearHelpers(resourceToolStripMenuItem.DropDownItems, textEditorToolStripMenuItem, resourceHelperPrefix);
            ClearHelpers(browserWidgetContextMenuStrip.Items, bwcmTextEditor, contentHelperPrefix);
        }

        private void AddHelper(ToolStripItemCollection dropdown, ToolStripItem tss, string prefix, int helper,
            string value)
        {
            ToolStripMenuItem tsiHelper = new ToolStripMenuItem(value, null, tsHelper_Click)
            {
                Name = prefix + helper,
                Tag = helper,
            };
            int i = dropdown.IndexOf(tss);
            while (true)
            {
                i++;
                if (i >= dropdown.Count) break;
                if (!dropdown[i].Name.StartsWith(prefix)) break;
            }
            dropdown.Insert(i, tsiHelper);
        }

        public void AddHelper(int helper, string value)
        {
            AddHelper(resourceToolStripMenuItem.DropDownItems, tsSepHelpers, "tsHelper", helper, value);
            AddHelper(browserWidgetContextMenuStrip.Items, bwcmSepHelpers, "bwcmHelper", helper, value);
        }

        private void SetHelpers(ToolStripItemCollection dropdown, ToolStripItem tss, string prefix,
            IEnumerable<HelperManager.Helper> helpers)
        {
            if (helpers.Count() == 0) return;

            int i = dropdown.IndexOf(tss);
            int j = 0;

            dropdown.Insert(++i, new ToolStripSeparator() {Name = prefix + j,});

            foreach (var helper in helpers)
            {
                ToolStripMenuItem tsiHelper = new ToolStripMenuItem(helper.label, null, tsHelper_Click)
                {
                    Name = prefix + j,
                    Tag = j++,
                };
                dropdown.Insert(++i, tsiHelper);
            }
        }

        public void SetHelpers(IEnumerable<HelperManager.Helper> helpers)
        {
            SetHelpers(resourceToolStripMenuItem.DropDownItems, textEditorToolStripMenuItem, resourceHelperPrefix,
                helpers);
            SetHelpers(browserWidgetContextMenuStrip.Items, bwcmTextEditor, contentHelperPrefix, helpers);
        }

        public class HelperClickEventArgs : EventArgs
        {
            public readonly int helper;

            public HelperClickEventArgs(int helper)
            {
                this.helper = helper;
            }
        }

        public delegate void HelperClickEventHandler(object sender, HelperClickEventArgs helper);

        public event HelperClickEventHandler HelperClick;

        protected void OnHelperClick(object sender, int i)
        {
            if (HelperClick != null) HelperClick(sender, new HelperClickEventArgs(i));
        }

        private void tsHelper_Click(object sender, EventArgs e)
        {
            OnHelperClick(sender, (int) ((sender as ToolStripMenuItem).Tag));
        }

        private void SetPreviewControlItems(ToolStripItemCollection dropdown, ToolStripItem tss, string prefix,
            IEnumerable<ToolStripItem> items)
        {
            int i = dropdown.Count - 1;
            int j = dropdown.Count - dropdown.IndexOf(tss);
            foreach (var item in items)
            {
                item.Name = prefix + (++j);
                dropdown.Insert(++i, item);
            }
        }

        public void SetPreviewControlItems(IEnumerable<ToolStripItem> items)
        {
            SetPreviewControlItems(resourceToolStripMenuItem.DropDownItems, textEditorToolStripMenuItem,
                resourceHelperPrefix, items);
            SetPreviewControlItems(browserWidgetContextMenuStrip.Items, bwcmTextEditor, contentHelperPrefix, items);
        }


        public void AddRecentFile(string value)
        {
            if (Properties.Settings.Default.MRUList == null)
                Properties.Settings.Default.MRUList = new StringCollection();
            if (Properties.Settings.Default.MRUList.Contains(value))
                Properties.Settings.Default.MRUList.Remove(value);
            Properties.Settings.Default.MRUList.Insert(0, value);
            while (Properties.Settings.Default.MRUList.Count > Properties.Settings.Default.MRUListSize)
                Properties.Settings.Default.MRUList.RemoveAt(Properties.Settings.Default.MRUList.Count - 1);
            UpdateMRUList();
        }

        private void UpdateMRUList()
        {
            this.mRUListToolStripMenuItem.DropDownItems.Clear();
            this.mRUListToolStripMenuItem.DropDownItems.Add(toolStripSeparator6);
            this.mRUListToolStripMenuItem.DropDownItems.Add(setMaxRecentToolStripMenuItem);

            int i = 0;
            if (Properties.Settings.Default.MRUList != null)
                foreach (string f in Properties.Settings.Default.MRUList)
                {
                    string s = makeName(f);

                    ToolStripMenuItem tsmiMRUListEntry = new ToolStripMenuItem();
                    tsmiMRUListEntry.Name = "tsmiRecent" + i;
                    tsmiMRUListEntry.ShortcutKeys = (Keys) (Keys.Control | ((Keys) (49 + i)));
                    tsmiMRUListEntry.Text = string.Format("&{0}. {1}", i + 1, s);
                    tsmiMRUListEntry.Click += new EventHandler(tsMRU_Click);
                    mRUListToolStripMenuItem.DropDownItems.Insert(i, tsmiMRUListEntry);
                    i++;
                }
        }

        public class MRUClickEventArgs : EventArgs
        {
            public readonly string filename;

            public MRUClickEventArgs(string filename)
            {
                this.filename = filename;
            }
        }

        public delegate void MRUClickEventHandler(object sender, MRUClickEventArgs filename);

        public event MRUClickEventHandler MRUClick;

        protected void OnMRUClick(object sender, int i)
        {
            if (MRUClick != null) MRUClick(sender, new MRUClickEventArgs(Properties.Settings.Default.MRUList[i]));
        }

        private void tsMRU_Click(object sender, EventArgs e)
        {
            OnMRUClick(sender, mRUListToolStripMenuItem.DropDownItems.IndexOf(sender as ToolStripMenuItem));
        }

        public void AddBookmark(string value)
        {
            if (Properties.Settings.Default.Bookmarks == null)
                Properties.Settings.Default.Bookmarks = new StringCollection();
            if (Properties.Settings.Default.Bookmarks.Contains(value))
                Properties.Settings.Default.Bookmarks.Remove(value);
            Properties.Settings.Default.Bookmarks.Insert(0, value);
            while (Properties.Settings.Default.Bookmarks.Count > Properties.Settings.Default.BookmarkSize)
                Properties.Settings.Default.Bookmarks.RemoveAt(Properties.Settings.Default.Bookmarks.Count - 1);
            UpdateBookmarks();
        }

        public void UpdateBookmarks()
        {
            this.bookmarkedPackagesToolStripMenuItem.DropDownItems.Clear();
            this.bookmarkedPackagesToolStripMenuItem.DropDownItems.Add(toolStripSeparator7);
            this.bookmarkedPackagesToolStripMenuItem.DropDownItems.Add(bookmarkCurrentToolStripMenuItem);
            this.bookmarkedPackagesToolStripMenuItem.DropDownItems.Add(setMaxBookmarksToolStripMenuItem);
            this.bookmarkedPackagesToolStripMenuItem.DropDownItems.Add(organiseBookmarksToolStripMenuItem);

            int i = 0;
            if (Properties.Settings.Default.Bookmarks != null)
                foreach (string f in Properties.Settings.Default.Bookmarks)
                {
                    string s = makeName(f);

                    ToolStripMenuItem tsmiBookmarkEntry = new ToolStripMenuItem();
                    tsmiBookmarkEntry.Name = "tsmiBookmark" + i;
                    tsmiBookmarkEntry.ShortcutKeys = (Keys) (Keys.Control | Keys.Shift | ((Keys) (49 + i)));
                    tsmiBookmarkEntry.Text = string.Format("&{0}. {1}", i + 1, s);
                    tsmiBookmarkEntry.Click += new EventHandler(tsBookmark_Click);
                    bookmarkedPackagesToolStripMenuItem.DropDownItems.Insert(i, tsmiBookmarkEntry);
                    i++;
                }
        }

        private string makeName(string savedName)
        {
            bool readWrite;
            if (savedName.StartsWith("0:"))
            {
                savedName = savedName.Substring(2);
                readWrite = false;
            }
            else if (savedName.StartsWith("1:"))
            {
                savedName = savedName.Substring(2);
                readWrite = true;
            }
            else readWrite = true;
            if (savedName.Length > 100)
                savedName = "..." +
                            savedName.Substring(Math.Max(0,
                                savedName.Length - Math.Max(100, Path.GetFileName(savedName).Length)));
            return (readWrite ? "[RW]" : "[RO]") + ": " + savedName;
        }

        public class BookmarkClickEventArgs : EventArgs
        {
            public readonly string filename;

            public BookmarkClickEventArgs(string filename)
            {
                this.filename = filename;
            }
        }

        public delegate void BookmarkClickEventHandler(object sender, BookmarkClickEventArgs filename);

        public event BookmarkClickEventHandler BookmarkClick;

        protected void OnBookmarkClick(object sender, int i)
        {
            if (BookmarkClick != null)
                BookmarkClick(sender, new BookmarkClickEventArgs(Properties.Settings.Default.Bookmarks[i]));
        }

        private void tsBookmark_Click(object sender, EventArgs e)
        {
            OnBookmarkClick(sender,
                bookmarkedPackagesToolStripMenuItem.DropDownItems.IndexOf(sender as ToolStripMenuItem));
        }
    }
}
