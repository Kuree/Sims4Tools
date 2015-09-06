using System;
using System.IO;

using CASPartResource.Handlers;

using s4pi.Interfaces;

namespace CASPartResource.Lists
{
	public class LODBlockList : DependentList<LODInfoEntry>
	{
		#region Attributes

		private readonly CountedTGIBlockList tgiList;

		#endregion

		#region Constructors

		public LODBlockList(EventHandler handler) : this(handler, new CountedTGIBlockList(handler))
		{
		}

		public LODBlockList(EventHandler handler, CountedTGIBlockList tgiList) : base(handler)
		{
			this.tgiList = tgiList;
		}

		public LODBlockList(EventHandler handler, Stream s, CountedTGIBlockList tgiList) : base(handler)
		{
			this.tgiList = tgiList;
			this.Parse(s);
		}

		#endregion

		#region Data I/O

		protected override void Parse(Stream s)
		{
			var r = new BinaryReader(s);
			var count = r.ReadByte();
			for (var i = 0; i < count; i++)
			{
				this.Add(new LODInfoEntry(1, this.handler, s, this.tgiList));
			}
		}

		public override void UnParse(Stream s)
		{
			var w = new BinaryWriter(s);
			w.Write((byte)this.Count);
			foreach (var unknownClass in this)
			{
				unknownClass.UnParse(s);
			}
		}

		protected override LODInfoEntry CreateElement(Stream s)
		{
			return new LODInfoEntry(1, this.handler, this.tgiList);
		}

		protected override void WriteElement(Stream s, LODInfoEntry element)
		{
			element.UnParse(s);
		}

		#endregion
	}
}