using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CASPartResource
{
    public class GEOMListResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        public DataBlobHandler unknownHeader { get; set; }
        public TGIBlock currentInstance { get; set; }
        public uint unknown1 { get; set; }
        public uint unknown2 { get; set; }
        public uint unknown3 { get; set; }
        public ReferenceBlock[] referenceBlockList { get; set; }

        #endregion


        public GEOMListResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }
        
        #region Data I/O
        private void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            this.unknownHeader = new DataBlobHandler(recommendedApiVersion, OnResourceChanged, 20, s);
            this.currentInstance = new TGIBlock(recommendedApiVersion, OnResourceChanged, "ITG", s);
            this.unknown1 = r.ReadUInt32();
            this.unknown2 = r.ReadUInt32();
            this.unknown3 = r.ReadUInt32();

            int count = r.ReadInt32();
            this.referenceBlockList = new ReferenceBlock[count];
            for (int i = 0; i < count; i++)
                this.referenceBlockList[i] = new ReferenceBlock(s);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            this.unknownHeader.UnParse(ms);
            this.currentInstance.UnParse(ms);
            w.Write(this.unknown1);
            w.Write(this.unknown2);
            w.Write(this.unknown3);
            w.Write(this.referenceBlockList.Length);
            foreach (var block in this.referenceBlockList)
                block.UnParse(ms);

            ms.Position = 0;
            return ms;
        }

        #endregion

        #region Sub-Class

        public class ReferenceBlock
        {
            public uint unknown1 { get; set; }
            public DataBlobHandler unknownBytes { get; set; }
            TGIBlockList tgiList { get; set; }

            public ReferenceBlock(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.unknown1 = r.ReadUInt32();
                this.unknownBytes = new DataBlobHandler(recommendedApiVersion, null, r.ReadBytes(5));

                int count = r.ReadInt32();

                this.tgiList = new TGIBlockList(null);
                for(int i = 0; i < count; i++)
                {
                    this.tgiList.Add(new TGIBlock(recommendedApiVersion, null, "ITG", s));
                }
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.unknown1);
                unknownBytes.UnParse(s);
                w.Write(this.tgiList.Count);
                foreach (var tgi in this.tgiList)
                    tgi.UnParse(s);
            }

            public string Value
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("{0}Unknown1: 0x{1:X8}\n", "".PadLeft(5, ' '), this.unknown1);
                    sb.AppendFormat("{0}{1}\n", "".PadLeft(5, ' '), this.unknownBytes.Value);
                    sb.AppendFormat("{0}GEOM List Count: 0x{1:X8}\n", "".PadLeft(5, ' '), this.tgiList.Count);
                    foreach (var tgi in this.tgiList)
                        sb.AppendFormat("{0}{1}\n", "".PadLeft(10, ' '), tgi.Value);
                    return sb.ToString();
                }
            }
        }
        #endregion

        public string Value
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(this.unknownHeader.Value);
                sb.AppendLine(this.currentInstance.Value);
                sb.AppendFormat("Unknown1: 0x{0:X8}\n", this.unknown1);
                sb.AppendFormat("Unknown2: 0x{0:X8}\n", this.unknown2);
                sb.AppendFormat("Unknown3: 0x{0:X8}\n", this.unknown3);
                sb.AppendFormat("GEOM Reference List: Count: 0x{0:X8}\n", this.referenceBlockList.Length);
                foreach (var block in this.referenceBlockList)
                    sb.AppendLine(block.Value);
                return sb.ToString();
            }
        }
    }

    /// <summary>
    /// ResourceHandler for AUEVResource wrapper
    /// </summary>
    public class GEOMListResourceHandler : AResourceHandler
    {
        public GEOMListResourceHandler()
        {
            this.Add(typeof(GEOMListResource), new List<string>(new string[] { "0xAC16FBEC", }));
        }
    }
}
