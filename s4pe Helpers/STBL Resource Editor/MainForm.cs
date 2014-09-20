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
using StblResource;


namespace s3pi_STBL_Resource_Editor
{
    public partial class MainForm : Form, s4pi.Helpers.IRunHelper
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(Stream ms)
            : this()
        {
            try
            {
                Application.UseWaitCursor = true;
                loadStbl(ms);
            }
            finally { Application.UseWaitCursor = false; }

            if (lbStrings.Items.Count > 0)
                lbStrings.SelectedIndices.Add(0);
        }

        byte[] result = null;
        public byte[] Result { get { return result; } }

        StblResource.StblResource stbl;
        List<uint> stblKeys;
        IDictionary<uint, string> map;
        void loadStbl(Stream data)
        {
            stbl = new StblResource.StblResource(0, data);
            map = (IDictionary<uint, string>)stbl;
            stblKeys = new List<uint>(map.Keys);
            foreach (uint key in map.Keys)
                lbStrings.Items.Add("0x" + key.ToString("X8") + ": " + partValue(map[key]));
        }
        string partValue(string value)
        {
            string res = "";

            foreach (char c in value)
            {
                if (Char.IsLetterOrDigit(c) || Char.IsPunctuation(c) || c == ' ') res += c;
                //if (res.Length > 20) break;
            }

            return res;
        }

        void saveStbl()
        {
            result = (byte[])stbl.AsBytes;
        }

        int currentIndex = -1;
        bool internalchg = false;
        private void lbStrings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (internalchg) return;

            if (currentIndex >= 0)
            {
                map[stblKeys[currentIndex]] = rtbValue.Text;
            }
            rtbValue.Enabled = lbStrings.SelectedIndices.Count > 0;
            if (lbStrings.SelectedIndices.Count == 0)
            {
                rtbValue.Text = "";
                currentIndex = -1;
                btnDelete.Enabled = false;
                btnChange.Enabled = false;
            }
            else
            {
                if (currentIndex >= 0)
                {
                    internalchg = true;
                    lbStrings.Items[currentIndex] = "0x" + stblKeys[currentIndex].ToString("X8") + ": " + partValue(rtbValue.Text);
                    internalchg = false;
                }

                currentIndex = lbStrings.SelectedIndex;
                rtbValue.Text = map[stblKeys[currentIndex]];
                tbGUID.Text = "0x" + stblKeys[currentIndex].ToString("X8");
                btnDelete.Enabled = true;
                btnChange.Enabled = tbGUID.Text.Length > 0;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            lbStrings.SelectedIndices.Clear();
            saveStbl();
            Environment.ExitCode = 0;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tbGUID_TextChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = tbGUID.Text.Length > 0;
            btnChange.Enabled = lbStrings.SelectedIndices.Count > 0 && tbGUID.Text.Length > 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            uint newGUID = getGUID();
            if (map.ContainsKey(newGUID)) { tbGUID.Focus(); tbGUID.SelectAll(); return; }

            lbStrings.Items.Add("0x" + newGUID.ToString("X8"));
            map.Add(newGUID, "");
            stblKeys.Add(newGUID);

            lbStrings.SelectedIndex = lbStrings.Items.Count - 1;
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            uint newGUID = getGUID();
            uint oldGUID = stblKeys[currentIndex];
            if (newGUID == oldGUID || map.ContainsKey(newGUID)) { tbGUID.Focus(); tbGUID.SelectAll(); return; }

            int i = currentIndex;
            lbStrings.SelectedIndices.Clear();
            string value = map[oldGUID];

            lbStrings.Items.RemoveAt(i);
            map.Remove(oldGUID);
            stblKeys.Remove(oldGUID);

            lbStrings.Items.Insert(i, "0x" + newGUID.ToString("X8") + ": " + partValue(value));
            map.Add(newGUID, value);
            stblKeys.Insert(i, newGUID);

            lbStrings.SelectedIndex = i;
        }

        uint getGUID()
        {
            uint newGUID;
            bool res = false;
            if (tbGUID.Text.StartsWith("0x"))
                res = uint.TryParse(tbGUID.Text.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out newGUID);
            else
                res = uint.TryParse(tbGUID.Text, System.Globalization.NumberStyles.Integer, null, out newGUID);

            if (!res)
            {
                newGUID = System.Security.Cryptography.FNV32.GetHash(tbGUID.Text);
            }
            return newGUID;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            uint oldGUID = stblKeys[currentIndex];
            if (map.ContainsKey(oldGUID))
            {
                int i = currentIndex;
                lbStrings.SelectedIndices.Clear();

                lbStrings.Items.RemoveAt(i);
                map.Remove(oldGUID);
                stblKeys.Remove(oldGUID);

                if (lbStrings.Items.Count == 0)
                    return;
                if (i >= lbStrings.Items.Count)
                    lbStrings.SelectedIndex = lbStrings.Items.Count - 1;
                else
                    lbStrings.SelectedIndex = i;
            }
        }
    }
}
