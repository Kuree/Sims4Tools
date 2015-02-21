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
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using s4pi.Extensions;
using s4pi.Interfaces;

namespace S4PIDemoFE
{
    public partial class ResourceDetails : Form, IResourceKey
    {
        bool internalchg;

        public ResourceDetails() : this(true, true) { }
        public ResourceDetails(bool useName, bool displayFilename)
        {
            InitializeComponent();
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
            tlpName.Enabled = UseName = useName;
            lbFilename.Visible = tbFilename.Visible = displayFilename;
        }
        public ResourceDetails(bool useName, bool displayFilename, IResourceKey rk)
            : this(useName, displayFilename)
        {
            internalchg = true;
            try
            {
                ResourceType = rk.ResourceType;
                ResourceGroup = rk.ResourceGroup;
                Instance = rk.Instance;
                btnOK.Enabled = btnOKCanEnable;
            }
            finally { internalchg = false; UpdateTGIN(); }
        }

        #region IResourceKey Members
        public uint ResourceType { get { return cbType.Value; } set { cbType.Value = value; UpdateTGIN(); } }
        public uint ResourceGroup
        {
            get { return Convert.ToUInt32(tbGroup.Text, tbGroup.Text.StartsWith("0x") ? 16 : 10); }
            set { tbGroup.Text = "0x" + value.ToString("X8"); UpdateTGIN(); }
        }
        public ulong Instance
        {
            get { return Convert.ToUInt64(tbInstance.Text, tbInstance.Text.StartsWith("0x") ? 16 : 10); }
            set { tbInstance.Text = "0x" + value.ToString("X16"); UpdateTGIN(); }
        }
        #endregion

        public string ResourceName { get { return tbName.Text; } set { tbName.Text = value; UpdateTGIN(); } }
        public bool Replace { get { return importSettings1.Replace; } }
        public bool Compress { get { return importSettings1.Compress; } set { importSettings1.Compress = value; } }

        public bool UseName { get { return importSettings1.UseName; } set { importSettings1.UseName = value; } }
        public bool AllowRename { get { return importSettings1.AllowRename; } set { importSettings1.AllowRename = value; } }

        public string Filename { get { return tbFilename.Text; } set { this.tbFilename.Text = value; FillPanel(); } }
        public static implicit operator TGIN(ResourceDetails form) { return form.details; }

        TGIN details;
        private void FillPanel()
        {
            internalchg = true;
            try
            {
                details = this.tbFilename.Text;
                cbType.Value = details.ResType;
                tbGroup.Text = "0x" + details.ResGroup.ToString("X8");
                tbInstance.Text = "0x" + details.ResInstance.ToString("X16");
                tbName.Text = details.ResName;
                btnOK.Enabled = btnOKCanEnable;
            }
            finally { internalchg = false; }
        }
        private void UpdateTGIN()
        {
            if (internalchg) return;
            details = new TGIN();
            details.ResType = cbType.Value;
            details.ResGroup = ResourceGroup;
            details.ResInstance = Instance;
            details.ResName = ResourceName;
        }

        bool btnOKCanEnable { get { return cbType.Valid && (tbGroup.Text.Length * tbInstance.Text.Length > 0); } }

        private void btnOKCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = (sender as Button).DialogResult;
        }

        private void cbType_ValidChanged(object sender, EventArgs e)
        {
            if (internalchg) return;
            btnOK.Enabled = btnOKCanEnable;
            if (btnOK.Enabled) UpdateTGIN();
        }

        private void cbType_ValueChanged(object sender, EventArgs e)
        {
            if (internalchg) return;
            btnOK.Enabled = btnOKCanEnable;
            if (btnOK.Enabled) UpdateTGIN();
        }

