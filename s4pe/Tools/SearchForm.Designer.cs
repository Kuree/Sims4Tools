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

namespace S4PIDemoFE.Tools
{
    partial class SearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.btnClose = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tbHex = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.lbxHits = new System.Windows.Forms.ListBox();
            this.searchContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyResourceKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteResourceKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCopy = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.cbType = new System.Windows.Forms.ResourceTypeCombo();
            this.ckbFilter = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.searchContextMenuStrip.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(714, 424);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tbHex, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBox1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(777, 32);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tbHex
            // 
            this.tbHex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbHex.Location = new System.Drawing.Point(110, 4);
            this.tbHex.Name = "tbHex";
            this.tbHex.Size = new System.Drawing.Size(583, 20);
            this.tbHex.TabIndex = 2;
            this.tbHex.TextChanged += new System.EventHandler(this.tbHex_TextChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(699, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Little-endian",
            "Big-endian"});
            this.comboBox1.Location = new System.Drawing.Point(3, 4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(101, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // lbxHits
            // 
            this.lbxHits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxHits.ContextMenuStrip = this.searchContextMenuStrip;
            this.lbxHits.FormattingEnabled = true;
            this.lbxHits.IntegralHeight = false;
            this.lbxHits.Location = new System.Drawing.Point(12, 182);
            this.lbxHits.Name = "lbxHits";
            this.lbxHits.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbxHits.Size = new System.Drawing.Size(777, 236);
            this.lbxHits.TabIndex = 2;
            this.lbxHits.SelectedIndexChanged += new System.EventHandler(this.lbxHits_SelectedIndexChanged);
            this.lbxHits.DoubleClick += new System.EventHandler(this.lbxHits_DoubleClick);
            // 
            // searchContextMenuStrip
            // 
            this.searchContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyResourceKeyToolStripMenuItem,
            this.pasteResourceKeyToolStripMenuItem});
            this.searchContextMenuStrip.Name = "searchContextMenuStrip";
            this.searchContextMenuStrip.Size = new System.Drawing.Size(215, 48);
            this.searchContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.searchContextMenuStrip_Opening);
            // 
            // copyResourceKeyToolStripMenuItem
            // 
            this.copyResourceKeyToolStripMenuItem.Name = "copyResourceKeyToolStripMenuItem";
            this.copyResourceKeyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyResourceKeyToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.copyResourceKeyToolStripMenuItem.Text = "&Copy TGIs";
            this.copyResourceKeyToolStripMenuItem.Click += new System.EventHandler(this.copyResourceKeyToolStripMenuItem_Click);
            // 
            // pasteResourceKeyToolStripMenuItem
            // 
            this.pasteResourceKeyToolStripMenuItem.Name = "pasteResourceKeyToolStripMenuItem";
            this.pasteResourceKeyToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteResourceKeyToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.pasteResourceKeyToolStripMenuItem.Text = "&Paste Resource Type";
            this.pasteResourceKeyToolStripMenuItem.Click += new System.EventHandler(this.pasteResourceKeyToolStripMenuItem_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Enabled = false;
            this.btnCopy.Location = new System.Drawing.Point(12, 424);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 3;
            this.btnCopy.Text = "&Copy TGIs";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGo.Enabled = false;
            this.btnGo.Location = new System.Drawing.Point(93, 424);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(75, 23);
            this.btnGo.TabIndex = 4;
            this.btnGo.Text = "&Go to";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 450);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(801, 22);
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(109, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Visible = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ContextMenuStrip = this.searchContextMenuStrip;
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.label6, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.label8, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label10, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label11, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.label12, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.label13, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.label14, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.label15, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.label16, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label17, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.label18, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.label1, 2, 2);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 50);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(481, 80);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.label3, 3);
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 2);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(258, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Enter your search term in any of the following formats:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(119, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Space-separated bytes:";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(128, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "a b2 c";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(203, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(271, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Use the endian drop-down to indicate whether bytes are";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(110, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Bytes without spaces:";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(128, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "0AB20C";
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(74, 41);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(48, 13);
            this.label10.TabIndex = 0;
            this.label10.Text = "Integers:";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(128, 41);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "0xCB20A";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(203, 41);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(266, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Integers (0x prefixed hex strings) are always big endian.";
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(203, 54);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(166, 13);
            this.label13.TabIndex = 0;
            this.label13.Text = "Quoted text is always little endian.";
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(128, 54);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(69, 13);
            this.label14.TabIndex = 0;
            this.label14.Text = "\"Hello world\"";
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(44, 54);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(78, 13);
            this.label15.TabIndex = 0;
            this.label15.Text = "Quoted strings:";
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(39, 67);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(83, 13);
            this.label16.TabIndex = 0;
            this.label16.Text = "Unicode strings:";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(128, 67);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(66, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "U\"€éúíóáÁ\"";
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(203, 67);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(206, 13);
            this.label18.TabIndex = 0;
            this.label18.Text = "Unicode can be little endian or big endian.";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(203, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(275, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "in \"file order\" (little endian) or \"integer order\" (big endian).";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.label2, 2);
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(261, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Use the Resource Type filter below to limit the search:";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ContextMenuStrip = this.searchContextMenuStrip;
            this.tableLayoutPanel3.Controls.Add(this.cbType, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.ckbFilter, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(12, 136);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(372, 40);
            this.tableLayoutPanel3.TabIndex = 11;
            // 
            // cbType
            // 
            this.cbType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbType.Enabled = false;
            this.cbType.Location = new System.Drawing.Point(90, 16);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(279, 21);
            this.cbType.TabIndex = 3;
            this.cbType.Value = ((uint)(0u));
            // 
            // ckbFilter
            // 
            this.ckbFilter.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbFilter.AutoSize = true;
            this.ckbFilter.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckbFilter.Location = new System.Drawing.Point(3, 18);
            this.ckbFilter.Name = "ckbFilter";
            this.ckbFilter.Size = new System.Drawing.Size(81, 17);
            this.ckbFilter.TabIndex = 0;
            this.ckbFilter.Text = "Enable filter";
            this.ckbFilter.UseVisualStyleBackColor = true;
            this.ckbFilter.CheckedChanged += new System.EventHandler(this.ckbFilter_CheckedChanged);
            // 
            // SearchForm
            // 
            this.AcceptButton = this.btnSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(801, 472);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.lbxHits);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnClose);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Package Search";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.searchContextMenuStrip.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox tbHex;
        private System.Windows.Forms.ListBox lbxHits;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip searchContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyResourceKeyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteResourceKeyToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.CheckBox ckbFilter;
        private System.Windows.Forms.ResourceTypeCombo cbType;
    }
}