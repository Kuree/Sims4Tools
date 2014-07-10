using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using s4pi.Interfaces;

namespace S4PIDemoFE
{
    public partial class NewGridForm : Form
    {
        static Size size = new Size();

        bool main = false;
        bool ignoreResize = false;

        public NewGridForm()
        {
            ignoreResize = true;
            InitializeComponent();
            ignoreResize = false;
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
            splitContainer1.Panel1Collapsed = true;
        }

        private NewGridForm(bool main)
            : this()
        {
            this.main = main;
            flpMainButtons.Visible = main;
            btnClose.Visible = !main;
            if (main)
            {
                this.AcceptButton = btnOK;
                this.CancelButton = btnCancel;
                //this.StartPosition = FormStartPosition.CenterParent;
                //Because WindowsDefaultLocation is so dumb, it's distracting to start centred then cascade.
                //Until a better method comes along, always cascade.
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
                size = S4PIDemoFE.Properties.Settings.Default.GridSize;
                if (size.Width == -1 && size.Height == -1)
                    size = new Size(DefaultSize.Width, DefaultSize.Height);
            }
            else
            {
                this.AcceptButton = this.CancelButton = btnClose;
                this.StartPosition = FormStartPosition.WindowsDefaultLocation;
            }
            Size = new Size(size.Width, size.Height);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (!ignoreResize) size = new Size(Width, Height);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (main) S4PIDemoFE.Properties.Settings.Default.GridSize = size;
        }

        public NewGridForm(AApiVersionedFields field, bool main) : this(main) { FieldList = null; s4piPropertyGrid1.s4piObject = field; }
        public NewGridForm(AApiVersionedFields field) : this(field, false) { }
        public NewGridForm(IGenericAdd list) : this(false) { FieldList = list; }

        IGenericAdd fieldList;
        public IGenericAdd FieldList
        {
            get { return fieldList; }
            set
            {
                this.fieldList = value;
                listBox1.Items.Clear();

                if (value == null)
                {
                    splitContainer1.Panel1Collapsed = true;
                    tlpAddDelete.Visible = false;
                    s4piPropertyGrid1.s4piObject = null;
                }
                else
                {
                    contextMenuStrip1.Items.Clear();
                    splitContainer1.Panel1Collapsed = false;
                    tlpAddDelete.Visible = true;
                    if (value.GetType().BaseType.IsGenericType && value.GetType().BaseType.GetGenericArguments()[0].IsAbstract)
                    {
                        btnAdd.Visible = SelectTypes(value.GetType().BaseType.GetGenericArguments()[0]);
                    }
                    else
                    {
                        btnAdd.Visible = true;
                    }
                    fillListBox(-1);
                }
            }
        }
        void fillListBox(int selectedIndex)
        {
            listBox1.SuspendLayout();
            listBox1.Items.Clear();
            string fmt = "[{0:X" + fieldList.Count.ToString("X").Length + "}] {1}";
            for (int i = 0; i < fieldList.Count; i++)
                listBox1.Items.Add(String.Format(fmt, i, fieldList[i].GetType().Name));
            if (selectedIndex == -1) selectedIndex = 0;
            listBox1.SelectedIndex = fieldList.Count > selectedIndex ? selectedIndex : fieldList.Count - 1;
            listBox1.ResumeLayout();
        }

