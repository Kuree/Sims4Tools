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
        public Import()
        {
            InitializeComponent();
        }

        public Import(Stream s):this()
        {
            RLEResource r = new RLEResource(1, s);
            this.Result = new byte[r.RawData.Length];
            r.RawData.CopyTo(this.Result, 0);
        }
        
    }
}
