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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace System.Windows.Forms
{
    internal partial class CopyableMessageBoxInternal : Form
    {
        internal Button theButton = null;
        private CopyableMessageBoxInternal()
        {
            InitializeComponent();
        }

        internal CopyableMessageBoxInternal(string message, string caption, CopyableMessageBoxIcon icon, IList<string> buttons, int defBtn, int cncBtn)
            : this()
        {
            if (buttons.Count < 1)
                throw new ArgumentLengthException("At least one button text must be supplied");

            this.tbMessage.Lines = (message + "\n").Split('\n');
            this.Text = caption;
            enumToGlyph(icon, lbIcon);
            CreateButtons(buttons, defBtn, cncBtn);

            this.DialogResult = DialogResult.OK;
        }

        protected override bool ScaleChildren { get { return false; } }

        Size ctlSize = new Size();
        void CopyableMessageBoxInternal_Layout(object sender, LayoutEventArgs e)
        {
            if (!ctlSize.IsEmpty) return;

            this.SuspendLayout();

            // Calculate our maximum work area
            Size winSize = Screen.PrimaryScreen.WorkingArea.Size;

            // Reduce to 80% of width and height
            winSize.Width = winSize.Width * 4 / 5;
            winSize.Height = winSize.Height * 4 / 5;

            // Now see how big the owning form is, if there is one - avoid its size by +/-10%
            if (CopyableMessageBox.OwningForm != null)
            {
                if (winSize.Width > CopyableMessageBox.OwningForm.Width * 0.9f && winSize.Width < CopyableMessageBox.OwningForm.Width * 1.1f)
                    winSize.Width = winSize.Width * 4 / 5;
                if (winSize.Height > CopyableMessageBox.OwningForm.Height * 0.9f && winSize.Height < CopyableMessageBox.OwningForm.Height * 1.1f)
                    winSize.Height = winSize.Height * 4 / 5;
            }

            // Now determine how much estate is lost to the form border
            int formWidth = Bounds.Width - ClientSize.Width;
            int formHeight = Bounds.Height - ClientSize.Height;

            // Space for the icon, if present
            lbIcon.PerformLayout();
            Size iconSize = lbIcon.Visible ? lbIcon.PreferredSize : new Size(0, 0);

            flpButtons.PerformLayout();
            flpButtons.ResumeLayout(true);
            Size btnSize = flpButtons.PreferredSize;

            // So, remaining size...
            int maxWidth = winSize.Width - formWidth - iconSize.Width;
            int maxHeight = winSize.Height - formHeight - btnSize.Height;

            // To calculate our text box size, we get an autosize TextBox to tell us how big it should be
            TextBox tb = new TextBox
            {
                AutoSize = true,
                Font = tbMessage.Font,
                Margin = new Padding(0),
                MaximumSize = new Size(maxWidth, maxHeight),
                Multiline = true,
                Lines = this.tbMessage.Lines,
                Location = new Point(this.Width, this.Height),
            };
            this.Controls.Add(tb);
            this.ResumeLayout(true);

            tbMessage.ClientSize = new Size(tb.PreferredSize.Width > maxWidth ? maxWidth : tb.PreferredSize.Width,
                tb.PreferredSize.Height > maxHeight ? maxHeight : tb.PreferredSize.Height);
            tbMessage.PerformLayout();
            tbMessage.ResumeLayout(true);

            int minWidth = Math.Max(btnSize.Width, iconSize.Width) + formWidth;
            int minHeight = btnSize.Height + iconSize.Height + formHeight;
            this.MinimumSize = new Size(minWidth, minHeight);

            int clientWidth = Math.Min(winSize.Width - formWidth, Math.Max(btnSize.Width, tableLayoutPanel1.PreferredSize.Width));
            int clientHeight = Math.Min(winSize.Height - formHeight, btnSize.Height + tableLayoutPanel1.PreferredSize.Height);
            ctlSize = new Size(clientWidth, clientHeight);
            this.ClientSize = ctlSize;

            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Size.Width - Size.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Size.Height - Size.Height) / 2);
            this.Controls.Remove(tb);
        }

        private void enumToGlyph(CopyableMessageBoxIcon icon, Label lb)
        {
            switch (icon)
            {
                case CopyableMessageBoxIcon.Information:
                    lb.Visible = true; lb.Text = "i"; lb.ForeColor = Color.Blue; lb.BackColor = Color.FromArgb(240, 240, 255); lb.Font = new Font(lb.Font, FontStyle.Italic); break;
                case CopyableMessageBoxIcon.Question:
                    lb.Visible = true; lb.Text = "?"; lb.ForeColor = Color.Green; lb.BackColor = Color.FromArgb(240, 255, 240); lb.Font = new Font(lb.Font, FontStyle.Regular); break;
                case CopyableMessageBoxIcon.Warning:
                    lb.Visible = true; lb.Text = "!"; lb.ForeColor = Color.Black; lb.BackColor = Color.Yellow; lb.Font = new Font(lb.Font, FontStyle.Bold); break;
                case CopyableMessageBoxIcon.Error:
                    lb.Visible = true; lb.Text = "X"; lb.ForeColor = Color.White; lb.BackColor = Color.Red; lb.Font = new Font(lb.Font, FontStyle.Bold); break;
                case CopyableMessageBoxIcon.None:
                default:
                    lb.Visible = false; break;
            }
        }

        private void CreateButtons(IList<string> buttons, int defBtn, int cncBtn)
        {
            flpButtons.SuspendLayout();
            flpButtons.Controls.Clear();
            for (int i = buttons.Count; i > 0; i--)
            {
                Button btn = CreateButton("button" + i, i, buttons[i - 1]);
                flpButtons.Controls.Add(btn);
                if (i == defBtn + 1) this.AcceptButton = btn;
                if (i == cncBtn + 1) this.CancelButton = btn;
            }
            flpButtons.ResumeLayout(true);

            int minW = 75;
            int minH = 23;
            foreach (var ctrl in flpButtons.Controls)
            {
                Button btn = ctrl as Button;
                if (btn == null) continue;
                minW = Math.Max(minW, btn.Width);
                minH = Math.Max(minH, btn.Height);
            }
            foreach (var ctrl in flpButtons.Controls)
            {
                Button btn = ctrl as Button;
                if (btn == null) continue;
                btn.Size = new Size(minW, minH);
            }
        }

        private Button CreateButton(string Name, int TabIndex, string Text)
        {
            Button newButton = new Button()
            {
                Anchor = System.Windows.Forms.AnchorStyles.None,
                Margin = new Forms.Padding(9),
                MinimumSize = new System.Drawing.Size(75, 23),
                UseVisualStyleBackColor = true,
                Name = Name,
                TabIndex = TabIndex,
                Text = Text,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly,
            };
            newButton.Click += new System.EventHandler(this.button_Click);
            return newButton;
        }

        private void button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;//To avoid it becoming Cancel
            theButton = sender as Button;
            this.Close();
        }

        private void ctl_SizeChanged(object sender, EventArgs e)
        {
            if (ctlSize.IsEmpty) return;

            tbMessage.ScrollBars = ClientSize.Height < ctlSize.Height || ClientSize.Width < ctlSize.Width ? ScrollBars.Vertical : ScrollBars.None;
        }
    }
}
