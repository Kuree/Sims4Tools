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
