using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using s4pi.Interfaces;

namespace System.Windows.Forms.TGIBlockListEditorForm
{
    /// <summary>
    /// A form for editing an AResource.DependentList&lt;AResource.TGIBlock&gt; object.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// A form for editing an AResource.DependentList&lt;AResource.TGIBlock&gt; object.
        /// The DialogResult will be set on return from ShowDialog indicating whether the user
        /// chose to Save (OK) or Abandon (Cancel).
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            tbGroup.Text = tbInstance.Text = "";
            listView1.Items.Clear();
            btnAdd.Enabled = items != null && (items.MaxSize == -1 || listView1.Items.Count < items.MaxSize);
            btnDelete.Enabled = listView1.SelectedItems.Count > 0;
            cbType.Enabled = tbGroup.Enabled = tbInstance.Enabled = false;
        }

        TGIBlockList items;
        /// <summary>
        /// The list of TGIBlocks to edit
        /// </summary>
        public IList<TGIBlock> Items
        {
            get { return items; }
            set
            {
                if (items == value) return;
                items = new TGIBlockList(null);
                foreach (IResourceKey rk in value) items.Add(new TGIBlock(0, null, rk));

                listView1.Items.Clear();

                foreach (TGIBlock tgib in items)
                {
                    ListViewItem lvi = CreateListViewItem(tgib);
                    lvi.Tag = tgib;
                    listView1.Items.Add(lvi);
                }
                if (items.Count > 0)
                    listView1.Items[0].Selected = true;
                btnAdd.Enabled = items.MaxSize == -1 || listView1.Items.Count < items.MaxSize;
                btnDelete.Enabled = listView1.SelectedItems.Count > 0;
            }
        }
        private ListViewItem CreateListViewItem(TGIBlock tgib)
        {
            ListViewItem lvi = new ListViewItem();
            lvi.Text = s4pi.Extensions.ExtList.Ext.ContainsKey("0x" + tgib.ResourceType.ToString("X8"))
                ? s4pi.Extensions.ExtList.Ext["0x" + tgib.ResourceType.ToString("X8")][0] : "";
            lvi.SubItems.AddRange(new string[] {
                "0x" + tgib.ResourceType.ToString("X8"),
                "0x" + tgib.ResourceGroup.ToString("X8"),
                "0x" + tgib.Instance.ToString("X16"),
            });
            return lvi;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            items.Add(new TGIBlock(0, null));
            ListViewItem lvi = CreateListViewItem(items[items.Count - 1]);
            lvi.Tag = items[items.Count - 1];
            listView1.Items.Add(lvi);
            lvi.Selected = true;
            btnAdd.Enabled = items.MaxSize == -1 || listView1.Items.Count < items.MaxSize;
            cbType.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int i = listView1.SelectedIndices[0];
            listView1.SelectedIndices.Clear();
            items.RemoveAt(i);
            listView1.Items.RemoveAt(i);
            i--;
            if (i < 0 && items.Count > 0) i = 0;
            if (i >= 0)
                listView1.Items[i].Selected = true;
            btnAdd.Enabled = items.MaxSize == -1 || listView1.Items.Count < items.MaxSize;
            btnDelete.Enabled = listView1.SelectedItems.Count > 0;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0)
            {
                cbType.Value = 0;
                tbGroup.Text = tbInstance.Text = "";
                btnPaste.Enabled = btnCopy.Enabled = false;
            }
            else
            {
                TGIBlock item = listView1.SelectedItems[0].Tag as TGIBlock;
                cbType.Value = item.ResourceType;
                tbGroup.Text = "0x" + item.ResourceGroup.ToString("X8");
                tbInstance.Text = "0x" + item.Instance.ToString("X16");
                btnPaste.Enabled = btnCopy.Enabled = true;
            }
            cbType.Enabled = tbGroup.Enabled = tbInstance.Enabled = btnDelete.Enabled = listView1.SelectedIndices.Count > 0;
        }

        private void cbType_ValueChanged(object sender, EventArgs e)
        {
            if (cbType.Valid) cbType_Validated(sender, e);
        }

        private void cbType_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !cbType.Valid;
        }

        private void cbType_Validated(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count <= 0) return;

            items[listView1.SelectedIndices[0]].ResourceType = cbType.Value;
            ListViewItem lvi = CreateListViewItem(items[listView1.SelectedIndices[0]]);
            listView1.SelectedItems[0].Text = lvi.Text;
            listView1.SelectedItems[0].SubItems[1].Text = lvi.SubItems[1].Text;
        }

        private void tbGroup_Validating(object sender, CancelEventArgs e)
        {
            uint res;
            string s = tbGroup.Text.Trim().ToLower();
            if (s.StartsWith("0x"))
                e.Cancel = !uint.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out res);
            else
                e.Cancel = !uint.TryParse(s, out res);
            if (e.Cancel) tbGroup.SelectAll();
        }

        private void tbGroup_Validated(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count <= 0) return;

            uint res;
            string s = tbGroup.Text.Trim().ToLower();
            if (s.StartsWith("0x"))
                res = uint.Parse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null);
            else
                res = uint.Parse(s);

            items[listView1.SelectedIndices[0]].ResourceGroup = res;
            ListViewItem lvi = CreateListViewItem(items[listView1.SelectedIndices[0]]);
            listView1.SelectedItems[0].SubItems[2].Text = lvi.SubItems[2].Text;
        }

        private void tbInstance_Validating(object sender, CancelEventArgs e)
        {
            ulong res;
            string s = tbInstance.Text.Trim().ToLower();
            if (s.StartsWith("0x"))
                e.Cancel = !ulong.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out res);
            else
                e.Cancel = !ulong.TryParse(s, out res);
            if (e.Cancel) tbInstance.SelectAll();
        }

        private void tbInstance_Validated(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count <= 0) return;

            ulong res;
            string s = tbInstance.Text.Trim().ToLower();
            if (s.StartsWith("0x"))
                res = ulong.Parse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null);
            else
                res = ulong.Parse(s);

            items[listView1.SelectedIndices[0]].Instance = res;
            ListViewItem lvi = CreateListViewItem(items[listView1.SelectedIndices[0]]);
            listView1.SelectedItems[0].SubItems[3].Text = lvi.SubItems[3].Text;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            TGIBlock item = new TGIBlock(0, null);

            copyRKToolStripMenuItem.Enabled = (listView1.SelectedIndices.Count != 0);
            pasteRKToolStripMenuItem.Enabled = copyRKToolStripMenuItem.Enabled && TGIBlock.TryParse(Clipboard.GetText(), item);
        }

        private void copyRKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(String.Join("\r\n",
                listView1.SelectedItems.OfType<ListViewItem>().Where(i => i.Tag is TGIBlock).Select(r => (r.Tag as TGIBlock).ToString())));
        }

        private void btnCopy_Click(object sender, EventArgs e) { copyRKToolStripMenuItem_Click(sender, e); }

        private void pasteRKToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0) return;

            TGIBlock item = new TGIBlock(0, null);
            if (!TGIBlock.TryParse(Clipboard.GetText(), item)) return;

            int i = listView1.SelectedIndices[0];
            items[i].ResourceType = item.ResourceType;
            items[i].ResourceGroup = item.ResourceGroup;
            items[i].Instance = item.Instance;

            ListViewItem lvi = CreateListViewItem(item);
            listView1.Items[i].Text = lvi.Text;
            listView1.Items[i].SubItems[1].Text = lvi.SubItems[1].Text;
            listView1.Items[i].SubItems[2].Text = lvi.SubItems[2].Text;
            listView1.Items[i].SubItems[3].Text = lvi.SubItems[3].Text;

            listView1.SelectedIndices.Remove(i);
            listView1.SelectedIndices.Add(i);
            cbType.Focus();
        }

        private void btnPaste_Click(object sender, EventArgs e) { pasteRKToolStripMenuItem_Click(sender, e); }
    }
}

