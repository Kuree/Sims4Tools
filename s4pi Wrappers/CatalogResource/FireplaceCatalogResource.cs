using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    public class FireplaceCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        byte fireplaceWidth;
        uint mantle;
        uint chimneyMantle;
        uint chimneyFullLevel;
        uint chimneyGroundLevel;
        uint chimneyBody;
        uint chimneyTop;
        uint chimneyCap;
        #endregion

        #region Constructors
        public FireplaceCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public FireplaceCatalogResource(int APIversion, Stream unused, FireplaceCatalogResource basis)
            : this(APIversion, basis.version, basis.common,
            basis.fireplaceWidth, basis.mantle, basis.chimneyMantle, basis.chimneyFullLevel, basis.chimneyGroundLevel, basis.chimneyBody, basis.chimneyTop, basis.chimneyCap,
            basis.list) { }
        public FireplaceCatalogResource(int APIversion, uint version, Common common,
            byte fireplaceType, uint mantle, uint chimneyMantle, uint chimneyFullLevel, uint chimneyGroundLevel, uint chimneyBody, uint chimneyTop, uint chimneyCap,
            TGIBlockList ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.common = new Common(requestedApiVersion, OnResourceChanged, common);
            this.fireplaceWidth = fireplaceType;
            this.mantle = mantle;
            this.chimneyMantle = chimneyMantle;
            this.chimneyFullLevel = chimneyFullLevel;
            this.chimneyGroundLevel = chimneyGroundLevel;
            this.chimneyBody = chimneyBody;
            this.chimneyTop = chimneyTop;
            this.chimneyCap = chimneyCap;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.fireplaceWidth = r.ReadByte();
            this.mantle = r.ReadUInt32();
            this.chimneyMantle = r.ReadUInt32();
            this.chimneyFullLevel = r.ReadUInt32();
            this.chimneyGroundLevel = r.ReadUInt32();
            this.chimneyBody = r.ReadUInt32();
            this.chimneyTop = r.ReadUInt32();
            this.chimneyCap = r.ReadUInt32();

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(FireplaceCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);

            w.Write(fireplaceWidth);
            w.Write(mantle);
            w.Write(chimneyMantle);
            w.Write(chimneyFullLevel);
            w.Write(chimneyGroundLevel);
            w.Write(chimneyBody);
            w.Write(chimneyTop);
            w.Write(chimneyCap);

            base.UnParse(s);

            w.Flush();

            return s;
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21)]
        public byte FireplaceWidth { get { return fireplaceWidth; } set { if (fireplaceWidth != value) { fireplaceWidth = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22), TGIBlockListContentField("TGIBlocks")]
        public uint MantleOBJDIndex { get { return mantle; } set { if (mantle != value) { mantle = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23), TGIBlockListContentField("TGIBlocks")]
        public uint ChimneyMantleOBJDIndex { get { return chimneyMantle; } set { if (chimneyMantle != value) { chimneyMantle = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(24), TGIBlockListContentField("TGIBlocks")]
        public uint ChimneyFullLevelOBJDIndex { get { return chimneyFullLevel; } set { if (chimneyFullLevel != value) { chimneyFullLevel = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(25), TGIBlockListContentField("TGIBlocks")]
        public uint ChimneyGroundLevelOBJDIndex { get { return chimneyGroundLevel; } set { if (chimneyGroundLevel != value) { chimneyGroundLevel = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(26), TGIBlockListContentField("TGIBlocks")]
        public uint ChimneyBodyOBJDIndex { get { return chimneyBody; } set { if (chimneyBody != value) { chimneyBody = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(27), TGIBlockListContentField("TGIBlocks")]
        public uint ChimneyTopOBJDIndex { get { return chimneyTop; } set { if (chimneyTop != value) { chimneyTop = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(28), TGIBlockListContentField("TGIBlocks")]
        public uint ChimneyCapOBJDIndex { get { return chimneyCap; } set { if (chimneyCap != value) { chimneyCap = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert TGIBlockList: no ElementPriority
        #endregion
    }
}
