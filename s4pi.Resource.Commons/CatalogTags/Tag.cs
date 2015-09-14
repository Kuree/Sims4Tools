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
		public ushort Index { get; set; }

		/// <summary>
		/// Gets or sets the human-readable value of this tag.
		/// </summary>
		[XmlText]
		public string Value { get; set; }

		/// <summary>
		/// Converts the index of this tag to ushort.
		/// </summary>
		public static implicit operator UInt16(Tag tag)
		{
			return tag.Index;
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
			if (!ReferenceEquals(tag1, null))
			{
				return tag1.Equals(tag2);
			}
			return ReferenceEquals(tag2, null);
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
