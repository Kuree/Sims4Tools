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
        public uint tgiOffset { get; set; }
        public uint presetCount { get; set; }
        public string name { get; set; }
        public float sortPriority { get; set; }
        public byte[] unknown1 { get; set; }

        public UInt32[] unknown2;
        public DataBlobHandler unknown3 { get; set; }
        public uint[] unknown4 { get; set; }
        public DataBlobHandler unknown5 { get; set; }

        public UnknownClass[] unknown6 { get; set; }

        public DataBlobHandler unknown7 { get; set; }

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
            if (presetCount != 0) Debug.WriteLine("Found non-zero one");
            name = BigEndianUnicodeString.Read(s);

            sortPriority = r.ReadSingle();
            unknown1 = r.ReadBytes(23);
            uint count = r.ReadUInt32();
            unknown2 = new uint[count];
            for (uint i = 0; i < count; i++)
                unknown2[i] = r.ReadUInt32();

            unknown3 = new DataBlobHandler(1, null, r.ReadBytes(2 * 3 * 4 + 1 + 2));

            byte count2 = r.ReadByte();
            unknown4 = new uint[count2];
            for (byte i = 0; i < count2; i++)
                unknown4[i] = r.ReadUInt32();

            unknown5 = new DataBlobHandler(1, null, r.ReadBytes(2 * 4));

            byte count3 = r.ReadByte();
            unknown6 = new UnknownClass[count3];
            for (byte i = 0; i < count3; i++)
                unknown6[i] = new UnknownClass(r);

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
        public uint Version { get { return version; } }
        public String Value { get { return ValueBuilder; } }
        #endregion



        #region Sub-Class
        public class UnknownClass
        {
            public uint unknown1 { get; set; }
            public DataBlobHandler unknown2 { get; set; }
            public byte[] indexList { get; set; }
            public UnknownClass(BinaryReader r)
            {
                unknown1 = r.ReadByte();
                unknown2 = new DataBlobHandler(1, null, r.ReadBytes(16));
                indexList = new byte[r.ReadByte()];
                for (int i = 0; i < indexList.Length; i++)
                    indexList[i] = r.ReadByte();
            }
        }
        #endregion
    }

    public class CASPartResourceTS4Handler : AResourceHandler
    {
        public CASPartResourceTS4Handler()
        {
            this.Add(typeof(CASPartResourceTS4), new List<string>(new string[] { "0x034AEECB", }));
        }
    }

}
