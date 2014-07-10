namespace S4PIDemoFE
{
    partial class ControlPanel
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ckbSortable = new System.Windows.Forms.CheckBox();
            this.btnHex = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.ckbNoUnWrap = new System.Windows.Forms.CheckBox();
            this.ckbUseNames = new System.Windows.Forms.CheckBox();
            this.btnCommit = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnGrid = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnHelper1 = new System.Windows.Forms.Button();
            this.btnHelper2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnHexEdit = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.ckbUseTags = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rb1Off = new System.Windows.Forms.RadioButton();
            this.rb1Hex = new System.Windows.Forms.RadioButton();
            this.rb1Preview = new System.Windows.Forms.RadioButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbSortable
            // 
            this.ckbSortable.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbSortable.AutoSize = true;
            this.ckbSortable.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbSortable.Location = new System.Drawing.Point(0, 5);
            this.ckbSortable.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.ckbSortable.Name = "ckbSortable";
            this.ckbSortable.Size = new System.Drawing.Size(45, 17);
            this.ckbSortable.TabIndex = 1;
            this.ckbSortable.Text = "Sort";
            this.ckbSortable.UseVisualStyleBackColor = true;
            this.ckbSortable.CheckedChanged += new System.EventHandler(this.ckbSortable_CheckedChanged);
            // 
            // btnHex
            // 
            this.btnHex.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHex.Enabled = false;
            this.btnHex.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnHex.Location = new System.Drawing.Point(365, 2);
            this.btnHex.Margin = new System.Windows.Forms.Padding(0);
            this.btnHex.Name = "btnHex";
            this.btnHex.Size = new System.Drawing.Size(53, 23);
            this.btnHex.TabIndex = 7;
            this.btnHex.Text = "Hex";
            this.btnHex.UseVisualStyleBackColor = true;
            this.btnHex.Click += new System.EventHandler(this.btnHex_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPreview.Enabled = false;
            this.btnPreview.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnPreview.Location = new System.Drawing.Point(424, 2);
            this.btnPreview.Margin = new System.Windows.Forms.Padding(0);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(53, 23);
            this.btnPreview.TabIndex = 8;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnValue_Click);
            // 
            // ckbNoUnWrap
            // 
            this.ckbNoUnWrap.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbNoUnWrap.AutoSize = true;
            this.ckbNoUnWrap.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbNoUnWrap.Location = new System.Drawing.Point(60, 5);
            this.ckbNoUnWrap.Margin = new System.Windows.Forms.Padding(3, 0, 12, 0);
            this.ckbNoUnWrap.Name = "ckbNoUnWrap";
            this.ckbNoUnWrap.Size = new System.Drawing.Size(87, 17);
            this.ckbNoUnWrap.TabIndex = 2;
            this.ckbNoUnWrap.Text = "Do not parse";
            this.ckbNoUnWrap.UseVisualStyleBackColor = true;
            this.ckbNoUnWrap.CheckedChanged += new System.EventHandler(this.ckbNoUnWrap_CheckedChanged);
            // 
            // ckbUseNames
            // 
            this.ckbUseNames.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbUseNames.AutoSize = true;
            this.ckbUseNames.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbUseNames.Location = new System.Drawing.Point(44, 0);
            this.ckbUseNames.Margin = new System.Windows.Forms.Padding(0);
            this.ckbUseNames.Name = "ckbUseNames";
            this.ckbUseNames.Size = new System.Drawing.Size(59, 17);
            this.ckbUseNames.TabIndex = 11;
            this.ckbUseNames.Text = "Names";
            this.ckbUseNames.UseVisualStyleBackColor = true;
            this.ckbUseNames.CheckedChanged += new System.EventHandler(this.ckbUseNames_CheckedChanged);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Enabled = false;
            this.btnCommit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnCommit.Location = new System.Drawing.Point(1019, 2);
            this.btnCommit.Margin = new System.Windows.Forms.Padding(0);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(75, 23);
            this.btnCommit.TabIndex = 14;
            this.btnCommit.Text = "&Commit";
            this.btnCommit.UseVisualStyleBackColor = true;
            this.btnCommit.Visible = false;
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 18;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ckbSortable, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCommit, 17, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPreview, 9, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnGrid, 11, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 15, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 13, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbNoUnWrap, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnHex, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.rb1Off, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.rb1Hex, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.rb1Preview, 5, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1094, 27);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnGrid
            // 
            this.btnGrid.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGrid.Enabled = false;
            this.btnGrid.Location = new System.Drawing.Point(483, 2);
            this.btnGrid.Margin = new System.Windows.Forms.Padding(0);
            this.btnGrid.Name = "btnGrid";
            this.btnGrid.Size = new System.Drawing.Size(53, 23);
            this.btnGrid.TabIndex = 9;
            this.btnGrid.Text = "&Grid";
            this.btnGrid.UseVisualStyleBackColor = true;
            this.btnGrid.Click += new System.EventHandler(this.btnGrid_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 6;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 6F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnHelper1, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnHelper2, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnHexEdit, 5, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(707, 1);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(247, 25);
            this.tableLayoutPanel2.TabIndex = 13;
            this.tableLayoutPanel2.Visible = false;
            // 
            // btnHelper1
            // 
            this.btnHelper1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHelper1.AutoSize = true;
            this.btnHelper1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnHelper1.Enabled = false;
            this.btnHelper1.Location = new System.Drawing.Point(56, 1);
            this.btnHelper1.Margin = new System.Windows.Forms.Padding(0);
            this.btnHelper1.MaximumSize = new System.Drawing.Size(105, 23);
            this.btnHelper1.MinimumSize = new System.Drawing.Size(20, 23);
            this.btnHelper1.Name = "btnHelper1";
            this.btnHelper1.Size = new System.Drawing.Size(54, 23);
            this.btnHelper1.TabIndex = 2;
            this.btnHelper1.Text = "Helper1";
            this.btnHelper1.UseVisualStyleBackColor = true;
            this.btnHelper1.Click += new System.EventHandler(this.btnHelper1_Click);
            // 
            // btnHelper2
            // 
            this.btnHelper2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHelper2.AutoSize = true;
            this.btnHelper2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnHelper2.Enabled = false;
            this.btnHelper2.Location = new System.Drawing.Point(118, 1);
            this.btnHelper2.Margin = new System.Windows.Forms.Padding(0);
            this.btnHelper2.MaximumSize = new System.Drawing.Size(105, 23);
            this.btnHelper2.MinimumSize = new System.Drawing.Size(20, 23);
            this.btnHelper2.Name = "btnHelper2";
            this.btnHelper2.Size = new System.Drawing.Size(54, 23);
            this.btnHelper2.TabIndex = 3;
            this.btnHelper2.Text = "Helper2";
            this.btnHelper2.UseVisualStyleBackColor = true;
            this.btnHelper2.Click += new System.EventHandler(this.btnHelper2_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "External:";
            // 
            // btnHexEdit
            // 
            this.btnHexEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHexEdit.AutoSize = true;
            this.btnHexEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnHexEdit.Enabled = false;
            this.btnHexEdit.Location = new System.Drawing.Point(180, 1);
            this.btnHexEdit.Margin = new System.Windows.Forms.Padding(0);
            this.btnHexEdit.MaximumSize = new System.Drawing.Size(105, 23);
            this.btnHexEdit.MinimumSize = new System.Drawing.Size(20, 23);
            this.btnHexEdit.Name = "btnHexEdit";
            this.btnHexEdit.Size = new System.Drawing.Size(66, 23);
            this.btnHexEdit.TabIndex = 4;
            this.btnHexEdit.Text = "Hex Editor";
            this.btnHexEdit.UseVisualStyleBackColor = true;
            this.btnHexEdit.Click += new System.EventHandler(this.btnHexEdit_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.ckbUseNames, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.ckbUseTags, 2, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(548, 5);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(153, 17);
            this.tableLayoutPanel3.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, 2);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Display:";
            // 
            // ckbUseTags
            // 
            this.ckbUseTags.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbUseTags.AutoSize = true;
            this.ckbUseTags.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ckbUseTags.Location = new System.Drawing.Point(103, 0);
            this.ckbUseTags.Margin = new System.Windows.Forms.Padding(0);
            this.ckbUseTags.Name = "ckbUseTags";
            this.ckbUseTags.Size = new System.Drawing.Size(50, 17);
            this.ckbUseTags.TabIndex = 12;
            this.ckbUseTags.Text = "Tags";
            this.ckbUseTags.UseVisualStyleBackColor = true;
            this.ckbUseTags.CheckedChanged += new System.EventHandler(this.ckbUseTags_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(159, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Preview:";
            // 
            // rb1Off
            // 
            this.rb1Off.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb1Off.AutoSize = true;
            this.rb1Off.Checked = true;
            this.rb1Off.Location = new System.Drawing.Point(207, 5);
            this.rb1Off.Margin = new System.Windows.Forms.Padding(0);
            this.rb1Off.Name = "rb1Off";
            this.rb1Off.Size = new System.Drawing.Size(39, 17);
            this.rb1Off.TabIndex = 4;
            this.rb1Off.TabStop = true;
            this.rb1Off.Text = "Off";
            this.rb1Off.UseVisualStyleBackColor = true;
            this.rb1Off.CheckedChanged += new System.EventHandler(this.rb1Off_CheckedChanged);
            // 
            // rb1Hex
            // 
            this.rb1Hex.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb1Hex.AutoSize = true;
            this.rb1Hex.Location = new System.Drawing.Point(246, 5);
            this.rb1Hex.Margin = new System.Windows.Forms.Padding(0);
            this.rb1Hex.Name = "rb1Hex";
            this.rb1Hex.Size = new System.Drawing.Size(44, 17);
            this.rb1Hex.TabIndex = 5;
            this.rb1Hex.Text = "&Hex";
            this.rb1Hex.UseVisualStyleBackColor = true;
            this.rb1Hex.CheckedChanged += new System.EventHandler(this.rb1Hex_CheckedChanged);
            // 
            // rb1Preview
            // 
            this.rb1Preview.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rb1Preview.AutoSize = true;
            this.rb1Preview.Location = new System.Drawing.Point(290, 5);
            this.rb1Preview.Margin = new System.Windows.Forms.Padding(0);
            this.rb1Preview.Name = "rb1Preview";
            this.rb1Preview.Size = new System.Drawing.Size(63, 17);
            this.rb1Preview.TabIndex = 6;
            this.rb1Preview.Text = "Pre&view";
            this.rb1Preview.UseVisualStyleBackColor = true;
            this.rb1Preview.CheckedChanged += new System.EventHandler(this.rb1Value_CheckedChanged);
            // 
            // ControlPanel
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ControlPanel";
            this.Size = new System.Drawing.Size(1094, 27);
            this.Load += new System.EventHandler(this.ControlPanel_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

          }

        #endregion

        private System.Windows.Forms.CheckBox ckbSortable;
        private System.Windows.Forms.Button btnHex;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.CheckBox ckbNoUnWrap;
        private System.Windows.Forms.CheckBox ckbUseNames;
        private System.Windows.Forms.Button btnCommit;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnGrid;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnHelper1;
        private System.Windows.Forms.Button btnHelper2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox ckbUseTags;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button btnHexEdit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rb1Off;
        private System.Windows.Forms.RadioButton rb1Hex;
        private System.Windows.Forms.RadioButton rb1Preview;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

