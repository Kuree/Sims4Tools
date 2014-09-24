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

namespace S4PIDemoFE.Settings
{
    partial class ExternalProgramsDialog
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tbUserHexEditor = new System.Windows.Forms.TextBox();
            this.ckbUserHexEditor = new System.Windows.Forms.CheckBox();
            this.btnHexEditorBrowse = new System.Windows.Forms.Button();
            this.ckbHexEditorTS = new System.Windows.Forms.CheckBox();
            this.ckbHexQuotes = new System.Windows.Forms.CheckBox();
            this.ofdUserEditor = new System.Windows.Forms.OpenFileDialog();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.tlpHelpers = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tbUserTextEditor = new System.Windows.Forms.TextBox();
            this.ckbUserTextEditor = new System.Windows.Forms.CheckBox();
            this.btnTextEditorBrowse = new System.Windows.Forms.Button();
            this.ckbTextEditorTS = new System.Windows.Forms.CheckBox();
            this.ckbTextQuotes = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpHelpers.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(349, 282);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(268, 282);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.tbUserHexEditor, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.ckbUserHexEditor, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnHexEditorBrowse, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.ckbHexEditorTS, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.ckbHexQuotes, 0, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(406, 102);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tbUserHexEditor
            // 
            this.tbUserHexEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUserHexEditor.Location = new System.Drawing.Point(24, 27);
            this.tbUserHexEditor.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.tbUserHexEditor.Name = "tbUserHexEditor";
            this.tbUserHexEditor.ReadOnly = true;
            this.tbUserHexEditor.Size = new System.Drawing.Size(298, 20);
            this.tbUserHexEditor.TabIndex = 2;
            // 
            // ckbUserHexEditor
            // 
            this.ckbUserHexEditor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbUserHexEditor.AutoSize = true;
            this.ckbUserHexEditor.Location = new System.Drawing.Point(3, 3);
            this.ckbUserHexEditor.Name = "ckbUserHexEditor";
            this.ckbUserHexEditor.Size = new System.Drawing.Size(149, 17);
            this.ckbUserHexEditor.TabIndex = 1;
            this.ckbUserHexEditor.Text = "Use an external hex editor";
            this.ckbUserHexEditor.UseVisualStyleBackColor = true;
            this.ckbUserHexEditor.CheckedChanged += new System.EventHandler(this.ckbUserHexEditor_CheckedChanged);
            // 
            // btnHexEditorBrowse
            // 
            this.btnHexEditorBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHexEditorBrowse.Enabled = false;
            this.btnHexEditorBrowse.Location = new System.Drawing.Point(328, 26);
            this.btnHexEditorBrowse.Name = "btnHexEditorBrowse";
            this.btnHexEditorBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnHexEditorBrowse.TabIndex = 3;
            this.btnHexEditorBrowse.Text = "Browse...";
            this.btnHexEditorBrowse.UseVisualStyleBackColor = true;
            this.btnHexEditorBrowse.Click += new System.EventHandler(this.btnHexEditorBrowse_Click);
            // 
            // ckbHexEditorTS
            // 
            this.ckbHexEditorTS.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbHexEditorTS.AutoSize = true;
            this.ckbHexEditorTS.Enabled = false;
            this.ckbHexEditorTS.Location = new System.Drawing.Point(24, 55);
            this.ckbHexEditorTS.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.ckbHexEditorTS.Name = "ckbHexEditorTS";
            this.ckbHexEditorTS.Size = new System.Drawing.Size(143, 17);
            this.ckbHexEditorTS.TabIndex = 4;
            this.ckbHexEditorTS.Text = "Does not update file time";
            this.ckbHexEditorTS.UseVisualStyleBackColor = true;
            // 
            // ckbHexQuotes
            // 
            this.ckbHexQuotes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbHexQuotes.AutoSize = true;
            this.ckbHexQuotes.Enabled = false;
            this.ckbHexQuotes.Location = new System.Drawing.Point(24, 78);
            this.ckbHexQuotes.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.ckbHexQuotes.Name = "ckbHexQuotes";
            this.ckbHexQuotes.Size = new System.Drawing.Size(170, 17);
            this.ckbHexQuotes.TabIndex = 4;
            this.ckbHexQuotes.Text = "Needs quotes around filename";
            this.ckbHexQuotes.UseVisualStyleBackColor = true;
            // 
            // ofdUserEditor
            // 
            this.ofdUserEditor.FileName = "*.exe";
            this.ofdUserEditor.Filter = "Program files|*.exe";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tlpHelpers, 0, 4);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(412, 264);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 226);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Helpers:";
            // 
            // tlpHelpers
            // 
            this.tlpHelpers.AutoSize = true;
            this.tlpHelpers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpHelpers.ColumnCount = 3;
            this.tlpHelpers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHelpers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpHelpers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHelpers.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpHelpers.Controls.Add(this.label1, 1, 0);
            this.tlpHelpers.Controls.Add(this.label3, 0, 0);
            this.tlpHelpers.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlpHelpers.Location = new System.Drawing.Point(3, 242);
            this.tlpHelpers.Name = "tlpHelpers";
            this.tlpHelpers.RowCount = 2;
            this.tlpHelpers.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpHelpers.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpHelpers.Size = new System.Drawing.Size(406, 13);
            this.tlpHelpers.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Disabled";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Info";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.tbUserTextEditor, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.ckbUserTextEditor, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnTextEditorBrowse, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.ckbTextEditorTS, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.ckbTextQuotes, 0, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 111);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 5;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(406, 102);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // tbUserTextEditor
            // 
            this.tbUserTextEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbUserTextEditor.Location = new System.Drawing.Point(24, 27);
            this.tbUserTextEditor.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.tbUserTextEditor.Name = "tbUserTextEditor";
            this.tbUserTextEditor.ReadOnly = true;
            this.tbUserTextEditor.Size = new System.Drawing.Size(298, 20);
            this.tbUserTextEditor.TabIndex = 2;
            // 
            // ckbUserTextEditor
            // 
            this.ckbUserTextEditor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbUserTextEditor.AutoSize = true;
            this.ckbUserTextEditor.Location = new System.Drawing.Point(3, 3);
            this.ckbUserTextEditor.Name = "ckbUserTextEditor";
            this.ckbUserTextEditor.Size = new System.Drawing.Size(149, 17);
            this.ckbUserTextEditor.TabIndex = 1;
            this.ckbUserTextEditor.Text = "Use an external text editor";
            this.ckbUserTextEditor.UseVisualStyleBackColor = true;
            this.ckbUserTextEditor.CheckedChanged += new System.EventHandler(this.ckbUserTextEditor_CheckedChanged);
            // 
            // btnTextEditorBrowse
            // 
            this.btnTextEditorBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnTextEditorBrowse.Enabled = false;
            this.btnTextEditorBrowse.Location = new System.Drawing.Point(328, 26);
            this.btnTextEditorBrowse.Name = "btnTextEditorBrowse";
            this.btnTextEditorBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnTextEditorBrowse.TabIndex = 3;
            this.btnTextEditorBrowse.Text = "Browse...";
            this.btnTextEditorBrowse.UseVisualStyleBackColor = true;
            this.btnTextEditorBrowse.Click += new System.EventHandler(this.btnTextEditorBrowse_Click);
            // 
            // ckbTextEditorTS
            // 
            this.ckbTextEditorTS.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbTextEditorTS.AutoSize = true;
            this.ckbTextEditorTS.Enabled = false;
            this.ckbTextEditorTS.Location = new System.Drawing.Point(24, 55);
            this.ckbTextEditorTS.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.ckbTextEditorTS.Name = "ckbTextEditorTS";
            this.ckbTextEditorTS.Size = new System.Drawing.Size(143, 17);
            this.ckbTextEditorTS.TabIndex = 4;
            this.ckbTextEditorTS.Text = "Does not update file time";
            this.ckbTextEditorTS.UseVisualStyleBackColor = true;
            // 
            // ckbTextQuotes
            // 
            this.ckbTextQuotes.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbTextQuotes.AutoSize = true;
            this.ckbTextQuotes.Enabled = false;
            this.ckbTextQuotes.Location = new System.Drawing.Point(24, 78);
            this.ckbTextQuotes.Margin = new System.Windows.Forms.Padding(24, 3, 3, 3);
            this.ckbTextQuotes.Name = "ckbTextQuotes";
            this.ckbTextQuotes.Size = new System.Drawing.Size(170, 17);
            this.ckbTextQuotes.TabIndex = 4;
            this.ckbTextQuotes.Text = "Needs quotes around filename";
            this.ckbTextQuotes.UseVisualStyleBackColor = true;
            // 
            // ExternalProgramsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(436, 317);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ExternalProgramsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "External Program Settings";
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlpHelpers.ResumeLayout(false);
            this.tlpHelpers.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox tbUserHexEditor;
        private System.Windows.Forms.CheckBox ckbUserHexEditor;
        private System.Windows.Forms.Button btnHexEditorBrowse;
        private System.Windows.Forms.OpenFileDialog ofdUserEditor;
        private System.Windows.Forms.CheckBox ckbHexEditorTS;
        private System.Windows.Forms.CheckBox ckbHexQuotes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tlpHelpers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TextBox tbUserTextEditor;
        private System.Windows.Forms.CheckBox ckbUserTextEditor;
        private System.Windows.Forms.Button btnTextEditorBrowse;
        private System.Windows.Forms.CheckBox ckbTextEditorTS;
        private System.Windows.Forms.CheckBox ckbTextQuotes;
    }
}