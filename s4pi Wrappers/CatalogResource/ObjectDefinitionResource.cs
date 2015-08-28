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
using s4pi.Interfaces;
using System.Text;
using System.Diagnostics;

namespace CatalogResource
{
    public class ObjectDefinitionResource : AResource
    {
        #region Attributes
        const int recommendedApiVersion = 1;
        ushort version;
        string name;
        string tuning;
        ulong tuningID;
        private List<PropertyID> propertyIDList;
        TGIBlockList icon;
        TGIBlockList rig;
        TGIBlockList slot;
        TGIBlockList model;
        TGIBlockList footprint;
        SimpleList<UInt32> components;
        string materialVariant;
        byte unknown1;
        uint simoleonPrice;
        float positiveEnvironmentScore;
        float negativeEnvironmentScore;
        UInt32 thumbnailGeometryState;
        bool Unknown2;
        UInt16[] environmentScoreEmotionTags;
        float[] environmentScores;
        ulong unknown3;
        bool isBaby;
        byte[] unknown4;
        byte[] data;
        #endregion

        #region Constructors
        public ObjectDefinitionResource(int APIversion, Stream s) : base(APIversion, s) { Parse(s); }
        #endregion

        #region Data I/O
        protected void Parse(Stream s)
        {
            if (s == null) s = this.UnParse();
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt16();
            long tablePosition = r.ReadUInt32();

            r.BaseStream.Position = tablePosition;
            ushort entryCount = r.ReadUInt16();
            propertyIDList = new List<PropertyID>();
            for (ushort i = 0; i < entryCount; i++)
            {
                uint type = r.ReadUInt32();
                PropertyID id = (PropertyID)type;
                uint offset = r.ReadUInt32();
                long nextPosition = r.BaseStream.Position;
                r.BaseStream.Position = offset;
                int count = 0;
                this.propertyIDList.Add(id);
                switch (id)
                {
                    case PropertyID.Name:
                        this.name = ReadString(r, OnResourceChanged);
                        break;
                    case PropertyID.Tuning:
                        count = r.ReadInt32();
                        this.tuning = Encoding.ASCII.GetString(r.ReadBytes(count));
                        break;
                    case PropertyID.TuningID:
                        this.tuningID = r.ReadUInt64(); // it might be swapped
                        break;
                    case PropertyID.Icon:
                        this.icon = ReadTGIBlock(r, OnResourceChanged);
                        break;
                    case PropertyID.Rig:
                        this.rig = ReadTGIBlock(r, OnResourceChanged);
                        break;
                    case PropertyID.Slot:
                        this.slot = ReadTGIBlock(r, OnResourceChanged);
                        break;
                    case PropertyID.Model:
                        this.model = ReadTGIBlock(r, OnResourceChanged);
                        break;
                    case PropertyID.Footprint:
                        this.footprint = ReadTGIBlock(r, OnResourceChanged);
                        break;
                    case PropertyID.Components:
                        int componentCount = r.ReadInt32();
                        this.components = new SimpleList<uint>(OnResourceChanged);
                        for (int m = 0; m < componentCount; m++) this.components.Add(r.ReadUInt32());
                        break;
                    case PropertyID.MaterialVariant:
                        this.materialVariant = ReadString(r, OnResourceChanged);
                        break;
                    case PropertyID.Unknown1:
                        this.unknown1 = r.ReadByte();
                        break;
                    case PropertyID.SimoleonPrice:
                        this.simoleonPrice = r.ReadUInt32();
                        break;
                    case PropertyID.PositiveEnvironmentScore:
                        this.positiveEnvironmentScore = r.ReadSingle();
                        break;
                    case PropertyID.NegativeEnvironmentScore:
                        this.negativeEnvironmentScore = r.ReadSingle();
                        break;
                    case PropertyID.ThumbnailGeometryState:
                        this.thumbnailGeometryState = r.ReadUInt32();
                        break;
                    case PropertyID.Unknown2:
                        this.Unknown2 = r.ReadBoolean();
                        break;
                    case PropertyID.EnvironmentScoreEmotionTags:
                        count = r.ReadInt32();
                        this.environmentScoreEmotionTags = new UInt16[count];
                        for (int m = 0; m < count; m++) this.environmentScoreEmotionTags[m] = r.ReadUInt16();
                        break;
                    case PropertyID.EnvironmentScores:
                        count = r.ReadInt32();
                        this.environmentScores = new float[count];
                        for (int m = 0; m < count; m++) this.environmentScores[m] = r.ReadSingle();
                        break;
                    case PropertyID.Unknown3:
                        this.unknown3 = r.ReadUInt64();
                        break;
                    case PropertyID.IsBaby:
                        this.isBaby = r.ReadBoolean();
                        break;
                    case PropertyID.Unknown4:
                        count = r.ReadInt32();
                        this.unknown4 = new byte[count];
                        for (int m = 0; m < count; m++) this.unknown4[m] = r.ReadByte();
                        break;
                    default:
                        break;
                }

                r.BaseStream.Position = nextPosition;

            }

            s.Position = 0;
            this.data = r.ReadBytes((int)s.Length);
        }

