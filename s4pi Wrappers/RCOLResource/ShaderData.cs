/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s4pi.Interfaces;
using System.IO;

namespace RCOLResource
{

    public class ShaderData : AHandlerElement, IEquatable<ShaderData>
    {
        public ShaderData(int APIversion, EventHandler handler, FieldType field, DataType dataType, Object data) : base(APIversion, handler)
        {
            this.field = field; this.shaderDataType = dataType; this.Data = data;
        }

        public FieldType field { get; set; }
        public DataType shaderDataType { get; set; }
        public Object Data { get; set; }


        const int recommendedApiVersion = 1;

        #region AHandlerElement Members
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        #region Data I/O
        public static ShaderData CreateShaderData(Stream s, long start, EventHandler handler, RCOL.RCOLChunkType rcolType)
        {
            BinaryReader r = new BinaryReader(s);
            var field = (FieldType)r.ReadUInt32();
            var shaderDataType = (DataType)r.ReadUInt32();
            int count = r.ReadInt32();
            uint offset = r.ReadUInt32();
            long pos = s.Position;
            s.Position = offset + start;
            Object data = null;
            switch(shaderDataType)
            {
                case DataType.dtFloat:
                    switch(count)
                    {
                        case 1:
                            data = Tuple.Create(r.ReadSingle());
                            break;
                        case 2:
                            data = Tuple.Create(r.ReadSingle(), r.ReadSingle());
                            break;
                        case 3:
                            data = Tuple.Create(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                            break;
                        case 4:
                            data = Tuple.Create(r.ReadSingle(), r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
                            break;
                        default:
                            break;
                    }
                    break;
                case DataType.dtImageMap:

                    data = new TGIBlock(1, handler, "ITG", s);
                    
                    break;
                case DataType.dtInt:
                    switch(count)
                    {
                        case 1:
                            data = Tuple.Create(r.ReadInt32());
                            break;
                        default:
                            break;
                    }
                    break;
                case DataType.dtTexture:
                    if(rcolType == RCOL.RCOLChunkType.GEOM)
                    {
                        data = r.ReadInt32();
                    }
                    else
                    {
                        data = new TGIBlock(1, handler, "ITG", s);
                    }
                    break;
                case DataType.dtUnknown:
                    break;
                default:
                    break;
                
            }
            s.Position = pos;
            return new ShaderData(1, handler, field, shaderDataType, data);
        }


        protected internal void UnParse(Stream s, long start)
        {

        }

        private int GetByteSize()
        {
            if(typeof(this.Data) == null)
            {

            }
            return 0;
        }
        #endregion

        public bool Equals(ShaderData other)
        {
            return this.field == other.field && this.Data == other.Data && this.shaderDataType == other.shaderDataType;
        }


        public string Value { get { return ValueBuilder; } }
    }

    public class ShaderDataList : DependentList<ShaderData>
    {
        RCOL.RCOLChunkType rcolType;
        public ShaderDataList(EventHandler handler, Stream s, RCOL.RCOLChunkType type, long start) : base(handler) { this.rcolType = type; Parse(s, start); }

        #region Data I/O
        void Parse(Stream s, long start)
        {
            BinaryReader r = new BinaryReader(s);
            int size = r.ReadInt32();
            int count = r.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                this.Add(ShaderData.CreateShaderData(s, start, size, elementHandler, this.rcolType));
            }
            
        }

        public void UnParse(Stream s, long start)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write(0); // will be written later
            w.Write(this.Count);
        }
        #endregion

        protected override ShaderData CreateElement(Stream s) { throw new NotSupportedException(); }
        protected override void WriteElement(Stream s, ShaderData element) { throw new NotSupportedException(); }
    }

    public enum ShaderType : uint
    {
        None = 0x00000000,
        Subtractive = 0x0B272CC5,
        Instanced = 0x0CB82EB8,
        FullBright = 0x14FA335E,
        PreviewWallsAndFloors = 0x213D6300,
        ShadowMap = 0x21FE207D,
        GlassForRabbitHoles = 0x265FFAA1,
        ImpostorWater = 0x277CF8EB,
        Rug = 0x2A72B9A1,
        Trampoline = 0x3939E094,
        Foliage = 0x4549E22E,
        ParticleAnim = 0x460E93F4,
        SolidPhong = 0x47C6638C,
        GlassForObjects = 0x492ECA7C,
        Stairs = 0x4CE2F497,
        OutdoorProp = 0x4D26BEC0,
        GlassForFences = 0x52986C62,
        SimSkin = 0x548394B9,
        Additive = 0x5AF16731,
        SimGlass = 0x5EDA9CDE,
        Fence = 0x67107FE8,
        LotImposter = 0x68601DE3,
        Blueprint = 0x6864A45E,
        BasinWater = 0x6AAD2AD5,
        StandingWater = 0x70FDE012,
        BuildingWindow = 0x7B036C01,
        Roof = 0x7BD05F63,
        GlassForPortals = 0x81DD204D,
        GlassForObjectsTranslucent = 0x849CF021,
        SimHair = 0x84FD7152,
        Landmark = 0x8A60B969,
        RabbitHoleHighDetail = 0x8D346BBC,
        CASRoom = 0x94B9A835,
        SimEyelashes = 0x9D9DA161,
        Gemstones = 0xA063C1D0,
        Counters = 0xA4172F62,
        FlatMirror = 0xA68D9E29,
        Painting = 0xAA495821,
        RabbitHoleMediumDetail = 0xAEDE7105,
        Phong = 0xB9105A6D,
        Floors = 0xBC84D000,
        DropShadow = 0xC09C7582,
        SimEyes = 0xCF8A70B4,
        Plumbob = 0xDEF16564,
        SculptureIce = 0xE5D98507,
        PhongAlpha = 0xFC5FC212,
        ParticleJet = 0xFF5E6908,
    }

