using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using s4pi.WrapperDealer;
using s4pi.Extensions;

namespace S4PIDemoFE.Settings
{
    public partial class ManageWrappersDialog : Form
    {
        public ManageWrappersDialog()
        {
            InitializeComponent();
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));

            lvWrappers.ListViewItemSorter = new ListViewColumnSorter() { SortColumn = 0, Order = SortOrder.Ascending, };
            lvDisabled.ListViewItemSorter = new ListViewColumnSorter() { SortColumn = 0, Order = SortOrder.Ascending, };

            PopulateWrappers();
            PopulateDisable();
        }

        void PopulateWrappers() { PopulateListView(lvWrappers, new List<KeyValuePair<string, Type>>(WrapperDealer.TypeMap), x => !WrapperDealer.Disabled.Contains(x)); }

        void PopulateDisable() { PopulateListView(lvDisabled, new List<KeyValuePair<string, Type>>(WrapperDealer.Disabled), x => true); }

        void PopulateListView(ListView lv, List<KeyValuePair<string, Type>> map, Predicate<KeyValuePair<string, Type>> match)
        {
            lv.Visible = false;
            lv.BeginUpdate();
            lv.Items.Clear();
            string fmt = "{0:D" + (map.Count.ToString().Length) + "}";
            int order = 0;
            foreach (var kvp in map)
            {
                order++;
                if (!match(kvp)) continue;
                string tag = getTag(kvp.Key);
                string wrapper = kvp.Value.Name;
                string file = System.IO.Path.GetFileName(kvp.Value.Assembly.Location);
                string title = GetAttrValue(kvp.Value.Assembly, typeof(System.Reflection.AssemblyTitleAttribute), "Title");
                string description = GetAttrValue(kvp.Value.Assembly, typeof(System.Reflection.AssemblyDescriptionAttribute), "Description");
                string company = GetAttrValue(kvp.Value.Assembly, typeof(System.Reflection.AssemblyCompanyAttribute), "Company");
                string product = GetAttrValue(kvp.Value.Assembly, typeof(System.Reflection.AssemblyProductAttribute), "Product");
                ListViewItem lvi = new ListViewItem(new string[] { String.Format(fmt, order), tag, kvp.Key, wrapper, file, title, description, company, product, });
                lvi.Tag = kvp;
                lv.Items.Add(lvi);
            }
            if (lv.Items.Count > 0)
                lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            lv.EndUpdate();
            lv.Visible = true;
        }

        string GetAttrValue(System.Reflection.Assembly assy, Type attrType, string field)
        {
            object[] ary = assy.GetCustomAttributes(attrType, false);
            if (ary == null || ary.Length != 1) return "(not set)";
            object value = attrType.InvokeMember(field, System.Reflection.BindingFlags.GetProperty, null, ary[0], new object[] { });
            return value == null ? "(not set)" : (string)value;
        }

        string getTag(string key)
        {
            if (ExtList.Ext.ContainsKey(key)) return ExtList.Ext[key][0];
            if (ExtList.Ext.ContainsKey("*")) return ExtList.Ext["*"][0];
            return "";
        }

        void doSort(ListView lv)
        {
            lv.Sort();
            if (lv.Items.Count > 0)
                lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            if (lv.SelectedItems.Count > 0)
                lv.EnsureVisible(lv.SelectedIndices[0]);
        }

        void SortBoth()
        {
            doSort(lvWrappers);
            doSort(lvDisabled);
        }

        void lvMove(ListView lvFrom, ListView lvTo)
        {
            if (lvFrom.SelectedIndices.Count == 0) return;
            lvFrom.BeginUpdate();
            lvTo.BeginUpdate();

            List<ListViewItem> llvi = new List<ListViewItem>();
            for (int i = 0; i < lvFrom.SelectedIndices.Count; i++)
                llvi.Add(lvFrom.Items[lvFrom.SelectedIndices[i]]);

            foreach (var lvi in llvi)
            {
                lvFrom.Items.Remove(lvi);
                lvTo.Items.Add(lvi);
            }
            SortBoth();

            lvTo.EndUpdate();
            lvFrom.EndUpdate();
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            WrapperDealer.Disabled.Clear();
            foreach (var lvi in lvDisabled.Items)
                WrapperDealer.Disabled.Add((KeyValuePair<string, Type>)((lvi as ListViewItem).Tag));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            lvMove(lvDisabled, lvWrappers);
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            lvMove(lvWrappers, lvDisabled);
        }

        private void lv_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ((sender as ListView).ListViewItemSorter as ListViewColumnSorter).SortColumn = e.Column;
            (sender as ListView).Sort();
            return;
        }

        private void lvWrappers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == lvWrappers)
                btnDisable.Enabled = lvWrappers.SelectedItems.Count > 0;
            else
                btnEnable.Enabled = lvDisabled.SelectedItems.Count > 0;
        }
    }
}
