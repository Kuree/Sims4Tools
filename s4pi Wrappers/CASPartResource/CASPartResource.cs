/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using CASPartResource.Lists;
using CASPartResource.Handlers;

using s4pi.Interfaces;
using s4pi.Settings;

namespace CASPartResource
{
	public class CASPartResource : AResource
	{
		internal const int recommendedApiVersion = 1;

		public override int RecommendedApiVersion
		{
			get { return recommendedApiVersion; }
		}

		#region Attributes

		public uint version;
		private uint presetCount;
		private string name;
		private float sortPriority;
		private ushort secondarySortIndex;
		private uint propertyID;
		private uint auralMaterialHash;
		private ParmFlag parmFlags;
		private ExcludePartFlag excludePartFlags;
        	private ulong excludeModifierRegionFlags;   // cmar - changed from uint to ulong with V 0x25
        	private FlagList flagList;                  // property 16-bit tag / 32-bit value pairs
        	private uint deprecatedPrice;               // deprecated
		private uint partTitleKey;
		private uint partDesptionKey;
		private byte uniqueTextureSpace;
		private BodyType bodyType;
        	int bodySubType;                            // cmar - changed from unused with V 0x25
		private AgeGenderFlags ageGender;
		private uint reserved1;                     // cmar - added V 0x20, set to 1
        	short packID;                               // cmar - added V 0x25
        	PackFlag packFlags;                         // cmar - added V 0x25
        	byte[] reserved2;                           // cmar - added V 0x25, nine bytes, set to 0
        	byte unused2;                               // cmar - only if V < 0x25
        	byte unused3;                               // cmar - only if V < 0x25
        	private SwatchColorList swatchColorCode;
		private byte buffResKey;
		private byte varientThumbnailKey;
		private ulong voiceEffectHash;
		private byte usedMaterialCount;             // cmar - added V 0x1E
		private uint materialSetUpperBodyHash;      // cmar - added V 0x1E
		private uint materialSetLowerBodyHash;      // cmar - added V 0x1E
		private uint materialSetShoesHash;          // cmar - added V 0x1E
		private OccultTypesDisabled hideForOccultFlags; // cmar = added V 0x1F
		private byte nakedKey;
		private byte parentKey;
		private int sortLayer;
		private LODBlockList lodBlockList;
		private SimpleList<byte> slotKey;
		private byte diffuseShadowKey;
		private byte shadowKey;
		private byte compositionMethod;
		private byte regionMapKey;
		private OverrideList overrides;
		private byte normalMapKey;
		private byte specularMapKey;
		private uint sharedUVMapSpace;
		private byte emissionMapKey;                // cmar - added V 0x1E
		private CountedTGIBlockList tgiList;

		#endregion

		public CASPartResource(int APIversion, Stream s) : base(APIversion, s)
		{
			if (this.stream == null || this.stream.Length == 0)
			{
				this.stream = this.UnParse();
				this.OnResourceChanged(this, EventArgs.Empty);
			}
			this.stream.Position = 0;
			this.Parse(this.stream);
		}

		#region Data I/O

