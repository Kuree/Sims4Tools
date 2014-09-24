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

namespace S4PIDemoFE.Settings
{
    public partial class OrganiseBookmarksDialog : Form
    {
        public OrganiseBookmarksDialog()
        {
            InitializeComponent();
            this.Icon = ((System.Drawing.Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));

            if (S4PIDemoFE.Properties.Settings.Default.Bookmarks == null)
            {
                listBox1.Items.Clear();
                S4PIDemoFE.Properties.Settings.Default.Bookmarks = new System.Collections.Specialized.StringCollection();
                btnAdd.Enabled = true;
            }
            else
            {
                Populate();
                listBox1.SelectedIndex = 0;
                btnAdd.Enabled = S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count < S4PIDemoFE.Properties.Settings.Default.BookmarkSize;
            }
            numericUpDown1.Value = S4PIDemoFE.Properties.Settings.Default.BookmarkSize;
        }

        private void OrganiseBookmarksDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count == 0)
                S4PIDemoFE.Properties.Settings.Default.Bookmarks = null;
        }

        void Populate()
        {
            listBox1.Items.Clear();
            for (int i = 0; i < S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count; i++)
            {
                string s = S4PIDemoFE.Properties.Settings.Default.Bookmarks[i];
                if (s.StartsWith("0:") || s.StartsWith("1:"))
                    s = (s.StartsWith("0:") ? "[RO] " : "[RW] ") + s.Substring(2);
                listBox1.Items.Add((i + 1) + ". " + s);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnMoveUp.Enabled = listBox1.SelectedIndex > 0;
            btnMoveDn.Enabled = listBox1.SelectedIndex >= 0 && listBox1.SelectedIndex < listBox1.Items.Count - 1;
            btnDelete.Enabled = listBox1.SelectedIndex >= 0;
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            MoveBookmark(-1);
        }

        private void btnMoveDn_Click(object sender, EventArgs e)
        {
            MoveBookmark(+1);
        }

        void MoveBookmark(int direction)
        {
            int index = listBox1.SelectedIndex;
            string s = S4PIDemoFE.Properties.Settings.Default.Bookmarks[index];
            S4PIDemoFE.Properties.Settings.Default.Bookmarks.RemoveAt(index);
            listBox1.Items.RemoveAt(index);
            S4PIDemoFE.Properties.Settings.Default.Bookmarks.Insert(index + direction, s);

            Populate();
            listBox1.SelectedIndex = S4PIDemoFE.Properties.Settings.Default.Bookmarks.IndexOf(s);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "*.package";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            foreach (string s in openFileDialog1.FileNames)
            {
                if (S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count >= S4PIDemoFE.Properties.Settings.Default.BookmarkSize)
                {
                    btnAdd.Enabled = false;
                    break;
                }
                S4PIDemoFE.Properties.Settings.Default.Bookmarks.Add((openFileDialog1.ReadOnlyChecked ? "0:" : "1:") + s);
            }

            Populate();
            listBox1.SelectedIndex = S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count - 1;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int index = listBox1.SelectedIndex;
            S4PIDemoFE.Properties.Settings.Default.Bookmarks.RemoveAt(index);
            listBox1.Items.RemoveAt(index);

            Populate();
            listBox1.SelectedIndex = index == S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count ? S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count - 1 : index;
            btnAdd.Enabled = S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count < S4PIDemoFE.Properties.Settings.Default.BookmarkSize;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            S4PIDemoFE.Properties.Settings.Default.BookmarkSize = (short)numericUpDown1.Value;
            if (S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count > S4PIDemoFE.Properties.Settings.Default.BookmarkSize)
            {
                if (listBox1.SelectedIndex >= S4PIDemoFE.Properties.Settings.Default.BookmarkSize)
                    listBox1.SelectedIndex = S4PIDemoFE.Properties.Settings.Default.BookmarkSize - 1;
                while (S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count > S4PIDemoFE.Properties.Settings.Default.BookmarkSize)
                    S4PIDemoFE.Properties.Settings.Default.Bookmarks.RemoveAt(S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count - 1);
                int index = listBox1.SelectedIndex;
                Populate();
                listBox1.SelectedIndex = index;
            }
            btnAdd.Enabled = S4PIDemoFE.Properties.Settings.Default.Bookmarks.Count < S4PIDemoFE.Properties.Settings.Default.BookmarkSize;
        }
    }
}
