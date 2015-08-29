using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace s4pi.Resource.Commons.CatalogTags
{
	/// <summary>
	/// Registry class that manages known catalog categories and tags.
	/// </summary>
	public class CatalogTagRegistry
	{
		private const string CatalogTuningFileName = "S4_03B33DDF_00000000_D89CB9186B79ACB7.xml";
		
		private static Dictionary<ushort, Tag> tags;
		private static Dictionary<ushort, Tag> categories;

		static CatalogTagRegistry()
		{
			ParseCategories();
		}

		private static void ParseCategories()
		{
			string executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (string.IsNullOrEmpty(executingPath))
			{
				throw new FileNotFoundException(string.Format("'{0}' not found in S4PE directory '{1}'.", CatalogTuningFileName, executingPath));
			}
			string resourcePath = Path.Combine(executingPath, CatalogTuningFileName);
			using (var stream = File.OpenRead(resourcePath))
			{
				var serializer = new XmlSerializer(typeof (TagDocument));
				var document = (TagDocument)serializer.Deserialize(stream);

				var categoryListing = document.Listings.First(t => t.Name == "TagCategory");
				var tagsListing = document.Listings.First(t => t.Name == "Tag");

				categories = categoryListing.Elements.ToDictionary(t => t.Index, t => t);
				tags = tagsListing.Elements.ToDictionary(t => t.Index, t => t);
			}
		}

		/// <summary>
		/// Fetches the matching tag for the specified <paramref name="index"/>.
		/// </summary>
		/// <returns>A <see cref="Tag"/> instance containing the matching value, or a default if no match was found.</returns>
		public static Tag FetchTag(ushort index)
		{
			Tag tag;
			if (tags.TryGetValue(index, out tag))
			{
				return tag;
			}
			return new Tag { Index = index };
		}

		/// <summary>
		/// Returns a collection of all known <see cref="Tag"/>s.
		/// </summary>
		public static IEnumerable<Tag> AllTags()
		{
			return tags.Values;
		}

		/// <summary>
		/// Returns a collection of all known category <see cref="Tag"/>s.
		/// </summary>
		public static IEnumerable<Tag> AllCategories()
		{
			return categories.Values;
		}

		/// <summary>
		/// Returns a collection of all know category <see cref="Tag"/>s and additional 
		/// dummy <see cref="Tag"/>s for tags that have no matching categories.
		/// </summary>
		public static IEnumerable<Tag> AllCategoriesWithDummiesForUnpairedTags()
		{
			var knownCategories = AllCategories().ToList();

			foreach (Tag tag in AllTags())
			{
				int index = tag.Value.IndexOf('_');
				if (index != -1)
				{
					string prefix = tag.Value.Substring(0, index);
					if (!knownCategories.Any(c => c.Value.Equals(prefix, StringComparison.OrdinalIgnoreCase)))
					{
						knownCategories.Add(new Tag { Index = ushort.MaxValue, Value = prefix });
					}
				}
			}

			return knownCategories;
		}

		/// <summary>
		/// Gets the matching category for the specified <see cref="Tag"/>. Will return a dummy if no match was found.
		/// </summary>
		public static Tag GetCategoryFor(Tag tag)
		{
			int index = tag.Value.IndexOf('_');
			string prefix = string.Empty;
			if (index != -1)
			{
				prefix = tag.Value.Substring(0, index);
			}

			var category = categories.Values.FirstOrDefault(t => t.Value.Equals(prefix, StringComparison.OrdinalIgnoreCase));

			return category ?? new Tag { Index = ushort.MaxValue, Value = prefix };
		}

		/// <summary>
		/// Returns a collection of all known <see cref="Tag"/> instances that match the
		/// given <paramref name="category"/>.
		/// </summary>
		public static IEnumerable<Tag> FetchTagsForCategory(string category)
		{
			string prefix = category + "_";
			
			return tags.Values.Where(t => t.Value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
		}
	}
}