		private void Parse(Stream s)
		{
			s.Position = 0;
			var r = new BinaryReader(s);
			this.version = r.ReadUInt32();
			this.TGIoffset = r.ReadUInt32() + 8;
			this.presetCount = r.ReadUInt32();
			if (this.presetCount != 0)
			{
				throw new Exception("Found non-zero one");
			}
			this.name = BigEndianUnicodeString.Read(s);

			this.sortPriority = r.ReadSingle();
			this.secondarySortIndex = r.ReadUInt16();
			this.propertyID = r.ReadUInt32();
			this.auralMaterialHash = r.ReadUInt32();
			this.parmFlags = (ParmFlag)r.ReadByte();
			this.excludePartFlags = (ExcludePartFlag)r.ReadUInt64();
            		if (this.version >= 36) this.excludeModifierRegionFlags = r.ReadUInt64();
            		else this.excludeModifierRegionFlags = r.ReadUInt32();

            		if (this.version >= 37) flagList = new FlagList(OnResourceChanged, s);
            		else
            		{
                		uint flagCount = r.ReadUInt32();
                		flagList = new FlagList(OnResourceChanged);
                		for (int i = 0; i < flagCount; i++)
                		{
                    			ushort cat = r.ReadUInt16();
                			ushort val = r.ReadUInt16();
                    			flagList.Add(new Flag(recommendedApiVersion, OnResourceChanged, cat, val));
                		}
            		}

			this.deprecatedPrice = r.ReadUInt32();
			this.partTitleKey = r.ReadUInt32();
			this.partDesptionKey = r.ReadUInt32();
			this.uniqueTextureSpace = r.ReadByte();
			this.bodyType = (BodyType)r.ReadInt32();
			this.bodySubType = r.ReadInt32();
			this.ageGender = (AgeGenderFlags)r.ReadUInt32();
			if (this.version >= 0x20)
			{
				this.reserved1 = r.ReadUInt32();
			}
            		if (this.version >= 34)
            		{
                		this.packID = r.ReadInt16();
                		this.packFlags = (PackFlag)r.ReadByte();
                		this.reserved2 = r.ReadBytes(9);
            		}
            		else
            		{
                		this.packID = 0;
                		this.unused2 = r.ReadByte();
                		if (this.unused2 > 0) this.unused3 = r.ReadByte();
            		}

			this.swatchColorCode = new SwatchColorList(this.OnResourceChanged, s);

			this.buffResKey = r.ReadByte();
			this.varientThumbnailKey = r.ReadByte();
			if (this.version >= 0x1C)
			{
				this.voiceEffectHash = r.ReadUInt64();
			}
			if (this.version >= 0x1E)
			{
				this.usedMaterialCount = r.ReadByte();
				if (this.usedMaterialCount > 0)
				{
					this.materialSetUpperBodyHash = r.ReadUInt32();
					this.materialSetLowerBodyHash = r.ReadUInt32();
					this.materialSetShoesHash = r.ReadUInt32();
				}
			}
			if (this.version >= 0x1F)
			{
				this.hideForOccultFlags = (OccultTypesDisabled)r.ReadUInt32();
			}
			this.nakedKey = r.ReadByte();
			this.parentKey = r.ReadByte();
			this.sortLayer = r.ReadInt32();

			// Don't move any of this before the -----
			// TGI block list
			var currentPosition = r.BaseStream.Position;
			r.BaseStream.Position = this.TGIoffset;
			var count4 = r.ReadByte();
			this.tgiList = new CountedTGIBlockList(this.OnResourceChanged, "IGT", count4, s);
			r.BaseStream.Position = currentPosition;
			this.lodBlockList = new LODBlockList(null, s, this.tgiList);
			//-------------

			var count = r.ReadByte();
			this.slotKey = new SimpleList<byte>(null);
			for (byte i = 0; i < count; i++)
			{
				this.slotKey.Add(r.ReadByte());
			}

			this.diffuseShadowKey = r.ReadByte();
			this.shadowKey = r.ReadByte();
			this.compositionMethod = r.ReadByte();
			this.regionMapKey = r.ReadByte();
			this.overrides = new OverrideList(null, s);
			this.normalMapKey = r.ReadByte();
			this.specularMapKey = r.ReadByte();
			if (this.version >= 0x1B)
			{
				this.sharedUVMapSpace = r.ReadUInt32();
			}
			if (this.version >= 0x1E)
			{
				this.emissionMapKey = r.ReadByte();
			}
		}