namespace System.Windows.Forms
{
    /// <summary>
    /// A modal form for editing an AResource.DependentList&lt;AResource.TGIBlock&gt; object.
    /// </summary>
    public static class TGIBlockListEditor
    {
        internal static Form OwningForm
        {
            get
            {
                Form owner = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
                if (owner != null && (owner.InvokeRequired || owner.IsDisposed || !owner.IsHandleCreated)) owner = null;
                return owner;
            }
        }

        /// <summary>
        /// Displays a modal form allowing a list of TGIBlocks to be edited (created, updated, deleted).
        /// </summary>
        /// <param name="ltgi">an AResource.DependentList&lt;AResource.TGIBlock&gt; object.</param>
        /// <returns>the DialogResult</returns>
        public static DialogResult Show(DependentList<TGIBlock> ltgi)
        {
            return Show(OwningForm, ltgi);
        }
        /// <summary>
        /// Displays a modal form allowing a list of TGIBlocks to be edited (created, updated, deleted).
        /// </summary>
        /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window
        /// and represents the top-level window that will own this form.</param>
        /// <param name="ltgi">an AResource.DependentList&lt;AResource.TGIBlock&gt; object.</param>
        /// <returns>the DialogResult</returns>
        public static DialogResult Show(IWin32Window owner, DependentList<TGIBlock> ltgi)
        {
            TGIBlockListEditorForm.MainForm theForm = new System.Windows.Forms.TGIBlockListEditorForm.MainForm();
            theForm.Items = ltgi;
            if (owner as Form != null) theForm.Icon = (owner as Form).Icon;
            DialogResult dr = theForm.ShowDialog();
            if (dr != DialogResult.OK) return dr;
            ltgi.Clear();
            ltgi.AddRange(theForm.Items);
            return dr;
        }
    }
}