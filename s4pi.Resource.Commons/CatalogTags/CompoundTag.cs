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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Windows.Forms.Design;

using s4pi.Interfaces;
using s4pi.Resource.Commons.Extensions;
using s4pi.Resource.Commons.Forms;

namespace s4pi.Resource.Commons.CatalogTags
{
	/// <summary>
	/// A compound tag that consists of category and value tag.
	/// </summary>
	[TypeConverter(typeof(CompoundTagTypeConverter))]
	[Editor(typeof(CompoundTagTypeEditor), typeof(UITypeEditor))]
	public class CompoundTag
	{
		private Tag category;
		private Tag value;

		/// <summary>
		/// Gets or sets the category this compound tag belongs to.
		/// </summary>
		public Tag Category
		{
			get { return this.category ?? new Tag(); }
			set { this.category = value; }
		}

		/// <summary>
		/// Gets or sets the value of this compound tag.
		/// </summary>
		public Tag Value
		{
			get { return this.value ?? new Tag(); }
			set { this.value = value; }
		}

		#region Override of System.Object

		public override string ToString()
		{
			return string.Format("{0} - {1}", this.Category, this.Value);
		}

		public override bool Equals(object obj)
		{
			var other = obj as CompoundTag;
			if (other != null)
			{
				return this.Category == other.Category && this.Value == other.Value;
			}
			return false;
		}

		public override int GetHashCode()
		{
			// use GetHashCode implementation for anonymous types; 
			// it's way smarter than anything we can come up with
			return new { this.Category, this.Value }.GetHashCode();
		}

		public static bool operator ==(CompoundTag tag1, CompoundTag tag2)
		{
			if (!ReferenceEquals(tag1, null))
			{
				return tag1.Equals(tag2);
			}
			return ReferenceEquals(tag2, null);
		}

		public static bool operator !=(CompoundTag tag1, CompoundTag tag2)
		{
			return !(tag1 == tag2);
		}

		#endregion

		private class CompoundTagTypeConverter : TypeConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return typeof(string) == destinationType;
			}

			public override object ConvertTo(ITypeDescriptorContext context,
											 CultureInfo culture,
											 object value,
											 Type destinationType)
			{
				if (destinationType == typeof(string))
				{
					var typedValue = value as TypedValue;
					if (typedValue != null)
					{
						var tag = typedValue.Value as CompoundTag;
						if (tag != null)
						{
							return string.Format("{0} - {1}", tag.Category.Value, tag.Value.Value);
						}
					}
				}

				return "unknown";
			}
		}

		private class CompoundTagTypeEditor : UITypeEditor
		{
			public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			{
				return UITypeEditorEditStyle.DropDown;
			}

			public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

				var listBox = new DoubleListBox();

				Tag[] categories = FetchAllCategories();

				listBox.AssignFlattenedSecondaryList(CatalogTagRegistry.AllTags().ToObjectArray());

				CompoundTag currentlySelectedTag = ExtractTag(value);
				Tag currentCategory = currentlySelectedTag.Category;

				listBox.AssignPrimaryList(categories.ToObjectArray());
				AssignSelectedIndex(categories.ToObjectArray(), currentCategory, index => listBox.PrimarySelectedIndex = index);

				object[] availableTags = UpdateTagListBox(currentCategory, listBox);
				AssignSelectedIndex(availableTags, currentlySelectedTag, index => listBox.SecondarySelectedIndex = index);

				listBox.PrimaryItemSelected += (sender, args) =>
											   {
												   int selectedIndex = listBox.PrimarySelectedIndex;
												   Tag selectedCategory = categories[selectedIndex];

												   UpdateTagListBox(selectedCategory, listBox);
											   };

				listBox.SecondaryItemSelected += (sender, args) => editorService.CloseDropDown();

				editorService.DropDownControl(listBox);

				Tag newCategory = listBox.PrimarySelectedItem as Tag;
				Tag newValue = listBox.SecondarySelectedItem as Tag;

				if (newCategory != null && newValue != null)
				{
					return new CompoundTag { Category = newCategory, Value = newValue };
				}
				return value;
			}

			#region Internals

			private static CompoundTag ExtractTag(object value)
			{
				TypedValue typedValue = (TypedValue)value;
				CompoundTag currentlySelectedTag = (CompoundTag)typedValue.Value;

				return currentlySelectedTag;
			}

			private static object[] UpdateTagListBox(Tag category, DoubleListBox listBox)
			{
				object[] availableTags = FetchTagsForCategory(category).ToObjectArray();
				listBox.AssignSecondaryList(availableTags);

				return availableTags;
			}

			private static void AssignSelectedIndex(object[] pool, object value, Action<int> mutator)
			{
				int index = Array.IndexOf(pool, value);
				if (index != -1)
				{
					mutator(index);
				}
			}

			private static Tag[] FetchAllCategories()
			{
				return
					CatalogTagRegistry.AllCategoriesWithDummiesForUnpairedTags().Order().ToArray();
			}

			private static Tag GetCurrentCategory(Tag currentlySelectedTag)
			{
				return CatalogTagRegistry.GetCategoryFor(currentlySelectedTag);
			}

			private static IEnumerable<Tag> FetchTagsForCategory(Tag category)
			{
				return CatalogTagRegistry.FetchTagsForCategory(category.Value).Order().ToArray();
			}

			#endregion
		}
	}
}
