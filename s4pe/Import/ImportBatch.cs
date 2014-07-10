using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace S4PIDemoFE
{
    public partial class ImportBatch : Form
    {
        public enum Mode
        {
            file = 0,
            package,
            paste,
            replaceFrom,
        }
        private string[] batch = null;
        private Mode mode = Mode.file;
        public ImportBatch()
        {
            InitializeComponent();
            this.Width = (int)(Application.OpenForms[0].Width * 0.8);
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
        }

        private ImportBatch(Mode mode) : this()
        {
            this.mode = mode;
            this.AllowDrop = this.mode == Mode.file;
            if (mode == Mode.replaceFrom)
            {
                importSettings1[S4PIDemoFE.Import.ImportSettings.ImportSettingsControl.Replace] = false;
                importSettings1.Replace = true;
                importSettings1[S4PIDemoFE.Import.ImportSettings.ImportSettingsControl.UseName] = false;
                importSettings1.UseName = false;
                importSettings1[S4PIDemoFE.Import.ImportSettings.ImportSettingsControl.AllowRename] = false;
                importSettings1.AllowRename = false;
            }
        }

        public ImportBatch(string[] batch, Mode mode) : this(mode) { addDrop(batch); }
        public ImportBatch(string[] fileDrop) : this(fileDrop, Mode.file) { }
        public ImportBatch(IList<MainForm.myDataFormat> ldata) : this(Mode.paste)
        {
            string[] fileDrop = new string[ldata.Count];
            for (int i = 0; i < ldata.Count; i++) fileDrop[i] = ldata[i].tgin;
            addDrop(fileDrop);
        }

        public string[] Batch { get { return (string[])batch.Clone(); } }

        public bool Replace { get { return importSettings1.Replace; } }

        public bool Compress { get { return importSettings1.Compress; } }

        public bool UseNames { get { return importSettings1.UseName; } set { importSettings1.UseName = value; } }

        public bool Rename { get { return importSettings1.AllowRename; } }

        void addDrop(string[] fileDrop)
        {
            batch = (string[])fileDrop.Clone();
            lbFiles.Items.Clear();
            lbFiles.Items.AddRange(fileDrop);
        }

        private void ImportBatch_DragOver(object sender, DragEventArgs e)
        {
            if ((new List<string>(e.Data.GetFormats())).Contains("FileDrop"))
                e.Effect = DragDropEffects.Copy;
        }

        private void ImportBatch_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileDrop = e.Data.GetData("FileDrop") as String[];
            if (fileDrop == null || fileDrop.Length == 0) return;
            addDrop(fileDrop);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
