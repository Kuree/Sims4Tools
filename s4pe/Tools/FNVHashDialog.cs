/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
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
            tbFVV24.Text = "0x" + FNV24.GetHash(tbInput.Text).ToString("X6");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
