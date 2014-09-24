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

namespace S4PIDemoFE.Filter
{
    partial class ResourceFilterWidget
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
            this.tlpResourceInfo = new System.Windows.Forms.TableLayoutPanel();
            this.tlpControls = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSet = new System.Windows.Forms.Button();
            this.btnRevise = new System.Windows.Forms.Button();
            this.ckbFilter = new System.Windows.Forms.CheckBox();
            this.btnQBE = new System.Windows.Forms.Button();
            this.btnPasteRK = new System.Windows.Forms.Button();
            this.lbCount = new System.Windows.Forms.Label();
            this.tlpResourceInfo.SuspendLayout();
            this.tlpControls.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpResourceInfo
            // 
            this.tlpResourceInfo.ColumnCount = 2;
            this.tlpResourceInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpResourceInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpResourceInfo.Controls.Add(this.tlpControls, 0, 0);
            this.tlpResourceInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpResourceInfo.Location = new System.Drawing.Point(0, 0);
            this.tlpResourceInfo.Name = "tlpResourceInfo";
            this.tlpResourceInfo.RowCount = 2;
            this.tlpResourceInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpResourceInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpResourceInfo.Size = new System.Drawing.Size(844, 150);
            this.tlpResourceInfo.TabIndex = 0;
            // 
            // tlpControls
            // 
            this.tlpControls.AutoSize = true;
            this.tlpControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpControls.ColumnCount = 4;
            this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpControls.Controls.Add(this.label1, 0, 0);
            this.tlpControls.Controls.Add(this.tableLayoutPanel1, 3, 0);
            this.tlpControls.Controls.Add(this.lbCount, 1, 0);
            this.tlpControls.Location = new System.Drawing.Point(3, 3);
            this.tlpControls.Name = "tlpControls";
            this.tlpControls.RowCount = 1;
            this.tlpControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpControls.Size = new System.Drawing.Size(285, 64);
            this.tlpControls.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Count: ";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnSet, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnRevise, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ckbFilter, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnQBE, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPasteRK, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(113, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(169, 58);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // btnSet
            // 
            this.btnSet.AutoSize = true;
            this.btnSet.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSet.Location = new System.Drawing.Point(127, 32);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(39, 23);
            this.btnSet.TabIndex = 5;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // btnRevise
            // 
            this.btnRevise.AutoSize = true;
            this.btnRevise.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRevise.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRevise.Location = new System.Drawing.Point(3, 3);
            this.btnRevise.Name = "btnRevise";
            this.btnRevise.Size = new System.Drawing.Size(50, 23);
            this.btnRevise.TabIndex = 1;
            this.btnRevise.Text = "Re&vise";
            this.btnRevise.UseVisualStyleBackColor = true;
            this.btnRevise.Click += new System.EventHandler(this.btnRevise_Click);
            // 
            // ckbFilter
            // 
            this.ckbFilter.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbFilter.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.ckbFilter, 2);
            this.ckbFilter.Location = new System.Drawing.Point(3, 35);
            this.ckbFilter.Name = "ckbFilter";
            this.ckbFilter.Size = new System.Drawing.Size(80, 17);
            this.ckbFilter.TabIndex = 4;
            this.ckbFilter.Text = "Filter &active";
            this.ckbFilter.UseVisualStyleBackColor = true;
            this.ckbFilter.CheckedChanged += new System.EventHandler(this.ckbFilter_CheckedChanged);
            // 
            // btnQBE
            // 
            this.btnQBE.AutoSize = true;
            this.btnQBE.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnQBE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnQBE.Location = new System.Drawing.Point(127, 3);
            this.btnQBE.Name = "btnQBE";
            this.btnQBE.Size = new System.Drawing.Size(39, 23);
            this.btnQBE.TabIndex = 3;
            this.btnQBE.Text = "Q&BE";
            this.btnQBE.UseVisualStyleBackColor = true;
            this.btnQBE.Click += new System.EventHandler(this.btnQBE_Click);
            // 
            // btnPasteRK
            // 
            this.btnPasteRK.AutoSize = true;
            this.btnPasteRK.Location = new System.Drawing.Point(59, 3);
            this.btnPasteRK.Name = "btnPasteRK";
            this.btnPasteRK.Size = new System.Drawing.Size(62, 23);
            this.btnPasteRK.TabIndex = 2;
            this.btnPasteRK.Text = "Paste R&K";
            this.btnPasteRK.UseVisualStyleBackColor = true;
            this.btnPasteRK.Click += new System.EventHandler(this.btnPasteRK_Click);
            // 
            // lbCount
            // 
            this.lbCount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbCount.AutoSize = true;
            this.lbCount.Location = new System.Drawing.Point(41, 25);
            this.lbCount.Margin = new System.Windows.Forms.Padding(0);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(49, 13);
            this.lbCount.TabIndex = 2;
            this.lbCount.Text = "nnnnnnn";
            // 
            // ResourceFilterWidget
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tlpResourceInfo);
            this.Name = "ResourceFilterWidget";
            this.Size = new System.Drawing.Size(844, 150);
            this.tlpResourceInfo.ResumeLayout(false);
            this.tlpResourceInfo.PerformLayout();
            this.tlpControls.ResumeLayout(false);
            this.tlpControls.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpResourceInfo;
        private System.Windows.Forms.CheckBox ckbFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.TableLayoutPanel tlpControls;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnSet;
        private System.Windows.Forms.Button btnRevise;
        private System.Windows.Forms.Button btnQBE;
        private System.Windows.Forms.Button btnPasteRK;
    }
}
