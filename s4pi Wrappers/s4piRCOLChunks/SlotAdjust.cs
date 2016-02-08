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

namespace s4pi.GenericRCOLResource
{
    public class SlotAdjust : ARCOLBlock
    {
        static bool checking = s4pi.Settings.Settings.Checking;
        const string TAG = "BOND";// For RCOLDealer, to match the Tag for the main resource; doesn't appear in RCOL chunk

        #region Attributes
        //uint tag = (uint)FOURCC(TAG);
        uint version = 4;
        AdjustmentList adjustments;
        #endregion

        #region Constructors
        public SlotAdjust(int apiVersion, EventHandler handler) : base(apiVersion, handler, null) { }
        public SlotAdjust(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler, s) { }
        public SlotAdjust(int apiVersion, EventHandler handler, SlotAdjust basis)
            : this(apiVersion, handler, basis.version, basis.adjustments) { }
        public SlotAdjust(int apiVersion, EventHandler handler, uint version, IEnumerable<Adjustment> adjustments)
            : base(apiVersion, handler, null)
        {
            this.version = version;
            this.adjustments = new AdjustmentList(handler, adjustments);
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x0355E0A6; } }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            /*
             * This RCOL chunk has no Tag
            tag = r.ReadUInt32();
            if (checking) if (tag != (uint)FOURCC(TAG))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: '{1}'; at 0x{2:X8}", FOURCC(tag), TAG, s.Position));/****/

            version = r.ReadUInt32();
            adjustments = new AdjustmentList(handler, s);
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            // w.Write(tag);// No Tag

            w.Write(version);
            if (adjustments == null) adjustments = new AdjustmentList(handler);
            adjustments.UnParse(ms);

