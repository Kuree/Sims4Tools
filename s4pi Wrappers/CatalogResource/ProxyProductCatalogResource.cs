using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    public class ProxyProductCatalogResource : CatalogResource
    {
        #region Attributes
        Tool toolType;
        #endregion

        #region Constructors
        public ProxyProductCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public ProxyProductCatalogResource(int APIversion, Stream unused, ProxyProductCatalogResource basis)
            : this(APIversion, basis.version, basis.common, basis.toolType) { }
        public ProxyProductCatalogResource(int APIversion, uint version, Common common, Tool toolType)
            : base(APIversion, version, common)
        {
            this.toolType = toolType;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);

            base.Parse(s);
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.toolType = (Tool)r.ReadUInt32();

            if (checking) if (this.GetType().Equals(typeof(ProxyProductCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);
            w.Write((uint)toolType);

            w.Flush();

            return s;
        }
        #endregion

        #region Sub-Types
        public enum Tool : uint
        {
            LevelFloorRectangle = 0x00000001,
            FlattenLot = 0x00000002,
            StairsRailing = 0x00000003,
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21)]
        public Tool ToolType { get { return toolType; } set { if (toolType != value) { toolType = value; OnResourceChanged(this, EventArgs.Empty); } } }
        #endregion
    }
}
