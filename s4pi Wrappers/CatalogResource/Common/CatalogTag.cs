using System;
using System.Collections.Generic;
using System.IO;

using s4pi.Interfaces;
using s4pi.Resource.Commons.CatalogTags;

namespace CatalogResource.Common
{
	public class CatalogTag : AHandlerElement, IEquatable<CatalogTag>
	{
		private Tag tag;

		#region Constructors

		public CatalogTag(int apiVersion, EventHandler handler, CatalogTag other)
			: this(apiVersion, handler, other.tag)
		{
		}

		public CatalogTag(int apiVersion, EventHandler handler)
			: base(apiVersion, handler)
		{
			this.MakeNew();
		}

		public CatalogTag(int apiVersion, EventHandler handler, Stream s)
			: base(apiVersion, handler)
		{
			this.Parse(s);
		}

		public CatalogTag(int apiVersion, EventHandler handler, Tag ctag)
			: base(apiVersion, handler)
		{
			this.tag = ctag;
		}

        	public CatalogTag(int apiVersion, EventHandler handler, uint tagValue)
        	    : base(apiVersion, handler)
        	{
            		this.tag = CatalogTagRegistry.FetchTag(tagValue);
        	}

		public bool Equals(CatalogTag other)
		{
			return this.tag == other.tag;
		}

		#endregion Constructors =========================================

		#region ContentFields

		[ElementPriority(1)]
		public Tag Tag
		{
			get { return this.tag; }
			set { if (this.tag != value) { this.tag = value; this.OnElementChanged(); } }
		}

		public override int RecommendedApiVersion
		{
			get { return CatalogCommon.kRecommendedApiVersion; }
		}

		public string Value { get { return this.ValueBuilder; } }

		public override List<string> ContentFields
		{
			get { return GetContentFields(0, this.GetType()); }
		}

		#endregion ContentFields ========================================

		void Parse(Stream s)
		{
			var br = new BinaryReader(s);
			this.tag = CatalogTagRegistry.FetchTag(br.ReadUInt32());
		}

		public void UnParse(Stream s)
		{
			var bw = new BinaryWriter(s);
			bw.Write(this.Tag.ToUint32());
		}

		private void MakeNew()
		{
			this.tag = new Tag();
		}
	}
}
