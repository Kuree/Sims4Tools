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
    partial class ManageWrappersDialog
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lvWrappers = new System.Windows.Forms.ListView();
            this.chaTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chaType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chaWrapper = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chaFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chaTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chaDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chaCompany = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chaProduct = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvDisabled = new System.Windows.Forms.ListView();
            this.chdTag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chdType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chdWrapper = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chdFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chdTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chdDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chdCompany = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chdProduct = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDisable = new System.Windows.Forms.Button();
            this.btnEnable = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chaOrder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chdOrder = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lvWrappers, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lvDisabled, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(907, 680);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(146, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.label1.Size = new System.Drawing.Size(117, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Available Wrappers";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(645, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.label2.Size = new System.Drawing.Size(114, 19);
            this.label2.TabIndex = 2;
            this.label2.Text = "Disabled Wrappers";
            // 
            // lvWrappers
            // 
            this.lvWrappers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chaOrder,
            this.chaTag,
            this.chaType,
            this.chaWrapper,
            this.chaFile,
            this.chaTitle,
            this.chaDescription,
            this.chaCompany,
            this.chaProduct});
            this.lvWrappers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvWrappers.FullRowSelect = true;
            this.lvWrappers.HideSelection = false;
            this.lvWrappers.Location = new System.Drawing.Point(3, 22);
            this.lvWrappers.Name = "lvWrappers";
            this.lvWrappers.ShowGroups = false;
            this.lvWrappers.Size = new System.Drawing.Size(404, 655);
            this.lvWrappers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvWrappers.TabIndex = 1;
            this.lvWrappers.UseCompatibleStateImageBehavior = false;
            this.lvWrappers.View = System.Windows.Forms.View.Details;
            this.lvWrappers.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lv_ColumnClick);
            this.lvWrappers.SelectedIndexChanged += new System.EventHandler(this.lvWrappers_SelectedIndexChanged);
            // 
            // chaTag
            // 
            this.chaTag.Text = "Tag";
            // 
            // chaType
            // 
            this.chaType.Text = "Type";
            // 
            // chaWrapper
            // 
            this.chaWrapper.Text = "Wrapper";
            // 
            // chaFile
            // 
            this.chaFile.Text = "File";
            // 
            // chaTitle
            // 
            this.chaTitle.Text = "Title";
            // 
            // chaDescription
            // 
            this.chaDescription.Text = "Description";
            // 
            // chaCompany
            // 
            this.chaCompany.Text = "Company";
            // 
            // chaProduct
            // 
            this.chaProduct.Text = "Product";
            // 
            // lvDisabled
            // 
            this.lvDisabled.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chdOrder,
            this.chdTag,
            this.chdType,
            this.chdWrapper,
            this.chdFile,
            this.chdTitle,
            this.chdDescription,
            this.chdCompany,
            this.chdProduct});
            this.lvDisabled.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvDisabled.FullRowSelect = true;
            this.lvDisabled.HideSelection = false;
            this.lvDisabled.Location = new System.Drawing.Point(500, 22);
            this.lvDisabled.Name = "lvDisabled";
            this.lvDisabled.ShowGroups = false;
            this.lvDisabled.Size = new System.Drawing.Size(404, 655);
            this.lvDisabled.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvDisabled.TabIndex = 3;
            this.lvDisabled.UseCompatibleStateImageBehavior = false;
            this.lvDisabled.View = System.Windows.Forms.View.Details;
            this.lvDisabled.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lv_ColumnClick);
            this.lvDisabled.SelectedIndexChanged += new System.EventHandler(this.lvWrappers_SelectedIndexChanged);
            // 
            // chdTag
            // 
            this.chdTag.Text = "Tag";
            // 
            // chdType
            // 
            this.chdType.Text = "Type";
            // 
            // chdWrapper
            // 
            this.chdWrapper.Text = "Wrapper";
            // 
            // chdFile
            // 
            this.chdFile.Text = "File";
            // 
            // chdTitle
            // 
            this.chdTitle.Text = "Title";
            // 
            // chdDescription
            // 
            this.chdDescription.Text = "Description";
            // 
            // chdCompany
            // 
            this.chdCompany.Text = "Company";
            // 
            // chdProduct
            // 
            this.chdProduct.Text = "Product";
            // 
            // panel1
            // 
            this.panel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.btnDisable);
            this.panel1.Controls.Add(this.btnEnable);
            this.panel1.Location = new System.Drawing.Point(413, 320);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(81, 58);
            this.panel1.TabIndex = 4;
            // 
            // btnDisable
            // 
            this.btnDisable.Enabled = false;
            this.btnDisable.Location = new System.Drawing.Point(3, 32);
            this.btnDisable.Name = "btnDisable";
            this.btnDisable.Size = new System.Drawing.Size(75, 23);
            this.btnDisable.TabIndex = 1;
            this.btnDisable.Text = "&Disable >>";
            this.btnDisable.UseVisualStyleBackColor = true;
            this.btnDisable.Click += new System.EventHandler(this.btnDisable_Click);
            // 
            // btnEnable
            // 
            this.btnEnable.Enabled = false;
            this.btnEnable.Location = new System.Drawing.Point(3, 3);
            this.btnEnable.Name = "btnEnable";
            this.btnEnable.Size = new System.Drawing.Size(75, 23);
            this.btnEnable.TabIndex = 0;
            this.btnEnable.Text = "<< &Enable";
            this.btnEnable.UseVisualStyleBackColor = true;
            this.btnEnable.Click += new System.EventHandler(this.btnEnable_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(844, 698);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(763, 698);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chaOrder
            // 
            this.chaOrder.Text = "Order";
            // 
            // chdOrder
            // 
            this.chdOrder.Text = "Order";
            // 
            // ManageWrappersDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(931, 733);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ManageWrappersDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ManageWrappers";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvWrappers;
        private System.Windows.Forms.ColumnHeader chaTag;
        private System.Windows.Forms.ColumnHeader chaType;
        private System.Windows.Forms.ColumnHeader chaWrapper;
        private System.Windows.Forms.ColumnHeader chaFile;
        private System.Windows.Forms.ListView lvDisabled;
        private System.Windows.Forms.ColumnHeader chdTag;
        private System.Windows.Forms.ColumnHeader chdType;
        private System.Windows.Forms.ColumnHeader chdWrapper;
        private System.Windows.Forms.ColumnHeader chdFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDisable;
        private System.Windows.Forms.Button btnEnable;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ColumnHeader chaTitle;
        private System.Windows.Forms.ColumnHeader chaDescription;
        private System.Windows.Forms.ColumnHeader chaCompany;
        private System.Windows.Forms.ColumnHeader chaProduct;
        private System.Windows.Forms.ColumnHeader chdTitle;
        private System.Windows.Forms.ColumnHeader chdDescription;
        private System.Windows.Forms.ColumnHeader chdCompany;
        private System.Windows.Forms.ColumnHeader chdProduct;
        private System.Windows.Forms.ColumnHeader chaOrder;
        private System.Windows.Forms.ColumnHeader chdOrder;
    }
}