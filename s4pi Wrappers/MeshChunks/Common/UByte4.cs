using System;
using System.IO;
using s4pi.Interfaces;

namespace meshExpImp.ModelBlocks
{
    public class UByte4 : AHandlerElement
    {
        private byte mA;
        private byte mB;
        private byte mC;
        private byte mD;

        public UByte4(int APIversion, EventHandler handler) : base(APIversion, handler) {}
        public UByte4(int APIversion, EventHandler handler, UByte4 basis): this(APIversion, handler, basis.A,basis.B,basis.C,basis.D){}
        public UByte4(int APIversion, EventHandler handler, Stream s): base(APIversion, handler){Parse(s);}
        public UByte4(int APIversion, EventHandler handler, byte a, byte b, byte c, byte d) : base(APIversion, handler)
        {
            mA = a;
            mB = b;
            mC = c;
            mD = d;
        }

        public string Value { get { return ToString(); } }
        public override string ToString()
        {
            return String.Format("[{0:X2},{1:X2},{2:X2},{3:X2}]", mA, mB, mC, mD);
        }
        [ElementPriority(1)]
        public byte A
        {
            get { return mA; }
            set { if(mA!=value){mA = value; OnElementChanged();} }
        }
        [ElementPriority(2)]
        public byte B
        {
            get { return mB; }
            set { if(mB!=value){mB = value; OnElementChanged();} }
        }
        [ElementPriority(3)]
        public byte C
        {
            get { return mC; }
            set { if(mC!=value){mC = value; OnElementChanged();} }
        }
        [ElementPriority(4)]
        public byte D
        {
            get { return mD; }
            set { if(mD!=value){mD = value; OnElementChanged();} }
        }

        public virtual void Parse(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            mA = br.ReadByte();
            mB = br.ReadByte();
            mC = br.ReadByte();
            mD = br.ReadByte();
        }

        public virtual void UnParse(Stream s)
        {
            BinaryWriter bw = new BinaryWriter(s);
            bw.Write(mA);
            bw.Write(mB);
            bw.Write(mC);
            bw.Write(mD);
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new UByte4(0, handler, this); }

        public override System.Collections.Generic.List<string> ContentFields
        {
            get { return GetContentFields(requestedApiVersion,GetType()); }
        }

        public override int RecommendedApiVersion
        {
            get { return 1; }
        }
    }
}