using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.DataResource
{
    public class DataResource : AResource
    {
        const int recommendedApiVersion = 1;
        static bool checking = s4pi.Settings.Settings.Checking;


        #region Attributes
        private uint version = 0x100;
        private uint dataTablePosition;
        private uint structureTablePosition;
        private StructureList structureList;
        private DataList dataList;
        #endregion

        #region Constructors
        public DataResource(int APIversion, Stream s) 
            : base(APIversion, s) 
        { 
            if (stream == null) 
            { 
                stream = UnParse(); 
                OnResourceChanged(this, EventArgs.Empty); 
            } 
            stream.Position = 0; 
            Parse(stream); /* */
        }
        #endregion

        #region AResource
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        public void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);

            uint magic = r.ReadUInt32();

            if (checking) if (magic != FOURCC("DATA"))
                    throw new InvalidDataException(String.Format("Expected magic tag 0x{0:X8}; read 0x{1:X8}; position 0x{2:X8}",
                        FOURCC("DATA"), magic, s.Position));

            version = r.ReadUInt32();

            //dataTablePosition = r.ReadUInt32() + (uint)r.BaseStream.Position - 4;
            if (!Util.GetOffset(r, out dataTablePosition))
                throw new InvalidDataException(String.Concat("Invalid Data Table Position: 0x", dataTablePosition.ToString("X8")));

            int dataCount = r.ReadInt32();

            //structureTablePosition = r.ReadUInt32() + (uint)r.BaseStream.Position - 4;
            if (!Util.GetOffset(r, out structureTablePosition))
                throw new InvalidDataException(String.Concat("Invalid Structure Table Position: 0x", structureTablePosition.ToString("X8")));

            int structureCount = r.ReadInt32();

            // Structure table

            this.structureList = new StructureList(OnResourceChanged, structureTablePosition, structureCount, r);

            this.dataList = new DataList(OnResourceChanged, this, dataCount, r);
        }

        private const uint blank = 0;

        protected override Stream UnParse()
        {
            Stream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            if (this.structureList == null)
                this.structureList = new StructureList(OnResourceChanged);
            if (this.dataList == null)
                this.dataList = new DataList(OnResourceChanged, this);

            // Write Header with blank offsets
            w.Write((uint)FOURCC("DATA"));
            w.Write(this.version);
            w.Write(blank); // data table offset
            w.Write(this.dataList.Count);
            w.Write(blank); // structure table offset
            w.Write(this.structureList.Count);

            // Padding between header and data table?
            w.Write(blank);
            w.Write(blank);

            // Write Data Table with blank Name and Structure offsets
            this.dataTablePosition = (uint)s.Position;
            this.dataList.UnParse(w);

            // Write Structure Table blank Name offsets
            this.structureTablePosition = (uint)s.Position;

            // Write Names and set Name positions in data
            foreach (Structure structure in this.structureList)
            {
                foreach (Field field in structure.FieldTable)
                {
                    if (string.IsNullOrEmpty(field.Name))
                    {
                        field.NamePosition = Util.NullOffset;
                    }
                    else
                    {
                        field.NamePosition = (uint)s.Position;
                        Util.WriteString(w, field.Name);
                    }
                }
                if (string.IsNullOrEmpty(structure.Name))
                {
                    structure.NamePosition = Util.NullOffset;
                }
                else
                {
                    structure.NamePosition = (uint)s.Position;
                    Util.WriteString(w, structure.Name);
                }
            }
            foreach (Data data in this.dataList)
            {
                if (string.IsNullOrEmpty(data.Name))
                {
                    data.NamePosition = Util.NullOffset;
                }
                else
                {
                    data.NamePosition = (uint)s.Position;
                    Util.WriteString(w, data.Name);
                }
            }

            // Go back, calculate and write offsets
            this.structureList.WriteOffsets(w);
            this.dataList.WriteOffsets(w);

            return s;
        }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public uint DataTablePosition { get { return dataTablePosition; } set { if (dataTablePosition != value) { dataTablePosition = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public uint StructureTablePosition { get { return structureTablePosition; } set { if (structureTablePosition != value) { structureTablePosition = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public StructureList StructureTable { get { return structureList; } set { if (structureList != value) { structureList = value == null ? new StructureList(OnResourceChanged) : new StructureList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(5)]
        public DataList DataTable { get { return dataList; } set { if (dataList != value) { dataList = value == null ? new DataList(OnResourceChanged, this) : new DataList(OnResourceChanged, this, value); OnResourceChanged(this, EventArgs.Empty); } } }
        #endregion

        public string Value { get { return ValueBuilder; } }

        #region Sub-Types

        public class Structure : AHandlerElement, IEquatable<Structure>
        {
            private const int recommendedApiVersion = 1;

            private uint myPosition;

            #region Attributes
            private uint namePosition;
            private string name;
            private uint nameHash;
            private uint unknown08;
            private uint size;
            private uint fieldTablePosition;
            private uint fieldCount;
            private FieldList fieldList;
            #endregion

            #region Constructors
            public Structure(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Structure(int APIversion, EventHandler handler, BinaryReader r) : base(APIversion, handler) { Parse(r); }
            #endregion

            #region Data I/O
            private void Parse(BinaryReader r)
            {
                myPosition = (uint)r.BaseStream.Position;

                Util.GetOffset(r, out namePosition);

                name = Util.GetString(r, namePosition);
                nameHash = r.ReadUInt32();
                unknown08 = r.ReadUInt32();
                size = r.ReadUInt32();
                Util.GetOffset(r, out fieldTablePosition);
                fieldCount = r.ReadUInt32();
            }

            internal void ParseFieldTable(BinaryReader r)
            {
                if (this.fieldTablePosition == Util.NullOffset)
                {
                    this.fieldList = new FieldList(this.handler);
                }
                else
                {
                    r.BaseStream.Position = this.fieldTablePosition;
                    this.fieldList = new FieldList(this.handler, this.fieldCount, r);
                }
            }

            public void UnParse(BinaryWriter w)
            {
                myPosition = (uint)w.BaseStream.Position;

                if (this.name == null) this.name = "";
                this.nameHash = FNV32.GetHash(this.name);

                if (this.fieldList == null)
                    this.fieldList = new FieldList(this.handler);
                if (this.fieldList.Count == 0)
                    this.fieldTablePosition = Util.NullOffset;

                w.Write(Util.Zero32); // Name Offset
                w.Write(this.nameHash);
                w.Write(this.unknown08);
                w.Write(this.size);
                w.Write(Util.Zero32); // Field Table Offset
                w.Write(this.fieldList.Count);
            }

            public void WriteOffsets(BinaryWriter w)
            {
                Stream s = w.BaseStream;
                //long savedPos = s.Position;
                s.Position = myPosition;

                w.Write(this.namePosition == Util.NullOffset
                    ? this.namePosition : this.namePosition - myPosition);
                
                s.Position += 12; // Name Hash, Unknown 08, and Size

                w.Write(this.fieldTablePosition == Util.NullOffset
                    ? this.fieldTablePosition : this.fieldTablePosition - myPosition - 0x10);

                //s.Position = savedPos;

                this.fieldList.WriteOffsets(w);
            }
            #endregion

            #region AHandlerElement
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion
            
            #region IEquatable<Structure>
            public bool Equals(Structure other)
            {
                return name == other.name && nameHash == other.nameHash && unknown08 == other.unknown08 
                    && size == other.size && fieldTablePosition == other.fieldTablePosition && fieldList.Equals(other.fieldList);
            }
            public override bool Equals(object obj) { return obj is Structure && this.Equals(obj as Structure); }
            public override int GetHashCode()
            {
                return namePosition.GetHashCode() ^ name.GetHashCode() ^ nameHash.GetHashCode() ^ unknown08.GetHashCode() 
                    ^ size.GetHashCode() ^ fieldTablePosition.GetHashCode();
            }
            #endregion

            public uint GetPosition() { return myPosition; } 

            #region Content Fields
            [ElementPriority(1)]
            public uint NamePosition { get { return namePosition; } set { if (namePosition != value) { namePosition = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public string Name { get { return name; } set { if (name != value) { name = value == null ? "" : value; nameHash = FNV32.GetHash(name); OnElementChanged(); } } }
            [ElementPriority(3)]
            public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public uint Unknown8 { get { return unknown08; } set { if (unknown08 != value) { unknown08 = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public uint Size { get { return size; } set { if (size != value) { size = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public uint FieldTablePosition { get { return fieldTablePosition; } set { if (fieldTablePosition != value) { fieldTablePosition = value; OnElementChanged(); } } }
            [ElementPriority(8)]
            public FieldList FieldTable { get { return fieldList; } set { if (fieldList != value) { fieldList = value == null ? new FieldList(handler) : new FieldList(handler, value); OnElementChanged(); } } }
            #endregion

            public string Value { get { return ValueBuilder; } }
        }
        public class StructureList : DependentList<Structure>
        {
            public StructureList(EventHandler handler) : base(handler) { }
            public StructureList(EventHandler handler, uint structureTablePosition, int structureCount, BinaryReader r)
                : base(handler)
            {
                this.elementHandler = handler;
                this.Capacity = structureCount;

                int i;
                Structure structure;
                Stream s = r.BaseStream;
                //long savedPos = s.Position;
                s.Position = structureTablePosition;
                for (i = 0; i < structureCount; i++)
                {
                    structure = new Structure(0, elementHandler, r);
                    this.Add(structure);
                }
                for (i = 0; i < structureCount; i++)
                {
                    structure = this[i];
                    structure.ParseFieldTable(r);
                }
                //s.Position = savedPos;
            }
            public StructureList(EventHandler handler, IEnumerable<Structure> ilt) : base(handler, ilt) { }

            #region Data I/O
            public void UnParse(BinaryWriter w)
            {
                int i;
                int count = this.Count;
                Structure structure;
                // Write the structures
                for (i = 0; i < count; i++)
                {
                    structure = this[i];
                    structure.UnParse(w);
                }
                // Write the field tables
                Stream s = w.BaseStream;
                for (i = 0; i < count; i++)
                {
                    structure = this[i];
                    structure.FieldTablePosition = (uint)s.Position;
                    structure.FieldTable.UnParse(w);
                }
            }

            public void WriteOffsets(BinaryWriter w)
            {
                Structure structure;
                int count = this.Count;
                for (int i = 0; i < count; i++)
                {
                    structure = this[i];
                    structure.WriteOffsets(w);
                }
            }
            #endregion

            public Structure GetFromPosition(uint position)
            {
                if (position == Util.NullOffset) return null;
                Structure structure;
                for (int i = this.Count - 1; i >= 0; i--)
                {
                    structure = this[i];
                    if (structure.GetPosition() == position)
                        return structure;
                }
                return null;
            }

            protected override Structure CreateElement(Stream s) { throw new NotImplementedException(); }
            protected override void WriteElement(Stream s, Structure element) { throw new NotImplementedException(); }
        }

        public class Field : AHandlerElement, IEquatable<Field>
        {
            private uint myPosition;

            #region Attributes
            private uint namePosition;
            private string name;
            private uint nameHash;
            private uint type;
            private uint dataOffset;
            private uint unknown10Position;
            #endregion

            #region Constructors
            public Field(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Field(int APIversion, EventHandler handler, BinaryReader r) : base(APIversion, handler) { Parse(r); }
            #endregion

            #region Data I/O
            private void Parse(BinaryReader r)
            {
                myPosition = (uint)r.BaseStream.Position;

                Util.GetOffset(r, out namePosition);

                name = Util.GetString(r, namePosition);
                nameHash = r.ReadUInt32();
                type = r.ReadUInt32();
                dataOffset = r.ReadUInt32();
                Util.GetOffset(r, out unknown10Position);
            }

            public void UnParse(BinaryWriter w)
            {
                myPosition = (uint)w.BaseStream.Position;

                if (this.name == null) this.name = "";
                this.nameHash = FNV32.GetHash(this.name);

                w.Write(Util.Zero32); // Name Offset
                w.Write(this.nameHash);
                w.Write(this.type);
                w.Write(this.dataOffset);
                w.Write(Util.Zero32); // Unknown 10 Offset
            }

            public void WriteOffsets(BinaryWriter w)
            {
                Stream s = w.BaseStream;
                //long savedPos = s.Position;
                s.Position = myPosition;

                w.Write(this.namePosition == Util.NullOffset
                    ? this.namePosition : this.namePosition - myPosition);
                s.Position += 12; // Name Hash, Type, and Data Offset

                w.Write(this.unknown10Position == Util.NullOffset
                    ? this.unknown10Position : this.unknown10Position - myPosition - 0x10);

                //s.Position = savedPos;
            }
            #endregion

            #region AHandlerElement
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Field>
            public bool Equals(Field other)
            {
                return name == other.name && nameHash == other.nameHash && type == other.type 
                    && dataOffset == other.dataOffset && unknown10Position == other.unknown10Position;
            }
            public override bool Equals(object obj) { return obj is Field && this.Equals(obj as Field); }
            public override int GetHashCode()
            {
                return name.GetHashCode() ^ nameHash.GetHashCode() ^ type.GetHashCode() 
                    ^ dataOffset.GetHashCode() ^ unknown10Position.GetHashCode();
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint NamePosition { get { return namePosition; } set { if (namePosition != value) { namePosition = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public string Name { get { return name; } set { if (name != value) { name = value == null ? "" : value; nameHash = FNV32.GetHash(name); OnElementChanged(); } } }
            [ElementPriority(3)]
            public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public uint Type { get { return type; } set { if (type != value) { type = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public uint DataOffset { get { return dataOffset; } set { if (dataOffset != value) { dataOffset = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public uint Unknown10Position { get { return unknown10Position; } set { if (unknown10Position != value) { unknown10Position = value; OnElementChanged(); } } }
            #endregion

            public string Value { get { return ValueBuilder; } }            
        }
        public class FieldList : DependentList<Field>, IEquatable<FieldList>
        {
            public FieldList(EventHandler handler) : base(handler) { }
            public FieldList(EventHandler handler, uint fieldCount, BinaryReader r)
                : base(handler)
            {
                this.elementHandler = handler;
                this.Capacity = (int)fieldCount;

                for (int i = 0; i < fieldCount; i++)
                {
                    this.Add(new Field(0, elementHandler, r));
                }
            }
            public FieldList(EventHandler handler, IEnumerable<Field> ilt) : base(handler, ilt) { }

            #region Data I/O
            public void UnParse(BinaryWriter w)
            {
                foreach (Field field in this)
                {
                    field.UnParse(w);
                }
            }

            public void WriteOffsets(BinaryWriter w)
            {
                foreach (Field field in this)
                {
                    field.WriteOffsets(w);
                }
            }
            #endregion

            protected override Field CreateElement(Stream s) { throw new NotImplementedException(); }
            protected override void WriteElement(Stream s, Field element) { throw new NotImplementedException(); }

            public bool Equals(FieldList other)
            {
                if (other == null || this.Count != other.Count)
                    return false;
                for (int i = this.Count - 1; i >= 0; i--)
                {
                    if (!this[i].Equals(other[i]))
                        return false;
                }
                return true;
            }
        }

        public class Data : AHandlerElement, IEquatable<Data>
        {
            private uint myPosition;

            private DataResource owner;

            #region Attributes
            private uint namePosition;
            private string name;
            private uint nameHash;
            private uint structurePosition;
            private Structure structure;
            private uint unknown0C;
            private uint unknown10;
            private uint fieldPosition;
            private uint fieldCount;
            private bool isNull;
            private DataBlobHandler fieldData;
            #endregion

            #region Constructors
            public Data(int APIversion, EventHandler handler, DataResource owner) : base(APIversion, handler) { this.owner = owner; }
            public Data(int APIversion, EventHandler handler, DataResource owner, BinaryReader r) : base(APIversion, handler) { this.owner = owner; Parse(r); }
            #endregion

            #region Data I/O
            private void Parse(BinaryReader r)
            {
                myPosition = (uint)r.BaseStream.Position;

                Util.GetOffset(r, out namePosition);

                name = Util.GetString(r, namePosition);
                nameHash = r.ReadUInt32();
                if (Util.GetOffset(r, out structurePosition))
                {
                    structure = this.owner.structureList.GetFromPosition(structurePosition);
                    if (structure == null)
                        throw new InvalidDataException(string.Format("Invalid Structure Position: 0x{0:X8}", structurePosition));
                }
                else
                    structure = null;
                unknown0C = r.ReadUInt32();
                unknown10 = r.ReadUInt32();
                if (!Util.GetOffset(r, out fieldPosition))
                    throw new InvalidDataException("Invalid Field Offset: 0x80000000");
                fieldCount = r.ReadUInt32();
            }

            internal void ParseFieldData(uint length, Stream s)
            {
                s.Position = this.fieldPosition;
                this.fieldData = new DataBlobHandler(requestedApiVersion, handler, length, s);
            }

            public void UnParse(BinaryWriter w)
            {
                myPosition = (uint)w.BaseStream.Position;

                if (this.name == null) this.name = "";
                this.nameHash = FNV32.GetHash(this.name);

                w.Write(Util.Zero32); // Name Offset
                w.Write(this.nameHash);
                w.Write(Util.Zero32); // Structure Offset
                w.Write(this.unknown0C);
                w.Write(this.unknown10);
                w.Write(Util.Zero32); // Field Offset
                w.Write(this.fieldCount);
            }

            public void WriteOffsets(BinaryWriter w)
            {
                Stream s = w.BaseStream;
                //long savedPos = s.Position;
                s.Position = myPosition;

                w.Write(this.namePosition == Util.NullOffset 
                    ? this.namePosition : this.namePosition - myPosition);
                s.Position += 4; // Name Hash

                if (this.structure == null)
                {
                    this.structurePosition = Util.NullOffset;
                    w.Write(this.structurePosition);
                }
                else
                {
                    this.structurePosition = this.structure.GetPosition();
                    w.Write(this.structurePosition - myPosition - 0x08);
                }
                s.Position += 8; // Unknown 0C and Unknown 10
                
                w.Write(this.fieldPosition == Util.NullOffset
                    ? this.fieldPosition : this.fieldPosition - myPosition - 0x14);
                
                //s.Position = savedPos;
            }
            #endregion

            #region AHandlerElement
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Data>
            public bool Equals(Data other)
            {
                return namePosition == other.namePosition && name == other.name && nameHash == other.nameHash
                    && structurePosition == other.structurePosition && unknown0C == other.unknown0C && unknown10 == other.unknown10
                    && fieldPosition == other.fieldPosition && fieldCount == other.fieldCount && isNull == other.isNull;
            }
            public override bool Equals(object obj) { return obj is Data && this.Equals(obj as Data); }
            public override int GetHashCode()
            {
                return namePosition.GetHashCode() ^ name.GetHashCode() ^ nameHash.GetHashCode()
                    ^ structurePosition.GetHashCode() ^ unknown0C.GetHashCode() ^ unknown10.GetHashCode()
                    ^ fieldPosition.GetHashCode() ^ fieldCount.GetHashCode() ^ isNull.GetHashCode();
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint NamePosition { get { return namePosition; } set { if (namePosition != value) { namePosition = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public string Name { get { return name; } set { if (name != value) { name = value == null ? "" : value; nameHash = FNV32.GetHash(name); OnElementChanged(); } } }
            [ElementPriority(3)]
            public uint NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public uint StructurePosition { get { return structurePosition; } set { if (structurePosition != value) { structurePosition = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public uint Unknown0C { get { return unknown0C; } set { if (unknown0C != value) { unknown0C = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public uint Unknown10 { get { return unknown10; } set { if (unknown10 != value) { unknown10 = value; OnElementChanged(); } } }
            [ElementPriority(7)]
            public uint FieldPosition { get { return fieldPosition; } set { if (fieldPosition != value) { fieldPosition = value; OnElementChanged(); } } }
            [ElementPriority(8)]
            public uint FieldCount { get { return fieldCount; } set { if (fieldCount != value) { fieldCount = value; OnElementChanged(); } } }
            [ElementPriority(9)]
            public bool IsNull { get { return isNull; } set { if (isNull != value) { isNull = value; OnElementChanged(); } } }
            [ElementPriority(10)]
            public DataBlobHandler FieldData 
            { 
                get { return fieldData; } 
                set 
                {
                    if (value == null) 
                        throw new InvalidOperationException();
                    if (fieldData != value) 
                    { 
                        fieldData = new DataBlobHandler(requestedApiVersion, handler, value); 
                        OnElementChanged(); 
                    } 
                } 
            }
            #endregion

            public string Value { get { return ValueBuilder; } }
        }
        public class DataList : DependentList<Data>
        {
            private class FieldPosComparer : IComparer<Data>
            {
                public static readonly FieldPosComparer Singleton = new FieldPosComparer();
                public FieldPosComparer() { }
                public int Compare(Data x, Data y) { return (int)x.FieldPosition - (int)y.FieldPosition; }
            }

            private List<Data> fieldPosSorted;

            private DataResource owner;

            public DataList(EventHandler handler, DataResource owner) : base(handler) { this.owner = owner; }
            public DataList(EventHandler handler, DataResource owner, int dataCount, BinaryReader r)
                : base(handler)
            {
                this.elementHandler = handler;
                this.owner = owner;
                this.Capacity = dataCount;

                Stream s = r.BaseStream;
                int i;
                //long savedPos = s.Position;
                Data data;
                fieldPosSorted = new List<Data>(dataCount);
                s.Position = owner.dataTablePosition;
                for (i = 0; i < dataCount; i++)
                {
                    data = new Data(0, elementHandler, owner, r);
                    this.Add(data);
                    fieldPosSorted.Add(data);
                }
                //s.Position = savedPos;

                // Go back and read the actual data
                uint length;
                fieldPosSorted.Sort(FieldPosComparer.Singleton);
                for (i = 0; i < dataCount; i++)
                {
                    data = fieldPosSorted[i];
                    length = i < dataCount - 1 
                        ? fieldPosSorted[i + 1].FieldPosition - data.FieldPosition 
                        : owner.structureTablePosition - data.FieldPosition;
                    data.ParseFieldData(length, s);
                }
            }
            public DataList(EventHandler handler, DataResource owner, IEnumerable<Data> ilt) : base(handler, ilt) { this.owner = owner; }

            #region Data I/O
            public void UnParse(BinaryWriter w)
            {
                int i;
                int count = this.Count;
                // Write the headers 
                for (i = 0; i < count; i++)
                {
                    this[i].UnParse(w);
                }
                // Padding between headers and data?
                w.Write(Util.Zero32);
                w.Write(Util.Zero32);
                // Write the data
                Data data;
                Stream s = w.BaseStream;
                for (i = 0; i < count; i++)
                {
                    data = this[i];
                    data.FieldPosition = (uint)s.Position;
                    data.FieldData.UnParse(s);
                }
            }

            public void WriteOffsets(BinaryWriter w)
            {
                foreach (Data data in this)
                {
                    data.WriteOffsets(w);
                }
            }
            #endregion

            protected override Data CreateElement(Stream s) { throw new NotImplementedException(); }
            protected override void WriteElement(Stream s, Data element) { throw new NotImplementedException(); }
        }

        #endregion
    }

    public class DataResourceHandler : AResourceHandler
    {
        public DataResourceHandler()
        {
            this.Add(typeof(DataResource), new List<string>(new string[] { "0x545AC67A", }));
        }
    }
}
