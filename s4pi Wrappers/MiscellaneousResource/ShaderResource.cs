using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s4pi.Interfaces;
using System.IO;

namespace s4pi.Miscellaneous
{
    public class ShaderResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        public uint version { get; set; }
        public ShaderDB shaderDBSection { get; set; }
        public SamplerLink sampleLinkSection { get; set; }

        public ShaderResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            s.Position = 0;
            uint tag = r.ReadUInt32();
            if (FOURCC(tag) != "SPKG") throw new InvalidDataException(string.Format("Expected to read SPKG, read {0}", FOURCC(tag)));
            this.version = r.ReadUInt32();
            this.shaderDBSection = new ShaderDB(RecommendedApiVersion, OnResourceChanged, s);
            this.sampleLinkSection = new SamplerLink(RecommendedApiVersion, OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write((uint)FOURCC("SPKG"));
            w.Write(this.version);
            this.shaderDBSection.UnParse(ms);
            this.sampleLinkSection.UnParse(ms);
            return ms;
        }
        #endregion

        #region Sub-Class
        public class ShaderDB : AHandlerElement
        {
            public ShaderDB(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

            public PlatformShader[] platformShaders { get; set; }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                uint tag = r.ReadUInt32();
                if (FOURCC(tag) != "SHDB") throw new InvalidDataException(string.Format("Expected to read SHDB, read {0}", FOURCC(tag)));
                uint size = r.ReadUInt32();
                tag = r.ReadUInt32();
                if (FOURCC(tag) != "LINK") throw new InvalidDataException(string.Format("Expected to read LINK, read {0}", FOURCC(tag)));
                int psCount = r.ReadInt32();
                this.platformShaders = new PlatformShader[psCount];
                for (int i = 0; i < psCount; i++) this.platformShaders[i] = new PlatformShader(recommendedApiVersion, handler, s);

            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)FOURCC("SHDB"));
                long sizePosition = s.Position;
                w.Write(0); // need to rewrite later
                w.Write((uint)FOURCC("LINK"));
                w.Write(this.platformShaders.Length);
                foreach (var ps in this.platformShaders) ps.UnParse(s);
            }

            #region Sub Class
            public class PlatformShader : AHandlerElement
            {
                public PlatformShader(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

                public bool isPixelShader { get; set; }
                public ushort shaderPlatformMask { get; set; }
                private byte[] data;

                #region Data I/O
                public void Parse(Stream s)
                {
                    BinaryReader r = new BinaryReader(s);
                    this.isPixelShader = r.ReadBoolean();
                    this.shaderPlatformMask = r.ReadUInt16();
                    uint shaderSize = r.ReadUInt32(); // should be byte size + 8
                    uint tag = r.ReadUInt32();
                    if (FOURCC(tag) != "VSHD") throw new InvalidDataException(string.Format("Expected to read VSHD, read {0}", FOURCC(tag)));
                    uint byteSize = r.ReadUInt32();
                    if (byteSize != shaderSize - 8) throw new NotSupportedException("Data inconsistent with EA's documentation");
                    this.data = r.ReadBytes((int)byteSize);
                }

                public void UnParse(Stream s)
                {
                    BinaryWriter w = new BinaryWriter(s);
                    w.Write(this.isPixelShader);
                    w.Write(this.shaderPlatformMask);
                    long shaderSizePosition = s.Position;
                    w.Write(0);
                    w.Write((uint)FOURCC("VSHD"));
                    long byteSizePosition = s.Position;
                    w.Write(0);
                    w.Write(this.data);
                    long endPosition = s.Position;
                    s.Position = shaderSizePosition;
                    w.Write((uint)(endPosition - shaderSizePosition));
                    s.Position = byteSizePosition;
                    w.Write((uint)(endPosition - byteSizePosition));
                    s.Position = endPosition;
                }

                #region AHandlerElement Members
                public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
                public override List<string> ContentFields { get { var res = GetContentFields(requestedApiVersion, this.GetType()); res.Remove("Data"); return res; } }
                #endregion

                public byte[] Data { get { return this.data; } set { if (!this.data.Equals(value)) { OnElementChanged(); this.data = value; } } }
                public string Value { get { return ValueBuilder; } }
                #endregion
            }
            #endregion


            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { var res = GetContentFields(requestedApiVersion, this.GetType()); res.Remove("Data"); return res; } }
            #endregion

            public string Value { get { return ValueBuilder; } }
        }

        public class SamplerLink : AHandlerElement
        {
            public SamplerState[] samplerStates { get; set; }
            public SamplerLink(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                uint tag = r.ReadUInt32();
                if (FOURCC(tag) != "SMDB") throw new InvalidDataException(string.Format("Expected to read SMDB, read {0}", FOURCC(tag)));
                tag = r.ReadUInt32();
                if (FOURCC(tag) != "SMPS") throw new InvalidDataException(string.Format("Expected to read SMPS, read {0}", FOURCC(tag)));
                int count = r.ReadInt32();
                this.samplerStates = new SamplerState[count];
                for (int i = 0; i < count; i++) this.samplerStates[i] = new SamplerState(RecommendedApiVersion, handler, s);
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)FOURCC("SMDB"));
                w.Write((uint)FOURCC("SMPS"));
                w.Write(this.samplerStates.Length);
                foreach (var state in this.samplerStates) state.UnParse(s);
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region Sub-Class
            public class SamplerState : AHandlerElement
            {
                public StatePairs[] statePair { get; set; }

                public SamplerState(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

                public void Parse(Stream s)
                {
                    BinaryReader r = new BinaryReader(s);
                    uint count = r.ReadUInt32() / 8;
                    this.statePair = new StatePairs[count];
                    for (int i = 0; i < count; i++) this.statePair[i] = new StatePairs(RecommendedApiVersion, handler, s);
                }

                public void UnParse(Stream s)
                {
                    BinaryWriter w = new BinaryWriter(s);
                    w.Write(this.statePair.Length * 8);
                    foreach (var p in this.statePair) p.UnParse(s);
                }

                #region AHandlerElement Members
                public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
                public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
                #endregion

                #region Sub Class
                public class StatePairs : AHandlerElement
                {
                    private uint value1;
                    private uint value2;

                    public StatePairs(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

                    public void Parse(Stream s)
                    {
                        BinaryReader r = new BinaryReader(s);
                        this.value1 = r.ReadUInt32();
                        this.value2 = r.ReadUInt32();
                    }

                    public void UnParse(Stream s)
                    {
                        BinaryWriter w = new BinaryWriter(s);
                        w.Write(this.value1);
                        w.Write(this.value2);
                    }

                    #region AHandlerElement Members
                    public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
                    public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
                    #endregion
                    public uint Value1 { get { return this.value1; } set { if (!this.value1.Equals(value)) { OnElementChanged(); this.value1 = value; } } }
                    public uint Value2 { get { return this.value2; } set { if (!this.value2.Equals(value)) { OnElementChanged(); this.value2 = value; } } }
                    public string Value { get { return ValueBuilder; } }
                }
                #endregion
            }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }


        #endregion

        public string Value { get { return ValueBuilder; } }
    }
}
