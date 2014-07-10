using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using s4pi.Interfaces;
using s4pi.Package;
using s4pi.Extensions;

namespace S4PIDemoFE
{
    partial class MainForm
    {
        const string myDataFormatSingleFile = "x-application/s3pe.singleFile";
        const string myDataFormatBatch = "x-application/s3pe.batch";

        [Serializable]
        public struct myDataFormat
        {
            public TGIN tgin;
            public byte[] data;
        }

        private void resourceImport()
        {
            bool useNames = controlPanel1.UseNames;
            try
            {
                this.Enabled = false;
                browserWidget1.Visible = false;
                controlPanel1.UseNames = false;
                DialogResult dr = importResourcesDialog.ShowDialog();
                if (dr != DialogResult.OK) return;

                if (importResourcesDialog.FileNames.Length > 1)
                    importBatch(importResourcesDialog.FileNames, importResourcesDialog.Title);
                else
                    importSingle(importResourcesDialog.FileName, importResourcesDialog.Title);
            }
            finally { controlPanel1.UseNames = useNames; browserWidget1.Visible = true; this.Enabled = true; }
        }

        private void resourceImportPackages()
        {
            try
            {
                this.Enabled = false;
                DialogResult dr = importPackagesDialog.ShowDialog();
                if (dr != DialogResult.OK) return;

                ImportBatch ib = new ImportBatch(importPackagesDialog.FileNames, ImportBatch.Mode.package);
                dr = ib.ShowDialog();
                if (dr != DialogResult.OK) return;

                importPackagesCommon(ib.Batch, importPackagesDialog.Title, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, ib.Compress, ib.UseNames,
                    rename: ib.Rename);
            }
            finally { this.Enabled = true; }
        }

        private void resourceReplaceFrom()
        {
            var savedTitle = importPackagesDialog.Title;
            try
            {
                this.Enabled = false;
                importPackagesDialog.Title = "Replace Selected Resources from Package(s)";
                DialogResult dr = importPackagesDialog.ShowDialog();
                if (dr != DialogResult.OK) return;

                ImportBatch ib = new ImportBatch(importPackagesDialog.FileNames, ImportBatch.Mode.replaceFrom);
                dr = ib.ShowDialog();
                if (dr != DialogResult.OK) return;

                importPackagesCommon(ib.Batch, importPackagesDialog.Title, DuplicateHandling.replace, ib.Compress, selection: browserWidget1.SelectedResources);
            }
            finally { this.Enabled = true; importPackagesDialog.Title = savedTitle; }
        }


