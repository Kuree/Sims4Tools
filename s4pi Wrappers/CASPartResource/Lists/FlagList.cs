using System;
using System.IO;

using CASPartResource.Handlers;

using s4pi.Interfaces;

namespace CASPartResource.Lists
{
	public class FlagList : DependentList<Flag>
	{
		public FlagList(EventHandler handler) : base(handler)
		{
		}

		public FlagList(EventHandler handler, Stream s) : base(handler, s)
		{
		}

		#region Data I/O

		protected override void Parse(Stream s)
		{
			var r = new BinaryReader(s);
			var count = r.ReadUInt32();
			for (var i = 0; i < count; i++)
			{
				this.Add(new Flag(CASPartResource.recommendedApiVersion, this.handler, s));
			}
		}

		public override void UnParse(Stream s)
		{
			var w = new BinaryWriter(s);
			w.Write((uint)this.Count);
			foreach (var flag in this)
			{
				flag.UnParse(s);
			}
		}

		#endregion

		protected override Flag CreateElement(Stream s)
		{
			return new Flag(CASPartResource.recommendedApiVersion, this.handler, s);
		}

		protected override void WriteElement(Stream s, Flag element)
		{
			element.UnParse(s);
		}
	}
}