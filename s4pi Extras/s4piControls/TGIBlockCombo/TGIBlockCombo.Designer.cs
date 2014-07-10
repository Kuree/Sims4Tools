namespace System.Windows.Forms
{
    partial class TGIBlockCombo
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbTGIBlocks = new System.Windows.Forms.ComboBox();
            this.btnTGIBlockListEditor = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.cbTGIBlocks, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnTGIBlockListEditor, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(632, 21);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // cbTGIBlocks
            // 
            this.cbTGIBlocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTGIBlocks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTGIBlocks.FormattingEnabled = true;
            this.cbTGIBlocks.Location = new System.Drawing.Point(0, 0);
            this.cbTGIBlocks.Margin = new System.Windows.Forms.Padding(0);
            this.cbTGIBlocks.Name = "cbTGIBlocks";
            this.cbTGIBlocks.Size = new System.Drawing.Size(615, 21);
            this.cbTGIBlocks.TabIndex = 0;
            this.cbTGIBlocks.SelectedIndexChanged += new System.EventHandler(this.cbTGIBlocks_SelectedIndexChanged);
            // 
            // btnTGIBlockListEditor
            // 
            this.btnTGIBlockListEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTGIBlockListEditor.Enabled = false;
            this.btnTGIBlockListEditor.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnTGIBlockListEditor.Location = new System.Drawing.Point(615, 2);
            this.btnTGIBlockListEditor.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.btnTGIBlockListEditor.Name = "btnTGIBlockListEditor";
            this.btnTGIBlockListEditor.Size = new System.Drawing.Size(17, 17);
            this.btnTGIBlockListEditor.TabIndex = 1;
            this.btnTGIBlockListEditor.Text = "ì";
            this.btnTGIBlockListEditor.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnTGIBlockListEditor.UseVisualStyleBackColor = true;
            this.btnTGIBlockListEditor.Click += new System.EventHandler(this.btnTGIBlockListEditor_Click);
            // 
            // TGIBlockCombo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TGIBlockCombo";
            this.Size = new System.Drawing.Size(632, 21);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private ComboBox cbTGIBlocks;
        private Button btnTGIBlockListEditor;
    }
}