        bool SelectTypes(Type abstractType)
        {
            contextMenuStrip1.ItemClicked -= new ToolStripItemClickedEventHandler(contextMenuStrip1_ItemClicked);
            contextMenuStrip1.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuStrip1_ItemClicked);
            System.Reflection.Assembly ass = abstractType.Assembly;
            foreach (Type type in ass.GetTypes())
            {
                if (!type.IsSubclassOf(abstractType) || type.IsAbstract) continue;

                ToolStripItem tsi = new ToolStripMenuItem(type.Name);
                tsi.Tag = type;
                contextMenuStrip1.Items.Add(tsi);
            }
            return contextMenuStrip1.Items.Count > 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            s4piPropertyGrid1.s4piObject = (AHandlerElement)(listBox1.SelectedIndex >= 0 ? fieldList[listBox1.SelectedIndex] : null);
            btnCopy.Enabled = btnInsert.Enabled = btnDelete.Enabled = listBox1.SelectedIndex >= 0;
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            AHandlerElement selectedElement = (AHandlerElement)(listBox1.SelectedIndex >= 0 ? fieldList[listBox1.SelectedIndex] : null);
            if (selectedElement != null)
                try
                {
                    fieldList.Add(selectedElement.Clone(null));
                    selectedIndex = fieldList.Count - 1;
                }
                catch (Exception ex)
                {
                    MainForm.IssueException(ex, "Copy failed");
                }
            fillListBox(selectedIndex);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.Items.Count == 0) simpleAddInsert(true);
            else { contextMenuStrip1.Tag = "Add"; contextMenuStrip1.Show((Control)sender, new Point(3, 3)); }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.Items.Count == 0) simpleAddInsert(false);
            else { contextMenuStrip1.Tag = "Insert"; contextMenuStrip1.Show((Control)sender, new Point(3, 3)); }
        }

        void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            Type type = e.ClickedItem.Tag as Type;
            if (type == null) return;

            int selectedIndex = listBox1.SelectedIndex;
            try
            {
                fieldList.Add(type);
                if ("Add".Equals(contextMenuStrip1.Tag))
                    selectedIndex = fieldList.Count - 1;
                else
                    doInsert();
            }
            catch (NotImplementedException)
            {
                CopyableMessageBox.Show("This list does not allow entries to be added this way.", "Grid", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MainForm.IssueException(ex, "");
            }
            fillListBox(selectedIndex);
        }

        private void simpleAddInsert(bool add)
        {
            int selectedIndex = listBox1.SelectedIndex;
            try
            {
                fieldList.Add();
                if (add)
                    selectedIndex = fieldList.Count - 1;
                else
                    doInsert();
            }
            catch (NotImplementedException)
            {
                CopyableMessageBox.Show("This list does not allow entries to be added this way.", "Grid", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MainForm.IssueException(ex, "");
            }
            fillListBox(selectedIndex);
        }

        private void doInsert()
        {
            int selectedIndex = listBox1.SelectedIndex;
            object added = fieldList[fieldList.Count - 1];
            fieldList.RemoveAt(fieldList.Count - 1);
            fieldList.Insert(selectedIndex, added);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            try
            {
                // We need to make sure the RemoveAt on AHandlerList<T> is used rather than on IList.
                // IGenericAdd extends IList, so by default, the one on IList gets resolved.
                // Using reflection we can get to the right one.
                Type t = fieldList.GetType();
                var mi = t.GetMethod("RemoveAt");
                var result = mi.Invoke(fieldList, new object[] { listBox1.SelectedIndex, });
            }
            catch (NotImplementedException)
            {
                CopyableMessageBox.Show("This list does not allow entries to be deleted this way.", "Grid", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MainForm.IssueException(ex, "");
            }
            listBox1.SelectedIndex = -1;
            fillListBox(selectedIndex);
        }

        private void btnExpAll_Click(object sender, EventArgs e)
        {
            if (s4piPropertyGrid1.SelectedGridItem == null) return;
            GridItem parent = GetRoot(s4piPropertyGrid1.SelectedGridItem);
            if (parent != null) ExpandAll(parent);
        }
        GridItem GetRoot(GridItem gridItem)
        {
            if (gridItem.GridItemType == GridItemType.Root) return gridItem;
            if (gridItem.Parent != null) return GetRoot(gridItem.Parent);
            return null;
        }
        void ExpandAll(GridItem gridItem)
        {
            foreach (GridItem g in gridItem.GridItems)
            {
                if (g.Label == "AsBytes") continue;
                if (g.GridItemType != GridItemType.Property) continue;
                if (g.Expandable && !g.Expanded)
                {
                    PropertyDescriptor pd = g.PropertyDescriptor;
                    if (pd.PropertyType == typeof(ArrayCTD))
                    {
                        ArrayCTD ctd = g.Value as ArrayCTD;
                        if (ctd == null || ctd.Value == null || ctd.Value.Length > 64) continue;
                        g.Expanded = true;
                        ExpandAll(g);
                    }
                    else
                    {
                        g.Expanded = true;
                        ExpandAll(g);
                    }
                }
            }
        }

        private void btnCollAll_Click(object sender, EventArgs e)
        {
            s4piPropertyGrid1.CollapseAllGridItems();
        }

        // added for http://dino.drealm.info/develforums/s4pi/index.php?topic=685.0
        bool OKtoClose = false;
        private void btnClose_Click(object sender, EventArgs e)
        {
            OKtoClose = true;
            this.Close();
        }

        private void NewGridForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !OKtoClose) e.Cancel = true;
        }
    }
}
