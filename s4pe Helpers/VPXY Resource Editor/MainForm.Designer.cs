namespace s3pe_VPXY_Resource_Editor
{
    partial class MainForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnAddPart = new System.Windows.Forms.Button();
            this.btnAddLinked = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tlpParts = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tlpLPControls = new System.Windows.Forms.TableLayoutPanel();
            this.btnLPDown = new System.Windows.Forms.Button();
            this.btnLPUp = new System.Windows.Forms.Button();
            this.btnLPAdd = new System.Windows.Forms.Button();
            this.btnLPDel = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tlpLinkedParts = new System.Windows.Forms.TableLayoutPanel();
            this.lbLPTitle = new System.Windows.Forms.Label();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.nudLowerX = new System.Windows.Forms.NumericUpDown();
            this.nudLowerY = new System.Windows.Forms.NumericUpDown();
            this.nudUpperX = new System.Windows.Forms.NumericUpDown();
            this.nudUpperY = new System.Windows.Forms.NumericUpDown();
            this.nudUpperZ = new System.Windows.Forms.NumericUpDown();
            this.nudLowerZ = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.ckbModular = new System.Windows.Forms.CheckBox();
            this.tbcFTPT = new System.Windows.Forms.TGIBlockCombo();
            this.lbCurrentPart = new System.Windows.Forms.Label();
            this.lbLPCurrent = new System.Windows.Forms.Label();
            this.btnEditTGIs = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tlpParts.SuspendLayout();
            this.tlpLPControls.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tlpLinkedParts.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerZ)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(661, 346);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "A&bandon";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(742, 346);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "&Save";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tlpLPControls, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel5, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel6, 2, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(805, 328);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btnMoveDown, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnMoveUp, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnAddPart, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.btnAddLinked, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.btnDel, 0, 6);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(388, 10);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 7;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(98, 133);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnMoveDown.AutoSize = true;
            this.btnMoveDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMoveDown.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnMoveDown.Location = new System.Drawing.Point(0, 22);
            this.btnMoveDown.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(25, 22);
            this.btnMoveDown.TabIndex = 1;
            this.btnMoveDown.Text = "ê";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnMoveUp.AutoSize = true;
            this.btnMoveUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMoveUp.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnMoveUp.Location = new System.Drawing.Point(0, 0);
            this.btnMoveUp.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(25, 22);
            this.btnMoveUp.TabIndex = 0;
            this.btnMoveUp.Text = "é";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnAddPart
            // 
            this.btnAddPart.AutoSize = true;
            this.btnAddPart.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddPart.Location = new System.Drawing.Point(0, 64);
            this.btnAddPart.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddPart.Name = "btnAddPart";
            this.btnAddPart.Size = new System.Drawing.Size(58, 23);
            this.btnAddPart.TabIndex = 2;
            this.btnAddPart.Text = "Add Part";
            this.btnAddPart.UseVisualStyleBackColor = true;
            this.btnAddPart.Click += new System.EventHandler(this.btnAddPart_Click);
            // 
            // btnAddLinked
            // 
            this.btnAddLinked.AutoSize = true;
            this.btnAddLinked.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddLinked.Enabled = false;
            this.btnAddLinked.Location = new System.Drawing.Point(0, 87);
            this.btnAddLinked.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddLinked.Name = "btnAddLinked";
            this.btnAddLinked.Size = new System.Drawing.Size(98, 23);
            this.btnAddLinked.TabIndex = 3;
            this.btnAddLinked.Text = "Add Linked Parts";
            this.btnAddLinked.UseVisualStyleBackColor = true;
            this.btnAddLinked.Click += new System.EventHandler(this.btnAddLinked_Click);
            // 
            // btnDel
            // 
            this.btnDel.AutoSize = true;
            this.btnDel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnDel.Location = new System.Drawing.Point(0, 110);
            this.btnDel.Margin = new System.Windows.Forms.Padding(0);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(70, 23);
            this.btnDel.TabIndex = 4;
            this.btnDel.Text = "Delete Part";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tlpParts);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 154);
            this.panel1.TabIndex = 1;
            // 
            // tlpParts
            // 
            this.tlpParts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpParts.AutoSize = true;
            this.tlpParts.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpParts.ColumnCount = 3;
            this.tlpParts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpParts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpParts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpParts.Controls.Add(this.label1, 1, 0);
            this.tlpParts.Location = new System.Drawing.Point(0, 0);
            this.tlpParts.Margin = new System.Windows.Forms.Padding(0);
            this.tlpParts.Name = "tlpParts";
            this.tlpParts.RowCount = 2;
            this.tlpParts.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpParts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpParts.Size = new System.Drawing.Size(387, 13);
            this.tlpParts.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.tlpParts.SetColumnSpan(this.label1, 2);
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(148, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Parts (in order)";
            // 
            // tlpLPControls
            // 
            this.tlpLPControls.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tlpLPControls.AutoSize = true;
            this.tlpLPControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpLPControls.ColumnCount = 1;
            this.tlpLPControls.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLPControls.Controls.Add(this.btnLPDown, 0, 1);
            this.tlpLPControls.Controls.Add(this.btnLPUp, 0, 0);
            this.tlpLPControls.Controls.Add(this.btnLPAdd, 0, 3);
            this.tlpLPControls.Controls.Add(this.btnLPDel, 0, 4);
            this.tlpLPControls.Enabled = false;
            this.tlpLPControls.Location = new System.Drawing.Point(388, 196);
            this.tlpLPControls.Margin = new System.Windows.Forms.Padding(0);
            this.tlpLPControls.Name = "tlpLPControls";
            this.tlpLPControls.RowCount = 5;
            this.tlpLPControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLPControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLPControls.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpLPControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLPControls.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLPControls.Size = new System.Drawing.Size(48, 110);
            this.tlpLPControls.TabIndex = 5;
            // 
            // btnLPDown
            // 
            this.btnLPDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnLPDown.AutoSize = true;
            this.btnLPDown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLPDown.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnLPDown.Location = new System.Drawing.Point(0, 22);
            this.btnLPDown.Margin = new System.Windows.Forms.Padding(0);
            this.btnLPDown.Name = "btnLPDown";
            this.btnLPDown.Size = new System.Drawing.Size(25, 22);
            this.btnLPDown.TabIndex = 1;
            this.btnLPDown.Text = "ê";
            this.btnLPDown.UseVisualStyleBackColor = true;
            this.btnLPDown.Click += new System.EventHandler(this.btnLPDown_Click);
            // 
            // btnLPUp
            // 
            this.btnLPUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnLPUp.AutoSize = true;
            this.btnLPUp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLPUp.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnLPUp.Location = new System.Drawing.Point(0, 0);
            this.btnLPUp.Margin = new System.Windows.Forms.Padding(0);
            this.btnLPUp.Name = "btnLPUp";
            this.btnLPUp.Size = new System.Drawing.Size(25, 22);
            this.btnLPUp.TabIndex = 0;
            this.btnLPUp.Text = "é";
            this.btnLPUp.UseVisualStyleBackColor = true;
            this.btnLPUp.Click += new System.EventHandler(this.btnLPUp_Click);
            // 
            // btnLPAdd
            // 
            this.btnLPAdd.AutoSize = true;
            this.btnLPAdd.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLPAdd.Location = new System.Drawing.Point(0, 64);
            this.btnLPAdd.Margin = new System.Windows.Forms.Padding(0);
            this.btnLPAdd.Name = "btnLPAdd";
            this.btnLPAdd.Size = new System.Drawing.Size(36, 23);
            this.btnLPAdd.TabIndex = 4;
            this.btnLPAdd.Text = "Add";
            this.btnLPAdd.UseVisualStyleBackColor = true;
            this.btnLPAdd.Click += new System.EventHandler(this.btnLPAdd_Click);
            // 
            // btnLPDel
            // 
            this.btnLPDel.AutoSize = true;
            this.btnLPDel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnLPDel.Location = new System.Drawing.Point(0, 87);
            this.btnLPDel.Margin = new System.Windows.Forms.Padding(0);
            this.btnLPDel.Name = "btnLPDel";
            this.btnLPDel.Size = new System.Drawing.Size(48, 23);
            this.btnLPDel.TabIndex = 4;
            this.btnLPDel.Text = "Delete";
            this.btnLPDel.UseVisualStyleBackColor = true;
            this.btnLPDel.Click += new System.EventHandler(this.btnLPDel_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoScroll = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.tlpLinkedParts);
            this.panel2.Location = new System.Drawing.Point(0, 174);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(388, 154);
            this.panel2.TabIndex = 3;
            // 
            // tlpLinkedParts
            // 
            this.tlpLinkedParts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpLinkedParts.AutoSize = true;
            this.tlpLinkedParts.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpLinkedParts.ColumnCount = 3;
            this.tlpLinkedParts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpLinkedParts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpLinkedParts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLinkedParts.Controls.Add(this.lbLPTitle, 1, 0);
            this.tlpLinkedParts.Enabled = false;
            this.tlpLinkedParts.Location = new System.Drawing.Point(0, 0);
            this.tlpLinkedParts.Margin = new System.Windows.Forms.Padding(0);
            this.tlpLinkedParts.Name = "tlpLinkedParts";
            this.tlpLinkedParts.RowCount = 2;
            this.tlpLinkedParts.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpLinkedParts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpLinkedParts.Size = new System.Drawing.Size(387, 13);
            this.tlpLinkedParts.TabIndex = 1;
            // 
            // lbLPTitle
            // 
            this.lbLPTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lbLPTitle.AutoSize = true;
            this.tlpLinkedParts.SetColumnSpan(this.lbLPTitle, 2);
            this.lbLPTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLPTitle.Location = new System.Drawing.Point(127, 0);
            this.lbLPTitle.Name = "lbLPTitle";
            this.lbLPTitle.Size = new System.Drawing.Size(133, 13);
            this.lbLPTitle.TabIndex = 0;
            this.lbLPTitle.Text = "&Linked Parts (in order)";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel5.ColumnCount = 4;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel5.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.label4, 1, 1);
            this.tableLayoutPanel5.Controls.Add(this.label5, 2, 1);
            this.tableLayoutPanel5.Controls.Add(this.label6, 3, 1);
            this.tableLayoutPanel5.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.label8, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.nudLowerX, 1, 2);
            this.tableLayoutPanel5.Controls.Add(this.nudLowerY, 2, 2);
            this.tableLayoutPanel5.Controls.Add(this.nudUpperX, 1, 3);
            this.tableLayoutPanel5.Controls.Add(this.nudUpperY, 2, 3);
            this.tableLayoutPanel5.Controls.Add(this.nudUpperZ, 3, 3);
            this.tableLayoutPanel5.Controls.Add(this.nudLowerZ, 3, 2);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(486, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 5;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(319, 110);
            this.tableLayoutPanel5.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.tableLayoutPanel5.SetColumnSpan(this.label3, 4);
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(117, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "&Bounding box";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(81, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "X";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(173, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Y";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(265, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(14, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Z";
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 32);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(36, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Lower";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 58);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Upper";
            // 
            // nudLowerX
            // 
            this.nudLowerX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLowerX.DecimalPlaces = 8;
            this.nudLowerX.Location = new System.Drawing.Point(45, 29);
            this.nudLowerX.Maximum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            0});
            this.nudLowerX.Minimum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            -2147483648});
            this.nudLowerX.Name = "nudLowerX";
            this.nudLowerX.Size = new System.Drawing.Size(86, 20);
            this.nudLowerX.TabIndex = 2;
            this.nudLowerX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLowerX.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // nudLowerY
            // 
            this.nudLowerY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLowerY.DecimalPlaces = 8;
            this.nudLowerY.Location = new System.Drawing.Point(137, 29);
            this.nudLowerY.Maximum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            0});
            this.nudLowerY.Minimum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            -2147483648});
            this.nudLowerY.Name = "nudLowerY";
            this.nudLowerY.Size = new System.Drawing.Size(86, 20);
            this.nudLowerY.TabIndex = 3;
            this.nudLowerY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLowerY.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // nudUpperX
            // 
            this.nudUpperX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.nudUpperX.DecimalPlaces = 8;
            this.nudUpperX.Location = new System.Drawing.Point(45, 55);
            this.nudUpperX.Maximum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            0});
            this.nudUpperX.Minimum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            -2147483648});
            this.nudUpperX.Name = "nudUpperX";
            this.nudUpperX.Size = new System.Drawing.Size(86, 20);
            this.nudUpperX.TabIndex = 5;
            this.nudUpperX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudUpperX.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // nudUpperY
            // 
            this.nudUpperY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.nudUpperY.DecimalPlaces = 8;
            this.nudUpperY.Location = new System.Drawing.Point(137, 55);
            this.nudUpperY.Maximum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            0});
            this.nudUpperY.Minimum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            -2147483648});
            this.nudUpperY.Name = "nudUpperY";
            this.nudUpperY.Size = new System.Drawing.Size(86, 20);
            this.nudUpperY.TabIndex = 6;
            this.nudUpperY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudUpperY.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // nudUpperZ
            // 
            this.nudUpperZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.nudUpperZ.DecimalPlaces = 8;
            this.nudUpperZ.Location = new System.Drawing.Point(229, 55);
            this.nudUpperZ.Maximum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            0});
            this.nudUpperZ.Minimum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            -2147483648});
            this.nudUpperZ.Name = "nudUpperZ";
            this.nudUpperZ.Size = new System.Drawing.Size(87, 20);
            this.nudUpperZ.TabIndex = 7;
            this.nudUpperZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudUpperZ.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // nudLowerZ
            // 
            this.nudLowerZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.nudLowerZ.DecimalPlaces = 8;
            this.nudLowerZ.Location = new System.Drawing.Point(229, 29);
            this.nudLowerZ.Maximum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            0});
            this.nudLowerZ.Minimum = new decimal(new int[] {
            0,
            -2147483648,
            0,
            -2147483648});
            this.nudLowerZ.Name = "nudLowerZ";
            this.nudLowerZ.Size = new System.Drawing.Size(87, 20);
            this.nudLowerZ.TabIndex = 4;
            this.nudLowerZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudLowerZ.ValueChanged += new System.EventHandler(this.nud_ValueChanged);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel6.AutoSize = true;
            this.tableLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.ckbModular, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.tbcFTPT, 0, 1);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(486, 174);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 3;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(319, 154);
            this.tableLayoutPanel6.TabIndex = 7;
            // 
            // ckbModular
            // 
            this.ckbModular.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ckbModular.AutoSize = true;
            this.ckbModular.Location = new System.Drawing.Point(3, 3);
            this.ckbModular.Name = "ckbModular";
            this.ckbModular.Size = new System.Drawing.Size(70, 17);
            this.ckbModular.TabIndex = 1;
            this.ckbModular.Text = "&Modular?";
            this.ckbModular.UseVisualStyleBackColor = true;
            this.ckbModular.CheckedChanged += new System.EventHandler(this.ckbModular_CheckedChanged);
            // 
            // tbcFTPT
            // 
            this.tbcFTPT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbcFTPT.Enabled = false;
            this.tbcFTPT.Location = new System.Drawing.Point(0, 23);
            this.tbcFTPT.Margin = new System.Windows.Forms.Padding(0);
            this.tbcFTPT.Name = "tbcFTPT";
            this.tbcFTPT.ShowEdit = false;
            this.tbcFTPT.Size = new System.Drawing.Size(319, 21);
            this.tbcFTPT.TabIndex = 2;
            this.tbcFTPT.TGIBlocks = null;
            this.tbcFTPT.SelectedIndexChanged += new System.EventHandler(this.tbcFTPT_SelectedIndexChanged);
            this.tbcFTPT.TGIBlockListChanged += new System.EventHandler(this.tbg_TGIBlockListChanged);
            // 
            // lbCurrentPart
            // 
            this.lbCurrentPart.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbCurrentPart.AutoSize = true;
            this.lbCurrentPart.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lbCurrentPart.Location = new System.Drawing.Point(91, 352);
            this.lbCurrentPart.Name = "lbCurrentPart";
            this.lbCurrentPart.Size = new System.Drawing.Size(17, 12);
            this.lbCurrentPart.TabIndex = 0;
            this.lbCurrentPart.Text = "è";
            // 
            // lbLPCurrent
            // 
            this.lbLPCurrent.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbLPCurrent.AutoSize = true;
            this.lbLPCurrent.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lbLPCurrent.Location = new System.Drawing.Point(114, 352);
            this.lbLPCurrent.Name = "lbLPCurrent";
            this.lbLPCurrent.Size = new System.Drawing.Size(17, 12);
            this.lbLPCurrent.TabIndex = 0;
            this.lbLPCurrent.Text = "è";
            // 
            // btnEditTGIs
            // 
            this.btnEditTGIs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditTGIs.AutoSize = true;
            this.btnEditTGIs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnEditTGIs.Location = new System.Drawing.Point(9, 346);
            this.btnEditTGIs.Margin = new System.Windows.Forms.Padding(0);
            this.btnEditTGIs.Name = "btnEditTGIs";
            this.btnEditTGIs.Size = new System.Drawing.Size(79, 23);
            this.btnEditTGIs.TabIndex = 2;
            this.btnEditTGIs.Text = "TGI Blocks...";
            this.btnEditTGIs.UseVisualStyleBackColor = true;
            this.btnEditTGIs.Click += new System.EventHandler(this.btnEditTGIs_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(829, 381);
            this.Controls.Add(this.lbLPCurrent);
            this.Controls.Add(this.lbCurrentPart);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnEditTGIs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = global::s3pe_VPXY_Resource_Editor.Properties.Resources.s3pe;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "VPXY Resource Editor";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tlpParts.ResumeLayout(false);
            this.tlpParts.PerformLayout();
            this.tlpLPControls.ResumeLayout(false);
            this.tlpLPControls.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tlpLinkedParts.ResumeLayout(false);
            this.tlpLinkedParts.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpperZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLowerZ)).EndInit();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.TableLayoutPanel tlpParts;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tlpLPControls;
        private System.Windows.Forms.Button btnLPDown;
        private System.Windows.Forms.Button btnLPUp;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tlpLinkedParts;
        private System.Windows.Forms.Label lbLPTitle;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private System.Windows.Forms.CheckBox ckbModular;
        private System.Windows.Forms.TGIBlockCombo tbcFTPT;
        private System.Windows.Forms.NumericUpDown nudLowerZ;
        private System.Windows.Forms.NumericUpDown nudLowerX;
        private System.Windows.Forms.NumericUpDown nudLowerY;
        private System.Windows.Forms.NumericUpDown nudUpperX;
        private System.Windows.Forms.NumericUpDown nudUpperY;
        private System.Windows.Forms.NumericUpDown nudUpperZ;
        private System.Windows.Forms.Label lbCurrentPart;
        private System.Windows.Forms.Label lbLPCurrent;
        private System.Windows.Forms.Button btnAddPart;
        private System.Windows.Forms.Button btnAddLinked;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnLPAdd;
        private System.Windows.Forms.Button btnLPDel;
        private System.Windows.Forms.Button btnEditTGIs;
    }
}

