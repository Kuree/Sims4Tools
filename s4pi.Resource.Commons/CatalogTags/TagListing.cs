using System.Xml.Serialization;

namespace s4pi.Resource.Commons.CatalogTags
{
	public class TagListing
	{
		[XmlAttribute("n")]
		public string Name { get; set; }

		[XmlArray("L", ElementName = "L")]
		[XmlArrayItem("T", typeof(Tag))]
		public Tag[] Elements { get; set; }
	}
}