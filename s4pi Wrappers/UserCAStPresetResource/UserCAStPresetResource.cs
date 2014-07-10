using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace UserCAStPreset
{
    public class UserCAStPresetResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint version = 3;
        uint unknown1; // base preset?
        uint unknown2;
        uint unknown3;
        PresetList presets;
        #endregion

        public UserCAStPresetResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        private void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            version = r.ReadUInt32();
            unknown1 = r.ReadUInt32();
            unknown2 = r.ReadUInt32();
            unknown3 = r.ReadUInt32();
            presets = new PresetList(OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(version);
            w.Write(unknown1);
            w.Write(unknown2);
            w.Write(unknown3);
            if (presets == null) presets = new PresetList(OnResourceChanged);
            presets.UnParse(s);
            return s;
        }
        #endregion

        #region Sub-types
        public class Preset : AHandlerElement, IEquatable<Preset>
        {
            const int recommendedApiVersion = 1;
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

            #region Attributes
            string xml;
            byte unknown1;
            byte unknown2;
            uint unknown3;
            byte unknown4;
            byte unknown5;
            byte unknown6;
            #endregion

            #region Constructors
            public Preset(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public Preset(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public Preset(int APIversion, EventHandler handler, Preset basis)
                : this(APIversion, handler, basis.xml, basis.unknown1, basis.unknown2, basis.unknown3, basis.unknown4, basis.unknown5, basis.unknown6) { }
            public Preset(int APIversion, EventHandler handler, string xml, byte unknown1, byte unknown2, uint unknown3, byte unknown4, byte unknown5, byte unknown6)
                : base(APIversion, handler)
            {
                this.xml = xml;
                this.unknown1 = unknown1;
                this.unknown2 = unknown2;
                this.unknown3 = unknown3;
                this.unknown4 = unknown4;
                this.unknown5 = unknown5;
                this.unknown6 = unknown6;
            }
            #endregion

            #region Data I/O
            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                xml = System.Text.Encoding.Unicode.GetString(r.ReadBytes(r.ReadInt32() * 2));
                unknown1 = r.ReadByte();
                unknown2 = r.ReadByte();
                unknown3 = r.ReadUInt32();
                unknown4 = r.ReadByte();
                unknown5 = r.ReadByte();
                unknown6 = r.ReadByte();
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(xml.Length);
                w.Write(System.Text.Encoding.Unicode.GetBytes(xml));
                w.Write(unknown1);
                w.Write(unknown2);
                w.Write(unknown3);
                w.Write(unknown4);
                w.Write(unknown5);
                w.Write(unknown6);
            }
            #endregion

            #region IEquatable<Preset> Members

            public bool Equals(Preset other)
            {
                return
                    this.xml.Equals(other.xml)
                    && this.unknown1.Equals(other.unknown1)
                    && this.unknown2.Equals(other.unknown2)
                    && this.unknown3.Equals(other.unknown3)
                    && this.unknown4.Equals(other.unknown4)
                    && this.unknown5.Equals(other.unknown5)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as Preset != null ? this.Equals(obj as Preset) : false;
            }
            public override int GetHashCode()
            {
                return
                    this.xml.GetHashCode()
                    ^ this.unknown1.GetHashCode()
                    ^ this.unknown2.GetHashCode()
                    ^ this.unknown3.GetHashCode()
                    ^ this.unknown4.GetHashCode()
                    ^ this.unknown5.GetHashCode()
                    ;
            }

            #endregion

            #region Content Fields
            //[ElementPriority(1)]
            //public string Xml { get { return xml; } set { if (xml != value) { xml = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public byte Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public byte Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public uint Unknown3 { get { return unknown3; } set { if (unknown3 != value) { unknown3 = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public byte Unknown4 { get { return unknown4; } set { if (unknown4 != value) { unknown4 = value; OnElementChanged(); } } }
            [ElementPriority(6)]
            public byte Unknown5 { get { return unknown5; } set { if (unknown5 != value) { unknown5 = value; OnElementChanged(); } } }
            [ElementPriority(7)]
            public byte Unknown6 { get { return unknown6; } set { if (unknown6 != value) { unknown6 = value; OnElementChanged(); } } }

            [ElementPriority(99)]
            public TextReader XmlFile
            {
                get { return new StringReader(xml); }
                set
                {
                    string temp = value.ReadToEnd();
                    if (xml != temp) { xml = temp; OnElementChanged(); }
                }
            }

            public string Value
            {
                get
                {
                    return "Xml: " + (xml.Length > 160 ? xml.Substring(0, 157) + "..." : xml)
                        + "\nUnknown1: " + this["Unknown1"]
                        + "\nUnknown2: " + this["Unknown2"]
                        + "\nUnknown3: " + this["Unknown3"]
                        + "\nUnknown4: " + this["Unknown4"]
                        + "\nUnknown5: " + this["Unknown5"]
                        + "\nUnknown6: " + this["Unknown6"]
                        ;
                }
            }
            #endregion
        }
        public class PresetList : DependentList<Preset>
        {
            #region Constructors
            public PresetList(EventHandler handler) : base(handler) { }
            public PresetList(EventHandler handler, Stream s) : base(handler, s) { }
            public PresetList(EventHandler handler, IEnumerable<Preset> le) : base(handler, le) { }
            #endregion

            #region Data I/O
            protected override Preset CreateElement(Stream s) { return new Preset(0, elementHandler, s); }
            protected override void WriteElement(Stream s, Preset element) { element.UnParse(s); }
            #endregion
        }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public uint Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public uint Unknown3 { get { return unknown3; } set { if (unknown3 != value) { unknown3 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(5)]
        public PresetList Presets { get { return presets; } set { if (!presets.Equals(value)) { presets = value == null ? null : new PresetList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for UserCAStPreset wrapper
    /// </summary>
    public class UserCAStPresetResourceHandler : AResourceHandler
    {
        public UserCAStPresetResourceHandler()
        {
            this.Add(typeof(UserCAStPresetResource), new List<string>(new string[] { "0x0591B1AF" }));
        }
    }
}
