/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones, 2014 By Keyi Zhang          *
 *  pljones@users.sf.net, kz005@bucknell.edu                               *
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
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using AutoUpdate;

using s4pi.Extensions;
using s4pi.Helpers;
using s4pi.Interfaces;
using s4pi.Package;
using s4pi.WrapperDealer;

using S4PIDemoFE.Settings;
using S4PIDemoFE.Tools;

using Version = AutoUpdate.Version;

namespace S4PIDemoFE
{
    public partial class MainForm : Form
    {
        static readonly List<string> fields = AApiVersionedFields.GetContentFields(0, typeof(AResourceIndexEntry));
        static readonly List<string> unwantedFields = new List<string>(new[] { "Stream" });
        static readonly List<string> unwantedFilterFields = new List<string>(new[] { "Chunkoffset", "Filesize", "Memsize", "Unknown2" });
        
		static List<string> ddsResources = new List<string>(new[] { "0x00B2D882", "0x8FFB80F6" });
        static readonly string myName;
        static readonly string tempName;

	    string cmdLineFilename;
	    List<string> cmdLineBatch = new List<string>();

	    static MainForm()
        {
            using (Splash splash = new Splash("Refreshing wrappers list..."))
            {
                splash.Show();
                Application.DoEvents();
                myName = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
                tempName = "s3pe-" + FNV64.GetHash(DateTime.UtcNow.ToString("O")).ToString("X16") + "-";
                foreach (string s in unwantedFields) fields.Remove(s);

                List<KeyValuePair<string, Type>> typeMap = new List<KeyValuePair<string, Type>>(WrapperDealer.TypeMap);
                WrapperDealer.Disabled.Clear();
                if (Properties.Settings.Default.DisabledWrappers != null)
                {
	                foreach (var v in Properties.Settings.Default.DisabledWrappers)
	                {
		                string[] kv = v.Trim().Split(new[] { ':' }, 2);
		                KeyValuePair<string, Type> kvp = typeMap.Find(x => x.Key == kv[0] && x.Value.FullName == kv[1]);
		                if (!kvp.Equals(default(KeyValuePair<string, Type>)))
		                {
			                WrapperDealer.Disabled.Add(kvp);
		                }
	                }
                }
            }
        }

	    public MainForm()
        {
            using (Splash splash = new Splash("Initialising form..."))
            {
                splash.Show();
                Application.DoEvents();
	            this.InitializeComponent();

                this.Text = myName;

                this.lbProgress.Text = "";

	            this.browserWidget1.Fields = new List<string>(fields.ToArray());
	            this.browserWidget1.ContextMenuStrip = this.menuBarWidget.browserWidgetContextMenuStrip;

                List<string> filterFields = new List<string>(fields);
                foreach (string f in unwantedFilterFields)
                    filterFields.Remove(f);
                filterFields.Insert(0, "Tag");
                filterFields.Insert(0, "Name");
	            this.resourceFilterWidget1.BrowserWidget = this.browserWidget1;
	            this.resourceFilterWidget1.Fields = filterFields;
	            this.resourceFilterWidget1.ContextMenuStrip = this.menuBarWidget.filterContextMenuStrip;
	            this.menuBarWidget.CMFilter_Click += this.menuBarWidget1_CMFilter_Click;

	            this.packageInfoWidget1.Fields = this.packageInfoFields1.Fields;
                this.PackageFilenameChanged += this.MainForm_PackageFilenameChanged;
                this.PackageChanged += this.MainForm_PackageChanged;

                this.SaveSettings += this.MainForm_SaveSettings;
                this.SaveSettings += this.browserWidget1.BrowserWidget_SaveSettings;
                this.SaveSettings += this.controlPanel1.ControlPanel_SaveSettings;

	            this.MainForm_LoadFormSettings();
            }
        }

	    public MainForm(params string[] args)
		    : this()
	    {
		    this.CmdLine(args);

		    // Settings for test mode
		    if (this.cmdlineTest)
		    {
		    }
	    }

	    void MainForm_LoadFormSettings()
        {
            FormWindowState s =
                Enum.IsDefined(typeof(FormWindowState), Properties.Settings.Default.FormWindowState)
                ? (FormWindowState)Properties.Settings.Default.FormWindowState
                : FormWindowState.Minimized;

            int defaultWidth = 4 * Screen.PrimaryScreen.WorkingArea.Width / 5;
            int defaultHeight = 4 * Screen.PrimaryScreen.WorkingArea.Height / 5;
            this.ClientSize = new Size(defaultWidth, defaultHeight);//needed to correctly work out the following
            int defaultSplitterDistance1 = this.splitContainer1.ClientSize.Height - (this.splitContainer1.Panel2MinSize + this.splitContainer1.SplitterWidth + 4);
            int defaultSplitterDistance2 = this.splitContainer2.ClientSize.Width / 2;

            if (s == FormWindowState.Minimized)
            {
                this.ClientSize = new Size(defaultWidth, defaultHeight);
	            this.splitContainer1.SplitterDistance = defaultSplitterDistance1;
	            this.splitContainer2.SplitterDistance = defaultSplitterDistance2;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                // these mustn't be negative
                int w = Properties.Settings.Default.PersistentWidth;
                int h = Properties.Settings.Default.PersistentHeight;
                this.ClientSize = new Size(w < 0 ? defaultWidth : w, h < 0 ? defaultHeight : h);

                int s1 = Properties.Settings.Default.Splitter1Position;
	            this.splitContainer1.SplitterDistance = s1 < 0 ? defaultSplitterDistance1 : s1;

                int s2 = Properties.Settings.Default.Splitter2Position;
	            this.splitContainer2.SplitterDistance = s2 < 0 ? defaultSplitterDistance2 : s2;

                // everything else assumed valid -- any problems, use the iconise/exit/run trick to fix

                this.StartPosition = FormStartPosition.Manual;
                this.Location = Properties.Settings.Default.PersistentLocation;
                this.WindowState = s;
            }
        }

        void MainForm_SaveSettings(object sender, EventArgs e)
        {
            Properties.Settings.Default.FormWindowState = (int)this.WindowState;
            Properties.Settings.Default.PersistentHeight = this.ClientSize.Height;
            Properties.Settings.Default.PersistentWidth = this.ClientSize.Width;
            Properties.Settings.Default.PersistentLocation = this.Location;
            Properties.Settings.Default.Splitter1Position = this.splitContainer1.SplitterDistance;
            Properties.Settings.Default.Splitter2Position = this.splitContainer2.SplitterDistance;

            Properties.Settings.Default.DisabledWrappers = new StringCollection();
            foreach (var kvp in WrapperDealer.Disabled)
                Properties.Settings.Default.DisabledWrappers.Add(kvp.Key + ":" + kvp.Value.FullName + "\n");
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (this.cmdLineFilename != null)
            {
	            this.Filename = this.cmdLineFilename;
	            this.cmdLineFilename = null;
            }
            if (this.cmdLineBatch.Count > 0)
            {
                try
                {
                    this.Enabled = false;
	                this.importBatch(this.cmdLineBatch.ToArray(), "-import");
                }
                finally { this.Enabled = true; }
	            this.cmdLineBatch = new List<string>();
            }
        }

        public bool IsClosing;
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
	        this.IsClosing = true;
            this.Enabled = false;
	        this.Filename = "";
            if (this.CurrentPackage != null) {
	            this.IsClosing = false; e.Cancel = true; this.Enabled = true; return; }

	        this.saveSettings();

	        this.cleanUpTemp();
        }