        protected override Stream UnParse()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(this.version);
            long position1 = s.Position;
            w.Write(0U); // table position
            List<long> positionTable = new List<long>(this.propertyIDList.Count);

            for (int i = 0; i < this.propertyIDList.Count; i++)
            {
                positionTable.Add(w.BaseStream.Position);
                PropertyID id = this.propertyIDList[i];
                switch (id)
                {
                    case PropertyID.Name:
                        WriteString(w, name);
                        break;
                    case PropertyID.Tuning:
                        WriteString(w, tuning);
                        break;
                    case PropertyID.TuningID:
                        w.Write(this.tuningID);
                        break;
                    case PropertyID.Icon:
                        WriteTGIBlock(w, this.icon);
                        break;
                    case PropertyID.Rig:
                        WriteTGIBlock(w, this.rig);
                        break;
                    case PropertyID.Slot:
                        WriteTGIBlock(w, this.slot);
                        break;
                    case PropertyID.Model:
                        WriteTGIBlock(w, this.model);
                        break;
                    case PropertyID.Footprint:
                        WriteTGIBlock(w, this.footprint);
                        break;
                    case PropertyID.Components:
                        w.Write(this.components.Count);
                        foreach (var m in this.components) w.Write(m);
                        break;
                    case PropertyID.MaterialVariant:
                        WriteString(w, this.materialVariant);
                        break;
                    case PropertyID.Unknown1:
                        w.Write(this.unknown1);
                        break;
                    case PropertyID.SimoleonPrice:
                        w.Write(this.simoleonPrice);
                        break;
                    case PropertyID.PositiveEnvironmentScore:
                        w.Write(this.positiveEnvironmentScore);
                        break;
                    case PropertyID.NegativeEnvironmentScore:
                        w.Write(this.negativeEnvironmentScore);
                        break;
                    case PropertyID.ThumbnailGeometryState:
                        w.Write(this.thumbnailGeometryState);
                        break;
                    case PropertyID.Unknown2:
                        w.Write(this.Unknown2);
                        break;
                    case PropertyID.EnvironmentScoreEmotionTags:
                        w.Write(this.environmentScoreEmotionTags.Length);
                        foreach (var value in this.environmentScoreEmotionTags) w.Write(value);
                        break;
                    case PropertyID.EnvironmentScores:
                        w.Write(this.environmentScores.Length);
                        foreach (var value in this.environmentScores) w.Write(value);
                        break;
                    case PropertyID.Unknown3:
                        w.Write(this.unknown3);
                        break;
                    case PropertyID.IsBaby:
                        w.Write(this.isBaby);
                        break;
                    case PropertyID.Unknown4:
                        w.Write(this.unknown4.Length);
                        foreach (var value in this.unknown4) w.Write(value);
                        break;
                    default:
                        break;
                }
            }
            long position2 = s.Position;
            w.Write((ushort)this.propertyIDList.Count);
            for (int i = 0; i < this.propertyIDList.Count; i++)
            {
                w.Write((uint)this.propertyIDList[i]);
                w.Write((uint)positionTable[i]);
            }
            long position3 = s.Position;
            uint pos2 = Convert.ToUInt32(position2);
            s.Position = position1;
            w.Write(pos2); // table position
            s.Position = position3;
            return s;
        }


