using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RCOLResource
{
    public class MTST : RCOLChunk
    {
        [ElementPriority(0)]
        public static RCOL.RCOLChunkType RCOLType { get { return RCOL.RCOLChunkType.MTST; } }

        public MTST(int APIversion, EventHandler handler, Stream s, TGIBlock currentTGI) : base(APIversion, handler, s, currentTGI) { }

        #region Attributes
        public uint version { get; set; }
        public uint namehash { get; set; }
        public uint defaultMaterial { get; set; }
        public Material[] materialList { get; set; }
        static bool checking = s4pi.Settings.Settings.Checking;
        const int recommendedApiVersion = 1;
        #endregion

        #region Data I/O
        protected override void Parse(System.IO.Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);
            uint tag = r.ReadUInt32();
            if (tag != (uint)RCOLType) throw new InvalidDataException(string.Format("Except to read 0x{0:8X}, read 0x{1:8X}", RCOLType, tag));
            this.version = r.ReadUInt32();
            this.namehash = r.ReadUInt32();
            this.defaultMaterial = r.ReadUInt32();
            uint count = r.ReadUInt32();
            this.materialList = new Material[count];
            for (uint i = 0; i < count; i++) this.materialList[i] = new Material(RecommendedApiVersion, handler, s);
        }

        protected internal override void UnParse(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write((uint)RCOLType);
            w.Write(this.version);
            w.Write(this.namehash);
            w.Write(this.defaultMaterial);
            w.Write(this.materialList.Length);
            foreach (var m in this.materialList) m.UnParse(s);
        }
        #endregion


        #region Sub Class
        public enum State : uint
        {
            Default = 0x2EA8FB98,
            Dirty = 0xEEAB4327,
            VeryDirty = 0x2E5DF9BB,
            Burnt = 0xC3867C32,
            Clogged = 0x257FB026,
            carLightsOff = 0xE4AF52C1,
        }

        public class Material : AHandlerElement
        {
            public Material(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s);}

            #region Attributes
            public uint mat { get; set; }
            public State stateID { get; set; }
            public uint varientID { get; set; }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region Data I/O
            protected void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.mat = r.ReadUInt32();
                this.stateID = (State)r.ReadUInt32();
                this.varientID = r.ReadUInt32();
            }

            protected internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.mat);
                w.Write((uint)this.stateID);
                w.Write(this.varientID);
            }
            #endregion
        }
        #endregion

        #region Content Fields
        public override string RCOLTag { get { return RCOLType.ToString(); } }
        #endregion
    }
}
