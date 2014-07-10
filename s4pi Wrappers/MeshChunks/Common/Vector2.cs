using System;
using System.Collections.Generic;
using s4pi.Interfaces;
using System.IO;

namespace meshExpImp.ModelBlocks
{
    public class Vector2 : AHandlerElement
    {
        protected float mX, mY;
        public Vector2(int APIversion, EventHandler handler, float x, float y): base(APIversion, handler){mX = x;mY = y;}
        public Vector2(int APIversion, EventHandler handler) : base(APIversion, handler) { }
        public Vector2(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
        public Vector2(int APIversion, EventHandler handler, Vector2 basis) : this(APIversion, handler, basis.X, basis.Y){ }

        [ElementPriority(1)]
        public float X
        {
            get { return mX; }
            set { if (mX != value) { mX = value; OnElementChanged(); } }
        }
        [ElementPriority(2)]
        public float Y
        {
            get { return mY; }
            set { if (mY != value) { mY = value; OnElementChanged(); } }
        }
        public virtual void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            mX = br.ReadSingle();
            mY = br.ReadSingle();
        }
        public virtual void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            bw.Write(mX);
            bw.Write(mY);
        }
        public override string ToString()
        {
            return String.Format("[{0,8:0.00000},{1,8:0.00000}]", X, Y);
        }

        public override List<string> ContentFields
        {
            get { return GetContentFields(requestedApiVersion, GetType()); }
        }

        public override int RecommendedApiVersion
        {
            get { return 1; }
        }

        public string Value { get { return ValueBuilder.Replace("\n", "; "); } }
    }
}