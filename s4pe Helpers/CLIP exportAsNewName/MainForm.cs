/***************************************************************************
 *  Copyright (C) 2011 by Peter L Jones                                    *
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
using System.Security.Cryptography;
using System.Windows.Forms;
using s3pi.Helpers;

namespace CLIPexportAsNewName
{
    public partial class MainForm : Form, IRunHelper
    {
        public MainForm()
        {
            InitializeComponent();
            tbClipName.Text = Program.CurrentName;
            tbClipName_TextChanged(null, null);
        }

        byte[] data = null;
        public MainForm(Stream s) : this() { data = new BinaryReader(s).ReadBytes((int)s.Length); }

        public byte[] Result { get { return null; } }

        private void tbClipName_TextChanged(object sender, EventArgs e) { tbIID.Text = string.Format("0x{0:X16}", FNV64CLIP.GetHash(tbClipName.Text)); }

        private void btnCancel_Click(object sender, EventArgs e) { this.Close(); }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                folderBrowserDialog1.SelectedPath = CLIPexportAsNewName.Properties.Settings.Default.LastExportFolder;
                DialogResult dr = folderBrowserDialog1.ShowDialog();
                if (dr != System.Windows.Forms.DialogResult.OK) return;

                CLIPexportAsNewName.Properties.Settings.Default.LastExportFolder = folderBrowserDialog1.SelectedPath;
                CLIPexportAsNewName.Properties.Settings.Default.Save();

                string filename = Path.Combine(folderBrowserDialog1.SelectedPath, String.Format("S3_{0:X8}_{1:X8}_{2:X16}_{3}%%+CLIP.animation",
                    0x6B20C4F3, Program.CurrentGroup, FNV64CLIP.GetHash(tbClipName.Text), tbClipName.Text));

                if (File.Exists(filename) && CopyableMessageBox.Show(String.Format("File '{0}' exists.\n\nReplace?", Path.GetFileName(filename)), "File exists",
                         CopyableMessageBoxButtons.YesNo, CopyableMessageBoxIcon.Question, 1, 1) != 0) return;

                using (BinaryWriter w = new BinaryWriter(new FileStream(filename, FileMode.Create)))
                {
                    w.Write(data);
                    w.Close();
                }

                Environment.ExitCode = 0;
            }
            finally { this.Close(); }
        }

    }
}
