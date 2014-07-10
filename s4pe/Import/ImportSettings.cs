using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace S4PIDemoFE.Import
{
    public partial class ImportSettings : FlowLayoutPanel
    {
        public ImportSettings()
        {
            InitializeComponent();
            ckbRename.Enabled = ckbUseName.Checked;
            rb1Reject.Checked = !rb1Replace.Checked;
        }

        public enum ImportSettingsControl
        {
            Replace,
            Compress,
            UseName,
            AllowRename,
        }
        public bool this[ImportSettingsControl control]
        {
            get { return (new Control[] { rb1Replace, ckbCompress, ckbUseName, ckbRename })[(int)control].Enabled; }
            set
            {
                if (control == ImportSettingsControl.Replace) rb1Replace.Enabled = rb1Reject.Enabled = value;
                else (new Control[] { null, ckbCompress, ckbUseName, ckbRename })[(int)control].Enabled = value;
            }
        }

        #region Properties
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool Replace { get { return rb1Replace.Checked; } set { rb1Reject.Checked = !(rb1Replace.Checked = value); } }
        [Category("Appearance")]
        [DefaultValue(false)]
        public bool Compress { get { return ckbCompress.Checked; } set { ckbCompress.Checked = value; } }
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool UseName { get { return ckbUseName.Checked; } set { ckbUseName.Checked = value; } }
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool AllowRename { get { return ckbRename.Checked; } set { ckbRename.Checked = value; } }
        #endregion

        #region Events
        [Category("Property Changed")]
        [Description("Raised when the Replace property changes")]
        public event EventHandler ReplaceChanged;

        [Category("Property Changed")]
        [Description("Raised when the Compress property changes")]
        public event EventHandler CompressChanged;

        [Category("Property Changed")]
        [Description("Raised when the UseName property changes")]
        public event EventHandler UseNameChanged;

        [Category("Property Changed")]
        [Description("Raised when the AllowRename property changes")]
        public event EventHandler AllowRenameChanged;
        #endregion

        protected virtual void OnReplaceChanged(object sender, EventArgs e) { if (ReplaceChanged != null) ReplaceChanged(sender, e); }
        protected virtual void OnCompressChanged(object sender, EventArgs e) { if (CompressChanged != null) CompressChanged(sender, e); }
        protected virtual void OnUseNameChanged(object sender, EventArgs e) { if (UseNameChanged != null) UseNameChanged(sender, e); }
        protected virtual void OnAllowRenameChanged(object sender, EventArgs e) { if (AllowRenameChanged != null) AllowRenameChanged(sender, e); }

        private void rb1_CheckedChanged(object sender, EventArgs e) { OnReplaceChanged(sender, e); }
        private void ckbCompress_CheckedChanged(object sender, EventArgs e) { OnCompressChanged(sender, e); }
        private void ckbUseName_CheckedChanged(object sender, EventArgs e) { ckbRename.Enabled = ckbUseName.Checked; OnUseNameChanged(sender, e); }
        private void ckbRename_CheckedChanged(object sender, EventArgs e) { OnAllowRenameChanged(sender, e); }
    }
}
