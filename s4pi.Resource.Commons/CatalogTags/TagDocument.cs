using System;
using System.Xml.Serialization;

namespace s4pi.Resource.Commons.CatalogTags
{
	[XmlRoot("M")]
	public class TagDocument
	{
		[XmlElement("C", ElementName = "C")]
		public TagListing[] Listings { get; set; }
	}
}
