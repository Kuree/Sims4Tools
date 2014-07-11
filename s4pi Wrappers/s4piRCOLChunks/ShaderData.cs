using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.GenericRCOLResource
{
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

    public abstract class ShaderData : AHandlerElement, IEquatable<ShaderData>
    {
        static bool checking = s4pi.Settings.Settings.Checking;

        const int recommendedApiVersion = 1;

        protected DependentList<TGIBlock> _ParentTGIBlocks;
        public DependentList<TGIBlock> ParentTGIBlocks { get { return _ParentTGIBlocks; } set { _ParentTGIBlocks = value; } }

        protected string _RCOLTag;
        public virtual string RCOLTag { get { return _RCOLTag; } set { _RCOLTag = value; } }

        protected FieldType field;
        long offsetPos = -1;

        #region Constructors
        protected ShaderData(int APIversion, EventHandler handler, FieldType field) : base(APIversion, handler) { this.field = field; }
        #endregion

        public static ShaderData CreateEntry(int APIversion, EventHandler handler, Stream s, long start, DependentList<TGIBlock> _ParentTGIBlocks = null, string _RCOLTag = "MATD")
        {
            BinaryReader r = new BinaryReader(s);
            FieldType field = (FieldType)r.ReadUInt32();
            DataType sdType = (DataType)r.ReadUInt32();
            int count = r.ReadInt32();
            uint offset = r.ReadUInt32();
            long pos = s.Position;
            s.Position = start + offset;
            try
            {
                #region Determine entry type
                switch (sdType)
                {
                    case DataType.dtFloat:
                        switch (count)
                        {
                            case 1: return new ElementFloat(APIversion, handler, field, s);
                            case 2: return new ElementFloat2(APIversion, handler, field, s);
                            case 3: return new ElementFloat3(APIversion, handler, field, s);
                            case 4: return new ElementFloat4(APIversion, handler, field, s);
                        }
                        throw new InvalidDataException(String.Format("Invalid count #{0}' for DataType 0x{1:X8} at 0x{2:X8}", count, (uint)sdType, s.Position));
                    case DataType.dtInt:
                        switch (count)
                        {
                            case 1: return new ElementInt(APIversion, handler, field, s);
                        }
                        throw new InvalidDataException(String.Format("Invalid count #{0}' for DataType 0x{1:X8} at 0x{2:X8}", count, (uint)sdType, s.Position));
                    case DataType.dtTexture:
                        switch (count)
                        {
                            case 4: return new ElementTextureRef(APIversion, handler, field, s, _ParentTGIBlocks, _RCOLTag);
                            case 5: return new ElementTextureKey(APIversion, handler, field, s);
                        }
                        throw new InvalidDataException(String.Format("Invalid count #{0}' for DataType 0x{1:X8} at 0x{2:X8}", count, (uint)sdType, s.Position));
                    case DataType.dtImageMap:
                        switch (count)
                        {
                            case 4: return new ElementImageMapKey(APIversion, handler, field, s);
                        }
                        throw new InvalidDataException(String.Format("Invalid count #{0}' for DataType 0x{1:X8} at 0x{2:X8}", count, (uint)sdType, s.Position));
                }
                throw new InvalidDataException(String.Format("Unknown DataType 0x{1:X8} with count #{0} at 0x{2:X8}", count, (uint)sdType, s.Position));
                #endregion
            }
            finally { s.Position = pos; }
        }

        internal int ByteCount() { return CountFromType; }

        #region Data I/O
        internal void UnParseHeader(Stream s)
        {
            BinaryWriter w = new BinaryWriter(s);
            w.Write((uint)field);
            w.Write((uint)DataTypeFromType);
            w.Write(CountFromType);
            offsetPos = s.Position;
            w.Write((uint)0);
        }

        internal void UnParseData(Stream s, long start)
        {
            if (checking) if (offsetPos < 0)
                    throw new InvalidOperationException();
            long pos = s.Position;
            s.Position = offsetPos;
            new BinaryWriter(s).Write((uint)(pos - start));
            s.Position = pos;
            UnParse(s);
        }

        protected abstract DataType DataTypeFromType { get; }
        protected abstract int CountFromType { get; }
        protected abstract void UnParse(Stream s);

        protected void ReadZeros(Stream s, int length) { while (length-- > 0) if (s.ReadByte() != 0) throw new InvalidDataException("Non-zero padding at 0x" + s.Position.ToString("X8")); }
        protected void WriteZeros(Stream s, int length) { while (length-- > 0) s.WriteByte(0); }
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
                res.Remove("ParentTGIBlocks");
                res.Remove("RCOLTag");
                return res;
            }
        }
        #endregion

        #region IEquatable<Entry> Members

        public virtual bool Equals(ShaderData other) { return this.GetType().Equals(other.GetType()) && this.field.Equals(other.field); }
        public override bool Equals(object obj)
        {
            return obj as ShaderData != null ? this.Equals(obj as ShaderData) : false;
        }
        public override int GetHashCode() { return this.field.GetHashCode(); }

        #endregion

        [ElementPriority(1)]
        public FieldType Field { get { return field; } set { if (field != value) { field = value; OnElementChanged(); } } }

        public string Value { get { return ValueBuilder.Replace("\n", "; "); } }
    }
    public class ElementFloat : ShaderData
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        Single data;
        #endregion

        #region Constructors
        public ElementFloat(int APIversion, EventHandler handler, FieldType field, Stream s) : base(APIversion, handler, field) { Parse(s); }
        public ElementFloat(int APIversion, EventHandler handler) : this(APIversion, handler, (FieldType)0, 0f) { }
        public ElementFloat(int APIversion, EventHandler handler, ElementFloat basis) : this(APIversion, handler, basis.field, basis.data) { }
        public ElementFloat(int APIversion, EventHandler handler, FieldType field, Single data) : base(APIversion, handler, field) { this.data = data; }
        #endregion

        #region Data I/O
        void Parse(Stream s) { data = new BinaryReader(s).ReadSingle(); }

        protected override void UnParse(Stream s) { new BinaryWriter(s).Write(data); }
        protected override DataType DataTypeFromType { get { return DataType.dtFloat; } }
        protected override int CountFromType { get { return 1; } }
        #endregion

        #region IEquatable<Entry> Members

        public override bool Equals(ShaderData other) { return base.Equals(other) && this.data == ((ElementFloat)other).data; }
        public override int GetHashCode() { return base.GetHashCode() ^ data.GetHashCode(); }

        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public Single Data { get { return data; } set { if (data != value) { data = value; OnElementChanged(); } } }
        #endregion
    }
    public class ElementFloat2 : ShaderData
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        Single data0;
        Single data1;
        #endregion

        #region Constructors
        public ElementFloat2(int APIversion, EventHandler handler, FieldType field, Stream s) : base(APIversion, handler, field) { Parse(s); }
        public ElementFloat2(int APIversion, EventHandler handler) : this(APIversion, handler, (FieldType)0, 0f, 0f) { }
        public ElementFloat2(int APIversion, EventHandler handler, ElementFloat2 basis) : this(APIversion, handler, basis.field, basis.data0, basis.data1) { }
        public ElementFloat2(int APIversion, EventHandler handler, FieldType field, Single data0, Single data1) : base(APIversion, handler, field) { this.data0 = data0; this.data1 = data1; }
        #endregion

        #region Data I/O
        void Parse(Stream s) { BinaryReader r = new BinaryReader(s); data0 = r.ReadSingle(); data1 = r.ReadSingle(); }

        protected override void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(data0); w.Write(data1); }
        protected override DataType DataTypeFromType { get { return DataType.dtFloat; } }
        protected override int CountFromType { get { return 2; } }
        #endregion

        #region IEquatable<Entry> Members

        public override bool Equals(ShaderData other)
        {
            return base.Equals(other)
                && this.data0 == ((ElementFloat2)other).data0
                && this.data1 == ((ElementFloat2)other).data1
                ;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ data0.GetHashCode() ^ data1.GetHashCode();
        }

        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public Single Data0 { get { return data0; } set { if (data0 != value) { data0 = value; OnElementChanged(); } } }
        [ElementPriority(12)]
        public Single Data1 { get { return data1; } set { if (data1 != value) { data1 = value; OnElementChanged(); } } }
        #endregion
    }
    public class ElementFloat3 : ShaderData
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        Single data0;
        Single data1;
        Single data2;
        #endregion

        #region Constructors
        public ElementFloat3(int APIversion, EventHandler handler, FieldType field, Stream s) : base(APIversion, handler, field) { Parse(s); }
        public ElementFloat3(int APIversion, EventHandler handler) : this(APIversion, handler, (FieldType)0, 0f, 0f, 0f) { }
        public ElementFloat3(int APIversion, EventHandler handler, ElementFloat3 basis) : this(APIversion, handler, basis.field, basis.data0, basis.data1, basis.data2) { }
        public ElementFloat3(int APIversion, EventHandler handler, FieldType field, Single data0, Single data1, Single data2) : base(APIversion, handler, field) { this.data0 = data0; this.data1 = data1; this.data2 = data2; }
        #endregion

        #region Data I/O
        void Parse(Stream s) { BinaryReader r = new BinaryReader(s); data0 = r.ReadSingle(); data1 = r.ReadSingle(); data2 = r.ReadSingle(); }

        protected override void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(data0); w.Write(data1); w.Write(data2); }
        protected override DataType DataTypeFromType { get { return DataType.dtFloat; } }
        protected override int CountFromType { get { return 3; } }
        #endregion

        #region IEquatable<Entry> Members

        public override bool Equals(ShaderData other)
        {
            return base.Equals(other)
                && this.data0 == ((ElementFloat3)other).data0
                && this.data1 == ((ElementFloat3)other).data1
                && this.data2 == ((ElementFloat3)other).data2
                ;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ data0.GetHashCode() ^ data1.GetHashCode() ^ data2.GetHashCode();
        }

        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public Single Data0 { get { return data0; } set { if (data0 != value) { data0 = value; OnElementChanged(); } } }
        [ElementPriority(12)]
        public Single Data1 { get { return data1; } set { if (data1 != value) { data1 = value; OnElementChanged(); } } }
        [ElementPriority(13)]
        public Single Data2 { get { return data2; } set { if (data2 != value) { data2 = value; OnElementChanged(); } } }
        #endregion
    }
    public class ElementFloat4 : ShaderData
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        Single data0;
        Single data1;
        Single data2;
        Single data3;
        #endregion

        #region Constructors
        public ElementFloat4(int APIversion, EventHandler handler, FieldType field, Stream s) : base(APIversion, handler, field) { Parse(s); }
        public ElementFloat4(int APIversion, EventHandler handler) : this(APIversion, handler, (FieldType)0, 0f, 0f, 0f, 0f) { }
        public ElementFloat4(int APIversion, EventHandler handler, ElementFloat4 basis) : this(APIversion, handler, basis.field, basis.data0, basis.data1, basis.data2, basis.data3) { }
        public ElementFloat4(int APIversion, EventHandler handler, FieldType field, Single data0, Single data1, Single data2, Single data3) : base(APIversion, handler, field) { this.data0 = data0; this.data1 = data1; this.data2 = data2; this.data3 = data3; }
        #endregion

        #region Data I/O
        void Parse(Stream s) { BinaryReader r = new BinaryReader(s); data0 = r.ReadSingle(); data1 = r.ReadSingle(); data2 = r.ReadSingle(); data3 = r.ReadSingle(); }

        protected override void UnParse(Stream s) { BinaryWriter w = new BinaryWriter(s); w.Write(data0); w.Write(data1); w.Write(data2); w.Write(data3); }
        protected override DataType DataTypeFromType { get { return DataType.dtFloat; } }
        protected override int CountFromType { get { return 4; } }
        #endregion

        #region IEquatable<Entry> Members

        public override bool Equals(ShaderData other)
        {
            return base.Equals(other)
                && this.data0 == ((ElementFloat4)other).data0
                && this.data1 == ((ElementFloat4)other).data1
                && this.data2 == ((ElementFloat4)other).data2
                && this.data3 == ((ElementFloat4)other).data3
                ;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ data0.GetHashCode() ^ data1.GetHashCode() ^ data2.GetHashCode() ^ data3.GetHashCode();
        }

        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public Single Data0 { get { return data0; } set { if (data0 != value) { data0 = value; OnElementChanged(); } } }
        [ElementPriority(12)]
        public Single Data1 { get { return data1; } set { if (data1 != value) { data1 = value; OnElementChanged(); } } }
        [ElementPriority(13)]
        public Single Data2 { get { return data2; } set { if (data2 != value) { data2 = value; OnElementChanged(); } } }
        [ElementPriority(14)]
        public Single Data3 { get { return data3; } set { if (data3 != value) { data3 = value; OnElementChanged(); } } }
        #endregion
    }
    public class ElementInt : ShaderData
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        Int32 data;
        #endregion

        #region Constructors
        public ElementInt(int APIversion, EventHandler handler, FieldType field, Stream s) : base(APIversion, handler, field) { Parse(s); }
        public ElementInt(int APIversion, EventHandler handler) : this(APIversion, handler, (FieldType)0, (int)0) { }
        public ElementInt(int APIversion, EventHandler handler, ElementInt basis) : this(APIversion, handler, basis.field, basis.data) { }
        public ElementInt(int APIversion, EventHandler handler, FieldType field, Int32 data) : base(APIversion, handler, field) { this.data = data; }
        #endregion

        #region Data I/O
        void Parse(Stream s) { data = new BinaryReader(s).ReadInt32(); }

        protected override void UnParse(Stream s) { new BinaryWriter(s).Write(data); }
        protected override DataType DataTypeFromType { get { return DataType.dtInt; } }
        protected override int CountFromType { get { return 1; } }
        #endregion

        #region IEquatable<Entry> Members

        public override bool Equals(ShaderData other) { return base.Equals(other) && this.data == ((ElementInt)other).data; }
        public override int GetHashCode() { return base.GetHashCode() ^ data.GetHashCode(); }

        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public Int32 Data { get { return data; } set { if (data != value) { data = value; OnElementChanged(); } } }
        #endregion
    }
    public class ElementTextureRef : ShaderData
    {
        const int recommendedApiVersion = 1;
        public override string RCOLTag
        {
            get { return _RCOLTag; }
            set
            {
                if (_RCOLTag != value)
                {
                    _RCOLTag = value;
                    if (value == "GEOM")
                        using (MemoryStream ms = new MemoryStream())
                        {
                            data.UnParse(ms);
                            ms.Position = 0;
                            index = new BinaryReader(ms).ReadInt32();
                        }
                    else
                        data = new GenericRCOLResource.ChunkReference(0, handler, (uint)index);
                }
            }
        }

        #region Attributes
        int index; //GEOM
        GenericRCOLResource.ChunkReference data; //!GEOM
        TGIBlock key; //The Sims 4
        #endregion

        #region Constructors
        private ElementTextureRef(int APIversion, EventHandler handler, FieldType field, DependentList<TGIBlock> ParentTGIBlocks = null, string RCOLTag = "MATD")
            : base(APIversion, handler, field) { _ParentTGIBlocks = ParentTGIBlocks; _RCOLTag = RCOLTag; }

        public ElementTextureRef(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null, string RCOLTag = "MATD")
            : this(APIversion, handler, (FieldType)0, ParentTGIBlocks, RCOLTag) 
        {
            /*if (false) // TS3
            {
                if (_RCOLTag == "GEOM")
                    index = 0;
                else
                    data = new GenericRCOLResource.ChunkReference(0, handler);
            }
            else /* TS4 */
            {
                key = new TGIBlock(0, handler, "ITG");
            }
        }

        public ElementTextureRef(int APIversion, EventHandler handler, FieldType field, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null, string RCOLTag = "MATD")
            : this(APIversion, handler, field, ParentTGIBlocks, RCOLTag) { Parse(s); }

        public ElementTextureRef(int APIversion, EventHandler handler, ElementTextureRef basis, DependentList<TGIBlock> ParentTGIBlocks = null, string RCOLTag = null)
            : this(APIversion, handler, basis.field, ParentTGIBlocks ?? basis._ParentTGIBlocks, RCOLTag ?? basis._RCOLTag) 
        {
            /*if (false) // TS3
            {
                if (_RCOLTag == "GEOM")
                    index = basis.index;
                else
                    data = new GenericRCOLResource.ChunkReference(0, handler, basis.data);
            }
            else /* TS4 */
            {
                key = new TGIBlock(0, handler, basis.key);
            }
        }

        public ElementTextureRef(int APIversion, EventHandler handler, FieldType field, Int32 index, DependentList<TGIBlock> ParentTGIBlocks)
            : this(APIversion, handler, field, ParentTGIBlocks, "GEOM") { this.index = index; }
        public ElementTextureRef(int APIversion, EventHandler handler, FieldType field, GenericRCOLResource.ChunkReference data)
            : this(APIversion, handler, field, null, "MATD") { this.data = new GenericRCOLResource.ChunkReference(0, handler, data); }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            /*if (false) // TS3
            {
                if (_RCOLTag == "GEOM")
                    index = new BinaryReader(s).ReadInt32();
                else
                    data = new GenericRCOLResource.ChunkReference(0, handler, s);
                ReadZeros(s, 12);
            }
            else /* TS4 */
            {
                key = new TGIBlock(requestedApiVersion, handler, "ITG", s);
            }
        }
        protected override void UnParse(Stream s)
        {
            /*if (false) // TS3
            {
                if (_RCOLTag == "GEOM")
                    new BinaryWriter(s).Write(index);
                else
                    data.UnParse(s);
                WriteZeros(s, 12);
            }
            else /* TS4 */
            {
                if (key == null) 
                    key = new TGIBlock(requestedApiVersion, handler, "ITG"); 
                key.UnParse(s);
            }
        }
        protected override DataType DataTypeFromType { get { return DataType.dtTexture; } }
        protected override int CountFromType { get { return 4; } }
        #endregion

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                /*if (false) // TS3
                {
                    if (_RCOLTag == "GEOM") res.Remove("Data");
                    else res.Remove("Index");
                    res.Remove("Key");
                }
                else /* TS4 */
                {
                    res.Remove("Data");
                    res.Remove("Index");
                }
                return res;
            }
        }

        #region IEquatable<Entry> Members

        public override bool Equals(ShaderData other)
        {
            if (!base.Equals(other))
                return false;

            ElementTextureRef _other = (ElementTextureRef)other;
            if (_RCOLTag != _other._RCOLTag)
                return false;

            /*if (false) // TS3
            {
                if (_RCOLTag == "GEOM")
                    return index.Equals(_other.index);
                else
                    return data.Equals(_other.data);
            }
            else /* TS4 */
            {
                return key.Equals(_other.key);
            }
        }
        public override int GetHashCode()
        {
            /*if (false) // TS3
            {
                if (_RCOLTag == "GEOM")
                    return base.GetHashCode() ^ index.GetHashCode();
                else
                    return base.GetHashCode() ^ data.GetHashCode();
            }
            else /* TS4 */
            {
                return base.GetHashCode() ^ key.GetHashCode();
            }
        }

        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public GenericRCOLResource.ChunkReference Data
        {
            get { if (_RCOLTag == "GEOM") throw new InvalidOperationException("Use Index not Data for GEOM"); return data; }
            set { if (_RCOLTag == "GEOM") throw new InvalidOperationException("Use Index not Data for GEOM"); if (data != value) { data = new GenericRCOLResource.ChunkReference(0, handler, value); OnElementChanged(); } }
        }
        [ElementPriority(11), TGIBlockListContentField("ParentTGIBlocks")]
        public Int32 Index
        {
            get { if (_RCOLTag != "GEOM") throw new InvalidOperationException("Use Data not Index except for GEOM"); return index; }
            set { if (_RCOLTag != "GEOM") throw new InvalidOperationException("Use Data not Index except for GEOM"); if (index != value) { index = value; OnElementChanged(); } }
        }
        [ElementPriority(11)]
        public IResourceKey Key 
        { 
            get { return key; } 
            set { if (!key.Equals(value)) { key = new TGIBlock(requestedApiVersion, handler, "ITG", value); OnElementChanged(); } } 
        }
        #endregion
    }
    public class ElementTextureKey : ShaderData
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        TGIBlock data;
        #endregion

        #region Constructors
        public ElementTextureKey(int APIversion, EventHandler handler, FieldType field, Stream s) : base(APIversion, handler, field) { Parse(s); }
        public ElementTextureKey(int APIversion, EventHandler handler) : this(APIversion, handler, (FieldType)0, (uint)0, (uint)0, (ulong)0) { }
        public ElementTextureKey(int APIversion, EventHandler handler, ElementTextureKey basis) : this(APIversion, handler, basis.field, basis.data) { }
        public ElementTextureKey(int APIversion, EventHandler handler, FieldType field, IResourceKey data) : base(APIversion, handler, field) { this.data = new TGIBlock(requestedApiVersion, handler, "ITG", data); }
        public ElementTextureKey(int APIversion, EventHandler handler, FieldType field, uint resourceType, uint resourceGroup, ulong instance) : base(APIversion, handler, field) { this.data = new TGIBlock(requestedApiVersion, handler, "ITG", resourceType, resourceGroup, instance); }
        #endregion

        #region Data I/O
        void Parse(Stream s) { data = new TGIBlock(requestedApiVersion, handler, "ITG", s); ReadZeros(s, 4); }

        protected override void UnParse(Stream s) { if (data == null) data = new TGIBlock(requestedApiVersion, handler, "ITG"); data.UnParse(s); WriteZeros(s, 4); }
        protected override DataType DataTypeFromType { get { return DataType.dtTexture; } }
        protected override int CountFromType { get { return 5; } }
        #endregion

        #region IEquatable<Entry> Members

        public override bool Equals(ShaderData other) { return base.Equals(other) && this.data == ((ElementTextureKey)other).data; }
        public override int GetHashCode() { return base.GetHashCode() ^ data.GetHashCode(); }

        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public IResourceKey Data { get { return data; } set { if (!data.Equals(value)) { data = new TGIBlock(requestedApiVersion, handler, "ITG", value); OnElementChanged(); } } }
        #endregion
    }
    public class ElementImageMapKey : ShaderData
    {
        const int recommendedApiVersion = 1;

        #region Attributes
        TGIBlock data;
        #endregion

        #region Constructors
        public ElementImageMapKey(int APIversion, EventHandler handler, FieldType field, Stream s) : base(APIversion, handler, field) { Parse(s); }
        public ElementImageMapKey(int APIversion, EventHandler handler) : this(APIversion, handler, (FieldType)0, (uint)0, (uint)0, (ulong)0) { }
        public ElementImageMapKey(int APIversion, EventHandler handler, ElementImageMapKey basis) : this(APIversion, handler, basis.field, basis.data) { }
        public ElementImageMapKey(int APIversion, EventHandler handler, FieldType field, IResourceKey data) : base(APIversion, handler, field) { this.data = new TGIBlock(requestedApiVersion, handler, "ITG", data); }
        public ElementImageMapKey(int APIversion, EventHandler handler, FieldType field, uint resourceType, uint resourceGroup, ulong instance) : base(APIversion, handler, field) { this.data = new TGIBlock(requestedApiVersion, handler, "ITG", resourceType, resourceGroup, instance); }
        #endregion

        #region Data I/O
        void Parse(Stream s) { data = new TGIBlock(requestedApiVersion, handler, "ITG", s); }

        protected override void UnParse(Stream s) { if (data == null) data = new TGIBlock(requestedApiVersion, handler, "ITG"); data.UnParse(s); }
        protected override DataType DataTypeFromType { get { return DataType.dtImageMap; } }
        protected override int CountFromType { get { return 4; } }
        #endregion

        #region IEquatable<Entry> Members

        public override bool Equals(ShaderData other) { return base.Equals(other) && this.data == ((ElementImageMapKey)other).data; }
        public override int GetHashCode() { return base.GetHashCode() ^ data.GetHashCode(); }

        #endregion

        #region Content Fields
        [ElementPriority(11)]
        public IResourceKey Data { get { return data; } set { if (!data.Equals(value)) { data = new TGIBlock(requestedApiVersion, handler, "ITG", value); OnElementChanged(); } } }
        #endregion
    }


    public class ShaderDataList : DependentList<ShaderData>
    {
        private DependentList<TGIBlock> _ParentTGIBlocks;
        public DependentList<TGIBlock> ParentTGIBlocks
        {
            get { return _ParentTGIBlocks; }
            set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (var i in this) if (i is ElementTextureRef) i.ParentTGIBlocks = _ParentTGIBlocks; } }
        }

        protected string _RCOLTag;
        public string RCOLTag
        {
            get { return _RCOLTag; }
            set { if (_RCOLTag != value) { _RCOLTag = value; foreach (var i in this) if (i is ElementTextureRef) i.RCOLTag = _RCOLTag; } }
        }

        static bool checking = s4pi.Settings.Settings.Checking;

        internal long dataPos = -1;

        #region Constructors
        public ShaderDataList(EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null, string RCOLTag = "MATD")
            : base(handler) { _ParentTGIBlocks = ParentTGIBlocks; _RCOLTag = RCOLTag; }
        public ShaderDataList(EventHandler handler, Stream s, long start, Nullable<int> expectedDataLen, DependentList<TGIBlock> ParentTGIBlocks = null, string RCOLTag = "MATD")
            : base(null)
        {
            elementHandler = handler;
            this._ParentTGIBlocks = ParentTGIBlocks;
            this._RCOLTag = RCOLTag;
            Parse(s, start, expectedDataLen);
            this.handler = handler;
        }
        public ShaderDataList(EventHandler handler, IEnumerable<ShaderData> ilt, DependentList<TGIBlock> ParentTGIBlocks = null, string RCOLTag = "MATD")
            : base(handler)
        {
            elementHandler = handler;
            this._ParentTGIBlocks = ParentTGIBlocks;
            this._RCOLTag = RCOLTag;
            foreach (var t in ilt) this.Add(t);
            this.handler = handler;
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s) { throw new NotSupportedException(); }
        internal void Parse(Stream s, long start, Nullable<int> expectedDataLen)
        {
            int dataLen = 0;
            for (int i = ReadCount(s); i > 0; i--)
            {
                ShaderData entry = ShaderData.CreateEntry(0, elementHandler, s, start, _ParentTGIBlocks, _RCOLTag);
                this.Add(entry);
                dataLen += this[Count - 1].ByteCount() * 4;
            }
            if (checking) if (expectedDataLen != null && expectedDataLen != dataLen)
                    throw new InvalidDataException(string.Format("Expected 0x{0:X8} bytes of data, read 0x{1:X8} at 0x{2:X8}", expectedDataLen, dataLen, s.Position));
            s.Position += dataLen;
        }
        public override void UnParse(Stream s) { throw new NotSupportedException(); }
        internal void UnParse(Stream s, long start)
        {
            WriteCount(s, Count);
            foreach (var element in this) element.UnParseHeader(s);
            dataPos = s.Position;
            foreach (var element in this) element.UnParseData(s, start);
        }

        protected override ShaderData CreateElement(Stream s) { throw new NotImplementedException(); }
        protected override void WriteElement(Stream s, ShaderData element) { throw new NotImplementedException(); }
        #endregion

        public override void Add(ShaderData item)
        {
            if (item is ElementTextureRef) { var element = item as ElementTextureRef; element.ParentTGIBlocks = _ParentTGIBlocks; element.RCOLTag = _RCOLTag; }
            base.Add(item);
        }
        public override void Add(Type elementType)
        {
            if (elementType.IsAbstract)
                throw new ArgumentException("Must pass a concrete element type.", "elementType");

            if (!typeof(ShaderData).IsAssignableFrom(elementType))
                throw new ArgumentException("The element type must belong to the generic type of the list.", "elementType");

            ShaderData newElement;
            if (typeof(ElementTextureRef).IsAssignableFrom(elementType))
                newElement = Activator.CreateInstance(elementType, new object[] { (int)0, elementHandler, _ParentTGIBlocks, _RCOLTag, }) as ElementTextureRef;
            else
                newElement = Activator.CreateInstance(elementType, new object[] { (int)0, elementHandler, }) as ShaderData;
            base.Add(newElement);
        }
    }
}