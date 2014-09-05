using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;
using System.Text;
using System.Diagnostics;

namespace CatalogResource
{
    public class ObjectCatalogResourceTS4 : AResource
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
        TGIBlockList components;
        string materialVariant;
        byte unknown1;
        uint simoleonPrice;
        float positiveEnvironmentScore;
        float negativeEnvironmentScore;
        UInt32 thumbnailGeometryState;
        bool Unknown2;
        float[] environmentScoreEmotionTags;
        float[] environmentScores;
        ulong unknown3;
        bool isBaby;
        byte[] unknown4;
        byte[] data;
        #endregion

        #region Constructors
        public ObjectCatalogResourceTS4(int APIversion, Stream s) : base(APIversion, s) { Parse(s); }   
        #endregion

        #region Data I/O
        protected void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            this.version = r.ReadUInt16();
            long tablePosition = r.ReadUInt32();

            r.BaseStream.Position = tablePosition;
            ushort entryCount = r.ReadUInt16();
            propertyIDList = new List<PropertyID>();
            for(ushort i  = 0; i < entryCount; i++)
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
                        this.components = ReadTGIBlock(r, OnResourceChanged);
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
                        this.environmentScoreEmotionTags = new float[count];
                        for (int m = 0; m < count; m++) this.environmentScoreEmotionTags[m] = r.ReadSingle();
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
            MemoryStream s = new MemoryStream(this.data);
            BinaryWriter w = new BinaryWriter(s);


            //w.Write(this.version);
            //w.Write(0U); // table position
            //List<ulong> positionTable = new List<ulong>(this.propertyIDList.Count);
            

            s.Position = 0;
            return s;
        }


        private static TGIBlockList ReadTGIBlock(BinaryReader r, EventHandler handler)
        {
            int count = r.ReadInt32() / 4;
            List<TGIBlock> tgiblockList = new List<TGIBlock>(count);
            for (int i = 0; i < count; i++)
            {
                ulong instance = r.ReadUInt64();
                instance = (instance << 32) | (instance >> 32); // swap instance
                uint type = r.ReadUInt32();
                uint group = r.ReadUInt32();
                tgiblockList.Add(new TGIBlock(recommendedApiVersion, handler, type, group, instance));
            }
            TGIBlockList result = new TGIBlockList(handler, tgiblockList);
            return result;
        }

        private static string ReadString(BinaryReader r, EventHandler handler)
        {
            int count = r.ReadInt32();
            return Encoding.ASCII.GetString(r.ReadBytes(count));     
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
                foreach(var flag in Enum.GetValues(typeof(PropertyID)))
                {
                    if(!this.propertyIDList.Contains((PropertyID)flag))
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
        public enum PropertyID: uint
        {
            Name = 0xE7F07786, // string
            Tuning = 0x790FA4BC, //string
            TuningID = 0xB994039B, //ulong
            Icon = 0xCADED888, // swapped ITG list
            Rig = 0xE206AE4F, // swapped ITG list
            Slot = 0x8A85AFF3, // swapped ITG list
            Model = 0x8D20ACC6, // swapped ITG list
            Footprint = 0x6C737AD8, // swapped ITG list
            Components = 0xE6E421FB,  // swapped ITG list
            MaterialVariant = 0xECD5A95F, // string
            Unknown1 = 0xAC8E1BC0 , //uint8 byte
            SimoleonPrice = 0xE4F4FAA4, // uint32
            PositiveEnvironmentScore = 0x7236BEEA , // float
            NegativeEnvironmentScore = 0x44FC7512, // float
            ThumbnailGeometryState = 0x4233F8A0,  // uint 32
            Unknown2 = 0xEC3712E6 , // bool
            EnvironmentScoreEmotionTags = 0x2172AEBE, // uint16 array
            EnvironmentScores = 0xDCD08394, // float array
            Unknown3 = 0x52F7F4BC,  // ulong
            IsBaby = 0xAEE67A1C, // bool
            Unknown4 = 0xF3936A90, // byte array
        }
        #endregion


        #region Content Fields
        public string Value { get { return ValueBuilder; } }
        public ushort Version { get { return version; } }
        public string Name { get { return name; } }
        public string Tuning { get { if (!this.propertyIDList.Contains(PropertyID.Tuning)) { throw new InvalidDataException(); } return this.tuning; } }
        public ulong TuningID { get { if (!this.propertyIDList.Contains(PropertyID.TuningID)) { throw new InvalidDataException(); } return this.tuningID; } }
        public TGIBlockList Icon { get { if (!this.propertyIDList.Contains(PropertyID.TuningID)) { throw new InvalidDataException(); } return this.icon; } }
        public TGIBlockList Rig { get { if (!this.propertyIDList.Contains(PropertyID.Rig)) { throw new InvalidDataException(); } return this.rig; } }
        public TGIBlockList Slot { get { if (!this.propertyIDList.Contains(PropertyID.Slot)) { throw new InvalidDataException(); } return this.slot; } }
        public TGIBlockList Model { get { if (!this.propertyIDList.Contains(PropertyID.Model)) { throw new InvalidDataException(); } return this.model; } }
        public TGIBlockList Footprint { get { if (!this.propertyIDList.Contains(PropertyID.Footprint)) { throw new InvalidDataException(); } return this.footprint; } }
        public TGIBlockList Components { get { if (!this.propertyIDList.Contains(PropertyID.Components)) { throw new InvalidDataException(); } return this.components; } }
        public string MaterialVariant { get { if (!this.propertyIDList.Contains(PropertyID.MaterialVariant)) { throw new InvalidDataException(); } return this.materialVariant; } }
        public UInt32 SimoleonPrice { get { if (!this.propertyIDList.Contains(PropertyID.SimoleonPrice)) { throw new InvalidDataException(); } return this.simoleonPrice; } }
        public float PositiveEnvironmentScore { get { if (!this.propertyIDList.Contains(PropertyID.PositiveEnvironmentScore)) { throw new InvalidDataException(); } return this.positiveEnvironmentScore; } }
        public float NegativeEnvironmentScore { get { if (!this.propertyIDList.Contains(PropertyID.NegativeEnvironmentScore)) { throw new InvalidDataException(); } return this.negativeEnvironmentScore; } }
        public UInt32 ThumbnailGeometryState { get { if (!this.propertyIDList.Contains(PropertyID.ThumbnailGeometryState)) { throw new InvalidDataException(); } return this.thumbnailGeometryState; } }
        public float[] EnvironmentScoreEmotionTags { get { if (!this.propertyIDList.Contains(PropertyID.EnvironmentScoreEmotionTags)) { throw new InvalidDataException(); } return this.environmentScoreEmotionTags; } }
        public float[] EnvironmentScores { get { if (!this.propertyIDList.Contains(PropertyID.EnvironmentScores)) { throw new InvalidDataException(); } return this.environmentScores; } }
        public bool IsBaby { get { if (!this.propertyIDList.Contains(PropertyID.IsBaby)) { throw new InvalidDataException(); } return this.isBaby; } }
        #endregion
    }

    public class ObjectCatalogResourceTS4Handler : AResourceHandler
    {
        public ObjectCatalogResourceTS4Handler()
        {
            this.Add(typeof(ObjectCatalogResourceTS4), new List<string>(new string[] { "0xC0DB5AE7", }));
        }
    }
}
