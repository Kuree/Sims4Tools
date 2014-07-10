using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    public class RailingCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        MaterialList materialList = null;
        uint railing4xModelVPXYIndex;
        uint railing1xModelVPXYIndex;
        uint postModelVPXYIndex;
        #endregion

        #region Constructors
        public RailingCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public RailingCatalogResource(int APIversion, Stream unused, RailingCatalogResource basis)
            : base(APIversion, basis.version, basis.common, basis.list)
        {
            this.materialList = (basis.version >= 0x00000003) ? new MaterialList(OnResourceChanged, basis.materialList) : null;
            this.common = new Common(requestedApiVersion, OnResourceChanged, basis.common);
            this.railing4xModelVPXYIndex = basis.railing4xModelVPXYIndex;
            this.railing1xModelVPXYIndex = basis.railing1xModelVPXYIndex;
            this.postModelVPXYIndex = basis.postModelVPXYIndex;
        }
        public RailingCatalogResource(int APIversion, uint version,
            Common common, uint railing4xModelIndex, uint railing1xModelIndex, uint postModelIndex,
            TGIBlockList ltgib)
            : this(APIversion, version,
            null,
            common, railing4xModelIndex, railing1xModelIndex, postModelIndex,
            ltgib)
        {
            if (checking) if (version >= 0x00000003)
                    throw new InvalidOperationException(String.Format("Constructor requires MaterialList for version {0}", version));
        }
        public RailingCatalogResource(int APIversion, uint version,
            MaterialList materialList,
            Common common, uint railing4xModelIndex, uint railing1xModelIndex, uint postModelIndex,
            TGIBlockList ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.materialList = materialList != null ? new MaterialList(OnResourceChanged, materialList) : null;
            this.common = new Common(requestedApiVersion, OnResourceChanged, common);
            this.railing4xModelVPXYIndex = railing4xModelIndex;
            this.railing1xModelVPXYIndex = railing1xModelIndex;
            this.postModelVPXYIndex = postModelIndex;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            base.Parse(s);
            this.materialList = (this.version >= 0x00000003) ? new MaterialList(OnResourceChanged, s) : null;
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.railing4xModelVPXYIndex = r.ReadUInt32();
            this.railing1xModelVPXYIndex = r.ReadUInt32();
            this.postModelVPXYIndex = r.ReadUInt32();

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(RailingCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (version >= 0x00000003)
            {
                if (materialList == null) materialList = new MaterialList(OnResourceChanged);
                materialList.UnParse(s);
            }
            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);

            w.Write(railing4xModelVPXYIndex);
            w.Write(railing1xModelVPXYIndex);
            w.Write(postModelVPXYIndex);

            base.UnParse(s);

            w.Flush();

            return s;
        }
        #endregion

        #region AApiVersionedFields
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                if (this.version < 0x00000003) res.Remove("Materials");
                return res;
            }
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        [ElementPriority(12)]
        public MaterialList Materials
        {
            get { if (version < 0x00000003) throw new InvalidOperationException(); return materialList; }
            set { if (version < 0x00000003) throw new InvalidOperationException(); if (materialList != value) { materialList = value == null ? null : new MaterialList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } }
        }
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21), TGIBlockListContentField("TGIBlocks")]
        public uint Railing4xModelVPXYIndex { get { return railing4xModelVPXYIndex; } set { if (railing4xModelVPXYIndex != value) { railing4xModelVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22), TGIBlockListContentField("TGIBlocks")]
        public uint Railing1xModelVPXYIndex { get { return railing1xModelVPXYIndex; } set { if (railing1xModelVPXYIndex != value) { railing1xModelVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23), TGIBlockListContentField("TGIBlocks")]
        public uint PostModelVPXYIndex { get { return postModelVPXYIndex; } set { if (postModelVPXYIndex != value) { postModelVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        //--insert TGIBlockList: no ElementPriority
        #endregion
    }
}
