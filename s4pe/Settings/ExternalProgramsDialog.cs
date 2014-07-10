using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace S4PIDemoFE.Settings
{
    public partial class ExternalProgramsDialog : Form
    {
        public ExternalProgramsDialog()
        {
            InitializeComponent();
            InitaliseHelpers();
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
        }

        struct HelperControls
        {
            public CheckBox cb;
            public Label lb;
            public Button btn;
            public HelperControls(string name, ref int i)
            {
                btn = new Button();
                btn.Anchor = AnchorStyles.None;
                btn.AutoSize = true;
                btn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                btn.Margin = Padding.Empty;
                btn.Name = "btn" + name;
                btn.TabIndex = i++;
                btn.Text = "info";

                cb = new CheckBox();
                //cb.Anchor = AnchorStyles.Right;
                cb.Anchor = AnchorStyles.None;
                cb.AutoSize = true;
                cb.Margin = Padding.Empty;
                cb.Name = "cb" + name;
                cb.TabIndex = i++;
                cb.Text = "";

                lb = new Label();
                lb.Anchor = AnchorStyles.Left;
                lb.AutoSize = true;
                lb.Margin = Padding.Empty;
                lb.TabIndex = i++;
                lb.Name = "lb" + name;
                lb.Text = name;
                lb.Click += new EventHandler(lb_Click);
            }

            void lb_Click(object sender, EventArgs e) { cb.Checked = !cb.Checked; }
        }

        List<HelperControls> lhc = new List<HelperControls>();
        void InitaliseHelpers()
        {
            s4pi.Helpers.HelperManager.Reload();
            int tabIndex = 4;
            foreach (var helper in s4pi.Helpers.HelperManager.Helpers)
            {
                HelperControls hc = new HelperControls(helper.id, ref tabIndex);
                lhc.Add(hc);
                hc.btn.Click += new EventHandler(hc_btn_Click);

                int h = tlpHelpers.Height;

                tlpHelpers.RowCount++;
                tlpHelpers.RowStyles.Insert(tlpHelpers.RowCount - 2, new RowStyle(SizeType.AutoSize));
                tlpHelpers.Controls.Add(hc.btn, 0, tlpHelpers.RowCount - 2);
                tlpHelpers.Controls.Add(hc.cb, 1, tlpHelpers.RowCount - 2);
                tlpHelpers.Controls.Add(hc.lb, 2, tlpHelpers.RowCount - 2);

                this.Height = this.Height - h + tlpHelpers.Height;
            }
        }

        public string[] DisabledHelpers
        {
            get
            {
                List<string> res = new List<string>();
                foreach (var hc in lhc)
                    if (hc.cb.Checked)
                        res.Add(hc.cb.Name.Substring(2));
                return res.ToArray();
            }
            set {
                foreach (var hc in lhc) hc.cb.Checked = false;
                foreach (string id in value)
                    foreach (var hc in lhc)
                        if (hc.cb.Name.Equals("cb" + id))
                            hc.cb.Checked = true;
            }
        }

        void hc_btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string id = btn.Name.Substring(3);
            foreach (var helper in s4pi.Helpers.HelperManager.Helpers)
            {
                if (helper.id != id) continue;
                string s = "";
                s += "File: Helpers\\" + helper.id + ".helper";
                s += "\nButton: " + helper.label;
                s += "\nDescription: " + helper.desc;
                CopyableMessageBox.Show(s);
                break;
            }
        }


        public bool HasUserHexEditor { get { return ckbUserHexEditor.Checked; } set { ckbUserHexEditor.Checked = value; } }

        public string UserHexEditor { get { return tbUserHexEditor.Text; } set { tbUserHexEditor.Text = value; } }

        public bool HexEditorIgnoreTS { get { return ckbHexEditorTS.Checked; } set { ckbHexEditorTS.Checked = value; } }
        
        public bool HexEditorWantsQuotes { get { return ckbHexQuotes.Checked; } set { ckbHexQuotes.Checked = value; } }

        private void ckbUserHexEditor_CheckedChanged(object sender, EventArgs e) { ckbHexQuotes.Enabled = ckbHexEditorTS.Enabled = btnHexEditorBrowse.Enabled = ckbUserHexEditor.Checked; }

        private void btnHexEditorBrowse_Click(object sender, EventArgs e)
        {
            ofdUserEditor.Title = "Choose your hex editor";
            DialogResult dr = ofdUserEditor.ShowDialog();
            if (dr != DialogResult.OK) return;
            tbUserHexEditor.Text = ofdUserEditor.FileName;
        }


        public bool HasUserTextEditor { get { return ckbUserTextEditor.Checked; } set { ckbUserTextEditor.Checked = value; } }

        public string UserTextEditor { get { return tbUserTextEditor.Text; } set { tbUserTextEditor.Text = value; } }

        public bool TextEditorIgnoreTS { get { return ckbTextEditorTS.Checked; } set { ckbTextEditorTS.Checked = value; } }

        public bool TextEditorWantsQuotes { get { return ckbTextQuotes.Checked; } set { ckbTextQuotes.Checked = value; } }

        private void ckbUserTextEditor_CheckedChanged(object sender, EventArgs e) { ckbTextQuotes.Enabled = ckbTextEditorTS.Enabled = btnTextEditorBrowse.Enabled = ckbUserTextEditor.Checked; }

        private void btnTextEditorBrowse_Click(object sender, EventArgs e)
        {
            ofdUserEditor.Title = "Choose your text editor";
            DialogResult dr = ofdUserEditor.ShowDialog();
            if (dr != DialogResult.OK) return;
            tbUserTextEditor.Text = ofdUserEditor.FileName;
        }
    }
}
