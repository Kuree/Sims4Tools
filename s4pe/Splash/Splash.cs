using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace S4PIDemoFE
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            Bitmap icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon"))).ToBitmap();
            pictureBox1.Image = new Bitmap(icon, new Size(64, 64));
        }

        public Splash(string value)
            : this()
        {
            label.Text = value;
        }

        public string Message { get { return label.Text; } set { label.Text = value; } }
    }
}
