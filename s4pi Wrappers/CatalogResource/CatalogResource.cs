using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using s4pi.Interfaces;

namespace CatalogResource
{
    /// <summary>
    /// A resource wrapper that understands Catalog Entry resources
    /// </summary>
    public abstract class CatalogResource : AResource
    {
        protected const Int32 recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        protected static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        protected uint version;
        protected Common common = null;
        #endregion

        #region Constructors
        protected CatalogResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = this.UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; this.Parse(stream); }
        protected CatalogResource(int APIversion, uint version, Common common) : base(APIversion, null) { this.version = version; this.common = new Common(requestedApiVersion, OnResourceChanged, common); }
        #endregion

        #region Data I/O
        protected virtual void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            version = r.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            MemoryStream s = new MemoryStream();
            new BinaryWriter(s).Write(version);
            return s;
        }
        #endregion

        #region Sub-classes
        public class Common : AHandlerElement
        {
            #region Attributes
            uint version;
            ulong nameGUID;
            ulong descGUID;
            string name = "";
            string desc = "";
            float price;
            float nicenessMultiplier;
            float crapScore;
            BuildBuyProductStatus buildBuyProductStatusFlags;
            ulong pngInstance;
            byte unknown7;
            float environmentScore;
            Fire fireType;
            byte isStealable;
            byte isReposessable;
            uint uiSortPriority;
            byte isPlaceableOnRoof;//>=0x0D
            byte isVisibleInWorldBuilder;//>=0x0E
            uint productNameHash;//>=0x0F
            #endregion

            #region Constructors
            internal Common(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }

            public Common(int APIversion, EventHandler handler, Common basis)
                : this(APIversion, handler,
                basis.version, basis.nameGUID, basis.descGUID, basis.name, basis.desc, basis.price,
                basis.nicenessMultiplier, basis.crapScore, basis.buildBuyProductStatusFlags, basis.pngInstance,
                basis.unknown7, basis.environmentScore, basis.fireType, basis.isStealable, basis.isReposessable, basis.uiSortPriority,
                basis.isPlaceableOnRoof,
                basis.isVisibleInWorldBuilder,
                basis.productNameHash
                ) { }

            public Common(int APIversion, EventHandler handler) : base(APIversion, handler) { }

            public Common(int APIversion, EventHandler handler,
                uint version, ulong nameGUID, ulong descGUID, string name, string desc, float price,
                float nicenessMultiplier, float crapScore, BuildBuyProductStatus buildBuyProductStatusFlags, ulong pngInstance,
                byte unknown7, float environmentScore, Fire fireType, byte isStealable, byte isReposessable, uint uiSortPriority
                )
                : this(APIversion, handler,
                version, nameGUID, descGUID, name, desc, price,
                nicenessMultiplier, crapScore, buildBuyProductStatusFlags, pngInstance,
                unknown7, environmentScore, fireType, isStealable, isReposessable, uiSortPriority,
                0,
                0,
                0
                )
            {
                if (version >= 0x0000000D)
                    throw new InvalidOperationException(String.Format("Constructor requires isPlaceableOnRoof for version {0}", version));
            }
            public Common(int APIversion, EventHandler handler, uint version, ulong nameGUID, ulong descGUID, string name, string desc, float price,
                float nicenessMultiplier, float crapScore, BuildBuyProductStatus buildBuyProductStatusFlags, ulong pngInstance,
                byte unknown7, float environmentScore, Fire fireType, byte isStealable, byte isReposessable, uint uiSortPriority,
                byte isPlaceableOnRoof//>=0x0D
                )
                : this(APIversion, handler,
                version, nameGUID, descGUID, name, desc, price,
                nicenessMultiplier, crapScore, buildBuyProductStatusFlags, pngInstance,
                unknown7, environmentScore, fireType, isStealable, isReposessable, uiSortPriority,
                isPlaceableOnRoof,
                0,
                0
                )
            {
                if (version >= 0x0000000E)
                    throw new InvalidOperationException(String.Format("Constructor requires isVisibleInWorldBuilder for version {0}", version));
            }
            public Common(int APIversion, EventHandler handler,
                uint version, ulong nameGUID, ulong descGUID, string name, string desc, float price,
                float nicenessMultiplier, float crapScore, BuildBuyProductStatus buildBuyProductStatusFlags, ulong pngInstance,
                byte unknown7, float environmentScore, Fire fireType, byte isStealable, byte isReposessable, uint uiSortPriority,
                byte isPlaceableOnRoof,//>=0x0D
                byte isVisibleInWorldBuilder//>=0x0E
                )
                : this(APIversion, handler,
                version, nameGUID, descGUID, name, desc, price,
                nicenessMultiplier, crapScore, buildBuyProductStatusFlags, pngInstance,
                unknown7, environmentScore, fireType, isStealable, isReposessable, uiSortPriority,
                isPlaceableOnRoof,
                isVisibleInWorldBuilder,
                0
                )
            {
                if (version >= 0x0000000F)
                    throw new InvalidOperationException(String.Format("Constructor requires productNameHash for version {0}", version));
            }
            public Common(int APIversion, EventHandler handler,
                uint version, ulong nameGUID, ulong descGUID, string name, string desc, float price,
                float nicenessMultiplier, float crapScore, BuildBuyProductStatus buildBuyProductStatusFlags, ulong pngInstance,
                byte unknown7, float environmentScore, Fire fireType, byte isStealable, byte isReposessable, uint uiSortPriority,
                byte isPlaceableOnRoof,//>=0x0D
                byte isVisibleInWorldBuilder,//>=0x0E
                uint productNameHash//>=0x0F
                )
                : base(APIversion, handler)
            {
                this.version = version;
                this.nameGUID = nameGUID;
                this.descGUID = descGUID;
                this.name = name;
                this.desc = desc;
                this.price = price;

                this.nicenessMultiplier = nicenessMultiplier;
                this.crapScore = crapScore;
                this.buildBuyProductStatusFlags = buildBuyProductStatusFlags;
                this.pngInstance = pngInstance;

                this.unknown7 = unknown7;
                this.environmentScore = environmentScore;
                this.fireType = fireType;
                this.isStealable = isStealable;
                this.isReposessable = isReposessable;
                this.uiSortPriority = uiSortPriority;

                this.isPlaceableOnRoof = isPlaceableOnRoof;
                this.isVisibleInWorldBuilder = isVisibleInWorldBuilder;
                this.productNameHash = productNameHash;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);

                version = r.ReadUInt32();
                nameGUID = r.ReadUInt64();
                descGUID = r.ReadUInt64();
                name = BigEndianUnicodeString.Read(s);
                desc = BigEndianUnicodeString.Read(s);
                price = r.ReadSingle();

                nicenessMultiplier = r.ReadSingle();
                crapScore = r.ReadSingle();
                buildBuyProductStatusFlags = (BuildBuyProductStatus)r.ReadByte();
                pngInstance = r.ReadUInt64();

                unknown7 = r.ReadByte();
                environmentScore = r.ReadSingle();
                fireType = (Fire)r.ReadUInt32();
                isStealable = r.ReadByte();
                isReposessable = r.ReadByte();
                uiSortPriority = r.ReadUInt32();

                if (version >= 0x0000000D)
                {
                    isPlaceableOnRoof = r.ReadByte();
                    if (version >= 0x0000000E)
                    {
                        isVisibleInWorldBuilder = r.ReadByte();
                        if (version >= 0x0000000F)
                        {
                            productNameHash = r.ReadUInt32();
                        }
                    }
                }
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(version);
                w.Write(nameGUID);
                w.Write(descGUID);
                BigEndianUnicodeString.Write(s, name);
                BigEndianUnicodeString.Write(s, desc);
                w.Write(price);
                w.Write(nicenessMultiplier);
                w.Write(crapScore);
                w.Write((byte)buildBuyProductStatusFlags);
                w.Write(pngInstance);

                w.Write(unknown7);
                w.Write(environmentScore);
                w.Write((uint)fireType);
                w.Write(isStealable);
                w.Write(isReposessable);
                w.Write(uiSortPriority);

                if (version >= 0x0000000D)
                {
                    w.Write(isPlaceableOnRoof);
                    if (version >= 0x0000000E)
                    {
                        w.Write(isVisibleInWorldBuilder);
                        if (version >= 0x0000000F)
                        {
                            w.Write(productNameHash);
                        }
                    }
                }
            }
            #endregion

            #region AHandlerElement
            public override List<string> ContentFields
            {
                get
                {
                    List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                    if (version < 0x0000000F)
                    {
                        res.Remove("ProductNameHash");
                        if (version < 0x0000000E)
                        {
                            res.Remove("IsVisibleInWorldBuilder");
                            if (this.version < 0x0000000D)
                                res.Remove("IsPlaceableOnRoof");
                        }
                    }
                    return res;
                }
            }
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            #endregion

            #region Sub-classes
            public enum Fire : uint
            {
                DoesNotBurn = 0,
                Chars,
                AshPile
            }

            [Flags]
            public enum BuildBuyProductStatus : byte
            {
                ShowInCatalog = 0x01,
                ProductForTesting = 0x02,
                ProductInDevelopment = 0x04,
                ShippingProduct = 0x08,

                DebugProduct = 0x10,
                ProductionProduct = 0x20,
                ObjProductMadeUsingNewEntryScheme = 0x40,
                //
            }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public uint Version { get { return version; } set { if (version != value) { version = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public ulong NameGUID { get { return nameGUID; } set { if (nameGUID != value) { nameGUID = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public ulong DescGUID { get { return descGUID; } set { if (descGUID != value) { descGUID = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public string Name { get { return name; } set { if (name != value) { name = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public string Desc { get { return desc; } set { if (desc != value) { desc = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public float Price { get { return price; } set { if (price != value) { price = value; OnElementChanged(); } } }
            [ElementPriority(7)]
            public float NicenessMultiplier { get { return nicenessMultiplier; } set { if (nicenessMultiplier != value) { nicenessMultiplier = value; OnElementChanged(); } } }
            [ElementPriority(8)]
            public float CrapScore { get { return crapScore; } set { if (crapScore != value) { crapScore = value; OnElementChanged(); } } }
            [ElementPriority(9)]
            public BuildBuyProductStatus BuildBuyProductStatusFlags { get { return buildBuyProductStatusFlags; } set { if (buildBuyProductStatusFlags != value) { buildBuyProductStatusFlags = value; OnElementChanged(); } } }
            [ElementPriority(10)]
            public ulong PngInstance { get { return pngInstance; } set { if (pngInstance != value) { pngInstance = value; OnElementChanged(); } } }
            [ElementPriority(11)]
            public byte Unknown7 { get { return unknown7; } set { if (unknown7 != value) { unknown7 = value; OnElementChanged(); } } }
            [ElementPriority(12)]
            public float EnvironmentScore { get { return environmentScore; } set { if (environmentScore != value) { environmentScore = value; OnElementChanged(); } } }
            [ElementPriority(13)]
            public Fire FireType { get { return fireType; } set { if (fireType != value) { fireType = value; OnElementChanged(); } } }
            [ElementPriority(14)]
            public bool IsStealable { get { return isStealable != 0; } set { if (IsStealable != value) { isStealable = (byte)(value ? 1 : 0); OnElementChanged(); } } }
            [ElementPriority(15)]
            public bool IsReposessable { get { return isReposessable != 0; } set { if (IsReposessable != value) { isReposessable = (byte)(value ? 1 : 0); OnElementChanged(); } } }
            [ElementPriority(16)]
            public uint UISortPriority { get { return uiSortPriority; } set { if (uiSortPriority != value) { uiSortPriority = value; OnElementChanged(); } } }
            [ElementPriority(17)]
            public bool IsPlaceableOnRoof
            {
                get { if (version < 0x0000000D) throw new InvalidOperationException(); return isPlaceableOnRoof != 0; }
                set { if (version < 0x0000000D) throw new InvalidOperationException(); if (IsPlaceableOnRoof != value) { isPlaceableOnRoof = (byte)(value ? 1 : 0); OnElementChanged(); } }
            }
            [ElementPriority(18)]
            public bool IsVisibleInWorldBuilder
            {
                get { if (version < 0x0000000E) throw new InvalidOperationException(); return isVisibleInWorldBuilder != 0; }
                set { if (version < 0x0000000E) throw new InvalidOperationException(); if (IsVisibleInWorldBuilder != value) { isVisibleInWorldBuilder = (byte)(value ? 1 : 0); OnElementChanged(); } }
            }
            [ElementPriority(19)]
            public uint ProductNameHash
            {
                get { if (version < 0x0000000F) throw new InvalidOperationException(); return productNameHash; }
                set { if (version < 0x0000000F) throw new InvalidOperationException(); if (productNameHash != value) { productNameHash = value; OnElementChanged(); } }
            }

            public String Value { get { return ValueBuilder; } }
            #endregion
        }

        #region ComplateElement
        public abstract class ComplateElement : AHandlerElement,
            IComparable<ComplateElement>, IEqualityComparer<ComplateElement>, IEquatable<ComplateElement>
        {
            #region Attributes
            protected string variableName;
            #endregion

            #region Constructors
            protected ComplateElement(int APIversion, EventHandler handler, string variableName) : base(APIversion, handler) { this.variableName = variableName; }
            protected ComplateElement(int APIversion, EventHandler handler, string variableName, Stream s) : this(APIversion, handler, variableName) { Parse(s); }

            public static ComplateElement CreateTypeCode(int APIversion, EventHandler handler, string variableName, byte typeCode, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
            {
                switch (typeCode)
                {
                    case 0x01: return new TC01_String(APIversion, handler, variableName, s);
                    case 0x02: return new TC02_ARGB(APIversion, handler, variableName, s);
                    case 0x03: return new TC03_TGIIndex(APIversion, handler, variableName, s, ParentTGIBlocks);
                    case 0x04: return new TC04_Single(APIversion, handler, variableName, s);
                    case 0x05: return new TC05_XY(APIversion, handler, variableName, s);
                    case 0x06: return new TC06_XYZ(APIversion, handler, variableName, s);
                    case 0x07: return new TC07_Boolean(APIversion, handler, variableName, s);
                }
                throw new InvalidDataException(String.Format("Unknown TypeCode 0x{0:X2} at 0x{1:X8}", typeCode, s.Position));
            }
            #endregion

            #region Data I/O
            protected abstract void Parse(Stream s);
            internal abstract void UnParse(Stream s);
            internal void UnParse(Stream s, byte typeCode) { ComplateString.Write(s, variableName); new BinaryWriter(s).Write(typeCode); }
            #endregion

            #region IComparable<TypeCode> Members

            public virtual int CompareTo(ComplateElement other) { int res = this.variableName.CompareTo(other.variableName); return res != 0 ? res : this.GetType().Name.CompareTo(other.GetType().Name); }

            #endregion

            #region IEqualityComparer<TypeCode> Members

            public virtual bool Equals(ComplateElement x, ComplateElement y) { if (x.GetType() != y.GetType()) return false; return x.CompareTo(y) == 0; }

            public virtual int GetHashCode(ComplateElement obj) { return ((Object)obj).GetHashCode(); }

            public override int GetHashCode() { return GetHashCode(this); }

            #endregion

            #region IEquatable<ComplateElement> Members

            public bool Equals(ComplateElement other) { return Equals(this, other); }

            public override bool Equals(object obj)
            {
                return obj as ComplateElement != null ? this.Equals((ComplateElement)obj) : false;
            }

            #endregion

            #region AApiVersionedFields
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region ContentFields
            [ElementPriority(0)]
            public string VariableName { get { return variableName; } set { if (variableName != value) { variableName = value; handler(this, EventArgs.Empty); } } }

            public virtual String Value
            {
                get
                {
                    return string.Join("; ", ValueBuilder.Split('\n')).Replace("VariableName: ", "");
                }
            }
            #endregion
        }

        public class TC01_String : ComplateElement
        {
            #region Attributes
            string stringValue;
            #endregion

            #region Constructors
            internal TC01_String(int APIversion, EventHandler handler, string variableName, Stream s) : base(APIversion, handler, variableName, s) { }
            public TC01_String(int APIversion, EventHandler handler) : this(APIversion, handler, 0, "StringVariable", "Value") { }
            public TC01_String(int APIversion, EventHandler handler, TC01_String basis) : this(APIversion, handler, 0, basis.variableName, basis.stringValue) { }
            public TC01_String(int APIversion, EventHandler handler, byte unused, string variableName, string stringValue) : base(APIversion, handler, variableName)
            {
                this.stringValue = stringValue;
            }
            #endregion

            #region Data I/O
            protected override void Parse(Stream s) { stringValue = ComplateString.Read(s); }

            internal override void UnParse(Stream s) { base.UnParse(s, 0x01); ComplateString.Write(s, stringValue); }
            #endregion

            public override int CompareTo(ComplateElement other)
            {
                int res = base.CompareTo(other);
                return res != 0 ? res : stringValue.CompareTo((other as TC01_String).stringValue);
            }

            public override int GetHashCode(ComplateElement obj)
            {
                TC01_String tc = obj as TC01_String;
                if (tc == null) return base.GetHashCode(obj);
                return tc.variableName.GetHashCode() ^ tc.stringValue.GetHashCode();
            }

            public string Data { get { return stringValue; } set { if (stringValue != value) stringValue = value; OnElementChanged(); } }
        }

        public class TC02_ARGB : ComplateElement
        {
            #region Attributes
            UInt32 argb;
            #endregion

            #region Constructors
            internal TC02_ARGB(int APIversion, EventHandler handler, string variableName, Stream s) : base(APIversion, handler, variableName, s) { }
            public TC02_ARGB(int APIversion, EventHandler handler) : this(APIversion, handler, 0, "ARGBVariable", (UInt32)0xFFAA8866) { }
            public TC02_ARGB(int APIversion, EventHandler handler, TC02_ARGB basis) : this(APIversion, handler, 0, basis.variableName, basis.argb) { }
            public TC02_ARGB(int APIversion, EventHandler handler, byte unused, string variableName, UInt32 argb) : base(APIversion, handler, variableName)
            {
                this.argb = argb;
            }
            #endregion

            #region Data I/O
            protected override void Parse(Stream s) { argb = new BinaryReader(s).ReadUInt32(); }

            internal override void UnParse(Stream s) { base.UnParse(s, 0x02); new BinaryWriter(s).Write(argb); }
            #endregion

            public override int CompareTo(ComplateElement other)
            {
                int res = base.CompareTo(other);
                return (res != 0) ? res : argb.CompareTo((other as TC02_ARGB).argb);
            }

            public override int GetHashCode(ComplateElement obj)
            {
                TC02_ARGB tc = obj as TC02_ARGB;
                if (tc == null) return base.GetHashCode(obj);
                return tc.variableName.GetHashCode() ^ tc.argb.GetHashCode();
            }

            [ElementPriority(1)]
            public UInt32 ARGB { get { return argb; } set { if (argb != value) { argb = value; OnElementChanged(); } } }
        }

        public class TC03_TGIIndex : ComplateElement
        {
            public DependentList<TGIBlock> ParentTGIBlocks { get; set; }
            public override List<string> ContentFields { get { List<string> res = base.ContentFields; res.Remove("ParentTGIBlocks"); return res; } }

            #region Attributes
            byte tgiIndex;
            #endregion

            #region Constructors
            internal TC03_TGIIndex(int APIversion, EventHandler handler, string variableName, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler, variableName, s) { this.ParentTGIBlocks = ParentTGIBlocks; }

            public TC03_TGIIndex(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, 0, "TGIIndexVariable", (byte)0x00, ParentTGIBlocks) { }
            public TC03_TGIIndex(int APIversion, EventHandler handler, TC03_TGIIndex basis, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, 0, basis.variableName, basis.tgiIndex, ParentTGIBlocks ?? basis.ParentTGIBlocks) { }
            public TC03_TGIIndex(int APIversion, EventHandler handler, byte unused, string variableName, byte tgiIndex, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler, variableName) { this.ParentTGIBlocks = ParentTGIBlocks; this.tgiIndex = tgiIndex; }
            #endregion

            #region Data I/O
            protected override void Parse(Stream s) { tgiIndex = new BinaryReader(s).ReadByte(); }

            internal override void UnParse(Stream s) { base.UnParse(s, 0x03); new BinaryWriter(s).Write(tgiIndex); }
            #endregion

            public override int CompareTo(ComplateElement other)
            {
                int res = base.CompareTo(other);
                return res != 0 ? res : tgiIndex.CompareTo((other as TC03_TGIIndex).tgiIndex);
            }

            public override int GetHashCode(ComplateElement obj)
            {
                TC03_TGIIndex tc = obj as TC03_TGIIndex;
                if (tc == null) return base.GetHashCode(obj);
                return tc.variableName.GetHashCode() ^ tc.tgiIndex.GetHashCode();
            }

            [TGIBlockListContentField("ParentTGIBlocks")]
            public byte TGIIndex { get { return tgiIndex; } set { if (tgiIndex != value) { tgiIndex = value; OnElementChanged(); } } }
        }

        public class TC04_Single : ComplateElement
        {
            #region Attributes
            float unknown1;
            #endregion

            #region Constructors
            internal TC04_Single(int APIversion, EventHandler handler, string variableName, Stream s) : base(APIversion, handler, variableName, s) { }
            public TC04_Single(int APIversion, EventHandler handler) : this(APIversion, handler, 0, "FloatVariable", 0f) { }
            public TC04_Single(int APIversion, EventHandler handler, TC04_Single basis) : this(APIversion, handler, 0, basis.variableName, basis.unknown1) { }
            public TC04_Single(int APIversion, EventHandler handler, byte unused, string variableName, float unknown1) : base(APIversion, handler, variableName)
            {
                this.unknown1 = unknown1;
            }
            #endregion

            #region Data I/O
            protected override void Parse(Stream s) { unknown1 = (new BinaryReader(s)).ReadSingle(); }

            internal override void UnParse(Stream s) { base.UnParse(s, 0x04); new BinaryWriter(s).Write(unknown1); }
            #endregion

            public override int CompareTo(ComplateElement other)
            {
                int res = base.CompareTo(other);
                return res != 0 ? res : unknown1.CompareTo((other as TC04_Single).unknown1);
            }

            public override int GetHashCode(ComplateElement obj)
            {
                TC04_Single tc = obj as TC04_Single;
                if (tc == null) return base.GetHashCode(obj);
                return tc.variableName.GetHashCode() ^ tc.unknown1.GetHashCode();
            }

            public float Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnElementChanged(); } } }
        }

        public class TC05_XY : ComplateElement
        {
            #region Attributes
            float unknown1;
            float unknown2;
            #endregion

            #region Constructors
            internal TC05_XY(int APIversion, EventHandler handler, string variableName, Stream s) : base(APIversion, handler, variableName, s) { }
            public TC05_XY(int APIversion, EventHandler handler) : this(APIversion, handler, 0, "XYVariable", 0f, 0f) { }
            public TC05_XY(int APIversion, EventHandler handler, TC05_XY basis) : this(APIversion, handler, 0, basis.variableName, basis.unknown1, basis.unknown2) { }
            public TC05_XY(int APIversion, EventHandler handler, byte unused, string variableName, float unknown1, float unknown2) : base(APIversion, handler, variableName)
            {
                this.unknown1 = unknown1;
                this.unknown2 = unknown2;
            }
            #endregion

            #region Data I/O
            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                unknown1 = r.ReadSingle();
                unknown2 = r.ReadSingle();
            }

            internal override void UnParse(Stream s)
            {
                base.UnParse(s, 0x05);
                BinaryWriter w = new BinaryWriter(s);
                w.Write(unknown1);
                w.Write(unknown2);
            }
            #endregion

            public override int CompareTo(ComplateElement other)
            {
                int res = base.CompareTo(other); if (res != 0) return res;
                TC05_XY tc = other as TC05_XY;
                res = unknown1.CompareTo(tc.unknown1); if (res != 0) return res;
                return unknown2.CompareTo(tc.unknown2);
            }

            public override int GetHashCode(ComplateElement obj)
            {
                TC05_XY tc = obj as TC05_XY;
                if (tc == null) return base.GetHashCode(obj);
                return tc.variableName.GetHashCode() ^ tc.unknown1.GetHashCode() ^ tc.unknown2.GetHashCode();
            }

            public float Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnElementChanged(); } } }
            public float Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnElementChanged(); } } }
        }

        public class TC06_XYZ : ComplateElement
        {
            #region Attributes
            float unknown1;
            float unknown2;
            float unknown3;
            #endregion

            #region Constructors
            internal TC06_XYZ(int APIversion, EventHandler handler, string variableName, Stream s) : base(APIversion, handler, variableName, s) { }
            public TC06_XYZ(int APIversion, EventHandler handler) : this(APIversion, handler, 0, "XYZVariable", 0f, 0f, 0f) { }
            public TC06_XYZ(int APIversion, EventHandler handler, TC06_XYZ basis) : this(APIversion, handler, 0, basis.variableName, basis.unknown1, basis.unknown2, basis.unknown3) { }
            public TC06_XYZ(int APIversion, EventHandler handler, byte unused, string variableName, float unknown1, float unknown2, float unknown3) : base(APIversion, handler, variableName)
            {
                this.unknown1 = unknown1;
                this.unknown2 = unknown2;
                this.unknown3 = unknown3;
            }
            #endregion

            #region Data I/O
            protected override void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                unknown1 = r.ReadSingle();
                unknown2 = r.ReadSingle();
                unknown3 = r.ReadSingle();
            }

            internal override void UnParse(Stream s)
            {
                base.UnParse(s, 0x06);
                BinaryWriter w = new BinaryWriter(s);
                w.Write(unknown1);
                w.Write(unknown2);
                w.Write(unknown3);
            }
            #endregion

            public override int CompareTo(ComplateElement other)
            {
                int res = base.CompareTo(other); if (res != 0) return res;
                TC06_XYZ tc = other as TC06_XYZ;
                res = unknown1.CompareTo(tc.unknown1); if (res != 0) return res;
                res = unknown2.CompareTo(tc.unknown2); if (res != 0) return res;
                return unknown3.CompareTo(tc.unknown3);
            }

            public override int GetHashCode(ComplateElement obj)
            {
                TC06_XYZ tc = obj as TC06_XYZ;
                if (tc == null) return base.GetHashCode(obj);
                return tc.variableName.GetHashCode() ^ tc.unknown1.GetHashCode() ^ tc.unknown2.GetHashCode() ^ tc.unknown3.GetHashCode();
            }

            public float Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnElementChanged(); } } }
            public float Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnElementChanged(); } } }
            public float Unknown3 { get { return unknown3; } set { if (unknown3 != value) { unknown3 = value; OnElementChanged(); } } }
        }

        public class TC07_Boolean : ComplateElement
        {
            #region Attributes
            byte unknown1;
            #endregion

            #region Constructors
            internal TC07_Boolean(int APIversion, EventHandler handler, string variableName, Stream s) : base(APIversion, handler, variableName, s) { }
            public TC07_Boolean(int APIversion, EventHandler handler) : this(APIversion, handler, 0, "TFVariable", false) { }
            public TC07_Boolean(int APIversion, EventHandler handler, TC07_Boolean basis) : this(APIversion, handler, 0, basis.variableName, basis.unknown1 != 0) { }
            public TC07_Boolean(int APIversion, EventHandler handler, byte unused, string variableName, bool value) : base(APIversion, handler, variableName)
            {
                this.unknown1 = (byte)(value ? 0x01 : 0x00);
            }
            #endregion

            #region Data I/O
            protected override void Parse(Stream s) { unknown1 = (new BinaryReader(s)).ReadByte(); }

            internal override void UnParse(Stream s)
            {
                base.UnParse(s, 0x07);
                (new BinaryWriter(s)).Write(unknown1);
            }
            #endregion

            public override int CompareTo(ComplateElement other)
            {
                int res = base.CompareTo(other);
                return res != 0 ? res : unknown1.CompareTo((other as TC07_Boolean).unknown1);
            }

            public override int GetHashCode(ComplateElement obj)
            {
                TC07_Boolean tc = obj as TC07_Boolean;
                if (tc == null) return base.GetHashCode(obj);
                return tc.variableName.GetHashCode() ^ tc.unknown1.GetHashCode();
            }

            public bool Unknown1 { get { return unknown1 != 0; } set { if (Unknown1 != value) { unknown1 = (byte)(value ? 0x01 : 0x00); OnElementChanged(); } } }
        }

        public class ComplateList : DependentList<ComplateElement>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (TC03_TGIIndex i in this.FindAll(e => e is TC03_TGIIndex)) i.ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            #region Constructors
            public ComplateList(EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler) { _ParentTGIBlocks = ParentTGIBlocks; }
            public ComplateList(EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; Parse(s); this.handler = handler; }
            public ComplateList(EventHandler handler, IEnumerable<ComplateElement> ltc, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; foreach (var t in ltc) this.Add((ComplateElement)t.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override ComplateElement CreateElement(Stream s)
            {
                return ComplateElement.CreateTypeCode(0, elementHandler, ComplateString.Read(s), new BinaryReader(s).ReadByte(), s, _ParentTGIBlocks);
            }

            protected override void WriteCount(Stream s, int count) { new BinaryWriter(s).Write(count/* - this.FindAll(tc => tc is TC_Padding).Count/**/); }
            protected override void WriteElement(Stream s, ComplateElement element) { element.UnParse(s); }
            #endregion

            public override void Add(ComplateElement item)
            {
                if (item is TC03_TGIIndex) (item as TC03_TGIIndex).ParentTGIBlocks = _ParentTGIBlocks;
                base.Add(item);
            }
            public override void Add(Type elementType)
            {
                if (elementType.IsAbstract)
                    throw new ArgumentException("Must pass a concrete element type.", "elementType");

                if (!typeof(ComplateElement).IsAssignableFrom(elementType))
                    throw new ArgumentException("The element type must belong to the generic type of the list.", "elementType");

                ComplateElement newElement;
                if (typeof(TC03_TGIIndex).IsAssignableFrom(elementType))
                    newElement = new TC03_TGIIndex(0, elementHandler, _ParentTGIBlocks);
                else
                    newElement = Activator.CreateInstance(elementType, new object[] { (int)0, elementHandler, }) as ComplateElement;
                base.Add(newElement);
            }
        }
        #endregion

        public class MaterialBlock : AHandlerElement,
            IComparable<MaterialBlock>, IEqualityComparer<MaterialBlock>, IEquatable<MaterialBlock>
        {
            private DependentList<TGIBlock>  _ParentTGIBlocks = null;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set
                {
                    if (_ParentTGIBlocks != value)
                    {
                        _ParentTGIBlocks = value;
                        if (complateList != null) complateList.ParentTGIBlocks = ParentTGIBlocks;
                        if (mbList != null) mbList.ParentTGIBlocks = ParentTGIBlocks;
                    }
                }
            }
            public override List<string> ContentFields { get { List<string> res = GetContentFields(requestedApiVersion, this.GetType()); res.Remove("ParentTGIBlocks"); return res; } }

            #region Attributes
            byte complateXMLIndex;
            string name;
            string pattern;
            ComplateList complateList = null;
            MaterialBlockList mbList = null;
            #endregion

            #region Constructors
            public MaterialBlock(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                _ParentTGIBlocks = ParentTGIBlocks;
                name = "";
                pattern = "";
                complateList = new ComplateList(handler, _ParentTGIBlocks);
                mbList = new MaterialBlockList(handler, _ParentTGIBlocks);
            }

            internal MaterialBlock(int APIversion, EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler) { _ParentTGIBlocks = ParentTGIBlocks; Parse(s); }

            public MaterialBlock(int APIversion, EventHandler handler, MaterialBlock basis, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, basis.complateXMLIndex, basis.name, basis.pattern, basis.complateList, basis.mbList, ParentTGIBlocks ?? basis._ParentTGIBlocks) { }

            public MaterialBlock(int APIversion, EventHandler handler, byte xmlindex, string unknown1, string unknown2,
                IEnumerable<ComplateElement> ltc, IEnumerable<MaterialBlock> lmb, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                _ParentTGIBlocks = ParentTGIBlocks;
                this.complateXMLIndex = xmlindex;
                this.name = unknown1;
                this.pattern = unknown2;
                complateList = ltc == null ? null : new ComplateList(handler, ltc, _ParentTGIBlocks);
                mbList = lmb == null ? null : new MaterialBlockList(handler, lmb, _ParentTGIBlocks);
            }
            #endregion

            #region Data I/O
            protected void Parse(Stream s)
            {
                this.complateXMLIndex = (new BinaryReader(s)).ReadByte();
                this.name = ComplateString.Read(s);
                this.pattern = ComplateString.Read(s);
                this.complateList = new ComplateList(handler, s, _ParentTGIBlocks);
                this.mbList = new MaterialBlockList(handler, s, _ParentTGIBlocks);
            }

            public void UnParse(Stream s)
            {
                (new BinaryWriter(s)).Write(complateXMLIndex);
                ComplateString.Write(s, name);
                ComplateString.Write(s, pattern);
                complateList.UnParse(s);
                mbList.UnParse(s);
            }
            #endregion

            #region IComparable<MaterialBlock> Members

            public int CompareTo(MaterialBlock other)
            {
                int res = complateXMLIndex.CompareTo(other.complateXMLIndex); if (res != 0) return res;
                res = name.CompareTo(other.name); if (res != 0) return res;
                res = pattern.CompareTo(other.pattern); if (res != 0) return res;
                res = complateList.Count.CompareTo(other.complateList.Count); if (res != 0) return res;
                for (int i = 0; i < complateList.Count; i++) { res = complateList[i].CompareTo(other.complateList[i]); if (res != 0) return res; }
                res = mbList.Count.CompareTo(other.mbList.Count); if (res != 0) return res;
                for (int i = 0; i < mbList.Count; i++) { res = mbList[i].CompareTo(other.mbList[i]); if (res != 0) return res; }
                return 0;
            }

            #endregion

            #region IEqualityComparer<MaterialBlock> Members

            public bool Equals(MaterialBlock x, MaterialBlock y) { return x.Equals(y); }

            public int GetHashCode(MaterialBlock obj) { return obj.GetHashCode(); }

            public override int GetHashCode()
            {
                int hc = complateXMLIndex.GetHashCode() ^ name.GetHashCode() ^ pattern.GetHashCode();
                foreach (ComplateElement tc in complateList) hc ^= tc.GetHashCode();
                foreach (MaterialBlock mb in mbList) hc ^= mb.GetHashCode();
                return hc;
            }

            #endregion

            #region IEquatable<MaterialBlock> Members

            public bool Equals(MaterialBlock other) { return this.CompareTo(other) == 0; }

            public override bool Equals(object obj)
            {
                return obj as MaterialBlock != null ? this.Equals((MaterialBlock)obj) : false;
            }

            #endregion

            #region ICloneable Members

            public object Clone() { return new MaterialBlock(requestedApiVersion, handler, this); }

            #endregion

            #region AHandlerElement
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            #endregion

            #region Content Fields
            [ElementPriority(1), TGIBlockListContentField("ParentTGIBlocks")]
            public byte ComplateXMLIndex { get { return complateXMLIndex; } set { if (complateXMLIndex != value) { complateXMLIndex = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public string Name { get { return name; } set { if (name != value) { name = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public string Pattern { get { return pattern; } set { if (pattern != value) { pattern = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public ComplateList ComplateOverrides { get { return complateList; } set { if (complateList != value) { complateList = value == null ? null : new ComplateList(handler, value, _ParentTGIBlocks); OnElementChanged(); } } }
            [ElementPriority(5)]
            public MaterialBlockList MaterialBlocks { get { return mbList; } set { if (mbList != value) { mbList = value == null ? null : new MaterialBlockList(handler, value, _ParentTGIBlocks); OnElementChanged(); } } }

            public String Value { get { return ValueBuilder; } }
            #endregion
        }

        public class MaterialBlockList : DependentList<MaterialBlock>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (var i in this) i.ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            #region Constructors
            public MaterialBlockList(EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler) { _ParentTGIBlocks = ParentTGIBlocks; }
            internal MaterialBlockList(EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; Parse(s); this.handler = handler; }
            public MaterialBlockList(EventHandler handler, IEnumerable<MaterialBlock> lmb, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; foreach (var t in lmb) this.Add((MaterialBlock)t.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override MaterialBlock CreateElement(Stream s) { return new MaterialBlock(0, elementHandler, s, _ParentTGIBlocks); }
            protected override void WriteElement(Stream s, MaterialBlock element) { element.UnParse(s); }
            #endregion

            public override void Add() { this.Add(new MaterialBlock(0, handler, _ParentTGIBlocks)); }
            public override void Add(MaterialBlock item) { item.ParentTGIBlocks = _ParentTGIBlocks; base.Add(item); }
       }

        public class Material : AHandlerElement,
            IComparable<Material>, IEqualityComparer<Material>, IEquatable<Material>
        {
            #region Attributes
            byte materialType;
            uint unknown1;
            ushort unknown2;
            MaterialBlock mb = null;
            TGIBlockList list = null;
            uint unknown3;
            #endregion

            #region Constructors
            public Material(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
                list = new TGIBlockList(handler);
                mb = new MaterialBlock(requestedApiVersion, handler, list);
            }
            internal Material(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Material(int APIversion, EventHandler handler, Material basis)
                : this(APIversion, handler,
                basis.materialType, basis.unknown1, basis.unknown2,
                basis.mb, basis.list, basis.unknown3) { }
            public Material(int APIversion, EventHandler handler, byte materialType, uint unknown1, ushort unknown2,
                MaterialBlock mb, IEnumerable<TGIBlock> ltgib, uint unknown3)
                : base(APIversion, handler)
            {
                this.materialType = materialType;
                this.unknown1 = unknown1;
                this.unknown2 = unknown2;
                this.list = ltgib == null ? null : new TGIBlockList(handler, ltgib);
                this.mb = mb == null ? null : new MaterialBlock(requestedApiVersion, handler, mb, this.list);
                this.unknown3 = unknown3;
            }
            #endregion

            #region Data I/O
            protected virtual void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);

                materialType = r.ReadByte();
                if (materialType != 1) unknown1 = r.ReadUInt32();
                long oset = r.ReadUInt32() + s.Position;
                unknown2 = r.ReadUInt16();
                long tgiPosn = r.ReadUInt32() + s.Position;
                long tgiSize = r.ReadUInt32();

                // For once, I see why I might want to skip forward, read the TGIBlockList: then it could be passed to the MaterialBlock.
                mb = new MaterialBlock(requestedApiVersion, handler, s);

                list = new TGIBlockList(handler, s, tgiPosn, tgiSize);

                if (checking) if (oset != s.Position)
                        throw new InvalidDataException(String.Format("Position of final DWORD read: 0x{0:X8}, actual: 0x{1:X8}",
                            oset, s.Position));

                unknown3 = r.ReadUInt32();

                mb.ParentTGIBlocks = list;
            }

            public virtual void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                long pOset, ptgiO, pos;

                w.Write(materialType);
                if (materialType != 1) w.Write(unknown1);

                pOset = s.Position;
                w.Write((uint)0); // oset

                w.Write(unknown2);

                ptgiO = s.Position;
                w.Write((uint)0); // tgiOffset
                w.Write((uint)0); // tgiSize

                mb.UnParse(s);

                list.UnParse(s, ptgiO);

                pos = s.Position;
                s.Position = pOset;
                w.Write((uint)(pos - pOset - sizeof(uint)));

                s.Position = pos;
                w.Write(unknown3);
            }
            #endregion

            #region IComparable<Material> Members

            public int CompareTo(Material other)
            {
                int res = materialType.CompareTo(other.materialType); if (res != 0) return res;
                res = unknown2.CompareTo(other.unknown2); if (res != 0) return res;
                res = mb.CompareTo(other.mb); if (res != 0) return res;
                return unknown3.CompareTo(other.unknown3);
            }

            #endregion

            #region IEqualityComparer<Material> Members

            public bool Equals(Material x, Material y) { return x.Equals(y); }

            public int GetHashCode(Material obj) { return obj.GetHashCode(); }

            public override int GetHashCode() { return materialType.GetHashCode() ^ unknown2.GetHashCode() ^ mb.GetHashCode() ^ unknown3.GetHashCode(); }

            #endregion

            #region IEquatable<Material> Members

            public bool Equals(Material other) { return this.CompareTo(other) == 0; }

            public override bool Equals(object obj)
            {
                return obj as Material != null ? this.Equals((Material)obj) : false;
            }

            #endregion

            #region ICloneable Members

            public virtual object Clone() { return new Material(requestedApiVersion, handler, this); }

            #endregion

            #region AHandlerElement
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public byte MaterialType { get { return materialType; } set { if (materialType != value) { materialType = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public uint Unknown1 { get { return unknown1; } set { if (materialType == 1) throw new InvalidOperationException(); if (unknown1 != value) { unknown1 = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public ushort Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public MaterialBlock MaterialBlock { get { return mb; } set { if (mb != value) { mb = new MaterialBlock(requestedApiVersion, handler, value, list); OnElementChanged(); } } }
            [ElementPriority(5)]
            public TGIBlockList TGIBlocks
            {
                get { return list; }
                set { if (list != (value as TGIBlockList)) { list = value == null ? null : new TGIBlockList(handler, value); mb.ParentTGIBlocks = list; OnElementChanged(); } }
            }
            [ElementPriority(6)]
            public uint Unknown3 { get { return unknown3; } set { if (unknown3 != value) { unknown3 = value; OnElementChanged(); } } }

            public String Value { get { return ValueBuilder; } }
            #endregion
        }

        public class MaterialList : DependentList<Material>
        {
            #region Constructors
            internal MaterialList(EventHandler handler) : base(handler) { }
            internal MaterialList(EventHandler handler, Stream s) : base(handler, s) { }
            public MaterialList(EventHandler handler, IEnumerable<Material> lme) : base(handler, lme) { }
            #endregion

            #region Data I/O
            protected override Material CreateElement(Stream s) { return new Material(0, elementHandler, s); }
            protected override void WriteElement(Stream s, Material element) { element.UnParse(s); }
            #endregion
        }

        static class ComplateString
        {
            #region Static initialiser
            static List<string> stringTable = new List<string>(new string[] {
                    /* 00 */  ""
                    /* 01 */, "filename"
                    /* 02 */, "X:"
                    /* 03 */, "-1"

                    /* 04 */, "assetRoot"
                    /* 05 */, "daeFileName"
                    /* 06 */, "daeFilePath"
                    /* 07 */, "Color"

                    /* 08 */, "ObjectRgbMask"
                    /* 09 */, "rgbmask"
                    /* 0a */, "specmap"
                    /* 0b */, "Background Image"

                    /* 0c */, "HSVShift Bg"
                    /* 0d */, "H Bg"
                    /* 0e */, "V Bg"
                    /* 0f */, "S Bg"


                    /* 10 */, "Base H Bg"
                    /* 11 */, "Base V Bg"
                    /* 12 */, "Base S Bg"
                    /* 13 */, "Mask"

                    /* 14 */, "Multiplier"
                    /* 15 */, "Dirt Layer"
                    /* 16 */, "1X Multiplier"
                    /* 17 */, "Specular"

                    /* 18 */, "Overlay"
                    /* 19 */, "Face"
                    /* 1a */, "partType"
                    /* 1b */, "gender"

                    /* 1c */, "bodyType"
                    /* 1d */, "age"
                    /* 1e */, "A"
                    /* 1f */, "M"


                    /* 20 */, "Stencil A"
                    /* 21 */, "Stencil B"
                    /* 22 */, "Stencil C"
                    /* 23 */, "Stencil D"

                    /* 24 */, "Stencil A Enabled"
                    /* 25 */, "Stencil B Enabled"
                    /* 26 */, "Stencil C Enabled"
                    /* 27 */, "Stencil D Enabled"

                    /* 28 */, "Stencil A Tiling"
                    /* 29 */, "Stencil B Tiling"
                    /* 2a */, "Stencil C Tiling"
                    /* 2b */, "Stencil D Tiling"

                    /* 2c */, "Stencil A Rotation"
                    /* 2d */, "Stencil B Rotation"
                    /* 2e */, "Stencil C Rotation"
                    /* 2f */, "Stencil D Rotation"


                    /* 30 */, "Pattern A"
                    /* 31 */, "Pattern B"
                    /* 32 */, "Pattern C"
                    /* 33 */, "Pattern A Enabled"

                    /* 34 */, "Pattern B Enabled"
                    /* 35 */, "Pattern C Enabled"
                    /* 36 */, "Pattern A Linked"
                    /* 37 */, "Pattern B Linked"

                    /* 38 */, "Pattern C Linked"
                    /* 39 */, "Pattern A Rotation"
                    /* 3a */, "Pattern B Rotation"
                    /* 3b */, "Pattern C Rotation"

                    /* 3c */, "Pattern A Tiling"
                    /* 3d */, "Pattern B Tiling"
                    /* 3e */, "Pattern C Tiling"
                    /* 3f */, "?0x3F"


                    /* 40 */, ""
                    /* 41 */, "MaskWidth"
                    /* 42 */, "MaskHeight"
                    /* 43 */, "ObjectRgbaMask"

                    /* 44 */, "RndColors"
                    /* 45 */, "Flat Color"
                    /* 46 */, "Alpha"
                    /* 47 */, "Color 0"

                    /* 48 */, "Color 1"
                    /* 49 */, "Color 2"
                    /* 4a */, "Color 3"
                    /* 4b */, "Color 4"

                    /* 4c */, "Channel 1"
                    /* 4d */, "Channel 2"
                    /* 4e */, "Channel 3"
                    /* 4f */, "Pattern D"


                    /* 50 */, "Pattern D Tiling"
                    /* 51 */, "Pattern D Enabled"
                    /* 52 */, "Pattern D Linked"
                    /* 53 */, "Pattern D Rotation"

                    /* 54 */, "HSVShift 1"
                    /* 55 */, "HSVShift 2"
                    /* 56 */, "HSVShift 3"
                    /* 57 */, "Channel 1 Enabled"

                    /* 58 */, "Channel 2 Enabled"
                    /* 59 */, "Channel 3 Enabled"
                    /* 5a */, "Base H 1"
                    /* 5b */, "Base V 1"

                    /* 5c */, "Base S 1"
                    /* 5d */, "Base H 2"
                    /* 5e */, "Base V 2"
                    /* 5f */, "Base S 2"


                    /* 60 */, "Base H 3"
                    /* 61 */, "Base V 3"
                    /* 62 */, "Base S 3"
                    /* 63 */, "H 1"

                    /* 64 */, "S 1"
                    /* 65 */, "V 1"
                    /* 66 */, "H 2"
                    /* 67 */, "S 2"

                    /* 68 */, "V 2"
                    /* 69 */, "H 3"
                    /* 6a */, "V 3"
                    /* 6b */, "S 3"

                    /* 6c */, "true"
                    /* 6d */, "1,0,0,0"
                    /* 6e */, "defaultFlatColor"
                    /* 6f */, "solidColor_1"
                });
            #endregion

            public static string Read(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                byte a = r.ReadByte();

                if (a == 0) return String.Empty;

                if ((a & 0x80) != 0)
                {
                    if ((a & 0x40) != 0) a = r.ReadByte();
                    else a &= 0x3F;
                    return Encoding.ASCII.GetString(r.ReadBytes(a));
                }
                
                if (a == 0x40) a += r.ReadByte();
                return stringTable[a];
            }

            public static void Write(Stream s, String value)
            {
                BinaryWriter w = new BinaryWriter(s);

                if (value == String.Empty)
                {
                    w.Write((byte)0);
                }
                else
                {
                    int i = stringTable.IndexOf(value);
                    if (i < 0)
                    {
                        if (value.Length > byte.MaxValue)
                            throw new ArgumentLengthException("value", byte.MaxValue);
                        if (value.Length > 0x3F)
                        {
                            w.Write((byte)(0x80 | 0x40));
                            w.Write((byte)value.Length);
                        }
                        else
                            w.Write((byte)(0x80 | value.Length));
                        w.Write(Encoding.ASCII.GetBytes(value));
                    }
                    else
                    {
                        if (i >= 0x40)
                        {
                            w.Write((byte)0x40);
                            w.Write((byte)(i - 0x40));
                        }
                        else
                        {
                            w.Write((byte)i);
                        }
                    }
                }
            }
        }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(11)]
        public Common CommonBlock { get { return common; } set { if (common != value) { common = new Common(requestedApiVersion, OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }

        public virtual String Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// A CatalogResource wrapper that contains a TGIBlockList
    /// </summary>
    public abstract class CatalogResourceTGIBlockList : CatalogResource
    {
        #region Attributes
        protected long tgiPosn, tgiSize;
        protected TGIBlockList list = null;
        #endregion

        #region Constructors
        public CatalogResourceTGIBlockList(int APIversion, Stream s) : base(APIversion, s)
        {
            // http://private/s3pi/index.php?topic=1368.0
            // As index values are defaulted to zero, we should have a zeroth entry.
            if (list == null || list.Count == 0)
            {
                list = new TGIBlockList(OnResourceChanged);
                list.Add();
            }
        }
        public CatalogResourceTGIBlockList(int APIversion, uint version, Common common, IEnumerable<TGIBlock> tgibl) : base(APIversion, version, common)
        {
            // http://private/s3pi/index.php?topic=1368.0
            // As index values are defaulted to zero, we should have a zeroth entry.
            if (tgibl == null)
                list = new TGIBlockList(OnResourceChanged);
            else
                list = new TGIBlockList(OnResourceChanged, tgibl);

            if (list.Count == 0)
                list.Add();
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            base.Parse(s);
            BinaryReader r = new BinaryReader(s);
            tgiPosn = r.ReadUInt32() + s.Position;
            tgiSize = r.ReadUInt32();
        }

        private long pos;
        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);
            pos = s.Position;
            w.Write((uint)0); // tgiOffset
            w.Write((uint)0); // tgiSize
            return s;
        }

        protected virtual void UnParse(Stream s)
        {
            // http://private/s3pi/index.php?topic=1368.0
            // As index values are defaulted to zero, we should have a zeroth entry.
            // By now, we really should have something, though...
            if (list == null || list.Count == 0)
            {
                list = new TGIBlockList(OnResourceChanged);
                list.Add();
            }
            list.UnParse(s, pos);
        }
        #endregion

        #region Content Fields
        public virtual TGIBlockList TGIBlocks { get { return list; } set { if (list != value) { list = value == null ? null : new TGIBlockList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for CatalogResource wrapper
    /// </summary>
    public class CatalogResourceHandler : AResourceHandler
    {
        public CatalogResourceHandler()
        {
            this.Add(typeof(FenceCatalogResource), new List<string>(new string[] { "0x0418FE2A" }));
            this.Add(typeof(FireplaceCatalogResource), new List<string>(new string[] { "0x04F3CC01" }));
            this.Add(typeof(FoundationCatalogResource), new List<string>(new string[] { "0x316C78F2" }));
            if (!s4pi.Settings.Settings.IsTS4) this.Add(typeof(ObjectCatalogResource), new List<string>(new string[] { "0x319E4F1D" }));
            this.Add(typeof(FountainPoolCatalogResource), new List<string>(new string[] { "0x0A36F07A" }));
            this.Add(typeof(ProxyProductCatalogResource), new List<string>(new string[] { "0x04AC5D93" }));
            this.Add(typeof(RailingCatalogResource), new List<string>(new string[] { "0x04C58103" }));
            this.Add(typeof(RoofPatternCatalogResource), new List<string>(new string[] { "0xF1EDBD86" }));
            this.Add(typeof(RoofStyleCatalogResource), new List<string>(new string[] { "0x91EDBD3E" }));
            this.Add(typeof(StairsCatalogResource), new List<string>(new string[] { "0x049CA4CD" }));
            this.Add(typeof(TerrainGeometryWaterBrushCatalogResource), new List<string>(new string[] { "0x04B30669", "0x060B390C" }));
            this.Add(typeof(TerrainPaintBrushCatalogResource), new List<string>(new string[] { "0x04ED4BB2" }));
            this.Add(typeof(WallCatalogResource), new List<string>(new string[] { "0x9151E6BC" }));
            this.Add(typeof(WallFloorPatternCatalogResource), new List<string>(new string[] { "0x515CA4CD" }));
        }
    }
}
