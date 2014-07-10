namespace s4pi.Filetable
{
    partial class GameFoldersForm
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
            this.tlpGameFolders = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnReset = new System.Windows.Forms.Button();
            this.tlpCustomContent = new System.Windows.Forms.TableLayoutPanel();
            this.ckbCustomContent = new System.Windows.Forms.CheckBox();
            this.tbCCFolder = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnCCEdit = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tlpGameFolders.SuspendLayout();
            this.tlpCustomContent.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpGameFolders
            // 
            this.tlpGameFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpGameFolders.AutoSize = true;
            this.tlpGameFolders.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpGameFolders.ColumnCount = 4;
            this.tlpGameFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpGameFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpGameFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpGameFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpGameFolders.Controls.Add(this.label1, 0, 0);
            this.tlpGameFolders.Controls.Add(this.label2, 1, 0);
            this.tlpGameFolders.Controls.Add(this.label3, 2, 0);
            this.tlpGameFolders.Controls.Add(this.label4, 3, 0);
            this.tlpGameFolders.Location = new System.Drawing.Point(12, 12);
            this.tlpGameFolders.Name = "tlpGameFolders";
            this.tlpGameFolders.RowCount = 2;
            this.tlpGameFolders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGameFolders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpGameFolders.Size = new System.Drawing.Size(604, 13);
            this.tlpGameFolders.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Game ID";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(66, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Enabled";
            this.label2.LocationChanged += new System.EventHandler(this.label2_LocationChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(125, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Install Folder";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(572, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Edit";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(541, 89);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Select the folder where your Sims3 game is installed.";
            this.folderBrowserDialog1.ShowNewFolderButton = false;
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.Location = new System.Drawing.Point(460, 89);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Revert";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tlpCustomContent
            // 
            this.tlpCustomContent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpCustomContent.ColumnCount = 4;
            this.tlpCustomContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tlpCustomContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpCustomContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpCustomContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpCustomContent.Controls.Add(this.ckbCustomContent, 1, 1);
            this.tlpCustomContent.Controls.Add(this.tbCCFolder, 2, 1);
            this.tlpCustomContent.Controls.Add(this.label6, 1, 0);
            this.tlpCustomContent.Controls.Add(this.label7, 2, 0);
            this.tlpCustomContent.Controls.Add(this.btnCCEdit, 3, 1);
            this.tlpCustomContent.Controls.Add(this.label12, 3, 0);
            this.tlpCustomContent.Controls.Add(this.label5, 0, 1);
            this.tlpCustomContent.Location = new System.Drawing.Point(12, 37);
            this.tlpCustomContent.Name = "tlpCustomContent";
            this.tlpCustomContent.RowCount = 3;
            this.tlpCustomContent.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomContent.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpCustomContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpCustomContent.Size = new System.Drawing.Size(604, 46);
            this.tlpCustomContent.TabIndex = 2;
            // 
            // ckbCustomContent
            // 
            this.ckbCustomContent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ckbCustomContent.AutoSize = true;
            this.ckbCustomContent.Location = new System.Drawing.Point(135, 19);
            this.ckbCustomContent.Name = "ckbCustomContent";
            this.ckbCustomContent.Size = new System.Drawing.Size(29, 17);
            this.ckbCustomContent.TabIndex = 1;
            this.ckbCustomContent.Text = " ";
            this.ckbCustomContent.UseVisualStyleBackColor = true;
            this.ckbCustomContent.CheckedChanged += new System.EventHandler(this.ckbCustomContent_CheckedChanged);
            // 
            // tbCCFolder
            // 
            this.tbCCFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCCFolder.Location = new System.Drawing.Point(182, 17);
            this.tbCCFolder.Name = "tbCCFolder";
            this.tbCCFolder.ReadOnly = true;
            this.tbCCFolder.Size = new System.Drawing.Size(378, 20);
            this.tbCCFolder.TabIndex = 3;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(123, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Enabled";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(182, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Install Folder";
            // 
            // btnCCEdit
            // 
            this.btnCCEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCCEdit.AutoSize = true;
            this.btnCCEdit.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCCEdit.Location = new System.Drawing.Point(566, 16);
            this.btnCCEdit.Name = "btnCCEdit";
            this.btnCCEdit.Size = new System.Drawing.Size(35, 23);
            this.btnCCEdit.TabIndex = 2;
            this.btnCCEdit.Text = "Edit";
            this.btnCCEdit.UseVisualStyleBackColor = true;
            this.btnCCEdit.Click += new System.EventHandler(this.btnCCEdit_Click);
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(569, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(29, 13);
            this.label12.TabIndex = 0;
            this.label12.Text = "Edit";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(21, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Custom Content";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "exe";
            this.openFileDialog1.FileName = "*.exe";
            this.openFileDialog1.Filter = "Program files|*.exe|All files|*.*";
            this.openFileDialog1.Title = "Locate package editor";
            // 
            // GameFoldersForm
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 124);
            this.Controls.Add(this.tlpGameFolders);
            this.Controls.Add(this.tlpCustomContent);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "GameFoldersForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Game Folders";
            this.Load += new System.EventHandler(this.GameFoldersForm_Load);
            this.tlpGameFolders.ResumeLayout(false);
            this.tlpGameFolders.PerformLayout();
            this.tlpCustomContent.ResumeLayout(false);
            this.tlpCustomContent.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpGameFolders;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TableLayoutPanel tlpCustomContent;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox ckbCustomContent;
        private System.Windows.Forms.Button btnCCEdit;
        private System.Windows.Forms.TextBox tbCCFolder;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}