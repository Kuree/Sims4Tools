using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace CASPartResource
{
    public class BlendUnitResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        private uint version;
        private ulong nameHash;
        private Int32IndexList tgiIndexes;
        private byte bidirectional;
        private CASPanelGroupType casPanelGroup;
        private CASPanelSortType sort;
        private uint unknown1;
        private TGIBlockList tgiBlocks;
        #endregion

        public BlendUnitResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        private void Parse(Stream s)
        {
            long tgiPosn, tgiSize;
            BinaryReader r = new BinaryReader(s);

            version = r.ReadUInt32();

            tgiPosn = r.ReadUInt32() + s.Position;
            tgiSize = r.ReadUInt32();

            nameHash = r.ReadUInt64();
            tgiIndexes = new Int32IndexList(OnResourceChanged, s);
            bidirectional = r.ReadByte();
            casPanelGroup = (CASPanelGroupType)r.ReadUInt32();
            sort = (CASPanelSortType)r.ReadUInt32();
            unknown1 = r.ReadUInt32();
            tgiBlocks = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            tgiIndexes.ParentTGIBlocks = tgiBlocks;
        }

        protected override Stream UnParse()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(version);

            long pos = s.Position;
            w.Write((uint)0); // tgiOffset
            w.Write((uint)0); // tgiSize

            w.Write(nameHash);
            if (tgiBlocks == null) tgiBlocks = new TGIBlockList(OnResourceChanged);
            if (tgiIndexes == null) tgiIndexes = new Int32IndexList(OnResourceChanged, tgiBlocks);
            tgiIndexes.UnParse(s);
            w.Write(bidirectional);
            w.Write((uint)casPanelGroup);
            w.Write((uint)sort);
            w.Write(unknown1);

            tgiBlocks.UnParse(s, pos);

            return s;
        }
        #endregion

        #region Sub-Types
        [Flags]
        public enum CASPanelGroupType : uint
        {
            Unknown0 = 0,
            Unknown1 = 1,
            HeadAndEars = 2,
            Unknown3 = 4,

            Mouth = 8,
            Nose = 16,
            Unknown6 = 32,
            Eyelash = 64,

            Eyes = 128,
            Unknown9 = 256,
            UnknownA = 512,
            UnknownB = 1024,

            UnknownC = 2048,
            UnknownD = 4096,
            UnknownE = 8192,
            UnknownF = 16384,
        }

        [Flags]
        public enum CASPanelSortType : uint
        {
            Unknown0 = 0,
            Unknown1 = 1,
            Unknown2 = 2,
            Unknown3 = 4,

            Unknown4 = 8,
            Unknown5 = 16,
            Unknown6 = 32,
            Unknown7 = 64,

            Unknown8 = 128,
            Unknown9 = 256,
            UnknownA = 512,
            UnknownB = 1024,

            UnknownC = 2048,
            UnknownD = 4096,
            UnknownE = 8192,
            UnknownF = 16384,
        }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public ulong NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public Int32IndexList TGIIndexes { get { return tgiIndexes; } set { if (!tgiIndexes.Equals(value)) { tgiIndexes = new Int32IndexList(OnResourceChanged, value, tgiBlocks); OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public byte Bidirectional { get { return bidirectional; } set { if (bidirectional != value) { bidirectional = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(5)]
        public CASPanelGroupType CASPanelGroup { get { return casPanelGroup; } set { if (casPanelGroup != value) { casPanelGroup = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(6)]
        public CASPanelSortType Sort { get { return sort; } set { if (sort != value) { sort = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(7)]
        public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(8)]
        public TGIBlockList TGIBlocks { get { return tgiBlocks; } set { if (!tgiBlocks.Equals(value)) { tgiBlocks = value == null ? null : new TGIBlockList(OnResourceChanged, value); tgiIndexes.ParentTGIBlocks = tgiBlocks; OnResourceChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for BlendUnitResource wrapper
    /// </summary>
    public class BlendUnitResourceHandler : AResourceHandler
    {
        public BlendUnitResourceHandler()
        {
            this.Add(typeof(BlendUnitResource), new List<string>(new string[] { "0xB52F5055" }));
        }
    }
}
