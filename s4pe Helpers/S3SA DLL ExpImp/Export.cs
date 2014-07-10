/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
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
using System.Reflection;
using System.Windows.Forms;
using s3pi.Interfaces;
using ScriptResource;

namespace S3SA_DLL_ExpImp
{
    public partial class Export : Form, s3pi.Helpers.IRunHelper
    {
        MyProgressBar mpb;
        public Export()
        {
            InitializeComponent();
            mpb = new MyProgressBar(label1, pb);
        }

        ScriptResource.ScriptResource s3sa;
        public Export(Stream s)
            : this()
        {
            s3sa = new ScriptResource.ScriptResource(0, s);
            sfdExport.FileName = Program.getAssemblyName(s3sa);
            Application.DoEvents();
        }


        public byte[] Result { get { return null; } }

        private void Export_Shown(object sender, EventArgs e)
        {
            try
            {
                DialogResult dr = sfdExport.ShowDialog();
                if (dr != DialogResult.OK)
                {
                    return;
                }

                mpb.Init("Export assembly...", (int)s3sa.Assembly.BaseStream.Length);
                byte[] data = new byte[s3sa.Assembly.BaseStream.Length];
                s3sa.Assembly.BaseStream.Read(data, 0, data.Length);

                using (FileStream fs = new FileStream(sfdExport.FileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                }

                mpb.Done();
            }
            finally { this.Close(); }
        }
    }
}
