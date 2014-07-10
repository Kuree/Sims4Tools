using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace CASPartResource
{
    /// <summary>
    /// A resource wrapper that understands Sim Outfit resources
    /// </summary>
    public class SimOutfitResource : AResource
    {
        const int recommendedApiVersion = 1;

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint version = 0x0015; // we can live in hope...

        //-- version >= 0x08 attributes
        XMLEntryList xmlEntries;//version >= 0x10
        UIntList unknown3;//version >= 0x09 && version < 0x10
        int unknown1;//version >= 0x10
        int unknown2;//version >= 0x10
        float heavyWeightSlider;
        float strengthSlider;
        float slimWeightSlider;
        uint unknown6;//version >= 0x09
        AgeGenderFlags age;
        AgeGenderFlags gender;
        AgeGenderFlags species;
        AgeGenderFlags handedness;//version >= 0x09
        short skinToneIndex;//byte if version < 0x15
        byte hairToneIndex;//version == 0x08
        float eyelashSlider;
        float muscleSlider;//version >= 0x11
        float breastSlider;//version >= 0x12
        UInt32 hairBaseColour;//version >= 0x0E
        UInt32 hairHaloHighColour;//version >= 0x0E
        UInt32 hairHaloLowColour;//version >= 0x0E
        float numCurls;//version >= 0x13
        float curlPixelRadius;//version >= 0x13
        TGIBlock furMap;//version >= 0x14
        uint unknown11;//version == 0x08
        byte unknown12;//version >= 0x09, version <= 0x0D
        CASEntryList caspEntries;
        byte zero;
        FaceEntryList faceEntries;
        uint unknown13;//version == 0x08
        CountedTGIBlockList tgiBlocks;

        //-- version < 0x08 attributes
        uint unknown1v7;
        uint unknown2v7;
        uint unknown3v7;
        AgeGenderFlags agev7;
        AgeGenderFlags genderv7;
        uint unknown4v7;
        TGIBlock skinTonev7;
        TGIBlock hairTonev7;
        uint unknown5v7;
        uint unknown6v7;
        CASV7EntryList caspEntriesv7;
        ushort unknown7v7;
        #endregion

        public SimOutfitResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            long tgiPosn = -1;

            BinaryReader r = new BinaryReader(s);

            version = r.ReadUInt32();

            if (version >= 0x08)
            {
                #region Non-graveyard versions
                tgiPosn = r.ReadInt32() + s.Position;

                if (version >= 0x09)
                {
                    if (version >= 0x10)
                    {
                        xmlEntries = new XMLEntryList(OnResourceChanged, s, version);
                    }
                    else
                    {
                        unknown3 = new UIntList(OnResourceChanged, s);
                    }
                }

                if (version >= 0x0E)
                {
                    unknown1 = r.ReadInt32();
                    unknown2 = r.ReadInt32();
                }

                heavyWeightSlider = r.ReadSingle();
                strengthSlider = r.ReadSingle();
                slimWeightSlider = r.ReadSingle();

                if (version >= 0x09)
                {
                    unknown6 = r.ReadUInt32();
                }

                age = new AgeGenderFlags(0, OnResourceChanged, s);
                gender = new AgeGenderFlags(0, OnResourceChanged, s);
                species = new AgeGenderFlags(0, OnResourceChanged, s);

                if (version >= 0x09)
                    handedness = new AgeGenderFlags(0, OnResourceChanged, s);

                if (version >= 0x15)
                    skinToneIndex = r.ReadInt16();
                else
                    skinToneIndex = r.ReadByte();

                if (version == 0x08)
                    hairToneIndex = r.ReadByte();

                eyelashSlider = r.ReadSingle();

                if (version >= 0x09)
                {
                    if (version >= 0x000E)
                    {
                        if (version >= 0x0011)
                        {
                            muscleSlider = r.ReadSingle();
                            if (version >= 0x0012)
                            {
                                breastSlider = r.ReadSingle();
                            }
                        }

                        hairBaseColour = r.ReadUInt32();
                        hairHaloHighColour = r.ReadUInt32();
                        hairHaloLowColour = r.ReadUInt32();

                        if (version >= 0x0013)
                        {
                            numCurls = r.ReadSingle();
                            curlPixelRadius = r.ReadSingle();
                            if (version >= 0x0014)
                            {
                                furMap = new TGIBlock(requestedApiVersion, OnResourceChanged, s);
                            }
                        }
                    }
                    else
                    {
                        unknown12 = r.ReadByte();//?hairToneIndex
                    }
                }
                else
                {
                    unknown11 = r.ReadUInt32();
                }

                caspEntries = new CASEntryList(OnResourceChanged, s, version);

                zero = r.ReadByte();
                if (checking) if (zero != 0)
                        throw new InvalidDataException(String.Format("Expected zero, read: 0x{0:X2}, at: 0x{1:X8}",
                            zero, s.Position));

                faceEntries = new FaceEntryList(OnResourceChanged, s, version);

                if (version < 0x0A)
                    unknown13 = r.ReadUInt32();

                if (checking) if (tgiPosn != s.Position)
                        throw new InvalidDataException(String.Format("Position of TGIBlock read: 0x{0:X8}, actual: 0x{1:X8}",
                            tgiPosn, s.Position));

                if (version >= 0x15)
                {
                    short count = r.ReadInt16();
                    tgiBlocks = new CountedTGIBlockList(OnResourceChanged, "IGT", count, s, ushort.MaxValue);
                }
                else
                {
                    byte count = r.ReadByte();
                    tgiBlocks = new CountedTGIBlockList(OnResourceChanged, "IGT", count, s, byte.MaxValue);
                }

                caspEntries.ParentTGIBlocks = tgiBlocks;
                faceEntries.ParentTGIBlocks = tgiBlocks;
                #endregion
            }
            else
            {
                #region Ancient versions
                unknown1v7 = r.ReadUInt32();
                unknown2v7 = r.ReadUInt32();
                unknown3v7 = r.ReadUInt32();

                agev7 = new AgeGenderFlags(0, OnResourceChanged, s);
                genderv7 = new AgeGenderFlags(0, OnResourceChanged, s);

                unknown4v7 = r.ReadUInt32();

                skinTonev7 = new TGIBlock(requestedApiVersion, OnResourceChanged, "IGT", s);
                hairTonev7 = new TGIBlock(requestedApiVersion, OnResourceChanged, "IGT", s);

                unknown5v7 = r.ReadUInt32();
                unknown6v7 = r.ReadUInt32();

                caspEntriesv7 = new CASV7EntryList(OnResourceChanged, s, version);

                unknown7v7 = r.ReadUInt16();
                #endregion
            }
        }

        protected override Stream UnParse()
        {
            long posn, tgiPosn, end;
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(version);

            if (version >= 0x08)
            {
                #region Non-graveyard versions
                posn = s.Position;
                w.Write((int)0); //offset

                if (version >= 0x09)
                {
                    if (version >= 0x10)
                    {
                        if (xmlEntries == null) xmlEntries = new XMLEntryList(OnResourceChanged, version);
                        xmlEntries.UnParse(s);
                    }
                    else
                    {
                        if (unknown3 == null) unknown3 = new UIntList(OnResourceChanged);
                        unknown3.UnParse(s);
                    }
                }

                if (version >= 0x0E)
                {
                    w.Write(unknown1);
                    w.Write(unknown2);
                }

                w.Write(heavyWeightSlider);
                w.Write(strengthSlider);
                w.Write(slimWeightSlider);
                if (version >= 0x09)
                {
                    w.Write(unknown6);
                }

                age.UnParse(s);
                gender.UnParse(s);
                species.UnParse(s);

                if (version >= 0x09)
                    handedness.UnParse(s);

                if (version >= 0x15)
                    w.Write(skinToneIndex);
                else
                    w.Write((byte)skinToneIndex);

                if (version == 0x08)
                    w.Write(hairToneIndex);

                w.Write(eyelashSlider);

                if (version >= 0x09)
                {
                    if (version >= 0x000E)
                    {
                        if (version >= 0x0011)
                        {
                            w.Write(muscleSlider);
                            if (version >= 0x0012)
                            {
                                w.Write(breastSlider);
                            }
                        }

                        w.Write(hairBaseColour);
                        w.Write(hairHaloHighColour);
                        w.Write(hairHaloLowColour);

                        if (version >= 0x0013)
                        {
                            w.Write(numCurls);
                            w.Write(curlPixelRadius);
                            if (version >= 0x0014)
                            {
                                if (furMap == null) furMap = new TGIBlock(requestedApiVersion, OnResourceChanged);
                                furMap.UnParse(s);
                            }
                        }
                    }
                    else
                    {
                        w.Write(unknown12);//?hairToneIndex
                    }
                }
                else
                {
                    w.Write(unknown11);
                }

                if (tgiBlocks == null) tgiBlocks = new CountedTGIBlockList(OnResourceChanged, "IGT");
                if (caspEntries == null) caspEntries = new CASEntryList(OnResourceChanged, version, tgiBlocks);
                caspEntries.UnParse(s);

                w.Write(zero);

                if (faceEntries == null) faceEntries = new FaceEntryList(OnResourceChanged, version, tgiBlocks);
                faceEntries.UnParse(s);

                if (version < 0x0A)
                    w.Write(unknown13);

                tgiPosn = s.Position;
                if (version >= 0x15)
                {
                    w.Write((short)tgiBlocks.Count);
                }
                else
                {
                    w.Write((byte)tgiBlocks.Count);
                }
                tgiBlocks.UnParse(s);

                end = s.Position;

                s.Position = posn;
                w.Write((int)(tgiPosn - posn - sizeof(int)));
                s.Position = end;
                #endregion
            }
            else
            {
                #region Ancient versions
                w.Write(unknown1v7);
                w.Write(unknown2v7);
                w.Write(unknown3v7);

                agev7.UnParse(s);
                genderv7.UnParse(s);

                w.Write(unknown4v7);

                skinTonev7.UnParse(s);
                hairTonev7.UnParse(s);

                w.Write(unknown5v7);
                w.Write(unknown6v7);

                caspEntriesv7.UnParse(s);

                w.Write(unknown7v7);
                #endregion
            }


            s.Flush();
            return s;
        }
        #endregion

        #region Sub-types
        public class XMLEntry : AHandlerElement, IEquatable<XMLEntry>
        {
            const int recommendedApiVersion = 1;

            public uint ParentVersion { get; set; }

            #region Attributes
            byte unknown1;//version>=0x0B
            string xml;
            #endregion

            #region Constructors
            public XMLEntry(int APIversion, EventHandler handler, uint parentVersion) : base(APIversion, handler)
            {
                if (s4pi.Settings.Settings.Checking) if (parentVersion < 0x0B)
                        throw new InvalidOperationException(String.Format("XMLEntry constructor requires Unknown1 for SimOutfitResource version {0}", parentVersion));
                this.ParentVersion = parentVersion;
            }
            public XMLEntry(int APIversion, EventHandler handler, Stream s, uint parentVersion) : this(APIversion, handler, parentVersion) { Parse(s); }
            public XMLEntry(int APIversion, EventHandler handler, XMLEntry basis, uint parentVersion) : this(APIversion, handler, basis.unknown1, basis.xml, parentVersion) { }
            public XMLEntry(int APIversion, EventHandler handler, byte unknown1, string xml, uint parentVersion)
                : this(APIversion, handler, parentVersion) { this.unknown1 = unknown1; this.xml = xml; }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                unknown1 = r.ReadByte();
                xml = System.Text.Encoding.Unicode.GetString(r.ReadBytes(r.ReadInt32() * 2));
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(unknown1);
                if (xml == null) xml = "";
                w.Write(xml.Length);
                w.Write(System.Text.Encoding.Unicode.GetBytes(xml));
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields
            {
                get
                {
                    var res = GetContentFields(requestedApiVersion, this.GetType());
                    res.Remove("ParentVersion");
                    return res;
                }
            }
            protected override List<string> ValueBuilderFields
            {
                get
                {
                    var res = base.ValueBuilderFields;
                    res.Remove("XmlFile");
                    return res;
                }
            }
            #endregion

            #region IEquatable<XMLEntry> Members

            public bool Equals(XMLEntry other)
            {
                return
                    this.unknown1 == other.unknown1
                    && this.xml == other.xml
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as XMLEntry != null ? this.Equals(obj as XMLEntry) : false;
            }

            public override int GetHashCode()
            {
                return
                    this.unknown1.GetHashCode()
                    ^ this.xml.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public byte Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public string Xml { get { return xml; } set { if (xml != value) { xml = value; OnElementChanged(); } } }

            public TextReader XmlFile { get { return new StringReader(xml); } set { string temp = value.ReadToEnd(); if (xml != temp) { xml = temp; OnElementChanged(); } } }

            public String Value
            {
                get
                {
                    var res = ValueBuilder;
                    if (res.Length > 255)
                        res = res.Substring(0, 252) + "...";
                    return res;
                }
            }
            #endregion
        }
        public class XMLEntryList : DependentList<XMLEntry>
        {
            private uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set { if (_ParentVersion != value) { _ParentVersion = value; foreach (var i in this) i.ParentVersion = _ParentVersion; } }
            }

            #region Constructors
            public XMLEntryList(EventHandler handler, uint parentVersion) : base(handler) { this._ParentVersion = parentVersion; }
            public XMLEntryList(EventHandler handler, Stream s, uint parentVersion) : this(null, parentVersion) { elementHandler = handler; Parse(s); this.handler = handler; }
            public XMLEntryList(EventHandler handler, IEnumerable<XMLEntry> le, uint parentVersion) : this(null, parentVersion) { elementHandler = handler; foreach (var x in le) this.Add((XMLEntry)x.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override XMLEntry CreateElement(Stream s) { return new XMLEntry(0, elementHandler, s, _ParentVersion); }
            protected override void WriteElement(Stream s, XMLEntry element) { element.UnParse(s); }
            #endregion

            public override void Add() { base.Add(new XMLEntry(0, handler, _ParentVersion)); }
            public override void Add(XMLEntry item) { item.ParentVersion = _ParentVersion; base.Add(item); }
        }

        public class IndexPair : AHandlerElement, IEquatable<IndexPair>
        {
            const int recommendedApiVersion = 1;
            public DependentList<TGIBlock> ParentTGIBlocks { get; set; }
            private uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set
                {
                    if (s4pi.Settings.Settings.Checking && _ParentVersion >= 0x15 && value < 0x15 && (!txtc1index.IsByteSized() || !txtc2index.IsByteSized()))
                        throw new InvalidOperationException();
                    _ParentVersion = value;
                }
            }

            #region Attributes
            short txtc1index;//byte if version < 0x15
            short txtc2index;//byte if version < 0x15
            #endregion

            #region Constructors
            public IndexPair(int APIversion, EventHandler handler, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null) : base(APIversion, handler)
            {
                this._ParentVersion = parentVersion;
                this.ParentTGIBlocks = ParentTGIBlocks;
            }
            public IndexPair(int APIversion, EventHandler handler, Stream s, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                this._ParentVersion = parentVersion;
                this.ParentTGIBlocks = ParentTGIBlocks;
                Parse(s);
            }
            public IndexPair(int APIversion, EventHandler handler, IndexPair basis, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, basis.txtc1index, basis.txtc2index, parentVersion, ParentTGIBlocks ?? basis.ParentTGIBlocks) { }
            public IndexPair(int APIversion, EventHandler handler, byte txtc1index, byte txtc2index, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, (short)txtc1index, (short)txtc2index, parentVersion, ParentTGIBlocks)
            {
                if (s4pi.Settings.Settings.Checking) if (parentVersion >= 0x15)
                        throw new InvalidOperationException(String.Format("IndexPair constructor requires Txtc1index and Txtc2index as ushort for SimOutfitResource version {0}", parentVersion));
            }
            public IndexPair(int APIversion, EventHandler handler, short txtc1index, short txtc2index, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                if (s4pi.Settings.Settings.Checking && parentVersion < 0x15 && (!txtc1index.IsByteSized() || !txtc2index.IsByteSized()))
                    throw new InvalidOperationException();

                this._ParentVersion = parentVersion;
                this.ParentTGIBlocks = ParentTGIBlocks;
                this.txtc1index = txtc1index;
                this.txtc2index = txtc2index;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                if (_ParentVersion >= 0x00000015)
                {
                    txtc1index = r.ReadInt16();
                    txtc2index = r.ReadInt16();
                }
                else
                {
                    txtc1index = r.ReadByte();
                    txtc2index = r.ReadByte();
                }
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                if (_ParentVersion >= 0x00000015)
                {
                    w.Write(txtc1index);
                    w.Write(txtc2index);
                }
                else
                {
                    w.Write((byte)txtc1index);
                    w.Write((byte)txtc2index);
                }
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields
            {
                get {
                    List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                    res.Remove("ParentTGIBlocks");
                    res.Remove("ParentVersion");
                    return res;
                }
            }
            #endregion

            #region IEquatable<IndexPair> Members

            public bool Equals(IndexPair other) { return this.txtc1index == other.txtc1index && this.txtc2index == other.txtc2index; }
            public override bool Equals(object obj)
            {
                return obj as IndexPair != null ? this.Equals(obj as IndexPair) : false;
            }
            public override int GetHashCode()
            {
                return txtc1index.GetHashCode() ^ txtc2index.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1), TGIBlockListContentField("ParentTGIBlocks")]
            public short TXTC1Index
            {
                get { return txtc1index; }
                set
                {
                    if (_ParentVersion < 0x00000015 && !value.IsByteSized()) throw new InvalidOperationException();
                    if (txtc1index != value) { txtc1index = value; OnElementChanged(); }
                }
            }
            [ElementPriority(2), TGIBlockListContentField("ParentTGIBlocks")]
            public short TXTC2Index
            {
                get { return txtc2index; }
                set
                {
                    if (_ParentVersion < 0x00000015 && !value.IsByteSized()) throw new InvalidOperationException();
                    if (txtc2index != value) { txtc2index = value; OnElementChanged(); }
                }
            }

            public string Value { get { return "{ " + string.Join("; ", ValueBuilder.Split('\n')) + " }"; } }
            #endregion
        }
        public class IndexPairList : DependentList<IndexPair>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (var i in this) i.ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            private uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set { if (_ParentVersion != value) { _ParentVersion = value; foreach (var item in this) item.ParentVersion = _ParentVersion; } }
            }

            #region Constructors
            public IndexPairList(EventHandler handler, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler, Byte.MaxValue) { _ParentVersion = parentVersion; _ParentTGIBlocks = ParentTGIBlocks; }
            public IndexPairList(EventHandler handler, Stream s, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, parentVersion, ParentTGIBlocks) { elementHandler = handler; Parse(s); this.handler = handler; }
            public IndexPairList(EventHandler handler, IEnumerable<IndexPair> le, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, parentVersion, ParentTGIBlocks) { elementHandler = handler; foreach (var t in le) this.Add((IndexPair)t.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return new BinaryReader(s).ReadByte(); }
            protected override void WriteCount(Stream s, int count) { new BinaryWriter(s).Write((Byte)count); }
            protected override IndexPair CreateElement(Stream s) { return new IndexPair(0, elementHandler, s, _ParentVersion, _ParentTGIBlocks); }
            protected override void WriteElement(Stream s, IndexPair element) { element.UnParse(s); }
            #endregion

            public override void Add() { base.Add(new IndexPair(0, handler, _ParentVersion, _ParentTGIBlocks)); }
            public override void Add(IndexPair item)
            {
                item.ParentVersion = _ParentVersion;
                item.ParentTGIBlocks = _ParentTGIBlocks;
                base.Add(item);
            }
        }

        public class CASEntry : AHandlerElement, IEquatable<CASEntry>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; txtcIndexes.ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            private uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set
                {
                    if (_ParentVersion != value)
                    {
                        if (s4pi.Settings.Settings.Checking && _ParentVersion >= 0x15 && value < 0x15 && !casPartIndex.IsByteSized())
                            throw new InvalidOperationException();
                        _ParentVersion = value;
                        txtcIndexes.ParentVersion = _ParentVersion;
                    }
                }
            }

            const int recommendedApiVersion = 1;

            #region Attributes
            short casPartIndex;//byte if version < 0x15
            ClothingType clothing;//version >= 0x0E
            IndexPairList txtcIndexes;
            #endregion

            #region Constructors
            public CASEntry(int APIversion, EventHandler handler, uint ParentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                _ParentVersion = ParentVersion;
                _ParentTGIBlocks = ParentTGIBlocks;
                txtcIndexes = null;
            }
            public CASEntry(int APIversion, EventHandler handler, Stream s, uint ParentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler) { _ParentVersion = ParentVersion; _ParentTGIBlocks = ParentTGIBlocks; Parse(s); }
            public CASEntry(int APIversion, EventHandler handler, CASEntry basis, uint ParentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, basis.casPartIndex, basis.clothing, basis.txtcIndexes, ParentVersion, ParentTGIBlocks ?? basis.ParentTGIBlocks) { }
            public CASEntry(int APIversion, EventHandler handler, byte casPartIndex, IEnumerable<IndexPair> ibe, uint ParentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, casPartIndex, 0, ibe, ParentVersion, ParentTGIBlocks)
            {
                if (s4pi.Settings.Settings.Checking && ParentVersion >= 0x0E)
                    throw new InvalidOperationException(String.Format("ClothingType must be specified for version {0}", ParentVersion));
            }
            public CASEntry(int APIversion, EventHandler handler, byte casPartIndex, ClothingType clothing, IEnumerable<IndexPair> ibe, uint ParentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, (short)casPartIndex, clothing, ibe, ParentVersion, ParentTGIBlocks)
            {
                if (s4pi.Settings.Settings.Checking && ParentVersion >= 0x15)
                    throw new InvalidOperationException(String.Format("CasPartIndex must be short for version {0}", ParentVersion));
            }
            public CASEntry(int APIversion, EventHandler handler, short casPartIndex, ClothingType clothing, IEnumerable<IndexPair> ibe, uint ParentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler)
            {
                if (s4pi.Settings.Settings.Checking && ParentVersion < 0x15 && !casPartIndex.IsByteSized())
                    throw new InvalidOperationException();

                _ParentVersion = ParentVersion;
                _ParentTGIBlocks = ParentTGIBlocks;
                this.casPartIndex = casPartIndex;
                this.clothing = clothing;
                this.txtcIndexes = ibe == null ? null : new IndexPairList(handler, ibe, _ParentVersion, _ParentTGIBlocks);
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                if (_ParentVersion >= 0x15)
                {
                    casPartIndex = r.ReadInt16();
                }
                else
                {
                    casPartIndex = r.ReadByte();
                }
                if (_ParentVersion >= 0x0E)
                    clothing = (ClothingType)r.ReadUInt32();
                txtcIndexes = new IndexPairList(handler, s, _ParentVersion, _ParentTGIBlocks);
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                if (_ParentVersion >= 0x15)
                {
                    w.Write(casPartIndex);
                }
                else
                {
                    w.Write((byte)casPartIndex);
                }
                if (_ParentVersion >= 0x0E)
                    w.Write((uint)clothing);
                if (txtcIndexes == null) txtcIndexes = new IndexPairList(handler, _ParentVersion, _ParentTGIBlocks);
                txtcIndexes.UnParse(s);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields
            {
                get
                {
                    List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                    res.Remove("ParentVersion");
                    res.Remove("ParentTGIBlocks");
                    if (_ParentVersion < 0x0000000E)
                    {
                        res.Remove("Clothing");
                    }
                    return res;
                }
            }
            #endregion

            #region IEquatable<CASEntry> Members

            public bool Equals(CASEntry other)
            {
                return
                    this.casPartIndex == other.casPartIndex
                    && this.clothing == other.clothing
                    && this.txtcIndexes.Equals(other.txtcIndexes)
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as CASEntry != null ? this.Equals(obj as CASEntry) : false;
            }

            public override int GetHashCode()
            {
                return casPartIndex.GetHashCode() ^ clothing.GetHashCode() ^ txtcIndexes.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1), TGIBlockListContentField("ParentTGIBlocks")]
            public short CASPartIndex
            {
                get { return casPartIndex; }
                set
                {
                    if (_ParentVersion < 0x00000015 && !value.IsByteSized()) throw new InvalidOperationException();
                    if (casPartIndex != value) { casPartIndex = value; OnElementChanged(); }
                }
            }
            [ElementPriority(2)]
            public ClothingType Clothing
            {
                get { if (_ParentVersion < 0x0000000E) throw new InvalidOperationException(); return clothing; }
                set { if (_ParentVersion < 0x0000000E) throw new InvalidOperationException(); if (clothing != value) { clothing = value; OnElementChanged(); } }
            }
            [ElementPriority(3)]
            public IndexPairList TXTCIndexes { get { return txtcIndexes; } set { if (!txtcIndexes.Equals(value)) { txtcIndexes = value == null ? null : new IndexPairList(handler, value, _ParentVersion, _ParentTGIBlocks); OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class CASEntryList : DependentList<CASEntry>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (var i in this) i.ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            private uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set { if (_ParentVersion != value) { _ParentVersion = value; foreach (var item in this) item.ParentVersion = _ParentVersion; } }
            }

            #region Constructors
            public CASEntryList(EventHandler handler, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler, Byte.MaxValue) { _ParentVersion = parentVersion; _ParentTGIBlocks = ParentTGIBlocks; }
            public CASEntryList(EventHandler handler, Stream s, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler, Byte.MaxValue) { _ParentVersion = parentVersion; _ParentTGIBlocks = ParentTGIBlocks; elementHandler = handler; Parse(s); this.handler = handler; }
            public CASEntryList(EventHandler handler, IEnumerable<CASEntry> le, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler, Byte.MaxValue) { _ParentVersion = parentVersion; _ParentTGIBlocks = ParentTGIBlocks; elementHandler = handler; foreach (var t in le) this.Add(t); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return new BinaryReader(s).ReadByte(); }
            protected override void WriteCount(Stream s, int count) { new BinaryWriter(s).Write((byte)count); }
            protected override CASEntry CreateElement(Stream s) { return new CASEntry(0, elementHandler, s, _ParentVersion, _ParentTGIBlocks); }
            protected override void WriteElement(Stream s, CASEntry element) { element.UnParse(s); }
            #endregion

            public override void Add() { base.Add(new CASEntry(0, handler, _ParentVersion, _ParentTGIBlocks)); }
            public override void Add(CASEntry item) { item.ParentVersion = _ParentVersion; item.ParentTGIBlocks = _ParentTGIBlocks; base.Add(item); }
        }

        public class TGIPair : AHandlerElement, IEquatable<TGIPair>
        {
            const int recommendedApiVersion = 1;
            public uint ParentVersion { get; set; }

            #region Attributes
            TGIBlock txtc1TGI;
            TGIBlock txtc2TGI;
            #endregion

            #region Constructors
            public TGIPair(int APIversion, EventHandler handler, uint parentVersion)
                : base(APIversion, handler)
            {
                this.ParentVersion = parentVersion;
            }
            public TGIPair(int APIversion, EventHandler handler, Stream s, uint parentVersion)
                : base(APIversion, handler)
            {
                this.ParentVersion = parentVersion;
                Parse(s);
            }
            public TGIPair(int APIversion, EventHandler handler, TGIPair basis, uint parentVersion)
                : this(APIversion, handler, basis.txtc1TGI, basis.txtc2TGI, parentVersion) { }
            public TGIPair(int APIversion, EventHandler handler, IResourceKey txtc1TGI, IResourceKey txtc2TGI, uint parentVersion)
                : base(APIversion, handler)
            {
                if (s4pi.Settings.Settings.Checking && parentVersion >= 0x08)
                    throw new InvalidOperationException();

                this.ParentVersion = parentVersion;
                this.txtc1TGI = new TGIBlock(0, handler, txtc1TGI);
                this.txtc2TGI = new TGIBlock(0, handler, txtc2TGI);
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                txtc1TGI = new TGIBlock(0, handler, s);
                txtc2TGI = new TGIBlock(0, handler, s);
            }

            internal void UnParse(Stream s)
            {
                txtc1TGI.UnParse(s);
                txtc2TGI.UnParse(s);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields
            {
                get
                {
                    List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                    res.Remove("ParentVersion");
                    return res;
                }
            }
            #endregion

            #region IEquatable<TGIPair> Members

            public bool Equals(TGIPair other) { return this.txtc1TGI.Equals(other.txtc1TGI) && this.txtc2TGI.Equals(other.txtc2TGI); }
            public override bool Equals(object obj)
            {
                return obj as TGIPair != null ? this.Equals(obj as TGIPair) : false;
            }
            public override int GetHashCode()
            {
                return txtc1TGI.GetHashCode() ^ txtc2TGI.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public IResourceKey TXTC1TGI { get { return txtc1TGI; } set { if (!txtc1TGI.Equals(value)) { txtc1TGI = new TGIBlock(0, handler, value); OnElementChanged(); } } }
            [ElementPriority(1)]
            public IResourceKey TXTC2TGI { get { return txtc2TGI; } set { if (!txtc2TGI.Equals(value)) { txtc2TGI = new TGIBlock(0, handler, value); OnElementChanged(); } } }

            public string Value { get { return "{ " + string.Join("; ", ValueBuilder.Split('\n')) + " }"; } }
            #endregion
        }
        public class TGIPairList : DependentList<TGIPair>
        {
            private uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set { if (_ParentVersion != value) { _ParentVersion = value; foreach (var item in this) item.ParentVersion = _ParentVersion; } }
            }

            #region Constructors
            public TGIPairList(EventHandler handler, uint parentVersion)
                : base(handler, Byte.MaxValue) { _ParentVersion = parentVersion; }
            public TGIPairList(EventHandler handler, Stream s, uint parentVersion)
                : this(null, parentVersion) { elementHandler = handler; Parse(s); this.handler = handler; }
            public TGIPairList(EventHandler handler, IEnumerable<TGIPair> le, uint parentVersion)
                : this(null, parentVersion) { elementHandler = handler; foreach (var t in le) this.Add((TGIPair)t.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return new BinaryReader(s).ReadByte(); }
            protected override void WriteCount(Stream s, int count) { new BinaryWriter(s).Write((Byte)count); }
            protected override TGIPair CreateElement(Stream s) { return new TGIPair(0, elementHandler, s, _ParentVersion); }
            protected override void WriteElement(Stream s, TGIPair element) { element.UnParse(s); }
            #endregion

            public override void Add() { base.Add(new TGIPair(0, handler, _ParentVersion)); }
            public override void Add(TGIPair item) { item.ParentVersion = _ParentVersion; base.Add(item); }
        }

        public class CASV7Entry : AHandlerElement, IEquatable<CASV7Entry>
        {
            const int recommendedApiVersion = 1;

            private uint _ParentVersion;
            public uint ParentVersion { get { return _ParentVersion; } set { if (_ParentVersion != value) { _ParentVersion = value; txtcTGIs.ParentVersion = _ParentVersion; } } }

            #region Attributes
            TGIBlock casPart;
            TGIPairList txtcTGIs;
            ClothingType clothing;
            #endregion

            #region Constructors
            public CASV7Entry(int APIversion, EventHandler handler, uint ParentVersion)
                : base(APIversion, handler)
            {
                _ParentVersion = ParentVersion;
                casPart = null;
                txtcTGIs = null;
            }
            public CASV7Entry(int APIversion, EventHandler handler, Stream s, uint ParentVersion)
                : base(APIversion, handler) { _ParentVersion = ParentVersion; Parse(s); }
            public CASV7Entry(int APIversion, EventHandler handler, CASV7Entry basis, uint ParentVersion)
                : this(APIversion, handler, basis.casPart, basis.txtcTGIs, basis.clothing, ParentVersion) { }
            public CASV7Entry(int APIversion, EventHandler handler, IResourceKey casPart, IEnumerable<TGIPair> ibe, ClothingType clothing, uint ParentVersion)
                : base(APIversion, handler)
            {
                _ParentVersion = ParentVersion;
                this.casPart = new TGIBlock(0, handler, casPart);
                this.txtcTGIs = ibe == null ? null : new TGIPairList(handler, ibe, _ParentVersion);
                this.clothing = clothing;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                casPart = new TGIBlock(0, handler, s);
                txtcTGIs = new TGIPairList(handler, s, _ParentVersion);
                clothing = (ClothingType)(new BinaryReader(s)).ReadUInt32();
            }

            internal void UnParse(Stream s)
            {
                if (casPart == null) casPart = new TGIBlock(0, handler);
                casPart.UnParse(s);
                if (txtcTGIs == null) txtcTGIs = new TGIPairList(handler, _ParentVersion);
                txtcTGIs.UnParse(s);
                (new BinaryWriter(s)).Write((uint)clothing);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields
            {
                get
                {
                    List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                    res.Remove("ParentVersion");
                    return res;
                }
            }
            #endregion

            #region IEquatable<CASV7Entry> Members

            public bool Equals(CASV7Entry other)
            {
                return
                    (this.casPart != null && this.casPart.Equals(other.casPart))
                    && (this.txtcTGIs != null && this.txtcTGIs.Equals(other.txtcTGIs))
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as CASV7Entry != null ? this.Equals(obj as CASV7Entry) : false;
            }

            public override int GetHashCode()
            {
                return casPart.GetHashCode() ^ txtcTGIs.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public IResourceKey CASPart { get { return casPart; } set { if (!casPart.Equals(value)) { casPart = value == null ? null : new TGIBlock(0, handler, value); OnElementChanged(); } } }
            [ElementPriority(2)]
            public TGIPairList TXTCs { get { return txtcTGIs; } set { if (!txtcTGIs.Equals(value)) { txtcTGIs = value == null ? null : new TGIPairList(handler, value, _ParentVersion); OnElementChanged(); } } }
            [ElementPriority(3)]
            public ClothingType Clothing { get { return clothing; } set { if (clothing != value) { clothing = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder; } }
            #endregion
        }
        public class CASV7EntryList : DependentList<CASV7Entry>
        {
            private uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set { if (_ParentVersion != value) { _ParentVersion = value; foreach (var item in this) item.ParentVersion = _ParentVersion; } }
            }

            #region Constructors
            public CASV7EntryList(EventHandler handler, uint parentVersion)
                : base(handler, Byte.MaxValue) { _ParentVersion = parentVersion; }
            public CASV7EntryList(EventHandler handler, Stream s, uint parentVersion)
                : base(handler, Byte.MaxValue) { _ParentVersion = parentVersion; elementHandler = handler; Parse(s); this.handler = handler; }
            public CASV7EntryList(EventHandler handler, IEnumerable<CASV7Entry> le, uint parentVersion)
                : base(handler, Byte.MaxValue) { _ParentVersion = parentVersion; elementHandler = handler; foreach (var t in le) this.Add(t); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return new BinaryReader(s).ReadByte(); }
            protected override void WriteCount(Stream s, int count) { new BinaryWriter(s).Write((byte)count); }
            protected override CASV7Entry CreateElement(Stream s) { return new CASV7Entry(0, elementHandler, s, _ParentVersion); }
            protected override void WriteElement(Stream s, CASV7Entry element) { element.UnParse(s); }
            #endregion

            public override void Add() { base.Add(new CASV7Entry(0, handler, _ParentVersion)); }
            public override void Add(CASV7Entry item) { item.ParentVersion = _ParentVersion; base.Add(item); }
        }

        public class FaceEntry : AHandlerElement, IEquatable<FaceEntry>
        {
            const int recommendedApiVersion = 1;
            public DependentList<TGIBlock> ParentTGIBlocks { get; set; }
            private uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set
                {
                    if (s4pi.Settings.Settings.Checking && _ParentVersion >= 0x15 && value < 0x15 && !faceIndex.IsByteSized())
                        throw new InvalidOperationException();
                    _ParentVersion = value;
                }
            }

            #region Attributes
            short faceIndex;//byte if version < 0x15
            float unknown1;
            #endregion

            #region Constructors
            public FaceEntry(int APIversion, EventHandler handler, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null) : base(APIversion, handler)
            {
                _ParentVersion = parentVersion;
                this.ParentTGIBlocks = ParentTGIBlocks;
            }
            public FaceEntry(int APIversion, EventHandler handler, Stream s, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, parentVersion, ParentTGIBlocks) { Parse(s); }
            public FaceEntry(int APIversion, EventHandler handler, FaceEntry basis, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, basis.faceIndex, basis.unknown1, parentVersion, ParentTGIBlocks) { }
            public FaceEntry(int APIversion, EventHandler handler, byte faceIndex, float unknown1, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, (short)faceIndex, unknown1, parentVersion, ParentTGIBlocks)
            {
                if (s4pi.Settings.Settings.Checking && ParentVersion >= 0x15)
                    throw new InvalidOperationException(String.Format("FaceIndex must be short for version {0}", ParentVersion));
            }
            public FaceEntry(int APIversion, EventHandler handler, short faceIndex, float unknown1, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, parentVersion, ParentTGIBlocks)
            {
                if (s4pi.Settings.Settings.Checking && parentVersion < 0x15 && faceIndex.IsByteSized())
                    throw new InvalidOperationException();

                this.faceIndex = faceIndex;
                this.unknown1 = unknown1;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                if (_ParentVersion >= 0x00000015)
                {
                    faceIndex = r.ReadInt16();
                }
                else
                {
                    faceIndex = r.ReadByte();
                }
                unknown1 = r.ReadSingle();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                if (_ParentVersion >= 0x00000015)
                {
                    w.Write(faceIndex);
                }
                else
                {
                    w.Write((byte)faceIndex);
                }
                w.Write(unknown1);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields
            {
                get
                {
                    List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                    res.Remove("ParentVersion");
                    res.Remove("ParentTGIBlocks");
                    return res;
                }
            }
            #endregion

            #region IEquatable<FaceEntry> Members

            public bool Equals(FaceEntry other)
            {
                return
                    this.faceIndex == other.faceIndex
                    && this.unknown1 == other.unknown1
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as FaceEntry != null ? this.Equals(obj as FaceEntry) : false;
            }

            public override int GetHashCode()
            {
                return faceIndex.GetHashCode() ^ unknown1.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1), TGIBlockListContentField("ParentTGIBlocks")]
            public short FaceIndex
            {
                get { return faceIndex; }
                set
                {
                    if (_ParentVersion < 0x00000015 && !value.IsByteSized()) throw new InvalidOperationException();
                    if (faceIndex != value) { faceIndex = value; OnElementChanged(); }
                }
            }
            [ElementPriority(2)]
            public float Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnElementChanged(); } } }

            public string Value { get { return "{ " + string.Join("; ", ValueBuilder.Split('\n')) + " }"; } }
            #endregion
        }
        public class FaceEntryList : DependentList<FaceEntry>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (var i in this) i.ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            private uint _ParentVersion;
            public uint ParentVersion
            {
                get { return _ParentVersion; }
                set { if (_ParentVersion != value) { _ParentVersion = value; foreach (var item in this) item.ParentVersion = _ParentVersion; } }
            }

            #region Constructors
            public FaceEntryList(EventHandler handler, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler, Byte.MaxValue) { _ParentVersion = parentVersion; _ParentTGIBlocks = ParentTGIBlocks; }
            public FaceEntryList(EventHandler handler, Stream s, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null) 
                : this(null, parentVersion, ParentTGIBlocks) { elementHandler = handler; Parse(s); this.handler = handler; }
            public FaceEntryList(EventHandler handler, IEnumerable<FaceEntry> le, uint parentVersion, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, parentVersion, ParentTGIBlocks) { elementHandler = handler; foreach (var t in le) this.Add((FaceEntry)t.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return new BinaryReader(s).ReadByte(); }
            protected override void WriteCount(Stream s, int count) { new BinaryWriter(s).Write((Byte)count); }
            protected override FaceEntry CreateElement(Stream s) { return new FaceEntry(0, elementHandler, s, _ParentVersion, _ParentTGIBlocks); }
            protected override void WriteElement(Stream s, FaceEntry element) { element.UnParse(s); }
            #endregion

            public override void Add() { base.Add(new FaceEntry(0, handler, _ParentVersion, _ParentTGIBlocks)); }
            public override void Add(FaceEntry item)
            {
                item.ParentVersion = _ParentVersion;
                item.ParentTGIBlocks = _ParentTGIBlocks;
                base.Add(item);
            }
        }
        #endregion

        #region AResource
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields
        {
            get
            {
                List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                if (version >= 0x00000008)
                {
                    res.RemoveAll(x => x.EndsWith("V7"));
                    if (version >= 0x00000009)
                    {
                        res.Remove("HairToneIndex");
                        res.Remove("Unknown11");
                        res.Remove("Unknown13");
                        if (version >= 0x0000000E)
                        {
                            res.Remove("Unknown12");
                            if (version >= 0x00000010)
                            {
                                res.Remove("Unknown3");
                            }
                        }
                    }
                    if (version < 0x00000014)
                    {
                        res.Remove("FurMap");
                        if (version < 0x00000013)
                        {
                            res.Remove("NumCurls");
                            res.Remove("CurlPixelRadius");
                            if (version < 0x00000012)
                            {
                                res.Remove("BreastSlider");
                                if (version < 0x00000011)
                                {
                                    res.Remove("MuscleSlider");
                                    if (version < 0x00000010)
                                    {
                                        res.Remove("XmlEntries");
                                        res.Remove("Unknown1");
                                        res.Remove("Unknown2");
                                        if (version < 0x0000000E)
                                        {
                                            res.Remove("HairBaseColour");
                                            res.Remove("HairHaloHighColour");
                                            res.Remove("HairHaloLowColour");
                                            if (version < 0x00000009)
                                            {
                                                res.Remove("Unknown3");
                                                res.Remove("Unknown6");
                                                res.Remove("Handedness");
                                                res.Remove("Unknown12");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    res.RemoveAll(x => !x.EndsWith("V7") && !x.Equals("Version") && !x.Equals("Value"));
                }
                return res;
            }
        }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; xmlEntries = new XMLEntryList(OnResourceChanged, xmlEntries, version); OnResourceChanged(this, EventArgs.Empty); } } }

        // Version >= 0x08
        [ElementPriority(2)]
        public XMLEntryList XmlEntries
        {
            get { if (version < 0x00000010) throw new InvalidOperationException(); return xmlEntries; }
            set { if (version < 0x00000010) throw new InvalidOperationException(); if (!xmlEntries.Equals(value)) { xmlEntries = value == null ? null : new XMLEntryList(OnResourceChanged, value, version); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(3)]
        public int Unknown1
        {
            get { if (version < 0x00000010) throw new InvalidOperationException(); return unknown1; }
            set { if (version < 0x00000010) throw new InvalidOperationException(); if (unknown1 != value) { unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(4)]
        public int Unknown2
        {
            get { if (version < 0x00000010) throw new InvalidOperationException(); return unknown2; }
            set { if (version < 0x00000010) throw new InvalidOperationException(); if (unknown2 != value) { unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(2)]
        public UIntList Unknown3
        {
            get { if (version < 0x00000009 || version > 0x00000010) throw new InvalidOperationException(); return unknown3; }
            set { if (version < 0x00000009 || version > 0x00000010) throw new InvalidOperationException(); if (!unknown3.Equals(value)) { unknown3 = new UIntList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(5)]
        public float HeavyWeightSlider
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return heavyWeightSlider; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (heavyWeightSlider != value) { heavyWeightSlider = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public float StrengthSlider
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return strengthSlider; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (strengthSlider != value) { strengthSlider = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(7)]
        public float SlimWeightSlider
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return slimWeightSlider; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (slimWeightSlider != value) { slimWeightSlider = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(8)]
        public uint Unknown6
        {
            get { if (version < 0x00000009) throw new InvalidOperationException(); return unknown6; }
            set { if (version < 0x00000009) throw new InvalidOperationException(); if (unknown6 != value) { unknown6 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(9)]
        public AgeFlags Age
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return age.Age; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (age.Age != value) { age.Age = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(10)]
        public GenderFlags Gender
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return gender.Gender; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (gender.Gender != value) { gender.Gender = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(11)]
        public SpeciesType Species
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return species.Species; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (species.Species != value) { species.Species = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(12)]
        public HandednessFlags Handedness
        {
            get { if (version < 0x00000009) throw new InvalidOperationException(); return handedness.Handedness; }
            set { if (version < 0x00000009) throw new InvalidOperationException(); if (handedness.Handedness != value) { handedness.Handedness = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(13), TGIBlockListContentField("TGIBlocks")]
        public short SkinToneIndex
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return skinToneIndex; }
            set
            {
                if (version < 0x00000008 || (version < 0x00000015 && !value.IsByteSized())) throw new InvalidOperationException();
                if (skinToneIndex != value) { skinToneIndex = value; OnResourceChanged(this, EventArgs.Empty); }
            }
        }
        [ElementPriority(14), TGIBlockListContentField("TGIBlocks")]
        public byte HairToneIndex
        {
            get { if (version != 0x00000008) throw new InvalidOperationException(); return hairToneIndex; }
            set { if (version != 0x00000008) throw new InvalidOperationException(); if (hairToneIndex != value) { hairToneIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(15)]
        public float EyelashSlider
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return eyelashSlider; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (eyelashSlider != value) { eyelashSlider = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(16)]
        public float MuscleSlider
        {
            get { if (version < 0x00000011) throw new InvalidOperationException(); return muscleSlider; }
            set { if (version < 0x00000011) throw new InvalidOperationException(); if (muscleSlider != value) { muscleSlider = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(17)]
        public float BreastSlider
        {
            get { if (version < 0x00000012) throw new InvalidOperationException(); return breastSlider; }
            set { if (version < 0x00000012) throw new InvalidOperationException(); if (breastSlider != value) { breastSlider = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(18)]
        public UInt32 HairBaseColour
        {
            get { if (version < 0x0000000E) throw new InvalidOperationException(); return hairBaseColour; }
            set { if (version < 0x0000000E) throw new InvalidOperationException(); if (!hairBaseColour.Equals(value)) { hairBaseColour = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(19)]
        public UInt32 HairHaloHighColour
        {
            get { if (version < 0x0000000E) throw new InvalidOperationException(); return hairHaloHighColour; }
            set { if (version < 0x0000000E) throw new InvalidOperationException(); if (!hairHaloHighColour.Equals(value)) { hairHaloHighColour = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(20)]
        public UInt32 HairHaloLowColour
        {
            get { if (version < 0x0000000E) throw new InvalidOperationException(); return hairHaloLowColour; }
            set { if (version < 0x0000000E) throw new InvalidOperationException(); if (!hairHaloLowColour.Equals(value)) { hairHaloLowColour = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(21)]
        public float NumCurls
        {
            get { if (version < 0x00000013) throw new InvalidOperationException(); return numCurls; }
            set { if (version < 0x00000013) throw new InvalidOperationException(); if (numCurls != value) { numCurls = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(22)]
        public float CurlPixelRadius
        {
            get { if (version < 0x00000013) throw new InvalidOperationException(); return curlPixelRadius; }
            set { if (version < 0x00000013) throw new InvalidOperationException(); if (curlPixelRadius != value) { curlPixelRadius = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(23)]
        public IResourceKey FurMap
        {
            get { if (version < 0x00000014) throw new InvalidOperationException(); return furMap; }
            set { if (version < 0x00000014) throw new InvalidOperationException(); if (!furMap.Equals(value)) { furMap = new TGIBlock(requestedApiVersion, OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(24)]
        public uint Unknown11
        {
            get { if (version != 0x00000008) throw new InvalidOperationException(); return unknown11; }
            set { if (version != 0x00000008) throw new InvalidOperationException(); if (unknown11 != value) { unknown11 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(24)]
        public byte Unknown12
        {
            get { if (version < 0x00000009 || version > 0x0000000D) throw new InvalidOperationException(); return unknown12; }
            set { if (version < 0x00000009 || version > 0x0000000D) throw new InvalidOperationException(); if (unknown12 != value) { unknown12 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(24)]
        public CASEntryList CASPEntries
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return caspEntries; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (!caspEntries.Equals(value)) { caspEntries = value == null ? null : new CASEntryList(OnResourceChanged, value, version, tgiBlocks); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(25)]
        public FaceEntryList FACEEntries
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return faceEntries; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (!faceEntries.Equals(value)) { faceEntries = value == null ? null : new FaceEntryList(OnResourceChanged, value, version, tgiBlocks); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(26)]
        public uint Unknown13
        {
            get { if (version != 0x00000008) throw new InvalidOperationException(); return unknown13; }
            set { if (version != 0x00000008) throw new InvalidOperationException(); if (unknown13 != value) { unknown13 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }

        public CountedTGIBlockList TGIBlocks
        {
            get { if (version < 0x00000008) throw new InvalidOperationException(); return tgiBlocks; }
            set { if (version < 0x00000008) throw new InvalidOperationException(); if (!tgiBlocks.Equals(value)) { tgiBlocks = value == null ? null : new CountedTGIBlockList(OnResourceChanged, "IGT", value); caspEntries.ParentTGIBlocks = tgiBlocks; OnResourceChanged(this, EventArgs.Empty); } }
        }

        // Version < 0x08
        [ElementPriority(2)]
        public uint Unknown1V7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return unknown1v7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (unknown1v7 != value) { unknown1v7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(3)]
        public uint Unknown2V7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return unknown2v7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (unknown2v7 != value) { unknown2v7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(4)]
        public uint Unknown3V7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return unknown3v7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (unknown3v7 != value) { unknown3v7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(5)]
        public AgeFlags AgeV7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return agev7.Age; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (agev7.Age != value) { agev7.Age = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(6)]
        public GenderFlags GenderV7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return genderv7.Gender; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (genderv7.Gender != value) { genderv7.Gender = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(7)]
        public uint Unknown4V7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return unknown4v7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (unknown4v7 != value) { unknown4v7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(8)]
        public IResourceKey SkinToneV7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return skinTonev7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (!skinTonev7.Equals(value)) { skinTonev7 = new TGIBlock(requestedApiVersion, OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(9)]
        public IResourceKey HairToneV7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return hairTonev7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (!hairTonev7.Equals(value)) { hairTonev7 = new TGIBlock(requestedApiVersion, OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(10)]
        public uint Unknown5V7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return unknown5v7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (unknown5v7 != value) { unknown5v7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(11)]
        public uint Unknown6V7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return unknown6v7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (unknown6v7 != value) { unknown6v7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(12)]
        public CASV7EntryList CASPEntriesV7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return caspEntriesv7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (!caspEntriesv7.Equals(value)) { caspEntriesv7 = value == null ? null : new CASV7EntryList(OnResourceChanged, value, version); OnResourceChanged(this, EventArgs.Empty); } }
        }
        public ushort Unknown7V7
        {
            get { if (version >= 0x00000008) throw new InvalidOperationException(); return unknown7v7; }
            set { if (version >= 0x00000008) throw new InvalidOperationException(); if (unknown7v7 != value) { unknown7v7 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for SimOutfitResource wrapper
    /// </summary>
    public class SimOutfitResourceHandler : AResourceHandler
    {
        public SimOutfitResourceHandler()
        {
            this.Add(typeof(SimOutfitResource), new List<string>(new string[] { "0x025ED6F4", "0xDEA2951C", }));
        }
    }

    static class UInt16Extensions
    {
        public static bool IsByteSized(this short value) { return (value & 0xFF) == value; }
    }
}
