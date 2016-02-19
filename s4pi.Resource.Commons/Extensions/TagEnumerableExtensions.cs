using System.Collections.Generic;
using System.Linq;

using s4pi.Resource.Commons.CatalogTags;

namespace s4pi.Resource.Commons.Extensions
{
	internal static class TagEnumerableExtensions
	{
		public static object[] ToObjectArray(this IEnumerable<Tag> tags)
		{
			return tags.Cast<object>().ToArray();
		}

		public static IOrderedEnumerable<Tag> Order(this IEnumerable<Tag> tags)
		{
			return tags.OrderBy(t => t.Index).ThenBy(t => t.Value);
		}
	}
}