        static List<uint> xmlList = new List<uint>(new uint[] {
            0x025C95B6, //xml: UI Layout definitions
            0x025ED6F4, //xml: Sim Outfit
            //-Anach: do not allow duplicates 0x0333406C, //xml: XML Resource (Uses include tuning constants)
            0x03B33DDF, //xml: Interaction Tuning (These are like the stuff you had in TTAB. Motive advertising and autonomy etc)
            0x044AE110, //xml: Complate Preset
            0x0604ABDA, //xml:
            //-Anach: delete these 0x73E93EEB, //xml: Package manifest
        });
        static List<uint> stblList = new List<uint>(new uint[] {
            0x220557DA, //STBL
        });
        static List<uint> nmapList = new List<uint>(new uint[] {
            0x0166038C, //NMAP
        });
        // See http://dino.drealm.info/den/denforum/index.php?topic=244.0
        static List<uint> deleteList = new List<uint>(new uint[] {
            0x73E93EEB, //xml: sims3pack manifest
            //http://dino.drealm.info/den/denforum/index.php?topic=724.0
            //-Anach: no, this is needed by CAS 0x626F60CD, //THUM: sims3pack
            //http://dino.drealm.info/den/denforum/index.php?topic=253.msg1234#msg1234
            //-Anach: no, this is needed by roofing 0x2E75C765, //ICON: sims3pack
        });
        static List<uint> allowList = new List<uint>();
        private void resourceImportAsDBC()
        {
            if (allowList.Count == 0)
            {
                allowList.AddRange(xmlList);
                allowList.AddRange(stblList);
                allowList.AddRange(nmapList);
            }
            try
            {
                this.Enabled = false;
                DialogResult dr = importPackagesDialog.ShowDialog();
                if (dr != DialogResult.OK) return;

                AutoSaveState autoSaveState = AutoSaveState.Always;
                if (S4PIDemoFE.Properties.Settings.Default.AskDBCAutoSave)
                    autoSaveState = AutoSaveState.Ask;
                importPackagesCommon(importPackagesDialog.FileNames, importPackagesDialog.Title, DuplicateHandling.allow, true,
                    useNames: true,
                    dupsList: allowList,
                    autoSaveState: autoSaveState);

                browserWidget1.Visible = false;
                lbProgress.Text = "Doing DBC clean up...";

                Application.DoEvents();
                DateTime now = DateTime.UtcNow;
                IList<IResourceIndexEntry> lrie = dupsOnly(CurrentPackage.FindAll(x => stblList.Contains(x.ResourceType)));
                foreach (IResourceIndexEntry dup in lrie)
                {
                    IList<IResourceIndexEntry> ldups = CurrentPackage.FindAll(rie => ((IResourceKey)dup).Equals(rie));
                    IResourceIndexEntry newRie = NewResource(dup, null, DuplicateHandling.allow, true);
                    IDictionary<ulong, string> newStbl = (IDictionary<ulong, string>)s4pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, newRie);
                    foreach (IResourceIndexEntry rie in ldups)
                    {
                        IDictionary<ulong, string> oldStbl = (IDictionary<ulong, string>)s4pi.WrapperDealer.WrapperDealer.GetResource(0, CurrentPackage, rie);
                        foreach (var kvp in oldStbl) if (!newStbl.ContainsKey(kvp.Key)) newStbl.Add(kvp);
                        rie.IsDeleted = true;
                        if (now.AddMilliseconds(100) < DateTime.UtcNow) { Application.DoEvents(); now = DateTime.UtcNow; }
                    }
                    CurrentPackage.ReplaceResource(newRie, (IResource)newStbl);
                    browserWidget1.Add(newRie, false);
                }

                // Get rid of Sims3Pack resource that sneak in
                CurrentPackage.FindAll(x =>
                {
                    if (now.AddMilliseconds(100) < DateTime.UtcNow) { Application.DoEvents(); now = DateTime.UtcNow; }
                    if (deleteList.Contains(x.ResourceType)) { x.IsDeleted = true; return false; }
                    return false;
                });

                // If there are any remaining duplicate XMLs, give up - they're too messy to fix automatically
                if (dupsOnly(CurrentPackage.FindAll(x => xmlList.Contains(x.ResourceType))).Count > 0)
                    CopyableMessageBox.Show("Manual merge of XML files required.");
            }
            finally { browserWidget1.Visible = true; lbProgress.Text = ""; Application.DoEvents(); this.Enabled = true; }
        }
        IList<IResourceIndexEntry> dupsOnly(IList<IResourceIndexEntry> list)
        {
            List<IResourceKey> seen = new List<IResourceKey>();
            List<IResourceIndexEntry> res = new List<IResourceIndexEntry>();
            foreach (IResourceIndexEntry rie in list)
            {
                if (!seen.Contains(rie)) seen.Add(rie);
                else if (!res.Contains(rie)) res.Add(rie);
            }
            return res;
        }


