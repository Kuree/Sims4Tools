using System;
using System.Collections.Generic;
using System.IO;

using s4pi.Interfaces;
using s4pi.Resource.Commons.CatalogTags;

namespace CASPartResource.Handlers
{
	public class Flag : AHandlerElement, IEquatable<Flag>
	{
		private CompoundTag compoundTag;

		public Flag(int APIversion, EventHandler handler, Stream s)
			: base(APIversion, handler)
		{
			var reader = new BinaryReader(s);
			var category = CatalogTagRegistry.FetchCategory(reader.ReadUInt16());
			var value = CatalogTagRegistry.FetchTag(reader.ReadUInt16());
			this.compoundTag = new CompoundTag { Category = category, Value = value };
		}

        	public Flag(int APIversion, EventHandler handler, ushort flagCategory, uint flagValue)
            		: base(APIversion, handler)
        	{
            		var category = CatalogTagRegistry.FetchCategory(flagCategory);
            		var value = CatalogTagRegistry.FetchTag(flagValue);
            		this.compoundTag = new CompoundTag { Category = category, Value = value };
        	}
        
        	public Flag(int APIversion, EventHandler handler) : base(APIversion, handler)
		{
		}

		public void UnParse(Stream s)
		{
			var w = new BinaryWriter(s);
			w.Write((ushort)this.compoundTag.Category);
			w.Write(this.compoundTag.Value);
		}

		#region AHandlerElement Members

		public override int RecommendedApiVersion
		{
			get { return CASPartResource.recommendedApiVersion; }
		}

		public override List<string> ContentFields
		{
			get { return GetContentFields(this.requestedApiVersion, this.GetType()); }
		}

		#endregion

		public bool Equals(Flag other)
		{
			if (other != null)
			{
				return this.compoundTag == other.compoundTag;
			}
			return false;
		}

		[ElementPriority(0)]
		public CompoundTag CompoundTag
		{
			get { return this.compoundTag; }
			set
			{
				if (value != this.compoundTag)
				{
					this.OnElementChanged();
					this.compoundTag = value;
				}
			}
		}

		public string Value
		{
			get { return this.ValueBuilder; }
		}
	}
}