        private TGIBlockList ReadTGIBlock(BinaryReader r, EventHandler handler, TGIBlock.Order order = TGIBlock.Order.ITG)
        {
            int count = r.ReadInt32() / 4;
            List<TGIBlock> tgiblockList = new List<TGIBlock>(count);
            for (int i = 0; i < count; i++)
            {
                ulong instance = 0;
                uint type = 0;
                uint group = 0;
                if (order == TGIBlock.Order.ITG)
                {
                    instance = r.ReadUInt64();
                    instance = (instance << 32) | (instance >> 32); // swap instance
                    type = r.ReadUInt32();
                    group = r.ReadUInt32();

                }
                else if (order == TGIBlock.Order.TGI)
                {
                    type = r.ReadUInt32();
                    group = r.ReadUInt32();
                    instance = r.ReadUInt64();
                    instance = (instance << 32) | (instance >> 32); // swap instance
                }
                tgiblockList.Add(new TGIBlock(recommendedApiVersion, handler, type, group, instance));
            }
            TGIBlockList result = new TGIBlockList(handler, tgiblockList);
            return result;
        }

        private string ReadString(BinaryReader r, EventHandler handler)
        {
            int count = r.ReadInt32();
            return Encoding.ASCII.GetString(r.ReadBytes(count));
        }

        private void WriteTGIBlock(BinaryWriter w, TGIBlockList value, TGIBlock.Order order = TGIBlock.Order.ITG)
        {
            w.Write(value.Count * 4);
            foreach (var tgi in value)
            {
                ulong instance = (tgi.Instance << 32) | (tgi.Instance >> 32);
                if (order == TGIBlock.Order.ITG)
                {
                    w.Write(instance);
                    w.Write(tgi.ResourceType);
                    w.Write(tgi.ResourceGroup);
                }
                else if (order == TGIBlock.Order.TGI)
                {
                    w.Write(tgi.ResourceType);
                    w.Write(tgi.ResourceGroup);
                    w.Write(instance);
                }
            }
        }

        private void WriteString(BinaryWriter w, string value)
        {
            w.Write(value.Length);
            w.Write(Encoding.ASCII.GetBytes(value));
        }


        #endregion

