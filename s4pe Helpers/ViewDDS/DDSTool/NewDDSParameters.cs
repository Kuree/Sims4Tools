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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace s3pe.DDSTool
{
    public partial class NewDDSParameters : Form
    {
        static string[] DXTitems = new string[] { "1", "3", "5", };
        static string[] DDSitems = new string[] { "0", "1", "4", "8", };

        public NewDDSParameters()
        {
            InitializeComponent();
            cbDepth.Items.Clear();
            cbDepth.Items.AddRange(DDSitems);
            cbDepth.SelectedIndex = cbDepth.Items.Count - 1;
        }

        public NewDDSParameters(int width, int height, bool useDXT, int alphaDepth = -1, bool disableColour = false, bool useLuminance = false) : this()
        {
            nudWidth.Value = width;
            nudHeight.Value = height;

            rb1DXT.Checked = rb1RGB.Checked = rb1Luminance.Checked = false;
            if (useDXT)
            {
                rb1DXT.Checked = true;
            }
            else if (!useLuminance)
            {
                rb1RGB.Checked = true;
            }
            else
            {
                rb1Luminance.Checked = true;
            }

            if (!useLuminance && alphaDepth != -1)
                cbDepth.SelectedIndex = (useDXT ? DXTitems : DDSitems).ToList().IndexOf("" + alphaDepth);

            if (disableColour)
                nudRed.Enabled = nudGreen.Enabled = nudBlue.Enabled = nudAlpha.Enabled = false;
        }

        private int GetAlphaDepth()
        {
            switch (cbDepth.SelectedIndex)
            {
                case 0: return 0;
                case 1: return 1;
                case 2: return 4;
                case 3: return 8;
                default: return -1;
            }
        }

        Result _result = new Result() { DialogResult = DialogResult.Cancel };
        public Result Value { get { return _result; } }

        public struct Result
        {
            public byte Red;
            public byte Green;
            public byte Blue;
            public byte Alpha;
            public int Width;
            public int Height;
            public bool UseDXT;
            public bool UseLuminance;
            public int AlphaDepth;
            public DialogResult DialogResult;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            _result = new Result
            {
                Red = (byte)nudRed.Value,
                Green = (byte)nudGreen.Value,
                Blue = (byte)nudBlue.Value,
                Alpha = (byte)nudAlpha.Value,
                Width = (int)nudWidth.Value,
                Height = (int)nudHeight.Value,
                UseDXT = rb1DXT.Checked,
                UseLuminance = rb1Luminance.Checked,
                AlphaDepth = rb1Luminance.Checked ? 8 : int.Parse((string)cbDepth.SelectedItem),
                DialogResult = this.DialogResult,
            };
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void rb1_CheckedChanged(object sender, EventArgs e)
        {
            cbDepth.SelectedIndex = -1;
            cbDepth.Items.Clear();
            if (rb1DXT.Checked)
                cbDepth.Items.AddRange(DXTitems);
            else if (rb1RGB.Checked)
                cbDepth.Items.AddRange(DDSitems);
            cbDepth.SelectedIndex = cbDepth.Items.Count - 1;
            cbDepth.Enabled = !rb1Luminance.Checked;
        }
    }
}