		protected override Stream UnParse()
		{
			var s = new MemoryStream();
			var w = new BinaryWriter(s);

			w.Write(this.version);
			w.Write(0); // tgi offset
			w.Write(this.presetCount);
			BigEndianUnicodeString.Write(s, this.name);
			w.Write(this.sortPriority);
			w.Write(this.secondarySortIndex);
			w.Write(this.propertyID);
			w.Write(this.auralMaterialHash);
			w.Write((byte)this.parmFlags);
			w.Write((ulong)this.excludePartFlags);
            		if (this.version >= 36)
            		{
                		w.Write(this.excludeModifierRegionFlags);
            		}
            		else
            		{
                		w.Write((uint)this.excludeModifierRegionFlags);
            		}
            		if (this.version >= 37)
            		{
                		if (this.flagList == null) this.flagList = new FlagList(OnResourceChanged);
                		this.flagList.UnParse(s);
            		}
            		else
            		{
                		w.Write(this.flagList.Count);
                		foreach (Flag f in this.flagList)
                		{
                    			w.Write((ushort)f.CompoundTag.Category);
                    			w.Write((ushort)f.CompoundTag.Value);
                		}
            		}
            		w.Write(this.deprecatedPrice);
			w.Write(this.partTitleKey);
			w.Write(this.partDesptionKey);
			w.Write(this.uniqueTextureSpace);
			w.Write((uint)this.bodyType);
			w.Write(this.bodySubType);
			w.Write((uint)this.ageGender);
			if (this.version >= 0x20)
			{
				w.Write(this.reserved1);
			}
            		if (this.version >= 34)
            		{
                		w.Write(this.packID);
        	 		w.Write((byte)this.packFlags);
                		if (reserved2 == null) this.reserved2 = new byte[9];
                		w.Write(this.reserved2);
            		}
            		else
            		{
                		w.Write(this.unused2);
                		if (this.unused2 > 0) w.Write(this.unused3);
            		}
            		if (this.swatchColorCode == null)
			{
				this.swatchColorCode = new SwatchColorList(this.OnResourceChanged);
			}
			this.swatchColorCode.UnParse(s);
			w.Write(this.buffResKey);
			w.Write(this.varientThumbnailKey);
			if (this.version >= 0x1C)
			{
				w.Write(this.voiceEffectHash);
			}
			if (this.version >= 0x1E)
			{
				w.Write(this.usedMaterialCount);
				if (this.usedMaterialCount > 0)
				{
					w.Write(this.materialSetUpperBodyHash);
					w.Write(this.materialSetLowerBodyHash);
					w.Write(this.materialSetShoesHash);
				}
			}
			if (this.version >= 0x1F)
			{
				w.Write((uint)this.hideForOccultFlags);
			}
			w.Write(this.nakedKey);
			w.Write(this.parentKey);
			w.Write(this.sortLayer);
			if (this.lodBlockList == null)
			{
				this.lodBlockList = new LODBlockList(this.OnResourceChanged);
			}
			this.lodBlockList.UnParse(s);
			if (this.slotKey == null)
			{
				this.slotKey = new SimpleList<byte>(this.OnResourceChanged);
			}
			w.Write((byte)this.slotKey.Count);
			foreach (var b in this.slotKey)
			{
				w.Write(b);
			}
			w.Write(this.diffuseShadowKey);
			w.Write(this.shadowKey);
			w.Write(this.compositionMethod);
			w.Write(this.regionMapKey);
			if (this.overrides == null)
			{
				this.overrides = new OverrideList(this.OnResourceChanged);
			}
			this.overrides.UnParse(s);
			w.Write(this.normalMapKey);
			w.Write(this.specularMapKey);
			if (this.version >= 0x1B)
			{
				w.Write(this.sharedUVMapSpace);
			}
			if (this.version >= 0x1E)
			{
				w.Write(this.emissionMapKey);
			}
			var tgiPosition = w.BaseStream.Position;
			w.BaseStream.Position = 4;
			w.Write(tgiPosition - 8);
			w.BaseStream.Position = tgiPosition;
			if (this.tgiList == null)
			{
				this.tgiList = new CountedTGIBlockList(this.OnResourceChanged);
			}
			w.Write((byte)this.tgiList.Count);
			foreach (var tgi in this.tgiList)
			{
				tgi.UnParse(s);
			}

			return s;
		}

