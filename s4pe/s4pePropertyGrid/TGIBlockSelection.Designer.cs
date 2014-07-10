namespace S4PIDemoFE
{
    partial class TGIBlockSelection
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
            this.tgiBlockCombo1 = new System.Windows.Forms.TGIBlockCombo();
            this.SuspendLayout();
            // 
            // tgiBlockCombo1
            // 
            this.tgiBlockCombo1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tgiBlockCombo1.Location = new System.Drawing.Point(0, 0);
            this.tgiBlockCombo1.Margin = new System.Windows.Forms.Padding(0);
            this.tgiBlockCombo1.Name = "tgiBlockCombo1";
            this.tgiBlockCombo1.Size = new System.Drawing.Size(632, 21);
            this.tgiBlockCombo1.TabIndex = 0;
            this.tgiBlockCombo1.TGIBlocks = null;
            this.tgiBlockCombo1.SelectedIndexChanged += new System.EventHandler(this.tgiBlockCombo1_SelectedIndexChanged);
            this.tgiBlockCombo1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tgiBlockCombo1_KeyPress);
            // 
            // TGIBlockSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tgiBlockCombo1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TGIBlockSelection";
            this.Size = new System.Drawing.Size(632, 21);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TGIBlockCombo tgiBlockCombo1;

    }
}
