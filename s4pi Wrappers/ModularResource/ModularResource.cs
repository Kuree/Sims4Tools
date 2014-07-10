using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace ModularResource
{
    public class ModularResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        ushort unknown1;
        ushort unknown2;
        Int32IndexList tgiIndexes;
        TGIBlockList tgiBlocks;
        #endregion

        public ModularResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            long tgiPosn, tgiSize;
            BinaryReader r = new BinaryReader(s);

            unknown1 = r.ReadUInt16();
            tgiPosn = r.ReadUInt32() + s.Position;
            tgiSize = r.ReadUInt32();
            unknown2 = r.ReadUInt16();
            tgiIndexes = new Int32IndexList(OnResourceChanged, s, Int16.MaxValue, ReadInt16, WriteInt16);
            tgiBlocks = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            tgiIndexes.ParentTGIBlocks = tgiBlocks;
        }

        protected override Stream UnParse()
        {
            long pos;
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(unknown1);
            pos = ms.Position;
            w.Write((uint)0);//tgiOffset
            w.Write((uint)0);//tgiSize
            w.Write(unknown2);
            if (tgiBlocks == null) tgiBlocks = new TGIBlockList(OnResourceChanged);
            if (tgiIndexes == null) tgiIndexes = new Int32IndexList(OnResourceChanged, Int16.MaxValue, ReadInt16, WriteInt16, tgiBlocks);
            tgiIndexes.UnParse(ms);
            tgiBlocks.UnParse(ms, pos);

            tgiIndexes.ParentTGIBlocks = tgiBlocks;

            return ms;
        }
        private static int ReadInt16(Stream s) { return new BinaryReader(s).ReadInt16(); }
        private static void WriteInt16(Stream s, int count) { new BinaryWriter(s).Write((Int16)count); }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public ushort Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public ushort Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public Int32IndexList TGIIndexes { get { return tgiIndexes; } set { if (tgiIndexes != value) { tgiIndexes = new Int32IndexList(OnResourceChanged, value, Int16.MaxValue, ReadInt16, WriteInt16, tgiBlocks); OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public TGIBlockList TGIBlocks { get { return tgiBlocks; } set { if (tgiBlocks != value) { tgiBlocks = value == null ? null : new TGIBlockList(OnResourceChanged, value); tgiIndexes.ParentTGIBlocks = tgiBlocks; OnResourceChanged(this, EventArgs.Empty); } } }

        public String Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for ModularResource wrapper
    /// </summary>
    public class ModularResourceHandler : AResourceHandler
    {
        public ModularResourceHandler()
        {
            this.Add(typeof(ModularResource), new List<string>(new string[] { "0xCF9A4ACE", }));
        }
    }
}
