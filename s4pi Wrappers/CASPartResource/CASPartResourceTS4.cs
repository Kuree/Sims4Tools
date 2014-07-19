using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace CASPartResource
{
    class CASPartResourceTS4 : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        public uint version;
        uint tgiOffset;
        uint presetCount;
        string name;
        float sortPriority;
        byte[] unknown1;
        IndexList<UInt32> unknown2;
        DataBlobHandler unknown3;
        uint[] unknown4;
        DataBlobHandler unknown5;
        List<UnknownClass> unknown6;
        DataBlobHandler unknown7;
        public TGIBlockList tgiList { get; set; }

        #endregion

        public CASPartResourceTS4(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            version = r.ReadUInt32();
            tgiOffset = r.ReadUInt32() + 8;
            presetCount = r.ReadUInt32();
            if (presetCount != 0) throw new Exception("Found non-zero one");
            name = BigEndianUnicodeString.Read(s);

            sortPriority = r.ReadSingle();
            unknown1 = r.ReadBytes(23);
            uint count = r.ReadUInt32();
            uint[] unknown2List = new uint[count];
            for (uint i = 0; i < count; i++)
                unknown2List[i] = r.ReadUInt32();
            unknown2 = new IndexList<uint>(OnResourceChanged, unknown2List);

            unknown3 = new DataBlobHandler(1, null, r.ReadBytes(2 * 3 * 4 + 1 + 2));

            byte count2 = r.ReadByte();
            unknown4 = new uint[count2];
            for (byte i = 0; i < count2; i++)
                unknown4[i] = r.ReadUInt32();

            unknown5 = new DataBlobHandler(1, null, r.ReadBytes(2 * 4));

            byte count3 = r.ReadByte();
            UnknownClass[] unknown6List = new UnknownClass[count3];
            for (byte i = 0; i < count3; i++)
                unknown6[i] = new UnknownClass(recommendedApiVersion, OnResourceChanged, s);
            //unknown6 = new IndexList<UnknownClass>(OnResourceChanged, unknown6List);

            unknown7 = new DataBlobHandler(1, null, r.ReadBytes(10));

            tgiList = new TGIBlockList(null);
            byte count4 = r.ReadByte();
            for (int i = 0; i < count4; i++)
                tgiList.Add(new TGIBlock(1, null, "IGT", s));
        }

        protected override Stream UnParse()
        {
            return new MemoryStream();
        }
        #endregion



        #region Content Fields
        [ElementPriority(0)]
        public uint Version { get { return version; } set { if (value != version)version = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(1)]
        public uint TGIoffset { get { return tgiOffset; } set { if (value != tgiOffset) tgiOffset = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(2)]
        public uint PresetCount { get { return presetCount; } set { if (value != presetCount) presetCount = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(3)]
        public string Name { get { return name; } set { if (!value.Equals(name)) name = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(4)]
        public float SortPriority { get { return sortPriority; } set { if (!value.Equals(sortPriority)) sortPriority = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(5)]
        public byte[] Unknown1 { get { return unknown1; } set { if (!unknown1.Equals(value)) unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(6)]
        public IndexList<UInt32> Unknown2 { get { return unknown2; } set { if (!value.Equals(unknown2)) unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } }
        
        
        
        public String Value { get { return ValueBuilder; } }
        #endregion



        #region Sub-Class
        public class UnknownClass : AHandlerElement, IEquatable<UnknownClass>
        {
            const int recommendedApiVersion = 1;


            public ushort unknown1 { get; set; }
            public DataBlobHandler unknown2 { get; set; }
            public byte[] indexList { get; set; }
            public UnknownClass(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public UnknownClass(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                unknown1 = r.ReadUInt16();
                unknown2 = new DataBlobHandler(1, null, r.ReadBytes(16));
                indexList = new byte[r.ReadByte()];
                for (int i = 0; i < indexList.Length; i++)
                    indexList[i] = r.ReadByte();
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            public string Value { get { return ValueBuilder; } }

            public bool Equals(UnknownClass other)
            {
                return false;
            }
        }

        //public class UnknownClassList : DependentList<UnknownClass>
        //{

        //}

        #endregion
    }

    public class CASPartResourceTS4Handler : AResourceHandler
    {
        public CASPartResourceTS4Handler()
        {
            if(s4pi.Settings.Settings.IsTS4)
                this.Add(typeof(CASPartResourceTS4), new List<string>(new string[] { "0x034AEECB", }));
        }
    }

}
