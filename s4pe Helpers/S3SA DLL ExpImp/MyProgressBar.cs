/***************************************************************************
 *  Copyright (C) 2011 by Peter L Jones                                    *
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

namespace S3SA_DLL_ExpImp
{
    public class MyProgressBar
    {
        Label l;
        ProgressBar pb;
        DateTime wait;
        public MyProgressBar(Label l, ProgressBar pb) { this.l = l; this.pb = pb; }

        public void Init(string label, int max) { l.Text = label; pb.Value = 0; pb.Maximum = max; wait = DateTime.UtcNow.AddSeconds(0.1); Application.DoEvents(); }
        public int Value { get { return pb.Value; } set { if (wait < DateTime.UtcNow) { pb.Value = value; wait = DateTime.UtcNow.AddSeconds(0.1); Application.DoEvents(); } } }
        public void Done() { pb.Value = pb.Maximum; l.Text = ""; }
    }
}
