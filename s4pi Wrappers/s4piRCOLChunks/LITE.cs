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
    public class LITE : ARCOLBlock
    {
        static bool checking = s4pi.Settings.Settings.Checking;
        const string TAG = "LITE";

        #region Attributes
        uint tag = (uint)FOURCC(TAG);
        uint version = 4;
        uint unknown1 = 0x84;
        ushort unknown2 = 0;
        LightSourceList lightSources = null;
        OccluderList occluders = null;
        #endregion

        #region Constructors
        public LITE(int APIversion, EventHandler handler) : base(APIversion, handler, null) { }
        public LITE(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        public LITE(int APIversion, EventHandler handler, LITE basis)
            : base(APIversion, handler, null)
        {
            this.version = basis.version;
            this.unknown1 = basis.unknown1;
            this.lightSources = basis.lightSources == null ? null : new LightSourceList(handler, basis.lightSources);
            this.occluders = basis.occluders == null ? null : new OccluderList(handler, basis.occluders);
            this.unknown2 = basis.unknown2;
        }
        #endregion

        #region ARCOLBlock
        [ElementPriority(2)]
        public override string Tag { get { return TAG; } }

        [ElementPriority(3)]
        public override uint ResourceType { get { return 0x03B4C61D; } }

        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            tag = r.ReadUInt32();
            if (checking) if (tag != (uint)FOURCC(TAG))
                    throw new InvalidDataException(String.Format("Invalid Tag read: '{0}'; expected: '{1}'; at 0x{2:X8}", FOURCC(tag), TAG, s.Position));
            version = r.ReadUInt32();
            unknown1 = r.ReadUInt32();
            byte lsCount = r.ReadByte();
            byte ssCount = r.ReadByte();
            unknown2 = r.ReadUInt16();
            lightSources = new LightSourceList(handler, lsCount, s);
            occluders = new OccluderList(handler, ssCount, s);
        }

        public override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(tag);
            w.Write(version);
            w.Write(unknown1);
            if (lightSources == null) lightSources = new LightSourceList(handler);
            w.Write((byte)lightSources.Count);
            if (occluders == null) occluders = new OccluderList(handler);
            w.Write((byte)occluders.Count);
            w.Write(unknown2);
            lightSources.UnParse(ms);
            occluders.UnParse(ms);

            return ms;
        }
        #endregion

        #region Sub-types
        public class LightSource : AHandlerElement, IEquatable<LightSource>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            LightSourceType lightSource = LightSourceType.Unknown;
            Vertex transform;
            RGB color;
            float intensity;
            protected float[] lightSourceDataArray = new float[24];

            GeneralLightSourceType lightSourceData;
            SpotLightSourceType spotLightSourceData;
            LampShadeLightSourceType lampShadeLightSourceData;
            TubeLightSourceType tubeShadeLightSourceData;
            SquareWindowLightSourceType squareWindowLightSourceData;
            CircularWindowLightSourceType circularWindowLightSourceData;
            SquareWindowLightSourceType squareAreaLightSourceData;
            CircularWindowLightSourceType discAreaLightSourceData;
            #endregion

            #region Constructors
            public LightSource(int APIversion, EventHandler handler)
                : this(APIversion, handler, LightSourceType.Unknown,
                0f, 0f, 0f,
                0f, 0f, 0f, 0f,
                new float[] { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, }) { }
            public LightSource(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); PointToLightSourceData(); }
            public LightSource(int APIversion, EventHandler handler, LightSource basis)
                : this(APIversion, handler, basis.lightSource
                , basis.transform.X, basis.transform.Y, basis.transform.Z
                , basis.color.X, basis.color.Y, basis.color.Z, basis.intensity
                , basis.lightSourceDataArray
                ) { }

            public LightSource(int APIversion, EventHandler handler, LightSourceType sectionType
                , float X, float Y, float Z
                , float R, float G, float B, float intensity
                , float[] lightSourceData
                )
                : base(APIversion, handler)
            {
                this.lightSource = sectionType;
                this.transform = new Vertex(requestedApiVersion, handler, X, Y, Z);
                this.color = new RGB(requestedApiVersion, handler, R, G, B);
                this.intensity = intensity;
                if (checking) if (lightSourceData.Length != 24)
                        throw new ArgumentException("Array length must be 24");
                this.lightSourceDataArray = (float[])lightSourceData.Clone();
                PointToLightSourceData();
            }
            #endregion

            void PointToLightSourceData()
            {
                lightSourceData = new GeneralLightSourceType(requestedApiVersion, handler, lightSourceDataArray);
                spotLightSourceData = new SpotLightSourceType(requestedApiVersion, handler, lightSourceDataArray);
                lampShadeLightSourceData = new LampShadeLightSourceType(requestedApiVersion, handler, lightSourceDataArray);
                tubeShadeLightSourceData = new TubeLightSourceType(requestedApiVersion, handler, lightSourceDataArray);
                squareWindowLightSourceData = new SquareWindowLightSourceType(requestedApiVersion, handler, lightSourceDataArray);
                circularWindowLightSourceData = new CircularWindowLightSourceType(requestedApiVersion, handler, lightSourceDataArray);
                squareAreaLightSourceData = new SquareWindowLightSourceType(requestedApiVersion, handler, lightSourceDataArray);
                discAreaLightSourceData = new CircularWindowLightSourceType(requestedApiVersion, handler, lightSourceDataArray);
            }

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                lightSource = (LightSourceType)r.ReadUInt32();
                transform = new Vertex(requestedApiVersion, handler, s);
                color = new RGB(requestedApiVersion, handler, s);
                intensity = r.ReadSingle();
                for (int i = 0; i < lightSourceDataArray.Length; i++)
                    lightSourceDataArray[i] = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)lightSource);
                transform.UnParse(s);
                color.UnParse(s);
                w.Write(intensity);
                for (int i = 0; i < lightSourceDataArray.Length; i++)
                    w.Write(lightSourceDataArray[i]);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            /// <summary>
            /// The list of available field names on this API object
            /// </summary>
            public override List<string> ContentFields
            {
                get
                {
                    List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                    List<string> removals = new List<string>(new string[]{
                        "LightSourceData",
                        "SpotLightLightSourceData",
                        "LampShadeLightSourceData",
                        "TubeLightLightSourceData",
                        "SquareWindowLightSourceData",
                        "CircularWindowLightSourceData",
                        "SquareAreaLightSourceData",
                        "DiscAreaLightSourceData",
                    });
                    switch (lightSource)
                    {
                        case LightSourceType.Spot: removals.Remove("SpotLightLightSourceData"); break;
                        case LightSourceType.LampShade: removals.Remove("LampShadeLightSourceData"); break;
                        case LightSourceType.TubeLight: removals.Remove("TubeLightLightSourceData"); break;
                        case LightSourceType.SquareWindow: removals.Remove("SquareWindowLightSourceData"); break;
                        case LightSourceType.CircularWindow: removals.Remove("CircularWindowLightSourceData"); break;
                        case LightSourceType.SquareAreaLight: removals.Remove("SquareAreaLightSourceData"); break;
                        case LightSourceType.DiscAreaLight: removals.Remove("DiscAreaLightSourceData"); break;
                        default: removals.Remove("LightSourceData"); break;
                    }
                    foreach (var rem in removals) res.Remove(rem);
                    return res;
                }
            }
            #endregion

            #region IEquatable<LightSource> Members

            public bool Equals(LightSource other)
            {
                return lightSource.Equals(other.lightSource)
                    && transform.Equals(other.transform)
                    && color.Equals(other.color)
                    && intensity.Equals(other.intensity)
                    && lightSourceDataArray.Equals<float>(other.lightSourceDataArray)
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as LightSource != null ? this.Equals(obj as LightSource) : false;
            }

            public override int GetHashCode()
            {
                return lightSource.GetHashCode()
                    ^ transform.GetHashCode()
                    ^ color.GetHashCode()
                    ^ intensity.GetHashCode()
                    ^ lightSourceDataArray.GetHashCode()
                    ;
            }

            #endregion
            
            #region Sub-types
            public enum LightSourceType : uint
            {
                Unknown = 0x00,//unused
                Ambient = 0x01,//unused
                Directional = 0x02,//unused
                Point = 0x03,
                Spot = 0x04,
                LampShade = 0x05,
                TubeLight = 0x06,
                SquareWindow = 0x07,
                CircularWindow = 0x08,
                SquareAreaLight = 0x09,
                DiscAreaLight = 0x0A,
                WorldLight = 0x0B,
            }

            public class RGB : Vertex
            {
                #region Constructors
                public RGB(int APIversion, EventHandler handler) : base(APIversion, handler) { }
                public RGB(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
                public RGB(int APIversion, EventHandler handler, RGB basis) : base(APIversion, handler, basis.X, basis.Y, basis.Z) { }
                public RGB(int APIversion, EventHandler handler, float x, float y, float z) : base(APIversion, handler, x, y, z) { }
                #endregion

                public override List<string> ContentFields
                {
                    get
                    {
                        List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                        res.Remove("X");
                        res.Remove("Y");
                        res.Remove("Z");
                        return res;
                    }
                }

                [ElementPriority(1)]
                public float R { get { return X; } set { X = value; } }
                [ElementPriority(2)]
                public float G { get { return Y; } set { Y = value; } }
                [ElementPriority(3)]
                public float B { get { return Z; } set { Z = value; } }
            }

            public class GeneralLightSourceType : AHandlerElement
            {
                protected float[] lightSourceData;

                public GeneralLightSourceType(int APIversion, EventHandler handler, float[] lightSourceData)
                    : base(APIversion, handler)
                { this.lightSourceData = lightSourceData; }

                #region AHandlerElement
                public override int RecommendedApiVersion { get { return 0; } }
                public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
                #endregion

                protected void setFloatN(int n, float value) { if (lightSourceData[n] != value) { lightSourceData[n] = value; OnElementChanged(); } }
                protected void setFloatN(int n, Vertex value)
                {
                    if (lightSourceData[n] != value.X || lightSourceData[n + 1] != value.Y || lightSourceData[n + 2] != value.Z)
                    {
                        lightSourceData[n] = value.X;
                        lightSourceData[n + 1] = value.Y;
                        lightSourceData[n + 2] = value.Z;
                        OnElementChanged();
                    }
                }

                public string Value { get { return ValueBuilder; } }

                public float Unknown_F01 { get { return lightSourceData[0]; } set { setFloatN(0, value); } }
                public float Unknown_F02 { get { return lightSourceData[1]; } set { setFloatN(1, value); } }
                public float Unknown_F03 { get { return lightSourceData[2]; } set { setFloatN(2, value); } }
                public float Unknown_F04 { get { return lightSourceData[3]; } set { setFloatN(3, value); } }
                public float Unknown_F05 { get { return lightSourceData[4]; } set { setFloatN(4, value); } }
                public float Unknown_F06 { get { return lightSourceData[5]; } set { setFloatN(5, value); } }
                public float Unknown_F07 { get { return lightSourceData[6]; } set { setFloatN(6, value); } }
                public float Unknown_F08 { get { return lightSourceData[7]; } set { setFloatN(7, value); } }
                public float Unknown_F09 { get { return lightSourceData[8]; } set { setFloatN(8, value); } }
                public float Unknown_F10 { get { return lightSourceData[9]; } set { setFloatN(9, value); } }
                public float Unknown_F11 { get { return lightSourceData[10]; } set { setFloatN(10, value); } }
                public float Unknown_F12 { get { return lightSourceData[11]; } set { setFloatN(11, value); } }
                public float Unknown_F13 { get { return lightSourceData[12]; } set { setFloatN(12, value); } }
                public float Unknown_F14 { get { return lightSourceData[13]; } set { setFloatN(13, value); } }
                public float Unknown_F15 { get { return lightSourceData[14]; } set { setFloatN(14, value); } }
                public float Unknown_F16 { get { return lightSourceData[15]; } set { setFloatN(15, value); } }
                public float Unknown_F17 { get { return lightSourceData[16]; } set { setFloatN(16, value); } }
                public float Unknown_F18 { get { return lightSourceData[17]; } set { setFloatN(17, value); } }
                public float Unknown_F19 { get { return lightSourceData[18]; } set { setFloatN(18, value); } }
                public float Unknown_F20 { get { return lightSourceData[19]; } set { setFloatN(19, value); } }
                public float Unknown_F21 { get { return lightSourceData[20]; } set { setFloatN(20, value); } }
                public float Unknown_F22 { get { return lightSourceData[21]; } set { setFloatN(21, value); } }
                public float Unknown_F23 { get { return lightSourceData[22]; } set { setFloatN(22, value); } }
                public float Unknown_F24 { get { return lightSourceData[23]; } set { setFloatN(23, value); } }
            }

            public class SpotLightSourceType : GeneralLightSourceType
            {
                public SpotLightSourceType(int APIversion, EventHandler handler, float[] lightSourceData) : base(APIversion, handler, lightSourceData) { }

                #region AHandlerElement
                public override int RecommendedApiVersion { get { return 0; } }
                public override List<string> ContentFields
                {
                    get
                    {
                        List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                        res.Remove("Unknown_F01");
                        res.Remove("Unknown_F02");
                        res.Remove("Unknown_F03");
                        res.Remove("Unknown_F04");
                        res.Remove("Unknown_F05");
                        return res;
                    }
                }
                #endregion

                [ElementPriority(1)]
                public Vertex At
                {
                    get { return new Vertex(0, null, lightSourceData[0], lightSourceData[1], lightSourceData[2]); }
                    set { setFloatN(0, value); }
                }
                [ElementPriority(2)]
                public float FalloffAngle { get { return lightSourceData[3]; } set { setFloatN(3, value); } }
                [ElementPriority(3)]
                public float BlurScale { get { return lightSourceData[4]; } set { setFloatN(4, value); } }
            }

            public class LampShadeLightSourceType : GeneralLightSourceType
            {
                public LampShadeLightSourceType(int APIversion, EventHandler handler, float[] lightSourceData) : base(APIversion, handler, lightSourceData) { }

                #region AHandlerElement
                public override int RecommendedApiVersion { get { return 0; } }
                public override List<string> ContentFields
                {
                    get
                    {
                        List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                        res.Remove("Unknown_F01");
                        res.Remove("Unknown_F02");
                        res.Remove("Unknown_F03");
                        res.Remove("Unknown_F04");
                        res.Remove("Unknown_F05");
                        res.Remove("Unknown_F06");
                        res.Remove("Unknown_F07");
                        res.Remove("Unknown_F08");
                        res.Remove("Unknown_F09");
                        return res;
                    }
                }
                #endregion

                [ElementPriority(1)]
                public Vertex At
                {
                    get { return new Vertex(0, null, lightSourceData[0], lightSourceData[1], lightSourceData[2]); }
                    set { setFloatN(0, value); }
                }
                [ElementPriority(2)]
                public float FalloffAngle { get { return lightSourceData[3]; } set { setFloatN(3, value); } }
                [ElementPriority(3)]
                public float ShadeLightRigMultiplier { get { return lightSourceData[4]; } set { setFloatN(4, value); } }
                [ElementPriority(4)]
                public float BottomAngle { get { return lightSourceData[5]; } set { setFloatN(5, value); } }
                [ElementPriority(5)]
                public RGB ShadeColor
                {
                    get { return new RGB(0, null, lightSourceData[6], lightSourceData[7], lightSourceData[8]); }
                    set { setFloatN(6, value); }
                }
            }

            public class TubeLightSourceType : GeneralLightSourceType
            {
                public TubeLightSourceType(int APIversion, EventHandler handler, float[] lightSourceData) : base(APIversion, handler, lightSourceData) { }

                #region AHandlerElement
                public override int RecommendedApiVersion { get { return 0; } }
                public override List<string> ContentFields
                {
                    get
                    {
                        List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                        res.Remove("Unknown_F01");
                        res.Remove("Unknown_F02");
                        res.Remove("Unknown_F03");
                        res.Remove("Unknown_F04");
                        res.Remove("Unknown_F05");
                        return res;
                    }
                }
                #endregion

                [ElementPriority(1)]
                public Vertex At
                {
                    get { return new Vertex(0, null, lightSourceData[0], lightSourceData[1], lightSourceData[2]); }
                    set { setFloatN(0, value); }
                }
                [ElementPriority(2)]
                public float TubeLength { get { return lightSourceData[3]; } set { setFloatN(3, value); } }
                [ElementPriority(3)]
                public float BlurScale { get { return lightSourceData[4]; } set { setFloatN(4, value); } }
            }

            public class SquareWindowLightSourceType : GeneralLightSourceType
            {
                public SquareWindowLightSourceType(int APIversion, EventHandler handler, float[] lightSourceData) : base(APIversion, handler, lightSourceData) { }

                #region AHandlerElement
                public override int RecommendedApiVersion { get { return 0; } }
                public override List<string> ContentFields
                {
                    get
                    {
                        List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                        res.Remove("Unknown_F01");
                        res.Remove("Unknown_F02");
                        res.Remove("Unknown_F03");

                        res.Remove("Unknown_F04");
                        res.Remove("Unknown_F05");
                        res.Remove("Unknown_F06");

                        res.Remove("Unknown_F07");
                        res.Remove("Unknown_F08");
                        res.Remove("Unknown_F09");
                        res.Remove("Unknown_F10");
                        return res;
                    }
                }
                #endregion

                [ElementPriority(1)]
                public Vertex At
                {
                    get { return new Vertex(0, null, lightSourceData[0], lightSourceData[1], lightSourceData[2]); }
                    set { setFloatN(0, value); }
                }
                [ElementPriority(2)]
                public Vertex Right
                {
                    get { return new Vertex(0, null, lightSourceData[3], lightSourceData[4], lightSourceData[5]); }
                    set { setFloatN(3, value); }
                }
                [ElementPriority(3)]
                public float Width { get { return lightSourceData[6]; } set { setFloatN(6, value); } }
                [ElementPriority(4)]
                public float Height { get { return lightSourceData[7]; } set { setFloatN(7, value); } }
                [ElementPriority(5)]
                public float FalloffAngle { get { return lightSourceData[8]; } set { setFloatN(8, value); } }
                [ElementPriority(6)]
                public float WindowTopBottomAngle { get { return lightSourceData[9]; } set { setFloatN(9, value); } }
            }

            public class CircularWindowLightSourceType : GeneralLightSourceType
            {
                public CircularWindowLightSourceType(int APIversion, EventHandler handler, float[] lightSourceData) : base(APIversion, handler, lightSourceData) { }

                #region AHandlerElement
                public override int RecommendedApiVersion { get { return 0; } }
                public override List<string> ContentFields
                {
                    get
                    {
                        List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                        res.Remove("Unknown_F01");
                        res.Remove("Unknown_F02");
                        res.Remove("Unknown_F03");

                        res.Remove("Unknown_F04");
                        res.Remove("Unknown_F05");
                        res.Remove("Unknown_F06");

                        res.Remove("Unknown_F07");
                        return res;
                    }
                }
                #endregion

                [ElementPriority(1)]
                public Vertex At
                {
                    get { return new Vertex(0, null, lightSourceData[0], lightSourceData[1], lightSourceData[2]); }
                    set { setFloatN(0, value); }
                }
                [ElementPriority(2)]
                public Vertex Right
                {
                    get { return new Vertex(0, null, lightSourceData[3], lightSourceData[4], lightSourceData[5]); }
                    set { setFloatN(3, value); }
                }
                [ElementPriority(3)]
                public float Radius { get { return lightSourceData[6]; } set { setFloatN(6, value); } }
            }
            #endregion
            
            #region Content Fields
            [ElementPriority(1)]
            public LightSourceType LightType { get { return lightSource; } set { if (lightSource != value) { lightSource = value; PointToLightSourceData(); OnElementChanged(); } } }
            [ElementPriority(2)]
            public Vertex Transform { get { return transform; } set { if (!transform.Equals(value)) { transform = new Vertex(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(3)]
            public RGB Color { get { return color; } set { if (!color.Equals(value)) { color = new RGB(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(4)]
            public float Intensity { get { return intensity; } set { if (intensity != value) { intensity = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public GeneralLightSourceType LightSourceData { get { return lightSourceData; } set { } }
            [ElementPriority(5)]
            public SpotLightSourceType SpotLightLightSourceData { get { return spotLightSourceData; } set { } }
            [ElementPriority(5)]
            public LampShadeLightSourceType LampShadeLightSourceData { get { return lampShadeLightSourceData; } set { } }
            [ElementPriority(5)]
            public TubeLightSourceType TubeLightLightSourceData { get { return tubeShadeLightSourceData; } set { } }
            [ElementPriority(5)]
            public SquareWindowLightSourceType SquareWindowLightSourceData { get { return squareWindowLightSourceData; } set { } }
            [ElementPriority(5)]
            public CircularWindowLightSourceType CircularWindowLightSourceData { get { return circularWindowLightSourceData; } set { } }
            [ElementPriority(5)]
            public SquareWindowLightSourceType SquareAreaLightSourceData { get { return squareAreaLightSourceData; } set { } }
            [ElementPriority(5)]
            public CircularWindowLightSourceType DiscAreaLightSourceData { get { return discAreaLightSourceData; } set { } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }

        public class LightSourceList : DependentList<LightSource>
        {
            int count;

            #region Constructors
            public LightSourceList(EventHandler handler) : base(handler, Byte.MaxValue) { }
            public LightSourceList(EventHandler handler, int count, Stream s) : base(null, Byte.MaxValue) { this.count = count; elementHandler = handler; Parse(s); this.handler = handler; }
            public LightSourceList(EventHandler handler, IEnumerable<LightSource> llp) : base(handler, llp, Byte.MaxValue) { }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return count; }
            protected override void WriteCount(Stream s, int count) { }

            protected override LightSource CreateElement(Stream s) { return new LightSource(0, elementHandler, s); }

            protected override void WriteElement(Stream s, LightSource element) { element.UnParse(s); }
            #endregion
        }

        public class Occluder : AHandlerElement, IEquatable<Occluder>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            OccluderType occluderType = 0;
            Vertex origin;
            Vertex normal;
            Vertex xAxis;
            Vertex yAxis;
            float pairOffset;
            #endregion

            #region Constructors
            public Occluder(int APIversion, EventHandler handler)
                : this(APIversion, handler, Occluder.OccluderType.Disc,
                    new Vertex(0, null, 0f, 0f, 0f), new Vertex(0, null, 0f, 0f, 0f), new Vertex(0, null, 0f, 0f, 0f), new Vertex(0, null, 0f, 0f, 0f), 0f) { }
            public Occluder(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Occluder(int APIversion, EventHandler handler, Occluder basis)
                : this(APIversion, handler, basis.occluderType, basis.origin, basis.normal, basis.xAxis, basis.yAxis, basis.pairOffset) { }
            public Occluder(int APIversion, EventHandler handler, OccluderType occluderType, Vertex origin, Vertex normal, Vertex xAxis, Vertex yAxis, float pairOffset)
                : base(APIversion, handler)
            {
                this.occluderType = occluderType;
                this.origin = new Vertex(requestedApiVersion, handler, origin);
                this.normal = new Vertex(requestedApiVersion, handler, normal);
                this.xAxis = new Vertex(requestedApiVersion, handler, xAxis);
                this.yAxis = new Vertex(requestedApiVersion, handler, yAxis);
                this.pairOffset = pairOffset;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                occluderType = (OccluderType)r.ReadUInt32();
                origin = new Vertex(recommendedApiVersion, handler, s);
                normal = new Vertex(recommendedApiVersion, handler, s);
                xAxis = new Vertex(recommendedApiVersion, handler, s);
                yAxis = new Vertex(recommendedApiVersion, handler, s);
                pairOffset = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)occluderType);
                origin.UnParse(s);
                normal.UnParse(s);
                xAxis.UnParse(s);
                yAxis.UnParse(s);
                w.Write(pairOffset);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            /// <summary>
            /// The list of available field names on this API object
            /// </summary>
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<Occluder> Members

            public bool Equals(Occluder other)
            {
                return occluderType.Equals(other.occluderType)
                    && origin.Equals(other.origin)
                    && normal.Equals(other.normal)
                    && xAxis.Equals(other.xAxis)
                    && yAxis.Equals(other.yAxis)
                    && pairOffset.Equals(other.pairOffset)
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as Occluder != null ? this.Equals(obj as Occluder) : false;
            }

            public override int GetHashCode()
            {
                return occluderType.GetHashCode()
                    ^ origin.GetHashCode()
                    ^ normal.GetHashCode()
                    ^ xAxis.GetHashCode()
                    ^ yAxis.GetHashCode()
                    ^ pairOffset.GetHashCode()
                    ;
            }

            #endregion

            #region Sub-types
            public enum OccluderType : uint
            {
                Disc = 0x00,
                Rectangle = 0x01,
            }
            #endregion

            #region Content Fields
            [ElementPriority(0)]
            public OccluderType Occluder_Type { get { return occluderType; } set { if (occluderType != value) { occluderType = value; OnElementChanged(); } } }
            [ElementPriority(1)]
            public Vertex Origin { get { return origin; } set { if (!origin.Equals(value)) { origin = new Vertex(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(2)]
            public Vertex Normal { get { return normal; } set { if (!normal.Equals(value)) { normal = new Vertex(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(3)]
            public Vertex XAxis { get { return xAxis; } set { if (!xAxis.Equals(value)) { xAxis = new Vertex(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(4)]
            public Vertex YAxis { get { return yAxis; } set { if (!yAxis.Equals(value)) { yAxis = new Vertex(requestedApiVersion, handler, value); OnElementChanged(); } } }
            [ElementPriority(5)]
            public float PairOffset { get { return pairOffset; } set { pairOffset = value; OnElementChanged(); } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }

        public class OccluderList : DependentList<Occluder>
        {
            int count;

            #region Constructors
            public OccluderList(EventHandler handler) : base(handler, Byte.MaxValue) { }
            public OccluderList(EventHandler handler, int count, Stream s) : base(null, Byte.MaxValue) { this.count = count; elementHandler = handler; Parse(s); this.handler = handler; }
            public OccluderList(EventHandler handler, IEnumerable<Occluder> lss) : base(handler, lss, Byte.MaxValue) { }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return count; }
            protected override void WriteCount(Stream s, int count) { }

            protected override Occluder CreateElement(Stream s) { return new Occluder(0, elementHandler, s); }

            protected override void WriteElement(Stream s, Occluder element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(12)]
        public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(13)]
        public ushort Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(14)]
        public LightSourceList LightSources { get { return lightSources; } set { if (lightSources != value) { lightSources = value == null ? null : new LightSourceList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }
        [ElementPriority(15)]
        public OccluderList Occluders { get { return occluders; } set { if (occluders != value) { occluders = value == null ? null : new OccluderList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }
}
