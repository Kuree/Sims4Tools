using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace S4PIDemoFE
{
    public partial class ExperimentalDBCWarning : Form
    {
        public ExperimentalDBCWarning()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void llMATY_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Help.ShowHelp(this, llMATY.Text);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = comboBox1.SelectedIndex == 2;
        }
    }
}
