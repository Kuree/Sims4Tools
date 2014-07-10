using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    public class RoofPatternCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        uint topMaterialVPXYIndex;//topMaterialVPXYIndex
        uint undersideMaterialVPXYIndex;//undersideMaterialVPXYIndex
        uint sideStripsMaterialVPXYIndex;//sideStripsMaterialVPXYIndex
        #endregion

        #region Constructors
        public RoofPatternCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public RoofPatternCatalogResource(int APIversion, Stream unused, RoofPatternCatalogResource basis)
            : base(APIversion, basis.version, basis.common, basis.list)
        {
            this.common = new Common(requestedApiVersion, OnResourceChanged, basis.common);
            this.topMaterialVPXYIndex = basis.topMaterialVPXYIndex;
            this.undersideMaterialVPXYIndex = basis.undersideMaterialVPXYIndex;
            this.sideStripsMaterialVPXYIndex = basis.sideStripsMaterialVPXYIndex;
        }
        public RoofPatternCatalogResource(int APIversion, uint version, Common common,
            uint topMaterialVPXYIndex, uint undersideMaterialVPXYIndex, uint sideStripsMaterialVPXYIndex,
            TGIBlockList ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.common = new Common(requestedApiVersion, OnResourceChanged, common);
            this.topMaterialVPXYIndex = topMaterialVPXYIndex;
            this.undersideMaterialVPXYIndex = undersideMaterialVPXYIndex;
            this.sideStripsMaterialVPXYIndex = sideStripsMaterialVPXYIndex;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.topMaterialVPXYIndex = r.ReadUInt32();
            this.undersideMaterialVPXYIndex = r.ReadUInt32();
            this.sideStripsMaterialVPXYIndex = r.ReadUInt32();

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(RoofPatternCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);

            w.Write(topMaterialVPXYIndex);
            w.Write(undersideMaterialVPXYIndex);
            w.Write(sideStripsMaterialVPXYIndex);

            base.UnParse(s);

            w.Flush();

            return s;
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21), TGIBlockListContentField("TGIBlocks")]
        public uint TopMaterialVPXYIndex { get { return topMaterialVPXYIndex; } set { if (topMaterialVPXYIndex != value) { topMaterialVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22), TGIBlockListContentField("TGIBlocks")]
        public uint UndersideMaterialVPXYIndex { get { return undersideMaterialVPXYIndex; } set { if (undersideMaterialVPXYIndex != value) { undersideMaterialVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23), TGIBlockListContentField("TGIBlocks")]
        public uint SideStripsMaterialVPXYIndex { get { return sideStripsMaterialVPXYIndex; } set { if (sideStripsMaterialVPXYIndex != value) { sideStripsMaterialVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert TGIBlockList: no ElementPriority
        #endregion
    }
}
