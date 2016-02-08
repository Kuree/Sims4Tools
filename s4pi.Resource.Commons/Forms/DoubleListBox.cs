/***************************************************************************
 *  Copyright (C) 2015, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Buzzler                                                                *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s4pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace s4pi.Resource.Commons.Forms
{
	/// <summary>
	/// <see cref="UserControl"/> implementation with two <see cref="ListBox"/> instances, e.g. for two step selections.
	/// </summary>
	public class DoubleListBox : UserControl
	{
		private readonly ListBox primaryListBox;
		private readonly ComboBox secondaryListBox;

		private object[] secondaryItems;
		private string lastText;
		private int lastCaretPosition;
		private object[] flattenedItems;

		/// <summary>
		/// Initializes a new instance of the <see cref="DoubleListBox"/> class.
		/// </summary>
		public DoubleListBox()
		{
			this.primaryListBox = new ListBox { MinimumSize = new Size(200, 145), Enabled = false };
			this.secondaryListBox = new ComboBox { MinimumSize = new Size(240, 140), Size = new Size(240, 135), DropDownStyle = ComboBoxStyle.Simple, Enabled = false };

			var tablePanel = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 1, ColumnCount = 2 };

			var flowPanel1 = new FlowLayoutPanel { Dock = DockStyle.Fill };
			var flowPanel2 = new FlowLayoutPanel { Dock = DockStyle.Fill };

			tablePanel.Controls.Add(flowPanel1, 0, 0);
			tablePanel.Controls.Add(flowPanel2, 1, 0);

			flowPanel1.Controls.Add(this.primaryListBox);
			flowPanel2.Controls.Add(this.secondaryListBox);

			this.Controls.Add(tablePanel);

			this.primaryListBox.SelectedValueChanged += this.OnPrimarySelected;
			this.secondaryListBox.SelectedValueChanged += this.OnSecondarySelected;

			this.secondaryListBox.TextUpdate += (sender, args) => this.lastCaretPosition = this.secondaryListBox.SelectionStart;
			this.secondaryListBox.KeyPress += (sender, args) => this.lastCaretPosition = this.secondaryListBox.SelectionStart + 1;

			this.secondaryListBox.TextChanged += (sender, args) =>
												 {
													 string text = this.secondaryListBox.Text;
													 if (text == this.lastText)
													 {
														 return;
													 }

													 string[] itemTexts = this.secondaryItems.Select(o => o.ToString()).ToArray();

													 var matchingIndices = itemTexts.Select((s, i) => new { Value = s, Index = i })
																					.Where(e => e.Value.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1)
																					.Select(e => e.Index)
																					.ToArray();

													 if (matchingIndices.Length == 1)
													 {
														 // single match -> select item
														 this.secondaryListBox.Items.Clear();
														 this.secondaryListBox.Items.AddRange(this.secondaryItems);

														 this.lastText = this.secondaryItems[matchingIndices[0]].ToString();
														 this.secondaryListBox.SelectedIndex = matchingIndices[0];

														 this.OnSecondarySelected(this, args);
													 }
													 else if (matchingIndices.Length > 0)
													 {
														 // multiple matches -> filter list
														 var items = this.secondaryItems.Where((o, i) => matchingIndices.Contains(i)).ToArray();
														 this.secondaryListBox.Items.Clear();
														 this.secondaryListBox.Items.AddRange(items);
													 }
													 else
													 {
														 // try to match globally
														 var globalMatch = this.flattenedItems.SingleOrDefault(e => e.ToString().Equals(text, StringComparison.OrdinalIgnoreCase));
														 if (globalMatch != null)
														 {
															 // global match found -> select item
															 this.secondaryListBox.Items.Clear();
															 this.secondaryListBox.Items.AddRange(this.flattenedItems);

															 this.lastText = globalMatch.ToString();
															 this.secondaryListBox.SelectedItem = globalMatch;

															 this.OnSecondarySelected(this, args);
														 }
														 else
														 {
															 // reset list
															 this.secondaryListBox.Items.Clear();
															 this.secondaryListBox.Items.Add(this.secondaryItems);
														 }
													 }
													 
													 this.lastText = text;
													 this.secondaryListBox.SelectionStart = this.lastCaretPosition;
												 };

			this.primaryListBox.Enabled = true;
			this.secondaryListBox.Enabled = true;
		}

		/// <summary>
		/// Occurs when an element from the primary list gets selected.
		/// </summary>
		public event EventHandler PrimaryItemSelected;

		/// <summary>
		/// Occurs when an element from the secondary list gets selected.
		/// </summary>
		public event EventHandler SecondaryItemSelected;

		/// <summary>
		/// Assigns the content of the primary list box.
		/// </summary>
		public void AssignPrimaryList(object[] values)
		{
			this.primaryListBox.Items.Clear();
			this.primaryListBox.Items.AddRange(values);
		}

		/// <summary>
		/// Assigns the content of the secondary list box.
		/// </summary>
		public void AssignSecondaryList(object[] values)
		{
			this.secondaryItems = values;

			this.secondaryListBox.Items.Clear();
			this.secondaryListBox.Items.AddRange(values);
		}

		/// <summary>
		/// Assigns the flattened lookup list that contains all secondary items for each primary item.
		/// The lookup will be used to match inputs against that have no direct matches in the current
		/// secondary list.
		/// </summary>
		public void AssignFlattenedSecondaryList(object[] values)
		{
			this.flattenedItems = values;
		}

		/// <summary>
		/// Gets or sets the index of the selected item of the primary list box.
		/// </summary>
		public int PrimarySelectedIndex
		{
			get { return this.primaryListBox.SelectedIndex; }
			set { this.primaryListBox.SelectedIndex = value; }
		}

		/// <summary>
		/// Gets the selected item of the primary list box.
		/// </summary>
		public object PrimarySelectedItem
		{
			get { return this.primaryListBox.SelectedItem; }
		}

		/// <summary>
		/// Gets or sets the index of the selected item of the secondary list box.
		/// </summary>
		public int SecondarySelectedIndex
		{
			get { return this.secondaryListBox.SelectedIndex; }
			set { this.secondaryListBox.SelectedIndex = value; }
		}

		/// <summary>
		/// Gets the selected item of the secondary list box.
		/// </summary>
		public object SecondarySelectedItem
		{
			get { return this.secondaryListBox.SelectedItem; }
		}

		private void OnPrimarySelected(object sender, EventArgs e)
		{
			this.RaiseEvent(this.PrimaryItemSelected, e);
		}

		private void OnSecondarySelected(object sender, EventArgs e)
		{
			this.RaiseEvent(this.SecondaryItemSelected, e);
		}

		private void RaiseEvent(EventHandler eventHandler, EventArgs e)
		{
			var handler = eventHandler;
			if (handler != null)
			{
				handler(this, e);
			}
		}
	}
}