        private void MainForm_PackageFilenameChanged(object sender, EventArgs e)
        {
            if (this.Filename.Length > 0 && File.Exists(this.Filename))
            {
                try
                {
	                this.CurrentPackage = Package.OpenPackage(0, this.Filename, this.ReadWrite);
	                this.menuBarWidget.AddRecentFile((this.ReadWrite ? "1:" : "0:") + this.Filename);
                    string s = this.Filename;
                    if (s.Length > 128)
                    {
                        s = Path.GetDirectoryName(s);
                        s = s.Substring(Math.Max(0, s.Length - 40));
                        s = "..." + Path.Combine(s, Path.GetFileName(this.Filename));
                    }
                    this.Text = String.Format("{0}: [{1}] {2}", myName, this.ReadWrite ? "RW" : "RO", s);
                }
                catch (InvalidDataException idex)
                {
                    if (idex.Message.Contains("magic tag"))
                    {
                        CopyableMessageBox.Show(
                            "Could not open package:\n" + this.Filename + "\n\n" +
                            "This file does not contain the expected package identifier in the header.\n" +
                            "This could be because it is a protected package (e.g. a Store item), a Sims3Pack or some other random file.\n\n" +
                            "---\nError message:\n" +
                            idex.Message, myName + ": Unable to open file", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                    }
                    else if (idex.Message.Contains("major version"))
                    {
                        CopyableMessageBox.Show(
                            "Could not open package:\n" + this.Filename + "\n\n" +
                            "This file does not contain the expected package major version number in the header.\n" +
                            "This could be because it is a package for Sims2 or Spore.\n\n" +
                            "---\nError message:\n" +
                            idex.Message, myName + ": Unable to open file", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                    }
                    else
                    {
                        IssueException(idex, "Could not open package:\n" + this.Filename);
                    }
	                this.Filename = "";
                }
                catch (UnauthorizedAccessException uaex)
                {
                    if (this.ReadWrite)
                    {
                        int i = CopyableMessageBox.Show(
                            "Could not open package:\n" + this.Filename + "\n\n" +
                            "The file could be write-protected, in which case it might be possible to open it read-only.\n\n" +
                            "---\nError message:\n" +
                            uaex.Message, myName + ": Unable to open file", CopyableMessageBoxIcon.Stop, new[] { "&Open read-only", "C&ancel" }, 1, 1);
                        if (i == 0) this.Filename = "0:" + this.Filename;
                        else this.Filename = "";
                    }
                    else
                    {
                        IssueException(uaex, "Could not open package:\n" + this.Filename);
	                    this.Filename = "";
                    }
                }
                catch (IOException ioex)
                {
                    int i = CopyableMessageBox.Show(
                        "Could not open package:\n" + this.Filename + "\n\n" +
                        "There may be another process with exclusive access to the file (e.g. The Sims 3).  " +
                        "After exiting the other process, you can retry opening the package.\n\n" +
                        "---\nError message:\n" +
                        ioex.Message, myName + ": Unable to open file", CopyableMessageBoxIcon.Stop, new[] { "&Retry", "C&ancel" }, 1, 1);
                    if (i == 0) this.Filename = (this.ReadWrite ? "1:" : "0:") + this.Filename;
                    else this.Filename = "";
                }
#if !DEBUG
                catch (Exception ex)
                {
                    IssueException(ex, "Could not open package:\n" + Filename);
                    Filename = "";
                }
#endif
            }
            else
            {
                this.Text = myName;
            }
        }

        private void MainForm_PackageChanged(object sender, EventArgs e)
        {
	        this.browserWidget1.Package = this.packageInfoWidget1.Package = this.CurrentPackage;
	        this.pnAuto.Controls.Clear();
            bool enable = this.CurrentPackage != null;
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBF_saveAs, enable);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBF_saveCopyAs, enable);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBF_close, enable);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBF_bookmarkCurrent, enable);
	        this.menuBarWidget.Enable(MenuBarWidget.MD.MBE, enable);
	        this.menuBarWidget.Enable(MenuBarWidget.MD.MBR, enable);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_add, enable);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBT_search, enable);
	        this.editDropDownOpening();
	        this.resourceDropDownOpening();
        }

        public event EventHandler SaveSettings;
        protected virtual void OnSaveSettings(object sender, EventArgs e) { if (this.SaveSettings != null) this.SaveSettings(sender, e); }


        internal static void IssueException(Exception ex, string prefix)
        {
            CopyableMessageBox.IssueException(ex,
                String.Format("{0}\nFront-end Distribution: {1}\nLibrary Distribution: {2}",
                prefix, Version.CurrentVersion, Version.LibraryVersion),
                myName);
        }

        #region Command Line

	    Dictionary<string, CmdInfo> options;

	    bool cmdlineTest;

	    delegate bool CmdLineCmd(ref List<string> cmdline);

	    struct CmdInfo
        {
            public CmdLineCmd cmd;
            public string help;
            public CmdInfo(CmdLineCmd cmd, string help) : this() { this.cmd = cmd; this.help = help; }
        }

	    void SetOptions()
        {
			this.options = new Dictionary<string, CmdInfo>
						   {
							   { "test", new CmdInfo(this.CmdLineTest, "Enable facilities still undergoing initial testing") },
							   { "import", new CmdInfo(this.CmdLineImport, "Import a batch of files into a new package") },
							   { "help", new CmdInfo(this.CmdLineHelp, "Display this help") }
						   };
        }

	    void CmdLine(params string[] args)
        {
	        this.SetOptions();
            List<string> cmdline = new List<string>(args);
            List<string> pkgs = new List<string>();
            while (cmdline.Count > 0)
            {
                string option = cmdline[0];
                cmdline.RemoveAt(0);
                if (option.StartsWith("/") || option.StartsWith("-"))
                {
                    option = option.Substring(1);
                    if (this.options.ContainsKey(option.ToLower()))
                    {
                        if (this.options[option.ToLower()].cmd(ref cmdline))
                            Environment.Exit(0);
                    }
                    else
                    {
                        CopyableMessageBox.Show(this, "Invalid command line option: '" + option + "'",
                            myName, CopyableMessageBoxIcon.Error, new List<string>(new[] { "OK" }), 0, 0);
                        Environment.Exit(1);
                    }
                }
                else
                {
                    if (pkgs.Count == 0)
                    {
                        if (!File.Exists(option))
                        {
                            CopyableMessageBox.Show(this, "File not found:\n" + option,
                                myName, CopyableMessageBoxIcon.Error, new List<string>(new[] { "OK" }), 0, 0);
                            Environment.Exit(1);
                        }
                        pkgs.Add(option);
	                    this.cmdLineFilename = option;
                    }
                    else
                    {
                        CopyableMessageBox.Show(this, "Can only accept one package on command line",
                            myName, CopyableMessageBoxIcon.Error, new List<string>(new[] { "OK" }), 0, 0);
                        Environment.Exit(1);
                    }
                }
            }
        }

	    bool CmdLineTest(ref List<string> cmdline) 
		{
	        this.cmdlineTest = true; 
			return false; 
		}

        bool CmdLineImport(ref List<string> cmdline)
        {
            if (cmdline.Count < 1)
            {
                CopyableMessageBox.Show(this, "-import requires one or more files",
                    myName, CopyableMessageBoxIcon.Error, new List<string>(new[] { "OK" }), 0, 0);
                Environment.Exit(1);
            }
            while (cmdline.Count > 0 && cmdline[0][0] != '/' && cmdline[0][0] != '-')
            {
                if (!File.Exists(cmdline[0]))
                {
                    CopyableMessageBox.Show(this, "File not found:\n" + cmdline[0],
                        myName, CopyableMessageBoxIcon.Error, new List<string>(new[] { "OK" }), 0, 0);
                    Environment.Exit(1);
                }
	            this.cmdLineBatch.Add(cmdline[0]);
                cmdline.RemoveAt(0);
            }
            return false;
        }

        bool CmdLineHelp(ref List<string> cmdline)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("The following command line options are available:\n");
            foreach (var kvp in this.options)
                sb.AppendFormat("{0}  --  {1}\n", kvp.Key, kvp.Value.help);
            sb.AppendLine("\nOptions must be prefixed with '/' or '-'\n\n");
            sb.AppendLine("A fully-qualified package name can also be supplied on the command line.");

            CopyableMessageBox.Show(this, sb.ToString(), "Command line options", CopyableMessageBoxIcon.Information, new List<string>(new[] { "OK" }), 0, 0);
            return true;
        }

        #endregion
		
        #region Package Filename

        string filename;

	    bool ReadWrite
	    {
		    get
		    {
			    return this.filename != null && this.filename.Length > 1 && this.filename.StartsWith("1:");
		    }
	    }
        
		string Filename
        {
            get { return this.filename == null || this.filename.Length < 2 ? "" : this.filename.Substring(2); }
            set
            {
	            this.CurrentPackage = null;
				if (this.CurrentPackage != null)
				{
					return;
				}

	            this.filename = value;
                if (!this.filename.StartsWith("0:") && !this.filename.StartsWith("1:"))
                {
	                this.filename = "1:" + this.filename;
                }

	            this.OnPackageFilenameChanged(this, new EventArgs());
            }
        }

        public event EventHandler PackageFilenameChanged;

	    protected virtual void OnPackageFilenameChanged(object sender, EventArgs e)
	    {
		    if (this.PackageFilenameChanged != null)
		    {
			    this.PackageFilenameChanged(sender, e);
		    }
	    }
       
		#endregion

        #region Current Package
        
		bool isPackageDirty;
	    IPackage package;
	    
		public event EventHandler PackageChanged;

	    bool IsPackageDirty
        {
            get { return this.ReadWrite && this.isPackageDirty; }
            set
            {
	            this.menuBarWidget.Enable(MenuBarWidget.MB.MBF_save, this.ReadWrite && value);

	            this.isPackageDirty = value;
            }
        }

	    IPackage CurrentPackage
        {
            get { return this.package; }
            set
            {
                if (this.package == value) return;

	            this.browserWidget1.SelectedResource = null;
                if (this.browserWidget1.SelectedResource != null) return;

                if (this.isPackageDirty)
                {
                    int res = CopyableMessageBox.Show("Current package has unsaved changes.\nSave now?",
                        myName, CopyableMessageBoxButtons.YesNoCancel, CopyableMessageBoxIcon.Warning, 2);/**///Causes error on Application.Exit();... so use this.Close();
                    if (res == 2) return;
                    if (res == 0)
                    {
                        if (this.ReadWrite) { if (!this.fileSave()) return; }
                        else { if (!this.fileSaveAs()) return; }
                    }
	                this.IsPackageDirty = false;
                }
                if (this.package != null) this.package.Dispose();//Package.ClosePackage(0, package);

	            this.package = value;
	            this.OnPackageChanged(this, new EventArgs());
            }
        }

	    protected virtual void OnPackageChanged(object sender, EventArgs e)
	    {
		    if (this.PackageChanged != null)
		    {
			    this.PackageChanged(sender, e);
		    }
	    }
        
		#endregion

        #region Current Resource
        
		string resourceName = "";
        HelperManager helpers;

        Exception resException;
        IResource resource;
        bool resourceIsDirty;
        
		void resource_ResourceChanged(object sender, EventArgs e)
        {
	        this.controlPanel1.CommitEnabled = this.resourceIsDirty = true;
        }

        #endregion

        #region Menu Bar

        private void menuBarWidget1_MBDropDownOpening(object sender, MenuBarWidget.MBDropDownOpeningEventArgs mn)
        {
            switch (mn.mn)
            {
                case MenuBarWidget.MD.MBF: break;
                case MenuBarWidget.MD.MBE:
		            this.editDropDownOpening(); break;
                case MenuBarWidget.MD.MBR:
		            this.resourceDropDownOpening(); break;
                case MenuBarWidget.MD.MBS: break;
                case MenuBarWidget.MD.MBH: break;
                case MenuBarWidget.MD.CMF:
		            this.filterContextMenuOpening(); break;
            }
        }

        #region File menu

        private void menuBarWidget1_MBFile_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBF_new:
		                this.fileNew(); break;
                    case MenuBarWidget.MB.MBF_open:
		                this.fileOpen(); break;
                    case MenuBarWidget.MB.MBF_save:
		                this.fileSave(); break;
                    case MenuBarWidget.MB.MBF_saveAs:
		                this.fileSaveAs(); break;
                    case MenuBarWidget.MB.MBF_saveCopyAs:
		                this.fileSaveCopyAs(); break;
                    case MenuBarWidget.MB.MBF_close:
		                this.fileClose(); break;
                    case MenuBarWidget.MB.MBF_setMaxRecent:
		                this.fileSetMaxRecent(); break;
                    case MenuBarWidget.MB.MBF_bookmarkCurrent:
		                this.fileBookmarkCurrent(); break;
                    case MenuBarWidget.MB.MBF_setMaxBookmarks:
		                this.fileSetMaxBookmarks(); break;
                    case MenuBarWidget.MB.MBF_organiseBookmarks:
		                this.fileOrganiseBookmarks(); break;
                    case MenuBarWidget.MB.MBF_exit:
		                this.fileExit(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void fileNew()
        {
	        this.Filename = "";
	        this.CurrentPackage = Package.NewPackage(0);
	        this.IsPackageDirty = true;
        }

        private void fileOpen()
        {
	        this.openFileDialog1.FileName = "";
	        this.openFileDialog1.FilterIndex = 1;

            // CAS demo path
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Electronic Arts", "The Sims 4 Create A Sim Demo", "Mods");
            if (Directory.Exists(path))
            {
	            this.openFileDialog1.CustomPlaces.Add(path);
            }
            
			// actual game path
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Electronic Arts", "The Sims 4", "Mods");
            if (Directory.Exists(path))
            {
	            this.openFileDialog1.CustomPlaces.Add(path);
            }

	        if (Properties.Settings.Default.CustomPlaces != null)
	        {
		        foreach (string place in Properties.Settings.Default.CustomPlaces)
		        {
			        this.openFileDialog1.CustomPlaces.Add(place);
		        }
	        }

            DialogResult dr = this.openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK)
            {
	            return;
            }

	        this.Filename = (this.openFileDialog1.ReadOnlyChecked ? "0:" : "1:") + this.openFileDialog1.FileName;
        }

        private bool fileSave()
        {
            if (this.CurrentPackage == null) return false;

            if (string.IsNullOrEmpty(this.Filename))
            {
	            return this.fileSaveAs();
            }

            Application.UseWaitCursor = true;
            Application.DoEvents();
	        try
	        {
		        this.CurrentPackage.SavePackage();
		        this.IsPackageDirty = false;
	        }
	        finally
	        {
		        Application.UseWaitCursor = false;
	        }
            return true;
        }

        private bool fileSaveAs()
        {
            if (this.CurrentPackage == null) return false;

	        this.saveAsFileDialog.FileName = "";
	        this.saveAsFileDialog.FilterIndex = 1;
            DialogResult dr = this.saveAsFileDialog.ShowDialog();
            if (dr != DialogResult.OK) return false;

            if (!string.IsNullOrEmpty(this.Filename) && Path.GetFullPath(this.saveAsFileDialog.FileName).Equals(Path.GetFullPath(this.Filename)))
            {
	            return this.fileSave();
            }

            Application.UseWaitCursor = true;
            Application.DoEvents();
	        try
	        {
		        this.CurrentPackage.SaveAs(this.saveAsFileDialog.FileName);
		        this.IsPackageDirty = false;
		        this.Filename = "1:" + this.saveAsFileDialog.FileName;
	        }
	        finally
	        {
		        Application.UseWaitCursor = false;
	        }
            return true;
        }

        private void fileSaveCopyAs()
        {
            if (this.CurrentPackage == null) return;

	        this.saveAsFileDialog.FileName = "";
	        this.saveAsFileDialog.FilterIndex = 1;
            DialogResult dr = this.saveAsFileDialog.ShowDialog();
            if (dr != DialogResult.OK) return;

            Application.UseWaitCursor = true;
            Application.DoEvents();
	        try
	        {
		        this.CurrentPackage.SaveAs(this.saveAsFileDialog.FileName);
	        }
	        finally
	        {
		        Application.UseWaitCursor = false;
	        }
        }

        private void fileClose()
        {
	        this.Filename = "";
        }

        private void menuBarWidget1_MRUClick(object sender, MenuBarWidget.MRUClickEventArgs filename)
        {
	        this.Filename = filename.filename;
        }

        private void fileSetMaxRecent()
        {
            GetNumberDialog gnd = new GetNumberDialog("Max number of files:", "Recent Files list", 0, 9,
                Properties.Settings.Default.MRUListSize);
            DialogResult dr = gnd.ShowDialog();
            if (dr != DialogResult.OK) return;
            Properties.Settings.Default.MRUListSize = (short)gnd.Value;
        }

        private void menuBarWidget1_BookmarkClick(object sender, MenuBarWidget.BookmarkClickEventArgs filename)
        {
	        this.Filename = filename.filename;
        }

        private void fileBookmarkCurrent()
        {
	        this.menuBarWidget.AddBookmark((this.ReadWrite ? "1:" : "0:") + this.Filename);
        }

        private void fileSetMaxBookmarks()
        {
            GetNumberDialog gnd = new GetNumberDialog("Max number of files:", "Bookmark list", 0, 9,
                Properties.Settings.Default.BookmarkSize);
            DialogResult dr = gnd.ShowDialog();
            if (dr != DialogResult.OK) return;
            Properties.Settings.Default.BookmarkSize = (short)gnd.Value;
        }

        private void fileOrganiseBookmarks()
        {
	        this.settingsOrganiseBookmarks();
        }

        private void fileExit()
        {
            this.Close();
        }

        #endregion

        #region Edit menu

        private void editDropDownOpening()
        {
            Application.DoEvents();
            bool enable = this.resException == null && this.resource != null;
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBE_copy, enable && this.canCopy());
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBE_save, enable && this.canSavePreview());
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBE_float, enable && this.canFloat());
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBE_ote, enable && this.canOTE());
        }

        private void menuBarWidget1_MBEdit_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBE_copy:
		                this.editCopy(); break;
                    case MenuBarWidget.MB.MBE_save:
		                this.editSavePreview(); break;
                    case MenuBarWidget.MB.MBE_float:
		                this.editFloat(); break;
                    case MenuBarWidget.MB.MBE_ote:
		                this.editOTE(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        bool canCopy()
        {
            if (this.pnAuto.Controls.Count != 1) return false;
            if (this.pnAuto.Controls[0] is RichTextBox) return (this.pnAuto.Controls[0] as RichTextBox).SelectedText.Length > 0;
            if (this.pnAuto.Controls[0] is PictureBox) return (this.pnAuto.Controls[0] as PictureBox).Image != null;
            return false;
        }
        private void editCopy()
        {
            if (this.pnAuto.Controls.Count != 1) return;

            if (this.pnAuto.Controls[0] is PictureBox && (this.pnAuto.Controls[0] as PictureBox).Image != null)
            {
                Clipboard.SetImage((this.pnAuto.Controls[0] as PictureBox).Image);
                return;
            }

            string selectedText = "";
            if (this.pnAuto.Controls[0] is RichTextBox) selectedText = (this.pnAuto.Controls[0] as RichTextBox).SelectedText;
            else return;
            if (selectedText.Length == 0) return;

            StringBuilder s = new StringBuilder();
            TextReader t = new StringReader(selectedText);
            for (var line = t.ReadLine(); line != null; line = t.ReadLine()) s.AppendLine(line);
            Clipboard.SetText(s.ToString(), TextDataFormat.UnicodeText);
        }

        bool canSavePreview()
        {
            if (this.browserWidget1.SelectedResource as AResourceIndexEntry == null) return false;
            if (this.pnAuto.Controls.Count < 1) return false;

            if (this.pnAuto.Controls[0] is RichTextBox) return true;
            return false;
        }
        private void editSavePreview()
        {
            if (!this.canSavePreview()) return;
            TGIN tgin = this.browserWidget1.SelectedResource as AResourceIndexEntry;
            tgin.ResName = this.resourceName;

            SaveFileDialog sfd = new SaveFileDialog
								 {
                DefaultExt = this.pnAuto.Controls[0] is RichTextBox ? ".txt" : ".hex",
                AddExtension = true,
                CheckPathExists = true,
                FileName = tgin + (this.pnAuto.Controls[0] is RichTextBox ? ".txt" : ".hex"),
                Filter = this.pnAuto.Controls[0] is RichTextBox ? "Text documents (*.txt*)|*.txt|All files (*.*)|*.*" : "Hex dumps (*.hex)|*.hex|All files (*.*)|*.*",
                FilterIndex = 1,
                OverwritePrompt = true,
                Title = "Save preview content",
                ValidateNames = true
            };
            DialogResult dr = sfd.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            using (StreamWriter sw = new StreamWriter(sfd.FileName))
            {
                if (this.pnAuto.Controls[0] is RichTextBox) { sw.Write((this.pnAuto.Controls[0] as RichTextBox).Text.Replace("\n", "\r\n")); }
                sw.Flush();
                sw.Close();
            }
        }

        bool canFloat()
        {
            return true;
        }
        private void editFloat()
        {
            if (!this.controlPanel1.HexOnly && this.controlPanel1.AutoPreview) this.controlPanel1_PreviewClick(null, EventArgs.Empty);
            else this.controlPanel1_HexClick(null, EventArgs.Empty);
        }

        bool canOTE()
        {
            if (!this.hasTextEditor) return false;
            if (this.pnAuto.Controls.Count != 1) return false;
            if (this.pnAuto.Controls[0] is RichTextBox) return true;
            return false;
        }
        private void editOTE()
        {
            if (!this.hasTextEditor) return;
            if (this.pnAuto.Controls.Count != 1) return;

            string text = "";
            if (this.pnAuto.Controls[0] is RichTextBox) { text = (this.pnAuto.Controls[0] as RichTextBox).Text; }
            else return;

            StringBuilder s = new StringBuilder();
            TextReader t = new StringReader(text);
            for (var line = t.ReadLine(); line != null; line = t.ReadLine()) s.AppendLine(line);
            text = s.ToString();

            UTF8Encoding utf8 = new UTF8Encoding();
            UnicodeEncoding unicode = new UnicodeEncoding();
            byte[] utf8Bytes = Encoding.Convert(unicode, utf8, unicode.GetBytes(text));

            string command = Properties.Settings.Default.TextEditorCmd;
            string filename = String.Format("{0}{1}{2}.txt", Path.GetTempPath(), tempName, FNV64.GetHash(DateTime.UtcNow.ToString("O")).ToString("X16"));
            using (BinaryWriter w = new BinaryWriter(new FileStream(filename, FileMode.Create), Encoding.UTF8))
            {
                w.Write(utf8Bytes);
                w.Close();
            }
            File.SetAttributes(filename, FileAttributes.ReadOnly | FileAttributes.Temporary);

            Process p = new Process();

            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = filename;
            p.StartInfo.UseShellExecute = false;
            p.Exited += this.p_Exited;
            p.EnableRaisingEvents = true;

            try { p.Start(); }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, String.Format("Application failed to start:\n{0}\n{1}", command, filename), "Launch failed");
                File.SetAttributes(filename, FileAttributes.Normal);
                File.Delete(filename);
            }
        }

        void p_Exited(object sender, EventArgs e)
        {
            Process p = sender as Process;
            File.SetAttributes(p.StartInfo.Arguments, FileAttributes.Normal);
            File.Delete(p.StartInfo.Arguments);

	        this.MakeFormVisible();
        }

        void cleanUpTemp()
        {
            foreach (var file in Directory.GetFiles(Path.GetTempPath(), String.Format("{0}*.txt", tempName)))
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch { }
        }
        #endregion

        #region Resource menu
        private void resourceDropDownOpening()
        {
            bool multiSelection = this.browserWidget1.SelectedResources.Count != 0;
            bool singleSelection = this.browserWidget1.SelectedResource != null;
            //menuBarWidget1.Enable(MenuBarWidget.MB.MBR_add, true);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_copy, multiSelection);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_paste, this.canPasteResource());
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_duplicate, singleSelection);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_replace, singleSelection);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_compressed, multiSelection);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_isdeleted, multiSelection);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_details, singleSelection);
            //menuBarWidget1.Enable(MenuBarWidget.MB.MBR_selectAll, true);
            //http://private/s4pi/index.php?topic=1188.msg6889#msg6889
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_copyRK, singleSelection);
            //menuBarWidget1.Enable(MenuBarWidget.MB.MBR_importResources, true);
            //menuBarWidget1.Enable(MenuBarWidget.MB.MBR_importPackages, true);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_replaceFrom, multiSelection);
            //menuBarWidget1.Enable(MenuBarWidget.MB.MBR_importAsDBC, true);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_exportResources, multiSelection);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_exportToPackage, multiSelection);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_hexEditor, singleSelection && this.hasHexEditor);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_textEditor, singleSelection && this.hasTextEditor);

            CheckState res = this.CompressedCheckState();
            if (res == CheckState.Indeterminate)
            {
	            this.menuBarWidget.Indeterminate(MenuBarWidget.MB.MBR_compressed);
            }
            else
            {
	            this.menuBarWidget.Checked(MenuBarWidget.MB.MBR_compressed, res == CheckState.Checked);
            }

            res = this.IsDeletedCheckState();
            if (res == CheckState.Indeterminate)
            {
	            this.menuBarWidget.Indeterminate(MenuBarWidget.MB.MBR_isdeleted);
            }
            else
            {
	            this.menuBarWidget.Checked(MenuBarWidget.MB.MBR_isdeleted, res == CheckState.Checked);
            }

        }

        bool canPasteResource()
        {
            return this.CurrentPackage != null &&
                (
                Clipboard.ContainsData(myDataFormatSingleFile)
                || Clipboard.ContainsData(myDataFormatBatch)
                || Clipboard.ContainsFileDropList()
                //|| Clipboard.ContainsText()
                );
        }

        private CheckState CompressedCheckState()
        {
            if (this.browserWidget1.SelectedResources.Count == 0)
                return CheckState.Unchecked;
	        if (this.browserWidget1.SelectedResources.Count == 1)
		        return this.browserWidget1.SelectedResource.Compressed != 0 ? CheckState.Checked : CheckState.Unchecked;

	        int state = 0;
            foreach (IResourceIndexEntry rie in this.browserWidget1.SelectedResources) if (rie.Compressed != 0) state++;
            if (state == 0 || state == this.browserWidget1.SelectedResources.Count)
                return state == this.browserWidget1.SelectedResources.Count ? CheckState.Checked : CheckState.Unchecked;

            return CheckState.Indeterminate;
        }
        private CheckState IsDeletedCheckState()
        {
            if (this.browserWidget1.SelectedResources.Count == 0)
                return CheckState.Unchecked;
	        if (this.browserWidget1.SelectedResources.Count == 1)
		        return this.browserWidget1.SelectedResource.IsDeleted ? CheckState.Checked : CheckState.Unchecked;

	        int state = 0;
            foreach (IResourceIndexEntry rie in this.browserWidget1.SelectedResources) if (rie.IsDeleted) state++;
            if (state == 0 || state == this.browserWidget1.SelectedResources.Count)
                return state == this.browserWidget1.SelectedResources.Count ? CheckState.Checked : CheckState.Unchecked;

            return CheckState.Indeterminate;
        }

        private void menuBarWidget1_MBResource_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                //this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBR_add:
		                this.resourceAdd(); break;
                    case MenuBarWidget.MB.MBR_copy:
		                this.resourceCopy(); break;
                    case MenuBarWidget.MB.MBR_paste:
		                this.resourcePaste(); break;
                    case MenuBarWidget.MB.MBR_duplicate:
		                this.resourceDuplicate(); break;
                    case MenuBarWidget.MB.MBR_replace:
		                this.resourceReplace(); break;
                    case MenuBarWidget.MB.MBR_compressed:
		                this.resourceCompressed(); break;
                    case MenuBarWidget.MB.MBR_isdeleted:
		                this.resourceIsDeleted(); break;
                    case MenuBarWidget.MB.MBR_details:
		                this.resourceDetails(); break;
                    case MenuBarWidget.MB.MBR_selectAll:
		                this.resourceSelectAll(); break;
                    case MenuBarWidget.MB.MBR_copyRK:
		                this.resourceCopyRK(); break;
                    case MenuBarWidget.MB.MBR_importResources:
		                this.resourceImport(); break;
                    case MenuBarWidget.MB.MBR_importPackages:
		                this.resourceImportPackages(); break;
                    case MenuBarWidget.MB.MBR_replaceFrom:
		                this.resourceReplaceFrom(); break;
                    case MenuBarWidget.MB.MBR_importAsDBC:
		                this.resourceImportAsDBC(); break;
                    case MenuBarWidget.MB.MBR_exportResources:
		                this.resourceExport(); break;
                    case MenuBarWidget.MB.MBR_exportToPackage:
		                this.resourceExportToPackage(); break;
                    case MenuBarWidget.MB.MBR_hexEditor:
		                this.resourceHexEdit(); break;
                    case MenuBarWidget.MB.MBR_textEditor:
		                this.resourceTextEdit(); break;
                }
            }
            finally { /*this.Enabled = true;/**/ }
        }

        private void resourceAdd()
        {
            ResourceDetails ir = new ResourceDetails(/*20120820 CurrentPackage.Find(x => x.ResourceType == 0x0166038C) != null/**/true, false);
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            IResourceIndexEntry rie = this.NewResource(ir, null, ir.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, ir.Compress);
            if (rie == null) return;

	        this.browserWidget1.Add(rie);
	        this.package.ReplaceResource(rie, this.resource);//Ensure there's an actual resource in the package

            if (ir.UseName && ir.ResourceName != null && ir.ResourceName.Length > 0)
	            this.browserWidget1.ResourceName(ir.Instance, ir.ResourceName, true, ir.AllowRename);
        }

        //private void resourceCut() { resourceCopy(); if (browserWidget1.SelectedResource != null) package.DeleteResource(browserWidget1.SelectedResource); }

        private void resourceCopy()
        {
            if (this.browserWidget1.SelectedResources.Count == 0) return;

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                if (this.browserWidget1.SelectedResources.Count == 1)
                {
                    myDataFormat d = new myDataFormat();
                    d.tgin = this.browserWidget1.SelectedResource as AResourceIndexEntry;
                    d.tgin.ResName = this.resourceName;
                    d.data = WrapperDealer.GetResource(0, this.CurrentPackage, this.browserWidget1.SelectedResource, true).AsBytes;//Don't need wrapper

                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    formatter.Serialize(ms, d);
                    DataFormats.Format f = DataFormats.GetFormat(myDataFormatSingleFile);
                    Clipboard.SetData(myDataFormatSingleFile, ms);
                }
                else
                {
                    List<myDataFormat> l = new List<myDataFormat>();
                    foreach (IResourceIndexEntry rie in this.browserWidget1.SelectedResources)
                    {
                        myDataFormat d = new myDataFormat();
                        d.tgin = rie as AResourceIndexEntry;
                        d.tgin.ResName = this.browserWidget1.ResourceName(rie);
                        d.data = WrapperDealer.GetResource(0, this.CurrentPackage, rie, true).AsBytes;//Don't need wrapper
                        l.Add(d);
                    }

                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    formatter.Serialize(ms, l);
                    DataFormats.Format f = DataFormats.GetFormat(myDataFormatBatch);
                    Clipboard.SetData(myDataFormatBatch, ms);
                }
            }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        // For "resourcePaste()" see Import/Import.cs

        private void resourceDuplicate()
        {
            if (this.resource == null) return;
            byte[] buffer = this.resource.AsBytes;
            MemoryStream ms = new MemoryStream();
            ms.Write(buffer, 0, buffer.Length);

            IResourceIndexEntry rie = this.CurrentPackage.AddResource(this.browserWidget1.SelectedResource, ms, false);
            rie.Compressed = this.browserWidget1.SelectedResource.Compressed;

            IResource res = WrapperDealer.GetResource(0, this.CurrentPackage, rie, true);//Don't need wrapper
	        this.package.ReplaceResource(rie, res); // Commit new resource to package
	        this.IsPackageDirty = true;

	        this.browserWidget1.Add(rie);
        }

        private void resourceCompressed()
        {
            ushort target = 0x5A42;
            if (this.CompressedCheckState() == CheckState.Checked) target = 0;
            foreach (IResourceIndexEntry rie in this.browserWidget1.SelectedResources)
            {
	            this.IsPackageDirty = !this.isPackageDirty || rie.Compressed != target;
                rie.Compressed = target;
            }
        }

        private void resourceIsDeleted()
        {
            bool target = true;
            if (this.IsDeletedCheckState() == CheckState.Checked) target = false;
            foreach (IResourceIndexEntry rie in this.browserWidget1.SelectedResources)
            {
	            this.IsPackageDirty = !this.isPackageDirty || rie.IsDeleted != target;
                rie.IsDeleted = target;
            }
        }

        private void resourceDetails()
        {
            if (this.browserWidget1.SelectedResource == null) return;

            ResourceDetails ir = new ResourceDetails(this.resourceName != null && this.resourceName.Length > 0, false, this.browserWidget1.SelectedResource);
            ir.Compress = this.browserWidget1.SelectedResource.Compressed != 0;
            if (ir.UseName) ir.ResourceName = this.resourceName;

            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

	        this.browserWidget1.ResourceKey = ir;
	        this.browserWidget1.SelectedResource.Compressed = (ushort)(ir.Compress ? 0x5A42 : 0);

            if (ir.UseName && ir.ResourceName != null && ir.ResourceName.Length > 0)
	            this.browserWidget1.ResourceName(ir.Instance, ir.ResourceName, true, ir.AllowRename);

	        this.IsPackageDirty = true;
        }

        private void resourceSelectAll()
        {
	        this.browserWidget1.SelectAll();
        }

        private void resourceCopyRK()
        {
            //http://dino.drealm.info/develforums/s4pi/index.php?topic=1188.msg6889#msg6889
            if (this.browserWidget1.SelectedResources.Count != 1) return;
            Clipboard.SetText(String.Join("\r\n", this.browserWidget1.SelectedResources.Select(r => r.ToString())));
        }

        private void resourceReplace()
        {
            IResourceIndexEntry rie = this.browserWidget1.SelectedResource;
            if (rie == null) return;

            List<string> ext;
            string resType = "0x" + rie.ResourceType.ToString("X8");
            if (ExtList.Ext.ContainsKey(resType)) ext = ExtList.Ext[resType];
            else ext = ExtList.Ext["*"];

	        this.replaceResourceDialog.Filter = ext[0] + " by type|S4_" + rie.ResourceType.ToString("X8") + "*.*" +
                "|" + ext[0] + " by ext|*" + ext[ext.Count - 1] +
                "|All files|*.*";
            int i = Properties.Settings.Default.ResourceReplaceFilterIndex;
	        this.replaceResourceDialog.FilterIndex = (i >= 0 && i < 3) ? Properties.Settings.Default.ResourceReplaceFilterIndex + 1 : 1;
	        this.replaceResourceDialog.FileName = this.replaceResourceDialog.Filter.Split('|')[i * 2 + 1];
            DialogResult dr = this.replaceResourceDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            Properties.Settings.Default.ResourceReplaceFilterIndex = this.replaceResourceDialog.FilterIndex - 1;

            IResource res;
            try
            {
                res = this.ReadResource(this.replaceResourceDialog.FileName);
            }
            catch (Exception ex)
            {
                IssueException(ex, "Could not open file:\n" + this.replaceResourceDialog.FileName + ".\nNo changes made.");
                return;
            }

            // Reload the resource we just replaced as there's no way to get a changed trigger from it
	        this.SuspendLayout();
	        this.browserWidget1.SelectedResource = null;

	        this.package.ReplaceResource(rie, res);
	        this.resourceIsDirty = this.controlPanel1.CommitEnabled = false;
	        this.IsPackageDirty = true;

	        this.browserWidget1.SelectedResource = rie;
	        this.ResumeLayout();
        }

        // For "resourceImport()", see Import/Import.cs
        // For "resourceImportPackages()", see Import/Import.cs

        /// <summary>
        /// How to handle duplicate resources when adding to a package
        /// </summary>
        enum DuplicateHandling
        {
            /// <summary>
            /// Refuse to create the request resource
            /// </summary>
            reject,
            /// <summary>
            /// Delete any conflicting resource
            /// </summary>
            replace,
            /// <summary>
            /// Ignore any conflicting resource
            /// </summary>
            allow
        }
        private IResourceIndexEntry NewResource(IResourceKey rk, MemoryStream ms, DuplicateHandling dups, bool compress)
        {
            IResourceIndexEntry rie = this.CurrentPackage.Find(x => rk.Equals(x));
            if (rie != null)
            {
                if (dups == DuplicateHandling.reject) return null;
                if (dups == DuplicateHandling.replace) this.CurrentPackage.DeleteResource(rie);
            }

            rie = this.CurrentPackage.AddResource(rk, ms, false);//we do NOT want to search again...
            if (rie == null) return null;

            rie.Compressed = (ushort)(compress ? 0x5A42 : 0);

	        this.IsPackageDirty = true;

            return rie;
        }

        private IResource ReadResource(string filename)
        {
            MemoryStream ms = new MemoryStream();
            using (BinaryReader br = new BinaryReader(new FileStream(filename, FileMode.Open)))
            {
                ms.Write(br.ReadBytes((int)br.BaseStream.Length), 0, (int)br.BaseStream.Length);
                br.Close();
            }

            IResource rres = WrapperDealer.CreateNewResource(0, "*");
            ConstructorInfo ci = rres.GetType().GetConstructor(new[] { typeof(int), typeof(Stream) });
            return (IResource)ci.Invoke(new object[] { 0, ms });
        }

        private void resourceExport()
        {
            if (this.browserWidget1.SelectedResources.Count > 1) {
	            this.exportBatch(); return; }

            if (this.browserWidget1.SelectedResource as AResourceIndexEntry == null) return;
            TGIN tgin = this.browserWidget1.SelectedResource as AResourceIndexEntry;
            tgin.ResName = this.resourceName;

	        this.exportFileDialog.FileName = tgin;
	        this.exportFileDialog.InitialDirectory = Properties.Settings.Default.LastExportPath;

            DialogResult dr = this.exportFileDialog.ShowDialog();
            if (dr != DialogResult.OK) return;
            Properties.Settings.Default.LastExportPath = Path.GetDirectoryName(this.exportFileDialog.FileName);

            Application.UseWaitCursor = true;
            Application.DoEvents();
            try {
	            this.exportFile(this.browserWidget1.SelectedResource, this.exportFileDialog.FileName); }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        private void exportBatch()
        {
	        this.exportBatchTarget.SelectedPath = Properties.Settings.Default.LastExportPath;
            DialogResult dr = this.exportBatchTarget.ShowDialog();
            if (dr != DialogResult.OK) return;
            Properties.Settings.Default.LastExportPath = this.exportBatchTarget.SelectedPath;

            Application.UseWaitCursor = true;
            Application.DoEvents();
            bool overwriteAll = false;
            bool skipAll = false;
            try
            {
                foreach (IResourceIndexEntry rie in this.browserWidget1.SelectedResources)
                {
                    if (rie as AResourceIndexEntry == null) continue;
                    TGIN tgin = rie as AResourceIndexEntry;
                    tgin.ResName = this.browserWidget1.ResourceName(rie);
                    string file = Path.Combine(this.exportBatchTarget.SelectedPath, tgin);
                    if (File.Exists(file))
                    {
                        if (skipAll) continue;
                        if (!overwriteAll)
                        {
                            Application.UseWaitCursor = false;
                            int i = CopyableMessageBox.Show("Overwrite file?\n" + file, myName, CopyableMessageBoxIcon.Question,
                                new List<string>(new[] { "&No", "N&o to all", "&Yes", "Y&es to all", "&Abandon" }), 0, 4);
                            if (i == 0) continue;
                            if (i == 1) { skipAll = true; continue; }
                            if (i == 3) overwriteAll = true;
                            if (i == 4) return;
                        }
                        Application.UseWaitCursor = true;
                    }
	                this.exportFile(rie, file);
                }
            }
            finally { Application.UseWaitCursor = false; Application.DoEvents(); }
        }

        private void exportFile(IResourceIndexEntry rie, string filename)
        {
            IResource res = WrapperDealer.GetResource(0, this.CurrentPackage, rie, true);//Don't need wrapper
            Stream s = res.Stream;
            s.Position = 0;
            if (s.Length != rie.Memsize) CopyableMessageBox.Show(String.Format("Resource stream has {0} bytes; index entry says {1}.", s.Length, rie.Memsize));
            BinaryWriter w = new BinaryWriter(new FileStream(filename, FileMode.Create));
            w.Write((new BinaryReader(s)).ReadBytes((int)s.Length));
            w.Close();
        }

        private void resourceExportToPackage()
        {
            DialogResult dr = this.exportToPackageDialog.ShowDialog();
            if (dr != DialogResult.OK) return;

            if (this.Filename != null && this.Filename.Length > 0 && Path.GetFullPath(this.Filename).Equals(Path.GetFullPath(this.exportToPackageDialog.FileName)))
            {
                CopyableMessageBox.Show("Target must not be same as source.", "Export to package", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                return;
            }

            bool isNew = false;
            IPackage target;
            if (!File.Exists(this.exportToPackageDialog.FileName))
            {
                try
                {
                    target = Package.NewPackage(0);
                    target.SaveAs(this.exportToPackageDialog.FileName);
                    target.Dispose(); // Package.ClosePackage(0, target);
                    isNew = true;
                }
                catch (Exception ex)
                {
                    IssueException(ex, "Export cannot begin.  Could not create target package:\n" + this.exportToPackageDialog.FileName);
                    return;
                }
            }

            bool replace = false;
            try
            {
                target = Package.OpenPackage(0, this.exportToPackageDialog.FileName, true);

                if (!isNew)
                {
                    int res = CopyableMessageBox.Show(
                        "Do you want to replace any duplicate resources in the target package discovered during export?",
                        "Export to package", CopyableMessageBoxIcon.Question, new List<string>(new[] { "Re&ject", "Re&place", "&Abandon" }), 0, 2);
                    if (res == 2) return;
                    replace = res == 0;
                }

            }
            catch (InvalidDataException idex)
            {
                if (idex.Message.Contains("magic tag"))
                {
                    CopyableMessageBox.Show(
                        "Export cannot begin.  Could not open target package:\n" + this.exportToPackageDialog.FileName + "\n\n" +
                        "This file does not contain the expected package identifier in the header.\n" +
                        "This could be because it is a protected package (e.g. a Store item).\n\n" +
                        idex.Message, myName + ": Unable to open file", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                }
                else if (idex.Message.Contains("major version"))
                {
                    CopyableMessageBox.Show(
                        "Export cannot begin.  Could not open target package:\n" + this.exportToPackageDialog.FileName + "\n\n" +
                        "This file does not contain the expected package major version number in the header.\n" +
                        "This could be because it is a package for Sims2 or Spore.\n\n" +
                        idex.Message, myName + ": Unable to open file", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                }
                else
                {
                    IssueException(idex, "Export cannot begin.  Could not open target package:\n" + this.exportToPackageDialog.FileName);
                }
                return;
            }
            catch (Exception ex)
            {
                IssueException(ex, "Export cannot begin.  Could not open target package:\n" + this.exportToPackageDialog.FileName);
                return;
            }

            // http://private/s4pi/index.php?topic=1377
            Dictionary<ulong, string> sourceNames = new Dictionary<ulong, string>();
            try
            {
                Application.UseWaitCursor = true;
	            this.lbProgress.Text = "Exporting to " + Path.GetFileNameWithoutExtension(this.exportToPackageDialog.FileName) + "...";
                Application.DoEvents();

	            this.progressBar1.Value = 0;
	            this.progressBar1.Maximum = this.browserWidget1.SelectedResources.Count;
                foreach (IResourceIndexEntry rie in this.browserWidget1.SelectedResources)
                {
                    if (!sourceNames.ContainsKey(rie.Instance))
                    {
                        string name = this.browserWidget1.ResourceName(rie);
                        if (name != "")
                            sourceNames.Add(rie.Instance, name);
                    }
	                this.exportResourceToPackage(target, rie, replace);
	                this.progressBar1.Value++;
                    if (this.progressBar1.Value % 100 == 0)
                        Application.DoEvents();
                }

                if (sourceNames.Count > 0)
                {
                    IResourceIndexEntry nmrie = target.Find(_key => _key.ResourceType == 0x0166038C);
                    if (nmrie == null)
                    {
                        TGIBlock newnmrk = new TGIBlock(0, null, 0x0166038C, 0, FNV64.GetHash(this.exportToPackageDialog.FileName + DateTime.Now));
                        nmrie = target.AddResource(newnmrk, null, true);
                        if (nmrie == null)
                        {
                            Application.UseWaitCursor = false;

                            CopyableMessageBox.Show(
                                "Name map could not be created in target package.\n\nNames will not be exported.",
                                "Export to package", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);

                            Application.UseWaitCursor = true;
                            Application.DoEvents();
                        }
                    }

                    if (nmrie != null)
                    {
                        IResource newnmap = WrapperDealer.GetResource(0, target, nmrie);
                        if (newnmap != null && typeof(IDictionary<ulong, string>).IsAssignableFrom(newnmap.GetType()))
                        {
                            IDictionary<ulong, string> targetNames = (IDictionary<ulong, string>)newnmap;
                            sourceNames.Where(_kv => targetNames.ContainsKey(_kv.Key))
                                .Select(_kv => targetNames[_kv.Key] = _kv.Value);
                            foreach (var _kv in sourceNames)
                                if (!targetNames.ContainsKey(_kv.Key))
                                    targetNames.Add(_kv);
                            target.ReplaceResource(nmrie, newnmap);
                        }
                    }
                }

	            this.progressBar1.Value = 0;

	            this.lbProgress.Text = "Saving...";
                Application.DoEvents();
                target.SavePackage();
                target.Dispose(); //Package.ClosePackage(0, target);
            }
            finally { target.Dispose();
	            this.lbProgress.Text = ""; Application.UseWaitCursor = false; Application.DoEvents(); } //Package.ClosePackage(0, target)
        }

        private void exportResourceToPackage(IPackage tgtpkg, IResourceIndexEntry srcrie, bool replace)
        {
            IResourceIndexEntry rie = tgtpkg.Find(x => ((IResourceKey)srcrie).Equals(x));
            if (rie != null)
            {
                if (!replace) return;
                tgtpkg.DeleteResource(rie);
            }

            rie = tgtpkg.AddResource(srcrie, null, true);
            if (rie == null) return;
            rie.Compressed = srcrie.Compressed;

            IResource srcres = WrapperDealer.GetResource(0, this.CurrentPackage, srcrie, true);//Don't need wrapper
            tgtpkg.ReplaceResource(rie, srcres);
        }

        private void resourceHexEdit()
        {
            if (this.resource == null) return;
	        this.HexEdit(this.browserWidget1.SelectedResource, this.resource);
        }

        private void resourceTextEdit()
        {
            if (this.resource == null) return;
	        this.TextEdit(this.browserWidget1.SelectedResource, this.resource);
        }

        private void menuBarWidget1_HelperClick(object sender, MenuBarWidget.HelperClickEventArgs helper)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
	            this.do_HelperClick(helper.helper);
            }
            finally { this.Enabled = true; }
        }
        #endregion

        #region Tools menu
        private void menuBarWidget1_MBTools_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBT_fnvHash:
		                this.toolsFNV(); break;
                    case MenuBarWidget.MB.MBT_search:
		                this.toolsSearch(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void toolsFNV()
        {
            FNVHashDialog fnvForm = new FNVHashDialog();
            fnvForm.Show();
        }

        private void toolsSearch()
        {
            SearchForm searchForm = new SearchForm();
            searchForm.Width = this.Width * 4 / 5;
            searchForm.Height = this.Height * 4 / 5;
            searchForm.CurrentPackage = this.CurrentPackage;
            searchForm.Go += this.searchForm_Go;
            searchForm.Show();
        }

        void searchForm_Go(object sender, SearchForm.GoEventArgs e)
        {
	        this.browserWidget1.SelectedResource = e.ResourceIndexEntry;
        }
        #endregion

        #region Settings menu
        private void menuBarWidget1_MBSettings_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBS_updates:
		                this.settingsAutomaticUpdates(); break;
                    case MenuBarWidget.MB.MBS_previewDDS:
		                this.settingsEnableDDSPreview(); break;
                    case MenuBarWidget.MB.MBS_fallbackTextPreview:
		                this.settingseEnableFallbackTextPreview(); break;
                    case MenuBarWidget.MB.MBS_fallbackHexPreview:
		                this.settingseEnableFallbackHexPreview(); break;
                    case MenuBarWidget.MB.MBS_askAutoSaveDBC:
		                this.settingsMBS_askAutoSaveDBC(); break;
                    case MenuBarWidget.MB.MBS_bookmarks:
		                this.settingsOrganiseBookmarks(); break;
					case MenuBarWidget.MB.MBS_customplaces:
						this.SettingsOrganiseCustomPlaces();
		                break;
                    case MenuBarWidget.MB.MBS_externals:
		                this.settingsExternalPrograms(); break;
                    case MenuBarWidget.MB.MBS_wrappers:
		                this.settingsManageWrappers(); break;
                    case MenuBarWidget.MB.MBS_saveSettings:
		                this.saveSettings(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void settingsAutomaticUpdates()
        {
            Checker.AutoUpdateChoice = !this.menuBarWidget.IsChecked(MenuBarWidget.MB.MBS_updates);
        }

        private void settingsEnableDDSPreview()
        {
            Properties.Settings.Default.EnableDDSPreview = !this.menuBarWidget.IsChecked(MenuBarWidget.MB.MBS_previewDDS);
	        this.menuBarWidget.Checked(MenuBarWidget.MB.MBS_previewDDS, Properties.Settings.Default.EnableDDSPreview);
            if (this.browserWidget1.SelectedResource != null)
	            this.controlPanel1_AutoChanged(this, EventArgs.Empty);
        }

        private void settingseEnableFallbackTextPreview()
        {
            Properties.Settings.Default.EnableFallbackTextPreview = !this.menuBarWidget.IsChecked(MenuBarWidget.MB.MBS_fallbackTextPreview);
	        this.menuBarWidget.Checked(MenuBarWidget.MB.MBS_fallbackTextPreview, Properties.Settings.Default.EnableFallbackTextPreview);
            if (Properties.Settings.Default.EnableFallbackTextPreview && Properties.Settings.Default.EnableFallbackHexPreview)
            {
                Properties.Settings.Default.EnableFallbackHexPreview = false;
	            this.menuBarWidget.Checked(MenuBarWidget.MB.MBS_fallbackHexPreview, false);
            }
            if (this.browserWidget1.SelectedResource != null)
	            this.controlPanel1_AutoChanged(this, EventArgs.Empty);
        }

        private void settingseEnableFallbackHexPreview()
        {
            Properties.Settings.Default.EnableFallbackHexPreview = !this.menuBarWidget.IsChecked(MenuBarWidget.MB.MBS_fallbackHexPreview);
	        this.menuBarWidget.Checked(MenuBarWidget.MB.MBS_fallbackHexPreview, Properties.Settings.Default.EnableFallbackHexPreview);
            if (Properties.Settings.Default.EnableFallbackTextPreview && Properties.Settings.Default.EnableFallbackHexPreview)
            {
                Properties.Settings.Default.EnableFallbackTextPreview = false;
	            this.menuBarWidget.Checked(MenuBarWidget.MB.MBS_fallbackTextPreview, false);
            }
            if (this.browserWidget1.SelectedResource != null)
	            this.controlPanel1_AutoChanged(this, EventArgs.Empty);
        }

        bool dbcWarningIssued;
        private void settingsMBS_askAutoSaveDBC()
        {
            if (!Properties.Settings.Default.AskDBCAutoSave && !this.dbcWarningIssued)
            {
	            this.dbcWarningIssued = true;
                if (CopyableMessageBox.Show("AutoSave during import of DBC packages is recommended.\n" +
                    "Are you sure you want to be prompted?", "Enable DBC AutoSave prompt",
                    CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Warning) != 0) return;
            }
            Properties.Settings.Default.AskDBCAutoSave = !this.menuBarWidget.IsChecked(MenuBarWidget.MB.MBS_askAutoSaveDBC);
	        this.menuBarWidget.Checked(MenuBarWidget.MB.MBS_askAutoSaveDBC, Properties.Settings.Default.AskDBCAutoSave);
        }

        private void settingsOrganiseBookmarks()
        {
            OrganiseBookmarksDialog obd = new OrganiseBookmarksDialog();
            obd.ShowDialog();
	        this.menuBarWidget.UpdateBookmarks();
        }

		private void SettingsOrganiseCustomPlaces()
		{
			var dialog = new OrganiseCustomPlacesDialog();
			dialog.ShowDialog(this);
		}

        bool hasHexEditor { get { return !string.IsNullOrEmpty(Properties.Settings.Default.HexEditorCmd); } }
        bool hasTextEditor { get { return !string.IsNullOrEmpty(Properties.Settings.Default.TextEditorCmd); } }
        
		private void settingsExternalPrograms()
        {
            ExternalProgramsDialog epd = new ExternalProgramsDialog();
            if (this.hasHexEditor)
            {
                epd.HasUserHexEditor = true;
                epd.UserHexEditor = Properties.Settings.Default.HexEditorCmd;
                epd.HexEditorIgnoreTS = Properties.Settings.Default.HexEditorIgnoreTS;
                epd.HexEditorWantsQuotes = Properties.Settings.Default.HexEditorWantsQuotes;
            }
            else
            {
                epd.HasUserHexEditor = false;
                epd.UserHexEditor = "";
                epd.HexEditorIgnoreTS = false;
                epd.HexEditorWantsQuotes = false;
            }
            if (this.hasTextEditor)
            {
                epd.HasUserTextEditor = true;
                epd.UserTextEditor = Properties.Settings.Default.TextEditorCmd;
                epd.TextEditorIgnoreTS = Properties.Settings.Default.TextEditorIgnoreTS;
                epd.TextEditorWantsQuotes = Properties.Settings.Default.TextEditorWantsQuotes;
            }
            else
            {
                epd.HasUserTextEditor = false;
                epd.UserTextEditor = "";
                epd.TextEditorIgnoreTS = false;
                epd.TextEditorWantsQuotes = false;
            }
            if (Properties.Settings.Default.DisabledHelpers == null)
                Properties.Settings.Default.DisabledHelpers = new StringCollection();

            string[] disabledHelpers = new string[Properties.Settings.Default.DisabledHelpers.Count];
            Properties.Settings.Default.DisabledHelpers.CopyTo(disabledHelpers, 0);
            epd.DisabledHelpers = disabledHelpers;
            DialogResult dr = epd.ShowDialog();
            if (dr != DialogResult.OK) return;
            if (epd.HasUserHexEditor && epd.UserHexEditor.Length > 0 && File.Exists(epd.UserHexEditor))
            {
                Properties.Settings.Default.HexEditorCmd = epd.UserHexEditor;
                Properties.Settings.Default.HexEditorIgnoreTS = epd.HexEditorIgnoreTS;
                Properties.Settings.Default.HexEditorWantsQuotes = epd.HexEditorWantsQuotes;
            }
            else
            {
                Properties.Settings.Default.HexEditorCmd = null;
                Properties.Settings.Default.HexEditorIgnoreTS = false;
                Properties.Settings.Default.HexEditorWantsQuotes = false;
            }
            if (epd.HasUserTextEditor && epd.UserTextEditor.Length > 0 && File.Exists(epd.UserTextEditor))
            {
                Properties.Settings.Default.TextEditorCmd = epd.UserTextEditor;
                Properties.Settings.Default.TextEditorIgnoreTS = epd.TextEditorIgnoreTS;
                Properties.Settings.Default.TextEditorWantsQuotes = epd.TextEditorWantsQuotes;
            }
            else
            {
                Properties.Settings.Default.TextEditorCmd = null;
                Properties.Settings.Default.TextEditorIgnoreTS = false;
                Properties.Settings.Default.TextEditorWantsQuotes = false;
            }
            disabledHelpers = epd.DisabledHelpers;
            if (disabledHelpers.Length == 0)
                Properties.Settings.Default.DisabledHelpers = null;
            else
            {
                Properties.Settings.Default.DisabledHelpers = new StringCollection();
                Properties.Settings.Default.DisabledHelpers.AddRange(epd.DisabledHelpers);
            }
            if (this.browserWidget1.SelectedResource != null && this.resource != null) {
	            this.setHexEditor();
	            this.setTextEditor();
	            this.setHelpers(); }
        }

        private void settingsManageWrappers()
        {
            new ManageWrappersDialog().ShowDialog();
            IResourceIndexEntry rie = this.browserWidget1.SelectedResource;
	        this.browserWidget1.SelectedResource = null;
	        this.browserWidget1.SelectedResource = rie;
        }

        private void saveSettings()
        {
	        this.OnSaveSettings(this, new EventArgs());
            Properties.Settings.Default.Save();
        }
        #endregion

        #region Help menu
        private void menuBarWidget1_MBHelp_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.MBH_contents:
		                this.helpContents(); break;
                    case MenuBarWidget.MB.MBH_about:
		                this.helpAbout(); break;
                    case MenuBarWidget.MB.MBH_update:
		                this.helpUpdate(); break;
                    case MenuBarWidget.MB.MBH_warranty:
		                this.helpWarranty(); break;
                    case MenuBarWidget.MB.MBH_licence:
		                this.helpLicence(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void helpContents()
        {
            string locale = CultureInfo.CurrentUICulture.Name;

            string baseFolder = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "HelpFiles");
            if (Directory.Exists(Path.Combine(baseFolder, locale)))
                baseFolder = Path.Combine(baseFolder, locale);
            else if (Directory.Exists(Path.Combine(baseFolder, locale.Substring(0, 2))))
                baseFolder = Path.Combine(baseFolder, locale.Substring(0, 2));

            Help.ShowHelp(this, "file:///" + Path.Combine(baseFolder, "Contents.htm"));
        }

        private void helpAbout()
        {
            string copyright = "\n" +
                myName + "Copyright (C) 2010 Peter L Jones, 2014 Keyi Zhang, aka Kuree\n" +
                "\n" +
                "This program comes with ABSOLUTELY NO WARRANTY; for details see Help->Warranty.\n" +
                "\n" +
                "This is free software, and you are welcome to redistribute it\n" +
                "under certain conditions; see Help->Licence for details.\n" +
                "\n" +
                "Special thanks to Peter L Jones, without whose work the program won't be done.\n";
            CopyableMessageBox.Show(String.Format(
                "{0}\n" +
                "Front-end Distribution: {1}\n" +
                "Library Distribution: {2}"
                , copyright
                , Version.CurrentVersion
                , Version.LibraryVersion
                ), myName);
        }

        private void helpUpdate()
        {
            bool msgDisplayed = Checker.GetUpdate(false);
            if (!msgDisplayed)
                CopyableMessageBox.Show("Your " + PortableSettingsProvider.ExecutableName + " is up to date", myName,
                    CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
        }

        private void helpWarranty()
        {
            CopyableMessageBox.Show("\n" +
                "Disclaimer of Warranty.\n" +
                "\n" +
                "THERE IS NO WARRANTY FOR THE PROGRAM, TO THE EXTENT PERMITTED BY APPLICABLE LAW." +
                " EXCEPT WHEN OTHERWISE STATED IN WRITING THE COPYRIGHT HOLDERS AND/OR OTHER" +
                " PARTIES PROVIDE THE PROGRAM \"AS IS\" WITHOUT WARRANTY OF ANY KIND," +
                " EITHER EXPRESSED OR IMPLIED, INCLUDING, BUT NOT LIMITED TO," +
                " THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE." +
                " THE ENTIRE RISK AS TO THE QUALITY AND PERFORMANCE OF THE PROGRAM IS WITH YOU." +
                " SHOULD THE PROGRAM PROVE DEFECTIVE, YOU ASSUME THE COST OF ALL NECESSARY SERVICING, REPAIR OR CORRECTION.\n" +
                "\n" +
                "\n" +
                "Limitation of Liability.\n" +
                "\n" +
                "IN NO EVENT UNLESS REQUIRED BY APPLICABLE LAW OR AGREED TO IN WRITING WILL ANY COPYRIGHT HOLDER," +
                " OR ANY OTHER PARTY WHO MODIFIES AND/OR CONVEYS THE PROGRAM AS PERMITTED ABOVE," +
                " BE LIABLE TO YOU FOR DAMAGES, INCLUDING ANY GENERAL, SPECIAL, INCIDENTAL OR CONSEQUENTIAL DAMAGES" +
                " ARISING OUT OF THE USE OR INABILITY TO USE THE PROGRAM" +
                " (INCLUDING BUT NOT LIMITED TO LOSS OF DATA OR DATA BEING RENDERED INACCURATE" +
                " OR LOSSES SUSTAINED BY YOU OR THIRD PARTIES OR A FAILURE OF THE PROGRAM TO OPERATE WITH ANY OTHER PROGRAMS)," +
                " EVEN IF SUCH HOLDER OR OTHER PARTY HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.\n" +
                "\n",
                myName);

        }

        private void helpLicence()
        {
            int dr = CopyableMessageBox.Show("\n" +
                "This program is distributed under the terms of the\nGNU General Public Licence version 3.\n" +
                "\n" +
                "If you wish to see the full text of the licence,\nplease visit http://www.fsf.org/licensing/licenses/gpl.html.\n" +
                "\n" +
                "Do you wish to visit this site now?" +
                "\n",
                myName,
                CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, 1);
            if (dr != 0) return;
            Help.ShowHelp(this, "http://www.fsf.org/licensing/licenses/gpl.html");
        }
        #endregion

        #region Filter context menu
        private void filterContextMenuOpening()
        {
            Application.DoEvents();
	        this.menuBarWidget.Enable(MenuBarWidget.MB.CMF_pasteRK, AResourceKey.TryParse(Clipboard.GetText(), new TGIBlock(0, null)));
        }

        private void menuBarWidget1_CMFilter_Click(object sender, MenuBarWidget.MBClickEventArgs mn)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();
                switch (mn.mn)
                {
                    case MenuBarWidget.MB.CMF_pasteRK:
		                this.filterPaste(); break;
                }
            }
            finally { this.Enabled = true; }
        }

        private void filterPaste()
        {
            TGIBlock value = new TGIBlock(0, null);
            if (!AResourceKey.TryParse(Clipboard.GetText(), value)) return;
	        this.resourceFilterWidget1.PasteResourceKey(value);
        }
        #endregion

        #endregion

        #region Browser Widget
        private void browserWidget1_SelectedResourceChanging(object sender, BrowserWidget.ResourceChangingEventArgs e)
        {
            if (this.resource == null) return;

	        this.resource.ResourceChanged -= this.resource_ResourceChanged;
            if (this.resourceIsDirty)
            {
                int dr = CopyableMessageBox.Show(
                    String.Format("Commit changes to {0}?",
                        e.name.Length > 0
                        ? e.name
                        : String.Format("TGI {0:X8}-{1:X8}-{2:X16}", this.browserWidget1.SelectedResource.ResourceType, this.browserWidget1.SelectedResource.ResourceGroup, this.browserWidget1.SelectedResource.Instance)
                    ), myName, CopyableMessageBoxButtons.YesNoCancel, CopyableMessageBoxIcon.Question, 1);
                if (dr == 2)
                {
                    e.Cancel = true;
                    return;
                }
                if (dr != 1)
	                this.controlPanel1_CommitClick(null, null);
            }
        }

        private void browserWidget1_SelectedResourceChanged(object sender, BrowserWidget.ResourceChangedEventArgs e)
        {
	        this.resourceName = e.name;
	        this.resource = null;
	        this.resException = null;
            if (this.browserWidget1.SelectedResource != null)
            {
                try
                {
	                this.resource = WrapperDealer.GetResource(0, this.CurrentPackage, this.browserWidget1.SelectedResource, this.controlPanel1.HexOnly);
                }
                catch (Exception ex)
                {
	                this.resException = ex;
                }
            }

            if (this.resource != null) this.resource.ResourceChanged += this.resource_ResourceChanged;

	        this.resourceIsDirty = this.controlPanel1.CommitEnabled = false;

	        this.menuBarWidget.ClearHelpers();

	        this.controlPanel1_AutoChanged(null, null);
            if (this.resource != null)
            {
	            this.controlPanel1.HexEnabled = true;
	            this.controlPanel1.ValueEnabled = this.hasStringValueContentField();
	            this.controlPanel1.GridEnabled = this.resource.ContentFields.Find(x => !x.Equals("AsBytes") && !x.Equals("Stream") && !x.Equals("Value")) != null;
	            this.setHexEditor();
	            this.setTextEditor();
	            this.setHelpers();
            }
            else
            {
	            this.controlPanel1.HexEnabled = this.controlPanel1.ValueEnabled = this.controlPanel1.GridEnabled = this.controlPanel1.Helper1Enabled = this.controlPanel1.Helper2Enabled = this.controlPanel1.HexEditEnabled = false;
	            this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_hexEditor, false);
	            this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_textEditor, false);
	            this.helpers = null;
            }

            bool selectedItems = this.resource != null || this.browserWidget1.SelectedResources.Count > 0; // one or more
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_exportResources, selectedItems);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_exportToPackage, selectedItems);
            //menuBarWidget1.Enable(MenuBarWidget.MB.MBE_cut, resource != null);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_copy, selectedItems);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_duplicate, this.resource != null);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_replace, this.resource != null);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_compressed, selectedItems);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_isdeleted, selectedItems);
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_details, this.resource != null);

	        this.resourceFilterWidget1.IndexEntry = this.browserWidget1.SelectedResource;
        }

        // Does the resource wrapper parse to a string value?
        bool hasStringValueContentField()
        {
            if (!this.resource.ContentFields.Contains("Value")) return false;

            Type t = AApiVersionedFields.GetContentFieldTypes(0, this.resource.GetType())["Value"];
            if (typeof(String).IsAssignableFrom(t)) return true;

            return false;
        }

        private void browserWidget1_DragOver(object sender, DragEventArgs e)
        {
            //if (package == null) return;
            if ((new List<string>(e.Data.GetFormats())).Contains("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        // For browserWidget1_DragDrop(), see Import/Import.cs

        private void browserWidget1_ItemActivate(object sender, EventArgs e)
        {
	        this.resourceDetails();
        }

        private void browserWidget1_DeletePressed(object sender, EventArgs e)
        {
	        this.resourceIsDeleted();
        }
        #endregion

        #region Resource Filter Widget
        private void resourceFilterWidget1_FilterChanged(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
	            this.browserWidget1.Filter = this.resourceFilterWidget1.FilterEnabled ? this.resourceFilterWidget1.Filter : null;
            }
            finally { this.Enabled = true; }
        }

        private void resourceFilterWidget1_PasteClicked(object sender, EventArgs e)
        {
	        this.filterPaste();
        }
        #endregion

        #region Control Panel Widget
        private void controlPanel1_SortChanged(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
	            this.browserWidget1.Sortable = this.controlPanel1.Sort;
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_HexOnlyChanged(object sender, EventArgs e)
        {
            Application.DoEvents();
            IResourceIndexEntry rie = this.browserWidget1.SelectedResource;
            if (rie != null)
            {
	            this.browserWidget1.SelectedResource = null;
	            this.browserWidget1.SelectedResource = rie;
            }
        }

        private void controlPanel1_AutoChanged(object sender, EventArgs e)
        {
            bool waiting = Application.UseWaitCursor;
            try
            {
                Application.UseWaitCursor = true;
                Application.DoEvents();
	            this.pnAuto.SuspendLayout();
	            this.pnAutoCleanUp();

                if (!this.controlPanel1.AutoOff)
                {

                    IBuiltInValueControl c = null;
                    if (this.resException != null)
                        c = this.getExceptionControl(this.resException);
                    else if (this.resource != null)
                    {

                        if (this.controlPanel1.AutoHex)
                        {
                            c = new HexControl(this.resource.Stream);
                        }
                        else if (this.controlPanel1.AutoPreview)
                        {
                            c = this.getPreviewControl();
                        }

                    }
                    if (c != null)
                    {
	                    this.menuBarWidget.SetPreviewControlItems(c.GetContextMenuItems(this.controlPanel1_CommitClick));
	                    this.pnAuto.Controls.Add(c.ValueControl);
                    }

                }

                foreach (Control c in this.pnAuto.Controls)
                {
                    c.ContextMenuStrip = this.menuBarWidget.previewContextMenuStrip;
                    c.Dock = DockStyle.Fill;
                    if (c is RichTextBox)
                    {
                        (c as RichTextBox).ZoomFactor = Properties.Settings.Default.PreviewZoomFactor;
                        (c as RichTextBox).ContentsResized += (s, cre) => { Properties.Settings.Default.PreviewZoomFactor = (s as RichTextBox).ZoomFactor; };
                    }
                }

            }
            finally
            {
	            this.pnAuto.ResumeLayout();
                Application.UseWaitCursor = waiting;
                Application.DoEvents();
            }
        }

        private void pnAutoCleanUp()
        {
            if (this.pnAuto.Controls.Count == 0) return;

            foreach (Control c in this.pnAuto.Controls)
                c.Dispose();
	        this.pnAuto.Controls.Clear();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void controlPanel1_HexClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

	            this.f_FloatControl((new HexControl(this.resource.Stream)).ValueControl);
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_PreviewClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                IBuiltInValueControl c = this.getPreviewControl();
                if (c == null) return;

	            this.f_FloatControl(c.ValueControl);
            }
            finally { this.Enabled = true; }
        }

        void f_FloatControl(Control c)
        {
            Form f = new Form();
            f.SuspendLayout();
            f.Controls.Add(c);
            c.Dock = DockStyle.Fill;
            f.Icon = this.Icon;

            f.Text = this.Text + ((this.resourceName != null && this.resourceName.Length > 0) ? " - " + this.resourceName : "");

            if (c.GetType().Equals(typeof(RichTextBox)))
            {
                f.ClientSize = new Size(this.ClientSize.Width - (this.ClientSize.Width / 5), this.ClientSize.Height - (this.ClientSize.Height / 5));
            }
            else if (c.GetType().Equals(typeof(PictureBox)))
            {
                f.ClientSize = c.Size;
                f.SizeGripStyle = SizeGripStyle.Hide;
            }

            f.StartPosition = FormStartPosition.CenterParent;
            f.ResumeLayout();
            f.FormClosed += this.f_FormClosed;
            f.Show(this);
        }

        void f_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!(sender as Form).IsDisposed) (sender as Form).Dispose();
        }

        IBuiltInValueControl getPreviewControl()
        {
            try
            {
                if (this.hasStringValueContentField())
                {
                    return new TextControl("" + this.resource["Value"]);
                }

                IBuiltInValueControl ibvc = ABuiltInValueControl.Lookup(this.browserWidget1.SelectedResource.ResourceType, this.resource.Stream);
                if (ibvc != null)
                {
                    return ibvc;
                }

                if (Properties.Settings.Default.EnableFallbackHexPreview)
                {
                    return new HexControl(this.resource.Stream);
                }

                if (Properties.Settings.Default.EnableFallbackTextPreview)
                {
                    return new TextControl(
                        "== == == == == == == == == == == == == == == == ==\n" +
                        " **  Fallback preview:  data may be incomplete  ** \n" +
                        "== == == == == == == == == == == == == == == == ==\n\n\n" +
                        (new StreamReader(this.resource.Stream)).ReadToEnd());
                }
            }
            catch (Exception ex)
            {
                return this.getExceptionControl(ex);
            }

            return null;
        }

        IBuiltInValueControl getExceptionControl(Exception ex)
        {
            IResourceIndexEntry rie = this.browserWidget1.SelectedResource;
            string s = "";
            if (rie != null) s += "Error reading resource " + rie;
            s += String.Format("\r\nFront-end Distribution: {0}\r\nLibrary Distribution: {1}\r\n",
                Version.CurrentVersion, Version.LibraryVersion);
            for (Exception inex = ex; inex != null; inex = inex.InnerException)
            {
                s += "\r\nSource: " + inex.Source;
                s += "\r\nAssembly: " + inex.TargetSite.DeclaringType.Assembly.FullName;
                s += "\r\n" + inex.Message;
                s += "\r\n----\r\nStack trace:\r\n" + inex.StackTrace + "\r\n----\r\n";
            }

            return new TextControl(s);
        }


        private void controlPanel1_GridClick(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                DialogResult dr = (new NewGridForm(this.resource as AApiVersionedFields, true)).ShowDialog();
                if (dr != DialogResult.OK)
	                this.resourceIsDirty = false;
                else
	                this.controlPanel1_CommitClick(null, null);
                IResourceIndexEntry rie = this.browserWidget1.SelectedResource;
	            this.browserWidget1.SelectedResource = null;
	            this.browserWidget1.SelectedResource = rie;
            }
            finally { this.Enabled = true; }
        }

        private void controlPanel1_UseNamesChanged(object sender, EventArgs e)
        {
            bool en = this.Enabled;
            try
            {
                this.Enabled = false;
	            this.browserWidget1.DisplayResourceNames = this.controlPanel1.UseNames;
            }
            finally { this.Enabled = en; }
        }

        private void controlPanel1_UseTagsChanged(object sender, EventArgs e)
        {
            bool en = this.Enabled;
            try
            {
                this.Enabled = false;
	            this.browserWidget1.DisplayResourceTags = this.controlPanel1.UseTags;
            }
            finally { this.Enabled = en; }
        }

        private void controlPanel1_CommitClick(object sender, EventArgs e)
        {
            if (this.resource == null) return;
            if (this.package == null) return;
	        this.package.ReplaceResource(this.browserWidget1.SelectedResource, this.resource);
	        this.resourceIsDirty = this.controlPanel1.CommitEnabled = false;
	        this.IsPackageDirty = true;
        }

        private void controlPanel1_HexEditClick(object sender, EventArgs e) {
	        this.HexEdit(this.browserWidget1.SelectedResource, this.resource); }
        #endregion

        #region Helpers
        void setHexEditor() {
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_hexEditor, this.hasHexEditor);
	        this.controlPanel1.HexEditEnabled = this.hasHexEditor; }

        void setTextEditor() {
	        this.menuBarWidget.Enable(MenuBarWidget.MB.MBR_textEditor, this.hasTextEditor); }

        void setHelpers()
        {
	        this.helpers = new HelperManager(this.browserWidget1.SelectedResource, this.resource, this.browserWidget1.ResourceName(this.browserWidget1.SelectedResource));
            if (Properties.Settings.Default.DisabledHelpers != null)
                foreach (string id in Properties.Settings.Default.DisabledHelpers)
                {
                    List<HelperManager.Helper> disabled = new List<HelperManager.Helper>();
                    foreach (var helper in this.helpers) if (helper.id == id) disabled.Add(helper);
                    foreach (var helper in disabled) this.helpers.Remove(helper);
                }
	        this.controlPanel1.Helper1Enabled = this.helpers.Count > 0;
	        this.controlPanel1.Helper1Label = this.helpers.Count > 0 && this.helpers[0].label.Length > 0 ? this.helpers[0].label : "Helper1";
	        this.controlPanel1.Helper1Tip = this.helpers.Count > 0 && this.helpers[0].desc.Length > 0 ? this.helpers[0].desc : "";

	        this.controlPanel1.Helper2Enabled = this.helpers.Count > 1;
	        this.controlPanel1.Helper2Label = this.helpers.Count > 1 && this.helpers[1].label.Length > 0 ? this.helpers[1].label : "Helper2";
	        this.controlPanel1.Helper1Tip = this.helpers.Count > 1 && this.helpers[1].desc.Length > 0 ? this.helpers[1].desc : "";

	        this.menuBarWidget.SetHelpers(this.helpers);
        }

        private void controlPanel1_Helper1Click(object sender, EventArgs e)
        {
	        this.do_HelperClick(0);
        }

        private void controlPanel1_Helper2Click(object sender, EventArgs e)
        {
	        this.do_HelperClick(1);
        }

        void do_HelperClick(int i)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                MemoryStream ms = this.helpers.execHelper(i);
	            this.MakeFormVisible();
                Application.DoEvents();

                if (!this.helpers[i].isReadOnly) this.afterEdit(ms);
            }
            finally { this.Enabled = true; }
        }

        void TextEdit(IResourceKey key, IResource res)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                MemoryStream ms = HelperManager.Edit(key, res,
                    Properties.Settings.Default.TextEditorCmd,
                    Properties.Settings.Default.TextEditorWantsQuotes,
                    Properties.Settings.Default.TextEditorIgnoreTS);

	            this.MakeFormVisible();
                Application.DoEvents();

	            this.afterEdit(ms);
            }
            finally { this.Enabled = true; }
        }

        void HexEdit(IResourceKey key, IResource res)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                MemoryStream ms = HelperManager.Edit(key, res,
                    Properties.Settings.Default.HexEditorCmd,
                    Properties.Settings.Default.HexEditorWantsQuotes,
                    Properties.Settings.Default.HexEditorIgnoreTS);

	            this.MakeFormVisible();
                Application.DoEvents();

	            this.afterEdit(ms);
            }
            finally { this.Enabled = true; }
        }

        void afterEdit(MemoryStream ms)
        {
            if (ms != null)
            {
                int dr = CopyableMessageBox.Show("Resource has been updated.  Commit changes?", "Commit changes?",
                    CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, 0);

                if (dr != 0) return;

                IResourceIndexEntry rie = this.NewResource(this.browserWidget1.SelectedResource, ms, DuplicateHandling.replace, this.browserWidget1.SelectedResource.Compressed != 0);
                if (rie != null) this.browserWidget1.Add(rie);
            }
        }

        void MakeFormVisible() { try { ForceFocus.Focus(this); } catch { } }
        #endregion



    }
}