    public enum FieldType : uint
    {
        None = 0x00000000,
        AlignAcrossDirection = 0x01885886, // Float
        DimmingCenterHeight = 0x01ADACE0, // Float
        Transparency = 0x05D22FD3, // Float
        BlendSourceMode = 0x0995E96C, // Float
        SharpSpecControl = 0x11483F01, // Float
        RotateSpeedRadsSec = 0x16BF7A44, // Float
        AlignToDirection = 0x17B78AF6, // Float
        DropShadowStrength = 0x1B1AB4D5, // Float
        ContourSmoothing = 0x1E27DCCD, // Float
        [Obsolete("Deprecated")]
        reflectivity = 0x29BCDD1F, // Float
        BlendOperation = 0x2D13B939, // Float
        RotationSpeed = 0x32003AD4, // Float
        DimmingRadius = 0x32DFA298, // Float
        IsGenericBox = 0x347C9E07, // Float
        IsSolidObject = 0x3BBF99CF, // Float
        NormalMapScale = 0x3C45E334, // Float
        NoAutomaticDaylightDimming = 0x3CB5FA70, // Float
        FramesPerSecond = 0x406ADE00, // Float
        BloomFactor = 0x4168508B, // Float
        EmissiveBloomMultiplier = 0x490E6EB4, // Float
        IsObject = 0x4C12ECE8, // Float
        IsPartition = 0x5250023D, // Float
        RippleSpeed = 0x52DEC070, // Float
        UseLampColor = 0x56B220CD, // Float
        TextureSpeedScale = 0x583DF357, // Float
        NoiseMapScale = 0x5E86DEA1, // Float
        AutoRainbow = 0x5F7800EA, // Float
        DebouncePower = 0x656025DF, // Float
        SpeedStretchFactor = 0x66479028, // Float
        WindSpeed = 0x66E9B6BC, // Float
        DaytimeOnly = 0x6BB389BC, // Float
        FramesRandomStartFactor = 0x7211F24F, // Float
        DeflectionThreshold = 0x7D621D61, // Float
        LifetimeSeconds = 0x84212733, // Float
        NormalBumpScale = 0x88C64AE2, // Float
        DeformerOffset = 0x8BDF4746, // Float
        EdgeDarkening = 0x8C27D8C9, // Float
        OverrideFactor = 0x8E35CCC0, // Float
        EmissiveLightMultiplier = 0x8EF71C85, // Float
        SharpSpecThreshold = 0x903BE4D3, // Float
        RugSort = 0x906997A9, // Float
        Layer2Shift = 0x92692CB2, // Float
        SpecStyle = 0x9554D40F, // Float
        FadeDistance = 0x957210EA, // Float
        BlendDestMode = 0x9BDECB37, // Float
        LightingEnabled = 0xA15E4594, // Float
        OverrideSpeed = 0xA3D6342E, // Float
        VisibleOnlyAtNight = 0xAC5D0A82, // Float
        UseDiffuseForAlphaTest = 0xB597FA7F, // Float
        SparkleSpeed = 0xBA13921E, // Float
        WindStrength = 0xBC4A2544, // Float
        HaloBlur = 0xC3AD4F50, // Float
        RefractionDistortionScale = 0xC3C472A1, // Float
        DiffuseMapUVChannel = 0xC45A5F41, // Float
        SpecularMapUVChannel = 0xCB053686, // Float
        ParticleCount = 0xCC31B828, // Float
        RippleDistanceScale = 0xCCB35B98, // Float
        DivetScale = 0xCE8C8311, // Float
        ForceAmount = 0xD4D51D02, // Float
        AnimSpeed = 0xD600CB63, // Float
        BackFaceDiffuseContribution = 0xD641A1B1, // Float
        BounceAmountMeters = 0xD8542D8B, // Float
        IsFloor = 0xD9C05335, // Float
        [Obsolete("Deprecated")]
        index_of_refraction = 0xDAA9532D, // Float
        BloomScale = 0xE29BA4AC, // Float
        AlphaMaskThreshold = 0xE77A2B60, // Float
        LightingDirectScale = 0xEF270EE4, // Float
        AlwaysOn = 0xF019641D, // Float
        Shininess = 0xF755F7FF, // Float
        FresnelOffset = 0xFB66A8CB, // Float
        BouncePower = 0xFBA6B898, // Float
        ShadowAlphaTest = 0xFEB1F9CB, // Float
        DiffuseUVScale = 0x2D4E507E, // Float2
        RippleHeights = 0x6A07D7E1, // Float2
        CutoutValidHeights = 0x6D43D7B7, // Float2
        UVTiling = 0x773CAB85, // Float2
        SizeScaleEnd = 0x891A3133, // Float2
        StretchRect = 0x8D38D12E, // Float2
        SizeScaleStart = 0x9A6C2EC8, // Float2
        WaterScrollSpeedLayer2 = 0xAFA11435, // Float2
        WaterScrollSpeedLayer1 = 0xAFA11436, // Float2
        NormalUVScale = 0xBA2D1AB9, // Float2
        DetailUVScale = 0xCD985A0B, // Float2
        SpecularUVScale = 0xF12E27C3, // Float2
        UVScrollSpeed = 0xF2EEA6EC, // Float2
        [Obsolete("Deprecated")]
        Ambient = 0x04A5DAA3, // Float3
        OverrideDirection = 0x0C12DED8, // Float3
        OverrideVelocity = 0x14677578, // Float3
        CounterMatrixRow1 = 0x1EF8655D, // Float3
        CounterMatrixRow2 = 0x1EF8655E, // Float3
        ForceDirection = 0x29881F55, // Float3
        Specular = 0x2CE11842, // Float3
        HaloLowColor = 0x2EB8E8D4, // Float3
        [Obsolete("Deprecated")]
        Emission = 0x3BD441A0, // Float3
        NormalMapUVSelector = 0x415368B4, // Float3
        UVScales = 0x420520E9, // Float3
        LightMapScale = 0x4F7DCB9B, // Float3
        Diffuse = 0x637DAA05, // Float3
        [Obsolete("Deprecated")]
        Reflective = 0x73C9923E, // Float3
        AmbientUVSelector = 0x797F8E81, // Float3
        HighlightColor = 0x90F8DCF0, // Float3
        DiffuseUVSelector = 0x91EEBAFF, // Float3
        [Obsolete("Deprecated")]
        Transparent = 0x988403F9, // Float3
        VertexColorScale = 0xA2FD73CA, // Float3
        SpecularUVSelector = 0xB63546AC, // Float3
        EmissionMapUVSelector = 0xBC823DDC, // Float3
        HaloHighColor = 0xD4043258, // Float3
        RootColor = 0xE90599F6, // Float3
        ForceVector = 0xEBA4727B, // Float3
        PositionTweak = 0xEF36D180, // Float3
        TimelineLength = 0x0081AE98, // Float4
        UVScale = 0x159BA53E, // Float4
        FrameData = 0x1E5B2324, // Float4
        AnimDir = 0x3F89C2EF, // Float4
        PosScale = 0x487648E5, // Float4
        Births = 0x568E0367, // Float4
        UVOffset = 0x57582869, // Float4
        PosOffset = 0x790EBF2C, // Float4
        AverageColor = 0x449A3A67, // Int
        MaskWidth = 0x707F712F, // Int
        MaskHeight = 0x849CDADC, // Int
        SparkleCube = 0x1D90C086, // Texture
        DropShadowAtlas = 0x22AD8507, // Texture
        DirtOverlay = 0x48372E62, // Texture
        OverlayTexture = 0x4DC0C8BC, // Texture
        JetTexture = 0x52CE211B, // Texture
        ColorRamp = 0x581835D6, // Texture
        DiffuseMap = 0x6CC0FD85, // Texture
        SelfIlluminationMap = 0x6E067554, // Texture
        NormalMap = 0x6E56548A, // Texture
        HaloRamp = 0x84F6E0FB, // Texture
        DetailMap = 0x9205DAA8, // Texture
        SpecularMap = 0xAD528A60, // Texture
        AmbientOcclusionMap = 0xB01CBA60, // Texture
        AlphaMap = 0xC3FAAC4F, // Texture
        MultiplyMap = 0xCD869A45, // Texture
        SpecCompositeTexture = 0xD652FADE, // Texture
        NoiseMap = 0xE19FD579, // Texture
        RoomLightMap = 0xE7CA9166, // Texture
        EmissionMap = 0xF303D152, // Texture
        RevealMap = 0xF3F22AC4, // Texture
        ImposterTextureAOandSI = 0x15C9D298, // TextureKey
        ImpostorDetailTexture = 0x56E1C6B2, // TextureKey
        ImposterTexture = 0xBDCF71C5, // TextureKey
        ImposterTextureWater = 0xBF3FB9FA, // TextureKey
    }

    public enum DataType : uint
    {
        dtUnknown = 0,
        dtFloat = 1,
        dtInt = 2,
        dtTexture = 4,
        dtImageMap = 0x00010004
    }
}
