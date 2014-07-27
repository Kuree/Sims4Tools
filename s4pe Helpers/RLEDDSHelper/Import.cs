using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using s4pi.Helpers;
using s4pi.ImageResource;

namespace RLEDDSHelper
{
    public partial class Import : Form, IRunHelper
    {
        public byte[] Result { get; private set; }
        private string filename;
        public Import(Stream s)
        {
            InitializeComponent();
            using (OpenFileDialog open = new OpenFileDialog() { Filter = "DDS File|*.dds" })
            {
                if (open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.filename = open.FileName;
                }
                else
                {
                    this.filename = "";
                }
            }
        }

        private void Import_Shown(object sender, EventArgs e)
        {
            if (this.filename != "")
            {
                using (FileStream fs = new FileStream(this.filename, FileMode.Open))
                {
                    RLEResource r = new RLEResource(1, null);
                    r.ImportToRLE(fs);
                    this.Result = new byte[r.RawData.Length];
                    r.RawData.CopyTo(this.Result, 0);
                    Environment.ExitCode = 0;
                }
            }
            
            this.Close();
        }
        
    }
}
