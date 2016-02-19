using System;
using System.IO;

using CASPartResource.Handlers;

using s4pi.Interfaces;

namespace CASPartResource.Lists
{
	public class OverrideList : DependentList<Override>
	{
		public OverrideList(EventHandler handler) : base(handler)
		{
		}

		public OverrideList(EventHandler handler, Stream s) : base(handler, s)
		{
		}

		#region Data I/O

		protected override void Parse(Stream s)
		{
			var r = new BinaryReader(s);
			var count = r.ReadByte();
			for (var i = 0; i < count; i++)
			{
				this.Add(new Override(CASPartResource.recommendedApiVersion, this.handler, s));
			}
		}

		public override void UnParse(Stream s)
		{
			var w = new BinaryWriter(s);
			w.Write((byte)this.Count);
			foreach (var Override in this)
			{
				Override.UnParse(s);
			}
		}

		#endregion

		protected override Override CreateElement(Stream s)
		{
			return new Override(CASPartResource.recommendedApiVersion, this.handler, s);
		}

		protected override void WriteElement(Stream s, Override element)
		{
			element.UnParse(s);
		}
	}
}