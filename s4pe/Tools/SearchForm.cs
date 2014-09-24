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
using System.ComponentModel;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using s4pi.Interfaces;

namespace S4PIDemoFE.Tools
{
    public partial class SearchForm : Form
    {
        public SearchForm()
        {
            InitializeComponent();
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
            lbxHits.Items.Clear();
            lbxHits.Font = new Font(FontFamily.GenericMonospace, lbxHits.Font.Size);
            comboBox1.SelectedIndex = 0;
        }

        #region Search form signal to main app
        [Browsable(true)]
        [Category("Action")]
        [Description("Raised when the user clicks \"Go to\"")]
        public event EventHandler<GoEventArgs> Go;

        public class GoEventArgs : EventArgs
        {
            IResourceIndexEntry rie;
            public GoEventArgs(IResourceIndexEntry rie) { this.rie = rie; }
            public IResourceIndexEntry ResourceIndexEntry { get { return rie; } }
        }

        protected virtual void OnGo(object sender, GoEventArgs e) { if (Go != null) Go(sender, e); }
        #endregion

        IPackage pkg;
        public IPackage CurrentPackage { get { return pkg; } set { pkg = value; } }

        string fromRIE(IResourceIndexEntry rie) { return (rie as AResourceKey) + ""; }

        public static IResourceKey Parse(string value)
        {
            IResourceKey result;
            if (!TryParse(value, out result)) throw new ArgumentException();
            return result;
        }
        public static bool TryParse(string value, out IResourceKey result)
        {
            result = (AResourceKey)new s4pi.Extensions.TGIN();

            string[] tgi = value.Trim().ToLower().Split('-');
            if (tgi.Length != 3) return false;
            foreach (var x in tgi) if (!x.StartsWith("0x")) return false;

            uint tg;
            if (!uint.TryParse(tgi[0].Substring(2), System.Globalization.NumberStyles.HexNumber, null, out tg)) return false;
            result.ResourceType = tg;
            if (!uint.TryParse(tgi[1].Substring(2), System.Globalization.NumberStyles.HexNumber, null, out tg)) return false;
            result.ResourceGroup = tg;

            ulong i;
            if (!ulong.TryParse(tgi[2].Substring(2), System.Globalization.NumberStyles.HexNumber, null, out i)) return false;
            result.Instance = i;

            return true;
        }

        #region Search thread
        List<IResourceIndexEntry> lrie;
        Thread searchThread;
        bool searching;
        static uint resourceType;
        void StartSearch()
        {
            lrie = new List<IResourceIndexEntry>();
            lbxHits.Items.Clear();
            this.SearchComplete += new EventHandler<BoolEventArgs>(SearchForm_SearchComplete);

            byte[] target;
            if (tbHex.Text.StartsWith("U"))
            {
                //unicode
                string hex = tbHex.Text.Substring(1).Trim('"');
                target = new byte[hex.Length * 2];
                if (comboBox1.SelectedIndex == 0)
                    for (int i = 0; i < target.Length; i += 2) { char c = hex[i / 2]; target[i] = (byte)(c & 0xff); target[i + 1] = (byte)(c >> 8); }
                else
                    for (int i = 0; i < target.Length; i += 2) { char c = hex[i / 2]; target[i + 1] = (byte)(c & 0xff); target[i] = (byte)(c >> 8); }
            }
            else if (tbHex.Text.StartsWith("\""))
            {
                string hex = tbHex.Text.Trim('"');
                target = new byte[hex.Length];
                for (int i = 0; i < target.Length; i++) target[i] = (byte)hex[i];
            }
            else if (tbHex.Text.Contains(" "))
            {
                string[] input = tbHex.Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                target = new byte[input.Length];
                if (comboBox1.SelectedIndex == 0)
                    for (int i = 0; i < input.Length; i++) target[i] = Convert.ToByte(input[i], 16);
                else
                    for (int i = 0; i < input.Length; i++) target[i] = Convert.ToByte(input[input.Length - 1 - i], 16);
            }
            else
            {
                if (tbHex.Text.StartsWith("0x") || comboBox1.SelectedIndex == 1)
                {
                    string hex = tbHex.Text.Substring(tbHex.Text.StartsWith("0x") ? 2 : 0);
                    if (hex.Length % 2 != 0) hex = "0" + hex;
                    target = new byte[(hex.Length + 1) / 2];
                    for (int i = 0; i < target.Length; i++)
                        target[i] = Convert.ToByte(hex.Substring(hex.Length - 2 - (2 * i), 2), 16);
                }
                else
                {
                    string hex = tbHex.Text;
                    if (hex.Length % 2 != 0) hex = "0" + hex;
                    target = new byte[(hex.Length + 1) / 2];
                    for (int i = 0; i < target.Length; i++)
                        target[i] = Convert.ToByte(hex.Substring(2 * i, 2), 16);
                }
            }

            resourceType = cbType.Value;
            SearchThread st = new SearchThread(this, pkg,
                x => ckbFilter.Checked ? x.ResourceType == resourceType : true,
                target, Add, updateProgress, stopSearch, OnSearchComplete);

            searchThread = new Thread(new ThreadStart(st.Search));
            searching = true;
            searchThread.Start();
        }

