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
using s4pi.Extensions;

namespace System.Windows.Forms
{
    public partial class TGIBlockCombo : UserControl
    {
        public TGIBlockCombo()
        {
            InitializeComponent();
        }

        public TGIBlockCombo(DependentList<TGIBlock> tgiBlocks) : this(tgiBlocks, -1, true) { }
        public TGIBlockCombo(DependentList<TGIBlock> tgiBlocks, int index) : this(tgiBlocks, index, true) { }
        public TGIBlockCombo(DependentList<TGIBlock> tgiBlocks, int index, bool showEdit) : this() { TGIBlocks = tgiBlocks; SelectedIndex = index; ShowEdit = showEdit; }

        DependentList<TGIBlock> tgiBlocks;
        public DependentList<TGIBlock> TGIBlocks
        {
            get { return tgiBlocks; }
            set
            {
                if (tgiBlocks == value) return;
                TGIBlock tgib = tgiBlocks == null ? null : cbTGIBlocks.SelectedIndex >= 0 && cbTGIBlocks.SelectedIndex < cbTGIBlocks.Items.Count ? tgiBlocks[cbTGIBlocks.SelectedIndex] : null;
                string selectedItem = tgib == null ? "(null)" : MakeItem(tgib);
                tgiBlocks = value;
                FillList(selectedItem);
            }
        }
        void FillList(string item)
        {
            int index = -1;
            cbTGIBlocks.Items.Clear();
            if (tgiBlocks != null)
                foreach (TGIBlock tgib in tgiBlocks)
                {
                    string s = MakeItem(tgib);
                    if (s == item) index = cbTGIBlocks.Items.Count;
                    cbTGIBlocks.Items.Add(s);
                }

            cbTGIBlocks.SelectedIndex = index;
            btnTGIBlockListEditor.Enabled = tgiBlocks != null;
        }
        string MakeItem(TGIBlock tgib)
        {
            string s = "";
            if (ExtList.Ext.ContainsKey("0x" + tgib.ResourceType.ToString("X8")))
                s += " (" + ExtList.Ext["0x" + tgib.ResourceType.ToString("X8")][0] + ")";
            return tgib + s;
        }

        public event EventHandler SelectedIndexChanged;
        protected void OnSelectedIndexChanged(object sender, EventArgs e) { if (SelectedIndexChanged != null) SelectedIndexChanged(sender, e); }
        [DefaultValue(-1)]
        public int SelectedIndex
        {
            get { return cbTGIBlocks.SelectedIndex; }
            set { if (cbTGIBlocks.SelectedIndex != value) { cbTGIBlocks.SelectedIndex = value; } }
        }

        [DefaultValue(true)]
        public bool ShowEdit { get { return btnTGIBlockListEditor.Visible; } set { btnTGIBlockListEditor.Visible = value; } }

        public override void Refresh()
        {
            TGIBlock tgib = tgiBlocks == null ? null : cbTGIBlocks.SelectedIndex >= 0 && cbTGIBlocks.SelectedIndex < tgiBlocks.Count ? tgiBlocks[cbTGIBlocks.SelectedIndex] : null;
            string selectedItem = tgib == null ? "(null)" : MakeItem(tgib);
            FillList(selectedItem);

            base.Refresh();
        }

        public event EventHandler TGIBlockListChanged;
        protected void OnTGIBlockListChanged(object sender, EventArgs e) { if (TGIBlockListChanged != null) TGIBlockListChanged(sender, e); }

        private void btnTGIBlockListEditor_Click(object sender, EventArgs e)
        {
            CountedTGIBlockList tgiBlocksCopy = new CountedTGIBlockList(null, tgiBlocks);
            DialogResult dr = TGIBlockListEditor.Show(tgiBlocksCopy);
            if (dr != DialogResult.OK) return;

            TGIBlock tgib = tgiBlocks == null ? null : cbTGIBlocks.SelectedIndex >= 0 && cbTGIBlocks.SelectedIndex < tgiBlocks.Count ? tgiBlocks[cbTGIBlocks.SelectedIndex] : null;
            string selectedItem = tgib == null ? "(null)" : MakeItem(tgib);
            tgiBlocks.Clear();
            tgiBlocks.AddRange(tgiBlocksCopy.ToArray());

            FillList(selectedItem);

            OnTGIBlockListChanged(this, EventArgs.Empty);
        }

        private void cbTGIBlocks_SelectedIndexChanged(object sender, EventArgs e) { OnSelectedIndexChanged(this, e); }
    }
}
