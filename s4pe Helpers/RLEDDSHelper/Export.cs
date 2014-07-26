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
using s4pi.ImageResource;

namespace RLEDDSHelper
{
    public partial class Export : Form
    {
        public Export()
        {
            InitializeComponent();
        }

        public Export(Stream s):this()
        {
            using (SaveFileDialog save = new SaveFileDialog() { Filter="DDS DXT5|*.dds"})
            {
                if(save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using(FileStream fs = new FileStream(save.FileName, FileMode.Create))
                    {
                        RLEResource r = new RLEResource(1, s);
                        r.ToDDS().CopyTo(fs);
                    }
                }
            }
        }
    }
}