            return ms;
        }

        //public override AHandlerElement Clone(EventHandler handler) { return new SlotAdjust(requestedApiVersion, handler, this); }
        #endregion

        #region Sub-types
        public class Adjustment : AHandlerElement, IEquatable<Adjustment>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            uint slotName;
            float offsetX;
            float offsetY;
            float offsetZ;
            float scaleX;
            float scaleY;
            float scaleZ;
            float quatX;
            float quatY;
            float quatZ;
            float quatW;
            #endregion

            #region Constructors
            public Adjustment(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public Adjustment(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }
            public Adjustment(int apiVersion, EventHandler handler, Adjustment basis)
                : this(apiVersion, handler,
                basis.slotName, basis.offsetX, basis.offsetY, basis.offsetZ, basis.scaleX, basis.scaleY, basis.scaleZ, basis.quatX, basis.quatY, basis.quatZ, basis.quatW) { }
            public Adjustment(int apiVersion, EventHandler handler,
                uint slotName, float offsetX, float offsetY, float offsetZ, float scaleX, float scaleY, float scaleZ, float quatX, float quatY, float quatZ, float quatW)
                : base(apiVersion, handler)
            {
                this.slotName = slotName;
                this.offsetX = offsetX;
                this.offsetY = offsetY;
                this.offsetZ = offsetZ;
                this.scaleX = scaleX;
                this.scaleY = scaleY;
                this.scaleZ = scaleZ;
                this.quatX = quatX;
                this.quatY = quatY;
                this.quatZ = quatZ;
                this.quatW = quatW;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                slotName = r.ReadUInt32();
                offsetX = r.ReadSingle();
                offsetY = r.ReadSingle();
                offsetZ = r.ReadSingle();
                scaleX = r.ReadSingle();
                scaleY = r.ReadSingle();
                scaleZ = r.ReadSingle();
                quatX = r.ReadSingle();
                quatY = r.ReadSingle();
                quatZ = r.ReadSingle();
                quatW = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(slotName);
                w.Write(offsetX);
                w.Write(offsetY);
                w.Write(offsetZ);
                w.Write(scaleX);
                w.Write(scaleY);
                w.Write(scaleZ);
                w.Write(quatX);
                w.Write(quatY);
                w.Write(quatZ);
                w.Write(quatW);
            }
            #endregion

            #region AHandlerElement
            //public override AHandlerElement Clone(EventHandler handler) { return new Adjustment(requestedApiVersion, handler, this); }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Adjustment>
            public bool Equals(Adjustment other)
            {
                return this.slotName == other.slotName &&
                    this.offsetX == other.offsetX &&
                    this.offsetY == other.offsetY &&
                    this.offsetZ == other.offsetZ &&
                    this.scaleX == other.scaleX &&
                    this.scaleY == other.scaleY &&
                    this.scaleZ == other.scaleZ &&
                    this.quatX == other.quatX &&
                    this.quatY == other.quatY &&
                    this.quatZ == other.quatZ &&
                    this.quatW == other.quatW;
            }

            public override bool Equals(object obj)
            {
                return obj as Adjustment != null ? this.Equals(obj as Adjustment) : false;
            }
            public override int GetHashCode()
            {
                return this.slotName.GetHashCode() ^
                    this.offsetX.GetHashCode() ^
                    this.offsetY.GetHashCode() ^
                    this.offsetZ.GetHashCode() ^
                    this.scaleX.GetHashCode() ^
                    this.scaleY.GetHashCode() ^
                    this.scaleZ.GetHashCode() ^
                    this.quatX.GetHashCode() ^
                    this.quatY.GetHashCode() ^
                    this.quatZ.GetHashCode() ^
                    this.quatW.GetHashCode();
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint SlotName { get { return slotName; } set { if (slotName != value) { slotName = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float OffsetX { get { return offsetX; } set { if (offsetX != value) { offsetX = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public float OffsetY { get { return offsetY; } set { if (offsetY != value) { offsetY = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public float OffsetZ { get { return offsetZ; } set { if (offsetZ != value) { offsetZ = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public float ScaleX { get { return scaleX; } set { if (scaleX != value) { scaleX = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public float ScaleY { get { return scaleY; } set { if (scaleY != value) { scaleY = value; OnElementChanged(); } } }
            [ElementPriority(7)]
            public float ScaleZ { get { return scaleZ; } set { if (scaleZ != value) { scaleZ = value; OnElementChanged(); } } }
            [ElementPriority(8)]
            public float QuatX { get { return quatX; } set { if (quatX != value) { quatX = value; OnElementChanged(); } } }
            [ElementPriority(9)]
            public float QuatY { get { return quatY; } set { if (quatY != value) { quatY = value; OnElementChanged(); } } }
            [ElementPriority(10)]
            public float QuatZ { get { return quatZ; } set { if (quatZ != value) { quatZ = value; OnElementChanged(); } } }
            [ElementPriority(11)]
            public float QuatW { get { return quatW; } set { if (quatW != value) { quatW = value; OnElementChanged(); } } }

            public string Value
            {
                get
                {
                    return ValueBuilder;
                    /*
                    string s = "";
                    foreach (string var in ContentFields) if (var != "Value") s += "\n" + var + ": " + this[var];
                    return s.TrimStart('\n');
                    /**/
                }
            }
            #endregion

        }
        public class AdjustmentList : DependentList<Adjustment>
        {
            public AdjustmentList(EventHandler handler) : base(handler) { }
            public AdjustmentList(EventHandler handler, Stream s) : base(handler, s) { }
            public AdjustmentList(EventHandler handler, IEnumerable<Adjustment> lsbp) : base(handler, lsbp) { }

            protected override Adjustment CreateElement(Stream s) { return new Adjustment(0, elementHandler, s); }
            protected override void WriteElement(Stream s, Adjustment element) { element.UnParse(s); }

            //public override void Add() { this.Add(new Adjustment(0, null)); }
        }
        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public AdjustmentList Adjustments { get { return adjustments; } set { if (adjustments != value) { adjustments = value == null ? null : new AdjustmentList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        
        public string Value { get { return ValueBuilder; } }
        #endregion
    }
}
