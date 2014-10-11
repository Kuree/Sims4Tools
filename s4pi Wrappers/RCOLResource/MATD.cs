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
        public bool isVideoSurface { get; set; }
        public bool isPaintingSurface { get; set; }
        public MTNF Mtnf { get; set; }
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


            this.isVideoSurface = r.ReadUInt32() != 0;
            this.isPaintingSurface = r.ReadUInt32() != 0;
            this.Mtnf = new MTNF(RecommendedApiVersion, null, s, RCOLType);

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


            w.Write(this.isVideoSurface ? 1 : 0);
            w.Write(this.isPaintingSurface ? 1 : 0);

            this.Mtnf.UnParse(s);


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
