/***************************************************************************
 *  Copyright (C) 2009, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Inge Jones                                                             *
 *  Buzzler                                                                *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s4pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

namespace CatalogResource
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using s4pi.Interfaces;

    public abstract class AbstractCatalogResource : AResource
    {
        private uint version = 0x19;
        private CatalogCommon commonA;
        private uint auralMaterialsVersion = 0x1;
        private uint auralMaterials1 = 0x811C9DC5;
        private uint auralMaterials2 = 0x811C9DC5;
        private uint auralMaterials3 = 0x811C9DC5;
        private uint auralPropertiesVersion = 0x2;
        private uint auralQuality = 0x811C9DC5;
        private uint auralAmbientObject = 0x811C9DC5;
        private ulong ambienceFileInstanceId;
        private byte isOverrideAmbience;
        private byte unknown01;
        private uint unused0;
        private uint unused1;
        private uint unused2;
        private uint placementFlagsHigh;
        private uint placementFlagsLow;
        private ulong slotTypeSet;
        private byte slotDecoSize;
        private ulong catalogGroup;
        private byte stateUsage;
        private ColorList colors;
        private uint fenceHeight;
        private byte isStackable;
        private byte canItemDepreciate;
        private TGIBlock fallbackObjectKey;

        protected AbstractCatalogResource(int APIversion, Stream s)
            : base(APIversion, s)
        {
            if (s == null || s.Length == 0)
            {
                s = this.UnParse();
                this.OnResourceChanged(this, EventArgs.Empty);
            }
            s.Position = 0;
            this.Parse(s);
        }

        private const int kRecommendedApiVersion = 1;

        public override int RecommendedApiVersion
        {
            get { return COBJResource.kRecommendedApiVersion; }
        }

        [ElementPriority(0)]
        public string TypeOfResource
        {
            get { return "CatalogObject"; }
        }

        [ElementPriority(1)]
        public uint Version
        {
            get { return this.version; }
            set
            {
                if (this.version != value)
                {
                    this.version = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(2)]
        public CatalogCommon CommonBlock
        {
            get { return this.commonA; }
            set
            {
                if (this.commonA != value)
                {
                    this.commonA = new CatalogCommon(COBJResource.kRecommendedApiVersion, this.OnResourceChanged, value);
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(5)]
        public uint AuralMaterialsVersion
        {
            get { return this.auralMaterialsVersion; }
            set
            {
                if (this.auralMaterialsVersion != value)
                {
                    this.auralMaterialsVersion = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(6)]
        public uint AuralMaterials1
        {
            get { return this.auralMaterials1; }
            set
            {
                if (this.auralMaterials1 != value)
                {
                    this.auralMaterials1 = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(7)]
        public uint AuralMaterials2
        {
            get { return this.auralMaterials2; }
            set
            {
                if (this.auralMaterials2 != value)
                {
                    this.auralMaterials2 = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(8)]
        public uint AuralMaterials3
        {
            get { return this.auralMaterials3; }
            set
            {
                if (this.auralMaterials3 != value)
                {
                    this.auralMaterials3 = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(9)]
        public uint AuralPropertiesVersion
        {
            get { return this.auralPropertiesVersion; }
            set
            {
                if (this.auralPropertiesVersion != value)
                {
                    this.auralPropertiesVersion = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(10)]
        public uint AuralQuality
        {
            get { return this.auralQuality; }
            set
            {
                if (this.auralQuality != value)
                {
                    this.auralQuality = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(11)]
        public uint AuralAmbientObject
        {
            get { return this.auralAmbientObject; }
            set
            {
                if (this.auralAmbientObject != value)
                {
                    this.auralAmbientObject = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(13)]
        public ulong AmbienceFileInstanceId
        {
            get { return this.ambienceFileInstanceId; }
            set
            {
                if (this.ambienceFileInstanceId != value)
                {
                    this.ambienceFileInstanceId = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(14)]
        public byte IsOverrideAmbience
        {
            get { return this.isOverrideAmbience; }
            set
            {
                if (this.isOverrideAmbience != value)
                {
                    this.isOverrideAmbience = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(15)]
        public byte Unknown01
        {
            get { return this.unknown01; }
            set
            {
                if (this.unknown01 != value)
                {
                    this.unknown01 = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(18)]
        public uint Unused0
        {
            get { return this.unused0; }
            set
            {
                if (this.unused0 != value)
                {
                    this.unused0 = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(19)]
        public uint Unused1
        {
            get { return this.unused1; }
            set
            {
                if (this.unused1 != value)
                {
                    this.unused1 = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(20)]
        public uint Unused2
        {
            get { return this.unused2; }
            set
            {
                if (this.unused2 != value)
                {
                    this.unused2 = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(22)]
        public uint PlacementFlagsHigh
        {
            get { return this.placementFlagsHigh; }
            set
            {
                if (this.placementFlagsHigh != value)
                {
                    this.placementFlagsHigh = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(24)]
        public uint PlacementFlagsLow
        {
            get { return this.placementFlagsLow; }
            set
            {
                if (this.placementFlagsLow != value)
                {
                    this.placementFlagsLow = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(25)]
        public ulong SlotTypeSet
        {
            get { return this.slotTypeSet; }
            set
            {
                if (this.slotTypeSet != value)
                {
                    this.slotTypeSet = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(26)]
        public byte SlotDecoSize
        {
            get { return this.slotDecoSize; }
            set
            {
                if (this.slotDecoSize != value)
                {
                    this.slotDecoSize = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(27)]
        public ulong CatalogGroup
        {
            get { return this.catalogGroup; }
            set
            {
                if (this.catalogGroup != value)
                {
                    this.catalogGroup = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(28)]
        public byte StateUsage
        {
            get { return this.stateUsage; }
            set
            {
                if (this.stateUsage != value)
                {
                    this.stateUsage = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(29)]
        public ColorList Colors
        {
            get { return this.colors; }
            set
            {
                if (this.colors != value)
                {
                    this.colors = new ColorList(this.OnResourceChanged, value);
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(30)]
        public uint FenceHeight
        {
            get { return this.fenceHeight; }
            set
            {
                if (this.fenceHeight != value)
                {
                    this.fenceHeight = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(36)]
        public byte IsStackable
        {
            get { return this.isStackable; }
            set
            {
                if (this.isStackable != value)
                {
                    this.isStackable = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(37)]
        public byte CanItemDepreciate
        {
            get { return this.canItemDepreciate; }
            set
            {
                if (this.canItemDepreciate != value)
                {
                    this.canItemDepreciate = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(38)]
        public TGIBlock FallbackObjectKey
        {
            get
            {
                if (this.version < 0x00000019)
                {
                    throw new InvalidOperationException();
                }
                return this.fallbackObjectKey;
            }
            set
            {
                if (this.version < 0x00000019)
                {
                    throw new InvalidOperationException();
                }
                if (this.fallbackObjectKey != value)
                {
                    this.fallbackObjectKey = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
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
                if (this.version < 0x00000019)
                {
                    res.Remove("FallbackObjectKey");
                }
                if (this.auralPropertiesVersion < 2)
                {
                    res.Remove("AuralAmbientObject");
                }
                if (this.auralPropertiesVersion != 3)
                {
                    res.Remove("AmbienceFileInstanceId");
                    res.Remove("IsOverrideAmbience");
                }
                if (this.auralPropertiesVersion != 4)
                {
                    res.Remove("Unknown01");
                }
                return res;
            }
        }

        protected virtual void Parse(Stream s)
        {
            var reader = new BinaryReader(s);
            this.version = reader.ReadUInt32();
            this.commonA = new CatalogCommon(COBJResource.kRecommendedApiVersion, this.OnResourceChanged, s);

            this.auralMaterialsVersion = reader.ReadUInt32();
            this.auralMaterials1 = reader.ReadUInt32();
            this.auralMaterials2 = reader.ReadUInt32();
            this.auralMaterials3 = reader.ReadUInt32();
            this.auralPropertiesVersion = reader.ReadUInt32();
            this.auralQuality = reader.ReadUInt32();
            if (this.auralPropertiesVersion > 1)
            {
                this.auralAmbientObject = reader.ReadUInt32();
            }
            if (this.auralPropertiesVersion == 3)
            {
                this.ambienceFileInstanceId = reader.ReadUInt64();
                this.isOverrideAmbience = reader.ReadByte();
            }
            if (this.auralPropertiesVersion == 4)
            {
                this.unknown01 = reader.ReadByte();
            }
            this.unused0 = reader.ReadUInt32();
            this.unused1 = reader.ReadUInt32();
            this.unused2 = reader.ReadUInt32();
            this.placementFlagsHigh = reader.ReadUInt32();
            this.placementFlagsLow = reader.ReadUInt32();
            this.slotTypeSet = reader.ReadUInt64();
            this.slotDecoSize = reader.ReadByte();
            this.catalogGroup = reader.ReadUInt64();
            this.stateUsage = reader.ReadByte();
            this.colors = new ColorList(this.OnResourceChanged, s);
            this.fenceHeight = reader.ReadUInt32();
            this.isStackable = reader.ReadByte();
            this.canItemDepreciate = reader.ReadByte();

            if (this.version >= 0x19)
            {
                this.fallbackObjectKey = new TGIBlock(this.RecommendedApiVersion, this.OnResourceChanged, "ITG", s);
            }
        }

        protected override Stream UnParse()
        {
            var s = new MemoryStream();
            var writer = new BinaryWriter(s);

            writer.Write(this.version);
            if (this.commonA == null)
            {
                this.commonA = new CatalogCommon(COBJResource.kRecommendedApiVersion, this.OnResourceChanged);
            }
            this.commonA.UnParse(s);
            writer.Write(this.auralMaterialsVersion);
            writer.Write(this.auralMaterials1);
            writer.Write(this.auralMaterials2);
            writer.Write(this.auralMaterials3);
            writer.Write(this.auralPropertiesVersion);
            writer.Write(this.auralQuality);
            if (this.auralPropertiesVersion > 1)
            {
                writer.Write(this.auralAmbientObject);
            }
            if (this.auralPropertiesVersion == 3)
            {
                writer.Write(this.ambienceFileInstanceId);
                writer.Write(this.isOverrideAmbience);
            }
            if (this.auralPropertiesVersion == 4)
            {
                writer.Write(this.unknown01);
            }
            writer.Write(this.unused0);
            writer.Write(this.unused1);
            writer.Write(this.unused2);
            writer.Write(this.placementFlagsHigh);
            writer.Write(this.placementFlagsLow);
            writer.Write(this.slotTypeSet);
            writer.Write(this.slotDecoSize);
            writer.Write(this.catalogGroup);
            writer.Write(this.stateUsage);
            if (this.colors == null)
            {
                this.colors = new ColorList(this.OnResourceChanged);
            }
            this.colors.UnParse(s);
            writer.Write(this.fenceHeight);
            writer.Write(this.isStackable);
            writer.Write(this.canItemDepreciate);
            if (this.version >= 0x19)
            {
                this.fallbackObjectKey.UnParse(s);
            }
            return s;
        }
    }
}