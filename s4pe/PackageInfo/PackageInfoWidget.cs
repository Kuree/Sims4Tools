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

namespace S4PIDemoFE.PackageInfo
{
    public partial class PackageInfoWidget : UserControl
    {
        public PackageInfoWidget()
        {
            InitializeComponent();
        }

        IList<string> fields = null;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Specifies the list of fields to display")]
        public IList<string> Fields
        {
            get { return fields; }
            set
            {
                if (fields == value) return;
                fields = value;
                SetFields();
            }
        }

        Dictionary<string, Label> values = null;
        void SetFields()
        {
            values = new Dictionary<string, Label>();
            this.tableLayoutPanel1.RowCount = fields.Count;
            for (int i = 0; i < fields.Count; i++)
            {
                Label l = new Label();
                l.Text = fields[i];
                l.TextAlign = ContentAlignment.MiddleRight;
                l.AutoSize = true;
                tableLayoutPanel1.Controls.Add(l, 0, i);
                l = new Label();
                l.TextAlign = ContentAlignment.MiddleLeft;
                l.AutoSize = true;
                tableLayoutPanel1.Controls.Add(l, 1, i);
                values.Add(fields[i], l);
            }
            tableLayoutPanel1.Update();
        }

        s4pi.Interfaces.IPackage package = null;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Specifies the package to display info for")]
        public s4pi.Interfaces.IPackage Package
        {
            get { return package; }
            set
            {
                if (package == value) return;
                package = value;
                foreach (Label l in values.Values) l.Text = "";
                if (package == null) return;
                for (int i = 0; i < fields.Count; i++)
                    values[fields[i]].Text = package[fields[i]];
            }
        }
    }
}