        #region AApiVersionedFields
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                foreach (var flag in Enum.GetValues(typeof(PropertyID)))
                {
                    if (!this.propertyIDList.Contains((PropertyID)flag))
                    {
                        res.Remove(((PropertyID)flag).ToString());
                    }
                }
                return res;
            }
        }
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        #endregion

        #region Sub-classes
        public enum PropertyID : uint
        {
            Name = 0xE7F07786, // string
            Tuning = 0x790FA4BC, //string
            TuningID = 0xB994039B, //ulong
            Icon = 0xCADED888, // swapped ITG list
            Rig = 0xE206AE4F, // swapped ITG list
            Slot = 0x8A85AFF3, // swapped ITG list
            Model = 0x8D20ACC6, // swapped ITG list
            Footprint = 0x6C737AD8, // swapped ITG list
            Components = 0xE6E421FB,  // UInt32 List
            MaterialVariant = 0xECD5A95F, // string
            Unknown1 = 0xAC8E1BC0, //uint8 byte
            SimoleonPrice = 0xE4F4FAA4, // uint32
            PositiveEnvironmentScore = 0x7236BEEA, // float
            NegativeEnvironmentScore = 0x44FC7512, // float
            ThumbnailGeometryState = 0x4233F8A0,  // uint 32
            Unknown2 = 0xEC3712E6, // bool
            EnvironmentScoreEmotionTags = 0x2172AEBE, // uint16 array
            EnvironmentScores = 0xDCD08394, // float array
            Unknown3 = 0x52F7F4BC,  // ulong
            IsBaby = 0xAEE67A1C, // bool
            Unknown4 = 0xF3936A90, // byte array
        }


        #endregion


        #region Content Fields
        [ElementPriority(0)]
        public ushort Version { get { return version; } }
        [ElementPriority(1)]
        public string Name { get { return name; } set { if (!value.Equals(this.name)) { this.name = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public string Tuning { get { if (!this.propertyIDList.Contains(PropertyID.Tuning)) { throw new InvalidDataException(); } return this.tuning; } set { if (!value.Equals(this.tuning)) { this.tuning = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public ulong TuningID { get { if (!this.propertyIDList.Contains(PropertyID.TuningID)) { throw new InvalidDataException(); } return this.tuningID; } set { if (!value.Equals(this.tuningID)) { this.tuningID = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public bool IsBaby { get { if (!this.propertyIDList.Contains(PropertyID.IsBaby)) { throw new InvalidDataException(); } return this.isBaby; } set { if (!value.Equals(this.isBaby)) { this.isBaby = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(5)]
        public SimpleList<UInt32> Components { get { if (!this.propertyIDList.Contains(PropertyID.Components)) { throw new InvalidDataException(); } return this.components; } set { if (!value.Equals(this.components)) { this.components = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(6)]
        public string MaterialVariant { get { if (!this.propertyIDList.Contains(PropertyID.MaterialVariant)) { throw new InvalidDataException(); } return this.materialVariant; } set { if (!value.Equals(this.materialVariant)) { this.materialVariant = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(7)]
        public UInt32 SimoleonPrice { get { if (!this.propertyIDList.Contains(PropertyID.SimoleonPrice)) { throw new InvalidDataException(); } return this.simoleonPrice; } set { if (!value.Equals(this.simoleonPrice)) { this.simoleonPrice = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(8)]
        public float PositiveEnvironmentScore { get { if (!this.propertyIDList.Contains(PropertyID.PositiveEnvironmentScore)) { throw new InvalidDataException(); } return this.positiveEnvironmentScore; } set { if (!value.Equals(this.positiveEnvironmentScore)) { this.positiveEnvironmentScore = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(9)]
        public float NegativeEnvironmentScore { get { if (!this.propertyIDList.Contains(PropertyID.NegativeEnvironmentScore)) { throw new InvalidDataException(); } return this.negativeEnvironmentScore; } set { if (!value.Equals(this.negativeEnvironmentScore)) { this.negativeEnvironmentScore = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(10)]
        public UInt32 ThumbnailGeometryState { get { if (!this.propertyIDList.Contains(PropertyID.ThumbnailGeometryState)) { throw new InvalidDataException(); } return this.thumbnailGeometryState; } set { if (!value.Equals(this.thumbnailGeometryState)) { this.thumbnailGeometryState = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(11)]
        public UInt16[] EnvironmentScoreEmotionTags { get { if (!this.propertyIDList.Contains(PropertyID.EnvironmentScoreEmotionTags)) { throw new InvalidDataException(); } return this.environmentScoreEmotionTags; } set { if (!value.Equals(this.environmentScoreEmotionTags)) { this.environmentScoreEmotionTags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public float[] EnvironmentScores { get { if (!this.propertyIDList.Contains(PropertyID.EnvironmentScores)) { throw new InvalidDataException(); } return this.environmentScores; } set { if (!value.Equals(this.environmentScores)) { this.environmentScores = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public TGIBlockList Icon { get { if (!this.propertyIDList.Contains(PropertyID.Icon)) { throw new InvalidDataException(); } return this.icon; } set { if (!value.Equals(this.icon)) { this.icon = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public TGIBlockList Rig { get { if (!this.propertyIDList.Contains(PropertyID.Rig)) { throw new InvalidDataException(); } return this.rig; } set { if (!value.Equals(this.rig)) { this.rig = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public TGIBlockList Slot { get { if (!this.propertyIDList.Contains(PropertyID.Slot)) { throw new InvalidDataException(); } return this.slot; } set { if (!value.Equals(this.slot)) { this.slot = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(16)]
        public TGIBlockList Model { get { if (!this.propertyIDList.Contains(PropertyID.Model)) { throw new InvalidDataException(); } return this.model; } set { if (!value.Equals(this.model)) { this.model = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(17)]
        public TGIBlockList Footprint { get { if (!this.propertyIDList.Contains(PropertyID.Footprint)) { throw new InvalidDataException(); } return this.footprint; } set { if (!value.Equals(this.footprint)) { this.footprint = value; OnResourceChanged(this, EventArgs.Empty); } } }


        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    public class ObjectDefinitionResourceHandler : AResourceHandler
    {
        public ObjectDefinitionResourceHandler()
        {
            this.Add(typeof(ObjectDefinitionResource), new List<string>(new string[] { "0xC0DB5AE7", }));
        }
    }
}
