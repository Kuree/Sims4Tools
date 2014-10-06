using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s4pi.Interfaces;
using System.IO;

namespace RCOLResource
{
    public class MTNF : AHandlerElement
    {
        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        const int recommendedApiVersion = 1;
        public uint mtnfUnknown1;
        public ShaderDataList sdList { get; set; }
        RCOL.RCOLChunkType type;
        #endregion

        public MTNF(int APIversion, EventHandler handler, Stream s, RCOL.RCOLChunkType type) : base(APIversion, handler) { this.type = type; Parse(s); }

        private void Parse(Stream s)
        {
            long start = s.Position;
            BinaryReader r = new BinaryReader(s);
            uint mtnfTag = r.ReadUInt32();
            if (checking) if (mtnfTag != (uint)FOURCC("MTNF") && mtnfTag != (uint)FOURCC("MTRL"))
                    throw new InvalidDataException(String.Format("Invalid mtnfTag read: '{0}'; expected: 'MTNF' or 'MTRL'; at 0x{1:X8}", FOURCC(mtnfTag), s.Position));
            mtnfUnknown1 = r.ReadUInt32();
            //r.ReadInt32();
            this.sdList = new ShaderDataList(handler, s, this.type, start );
        }


        public string Value { get { return ValueBuilder; } }


        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion
    }
}