		#endregion

		#region Content Fields

		[ElementPriority(0)]
		public uint Version
		{
			get { return this.version; }
			set
			{
				if (value != this.version)
				{
					this.version = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(1)]
		public uint TGIoffset { get; private set; }

		[ElementPriority(2)]
		public uint PresetCount
		{
			get { return this.presetCount; }
			set
			{
				if (value != this.presetCount)
				{
					this.presetCount = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(3)]
		public string Name
		{
			get { return this.name; }
			set
			{
				if (!value.Equals(this.name))
				{
					this.name = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(4)]
		public float SortPriority
		{
			get { return this.sortPriority; }
			set
			{
				if (!value.Equals(this.sortPriority))
				{
					this.sortPriority = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(5)]
		public ushort SecondarySortIndex
		{
			get { return this.secondarySortIndex; }
			set
			{
				if (!value.Equals(this.secondarySortIndex))
				{
					this.secondarySortIndex = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(6)]
		public uint PropertyID
		{
			get { return this.propertyID; }
			set
			{
				if (!value.Equals(this.propertyID))
				{
					this.propertyID = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(7)]
		public uint AuralMaterialHash
		{
			get { return this.auralMaterialHash; }
			set
			{
				if (!value.Equals(this.auralMaterialHash))
				{
					this.auralMaterialHash = value;
					this.OnResourceChanged(this, EventArgs.Empty);
				}
			}
		}

		[ElementPriority(8)]
		public ParmFlag ParmFlags
		{
			get { return this.parmFlags; }
			set
			{
				if (!value.Equals(this.parmFlags))
				{
					this.parmFlags = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(9)]
		public ExcludePartFlag ExcludePartFlags
		{
			get { return this.excludePartFlags; }
			set
			{
				if (!value.Equals(this.excludePartFlags))
				{
					this.excludePartFlags = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(10)]
		public ulong ExcludeModifierRegionFlags
		{
			get { return this.excludeModifierRegionFlags; }
			set
			{
				if (!value.Equals(this.excludeModifierRegionFlags))
				{
					this.excludeModifierRegionFlags = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(11)]
		public FlagList CASFlagList
		{
			get { return this.flagList; }
			set
			{
				if (!value.Equals(this.flagList))
				{
					this.flagList = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(12)]
		public uint DeprecatedPrice
		{
			get { return this.deprecatedPrice; }
			set
			{
				if (!value.Equals(this.deprecatedPrice))
				{
					this.deprecatedPrice = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(13)]
		public uint PartTitleKey
		{
			get { return this.partTitleKey; }
			set
			{
				if (!value.Equals(this.partTitleKey))
				{
					this.partTitleKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(14)]
		public uint PartDescriptionKey
		{
			get { return this.partDesptionKey; }
			set
			{
				if (!value.Equals(this.partDesptionKey))
				{
					this.partDesptionKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(15)]
		public byte UniqueTextureSpace
		{
			get { return this.uniqueTextureSpace; }
			set
			{
				if (!value.Equals(this.uniqueTextureSpace))
				{
					this.uniqueTextureSpace = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(16)]
		public BodyType BodyType
		{
			get { return this.bodyType; }
			set
			{
				if (!value.Equals(this.bodyType))
				{
					this.bodyType = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(17)]
		public int BodySubType
		{
			get { return this.bodySubType; }
			set
			{
				if (!value.Equals(this.bodySubType))
				{
					this.bodySubType = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(18)]
		public AgeGenderFlags AgeGender
		{
			get { return this.ageGender; }
			set
			{
				if (!value.Equals(this.ageGender))
				{
					this.ageGender = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(19)]
		public uint Reserved1
		{
			get { return this.reserved1; }
			set
			{
				if (!value.Equals(this.reserved1))
				{
					this.reserved1 = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

        	[ElementPriority(20)]
        	public short PackID 
        	{
            		get { return this.packID; } 
            		set 
            		{
                		if (this.packID == 0 || value > 0)
                		{
                    			this.packID = value;
                    			OnResourceChanged(this, EventArgs.Empty);
                		}
            		} 
        	}

        	[ElementPriority(21)]
        	public PackFlag PackFlags 
        	{
            		get { return this.packFlags; } 
            	set 
            	{
                	if (!value.Equals(this.packFlags))
                	{
                    	this.packFlags = value;
                	}
                	OnResourceChanged(this, EventArgs.Empty); 
            	} 
        	}

        	[ElementPriority(22)]
        	public byte[] Reserved2 
        	{
            		get { return this.reserved2; } 
            		set 
            		{
                		if (!value.Equals(this.reserved2)) this.reserved2 = value; OnResourceChanged(this, EventArgs.Empty); 
            		} 
        	}

        	[ElementPriority(20)]
		public byte Unused2
		{
			get { return this.unused2; }
			set
			{
				if (!value.Equals(this.unused2))
				{
					this.unused2 = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(21)]
		public byte Unused3
		{
			get { return this.unused3; }
			set
			{
				if (!value.Equals(this.unused3))
				{
					this.unused3 = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(22)]
		public SwatchColorList SwatchColorCode
		{
			get { return this.swatchColorCode; }
			set
			{
				if (!this.swatchColorCode.Equals(value))
				{
					this.swatchColorCode = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(23), TGIBlockListContentField("TGIList")]
		public byte BuffResKey
		{
			get { return this.buffResKey; }
			set
			{
				if (!value.Equals(this.buffResKey))
				{
					this.buffResKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(24), TGIBlockListContentField("TGIList")]
		public byte VarientThumbnailKey
		{
			get { return this.varientThumbnailKey; }
			set
			{
				if (!value.Equals(this.varientThumbnailKey))
				{
					this.varientThumbnailKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(25)]
		public ulong VoiceEffectHash
		{
			get { return this.voiceEffectHash; }
			set
			{
				if (!value.Equals(this.voiceEffectHash))
				{
					this.voiceEffectHash = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(26)]
		public uint MaterialSetUpperBodyHash
		{
			get { return this.materialSetUpperBodyHash; }
			set
			{
				if (!value.Equals(this.materialSetUpperBodyHash))
				{
					this.materialSetUpperBodyHash = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(27)]
		public uint MaterialSetLowerBodyHash
		{
			get { return this.materialSetLowerBodyHash; }
			set
			{
				if (!value.Equals(this.materialSetLowerBodyHash))
				{
					this.materialSetLowerBodyHash = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(28)]
		public uint MaterialSetShoesHash
		{
			get { return this.materialSetShoesHash; }
			set
			{
				if (!value.Equals(this.materialSetShoesHash))
				{
					this.materialSetShoesHash = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(29)]
		public OccultTypesDisabled HideForOccultFlags
		{
			get { return this.hideForOccultFlags; }
			set
			{
				if (!value.Equals(this.hideForOccultFlags))
				{
					this.hideForOccultFlags = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(30), TGIBlockListContentField("TGIList")]
		public byte NakedKey
		{
			get { return this.nakedKey; }
			set
			{
				if (!value.Equals(this.nakedKey))
				{
					this.nakedKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(31), TGIBlockListContentField("TGIList")]
		public byte ParentKey
		{
			get { return this.parentKey; }
			set
			{
				if (!value.Equals(this.parentKey))
				{
					this.parentKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(32)]
		public int SortLayer
		{
			get { return this.sortLayer; }
			set
			{
				if (!value.Equals(this.sortLayer))
				{
					this.sortLayer = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(33)]
		public LODBlockList LodBlockList
		{
			get { return this.lodBlockList; }
			set
			{
				if (!this.lodBlockList.Equals(value))
				{
					this.lodBlockList = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(34), TGIBlockListContentField("TGIList")]
		public SimpleList<byte> SlotKey
		{
			get { return this.slotKey; }
			set
			{
				if (!value.Equals(this.slotKey))
				{
					this.slotKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(35), TGIBlockListContentField("TGIList")]
		public byte DiffuseShadowKey
		{
			get { return this.diffuseShadowKey; }
			set
			{
				if (!value.Equals(this.diffuseShadowKey))
				{
					this.diffuseShadowKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(36), TGIBlockListContentField("TGIList")]
		public byte ShadowKey
		{
			get { return this.shadowKey; }
			set
			{
				if (!value.Equals(this.shadowKey))
				{
					this.shadowKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(37)]
		public byte CompositionMethod
		{
			get { return this.compositionMethod; }
			set
			{
				if (!value.Equals(this.compositionMethod))
				{
					this.compositionMethod = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(38), TGIBlockListContentField("TGIList")]
		public byte RegionMapKey
		{
			get { return this.regionMapKey; }
			set
			{
				if (!value.Equals(this.regionMapKey))
				{
					this.regionMapKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(39)]
		public OverrideList Overrides
		{
			get { return this.overrides; }
			set
			{
				if (!value.Equals(this.overrides))
				{
					this.overrides = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(40), TGIBlockListContentField("TGIList")]
		public byte NormalMapKey
		{
			get { return this.normalMapKey; }
			set
			{
				if (!value.Equals(this.normalMapKey))
				{
					this.normalMapKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(41), TGIBlockListContentField("TGIList")]
		public byte SpecularMapKey
		{
			get { return this.specularMapKey; }
			set
			{
				if (!value.Equals(this.specularMapKey))
				{
					this.specularMapKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(42)]
		public uint SharedUVMapSpace
		{
			get
			{
				if (this.version < 0x1B)
				{
					throw new InvalidOperationException("Version not supported");
				}
				return this.sharedUVMapSpace;
			}
			set
			{
				if (this.version < 0x1B)
				{
					throw new InvalidOperationException("Version not Supported");
				}
				this.sharedUVMapSpace = value;
			}
		}

		[ElementPriority(43), TGIBlockListContentField("TGIList")]
		public byte EmissionMapKey
		{
			get { return this.emissionMapKey; }
			set
			{
				if (!value.Equals(this.emissionMapKey))
				{
					this.emissionMapKey = value;
				}
				this.OnResourceChanged(this, EventArgs.Empty);
			}
		}

		[ElementPriority(44)]
		public CountedTGIBlockList TGIList
		{
			get { return this.tgiList; }
			set
			{
				if (!value.Equals(this.tgiList))
				{
					this.OnResourceChanged(this, EventArgs.Empty);
					this.tgiList = value;
				}
			}
		}

		public string Value
		{
			get { return this.ValueBuilder; }
		}

		public override List<string> ContentFields
		{
			get
			{
				var res = base.ContentFields;
				if (this.version < 0x1B)
				{
					res.Remove("SharedUVMapSpace");
				}
				if (this.version < 0x1C)
				{
					res.Remove("VoiceEffectHash");
				}
				if (this.version < 0x1E)
				{
					res.Remove("MaterialSetUpperBodyHash");
					res.Remove("MaterialSetLowerBodyHash");
					res.Remove("MaterialSetShoesHash");
					res.Remove("EmissionMapKey");
				}
				if (this.version < 0x1F)
				{
					res.Remove("OccultBitField");
				}
				if (this.version < 0x20)
				{
					res.Remove("Reserved1");
				}
                		if (this.version < 0x25)
                		{
                			res.Remove("PackID");
                    			res.Remove("PackFlags");
                    			res.Remove("Reserved2");
                		}
                		else
                		{
                    			res.Remove("Unused2");
                    			res.Remove("Unused3");
                		}
				return res;
			}
		}

		#endregion

	}

	public class CASPartResourceHandler : AResourceHandler
	{
		public CASPartResourceHandler()
		{
			if (Settings.IsTS4)
			{
				this.Add(typeof(CASPartResource), new List<string>(new[] { "0x034AEECB" }));
			}
		}
	}
}