        internal enum AutoSaveState
        {
            Never,
            Ask,
            Always,
        }
        private void importPackagesCommon(string[] packageList, string title, DuplicateHandling dups, bool compress,
            bool useNames = false,
            bool rename = false,
            List<uint> dupsList = null,
            AutoSaveState autoSaveState = AutoSaveState.Ask,
            IList<IResourceIndexEntry> selection = null
            )
        {
            bool CPuseNames = controlPanel1.UseNames;
            DateTime now = DateTime.UtcNow;

            bool autoSave = false;
            if (autoSaveState == AutoSaveState.Ask)
            {
                switch (CopyableMessageBox.Show("Auto-save current package after each package imported?", title,
                     CopyableMessageBoxButtons.YesNoCancel, CopyableMessageBoxIcon.Question))
                {
                    case 0: autoSave = true; break;
                    case 2: return;
                }
            }
            else
                autoSave = autoSaveState == AutoSaveState.Always;

            try
            {
                browserWidget1.Visible = false;
                controlPanel1.UseNames = false;

                bool skipAll = false;
                foreach (string filename in packageList)
                {
                    if (Filename != null && Filename.Length > 0 && Path.GetFullPath(Filename).Equals(Path.GetFullPath(filename)))
                    {
                        CopyableMessageBox.Show("Skipping current package.", importPackagesDialog.Title);
                        continue;
                    }

                    lbProgress.Text = "Importing " + Path.GetFileNameWithoutExtension(filename) + "...";
                    Application.DoEvents();
                    IPackage imppkg = null;
                    try
                    {
                        imppkg = Package.OpenPackage(0, filename);
                    }
                    catch (InvalidDataException ex)
                    {
                        if (skipAll) continue;
                        int btn = CopyableMessageBox.Show(String.Format("Could not open package {0}.\n{1}", Path.GetFileName(filename), ex.Message),
                            title, CopyableMessageBoxIcon.Error, new List<string>(new string[] {
                            "Skip this", "Skip all", "Abort"}), 0, 0);
                        if (btn == 0) continue;
                        if (btn == 1) { skipAll = true; continue; }
                        break;
                    }
                    try
                    {
                        List<Tuple<myDataFormat, DuplicateHandling>> limp = new List<Tuple<myDataFormat, DuplicateHandling>>();
                        List<IResourceIndexEntry> lrie = selection == null
                            ? imppkg.GetResourceList
                            : imppkg.FindAll(rie => selection.Any(tgt => ((AResourceKey)tgt).Equals(rie)));
                        progressBar1.Value = 0;
                        progressBar1.Maximum = lrie.Count;
                        foreach (IResourceIndexEntry rie in lrie)
                        {
                            try
                            {
                                if (rie.ResourceType == 0x0166038C)//NMAP
                                {
                                    if (useNames)
                                        browserWidget1.MergeNamemap(s4pi.WrapperDealer.WrapperDealer.GetResource(0, imppkg, rie) as IDictionary<ulong, string>, true, rename);
                                }
                                else
                                {
                                    IResource res = s4pi.WrapperDealer.WrapperDealer.GetResource(0, imppkg, rie, true);

                                    myDataFormat impres = new myDataFormat()
                                    {
                                        tgin = rie as AResourceIndexEntry,
                                        data = res.AsBytes,
                                    };
									
									// dups Replace | Reject | Allow
									// dupsList null | list of allowable dup types
									DuplicateHandling dupThis =
										dups == DuplicateHandling.allow
											? dupsList == null || dupsList.Contains(rie.ResourceType) ? DuplicateHandling.allow : DuplicateHandling.replace
											: dups;

                                    limp.Add(Tuple.Create(impres, dupThis));
                                    progressBar1.Value++;
                                    if (now.AddMilliseconds(100) < DateTime.UtcNow) { Application.DoEvents(); now = DateTime.UtcNow; }
                                }
                            }
                            catch (Exception ex)
                            {
                                string rk = "";
                                if (rie != null) rk = "(RK: " + rie + ")\n";
                                else rk = "(RK is null)\n";

                                CopyableMessageBox.IssueException(ex, "Could not import all resources - aborting.\n" + rk, title);
                                throw new IgnoredException(ex);
                            }
                        }
                        progressBar1.Value = 0;

                        IEnumerable<IResourceIndexEntry> rieList = limp
                            .Select(x => NewResource((AResourceKey)x.Item1.tgin, new MemoryStream(x.Item1.data), x.Item2, compress))
                            .Where(x => x != null);
                        browserWidget1.AddRange(rieList);
                    }
                    catch (IgnoredException) { break; }//just the thrown exception, stop looping
                    catch (Exception ex)
                    {
                        CopyableMessageBox.IssueException(ex, "Could not import all resources - aborting.\n", title);
                        break;
                    }
                    finally { Package.ClosePackage(0, imppkg); }
                    if (autoSave) if (!fileSave()) break;
                }
            }
            finally
            {
                lbProgress.Text = "";
                progressBar1.Value = 0;
                progressBar1.Maximum = 0;
                controlPanel1.UseNames = CPuseNames;
                browserWidget1.Visible = true;
                ForceFocus.Focus(Application.OpenForms[0]);
                Application.DoEvents();
            }
        }

