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
