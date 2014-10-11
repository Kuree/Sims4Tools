using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RCOLResource
{
    public class MTRL : AHandlerElement
    {
        static bool checking = s4pi.Settings.Settings.Checking;
        RCOL.RCOLChunkType type;
        const int recommendedApiVersion = 1;
        uint mtrlUnknown1;
        ushort mtrlUnknown2;
        ushort mtrlUnknown3;
        public ShaderDataList sdList { get; set; }

        public MTRL(int APIversion, EventHandler handler, Stream s, RCOL.RCOLChunkType type) : base(APIversion, handler) { this.type = type; Parse(s); }

        private void Parse(Stream s)
        {
            long start = s.Position;
            BinaryReader r = new BinaryReader(s);
            uint mtnfTag = r.ReadUInt32();
            if (checking) if (mtnfTag != (uint)FOURCC("MTRL"))
                    throw new InvalidDataException(String.Format("Invalid mtnfTag read: '{0}'; expected: 'MTRL'; at 0x{1:X8}", FOURCC(mtnfTag), s.Position));
            mtrlUnknown1 = r.ReadUInt32();
            mtrlUnknown2 = r.ReadUInt16();
            mtrlUnknown3 = r.ReadUInt16();
            //r.ReadInt32();
            this.sdList = new ShaderDataList(handler, s, this.type, start );
        }

        public void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            int start = (int)s.Position;
            w.Write((int)FOURCC("MTRL"));
            w.Write(mtrlUnknown1);
            w.Write(mtrlUnknown2);
            w.Write(mtrlUnknown2);
            int offset = 0;
            foreach(var data in this.sdList)
            {
                data.UnParse(s, start, ref offset);
            }
        }


        public string Value { get { return ValueBuilder; } }


        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion
    }
}
