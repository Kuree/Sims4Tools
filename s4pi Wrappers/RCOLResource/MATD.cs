using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RCOLResource
{
    public class MATD : RCOLChunk
    {
        public static RCOL.RCOLChunkType RCOLType { get { return RCOL.RCOLChunkType.MATD; } }

        public MATD(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { Parse(s); }

        #region Attributes
        public uint version;
        static bool checking = s4pi.Settings.Settings.Checking;
        public uint materialNameHash { get; set; }
        public ShaderType shaderNameHash { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }
        public MTRL MTRL { get; set; }
        //public MTRL Mtrl { get; set; }
        #endregion

        #region Data I/O
        protected override void Parse(System.IO.Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            uint tag = r.ReadUInt32();
            if (tag != (uint)RCOLType) throw new InvalidDataException(string.Format("Except to read 0x{0:8X}, read 0x{1:8X}", RCOLType, tag));
            this.version = r.ReadUInt32();
            this.materialNameHash = r.ReadUInt32();
            this.shaderNameHash = (ShaderType)r.ReadUInt32();
            uint length = r.ReadUInt32();


            this.Unknown1 = r.ReadInt32();
            this.Unknown2 = r.ReadInt32();
            this.MTRL = new MTRL(RecommendedApiVersion, null, s, RCOLType);

        }

        protected internal override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write((uint)RCOLType);
            w.Write(this.version);
            w.Write(this.materialNameHash);
            w.Write((uint)this.shaderNameHash);

            long lengthPosition = s.Position;
            w.Write(0);


            w.Write(this.Unknown1);
            w.Write(this.Unknown2);

            this.MTRL.UnParse(s);


            long position = s.Position;
            s.Position = lengthPosition;
            w.Write(position - lengthPosition - 12);
            s.Position = position;
        }
        #endregion

        #region Content Fields
        public override string RCOLTag { get { return RCOLType.ToString(); } }
        #endregion
    }
}
