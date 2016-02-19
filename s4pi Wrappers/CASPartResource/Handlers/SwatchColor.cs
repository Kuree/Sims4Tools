using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using s4pi.Interfaces;

namespace CASPartResource.Handlers
{
	public class SwatchColor : AHandlerElement, IEquatable<SwatchColor>
	{
		private Color color;

		public SwatchColor(int apiVersion, EventHandler handler, Stream s)
			: base(apiVersion, handler)
		{
			var r = new BinaryReader(s);
			this.color = Color.FromArgb(r.ReadInt32());
		}

		public SwatchColor(int apiVersion, EventHandler handler, Color color) : base(apiVersion, handler)
		{
			this.color = color;
		}

		public SwatchColor(int apiVersion, EventHandler handler) : base(apiVersion, handler)
		{
		}

		public void UnParse(Stream s)
		{
			var w = new BinaryWriter(s);
			w.Write(this.color.ToArgb());
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

		public bool Equals(SwatchColor other)
		{
			return other.Equals(this.color);
		}

		public Color Color
		{
			get { return this.color; }
			set
			{
				if (!this.color.Equals(value))
				{
					this.color = value;
					this.OnElementChanged();
				}
			}
		}

		public string Value
		{
			get
			{
				{
					return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", this.color.A, this.color.R, this.color.G, this.color.B);
				}
			}
		}
	}
}