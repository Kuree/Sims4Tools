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
