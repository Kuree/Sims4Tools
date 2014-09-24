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

namespace S4PIDemoFE
{
    partial class ResourceDetails
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
            this.importSettings1 = new S4PIDemoFE.Import.ImportSettings();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbType = new System.Windows.Forms.ResourceTypeCombo();
            this.tbGroup = new System.Windows.Forms.TextBox();
            this.tbInstance = new System.Windows.Forms.TextBox();
            this.lbFilename = new System.Windows.Forms.Label();
            this.tbFilename = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tlpName = new System.Windows.Forms.TableLayoutPanel();
            this.tbName = new System.Windows.Forms.TextBox();
            this.btnFNV64 = new System.Windows.Forms.Button();
            this.btnCLIPIID = new System.Windows.Forms.Button();
            this.btnFNV32 = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyResourceKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteResourceKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPaste = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpName.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // importSettings1
            // 
            this.importSettings1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.importSettings1.AutoSize = true;
            this.importSettings1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.importSettings1, 2);
            this.importSettings1.Compress = true;
            this.importSettings1.Location = new System.Drawing.Point(3, 82);
            this.importSettings1.Name = "importSettings1";
            this.importSettings1.Size = new System.Drawing.Size(480, 52);
            this.importSettings1.TabIndex = 9;
            this.importSettings1.UseNameChanged += new System.EventHandler(this.ckbUseName_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(423, 204);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOKCancel_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(342, 204);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnOKCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbType, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbGroup, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbInstance, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbFilename, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.tbFilename, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.importSettings1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tlpName, 1, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(486, 186);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Instance";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Group";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Type";
            // 
            // cbType
            // 
            this.cbType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbType.Location = new System.Drawing.Point(58, 3);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(425, 21);
            this.cbType.TabIndex = 2;
            this.cbType.Value = ((uint)(0u));
            this.cbType.ValidChanged += new System.EventHandler(this.cbType_ValidChanged);
            this.cbType.ValueChanged += new System.EventHandler(this.cbType_ValueChanged);
            // 
            // tbGroup
            // 
            this.tbGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbGroup.Location = new System.Drawing.Point(58, 30);
            this.tbGroup.Name = "tbGroup";
            this.tbGroup.Size = new System.Drawing.Size(425, 20);
            this.tbGroup.TabIndex = 4;
            this.tbGroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbGroup.TextChanged += new System.EventHandler(this.tbGroupInstance_TextChanged);
            // 
            // tbInstance
            // 
            this.tbInstance.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInstance.Location = new System.Drawing.Point(58, 56);
            this.tbInstance.Name = "tbInstance";
            this.tbInstance.Size = new System.Drawing.Size(425, 20);
            this.tbInstance.TabIndex = 6;
            this.tbInstance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbInstance.TextChanged += new System.EventHandler(this.tbGroupInstance_TextChanged);
            // 
            // lbFilename
            // 
            this.lbFilename.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbFilename.AutoSize = true;
            this.lbFilename.Location = new System.Drawing.Point(3, 169);
            this.lbFilename.Name = "lbFilename";
            this.lbFilename.Size = new System.Drawing.Size(49, 13);
            this.lbFilename.TabIndex = 12;
            this.lbFilename.Text = "Filename";
            // 
            // tbFilename
            // 
            this.tbFilename.AllowDrop = true;
            this.tbFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFilename.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbFilename.Location = new System.Drawing.Point(58, 169);
            this.tbFilename.Name = "tbFilename";
            this.tbFilename.ReadOnly = true;
            this.tbFilename.Size = new System.Drawing.Size(425, 13);
            this.tbFilename.TabIndex = 12;
            this.tbFilename.DragDrop += new System.Windows.Forms.DragEventHandler(this.tbFilename_DragDrop);
            this.tbFilename.DragOver += new System.Windows.Forms.DragEventHandler(this.tbFilename_DragOver);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Name";
            // 
            // tlpName
            // 
            this.tlpName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpName.AutoSize = true;
            this.tlpName.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpName.ColumnCount = 4;
            this.tlpName.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpName.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpName.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpName.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpName.Controls.Add(this.tbName, 0, 0);
            this.tlpName.Controls.Add(this.btnFNV64, 1, 0);
            this.tlpName.Controls.Add(this.btnCLIPIID, 2, 0);
            this.tlpName.Controls.Add(this.btnFNV32, 3, 0);
            this.tlpName.Enabled = false;
            this.tlpName.Location = new System.Drawing.Point(55, 137);
            this.tlpName.Margin = new System.Windows.Forms.Padding(0);
            this.tlpName.Name = "tlpName";
            this.tlpName.RowCount = 1;
            this.tlpName.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpName.Size = new System.Drawing.Size(431, 29);
            this.tlpName.TabIndex = 11;
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.Location = new System.Drawing.Point(3, 4);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(227, 20);
            this.tbName.TabIndex = 1;
            this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            // 
            // btnFNV64
            // 
            this.btnFNV64.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnFNV64.Location = new System.Drawing.Point(236, 3);
            this.btnFNV64.Name = "btnFNV64";
            this.btnFNV64.Size = new System.Drawing.Size(60, 23);
            this.btnFNV64.TabIndex = 2;
            this.btnFNV64.Text = "FNV&64";
            this.btnFNV64.UseVisualStyleBackColor = true;
            this.btnFNV64.Click += new System.EventHandler(this.btnFNV64_Click);
            // 
            // btnCLIPIID
            // 
            this.btnCLIPIID.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCLIPIID.Location = new System.Drawing.Point(302, 3);
            this.btnCLIPIID.Name = "btnCLIPIID";
            this.btnCLIPIID.Size = new System.Drawing.Size(60, 23);
            this.btnCLIPIID.TabIndex = 2;
            this.btnCLIPIID.Text = "CLIP &IID";
            this.btnCLIPIID.UseVisualStyleBackColor = true;
            this.btnCLIPIID.Click += new System.EventHandler(this.btnCLIPIID_Click);
            // 
            // btnFNV32
            // 
            this.btnFNV32.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnFNV32.Location = new System.Drawing.Point(368, 3);
            this.btnFNV32.Name = "btnFNV32";
            this.btnFNV32.Size = new System.Drawing.Size(60, 23);
            this.btnFNV32.TabIndex = 3;
            this.btnFNV32.Text = "FNV&32";
            this.btnFNV32.UseVisualStyleBackColor = true;
            this.btnFNV32.Click += new System.EventHandler(this.btnFNV32_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Location = new System.Drawing.Point(12, 204);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 4;
            this.btnCopy.Text = "&Copy RK";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyResourceKeyToolStripMenuItem,
            this.pasteResourceKeyToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(168, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // copyResourceKeyToolStripMenuItem
            // 
            this.copyResourceKeyToolStripMenuItem.Name = "copyResourceKeyToolStripMenuItem";
            this.copyResourceKeyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.copyResourceKeyToolStripMenuItem.Text = "&Copy ResourceKey";
            this.copyResourceKeyToolStripMenuItem.Click += new System.EventHandler(this.copyResourceKeyToolStripMenuItem_Click);
            // 
            // pasteResourceKeyToolStripMenuItem
            // 
            this.pasteResourceKeyToolStripMenuItem.Name = "pasteResourceKeyToolStripMenuItem";
            this.pasteResourceKeyToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.pasteResourceKeyToolStripMenuItem.Text = "&Paste ResourceKey";
            this.pasteResourceKeyToolStripMenuItem.Click += new System.EventHandler(this.pasteResourceKeyToolStripMenuItem_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPaste.Location = new System.Drawing.Point(93, 204);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(75, 23);
            this.btnPaste.TabIndex = 6;
            this.btnPaste.Text = "&Paste RK";
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // ResourceDetails
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(510, 239);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ResourceDetails";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Resource Details";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlpName.ResumeLayout(false);
            this.tlpName.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbGroup;
        private System.Windows.Forms.TextBox tbInstance;
        private System.Windows.Forms.Label lbFilename;
        private System.Windows.Forms.TextBox tbFilename;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbName;
        private S4PIDemoFE.Import.ImportSettings importSettings1;
        private System.Windows.Forms.ResourceTypeCombo cbType;
        private System.Windows.Forms.TableLayoutPanel tlpName;
        private System.Windows.Forms.Button btnFNV64;
        private System.Windows.Forms.Button btnCLIPIID;
        private System.Windows.Forms.Button btnFNV32;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyResourceKeyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteResourceKeyToolStripMenuItem;
        private System.Windows.Forms.Button btnPaste;
    }
}