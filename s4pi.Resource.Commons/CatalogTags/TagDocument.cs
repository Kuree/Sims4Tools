namespace s4pi.Resource.Commons.CatalogTags
{
    using System.Xml.Serialization;

    [XmlRoot("M")]
	public class TagDocument
	{
		[XmlElement("C", ElementName = "C")]
		public TagListing[] Listings { get; set; }
	}
}
