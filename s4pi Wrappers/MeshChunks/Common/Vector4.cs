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
using s4pi.Interfaces;
using System.IO;

namespace meshExpImp.ModelBlocks
{
    public class Vector4 : Vector3
    {
        protected float mW;

        public Vector4(int APIversion, EventHandler handler) : base(APIversion, handler) { }
        public Vector4(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public Vector4(int APIversion, EventHandler handler, Vector4 basis) : this(APIversion, handler, basis.X, basis.Y, basis.Z, basis.W) { }
        public Vector4(int APIversion, EventHandler handler, float x, float y, float z, float w) : base(APIversion, handler, x, y, z) { mW = w; }

        [ElementPriority(4)]
        public float W
        {
            get { return mW; }
            set { if (mW != value) { mW = value; OnElementChanged(); } }
        }

        public override void Parse(Stream s)
        {
            base.Parse(s);
            var br = new BinaryReader(s);
            mW = br.ReadSingle();
        }
        public override void UnParse(Stream s)
        {
            base.UnParse(s);
            var bw = new BinaryWriter(s);
            bw.Write(mW);
        }
        public override string ToString()
        {
            return String.Format("[{0,8:0.00000},{1,8:0.00000},{2,8:0.00000},{3,8:0.00000}]", X, Y, Z, W);
        }

        // public override AHandlerElement Clone(EventHandler handler) { return new Vector4(0, handler, this); }

        public new string Value { get { return ValueBuilder.Replace("\n", "; "); } }
    }
}