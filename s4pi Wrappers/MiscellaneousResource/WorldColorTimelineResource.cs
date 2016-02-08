/***************************************************************************
 *  Copyright (C) 2016 by Sims 4 Tools Development Team                    *
 *  Credits: Peter Jones, Cmar                                             *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
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
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s4pi.Miscellaneous
{
    public class WorldColorTimeLineResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint version;
        ColorTimeLineList colorTimeLines;
        #endregion

        public WorldColorTimeLineResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader br = new BinaryReader(s);
            this.version = br.ReadUInt32();
            this.colorTimeLines = new ColorTimeLineList(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(this.version);
            this.colorTimeLines.UnParse(ms);
            return ms;
        }
        #endregion
        
        #region Subclasses
        public class ColorTimeLineList : DependentList<ColorTimeLine>
        {
                public ColorTimeLineList(EventHandler handler, long maxSize = -1)
                    : base(handler, maxSize)
                {
                }

                public ColorTimeLineList(EventHandler handler, IEnumerable<ColorTimeLine> timeline, long maxSize = -1)
                    : base(handler, timeline, maxSize)
                {
                }

                public ColorTimeLineList(EventHandler handler, Stream s, long maxSize = -1)
                    : base(handler, s, maxSize)
                {
                }

                protected override ColorTimeLine CreateElement(Stream s)
                {
                    return new ColorTimeLine(recommendedApiVersion, this.elementHandler, s);
                }

                protected override void WriteElement(Stream s, ColorTimeLine element)
                {
                    element.UnParse(s);
                }

        }

        public class ColorTimeLine : AHandlerElement, IEquatable<ColorTimeLine>
        {
            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            #endregion
            
            uint version;
            ColorTimeLineData ambientColors;
            ColorTimeLineData directionalColors;
            ColorTimeLineData shadowColors;
            ColorTimeLineData skyHorizonColors;
            ColorTimeLineData fogStartRange;
            ColorTimeLineData fogEndRange;
            ColorTimeLineData skyHorizonDarkColors;
            ColorTimeLineData skyLightColors;
            ColorTimeLineData skyDarkColors;
            ColorTimeLineData sunColors;
            ColorTimeLineData haloColors;
            ColorTimeLineData sunDarkCloudColors;
            ColorTimeLineData sunLightCloudColors;
            ColorTimeLineData horizonDarkCloudColors;
            ColorTimeLineData horizonLightCloudColors;
            ColorTimeLineData cloudShadowCloudColors;
            uint poiId;   // Point of interest ID
            ColorTimeLineData bloomThresholds;
            ColorTimeLineData bloomIntensities;

            float sunriseTime;
            float sunsetTime;

            ColorTimeLineData denseFogColors;
            ColorTimeLineData denseFogStartRange;
            ColorTimeLineData denseFogEndRange;
            ColorTimeLineData moonRadiusMultipliers;
            ColorTimeLineData sunRadiusMultipliers;
            float starsAppearTime;
            float starsDisappearTime;
    
            bool remapTimeline;

           /* public ColorTimeLine(int APIversion, EventHandler handler, ColorTimeLine other)
                : this(APIversion, handler, 
                    other.version,
                    other.ambientColors,
                    other.directionalColors,
                    other.shadowColors,
                    other.skyHorizonColors,
                    other.mFogStartRange,
                    other.mFogEndRange,
                    other.skyHorizonDarkColors,
                    other.skyLightColors,
                    other.skyDarkColors,
                    other.sunColors,
                    other.haloColors,
                    other.sunDarkCloudColors,
                    other.sunLightCloudColors,
                    other.horizonDarkCloudColors,
                    other.horizonLightCloudColors,
                    other.cloudShadowCloudColors,
                    other.poiId, 
                    other.bloomThresholds,
                    other.bloomIntensities,
                    other.SunriseTime,
                    other.SunsetTime,
                    other.denseFogColors,
                    other.denseFogStartRange,
                    other.denseFogEndRange,
                    other.moonRadiusMultipliers,
                    other.sunRadiusMultipliers,
                    other.starsAppearTime,
                    other.starsDisappearTime,
                    other.remapTimeline)
            {
            } */
            public ColorTimeLine(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                this.version = 13;
                this.ambientColors = new ColorTimeLineData(1, handler);
                this.directionalColors = new ColorTimeLineData(1, handler);
                this.shadowColors = new ColorTimeLineData(1, handler);
                this.skyHorizonColors = new ColorTimeLineData(1, handler);
                this.fogStartRange = new ColorTimeLineData(1, handler);
                this.fogEndRange = new ColorTimeLineData(1, handler);
                this.skyHorizonDarkColors = new ColorTimeLineData(1, handler);
                this.skyLightColors = new ColorTimeLineData(1, handler);
                this.skyDarkColors = new ColorTimeLineData(1, handler);
                this.sunColors = new ColorTimeLineData(1, handler);
                this.haloColors = new ColorTimeLineData(1, handler);
                this.sunDarkCloudColors = new ColorTimeLineData(1, handler);
                this.sunLightCloudColors = new ColorTimeLineData(1, handler);
                this.horizonDarkCloudColors = new ColorTimeLineData(1, handler);
                this.horizonLightCloudColors = new ColorTimeLineData(1, handler);
                this.cloudShadowCloudColors = new ColorTimeLineData(1, handler);
                this.bloomThresholds = new ColorTimeLineData(1, handler);
                this.bloomIntensities = new ColorTimeLineData(1, handler);
                this.denseFogColors = new ColorTimeLineData(1, handler);
                this.denseFogStartRange = new ColorTimeLineData(1, handler);
                this.denseFogEndRange = new ColorTimeLineData(1, handler);
                this.moonRadiusMultipliers = new ColorTimeLineData(1, handler);
                this.sunRadiusMultipliers = new ColorTimeLineData(1, handler);
                this.remapTimeline = true;
            }
            public ColorTimeLine(int apiVersion, EventHandler handler, Stream s)
                : this(apiVersion, handler)
            {
                this.Parse(s);
            }

            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.version = br.ReadUInt32();
                this.ambientColors = new ColorTimeLineData(1, handler, s);
                this.directionalColors = new ColorTimeLineData(1, handler, s);
                this.shadowColors = new ColorTimeLineData(1, handler, s);
                this.skyHorizonColors = new ColorTimeLineData(1, handler, s);
                this.fogStartRange = new ColorTimeLineData(1, handler, s);
                this.fogEndRange = new ColorTimeLineData(1, handler, s);
                this.skyHorizonDarkColors = new ColorTimeLineData(1, handler, s);
                this.skyLightColors = new ColorTimeLineData(1, handler, s);
                this.skyDarkColors = new ColorTimeLineData(1, handler, s);
                this.sunColors = new ColorTimeLineData(1, handler, s);
                this.haloColors = new ColorTimeLineData(1, handler, s);
                this.sunDarkCloudColors = new ColorTimeLineData(1, handler, s);
                this.sunLightCloudColors = new ColorTimeLineData(1, handler, s);
                this.horizonDarkCloudColors = new ColorTimeLineData(1, handler, s);
                this.horizonLightCloudColors = new ColorTimeLineData(1, handler, s);
                this.cloudShadowCloudColors = new ColorTimeLineData(1, handler, s);
                this.poiId = br.ReadUInt32();
                this.bloomThresholds = new ColorTimeLineData(1, handler, s);
                this.bloomIntensities = new ColorTimeLineData(1, handler, s);
                this.sunriseTime = br.ReadSingle();
                this.sunsetTime = br.ReadSingle();
                this.denseFogColors = new ColorTimeLineData(1, handler, s);
                this.denseFogStartRange = new ColorTimeLineData(1, handler, s);
                this.denseFogEndRange = new ColorTimeLineData(1, handler, s);
                this.moonRadiusMultipliers = new ColorTimeLineData(1, handler, s);
                this.sunRadiusMultipliers = new ColorTimeLineData(1, handler, s);
                this.starsAppearTime = br.ReadSingle();
                this.starsDisappearTime = br.ReadSingle();
                if (this.version >= 14)
                {
                    this.remapTimeline = br.ReadBoolean();
                } 
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(this.version);
                this.ambientColors.UnParse(s);
                this.directionalColors.UnParse(s);
                this.shadowColors.UnParse(s);
                this.skyHorizonColors.UnParse(s);
                this.fogStartRange.UnParse(s);
                this.fogEndRange.UnParse(s);
                this.skyHorizonDarkColors.UnParse(s);
                this.skyLightColors.UnParse(s);
                this.skyDarkColors.UnParse(s);
                this.sunColors.UnParse(s);
                this.haloColors.UnParse(s);
                this.sunDarkCloudColors.UnParse(s);
                this.sunLightCloudColors.UnParse(s);
                this.horizonDarkCloudColors.UnParse(s);
                this.horizonLightCloudColors.UnParse(s);
                this.cloudShadowCloudColors.UnParse(s);
                bw.Write(this.poiId);
                this.bloomThresholds.UnParse(s);
                this.bloomIntensities.UnParse(s);
                bw.Write(this.sunriseTime);
                bw.Write(this.sunsetTime);
                this.denseFogColors.UnParse(s);
                this.denseFogStartRange.UnParse(s);
                this.denseFogEndRange.UnParse(s);
                this.moonRadiusMultipliers.UnParse(s);
                this.sunRadiusMultipliers.UnParse(s);
                bw.Write(this.starsAppearTime);
                bw.Write(this.starsDisappearTime);
                if (this.version >= 14)
                {
                    bw.Write(this.remapTimeline);
                }
            }

            [ElementPriority(0)]
            public uint Version { get { return this.version; } set { if (this.version != value) { OnElementChanged(); this.version = value; } } }
            [ElementPriority(1)]
            public ColorTimeLineData AmbientColors { get { return this.ambientColors; } set { if (this.ambientColors != value) { OnElementChanged(); this.ambientColors = value; } } }
            [ElementPriority(2)]
            public ColorTimeLineData DirectionalColors { get { return this.directionalColors; } set { if (this.directionalColors != value) { OnElementChanged(); this.directionalColors = value; } } }
            [ElementPriority(3)]
            public ColorTimeLineData ShadowColors { get { return this.shadowColors; } set { if (this.shadowColors != value) { OnElementChanged(); this.shadowColors = value; } } }
            [ElementPriority(4)]
            public ColorTimeLineData SkyHorizonColors { get { return this.skyHorizonColors; } set { if (this.skyHorizonColors != value) { OnElementChanged(); this.skyHorizonColors = value; } } }
            [ElementPriority(5)]
            public ColorTimeLineData FogStartRange { get { return this.fogStartRange; } set { if (this.fogStartRange != value) { OnElementChanged(); this.fogStartRange = value; } } }
            [ElementPriority(6)]
            public ColorTimeLineData FogEndRange { get { return this.fogEndRange; } set { if (this.fogEndRange != value) { OnElementChanged(); this.fogEndRange = value; } } }
            [ElementPriority(7)]
            public ColorTimeLineData SkyHorizonDarkColors { get { return this.skyHorizonDarkColors; } set { if (this.skyHorizonDarkColors != value) { OnElementChanged(); this.skyHorizonDarkColors = value; } } }
            [ElementPriority(8)]
            public ColorTimeLineData SkyLightColors { get { return this.skyLightColors; } set { if (this.skyLightColors != value) { OnElementChanged(); this.skyLightColors = value; } } }
            [ElementPriority(9)]
            public ColorTimeLineData SkyDarkColors { get { return this.skyDarkColors; } set { if (this.skyDarkColors != value) { OnElementChanged(); this.skyDarkColors = value; } } }
            [ElementPriority(10)]
            public ColorTimeLineData SunColors { get { return this.sunColors; } set { if (this.sunColors != value) { OnElementChanged(); this.sunColors = value; } } }
            [ElementPriority(11)]
            public ColorTimeLineData HaloColors { get { return this.haloColors; } set { if (this.haloColors != value) { OnElementChanged(); this.haloColors = value; } } }
            [ElementPriority(12)]
            public ColorTimeLineData SunDarkCloudColors { get { return this.sunDarkCloudColors; } set { if (this.sunDarkCloudColors != value) { OnElementChanged(); this.sunDarkCloudColors = value; } } }
            [ElementPriority(13)]
            public ColorTimeLineData SunLightCloudColors { get { return this.sunLightCloudColors; } set { if (this.sunLightCloudColors != value) { OnElementChanged(); this.sunLightCloudColors = value; } } }
            [ElementPriority(14)]
            public ColorTimeLineData HorizonDarkCloudColors { get { return this.horizonDarkCloudColors; } set { if (this.horizonDarkCloudColors != value) { OnElementChanged(); this.horizonDarkCloudColors = value; } } }
            [ElementPriority(15)]
            public ColorTimeLineData HorizonLightCloudColors { get { return this.horizonLightCloudColors; } set { if (this.horizonLightCloudColors != value) { OnElementChanged(); this.horizonLightCloudColors = value; } } }
            [ElementPriority(16)]
            public ColorTimeLineData CloudShadowCloudColors { get { return this.cloudShadowCloudColors; } set { if (this.cloudShadowCloudColors != value) { OnElementChanged(); this.cloudShadowCloudColors = value; } } }
            [ElementPriority(17)]
            public uint PointOfInterestId { get { return this.poiId; } set { if (this.poiId != value) { OnElementChanged(); this.poiId = value; } } }
            [ElementPriority(18)]
            public ColorTimeLineData BloomThresholds { get { return this.bloomThresholds; } set { if (this.bloomThresholds != value) { OnElementChanged(); this.bloomThresholds = value; } } }
            [ElementPriority(19)]
            public ColorTimeLineData BloomIntensities { get { return this.bloomIntensities; } set { if (this.bloomIntensities != value) { OnElementChanged(); this.bloomIntensities = value; } } }
            [ElementPriority(20)]
            public float SunriseTime { get { return this.sunriseTime; } set { if (this.sunriseTime != value) { OnElementChanged(); this.sunriseTime = value; } } }
            [ElementPriority(21)]
            public float SunsetTime { get { return this.sunsetTime; } set { if (this.sunsetTime != value) { OnElementChanged(); this.sunsetTime = value; } } }
            [ElementPriority(22)]
            public ColorTimeLineData DenseFogColors { get { return this.denseFogColors; } set { if (this.denseFogColors != value) { OnElementChanged(); this.denseFogColors = value; } } }
            [ElementPriority(23)]
            public ColorTimeLineData DenseFogStartRange { get { return this.denseFogStartRange; } set { if (this.denseFogStartRange != value) { OnElementChanged(); this.denseFogStartRange = value; } } }
            [ElementPriority(24)]
            public ColorTimeLineData DenseFogEndRange { get { return this.denseFogEndRange; } set { if (this.denseFogEndRange != value) { OnElementChanged(); this.denseFogEndRange = value; } } }
            [ElementPriority(25)]
            public ColorTimeLineData MoonRadiusMultipliers { get { return this.moonRadiusMultipliers; } set { if (this.moonRadiusMultipliers != value) { OnElementChanged(); this.moonRadiusMultipliers = value; } } }
            [ElementPriority(26)]
            public ColorTimeLineData SunRadiusMultipliers { get { return this.sunRadiusMultipliers; } set { if (this.sunRadiusMultipliers != value) { OnElementChanged(); this.sunRadiusMultipliers = value; } } }
            [ElementPriority(27)]
            public float StarsAppearTime { get { return this.starsAppearTime; } set { if (this.starsAppearTime != value) { OnElementChanged(); this.starsAppearTime = value; } } }
            [ElementPriority(28)]
            public float StarsDisappearTime { get { return this.starsDisappearTime; } set { if (this.starsDisappearTime != value) { OnElementChanged(); this.starsDisappearTime = value; } } }
            [ElementPriority(29)]
            public bool RemapTimeline { get { return this.remapTimeline; } set { if (this.remapTimeline != value) { OnElementChanged(); this.remapTimeline = value; } } }
            
            public string Value { get { return ValueBuilder; } }

            public override List<string> ContentFields
            {
                get
                {
                    var res = GetContentFields(recommendedApiVersion, this.GetType());
                    if (this.version < 14)
                    {
                        res.Remove("RemapTimeline");
                    }
                    return res;
                }
            }

            public bool Equals(ColorTimeLine other)
            {
                return this.ambientColors.Equals(other.ambientColors) &&
                this.directionalColors.Equals(other.directionalColors) &&
                this.shadowColors.Equals(other.shadowColors) &&
                this.skyHorizonColors.Equals(other.skyHorizonColors) &&
                this.fogStartRange.Equals(other.fogStartRange) &&
                this.fogEndRange.Equals(other.fogEndRange) &&
                this.skyHorizonDarkColors.Equals(other.skyHorizonDarkColors) &&
                this.skyLightColors.Equals(other.skyLightColors) &&
                this.skyDarkColors.Equals(other.skyDarkColors) &&
                this.sunColors.Equals(other.sunColors) &&
                this.haloColors.Equals(other.haloColors) &&
                this.sunDarkCloudColors.Equals(other.sunDarkCloudColors) &&
                this.sunLightCloudColors.Equals(other.sunLightCloudColors) &&
                this.horizonDarkCloudColors.Equals(other.horizonDarkCloudColors) &&
                this.horizonLightCloudColors.Equals(other.horizonLightCloudColors) &&
                this.cloudShadowCloudColors.Equals(other.cloudShadowCloudColors) &&
                this.poiId == other.poiId &&
                this.bloomThresholds.Equals(other.bloomThresholds) &&
                this.bloomIntensities.Equals(other.bloomIntensities) &&
                this.sunriseTime == other.sunriseTime &&
                this.sunsetTime == other.sunsetTime &&
                this.denseFogColors.Equals(other.denseFogColors) &&
                this.denseFogStartRange.Equals(other.denseFogStartRange) &&
                this.denseFogEndRange.Equals(other.denseFogEndRange) &&
                this.moonRadiusMultipliers.Equals(other.moonRadiusMultipliers) &&
                this.sunRadiusMultipliers.Equals(other.sunRadiusMultipliers) &&
                this.starsAppearTime == other.starsAppearTime &&
                this.starsDisappearTime == other.starsDisappearTime &&
                this.remapTimeline == other.remapTimeline;
            }
        }

        public class ColorTimeLineData : AHandlerElement, IEquatable<ColorTimeLineData>
        {
            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            ColorDataList colorData;

            public ColorTimeLineData(int apiVersion, EventHandler handler, ColorTimeLineData other)
                : this(apiVersion, handler, other.colorData)
            {
            }
            public ColorTimeLineData(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                this.colorData = new ColorDataList(handler);
            }
            public ColorTimeLineData(int apiVersion, EventHandler handler, Stream s)
                : this(apiVersion, handler)
            {
                this.Parse(s);
            }
            public ColorTimeLineData(int apiVersion, EventHandler handler, ColorDataList colorData)
                : base(apiVersion, handler)
            {
                this.colorData = new ColorDataList(handler, colorData);
            }

            void Parse(Stream s)
            {
                this.colorData = new ColorDataList(handler, s);
            }

            public void UnParse(Stream s)
            {
                this.colorData.UnParse(s);
            }

            [ElementPriority(0)]
            public ColorDataList ColorData { get { return this.colorData; } set { if (this.colorData != value) { OnElementChanged(); this.colorData = value; } } }

            public string Value { get { return ValueBuilder; } }

            public bool Equals(ColorTimeLineData other)
            {
                return Enumerable.SequenceEqual(this.colorData, other.colorData);
            }
        }

        public class ColorDataList : DependentList<ColorData>
        {
            public ColorDataList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public ColorDataList(EventHandler handler, IEnumerable<ColorData> colorData, long maxSize = -1)
                : base(handler, colorData, maxSize)
            {
            }

            public ColorDataList(EventHandler handler, Stream s, long maxSize = -1)
                : base(handler, s, maxSize)
            {
            }

            protected override ColorData CreateElement(Stream s)
            {
                return new ColorData(recommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, ColorData element)
            {
                element.UnParse(s);
            }

        }
        
        public class ColorData : AHandlerElement, IEquatable<ColorData>
        {
            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            float r, g, b, a;
            float time;

            public ColorData(int apiVersion, EventHandler handler, ColorData other)
                : this(apiVersion, handler, other.r, other.g, other.b, other.a, other.time)
            {
            }
            public ColorData(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
            }
            public ColorData(int apiVersion, EventHandler handler, Stream s)
                : this(apiVersion, handler)
            {
                this.Parse(s);
            }
            public ColorData(int apiVersion, EventHandler handler, float r, float g, float b, float a, float time)
                : base(apiVersion, handler)
            {
                this.r = r;
                this.g = g;
                this.b = b;
                this.a = a;
                this.time = time;
            }

            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.r = br.ReadSingle();
                this.g = br.ReadSingle();
                this.b = br.ReadSingle();
                this.a = br.ReadSingle();
                this.time = br.ReadSingle();
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(this.r);
                bw.Write(this.g);
                bw.Write(this.b);
                bw.Write(this.a);
                bw.Write(this.time);
            }

            [ElementPriority(0)]
            public float R { get { return this.r; } set { if (this.r != value) { OnElementChanged(); this.r = value; } } }
            [ElementPriority(1)]
            public float G { get { return this.g; } set { if (this.g != value) { OnElementChanged(); this.g = value; } } }
            [ElementPriority(2)]
            public float B { get { return this.b; } set { if (this.b != value) { OnElementChanged(); this.b = value; } } }
            [ElementPriority(3)]
            public float A { get { return this.a; } set { if (this.a != value) { OnElementChanged(); this.a = value; } } }
            [ElementPriority(4)]
            public float Time { get { return this.time; } set { if (this.time != value) { OnElementChanged(); this.time = value; } } }

            public string Value { get { return String.Format("R: {0}, G: {1} B: {2} A: {3} Time: {4}", this.r, this.g, this.b, this.a, this.time); } }

            public bool Equals(ColorData other)
            {
                return this.r == other.r &&
                       this.g == other.g &&
                       this.b == other.b &&
                       this.a == other.a &&
                       this.time == other.time;
            }
        }        
        #endregion


        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public ColorTimeLineList ColorTimeLines
        {
            get { return this.colorTimeLines; }
            set { if (this.colorTimeLines != value) { this.colorTimeLines = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        public string Value { get { return ValueBuilder; } }

        #endregion
    }

    /// <summary>
    /// ResourceHandler for WorldColorTimeLineResource wrapper
    /// </summary>
    public class WorldColorTimeLineResourceHandler : AResourceHandler
    {
        public WorldColorTimeLineResourceHandler()
        {
            this.Add(typeof(WorldColorTimeLineResource), new List<string>(new string[] { "0x19301120", }));
        }
    }
}
