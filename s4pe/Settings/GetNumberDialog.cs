using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace S4PIDemoFE
{
    public partial class GetNumberDialog : Form
    {
        public GetNumberDialog()
        {
            InitializeComponent();
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
        }

        public GetNumberDialog(string message, decimal min, decimal max) : this(message, "", min, max, min) { }
        public GetNumberDialog(string message, string caption, decimal min, decimal max, decimal value) : this() { Message = message; Caption = caption; Min = min; Max = max; Value = value; }

        public string Message
        {
            get { return label1.Text; }
            set { label1.Text = value; }
        }

        public string Caption
        {
            get { return this.Text; }
            set { this.Text = value; }
        }

        public decimal Min
        {
            get { return numericUpDown1.Minimum; }
            set { numericUpDown1.Minimum = value; }
        }

        public decimal Max
        {
            get { return numericUpDown1.Maximum; }
            set { numericUpDown1.Maximum = value; }
        }

        public decimal Value
        {
            get { return numericUpDown1.Value; }
            set { numericUpDown1.Value = value; }
        }
    }
}
