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
using System.Linq;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using s4pi.Interfaces;
using s4pi.Resource.Commons.Extensions;
using s4pi.Resource.Commons.Forms;

namespace s4pi.Resource.Commons.CatalogTags
{
	/// <summary>
	/// Represents a category tag that contains index and value.
	/// </summary>
	[TypeConverter(typeof(TagTypeConverter))]
	[Editor(typeof(TagTypeEditor), typeof(UITypeEditor))]
	public class Tag
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Tag"/> class.
		/// </summary>
		public Tag()
		{
			this.Value = "unknown";
		}

		/// <summary>
		/// Gets or sets the index of this tag.
		/// </summary>
		[XmlAttribute(AttributeName = "ev")]
		public uint Index { get; set; }

		/// <summary>
		/// Gets or sets the human-readable value of this tag.
		/// </summary>
		[XmlText]
		public string Value { get; set; }

        /// <summary>
        /// Converts the index of this tag to uint.
        /// </summary>
	    public uint ToUInt32()
	    {
	        return this.Index;
	    }

	    /// <summary>
	    /// Converts the index of this tag to ushort.
	    /// </summary>
        public ushort ToUInt16()
	    {
	        return (ushort)this.Index;
	    }

		#region Overrides of System.Object

		public override string ToString()
		{
			return string.Format("0x{0:X4} {1}", this.Index, this.Value);
		}

		public override bool Equals(object obj)
		{
			var other = obj as Tag;
			if (other != null)
			{
				return this.Index == other.Index;
			}

			return false;
		}

		public static bool operator ==(Tag tag1, Tag tag2)
		{
			if (!object.ReferenceEquals(tag1, null))
			{
				return tag1.Equals(tag2);
			}
			return object.ReferenceEquals(tag2, null);
		}

		public static bool operator !=(Tag tag1, Tag tag2)
		{
			return !(tag1 == tag2);
		}

		public override int GetHashCode()
		{
			return this.Index.GetHashCode();
		}

		#endregion

		private class TagTypeConverter : TypeConverter
		{
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return typeof(string) == destinationType;
			}

			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType ==  typeof(string))
				{
					var typedValue = value as TypedValue;
					if (typedValue != null)
					{
						var tag = typedValue.Value as Tag;
						if (tag != null)
						{
							return tag.Value;
						}
					}
				}

				return "unknown";
			}
		}

		private class TagTypeEditor : UITypeEditor
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

				Tag currentlySelectedTag = ExtractTag(value);
				Tag currentCategory = GetCurrentCategory(currentlySelectedTag);

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

				return listBox.SecondarySelectedItem ?? value;
			}

			#region Internals

			private static Tag ExtractTag(object value)
			{
				TypedValue typedValue = (TypedValue)value;
				Tag currentlySelectedTag = (Tag)typedValue.Value;

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
				return CatalogTagRegistry.AllCategoriesWithDummiesForUnpairedTags().Order().ToArray();
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
