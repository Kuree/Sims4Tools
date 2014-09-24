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
    public partial class ControlPanel : UserControl
    {
        public ControlPanel()
        {
            InitializeComponent();
        }

        void ControlPanel_LoadSettings()
        {
            Sort = S4PIDemoFE.Properties.Settings.Default.Sort;
            HexOnly = S4PIDemoFE.Properties.Settings.Default.HexOnly;
            switch (S4PIDemoFE.Properties.Settings.Default.AutoChoice)
            {
                case 0: AutoOff = true; break;
                case 1: AutoHex = true; break;
                case 2: AutoPreview = true; break;
            }
            UseNames = S4PIDemoFE.Properties.Settings.Default.UseNames;
            UseTags = S4PIDemoFE.Properties.Settings.Default.UseTags;

            OnSortChanged(this, EventArgs.Empty);
            OnHexOnlyChanged(this, EventArgs.Empty);
            OnAutoChanged(this, EventArgs.Empty);
            OnUseNamesChanged(this, EventArgs.Empty);
            OnUseTagsChanged(this, EventArgs.Empty);
        }

        public void ControlPanel_SaveSettings(object sender, EventArgs e)
        {
            S4PIDemoFE.Properties.Settings.Default.Sort = Sort;
            S4PIDemoFE.Properties.Settings.Default.AutoChoice = (short)(AutoHex ? 1 : AutoPreview ? 2 : 0);
            S4PIDemoFE.Properties.Settings.Default.HexOnly = HexOnly;
            S4PIDemoFE.Properties.Settings.Default.UseNames = UseNames;
            S4PIDemoFE.Properties.Settings.Default.UseTags = UseTags;
        }

        private void ControlPanel_Load(object sender, EventArgs e)
        {
            ControlPanel_LoadSettings();
        }

        #region Sort checkbox
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("The state of the Sort checkbox")]
        public bool Sort { get { return ckbSortable.Checked; } set { ckbSortable.Checked = value; } }

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Occurs when the Sort checkbox changes")]
        public event EventHandler SortChanged;
        protected virtual void OnSortChanged(object sender, EventArgs e) { if (SortChanged != null) SortChanged(sender, e); }
        private void ckbSortable_CheckedChanged(object sender, EventArgs e) { OnSortChanged(sender, e); }
        #endregion

        #region Hex button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Hex button")]
        public bool HexEnabled { get { return btnHex.Enabled; } set { btnHex.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Hex button is clicked")]
        public event EventHandler HexClick;
        protected virtual void OnHexClick(object sender, EventArgs e) { if (HexClick != null) HexClick(sender, e); }
        private void btnHex_Click(object sender, EventArgs e) { OnHexClick(sender, e); }
        #endregion

        #region Auto radio buttons

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Occurs when the Auto radio button selection changes")]
        public event EventHandler AutoChanged;
        protected virtual void OnAutoChanged(object sender, EventArgs e) { if (AutoChanged != null) AutoChanged(sender, e); }
        private void rb1Off_CheckedChanged(object sender, EventArgs e) { OnAutoChanged(sender, e); }
        private void rb1Hex_CheckedChanged(object sender, EventArgs e) { OnAutoChanged(sender, e); }
        private void rb1Value_CheckedChanged(object sender, EventArgs e) { OnAutoChanged(sender, e); }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("The state of the Auto Off radio button")]
        public bool AutoOff { get { return rb1Off.Checked; } set { rb1Off.Checked = value; } }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("The state of the Auto Hex radio button")]
        public bool AutoHex { get { return rb1Hex.Checked; } set { rb1Hex.Checked = value; } }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("The state of the Auto Preview radio button")]
        public bool AutoPreview { get { return rb1Preview.Checked; } set { rb1Preview.Checked = value; } }
        #endregion

        #region HexOnly checkbox
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("The state of the HexOnly checkbox")]
        public bool HexOnly { get { return ckbNoUnWrap.Checked; } set { ckbNoUnWrap.Checked = value; } }

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Occurs when the HexOnly checkbox changes")]
        public event EventHandler HexOnlyChanged;
        protected virtual void OnHexOnlyChanged(object sender, EventArgs e) { if (HexOnlyChanged != null) HexOnlyChanged(sender, e); }
        private void ckbNoUnWrap_CheckedChanged(object sender, EventArgs e) { OnHexOnlyChanged(sender, e); }
        #endregion

        #region Value button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Value button")]
        public bool ValueEnabled { get { return btnPreview.Enabled; } set { btnPreview.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Value button is clicked")]
        public event EventHandler ValueClick;
        protected virtual void OnValueClick(object sender, EventArgs e) { if (ValueClick != null) ValueClick(sender, e); }
        private void btnValue_Click(object sender, EventArgs e) { OnValueClick(sender, e); }
        #endregion

        #region Grid button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Grid button")]
        public bool GridEnabled { get { return btnGrid.Enabled; } set { btnGrid.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Grid button is clicked")]
        public event EventHandler GridClick;
        protected virtual void OnGridClick(object sender, EventArgs e) { if (GridClick != null) GridClick(sender, e); }
        private void btnGrid_Click(object sender, EventArgs e) { OnGridClick(sender, e); }
        #endregion

        #region UseNames checkbox
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("The state of the UseNames checkbox")]
        public bool UseNames { get { return ckbUseNames.Checked; } set { ckbUseNames.Checked = value; } }

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Occurs when the UseNames checkbox changes")]
        public event EventHandler UseNamesChanged;
        protected virtual void OnUseNamesChanged(object sender, EventArgs e) { if (UseNamesChanged != null) UseNamesChanged(sender, e); }
        private void ckbUseNames_CheckedChanged(object sender, EventArgs e) { OnUseNamesChanged(sender, e); }
        #endregion

        #region UseTags checkbox
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("The state of the UseTags checkbox")]
        public bool UseTags { get { return ckbUseTags.Checked; } set { ckbUseTags.Checked = value; } }

        [Browsable(true)]
        [Category("Property Changed")]
        [Description("Occurs when the UseNames checkbox changes")]
        public event EventHandler UseTagsChanged;
        protected virtual void OnUseTagsChanged(object sender, EventArgs e) { if (UseTagsChanged != null) UseTagsChanged(sender, e); }
        private void ckbUseTags_CheckedChanged(object sender, EventArgs e) { OnUseTagsChanged(sender, e); }
        #endregion

        #region Helper1 button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Helper1 button")]
        public bool Helper1Enabled { get { return btnHelper1.Enabled; } set { btnHelper1.Enabled = value; } }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("Helper1")]
        [Description("Helper1 button label")]
        public string Helper1Label { get { return btnHelper1.Text; } set { btnHelper1.Text = value; } }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Helper1 button tooltip")]
        public string Helper1Tip { get { return toolTip1.GetToolTip(btnHelper1); } set { toolTip1.SetToolTip(btnHelper1, value); } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Helper1 button is clicked")]
        public event EventHandler Helper1Click;
        protected virtual void OnHelper1Click(object sender, EventArgs e) { if (Helper1Click != null) Helper1Click(sender, e); }
        private void btnHelper1_Click(object sender, EventArgs e) { OnHelper1Click(sender, e); }
        #endregion

        #region Helper2 button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Helper2 button")]
        public bool Helper2Enabled { get { return btnHelper2.Enabled; } set { btnHelper2.Enabled = value; } }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("Helper2")]
        [Description("Helper2 button label")]
        public string Helper2Label { get { return btnHelper2.Text; } set { btnHelper2.Text = value; } }

        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Helper2 button tooltip")]
        public string Helper2Tip { get { return toolTip1.GetToolTip(btnHelper2); } set { toolTip1.SetToolTip(btnHelper2, value); } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Helper2 button is clicked")]
        public event EventHandler Helper2Click;
        protected virtual void OnHelper2Click(object sender, EventArgs e) { if (Helper2Click != null) Helper2Click(sender, e); }
        private void btnHelper2_Click(object sender, EventArgs e) { OnHelper2Click(sender, e); }
        #endregion

        #region HexEdit button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the HexEdit button")]
        public bool HexEditEnabled { get { return btnHexEdit.Enabled; } set { btnHexEdit.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the HexEdit button is clicked")]
        public event EventHandler HexEditClick;
        protected virtual void OnHexEditClick(object sender, EventArgs e) { if (HexEditClick != null) HexEditClick(sender, e); }
        private void btnHexEdit_Click(object sender, EventArgs e) { OnHexEditClick(sender, e); }
        #endregion

        #region Commit button
        [Browsable(true)]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("The state of the Commit button")]
        public bool CommitEnabled { get { return btnCommit.Enabled; } set { btnCommit.Enabled = value; } }

        [Browsable(true)]
        [Category("Action")]
        [Description("Occurs when the Commit button is clicked")]
        public event EventHandler CommitClick;
        protected virtual void OnCommitClick(object sender, EventArgs e) { if (CommitClick != null) CommitClick(sender, e); }
        private void btnCommit_Click(object sender, EventArgs e) { OnCommitClick(sender, e); }
        #endregion
    }
}