        private void tbGroupInstance_TextChanged(object sender, EventArgs e)
        {
            if (internalchg) return;
            TextBox tb = sender as TextBox;
            if (tb.Text.Length > 0)
                try
                {
                    if (tbInstance.Equals(sender))
                        Convert.ToUInt64(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                    else
                        Convert.ToUInt32(tb.Text, tb.Text.StartsWith("0x") ? 16 : 10);
                    btnOK.Enabled = btnOKCanEnable;
                    if (btnOK.Enabled) UpdateTGIN();
                }
                catch { btnOK.Enabled = false; }
            else
                btnOK.Enabled = false;
        }

        private void ckbUseName_CheckedChanged(object sender, EventArgs e)
        {
            tlpName.Enabled = importSettings1.UseName;
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            if (btnOK.Enabled) UpdateTGIN();
        }

        private void btnFNV64_Click(object sender, EventArgs e)
        {
            tbInstance.Text = "0x" + FNV64.GetHash(tbName.Text).ToString("X16");
        }

        private void btnCLIPIID_Click(object sender, EventArgs e)
        {
            tbInstance.Text = "0x" + FNV64CLIP.GetHash(tbName.Text).ToString("X16");
        }

        private void btnFNV32_Click(object sender, EventArgs e)
        {
            tbInstance.Text = "0x" + FNV32.GetHash(tbName.Text).ToString("X16");
        }

        private void tbFilename_DragOver(object sender, DragEventArgs e)
        {
            if ((new List<string>(e.Data.GetFormats())).Contains("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        private void tbFilename_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileDrop = e.Data.GetData("FileDrop") as String[];
            if (fileDrop != null && fileDrop.Length > 0)
                Filename = fileDrop[0];
        }

        #region IEqualityComparer<IResourceKey> Members
        public bool Equals(IResourceKey x, IResourceKey y) { return x.Equals(y); }
        public int GetHashCode(IResourceKey obj) { return obj.GetHashCode(); }
        public override int GetHashCode() { return ResourceType.GetHashCode() ^ ResourceGroup.GetHashCode() ^ Instance.GetHashCode(); }
        #endregion

        #region IEquatable<IResourceKey> Members

        public bool Equals(IResourceKey other) { return CompareTo(other) == 0; }

        #endregion

        #region IComparable<IResourceKey> Members

        public int CompareTo(IResourceKey other)
        {
            int res = ResourceType.CompareTo(other.ResourceType); if (res != 0) return res;
            res = ResourceGroup.CompareTo(other.ResourceGroup); if (res != 0) return res;
            return Instance.CompareTo(other.Instance);
        }

        #endregion

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            TGIBlock item = new TGIBlock(0, null);
            pasteResourceKeyToolStripMenuItem.Enabled = copyResourceKeyToolStripMenuItem.Enabled && TGIBlock.TryParse(Clipboard.GetText(), item);
        }

        private void copyResourceKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(((AResourceKey)details) + "");
        }

        private void btnCopy_Click(object sender, EventArgs e) { copyResourceKeyToolStripMenuItem_Click(sender, e); }

        private void pasteResourceKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TGIBlock item = new TGIBlock(0, null);
            if (!TGIBlock.TryParse(Clipboard.GetText(), item)) return;

            cbType.Value = item.ResourceType;
            tbGroup.Text = "0x" + item.ResourceGroup.ToString("X8");
            tbInstance.Text = "0x" + item.Instance.ToString("X16");
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            pasteResourceKeyToolStripMenuItem_Click(sender, e);
        }

        private void btnHighBit_Click(object sender, EventArgs e)
        {
            const uint uintMask = 1u << 31;
            const ulong ulongMask = 1ul << 63;
            const string prefix = "0x";
            uint uintResult;
            ulong ulongResult;
            if (tbGroup.Text.StartsWith((prefix)))
            {
                if(uint.TryParse(tbGroup.Text.Substring(2), NumberStyles.HexNumber, null, out uintResult))
                {
                    uintResult |= uintMask;
                    tbGroup.Text = prefix + uintResult.ToString("X8");
                }
            }
            else
            {
                if (uint.TryParse(tbGroup.Text, NumberStyles.Number, null, out uintResult))
                {
                    uintResult |= uintMask;
                    tbGroup.Text = prefix + uintResult.ToString("X8");
                }
            }

            if (tbInstance.Text.StartsWith((prefix)))
            {
                if (ulong.TryParse(tbInstance.Text.Substring(2), NumberStyles.HexNumber, null, out ulongResult))
                {
                    ulongResult |= ulongMask;
                    tbInstance.Text = prefix + ulongResult.ToString("X16");
                }
            }
            else
            {
                if (ulong.TryParse(tbInstance.Text, NumberStyles.Number, null, out ulongResult))
                {
                    ulongResult |= ulongMask;
                    tbInstance.Text = prefix + ulongResult.ToString("X16");
                }
            }
        }
    }
}
