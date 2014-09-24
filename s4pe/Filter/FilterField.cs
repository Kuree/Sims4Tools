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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using s4pi.Interfaces;

namespace S4PIDemoFE.Filter
{
    public partial class FilterField : UserControl
    {
        Regex rxFilter = null;
        Regex rxValue = null;
        bool exact = true;

        public FilterField(bool exact = true)
        {
            InitializeComponent();
            this.exact = exact;
            if (!exact) tbApplied.TextAlign = tbEntry.TextAlign = HorizontalAlignment.Left;
        }

        [Category("Appearance")]
        [Description("Indicate whether the filter field checkbox is checked")]
        public bool Checked { get { return ckbFilter.Checked; } set { ckbFilter.Checked = value; } }

        [Category("Appearance")]
        [Description("Value to filter for")]
        public Regex Filter
        {
            get { return rxFilter; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                rxFilter = value;

                tbApplied.SuspendLayout();
                tbApplied.Text = (rxFilter == null) ? "*" : rxFilter.ToString().TrimStart('^').TrimEnd('$');
                while (tbApplied.Text.StartsWith(".*")) tbApplied.Text = tbApplied.Text.Substring(2);
                while (tbApplied.Text.EndsWith(".*")) tbApplied.Text = tbApplied.Text.Substring(0, tbApplied.Text.Length - 2);
                tbApplied.ResumeLayout();

                if (tbApplied.Text == ".*") tbApplied.Text = "*";
            }
        }

        [Category("Appearance")]
        [Description("Title for the field")]
        public string Title { get { return lbField.Text; } set { lbField.Text = value; } }

        [Category("Appearance")]
        [Description("Value being entered")]
        public Regex Value
        {
            get { return rxValue; }
            set
            {
                if (value == null) throw new ArgumentNullException();
                rxValue = value;

                tbEntry.SuspendLayout();
                tbEntry.Text = (rxValue == null) ? "" : rxValue.ToString().TrimStart('^').TrimEnd('$');
                while (tbEntry.Text.StartsWith(".*")) tbEntry.Text = tbEntry.Text.Substring(2);
                while (tbEntry.Text.EndsWith(".*")) tbEntry.Text = tbEntry.Text.Substring(0, tbEntry.Text.Length - 2);
                tbEntry.ResumeLayout();
            }
        }

        [Category("Behavior")]
        [Description("Whether filter field is an exact (rather than sub-string) match")]
        public bool Exact { get { return exact; } set { exact = value; } }

        /// <summary>
        /// Set Value from Filter
        /// </summary>
        [Description("Set Value from Filter")]
        public void Revise() { Value = Filter; }

        /// <summary>
        /// Set Filter from Value taking Checked into account
        /// </summary>
        [Description("Set Filter from Value taking Checked into account")]
        public void Set() { Filter = Checked ? Value : new Regex(".*"); }



        private void tbEntry_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tbEntry.Text.Length == 0) { rxValue = new Regex(""); return; }
                Value = new Regex("^" + (exact ? "" : ".*") + tbEntry.Text.TrimStart('^').TrimEnd('$') + (exact ? "" : ".*") + "$", RegexOptions.IgnoreCase);
            }
            catch (System.ArgumentException) { Value = rxValue; tbEntry.SelectAll(); }
        }
    }
}
