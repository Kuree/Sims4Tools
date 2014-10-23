/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
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
