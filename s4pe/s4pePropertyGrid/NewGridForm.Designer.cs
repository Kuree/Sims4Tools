namespace S4PIDemoFE
{
    partial class NewGridForm
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
            this.btnClose = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.s4piPropertyGrid1 = new S4PIDemoFE.s4piPropertyGrid();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCollAll = new System.Windows.Forms.Button();
            this.btnExpAll = new System.Windows.Forms.Button();
            this.flpMainButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tlpAddDelete = new System.Windows.Forms.TableLayoutPanel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnInsert = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flpMainButtons.SuspendLayout();
            this.tlpAddDelete.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnClose.Location = new System.Drawing.Point(336, 0);
            this.btnClose.Margin = new System.Windows.Forms.Padding(0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "OK";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.s4piPropertyGrid1);
            this.splitContainer1.Size = new System.Drawing.Size(771, 242);
            this.splitContainer1.SplitterDistance = 240;
            this.splitContainer1.TabIndex = 0;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(240, 242);
            this.listBox1.TabIndex = 1;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // s3PIPropertyGrid1
            // 
            this.s4piPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.s4piPropertyGrid1.HelpVisible = false;
            this.s4piPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.s4piPropertyGrid1.Name = "s3PIPropertyGrid1";
            this.s4piPropertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.s4piPropertyGrid1.Size = new System.Drawing.Size(527, 242);
            this.s4piPropertyGrid1.TabIndex = 2;
            this.s4piPropertyGrid1.ToolbarVisible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnCollAll, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnExpAll, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.flpMainButtons, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(372, 263);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(411, 23);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // btnCollAll
            // 
            this.btnCollAll.Location = new System.Drawing.Point(0, 0);
            this.btnCollAll.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btnCollAll.Name = "btnCollAll";
            this.btnCollAll.Size = new System.Drawing.Size(75, 23);
            this.btnCollAll.TabIndex = 3;
            this.btnCollAll.Text = "Collapse All";
            this.btnCollAll.UseVisualStyleBackColor = true;
            this.btnCollAll.Click += new System.EventHandler(this.btnCollAll_Click);
            // 
            // btnExpAll
            // 
            this.btnExpAll.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnExpAll.Location = new System.Drawing.Point(87, 0);
            this.btnExpAll.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btnExpAll.Name = "btnExpAll";
            this.btnExpAll.Size = new System.Drawing.Size(75, 23);
            this.btnExpAll.TabIndex = 1;
            this.btnExpAll.Text = "Expand All";
            this.btnExpAll.UseVisualStyleBackColor = true;
            this.btnExpAll.Click += new System.EventHandler(this.btnExpAll_Click);
            // 
            // flpMainButtons
            // 
            this.flpMainButtons.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.flpMainButtons.AutoSize = true;
            this.flpMainButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flpMainButtons.Controls.Add(this.btnCancel);
            this.flpMainButtons.Controls.Add(this.btnOK);
            this.flpMainButtons.Location = new System.Drawing.Point(174, 0);
            this.flpMainButtons.Margin = new System.Windows.Forms.Padding(0);
            this.flpMainButtons.Name = "flpMainButtons";
            this.flpMainButtons.Size = new System.Drawing.Size(162, 23);
            this.flpMainButtons.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(0, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(87, 0);
            this.btnOK.Margin = new System.Windows.Forms.Padding(0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Commit";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tlpAddDelete
            // 
            this.tlpAddDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tlpAddDelete.AutoSize = true;
            this.tlpAddDelete.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpAddDelete.ColumnCount = 5;
            this.tlpAddDelete.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAddDelete.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpAddDelete.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAddDelete.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAddDelete.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpAddDelete.Controls.Add(this.btnAdd, 2, 0);
            this.tlpAddDelete.Controls.Add(this.btnCopy, 0, 0);
            this.tlpAddDelete.Controls.Add(this.btnDelete, 4, 0);
            this.tlpAddDelete.Controls.Add(this.btnInsert, 3, 0);
            this.tlpAddDelete.Location = new System.Drawing.Point(12, 263);
            this.tlpAddDelete.Margin = new System.Windows.Forms.Padding(0);
            this.tlpAddDelete.Name = "tlpAddDelete";
            this.tlpAddDelete.RowCount = 1;
            this.tlpAddDelete.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpAddDelete.Size = new System.Drawing.Size(254, 23);
            this.tlpAddDelete.TabIndex = 1;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(86, 0);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(9, 0, 6, 0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(48, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "&Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Enabled = false;
            this.btnCopy.Location = new System.Drawing.Point(0, 0);
            this.btnCopy.Margin = new System.Windows.Forms.Padding(0, 0, 9, 0);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(48, 23);
            this.btnCopy.TabIndex = 2;
            this.btnCopy.Text = "Cop&y";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(206, 0);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(48, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "De&lete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // btnInsert
            // 
            this.btnInsert.Enabled = false;
            this.btnInsert.Location = new System.Drawing.Point(146, 0);
            this.btnInsert.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.Size = new System.Drawing.Size(48, 23);
            this.btnInsert.TabIndex = 2;
            this.btnInsert.Text = "I&nsert";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // NewGridForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(795, 301);
            this.Controls.Add(this.tlpAddDelete);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.splitContainer1);
            this.ControlBox = false;
            this.Name = "NewGridForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "Data Grid";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NewGridForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flpMainButtons.ResumeLayout(false);
            this.tlpAddDelete.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private s4piPropertyGrid s4piPropertyGrid1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flpMainButtons;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TableLayoutPanel tlpAddDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btnExpAll;
        private System.Windows.Forms.Button btnCollAll;
        private System.Windows.Forms.Button btnInsert;
    }
}