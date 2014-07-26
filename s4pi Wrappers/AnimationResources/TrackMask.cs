using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.Animation
{
    public class TrackMask : ARCOLBlock
    {
        public const UInt32 kDefaultVersion = 0x00000200U;
        private TrackMaskList mTrackMasks;
        private byte[] mUnused;
        private UInt32 mVersion;
        // Part of Unused in version 0x201
        private TGIBlock mRigKey;
        private float mVertexAnimBlendWeight;
        private const string kRigKeyOrder = "ITG";

        public TrackMask(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s)
        {
        }

        public TrackMask(int APIversion, EventHandler handler, TrackMask basis)
            : this(APIversion, handler, basis.Version, basis.RigKey, basis.VertexAnimBlendWeight, basis.Unused, basis.TrackMasks)
        {
        }

        public TrackMask(int APIversion, EventHandler handler) : this(APIversion, handler, kDefaultVersion, null, null)
        {
        }

        public TrackMask(int APIversion, EventHandler handler, uint version, byte[] unused, TrackMaskList trackMasks)
            : base(APIversion, handler, null)
        {
            mVersion = version;
            mRigKey = new TGIBlock(APIversion, handler, kRigKeyOrder);
            mUnused = unused ?? (version < 0x201 ? new byte[48] : new byte[28]);
            mTrackMasks = trackMasks ?? new TrackMaskList(handler);
        }

        public TrackMask(int APIversion, EventHandler handler, uint version,
                         IResourceKey rigKey, float vertexAnimBlendWeight, byte[] unused, TrackMaskList trackMasks)
            : base(APIversion, handler, null)
        {
            mVersion = version;
            mRigKey = rigKey == null ? new TGIBlock(APIversion, handler, kRigKeyOrder)
                : new TGIBlock(APIversion, handler, kRigKeyOrder, rigKey);
            mVertexAnimBlendWeight = vertexAnimBlendWeight;
            mUnused = unused ?? (version < 0x201 ? new byte[48] : new byte[28]);
            mTrackMasks = trackMasks ?? new TrackMaskList(handler);
        }

        public override List<string> ContentFields
        {
            get
            {
                List<string> results = base.ContentFields;
                if (mVersion < 0x201)
                {
                    results.Remove("RigKey");
                    results.Remove("VertexAnimBlendWeight");
                }
                return results;
            }
        }

        [ElementPriority(1)]
        public uint Version
        {
            get { return mVersion; }
            set
            {
                if (mVersion != value)
                {
                    mVersion = value;
                    OnRCOLChanged(this, new EventArgs());
                }
            }
        }

        [ElementPriority(2)]
        public IResourceKey RigKey
        {
            get { return mRigKey; }
            set
            {
                if (!mRigKey.Equals(value))
                {
                    mRigKey = new TGIBlock(requestedApiVersion, handler, kRigKeyOrder, value);
                    OnRCOLChanged(this, new EventArgs());
                }
            }
        }

        [ElementPriority(3)]
        public float VertexAnimBlendWeight
        {
            get { return mVertexAnimBlendWeight; }
            set
            {
                if (mVertexAnimBlendWeight != value)
                {
                    mVertexAnimBlendWeight = value;
                    OnRCOLChanged(this, new EventArgs());
                }
            }
        }

        [ElementPriority(4)]
        public byte[] Unused
        {
            get { return mUnused; }
            set
            {
                if (mUnused != value)
                {
                    mUnused = value;
                    OnRCOLChanged(this, new EventArgs());
                }
            }
        }

        [ElementPriority(5)]
        public TrackMaskList TrackMasks
        {
            get { return mTrackMasks; }
            set
            {
                if (mTrackMasks != value)
                {
                    mTrackMasks = value;
                    OnRCOLChanged(this, new EventArgs());
                }
            }
        }

        public string Value
        {
            get { return ValueBuilder; }
        }

        public override uint ResourceType
        {
            get { return 0x033260E3; }
        }

        public override string Tag
        {
            get { return "TkMk"; }
        }

        protected override void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            if (FOURCC(br.ReadUInt32()) != Tag)
                throw new InvalidDataException("Invalid Tag, Expected " + Tag);
            mVersion = br.ReadUInt32();
            if (mVersion < 0x201)
            {
                mUnused = br.ReadBytes(48);
            }
            else
            {
                mRigKey = new TGIBlock(requestedApiVersion, handler, kRigKeyOrder, s);
                mVertexAnimBlendWeight = br.ReadSingle();
                mUnused = br.ReadBytes(28);
            }
            mTrackMasks = new TrackMaskList(handler, s);
            if (s.Position != s.Length)
                throw new InvalidDataException("Unexpected End of File");
        }

        public override Stream UnParse()
        {
            var s = new MemoryStream();
            var bw = new BinaryWriter(s);
            bw.Write((uint) FOURCC(Tag));
            bw.Write(mVersion);
            if (mVersion < 0x201)
            {
                if (mUnused == null) mUnused = new byte[48];
                if (mUnused.Length != 48)
                {
                    byte[] unused = mUnused;
                    mUnused = new byte[48];
                    Array.Copy(unused, 0, mUnused, 0, unused.Length < 48 ? unused.Length : 48);
                }
                bw.Write(mUnused);
            }
            else
            {
                if (mRigKey == null)
                    mRigKey = new TGIBlock(requestedApiVersion, handler, kRigKeyOrder);
                mRigKey.UnParse(s);
                bw.Write(mVertexAnimBlendWeight);
                if (mUnused == null) mUnused = new byte[28];
                if (mUnused.Length != 28)
                {
                    byte[] unused = mUnused;
                    mUnused = new byte[28];
                    Array.Copy(unused, 0, mUnused, 0, unused.Length < 28 ? unused.Length : 28);
                }
                bw.Write(mUnused);
            }
            if (mTrackMasks == null) mTrackMasks = new TrackMaskList(handler);
            mTrackMasks.UnParse(s);
            return s;
        }

        public override AHandlerElement Clone(EventHandler handler)
        {
            return new TrackMask(0, handler, this);
        }

        #region Nested type: TrackMaskList

        public class TrackMaskList : SimpleList<Single>
        {
            public TrackMaskList(EventHandler handler) : base(handler, ReadElement, WriteElement)
            {
            }

            public TrackMaskList(EventHandler handler, IEnumerable<float> ilt) : base(handler, ilt, ReadElement, WriteElement)
            {
            }

            public TrackMaskList(EventHandler handler, Stream s) : base(handler, s, ReadElement, WriteElement)
            {
            }

            private static Single ReadElement(Stream s)
            {
                return new BinaryReader(s).ReadSingle();
            }

            private static void WriteElement(Stream s, Single element)
            {
                new BinaryWriter(s).Write(element);
            }
        }

        #endregion
    }
}