        class IgnoredException : Exception
        {
            public IgnoredException(Exception innerException) : base("Ignore this", innerException) { }
        }

        private void resourcePaste()
        {
            try
            {
                this.Enabled = false;
                if (Clipboard.ContainsData(myDataFormatSingleFile))
                {
                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = Clipboard.GetData(myDataFormatSingleFile) as MemoryStream;
                    myDataFormat d = (myDataFormat)formatter.Deserialize(ms);
                    ms.Close();

                    importSingle(d);
                }
                else if (Clipboard.ContainsData(myDataFormatBatch))
                {
                    IFormatter formatter = new BinaryFormatter();
                    MemoryStream ms = Clipboard.GetData(myDataFormatBatch) as MemoryStream;
                    List<myDataFormat> l = (List<myDataFormat>)formatter.Deserialize(ms);
                    ms.Close();

                    importBatch(l);
                }
                else if (Clipboard.ContainsFileDropList())
                {
                    System.Collections.Specialized.StringCollection fileDrop = Clipboard.GetFileDropList();
                    if (fileDrop == null || fileDrop.Count == 0) return;

                    if (fileDrop.Count == 1)
                    {
                        importSingle(fileDrop[0], "Resource->Paste");
                    }
                    else
                    {
                        string[] batch = new string[fileDrop.Count];
                        for (int i = 0; i < fileDrop.Count; i++) batch[i] = fileDrop[i];
                        importBatch(batch, "Resource->Paste");
                    }
                }
            }
            finally { this.Enabled = true; }
        }

        private void browserWidget1_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileDrop = e.Data.GetData("FileDrop") as String[];
            if (fileDrop == null || fileDrop.Length == 0) return;

