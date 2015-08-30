using System;
using System.Collections.Generic;
using System.IO;

using s4pi.Interfaces;

namespace CatalogResource.Common
{
	public class CatalogTagList : DependentList<CatalogTag>
	{
		public CatalogTagList(EventHandler handler, long maxSize = -1)
			: base(handler, maxSize)
		{
		}

		public CatalogTagList(EventHandler handler, IEnumerable<CatalogTag> ilt, long maxSize = -1)
			: base(handler, ilt, maxSize)
		{
		}

		public CatalogTagList(EventHandler handler, Stream s, long maxSize = -1)
			: base(handler, s, maxSize)
		{
		}

		protected override CatalogTag CreateElement(Stream s)
		{
			return new CatalogTag(CatalogCommon.kRecommendedApiVersion, this.elementHandler, s);
		}

		protected override void WriteElement(Stream s, CatalogTag element)
		{
			element.UnParse(s);
		}
	}
}