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
using System.Windows.Forms;
using s4pi.Interfaces;
using System.Text.RegularExpressions;

namespace S4PIDemoFE.Filter
{
    public partial class ResourceFilterWidget : UserControl
    {
        IList<string> fields = null;
        IResourceIndexEntry ie = null;
        BrowserWidget bw = null;
        Dictionary<string, FilterField> values = null;

        public ResourceFilterWidget()
        {
            InitializeComponent();
            lbCount.Text = "0";
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<string> Fields
        {
            get { return fields; }
            set
            {
                if (fields == value) return;
                fields = value;
                SetFields();
            }
        }

        static List<string> nonRIEFields = new List<string>(new string[] {
            "Name", "Tag",
        });
        void SetFields()
        {
            Dictionary<string, Type> cft = AApiVersionedFields.GetContentFieldTypes(0, typeof(AResourceIndexEntry));
            values = new Dictionary<string, FilterField>();
            tlpResourceInfo.ColumnCount = fields.Count + 1;
            tlpResourceInfo.RowCount = 1;
            tlpResourceInfo.RowStyles.Clear();
            tlpResourceInfo.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tlpResourceInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tlpResourceInfo.ColumnStyles.Clear();
            tlpResourceInfo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tlpResourceInfo.Controls.Add(tlpControls, 0, 0);
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i] == "Name")
                    tlpResourceInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 133));
                else if (fields[i] == "Instance")
                    tlpResourceInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 150));
                else if (i > 4 || fields[i] == "Tag")
                    tlpResourceInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75));
                else
                    tlpResourceInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                FilterField ff = new FilterField(!nonRIEFields.Contains(fields[i]));
                ff.Name = fields[i];
                ff.Dock = DockStyle.Fill;
                ff.Checked = false;
                ff.Filter = new Regex("");
                ff.Title = fields[i];
                ff.Value = new Regex("");
                ff.TabIndex = i + 2;
                tlpResourceInfo.Controls.Add(ff, i + 1, 0);
                values.Add(fields[i], ff);
            }
            tlpResourceInfo.PerformLayout();
        }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool FilterEnabled { get { return ckbFilter.Checked; } set { ckbFilter.Checked = value; } }

        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool PasteButtonEnabled { get { return btnPasteRK.Enabled; } set { btnPasteRK.Enabled = value; } }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IResourceIndexEntry IndexEntry
        {
            get { return ie; }
            set
            {
                if (ie == value) return;
                ie = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<KeyValuePair<string, Regex>> Filter
        {
            get
            {
                if (values == null) return new List<KeyValuePair<string, Regex>>();
                List<KeyValuePair<string, Regex>> f = new List<KeyValuePair<string, Regex>>();
                foreach (string s in fields) if (values[s].Filter != null && !values[s].Filter.ToString().Equals("^.*$"))
                    f.Add(new KeyValuePair<string, Regex>(s, values[s].Filter));
                return f;
            }
            set
            {
                if (values == null) return;

                foreach (string s in values.Keys)
                    values[s].Checked = false;

                foreach (KeyValuePair<string, Regex> kvp in value)
                {
                    if (values.ContainsKey(kvp.Key))
                    {
                        values[kvp.Key].Checked = true;
                        values[kvp.Key].Value = kvp.Value;
                    }
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BrowserWidget BrowserWidget
        {
            get { return bw; }
            set
            {
                if (bw == value) return;
                if (bw != null) bw.ListUpdated -= new EventHandler(bw_ListUpdated);
                bw = value;
                if (bw != null) bw.ListUpdated += new EventHandler(bw_ListUpdated);
            }
        }

        [Description("Raised when the value of the filter changes")]
        [Category("Property Changed")]
        public event EventHandler FilterChanged;
        protected virtual void OnFilterChanged(object sender, EventArgs e) { if (FilterChanged != null) FilterChanged(sender, e); }

        [Description("Raised when the 'Paste RK' button is clicked")]
        [Category("Action")]
        public event EventHandler PasteClicked;
        protected virtual void OnPasteClicked(object sender, EventArgs e) { if (PasteClicked != null) PasteClicked(sender, e); }

        public void PasteResourceKey(AResourceKey rk)
        {
            foreach (string s in new String[] { "ResourceType", "ResourceGroup", "Instance", })
                if (fields.Contains(s)) values[s].Value = new Regex(rk[s].ToString("X"));
        }

        private void ckbFilter_CheckedChanged(object sender, EventArgs e) { OnFilterChanged(this, new EventArgs()); }

        private void btnRevise_Click(object sender, EventArgs e) { foreach (KeyValuePair<string, FilterField> kvp in values) kvp.Value.Revise(); }

        private void btnQBE_Click(object sender, EventArgs e)
        {
            if (ie == null || values == null) return;
            foreach (string s in fields)
                values[s].Value = new Regex(s.Equals("Name") ? bw.ResourceName(ie).MakeSafe() : s.Equals("Tag") ? bw.ResourceTag(ie) : ie[s].ToString("X"));
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, FilterField> kvp in values) kvp.Value.Set();
            if (ckbFilter.Checked) OnFilterChanged(this, new EventArgs());
        }

        private void bw_ListUpdated(object sender, EventArgs e) { lbCount.Text = (sender as BrowserWidget).Count.ToString(); }

        private void btnPasteRK_Click(object sender, EventArgs e) { OnPasteClicked(sender, e); }
    }

    static class Extension
    {
        public static string MakeSafe(this string value)
        {
            return value.Replace(@"\", @"\\");
        }
    }
}
