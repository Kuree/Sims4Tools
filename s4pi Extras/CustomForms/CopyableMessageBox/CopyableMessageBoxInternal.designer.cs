namespace System.Windows.Forms
{
    partial class CopyableMessageBoxInternal
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
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.lbIcon = new System.Windows.Forms.Label();
            this.flpButtons.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpButtons
            // 
            this.flpButtons.Controls.Add(this.button1);
            this.flpButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flpButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flpButtons.Location = new System.Drawing.Point(0, 91);
            this.flpButtons.Name = "flpButtons";
            this.flpButtons.Size = new System.Drawing.Size(174, 42);
            this.flpButtons.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(90, 9);
            this.button1.Margin = new System.Windows.Forms.Padding(9);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tbMessage, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbIcon, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(174, 91);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tbMessage
            // 
            this.tbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbMessage.Location = new System.Drawing.Point(92, 12);
            this.tbMessage.Margin = new System.Windows.Forms.Padding(12);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.ReadOnly = true;
            this.tbMessage.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbMessage.Size = new System.Drawing.Size(70, 67);
            this.tbMessage.TabIndex = 0;
            this.tbMessage.Text = "Text here\r\nmore text\r\nmo\r\nre\r\ntext";
            this.tbMessage.SizeChanged += new System.EventHandler(this.ctl_SizeChanged);
            // 
            // lbIcon
            // 
            this.lbIcon.AutoSize = true;
            this.lbIcon.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.lbIcon.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbIcon.Font = new System.Drawing.Font("Lucida Console", 32F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbIcon.ForeColor = System.Drawing.Color.Blue;
            this.lbIcon.Location = new System.Drawing.Point(16, 16);
            this.lbIcon.Margin = new System.Windows.Forms.Padding(16);
            this.lbIcon.Name = "lbIcon";
            this.lbIcon.Size = new System.Drawing.Size(48, 45);
            this.lbIcon.TabIndex = 1;
            this.lbIcon.Text = "X";
            this.lbIcon.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // CopyableMessageBoxInternal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(174, 133);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.flpButtons);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "CopyableMessageBoxInternal";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CopyableMessageBox";
            this.Layout += new System.Windows.Forms.LayoutEventHandler(this.CopyableMessageBoxInternal_Layout);
            this.flpButtons.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox tbMessage;
        private System.Windows.Forms.Label lbIcon;
        private System.Windows.Forms.Button button1;

    }
}