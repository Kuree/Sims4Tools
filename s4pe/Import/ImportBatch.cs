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
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace S4PIDemoFE
{
    public partial class ImportBatch : Form
    {
        public enum Mode
        {
            file = 0,
            package,
            paste,
            replaceFrom,
        }
        private string[] batch = null;
        private Mode mode = Mode.file;
        public ImportBatch()
        {
            InitializeComponent();
            this.Width = (int)(Application.OpenForms[0].Width * 0.8);
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
        }

        private ImportBatch(Mode mode) : this()
        {
            this.mode = mode;
            this.AllowDrop = this.mode == Mode.file;
            if (mode == Mode.replaceFrom)
            {
                importSettings1[S4PIDemoFE.Import.ImportSettings.ImportSettingsControl.Replace] = false;
                importSettings1.Replace = true;
                importSettings1[S4PIDemoFE.Import.ImportSettings.ImportSettingsControl.UseName] = false;
                importSettings1.UseName = false;
                importSettings1[S4PIDemoFE.Import.ImportSettings.ImportSettingsControl.AllowRename] = false;
                importSettings1.AllowRename = false;
            }
        }

        public ImportBatch(string[] batch, Mode mode) : this(mode) { addDrop(batch); }
        public ImportBatch(string[] fileDrop) : this(fileDrop, Mode.file) { }
        public ImportBatch(IList<MainForm.myDataFormat> ldata) : this(Mode.paste)
        {
            string[] fileDrop = new string[ldata.Count];
            for (int i = 0; i < ldata.Count; i++) fileDrop[i] = ldata[i].tgin;
            addDrop(fileDrop);
        }

        public string[] Batch { get { return (string[])batch.Clone(); } }

        public bool Replace { get { return importSettings1.Replace; } }

        public bool Compress { get { return importSettings1.Compress; } }

        public bool UseNames { get { return importSettings1.UseName; } set { importSettings1.UseName = value; } }

        public bool Rename { get { return importSettings1.AllowRename; } }

        void addDrop(string[] fileDrop)
        {
            batch = (string[])fileDrop.Clone();
            lbFiles.Items.Clear();
            lbFiles.Items.AddRange(fileDrop);
        }

        private void ImportBatch_DragOver(object sender, DragEventArgs e)
        {
            if ((new List<string>(e.Data.GetFormats())).Contains("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        private void ImportBatch_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileDrop = e.Data.GetData("FileDrop") as String[];
            if (fileDrop == null || fileDrop.Length == 0) return;
            addDrop(fileDrop);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
