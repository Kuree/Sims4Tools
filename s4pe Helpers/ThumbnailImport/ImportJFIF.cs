/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
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
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 *																		   *
 * Modified May 2015 by Camille Marinetti								   *
 ***************************************************************************/
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

namespace JFIFImportHelper
{
    public partial class Import : Form, IRunHelper
    {
        public byte[] Result { get; private set; }
        private string filename;
        public Import(Stream s)
        {
            InitializeComponent();
            using (OpenFileDialog open = new OpenFileDialog() { Filter = "PNG File|*.png" })
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
                    ThumbnailResource t = new ThumbnailResource(1, fs);
                    Stream s = t.ToJFIF();
                    s.Position = 0;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        s.CopyTo(ms);
                        this.Result = ms.ToArray();
                        Environment.ExitCode = 0;
                    }
                }
            }
            
            this.Close();
        }
        
    }
}
