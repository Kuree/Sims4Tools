using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ViewDDS
{
    public partial class MainForm : Form
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
                ddsPanel1.DDSLoad(ms);
            }
            finally { Application.UseWaitCursor = false; }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void licenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "file:///" + Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "ViewDDS-Licence.htm"));
        }
    }
}
