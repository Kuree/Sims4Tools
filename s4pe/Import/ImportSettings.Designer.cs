namespace S4PIDemoFE.Import
{
    partial class ImportSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ckbCompress = new System.Windows.Forms.CheckBox();
            this.ckbUseName = new System.Windows.Forms.CheckBox();
            this.ckbRename = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rb1Replace = new System.Windows.Forms.RadioButton();
            this.rb1Reject = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbCompress
            // 
            this.ckbCompress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbCompress.AutoSize = true;
            this.ckbCompress.Checked = true;
            this.ckbCompress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbCompress.Location = new System.Drawing.Point(3, 32);
            this.ckbCompress.Name = "ckbCompress";
            this.ckbCompress.Size = new System.Drawing.Size(72, 17);
            this.ckbCompress.TabIndex = 2;
            this.ckbCompress.Text = "Compress";
            this.ckbCompress.UseVisualStyleBackColor = true;
            this.ckbCompress.CheckedChanged += new System.EventHandler(this.ckbCompress_CheckedChanged);
            // 
            // ckbUseName
            // 
            this.ckbUseName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbUseName.AutoSize = true;
            this.ckbUseName.Checked = true;
            this.ckbUseName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbUseName.Location = new System.Drawing.Point(81, 32);
            this.ckbUseName.Name = "ckbUseName";
            this.ckbUseName.Size = new System.Drawing.Size(118, 17);
            this.ckbUseName.TabIndex = 3;
            this.ckbUseName.Text = "Use resource name";
            this.ckbUseName.UseVisualStyleBackColor = true;
            this.ckbUseName.CheckedChanged += new System.EventHandler(this.ckbUseName_CheckedChanged);
            // 
            // ckbRename
            // 
            this.ckbRename.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbRename.AutoSize = true;
            this.ckbRename.Location = new System.Drawing.Point(3, 55);
            this.ckbRename.Name = "ckbRename";
            this.ckbRename.Size = new System.Drawing.Size(112, 17);
            this.ckbRename.TabIndex = 4;
            this.ckbRename.Text = "Rename if present";
            this.ckbRename.UseVisualStyleBackColor = true;
            this.ckbRename.CheckedChanged += new System.EventHandler(this.ckbRename_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.rb1Replace);
            this.flowLayoutPanel1.Controls.Add(this.rb1Reject);
            this.SetFlowBreak(this.flowLayoutPanel1, true);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(235, 23);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // rb1Replace
            // 
            this.rb1Replace.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb1Replace.AutoSize = true;
            this.rb1Replace.Checked = true;
            this.rb1Replace.Location = new System.Drawing.Point(3, 3);
            this.rb1Replace.Name = "rb1Replace";
            this.rb1Replace.Size = new System.Drawing.Size(116, 17);
            this.rb1Replace.TabIndex = 1;
            this.rb1Replace.TabStop = true;
            this.rb1Replace.Text = "Replace duplicates";
            this.rb1Replace.UseVisualStyleBackColor = true;
            this.rb1Replace.CheckedChanged += new System.EventHandler(this.rb1_CheckedChanged);
            // 
            // rb1Reject
            // 
            this.rb1Reject.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb1Reject.AutoSize = true;
            this.rb1Reject.Location = new System.Drawing.Point(125, 3);
            this.rb1Reject.Name = "rb1Reject";
            this.rb1Reject.Size = new System.Drawing.Size(107, 17);
            this.rb1Reject.TabIndex = 2;
            this.rb1Reject.Text = "Reject duplicates";
            this.rb1Reject.UseVisualStyleBackColor = true;
            this.rb1Reject.CheckedChanged += new System.EventHandler(this.rb1_CheckedChanged);
            // 
            // ImportSettings
            // 
            this.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.ckbCompress);
            this.Controls.Add(this.ckbUseName);
            this.Controls.Add(this.ckbRename);
            this.Size = new System.Drawing.Size(242, 46);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbCompress;
        private System.Windows.Forms.CheckBox ckbUseName;
        private System.Windows.Forms.CheckBox ckbRename;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton rb1Replace;
        private System.Windows.Forms.RadioButton rb1Reject;
    }
}
