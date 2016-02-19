using System;
using System.Collections.Generic;
using System.IO;

using s4pi.Interfaces;

namespace CASPartResource.Handlers
{
	public class LODInfoEntry : AHandlerElement, IEquatable<LODInfoEntry>
	{
		private const int recommendedApiVersion = 1;

		private readonly CountedTGIBlockList tgiList;

		private byte level;
		private uint unused;
		private LODAssetList lodAssetList;
		private ByteIndexList lodKeyList;

		public LODInfoEntry(int apiVersion, EventHandler handler) : base(apiVersion, handler)
		{
			this.UnParse(new MemoryStream());
		}

		public LODInfoEntry(int apiVersion, EventHandler handler, CountedTGIBlockList tgiList) : base(apiVersion, handler)
		{
			this.tgiList = tgiList;
		}

		public LODInfoEntry(int apiVersion, EventHandler handler, Stream s, CountedTGIBlockList tgiList)
			: base(apiVersion, handler)
		{
			this.tgiList = tgiList;
			this.Parse(s);
		}

		public void Parse(Stream s)
		{
			var r = new BinaryReader(s);
			this.level = r.ReadByte();

			this.unused = r.ReadUInt32();

			this.lodAssetList = new LODAssetList(null, s);

			var byteList = new byte[r.ReadByte()];
			for (var i = 0; i < byteList.Length; i++)
			{
				byteList[i] = r.ReadByte();
			}
			this.lodKeyList = new ByteIndexList(this.handler, byteList, this.tgiList);
		}

		public void UnParse(Stream s)
		{
			var w = new BinaryWriter(s);
			w.Write(this.level);
			w.Write(this.unused);

			if (this.lodAssetList == null)
			{
				this.lodAssetList = new LODAssetList(this.handler);
			}
			this.lodAssetList.UnParse(s);

			if (this.lodKeyList == null)
			{
				this.lodKeyList = new ByteIndexList(this.handler, new CountedTGIBlockList(this.handler));
			}
			w.Write((byte)this.lodKeyList.Count);
			foreach (var b in this.lodKeyList)
			{
				w.Write(b);
			}
		}

		#region AHandlerElement Members

		public override int RecommendedApiVersion
		{
			get { return recommendedApiVersion; }
		}

		public override List<string> ContentFields
		{
			get { return GetContentFields(this.requestedApiVersion, this.GetType()); }
		}

		#endregion

		#region Content Fields

		[ElementPriority(0)]
		public byte Level
		{
			get { return this.level; }
			set
			{
				if (!value.Equals(this.level))
				{
					this.level = value;
					this.OnElementChanged();
				}
			}
		}

		public uint Unused
		{
			get { return this.unused; }
			set
			{
				if (!value.Equals(this.unused))
				{
					this.unused = value;
					this.OnElementChanged();
				}
			}
		}

		[ElementPriority(3)]
		public LODAssetList LodAssetList
		{
			get { return this.lodAssetList; }
			set
			{
				if (!value.Equals(this.lodAssetList))
				{
					this.lodAssetList = value;
					this.OnElementChanged();
				}
			}
		}

		[ElementPriority(4)]
		public ByteIndexList LODKeyList
		{
			get { return this.lodKeyList; }
			set
			{
				if (!value.Equals(this.lodKeyList))
				{
					value = this.lodKeyList;
				}
				this.OnElementChanged();
			}
		}

		public string Value
		{
			get { return this.ValueBuilder; }
		}

		#endregion

		#region IEquatable

		public bool Equals(LODInfoEntry other)
		{
			return this.level == other.level && this.unused == other.unused && this.lodKeyList.Equals(other.lodKeyList);
		}

		#endregion

		#region Sub-class

		public class LodAssets : AHandlerElement, IEquatable<LodAssets>
		{
			#region Attribute

			private const int recommendedApiVersion = 1;

			private int sorting;
			private int specLevel;
			private int castShadow;

			#endregion

			#region Constructor

			public LodAssets(int apiVersion, EventHandler handler) : base(apiVersion, handler)
			{
			}

			public LodAssets(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler)
			{
				this.Parse(s);
			}

			#endregion

			#region AHandlerElement Members

			public override int RecommendedApiVersion
			{
				get { return recommendedApiVersion; }
			}

			public override List<string> ContentFields
			{
				get { return GetContentFields(this.requestedApiVersion, this.GetType()); }
			}

			#endregion

			#region Data I/O

			protected void Parse(Stream s)
			{
				var r = new BinaryReader(s);
				this.sorting = r.ReadInt32();
				this.specLevel = r.ReadInt32();
				this.castShadow = r.ReadInt32();
			}

			public void UnParse(Stream s)
			{
				var w = new BinaryWriter(s);
				w.Write(this.sorting);
				w.Write(this.specLevel);
				w.Write(this.castShadow);
			}

			#endregion

			#region IEquatable

			public bool Equals(LodAssets other)
			{
				return this.sorting == other.sorting && this.specLevel == other.specLevel && this.castShadow == other.castShadow;
			}

			#endregion

			#region Content Fields

			public int Sorting
			{
				get { return this.sorting; }
				set
				{
					if (!value.Equals(this.sorting))
					{
						this.OnElementChanged();
						this.sorting = value;
					}
				}
			}

			public int SpecLevel
			{
				get { return this.specLevel; }
				set
				{
					if (!value.Equals(this.specLevel))
					{
						this.OnElementChanged();
						this.specLevel = value;
					}
				}
			}

			public int CastShadow
			{
				get { return this.castShadow; }
				set
				{
					if (!value.Equals(this.castShadow))
					{
						this.OnElementChanged();
						this.castShadow = value;
					}
				}
			}

			public string Value
			{
				get { return this.ValueBuilder; }
			}

			#endregion
		}

		public class LODAssetList : DependentList<LodAssets>
		{
			public LODAssetList(EventHandler handler) : base(handler)
			{
			}

			public LODAssetList(EventHandler handler, Stream s) : base(handler)
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
					this.Add(new LodAssets(1, this.handler, s));
				}
			}

			public override void UnParse(Stream s)
			{
				var w = new BinaryWriter(s);
				w.Write((byte)this.Count);
				foreach (var asset in this)
				{
					asset.UnParse(s);
				}
			}

			protected override LodAssets CreateElement(Stream s)
			{
				return new LodAssets(1, this.handler, s);
			}

			protected override void WriteElement(Stream s, LodAssets element)
			{
				element.UnParse(s);
			}

			#endregion
		}

		#endregion
	}
}