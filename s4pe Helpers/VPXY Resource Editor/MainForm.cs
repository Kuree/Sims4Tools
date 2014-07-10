/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
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
using System.IO;
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;

namespace s3pe_VPXY_Resource_Editor
{
    public partial class MainForm : Form, s3pi.Helpers.IRunHelper
    {
        const string myName = "s3pe VPXY Resource Editor";
        public MainForm()
        {
            InitializeComponent();

            this.Controls.Remove(lbCurrentPart);
            this.Controls.Remove(lbLPCurrent);
            lnud.AddRange(new NumericUpDown[]{
                nudLowerX, nudLowerY, nudLowerZ,
                nudUpperX, nudUpperY, nudUpperZ,
            });
        }

        public MainForm(Stream s)
            :this()
        {
            try
            {
                Application.UseWaitCursor = true;
                loadVPXY(s);
            }
            finally { Application.UseWaitCursor = false; }
            btnOK.Enabled = btnOKEnabled;
        }

        bool btnOKEnabled { get { return dirty && vpxy.TGIBlocks.Count > 0 && (!vpxy.Modular || tbcFTPT.SelectedIndex >= 0); } }

        byte[] result = null;
        public byte[] Result { get { return result; } }

        GenericRCOLResource rcol;
        VPXY vpxy;
        List<TGIBlockCombo> ltbc = new List<TGIBlockCombo>();
        List<TGIBlockCombo> lLPtbc = new List<TGIBlockCombo>();
        List<NumericUpDown> lnud = new List<NumericUpDown>();
        int currentVPXYEntry = -1;
        int currentPartEntry = -1;
        int currentLPEntry = -1;
        bool dirty = false;

        void loadVPXY(Stream data)
        {
            try
            {
                rcol = new GenericRCOLResource(0, data);
            }
            catch { rcol = null; }
            if (rcol == null || rcol.ChunkEntries.Count != 1 || rcol.ChunkEntries[0].RCOLBlock.Tag != "VPXY")
            {
                throw new Exception("RCOL was not a VPXY resource.");
            }
            vpxy = rcol.ChunkEntries[0].RCOLBlock as VPXY;
            if (vpxy == null)
            {
                throw new Exception("VPXY resource contains invalid RCOL Chunk.");
            }

            FillPartsTLP();

            nudLowerX.Value = new Decimal(vpxy.Bounds.Min.X);
            nudLowerY.Value = new Decimal(vpxy.Bounds.Min.Y);
            nudLowerZ.Value = new Decimal(vpxy.Bounds.Min.Z);
            nudUpperX.Value = new Decimal(vpxy.Bounds.Max.X);
            nudUpperY.Value = new Decimal(vpxy.Bounds.Max.Y);
            nudUpperZ.Value = new Decimal(vpxy.Bounds.Max.Z);

            tbcFTPT.Enabled = ckbModular.Checked = vpxy.Modular;
            tbcFTPT.TGIBlocks = vpxy.TGIBlocks;
            tbcFTPT.SelectedIndex = vpxy.Modular && vpxy.FTPTIndex < vpxy.TGIBlocks.Count ? (int)vpxy.FTPTIndex : -1;

            rcol.ResourceChanged += new EventHandler(rcol_ResourceChanged);
        }

        void saveVPXY()
        {
            rcol.ResourceChanged -= new EventHandler(rcol_ResourceChanged);
            try
            {
                ClearLinkedPartsTLP(true);
                CountedTGIBlockList ltgib = new CountedTGIBlockList(null);
                vpxy.Entries.Clear();
                int count = 0;
                byte count00 = 1;
                for (int row = 1; row < tlpParts.RowCount - 1; row++)
                {
                    TGIBlockCombo c = tlpParts.GetControlFromPosition(2, row) as TGIBlockCombo;
                    if (ltbc.IndexOf(c) < 0) continue;
                    if (c.SelectedIndex < 0) continue;
                    ltgib.Add(vpxy.TGIBlocks[c.SelectedIndex]);
                    vpxy.Entries.Add(new VPXY.Entry01(0, null, 1, count++));
                    if (c.Tag != null)
                    {
                        VPXY.Entry00 e00 = c.Tag as VPXY.Entry00;
                        if (e00.TGIIndexes.Count <= 0) continue;
                        e00.EntryID = count00++;
                        e00.TGIIndexes.ForEach(elem =>
                        {
                            ltgib.Add(vpxy.TGIBlocks[elem]);
                            elem = count++;
                        });
                        if (e00.TGIIndexes.Count > 0)
                            vpxy.Entries.Add(e00);
                    }
                }
                if (vpxy.Modular)
                {
                    ltgib.Add(vpxy.TGIBlocks[tbcFTPT.SelectedIndex]);
                    vpxy.FTPTIndex = count++;
                }

                vpxy.TGIBlocks.Clear();
                vpxy.TGIBlocks.AddRange(ltgib);

                result = (byte[])rcol.AsBytes.Clone();
            }
            finally { rcol.ResourceChanged -= new EventHandler(rcol_ResourceChanged); }
        }

