/***************************************************************************
 *  Copyright (C) 2013 by Peter L Jones                                    *
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
using ScriptResource;

namespace S3SA_DLL_ExpImp
{
    public partial class View : Form, s3pi.Helpers.IRunHelper
    {
        ScriptResource.ScriptResource s3sa;

        public View()
        {
            InitializeComponent();
            lbAssyName.Text = "";

            tbViewer.Text = Properties.Settings.Default.AssyViewerCmd;
            tbArgs.Text = Properties.Settings.Default.AssyViewerArgs;
            btnOK.Enabled = tbViewer.Text.Length > 0 && File.Exists(tbViewer.Text);
        }

        public View(Stream s)
            : this()
        {
            s3sa = new ScriptResource.ScriptResource(0, s);
            lbAssyName.Text = Program.getAssemblyName(s3sa);
        }

        public byte[] Result { get { return null; } }

        private void View_Load(object sender, EventArgs e)
        {
            if (btnOK.Enabled && Program.ViewGo)
            {
                doViewer(false);
                this.Close();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofdAssyViewer.ShowDialog();

            if (dr == DialogResult.OK)
            {
                tbViewer.Text = ofdAssyViewer.FileName;
                btnOK.Enabled = tbViewer.Text.Length > 0 && File.Exists(tbViewer.Text);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Environment.ExitCode = 0;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            doViewer();
            this.Close();
        }

        private void doViewer(bool fromForm = true)
        {
            string tmpFilename = Path.Combine(Path.GetTempPath(), lbAssyName.Text);
            using (FileStream fs = new FileStream(tmpFilename, FileMode.Create, FileAccess.Write))
            {
                byte[] data = new byte[s3sa.Assembly.BaseStream.Length];
                s3sa.Assembly.BaseStream.Read(data, 0, data.Length);
                fs.Write(data, 0, data.Length);
            }

            Properties.Settings.Default.AssyViewerCmd = tbViewer.Text;
            Properties.Settings.Default.AssyViewerArgs = tbArgs.Text;

            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = Properties.Settings.Default.AssyViewerCmd;

            if (tbArgs.Text.Contains("{}"))
            {
                p.StartInfo.Arguments = tbArgs.Text.Replace("{}", tmpFilename);
            }
            else if (tbArgs.Text.Length > 0)
            {
                p.StartInfo.Arguments = String.Format("{0} {1}", tbArgs.Text, tmpFilename);
            }
            else
            {
                p.StartInfo.Arguments = tmpFilename;
            }

            p.StartInfo.UseShellExecute = false;

            if (fromForm)
                CopyableMessageBox.Show(this.GetType().Assembly.FullName + "\n"
                    + String.Format("Starting:\n{0}\n{1}", p.StartInfo.FileName, p.StartInfo.Arguments),
                        "Launch", CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Information);

            try
            {
                p.Start();

                Properties.Settings.Default.Save();
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex,
                    this.GetType().Assembly.FullName + "\n" + String.Format("Application failed to start:\n{0}\n{1}", p.StartInfo.FileName, p.StartInfo.Arguments),
                    "Launch failed");
                Environment.ExitCode = 1;
            }
        }
    }
}
