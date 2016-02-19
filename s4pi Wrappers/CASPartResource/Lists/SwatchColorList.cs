using System;
using System.Drawing;
using System.IO;

using CASPartResource.Handlers;

using s4pi.Interfaces;

namespace CASPartResource.Lists
{
	public class SwatchColorList : DependentList<SwatchColor>
	{
		public SwatchColorList(EventHandler handler) : base(handler)
		{
		}

		public SwatchColorList(EventHandler handler, Stream s) : base(handler)
		{
			this.Parse(s);
		}

		#region Data I/O

		protected override void Parse(Stream s)
		{
			var r = new BinaryReader(s);
			var count = r.ReadByte();
			for (var i = 0; i < count; i++)
			{
				this.Add(new SwatchColor(1, this.handler, s));
			}
		}

		public override void UnParse(Stream s)
		{
			var w = new BinaryWriter(s);
			w.Write((byte)this.Count);
			foreach (var color in this)
			{
				color.UnParse(s);
			}
		}

		protected override SwatchColor CreateElement(Stream s)
		{
			return new SwatchColor(1, this.handler, Color.Black);
		}

		protected override void WriteElement(Stream s, SwatchColor element)
		{
			element.UnParse(s);
		}

		#endregion
	}
}