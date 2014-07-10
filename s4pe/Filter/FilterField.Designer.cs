namespace S4PIDemoFE.Filter
{
    partial class FilterField
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
            this.lbField = new System.Windows.Forms.Label();
            this.tbEntry = new System.Windows.Forms.TextBox();
            this.tbApplied = new System.Windows.Forms.TextBox();
            this.ckbFilter = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lbField, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbEntry, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbApplied, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.ckbFilter, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(102, 82);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lbField
            // 
            this.lbField.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lbField.AutoEllipsis = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lbField, 2);
            this.lbField.Location = new System.Drawing.Point(3, 0);
            this.lbField.Name = "lbField";
            this.lbField.Size = new System.Drawing.Size(96, 13);
            this.lbField.TabIndex = 1;
            this.lbField.Text = "label1";
            this.lbField.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbEntry
            // 
            this.tbEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbEntry.Location = new System.Drawing.Point(24, 16);
            this.tbEntry.Name = "tbEntry";
            this.tbEntry.Size = new System.Drawing.Size(75, 20);
            this.tbEntry.TabIndex = 3;
            this.tbEntry.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbEntry.Leave += new System.EventHandler(this.tbEntry_Leave);
            // 
            // tbApplied
            // 
            this.tbApplied.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbApplied.Location = new System.Drawing.Point(24, 42);
            this.tbApplied.Name = "tbApplied";
            this.tbApplied.ReadOnly = true;
            this.tbApplied.Size = new System.Drawing.Size(75, 20);
            this.tbApplied.TabIndex = 4;
            this.tbApplied.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ckbFilter
            // 
            this.ckbFilter.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbFilter.AutoSize = true;
            this.ckbFilter.Location = new System.Drawing.Point(3, 19);
            this.ckbFilter.Name = "ckbFilter";
            this.ckbFilter.Size = new System.Drawing.Size(15, 14);
            this.ckbFilter.TabIndex = 2;
            this.ckbFilter.UseVisualStyleBackColor = true;
            // 
            // FilterField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "FilterField";
            this.Size = new System.Drawing.Size(102, 82);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox ckbFilter;
        private System.Windows.Forms.TextBox tbEntry;
        private System.Windows.Forms.TextBox tbApplied;
        private System.Windows.Forms.Label lbField;
    }
}