        void ClearPartsTLP()
        {
            for (int i = 0; i < tlpParts.Controls.Count; i++)
                if (tlpParts.Controls[i] as Label == null)
                    tlpParts.Controls.Remove(tlpParts.Controls[i--]);
            while (tlpParts.RowStyles.Count > 2)
                tlpParts.RowStyles.RemoveAt(1);
            tlpParts.RowCount = 2;
        }
        void FillPartsTLP()
        {
            tlpParts.SuspendLayout();
            ClearPartsTLP();
            int tabindex = 1;
            for (int i = 0; i < vpxy.Entries.Count; i++)
            {
                if (vpxy.Entries[i] as VPXY.Entry00 != null)
                {
                    ltbc[ltbc.Count - 1].Tag = vpxy.Entries[i] as VPXY.Entry00;
                }
                else if (vpxy.Entries[i] as VPXY.Entry01 != null)
                {
                    VPXY.Entry01 e01 = vpxy.Entries[i] as VPXY.Entry01;
                    AddTableRowTBC(tlpParts, i, (int)e01.TGIIndex, ref tabindex);
                }
            }
            tlpParts.ResumeLayout();
        }
        void RenumberTLP()
        {
            tlpParts.SuspendLayout();
            int count = 0;
            int row = 1;
            for (int i = 0; i < vpxy.Entries.Count; i++)
            {
                if (vpxy.Entries[i] as VPXY.Entry00 != null)
                {
                    count += (vpxy.Entries[i] as VPXY.Entry00).TGIIndexes.Count;
                }
                else if (vpxy.Entries[i] as VPXY.Entry01 != null)
                {
                    Label lb = tlpParts.GetControlFromPosition(0, row++) as Label;
                    lb.Text = count.ToString("X");
                    lb.Tag = ltbc[row - 2];
                    lb.TabIndex = (tlpParts.GetControlFromPosition(2, row - 1) as TGIBlockCombo).TabIndex - 1;
                    count++;
                }
            }
            if (currentVPXYEntry != -1)
                currentVPXYEntry = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(ltbc[currentPartEntry]).Row)).Text, System.Globalization.NumberStyles.HexNumber);
            tlpParts.ResumeLayout();
        }
        void ClearLinkedPartsTLP(bool saving)
        {
            if (currentPartEntry != -1)
            {
                VPXY.Entry00 e00 = ltbc[currentPartEntry].Tag as VPXY.Entry00;
                if (e00 != null)
                {
                    e00.TGIIndexes.Clear();
                    for (int row = 1; row < tlpLinkedParts.RowCount - 1; row++)
                    {
                        TGIBlockCombo c = tlpLinkedParts.GetControlFromPosition(2, row) as TGIBlockCombo;
                        if (lLPtbc.IndexOf(c) < 0) continue;
                        if (c.SelectedIndex < 0) continue;
                        e00.TGIIndexes.Add(c.SelectedIndex);
                    }
                    if (saving && e00.TGIIndexes.Count == 0)
                    {
                        vpxy.Entries.Remove(e00);
                        ltbc[currentPartEntry].Tag = null;
                        RenumberTLP();
                    }
                }
            }
            currentLPEntry = -1;
            lLPtbc = null;
            for (int i = 0; i < tlpLinkedParts.Controls.Count; i++)
                if (!tlpLinkedParts.Controls[i].Equals(lbLPTitle))
                    tlpLinkedParts.Controls.Remove(tlpLinkedParts.Controls[i--]);
            while (tlpLinkedParts.RowStyles.Count > 2)
                tlpLinkedParts.RowStyles.RemoveAt(1);
            tlpLinkedParts.RowCount = 2;
            tlpLPControls.Enabled = tlpLinkedParts.Enabled = false;
        }
        void FillLinkedPartsTLP(int offset, VPXY.Entry00 entry)
        {
            tlpLinkedParts.SuspendLayout();
            //ClearLinkedPartsTLP(false);// should never be needed
            lLPtbc = new List<TGIBlockCombo>();
            int tabindex = 1;
            for (int i = 0; i < entry.TGIIndexes.Count; i++)
            {
                AddTableRowTBC(tlpLinkedParts, offset + 1 + i, entry.TGIIndexes[i], ref tabindex);
            }
            tlpLPControls.Enabled = tlpLinkedParts.Enabled = true;
            tlpLinkedParts.ResumeLayout();
        }
        void AddTableRowTBC(TableLayoutPanel tlp, int entry, int index, ref int tabindex)
        {
            tlp.RowCount++;
            tlp.RowStyles.Insert(tlp.RowCount - 2, new RowStyle(SizeType.AutoSize));

            Label lb = new Label();
            TGIBlockCombo tbc = new TGIBlockCombo(vpxy.TGIBlocks, index, false);

            lb.AutoSize = true;
            lb.BorderStyle = BorderStyle.Fixed3D;
            lb.Dock = DockStyle.Fill;
            lb.FlatStyle = FlatStyle.Standard;
            lb.Margin = new Padding(0);
            lb.Name = "lbEntry" + tabindex;
            lb.TabIndex++;
            lb.Text = entry.ToString("X");
            lb.TextAlign = ContentAlignment.MiddleRight;
            lb.Tag = tbc;
            lb.Click += new EventHandler(lb_Click);
            tlp.Controls.Add(lb, 0, tlp.RowCount - 2);

            tbc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tbc.Name = "tbc" + tabindex;
            tbc.TabIndex = tabindex++;
            tbc.Enter += new EventHandler(tbc_Enter);
            tbc.SelectedIndexChanged += new EventHandler(tbc_SelectedIndexChanged);
            tlp.Controls.Add(tbc, 2, tlp.RowCount - 2);

            if (tlp == tlpParts)
                ltbc.Add(tbc);
            else
                lLPtbc.Add(tbc);
            tbc.Focus();

            tbc.TGIBlockListChanged += new EventHandler(tbg_TGIBlockListChanged);
        }
        void LPRenumberTLP()
        {
            tlpLinkedParts.SuspendLayout();
            RenumberTLP();
            VPXY.Entry00 entry = vpxy.Entries[currentVPXYEntry + 1] as VPXY.Entry00;
            int count = currentVPXYEntry + 1;
            int row = 1;
            for (int i = 0; i < entry.TGIIndexes.Count; i++)
            {
                Label lb = tlpLinkedParts.GetControlFromPosition(0, row++) as Label;
                lb.Text = count.ToString("X");
                lb.Tag = lLPtbc[row - 2];
                lb.TabIndex = (tlpLinkedParts.GetControlFromPosition(2, row - 1) as TGIBlockCombo).TabIndex - 1;
                count++;
            }
            tlpLinkedParts.ResumeLayout();
        }

        void rcol_ResourceChanged(object sender, EventArgs e)
        {
            dirty = true;
            btnOK.Enabled = btnOKEnabled;
        }

        void tbg_TGIBlockListChanged(object sender, EventArgs e)
        {
            if (ltbc != null) foreach (TGIBlockCombo tbc in ltbc) tbc.Refresh();
            if (lLPtbc != null) foreach (TGIBlockCombo tbc in lLPtbc) tbc.Refresh();
            if (vpxy.Modular) tbcFTPT.Refresh();
        }

        void lb_Click(object sender, EventArgs e) { (((Label)sender).Tag as TGIBlockCombo).Focus(); }

        void tbc_Enter(object sender, EventArgs e)
        {
            TGIBlockCombo tbc = sender as TGIBlockCombo;

            if (ltbc.Contains(tbc))
            {
                ClearLinkedPartsTLP(true);//before currentEntry changes
                currentPartEntry = ltbc.IndexOf(tbc);
                btnMoveUp.Enabled = currentPartEntry > 0;
                btnMoveDown.Enabled = currentPartEntry < ltbc.Count - 1;
                currentVPXYEntry = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(ltbc[currentPartEntry]).Row)).Text, System.Globalization.NumberStyles.HexNumber);
                tlpParts.Controls.Add(lbCurrentPart, 1, currentPartEntry + 1);

                if (tbc.Tag != null)
                {
                    btnAddLinked.Enabled = false;
                    FillLinkedPartsTLP(currentVPXYEntry, tbc.Tag as VPXY.Entry00);
                }
                else
                    btnAddLinked.Enabled = true;
            }
            else
            {
                currentLPEntry = lLPtbc.IndexOf(tbc);
                btnLPUp.Enabled = currentLPEntry > 0;
                btnLPDown.Enabled = currentLPEntry < lLPtbc.Count - 1;
                tlpLinkedParts.Controls.Add(lbLPCurrent, 1, currentLPEntry + 1);
            }
        }

        void tbc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TGIBlockCombo tbc = sender as TGIBlockCombo;

            if (ltbc.Contains(tbc))
            {
                int i = int.Parse(((Label)tlpParts.GetControlFromPosition(0, tlpParts.GetCellPosition(tbc).Row)).Text, System.Globalization.NumberStyles.HexNumber);
                VPXY.Entry01 e01 = vpxy.Entries[i] as VPXY.Entry01;
                e01.TGIIndex = (tbc.SelectedIndex >= 0) ? tbc.SelectedIndex : 0;
            }
            else
            {
                if (currentPartEntry == -1) return;
                VPXY.Entry00 e00 = ltbc[currentPartEntry].Tag as VPXY.Entry00;
                int i = lLPtbc.IndexOf(tbc);
                e00.TGIIndexes[i] = (tbc.SelectedIndex >= 0) ? tbc.SelectedIndex : 0;
            }
        }

        private void btnTGIEditor_Click(object sender, EventArgs e)
        {
            CountedTGIBlockList tgiBlocksCopy = new CountedTGIBlockList(null, vpxy.TGIBlocks);
            DialogResult dr = TGIBlockListEditor.Show(tgiBlocksCopy);
            if (dr != DialogResult.OK) return;
            vpxy.TGIBlocks.Clear();
            vpxy.TGIBlocks.AddRange(tgiBlocksCopy.ToArray());
        }

        delegate void TLPRenumber();
        void moveUp(TableLayoutPanel tlp, List<TGIBlockCombo> ltbc, Label lb, TLPRenumber renumber, ref int entry)
        {
            TGIBlockCombo c1 = tlp.GetControlFromPosition(2, entry + 1) as TGIBlockCombo;//this control
            TGIBlockCombo c2 = tlp.GetControlFromPosition(2, entry) as TGIBlockCombo;//the one above to swap with
            tlp.Controls.Remove(c1);//leaves entry + 1 free
            tlp.Controls.Add(c2, 2, entry + 1);//leaves entry free
            tlp.Controls.Add(c1, 2, entry);
            c1.TabIndex--;
            c2.TabIndex++;

            int i = ltbc.IndexOf(c1);
            ltbc.RemoveAt(i);
            ltbc.Insert(i - 1, c1);

            entry--;
            tlp.Controls.Add(lb, 1, entry + 1);
            renumber();
            c1.Focus();
        }

        void moveDown(TableLayoutPanel tlp, List<TGIBlockCombo> ltbc, Label lb, TLPRenumber renumber, ref int entry)
        {
            TGIBlockCombo c1 = tlp.GetControlFromPosition(2, entry + 1) as TGIBlockCombo;//this control
            TGIBlockCombo c2 = tlp.GetControlFromPosition(2, entry + 2) as TGIBlockCombo;//the one below to swap with
            tlp.Controls.Remove(c1);//leaves entry + 1 free
            tlp.Controls.Add(c2, 2, entry + 1);//leaves entry + 2 free
            tlp.Controls.Add(c1, 2, entry + 2);
            c2.TabIndex--;
            c1.TabIndex++;

            int i = ltbc.IndexOf(c1);
            ltbc.RemoveAt(i);
            ltbc.Insert(i + 1, c1);

            entry++;
            tlp.Controls.Add(lb, 1, entry + 1);
            renumber();
            c1.Focus();
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (currentPartEntry < 1 || ltbc.Count < 2) return;

            VPXY.Entry entry = vpxy.Entries[currentVPXYEntry];
            vpxy.Entries.RemoveAt(currentVPXYEntry);
            if (ltbc[currentPartEntry].Tag != null) vpxy.Entries.RemoveAt(currentVPXYEntry);
            vpxy.Entries.Insert(currentVPXYEntry - 1, entry);
            if (ltbc[currentPartEntry].Tag != null) vpxy.Entries.Insert(currentVPXYEntry, ltbc[currentPartEntry].Tag as VPXY.Entry00);

            moveUp(tlpParts, ltbc, lbCurrentPart, RenumberTLP, ref currentPartEntry);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            if (currentPartEntry == ltbc.Count - 1 || ltbc.Count < 2) return;

            VPXY.Entry entry = vpxy.Entries[currentVPXYEntry];
            vpxy.Entries.RemoveAt(currentVPXYEntry);
            if (ltbc[currentPartEntry].Tag != null) vpxy.Entries.RemoveAt(currentVPXYEntry);
            vpxy.Entries.Insert(currentVPXYEntry + 1, entry);
            if (ltbc[currentPartEntry].Tag != null) vpxy.Entries.Insert(currentVPXYEntry + 2, ltbc[currentPartEntry].Tag as VPXY.Entry00);

            moveDown(tlpParts, ltbc, lbCurrentPart, RenumberTLP, ref currentPartEntry);
        }

        private void btnAddPart_Click(object sender, EventArgs e)
        {
            int ec = vpxy.Entries.Count;
            int tabindex = tlpParts.RowCount;
            vpxy.Entries.Add(new VPXY.Entry01(0, null, 1, 0));
            AddTableRowTBC(tlpParts, ec, -1, ref tabindex);
            tbc_Enter(ltbc[ltbc.Count - 1], EventArgs.Empty);
        }

        private void btnAddLinked_Click(object sender, EventArgs e)
        {
            VPXY.Entry00 e00 = new VPXY.Entry00(0, null, 0, (byte)vpxy.Entries.Count, new List<int>());
            vpxy.Entries.Insert(currentVPXYEntry + 1, e00);
            ltbc[currentVPXYEntry].Tag = e00;

            RenumberTLP();
            btnAddLinked.Enabled = false;
            FillLinkedPartsTLP(currentVPXYEntry, e00);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (currentPartEntry == -1) return;
            ClearLinkedPartsTLP(true);
            tlpParts.Controls.Remove(lbCurrentPart);

            VPXY.Entry entry = vpxy.Entries[currentVPXYEntry];
            vpxy.Entries.RemoveAt(currentVPXYEntry);
            if (ltbc[currentPartEntry].Tag != null) vpxy.Entries.RemoveAt(currentVPXYEntry);

            Control c1 = tlpParts.GetControlFromPosition(0, currentPartEntry + 1);
            tlpParts.Controls.Remove(c1);
            c1 = tlpParts.GetControlFromPosition(2, currentPartEntry + 1);
            tlpParts.Controls.Remove(c1);
            ltbc.Remove(c1 as TGIBlockCombo);

            for (int row = currentPartEntry + 1; row < tlpParts.RowCount - 2; row++)
            {
                c1 = tlpParts.GetControlFromPosition(0, row + 1);
                c1.TabIndex--;
                tlpParts.Controls.Add(c1, 0, row);
                c1 = tlpParts.GetControlFromPosition(2, row + 1);
                c1.TabIndex--;
                tlpParts.Controls.Add(c1, 2, row);
            }
            tlpParts.RowCount--;

            currentVPXYEntry = currentPartEntry = -1;
            btnAddLinked.Enabled = false;
            RenumberTLP();
        }

        private void btnLPUp_Click(object sender, EventArgs e)
        {
            if (currentPartEntry == -1) return;
            VPXY.Entry00 e00 = ltbc[currentPartEntry].Tag as VPXY.Entry00;
            if (currentLPEntry < 1 || e00.TGIIndexes.Count < 2) return;

            int val = e00.TGIIndexes[currentLPEntry];
            e00.TGIIndexes.RemoveAt(currentLPEntry);
            e00.TGIIndexes.Insert(currentLPEntry - 1, val);

            moveUp(tlpLinkedParts, lLPtbc, lbLPCurrent, LPRenumberTLP, ref currentLPEntry);
        }

        private void btnLPDown_Click(object sender, EventArgs e)
        {
            if (currentPartEntry == -1) return;
            VPXY.Entry00 e00 = ltbc[currentPartEntry].Tag as VPXY.Entry00;
            if (currentLPEntry == e00.TGIIndexes.Count - 1 || e00.TGIIndexes.Count < 2) return;

            int val = e00.TGIIndexes[currentLPEntry];
            e00.TGIIndexes.RemoveAt(currentLPEntry);
            e00.TGIIndexes.Insert(currentLPEntry + 1, val);

            moveDown(tlpLinkedParts, lLPtbc, lbLPCurrent, LPRenumberTLP, ref currentLPEntry);
        }

        private void btnLPAdd_Click(object sender, EventArgs e)
        {
            int tabindex = tlpLinkedParts.RowCount;
            VPXY.Entry00 e00 = ltbc[currentPartEntry].Tag as VPXY.Entry00;
            e00.TGIIndexes.Add();
            AddTableRowTBC(tlpLinkedParts, currentVPXYEntry + e00.TGIIndexes.Count, -1, ref tabindex);
            tbc_Enter(lLPtbc[lLPtbc.Count - 1], EventArgs.Empty);
            LPRenumberTLP();
        }

        private void btnLPDel_Click(object sender, EventArgs e)
        {
            if (currentLPEntry == -1) return;
            tlpLinkedParts.Controls.Remove(lbLPCurrent);

            VPXY.Entry00 e00 = ltbc[currentPartEntry].Tag as VPXY.Entry00;
            e00.TGIIndexes.RemoveAt(currentLPEntry);

            Control c1 = tlpLinkedParts.GetControlFromPosition(0, currentLPEntry + 1);
            tlpLinkedParts.Controls.Remove(c1);
            c1 = tlpLinkedParts.GetControlFromPosition(2, currentLPEntry + 1);
            tlpLinkedParts.Controls.Remove(c1);
            lLPtbc.Remove(c1 as TGIBlockCombo);

            for (int row = currentLPEntry + 1; row < tlpLinkedParts.RowCount - 2; row++)
            {
                c1 = tlpLinkedParts.GetControlFromPosition(0, row + 1);
                c1.TabIndex--;
                tlpLinkedParts.Controls.Add(c1, 0, row);
                c1 = tlpLinkedParts.GetControlFromPosition(2, row + 1);
                c1.TabIndex--;
                tlpLinkedParts.Controls.Add(c1, 2, row);
            }
            tlpLinkedParts.RowCount--;
            currentLPEntry = -1;
            LPRenumberTLP();
        }

        private void nud_ValueChanged(object sender, EventArgs e)
        {
            switch (lnud.IndexOf(sender as NumericUpDown))
            {
                case 0: vpxy.Bounds.Min.X = Decimal.ToSingle(nudLowerX.Value); break;
                case 1: vpxy.Bounds.Min.Y = Decimal.ToSingle(nudLowerY.Value); break;
                case 2: vpxy.Bounds.Min.Z = Decimal.ToSingle(nudLowerZ.Value); break;
                case 3: vpxy.Bounds.Max.X = Decimal.ToSingle(nudUpperX.Value); break;
                case 4: vpxy.Bounds.Max.Y = Decimal.ToSingle(nudUpperY.Value); break;
                case 5: vpxy.Bounds.Max.Z = Decimal.ToSingle(nudUpperZ.Value); break;
            }
        }

        private void ckbModular_CheckedChanged(object sender, EventArgs e)
        {
            vpxy.Modular = tbcFTPT.Enabled = ckbModular.Checked;
            if (ckbModular.Checked) tbcFTPT.Refresh();
        }

        private void tbcFTPT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!vpxy.Modular) return;
            vpxy.FTPTIndex = (byte)(tbcFTPT.SelectedIndex > 0 ? tbcFTPT.SelectedIndex : 0);
        }

        private void btnEditTGIs_Click(object sender, EventArgs e)
        {
            DialogResult dr = TGIBlockListEditor.Show(this, vpxy.TGIBlocks);
            if (dr != DialogResult.OK) return;
            tbg_TGIBlockListChanged(sender, e);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Environment.ExitCode = dirty ? 0 : 1;
            saveVPXY();
            this.Close();
        }
    }
}
