using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace S4PIDemoFE.Tools
{
    public partial class FNVHashDialog : Form
    {
        public FNVHashDialog()
        {
            InitializeComponent();
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            tbFNV32.Text = "0x" + FNV32.GetHash(tbInput.Text).ToString("X8");
            tbFNV64.Text = "0x" + FNV64.GetHash(tbInput.Text).ToString("X16");
            tbCLIPIID.Text = "0x" + FNV64CLIP.GetHash(tbInput.Text).ToString("X16");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
