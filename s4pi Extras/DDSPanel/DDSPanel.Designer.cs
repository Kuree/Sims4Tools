namespace System.Windows.Forms
{
    partial class DDSPanel
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
                if (ddsFile != null)
                {
                    ddsFile.Dispose();
                    ddsFile = null;
                }
                if (ddsMask != null)
                {
                    ddsMask.Dispose();
                    ddsMask = null;
                }
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.ckbR = new System.Windows.Forms.CheckBox();
            this.ckbG = new System.Windows.Forms.CheckBox();
            this.ckbB = new System.Windows.Forms.CheckBox();
            this.ckbA = new System.Windows.Forms.CheckBox();
            this.ckbI = new System.Windows.Forms.CheckBox();
            this.tlpSize = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.lbSize = new System.Windows.Forms.Label();
            this.lbDDSFmt = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.tlpSize.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(543, 157);
            this.tableLayoutPanel1.TabIndex = 1;
            this.tableLayoutPanel1.Click += new System.EventHandler(this.control_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(0, 29);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(543, 128);
            this.panel1.TabIndex = 2;
            this.panel1.Click += new System.EventHandler(this.control_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImage = global::System.Windows.Forms.Properties.Resources.checkerboard;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(128, 128);
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.control_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.ckbR);
            this.flowLayoutPanel1.Controls.Add(this.ckbG);
            this.flowLayoutPanel1.Controls.Add(this.ckbB);
            this.flowLayoutPanel1.Controls.Add(this.ckbA);
            this.flowLayoutPanel1.Controls.Add(this.ckbI);
            this.flowLayoutPanel1.Controls.Add(this.tlpSize);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 3);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(512, 23);
            this.flowLayoutPanel1.TabIndex = 0;
            this.flowLayoutPanel1.Click += new System.EventHandler(this.control_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Enable channels:";
            this.label1.Click += new System.EventHandler(this.control_Click);
            // 
            // ckbR
            // 
            this.ckbR.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbR.AutoSize = true;
            this.ckbR.Checked = true;
            this.ckbR.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbR.Location = new System.Drawing.Point(98, 3);
            this.ckbR.Name = "ckbR";
            this.ckbR.Size = new System.Drawing.Size(46, 17);
            this.ckbR.TabIndex = 0;
            this.ckbR.Text = "Red";
            this.ckbR.UseVisualStyleBackColor = true;
            this.ckbR.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbG
            // 
            this.ckbG.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbG.AutoSize = true;
            this.ckbG.Checked = true;
            this.ckbG.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbG.Location = new System.Drawing.Point(150, 3);
            this.ckbG.Name = "ckbG";
            this.ckbG.Size = new System.Drawing.Size(55, 17);
            this.ckbG.TabIndex = 1;
            this.ckbG.Text = "Green";
            this.ckbG.UseVisualStyleBackColor = true;
            this.ckbG.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbB
            // 
            this.ckbB.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbB.AutoSize = true;
            this.ckbB.Checked = true;
            this.ckbB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbB.Location = new System.Drawing.Point(211, 3);
            this.ckbB.Name = "ckbB";
            this.ckbB.Size = new System.Drawing.Size(47, 17);
            this.ckbB.TabIndex = 2;
            this.ckbB.Text = "Blue";
            this.ckbB.UseVisualStyleBackColor = true;
            this.ckbB.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbA
            // 
            this.ckbA.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbA.AutoSize = true;
            this.ckbA.Checked = true;
            this.ckbA.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckbA.Location = new System.Drawing.Point(264, 3);
            this.ckbA.Name = "ckbA";
            this.ckbA.Size = new System.Drawing.Size(53, 17);
            this.ckbA.TabIndex = 3;
            this.ckbA.Text = "Alpha";
            this.ckbA.UseVisualStyleBackColor = true;
            this.ckbA.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // ckbI
            // 
            this.ckbI.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbI.AutoSize = true;
            this.ckbI.Location = new System.Drawing.Point(323, 3);
            this.ckbI.Name = "ckbI";
            this.ckbI.Size = new System.Drawing.Size(53, 17);
            this.ckbI.TabIndex = 5;
            this.ckbI.Text = "Invert";
            this.ckbI.UseVisualStyleBackColor = true;
            this.ckbI.CheckedChanged += new System.EventHandler(this.ckb_CheckedChanged);
            // 
            // tlpSize
            // 
            this.tlpSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tlpSize.AutoSize = true;
            this.tlpSize.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpSize.ColumnCount = 3;
            this.tlpSize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSize.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpSize.Controls.Add(this.label2, 0, 0);
            this.tlpSize.Controls.Add(this.lbSize, 1, 0);
            this.tlpSize.Controls.Add(this.lbDDSFmt, 2, 0);
            this.tlpSize.Location = new System.Drawing.Point(379, 5);
            this.tlpSize.Margin = new System.Windows.Forms.Padding(0);
            this.tlpSize.Name = "tlpSize";
            this.tlpSize.RowCount = 1;
            this.tlpSize.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSize.Size = new System.Drawing.Size(133, 13);
            this.tlpSize.TabIndex = 6;
            this.tlpSize.Click += new System.EventHandler(this.control_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Size:";
            this.label2.Click += new System.EventHandler(this.control_Click);
            // 
            // lbSize
            // 
            this.lbSize.AutoSize = true;
            this.lbSize.Location = new System.Drawing.Point(39, 0);
            this.lbSize.Name = "lbSize";
            this.lbSize.Size = new System.Drawing.Size(41, 13);
            this.lbSize.TabIndex = 1;
            this.lbSize.Text = "XX, YY";
            this.lbSize.Click += new System.EventHandler(this.control_Click);
            // 
            // lbDDSFmt
            // 
            this.lbDDSFmt.AutoSize = true;
            this.lbDDSFmt.Location = new System.Drawing.Point(86, 0);
            this.lbDDSFmt.Name = "lbDDSFmt";
            this.lbDDSFmt.Size = new System.Drawing.Size(44, 13);
            this.lbDDSFmt.TabIndex = 2;
            this.lbDDSFmt.Text = "DDSfmt";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // DDSPanel
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "DDSPanel";
            this.Size = new System.Drawing.Size(543, 157);
            this.Resize += new System.EventHandler(this.DDSPanel_Resize);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tlpSize.ResumeLayout(false);
            this.tlpSize.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ckbR;
        private System.Windows.Forms.CheckBox ckbG;
        private System.Windows.Forms.CheckBox ckbB;
        private System.Windows.Forms.CheckBox ckbA;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox ckbI;
        private TableLayoutPanel tlpSize;
        private Label label2;
        private Label lbSize;
        private Label lbDDSFmt;
    }
}
