using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace S4PIDemoFE.Settings
{
	public partial class OrganiseCustomPlacesDialog : Form
	{
		public OrganiseCustomPlacesDialog()
		{
			this.InitializeComponent();
			this.Icon = ((Icon)(new ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));

		    StringCollection customPlaces = Properties.Settings.Default.CustomPlaces;
		    if (customPlaces == null)
			{
				this.listBox1.Items.Clear();
				Properties.Settings.Default.CustomPlaces = new StringCollection();
				this.btnAdd.Enabled = true;
			}
			else
			{
                this.Populate();
				this.listBox1.SelectedIndex = 0;
				this.btnAdd.Enabled = customPlaces.Count < Properties.Settings.Default.CustomPlacesCount;
			}

			this.numericUpDown1.Value = Properties.Settings.Default.CustomPlacesCount;
		}

		private void Populate()
		{
			this.listBox1.Items.Clear();
			for (int i = 0; i < Properties.Settings.Default.CustomPlaces.Count; i++)
			{
				string s = Properties.Settings.Default.CustomPlaces[i];
				this.listBox1.Items.Add((i + 1) + ". " + s);
			}
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.btnMoveUp.Enabled = this.listBox1.SelectedIndex > 0;
			this.btnMoveDn.Enabled = this.listBox1.SelectedIndex >= 0 && this.listBox1.SelectedIndex < this.listBox1.Items.Count - 1;
			this.btnDelete.Enabled = this.listBox1.SelectedIndex >= 0;
		}

		private void btnMoveUp_Click(object sender, EventArgs e)
		{
			this.MoveBookmark(-1);
		}

		private void btnMoveDn_Click(object sender, EventArgs e)
		{
			this.MoveBookmark(+1);
		}

		void MoveBookmark(int direction)
		{
			int index = this.listBox1.SelectedIndex;
			string s = Properties.Settings.Default.CustomPlaces[index];
			Properties.Settings.Default.CustomPlaces.RemoveAt(index);
			this.listBox1.Items.RemoveAt(index);
			Properties.Settings.Default.CustomPlaces.Insert(index + direction, s);

			this.Populate();
			this.listBox1.SelectedIndex = Properties.Settings.Default.CustomPlaces.IndexOf(s);
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			DialogResult dr = this.folderBrowserDialog1.ShowDialog(this);

			if (dr != DialogResult.OK || string.IsNullOrEmpty(this.folderBrowserDialog1.SelectedPath))
			{
				return;
			}

			Properties.Settings.Default.CustomPlaces.Add(this.folderBrowserDialog1.SelectedPath);

			this.Populate();
			this.listBox1.SelectedIndex = Properties.Settings.Default.CustomPlaces.Count - 1;

			this.UpdateAddButton();
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			int index = this.listBox1.SelectedIndex;
			Properties.Settings.Default.CustomPlaces.RemoveAt(index);
			this.listBox1.Items.RemoveAt(index);

			this.Populate();
			this.listBox1.SelectedIndex = index == Properties.Settings.Default.CustomPlaces.Count ? Properties.Settings.Default.CustomPlaces.Count - 1 : index;
			this.UpdateAddButton();
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.CustomPlacesCount = (short)this.numericUpDown1.Value;
			if (Properties.Settings.Default.CustomPlaces.Count > Properties.Settings.Default.CustomPlacesCount)
			{
				if (this.listBox1.SelectedIndex >= Properties.Settings.Default.CustomPlacesCount)
				{
					this.listBox1.SelectedIndex = Properties.Settings.Default.CustomPlacesCount - 1;
				}
				while (Properties.Settings.Default.CustomPlaces.Count > Properties.Settings.Default.CustomPlacesCount)
				{
					Properties.Settings.Default.CustomPlaces.RemoveAt(Properties.Settings.Default.CustomPlaces.Count - 1);
				}
				int index = this.listBox1.SelectedIndex;
				this.Populate();
				this.listBox1.SelectedIndex = index;
			}
			this.UpdateAddButton();
		}

		private void UpdateAddButton()
		{
			this.btnAdd.Enabled = Properties.Settings.Default.CustomPlaces.Count < Properties.Settings.Default.CustomPlacesCount;
		}
	}
}