            Application.DoEvents();
            try
            {
                this.Enabled = false;
                if (fileDrop.Length > 1)
                    importBatch(fileDrop, "File(s)->Drop");
                else if (Directory.Exists(fileDrop[0]))
                    importBatch(fileDrop, "File(s)->Drop");
                else
                    importSingle(fileDrop[0], "File(s)->Drop");
            }
            finally { this.Enabled = true; }
        }

        static string[] asPkgExts = new string[] { ".package", ".world", ".dbc", ".nhd", };
        void importSingle(string filename, string title)
        {
            if (CurrentPackage == null)
                fileNew();

            if (new List<string>(asPkgExts).Contains(filename.Substring(filename.LastIndexOf('.'))))
            {
                ImportBatch ib = new ImportBatch(new string[] { filename, }, ImportBatch.Mode.package);
                DialogResult dr = ib.ShowDialog();
                if (dr != DialogResult.OK) return;

                importPackagesCommon(new string[] { filename, }, title, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, ib.Compress, ib.UseNames);
            }
            else
            {
                ResourceDetails ir = new ResourceDetails(/*20120820 CurrentPackage.Find(x => x.ResourceType == 0x0166038C) != null/**/true, true);
                ir.Filename = filename;
                DialogResult dr = ir.ShowDialog();
                if (dr != DialogResult.OK) return;

                importFile(ir.Filename, ir, ir.UseName, ir.AllowRename, ir.Compress, ir.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, true);
            }
        }

        void importSingle(myDataFormat data)
        {
            ResourceDetails ir = new ResourceDetails(/*20120820 CurrentPackage.Find(x => x.ResourceType == 0x0166038C) != null/**/true, true);
            ir.Filename = data.tgin;
            DialogResult dr = ir.ShowDialog();
            if (dr != DialogResult.OK) return;

            data.tgin = ir;
            importStream(data, ir.UseName, ir.AllowRename, ir.Compress, ir.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, true);
        }

        string[] getFiles(string folder)
        {
            if (!Directory.Exists(folder)) return null;
            List<string> files = new List<string>();
            foreach (string dir in Directory.GetDirectories(folder))
                files.AddRange(getFiles(dir));
            files.AddRange(Directory.GetFiles(folder));
            return files.ToArray();
        }
        void importBatch(string[] batch, string title)
        {
            if (CurrentPackage == null)
                fileNew();

            List<string> foo = new List<string>();
            foreach (string bar in batch)
                if (Directory.Exists(bar)) foo.AddRange(getFiles(bar));
                else if (File.Exists(bar)) foo.Add(bar);
            batch = foo.ToArray();

            ImportBatch ib = new ImportBatch(batch);
            ib.Text = title;
            DialogResult dr = ib.ShowDialog();
            if (dr != DialogResult.OK) return;

            List<string> resList = new List<string>();
            List<string> pkgList = new List<string>();
            List<string> folders = new List<string>();

            List<string> pkgExts = new List<string>(asPkgExts);
            foreach (string s in batch)
                (pkgExts.Contains(s.Substring(s.LastIndexOf('.'))) ? pkgList : resList).Add(s);

            try
            {
                if (pkgList.Count > 0)
                    importPackagesCommon(pkgList.ToArray(), title, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, ib.Compress, ib.UseNames);

                bool nmOK = true;
                foreach (string filename in resList)
                {
                    nmOK = importFile(filename, filename, nmOK && ib.UseNames, ib.Rename, ib.Compress, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, false);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not import all resources - aborting.\n", title);
            }
        }

        void importBatch(IList<myDataFormat> ldata)
        {
            ImportBatch ib = new ImportBatch(ldata);
            DialogResult dr = ib.ShowDialog();
            if (dr != DialogResult.OK) return;

            List<myDataFormat> output = new List<myDataFormat>();
            foreach (string b in ib.Batch)
            {
                foreach (myDataFormat data in ldata)
                    if (data.tgin == b) { output.Add(data); goto next; }
            next: { }
            }

            if (output.Count == 0) return;
            if (output.Count == 1) { importSingle(output[0]); return; }

            try
            {
                foreach (myDataFormat data in output)
                {
                    importStream(data, ib.UseNames, ib.Rename, ib.Compress, ib.Replace ? DuplicateHandling.replace : DuplicateHandling.reject, false);
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex, "Could not import all resources.\n", "Aborting import");
            }
        }

        bool importFile(string filename, TGIN tgin, bool useName, bool rename, bool compress, DuplicateHandling dups, bool select)
        {
            IResourceKey rk = (TGIBlock)tgin;
            string resName = tgin.ResName;
            bool nmOK = true;
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            BinaryReader r = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            w.Write(r.ReadBytes((int)r.BaseStream.Length));
            r.Close();
            w.Flush();

            if (useName && resName != null && resName.Length > 0)
                nmOK = browserWidget1.ResourceName(rk.Instance, resName, true, rename);

            IResourceIndexEntry rie = NewResource(rk, ms, dups, compress);
            if (rie != null) browserWidget1.Add(rie, select);
            return nmOK;
        }

        void importStream(myDataFormat data, bool useName, bool rename, bool compress, DuplicateHandling dups, bool select)
        {
            if (useName && data.tgin.ResName != null && data.tgin.ResName.Length > 0)
                browserWidget1.ResourceName(data.tgin.ResInstance, data.tgin.ResName, true, rename);

            IResourceIndexEntry rie = NewResource((TGIBlock)data.tgin, new MemoryStream(data.data), dups, compress);
            if (rie != null) browserWidget1.Add(rie, select);
        }
    }
}
