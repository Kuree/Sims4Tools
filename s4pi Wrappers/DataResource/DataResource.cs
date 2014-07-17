using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.DataResource
{
    public class DataResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        static bool checking = s4pi.Settings.Settings.Checking;


        #region Attributes
        public uint version { get; set; }
        public List<Structure> StructureList { get; set; }
        public List<Data> DataList { get; set; }
        #endregion

        public DataResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }
        
        
        
        public void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);

            uint magic = r.ReadUInt32();

            if (checking) if (magic != FOURCC("DATA"))
                    throw new InvalidDataException(String.Format("Expected magic tag 0x{0:X8}; read 0x{1:X8}; position 0x{2:X8}",
                        FOURCC("AUEV"), magic, s.Position));

            version = r.ReadUInt32();

            uint dataTableOffset = r.ReadUInt32() + (uint)r.BaseStream.Position - 4;

            uint dataCount = r.ReadUInt32();

            uint structureTableOffest;
            //= r.ReadUInt32() + (uint)r.BaseStream.Position -4;
            if (!Util.GetOffset(r, out structureTableOffest))
                throw new InvalidDataException();

            uint structureTableCount = r.ReadUInt32();

            // Structure table

            r.BaseStream.Position = structureTableOffest;
            this.StructureList = new List<Structure>();

            for (int i = 0; i < structureTableCount; i++)
            {
                this.StructureList.Add(new Structure(r));
            }

            r.BaseStream.Position = dataTableOffset;
            this.DataList = new List<Data>();
            for(int i = 0; i < dataCount; i++)
            {
                this.DataList.Add(new Data(r));
            }
            
        }

        protected override Stream UnParse()
        {
            throw new NotImplementedException();
        }



        public class Structure
        {
            public uint NameOffset { get; set; }
            public uint nameHash { get; set; }
            public uint Unknown08 { get; set; }
            public uint Size { get; set; }
            public uint FieldTableOffset { get { return filedTableOffset; } }
            public uint FieldCount { get; set; }
            public List<Field> FieldList { get; set; }
            public bool IsNull { get; set; }
            public string Name { get; set; }

            uint nameOffset;
            uint filedTableOffset;
            public Structure(BinaryReader r)
            {

                Util.GetOffset(r, out nameOffset);
                
                nameHash = r.ReadUInt32();
                Name = Util.GetString(r, nameOffset);
                Unknown08 = r.ReadUInt32();
                Size = r.ReadUInt32();
                if (!Util.GetOffset(r, out filedTableOffset))
                    return;
                //if (FieldTableOffset == 0x80000000) { this.IsNull = true; return; } else { FieldTableOffset += (uint)r.BaseStream.Position - 4; IsNull = false; }
                FieldCount = r.ReadUInt32();

                r.BaseStream.Position = FieldTableOffset;
                FieldList = new List<Field>();
                for (int i = 0; i < FieldCount; i++)
                {
                    FieldList.Add(new Field(r));
                }

            }
        }

        public class Field
        {

            public uint NameOffset { get { return nameOffset; } }
            public uint NameHash { get { return nameHash; } }
            public uint Type { get; set; }
            public uint DataOffset { get; set; }
            public uint Unknown10Offset { get; set; }
            public string Name { get; set; }
            public bool IsNull { get; set; }

            uint nameOffset;
            uint nameHash;
            public Field(BinaryReader r)
            {

                Util.GetOffset(r, out nameOffset);

                Name = Util.GetString(r, nameOffset);
                nameHash = r.ReadUInt32();
                Type = r.ReadUInt32();
                DataOffset = r.ReadUInt32() + (uint)r.BaseStream.Position - 4;
                Unknown10Offset = r.ReadUInt32() + (uint)r.BaseStream.Position - 4;
            }
        }

        public class Data
        {
            uint NameOffset { get { return nameOffset; } }
            uint name_hash { get; set; }
            uint structure_offset { get; set; }
            uint unknown_0C { get; set; }
            uint unknown_10 { get; set; }
            uint field_offset { get; set; }
            uint field_count { get; set; }
            bool IsNull { get; set; }
            string Name { get; set; }

            uint nameOffset;
            public Data(BinaryReader r)
            {
                Util.GetOffset(r, out nameOffset);

                Name = Util.GetString(r, nameOffset);
                //Name = Util.GetString(r, name_offset + r.BaseStream.Position - 4);
                name_hash = r.ReadUInt32();
                structure_offset = r.ReadUInt32();
                unknown_0C = r.ReadUInt32();
                unknown_10 = r.ReadUInt32();
                field_offset = r.ReadUInt32() + (uint)r.BaseStream.Position - 4;
                field_count = r.ReadUInt32();
            }
        }
    }
}
