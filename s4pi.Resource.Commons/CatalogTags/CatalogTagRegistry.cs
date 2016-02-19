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

		private static Dictionary<uint, Tag> tags;
		private static Dictionary<uint, Tag> categories;

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
		public static Tag FetchTag(uint index)
		{
			return GetTagOrDefault(tags, index);
		}

		/// <summary>
		/// Fetches the matching category for the specified <paramref name="index"/>.
		/// </summary>
		/// <returns>A <see cref="Tag"/> instance containing the matching value, or a default if no match was found.</returns>
		public static Tag FetchCategory(uint index)
		{
			return GetTagOrDefault(categories, index);
		}

		private static Tag GetTagOrDefault(IDictionary<uint, Tag> dictionary, uint index)
		{
			Tag tag;
			if (dictionary.TryGetValue(index, out tag))
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
						knownCategories.Add(new Tag { Index = uint.MaxValue, Value = prefix });
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

			return category ?? new Tag { Index = uint.MaxValue, Value = prefix };
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