        void SearchForm_SearchComplete(object sender, SearchForm.BoolEventArgs e)
        {
            searching = false;
            while (searchThread != null && searchThread.IsAlive)
                searchThread.Join(100);
            searchThread = null;

            this.toolStripProgressBar1.Visible = false;
            this.toolStripStatusLabel1.Visible = false;

            tbHex.Enabled = true;
            btnSearch.Text = "&Search";
        }

        void AbortSearch()
        {
            if (!searching) SearchForm_SearchComplete(null, new BoolEventArgs(false));
            else searching = false;
        }

        delegate void AddCallBack(IResourceIndexEntry rie);
        void Add(IResourceIndexEntry rie)
        {
            lbxHits.Items.Add(fromRIE(rie));
            lrie.Add(rie);
        }

        class BoolEventArgs : EventArgs
        {
            public bool arg;
            public BoolEventArgs(bool arg) { this.arg = arg; }
        }
        event EventHandler<BoolEventArgs> SearchComplete;
        delegate void searchCompleteCallback(bool complete);
        void OnSearchComplete(bool complete) { if (SearchComplete != null) { SearchComplete(this, new BoolEventArgs(complete)); } }

        delegate void updateProgressCallback(bool changeText, string text, bool changeMax, int max, bool changeValue, int value);
        void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
        {
            if (changeText)
            {
                toolStripStatusLabel1.Visible = text.Length > 0;
                toolStripStatusLabel1.Text = text;
            }
            if (changeMax)
            {
                if (max == -1)
                    toolStripProgressBar1.Visible = false;
                else
                {
                    toolStripProgressBar1.Visible = true;
                    toolStripProgressBar1.Maximum = max;
                }
            }
            if (changeValue)
                toolStripProgressBar1.Value = value;
        }

        public delegate bool stopSearchCallback();
        private bool stopSearch() { return !searching; }

        class SearchThread
        {
            SearchForm form;
            IPackage pkg;
            Predicate<IResourceIndexEntry> match;
            byte[] pattern;
            AddCallBack addCB;
            updateProgressCallback updateProgressCB;
            stopSearchCallback stopSearchCB;
            searchCompleteCallback searchCompleteCB;

            public SearchThread(SearchForm form, IPackage pkg, Predicate<IResourceIndexEntry> match, byte[] pattern,
                AddCallBack addCB, updateProgressCallback updateProgressCB, stopSearchCallback stopSearchCB, searchCompleteCallback searchCompleteCB)
            {
                this.form = form;
                this.pkg = pkg;
                this.match = match;
                this.pattern = (byte[])pattern.Clone();
                this.addCB = addCB;
                this.updateProgressCB = updateProgressCB;
                this.stopSearchCB = stopSearchCB;
                this.searchCompleteCB = searchCompleteCB;
            }

