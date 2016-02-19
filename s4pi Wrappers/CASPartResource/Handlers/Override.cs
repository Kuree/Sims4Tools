using System;
using System.Collections.Generic;
using System.IO;

using s4pi.Interfaces;

namespace CASPartResource.Handlers
{
	public class Override : AHandlerElement, IEquatable<Override>
	{
		private byte region;
		private float layer;

		public Override(int apiVersion, EventHandler handler, Stream s)
			: base(apiVersion, handler)
		{
			var r = new BinaryReader(s);
			this.region = r.ReadByte();
			this.layer = r.ReadSingle();
		}

		public Override(int apiVersion, EventHandler handler) : base(apiVersion, handler)
		{
		}

		internal void UnParse(Stream s)
		{
			var w = new BinaryWriter(s);
			w.Write(this.region);
			w.Write(this.layer);
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

		public bool Equals(Override other)
		{
			return this.region == other.region && Math.Abs(this.layer - other.layer) < 0.001;
		}

		[ElementPriority(0)]
		public byte Region
		{
			get { return this.region; }
			set
			{
				if (value != this.region)
				{
					this.OnElementChanged();
					this.region = value;
				}
			}
		}

		[ElementPriority(1)]
		public float Layer
		{
			get { return this.layer; }
			set
			{
				if (value != this.layer)
				{
					this.OnElementChanged();
					this.layer = value;
				}
			}
		}

		public string Value
		{
			get { return this.ValueBuilder; }
		}
	}
}