            public void Search()
            {
                bool complete = false;
                try
                {
                    updateProgress(true, "Retrieving resource index...", true, 0, true, 0);
                    IList<IResourceIndexEntry> lrie = pkg.FindAll(match);
                    if (stopSearch) return;

                    updateProgress(true, "Starting search... 0%", true, lrie.Count, true, 0);
                    int freq = Math.Max(1, lrie.Count / 100);

                    int hits = 0;
                    for (int i = 0; i < lrie.Count; i++)
                    {
                        if (stopSearch) return;

                        if (search(lrie[i]))
                        {
                            Add(lrie[i]);
                            hits++;
                        }

                        if (i % freq == 0)
                            updateProgress(true, String.Format("[Hits {0}] Searching... {1}%", hits, i * 100 / lrie.Count), false, -1, true, i);
                    }
                    complete = true;
                }
                catch (ThreadInterruptedException) { }
                finally
                {
                    updateProgress(true, "Search ended", true, 0, true, 0);
                    searchComplete(complete);
                }
            }

            bool search(IResourceIndexEntry rie)
            {
                IResource res = s4pi.WrapperDealer.WrapperDealer.GetResource(0, pkg, rie, true);// we're searching bytes
                byte[] resBytes = res.AsBytes;
                for (int i = 0; i < resBytes.Length - pattern.Length; i++)
                {
                    for (int j = 0; j < pattern.Length; j++)
                        if (resBytes[i + j] != pattern[j]) goto fail;
                    return true;
                fail: { }
                }
                return false;
            }

            void Add(IResourceIndexEntry rie) { Thread.Sleep(0); if (form.IsHandleCreated) form.Invoke(addCB, new object[] { rie, }); }

            void updateProgress(bool changeText, string text, bool changeMax, int max, bool changeValue, int value)
            {
                Thread.Sleep(0);
                if (form.IsHandleCreated) form.Invoke(updateProgressCB, new object[] { changeText, text, changeMax, max, changeValue, value, });
            }

            bool stopSearch { get { Thread.Sleep(0); return !form.IsHandleCreated || (bool)form.Invoke(stopSearchCB); } }

            void searchComplete(bool complete) { Thread.Sleep(0); if (form.IsHandleCreated) form.BeginInvoke(searchCompleteCB, new object[] { complete, }); }
        }
        #endregion

        #region Form control event handlers
        Regex rxX2 = new Regex(@"^[0-9A-F][0-9A-F]?(\s+([0-9A-F][0-9A-F]?)?)*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex rxXn = new Regex(@"^(0x)?[0-9A-F]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        Regex rxStr = new Regex(@"^U?"".*""$", RegexOptions.Compiled);
        private void tbHex_TextChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = (pkg != null) && (rxX2.IsMatch(tbHex.Text) || rxXn.IsMatch(tbHex.Text) || rxStr.IsMatch(tbHex.Text));
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (searching)
                AbortSearch();
            else
            {
                tbHex.Enabled = false;
                btnSearch.Text = "&Stop";
                StartSearch();
            }
        }

        private void lbxHits_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnGo.Enabled = lbxHits.SelectedItems.Count == 1;
            btnCopy.Enabled = lbxHits.SelectedItems.Count > 0;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (int i in lbxHits.SelectedIndices) s.AppendLine(fromRIE(lrie[i]));
            Clipboard.SetText(s.ToString());
        }

        private void ckbFilter_CheckedChanged(object sender, EventArgs e) { cbType.Enabled = ckbFilter.Checked; }

        private void searchContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            copyResourceKeyToolStripMenuItem.Enabled = lbxHits.SelectedItems.Count > 0;

            IResourceKey rk;
            pasteResourceKeyToolStripMenuItem.Enabled = Clipboard.ContainsText() && TryParse(Clipboard.GetText(), out rk);
        }

        private void copyResourceKeyToolStripMenuItem_Click(object sender, EventArgs e) { btnCopy_Click(sender, e); }

        private void pasteResourceKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IResourceKey rk;
            if (TryParse(Clipboard.GetText(), out rk))
            {
                cbType.Value = rk.ResourceType;
                //tbResourceGroup.Text = "0x" + rk.ResourceGroup.ToString("X8");
                //tbInstance.Text = "0x" + rk.Instance.ToString("X16");
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            OnGo(this, new GoEventArgs(lrie[lbxHits.SelectedIndex]));
        }

        private void lbxHits_DoubleClick(object sender, EventArgs e)
        {
            OnGo(this, new GoEventArgs(lrie[lbxHits.SelectedIndex]));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion
    }
}
