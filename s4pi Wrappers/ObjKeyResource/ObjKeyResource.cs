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

namespace ObjKeyResource
{
    /// <summary>
    /// A resource wrapper that understands Catalog Entry resources
    /// </summary>
    public class ObjKeyResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint format = 7;
        ComponentList components;
        ComponentDataList componentData;
        byte unknown1;
        TGIBlockList tgiBlocks;
        #endregion

        public ObjKeyResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            long tgiPosn, tgiSize;
            BinaryReader r = new BinaryReader(s);

            format = r.ReadUInt32();
            tgiPosn = r.ReadUInt32() + s.Position;
            tgiSize = r.ReadUInt32();

            components = new ComponentList(OnResourceChanged, s);
            componentData = new ComponentDataList(OnResourceChanged, s);
            unknown1 = r.ReadByte();

            tgiBlocks = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            componentData.ParentTGIBlocks = tgiBlocks;
        }

        protected override Stream UnParse()
        {
            long posn;
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);

            w.Write(format);

            posn = s.Position;
            w.Write((uint)0);
            w.Write((uint)0);

            if (components == null) components = new ComponentList(OnResourceChanged);
            components.UnParse(s);

            if (tgiBlocks == null) tgiBlocks = new TGIBlockList(OnResourceChanged);
            if (componentData == null) componentData = new ComponentDataList(OnResourceChanged, tgiBlocks);
            componentData.UnParse(s);

            w.Write(unknown1);

            tgiBlocks.UnParse(s, posn);

            s.Flush();

            return s;
        }
        #endregion

        #region Sub-classes
        public enum Component : uint
        {
            Animation = 0xee17c6ad,
            Effect = 0x80d91e9e,
            Footprint = 0xc807312a,
            Lighting = 0xda6c50fd,
            Location = 0x461922c8,
            LotObject = 0x6693c8b3,
            Model = 0x2954e734,
            Physics = 0x1a8feb14,
            Sacs = 0x3ae9a8e7,
            Script = 0x23177498,
            Sim = 0x22706efa,
            Slot = 0x2ef1e401,
            Steering = 0x61bd317c,
            Transform = 0x54cb7ebb,
            Tree = 0xc602cd31,
            VisualState = 0x50b3d17c,
        }

        static Dictionary<Component, string> ComponentDataMap;

        public class ComponentElement : AHandlerElement, IEquatable<ComponentElement>
        {
            Component element;
            public ComponentElement(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public ComponentElement(int APIversion, EventHandler handler, ComponentElement basis) : this(APIversion, handler, basis.element) { }
            public ComponentElement(int APIversion, EventHandler handler, uint value) : base(APIversion, handler) { element = (Component)value; }
            public ComponentElement(int APIversion, EventHandler handler, Component element) : base(APIversion, handler) { this.element = element; }

            static ComponentElement()
            {
                ComponentDataMap = new Dictionary<Component, string>();
                ComponentDataMap.Add(Component.Sim, "simOutfitKey");
                ComponentDataMap.Add(Component.Script, "scriptClass");
                ComponentDataMap.Add(Component.Model, "modelKey");
                ComponentDataMap.Add(Component.Steering, "steeringInstance");
                ComponentDataMap.Add(Component.Tree, "modelKey");
                ComponentDataMap.Add(Component.Footprint, "footprintKey");
            }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return 1; } }

            public override List<string> ContentFields { get { return AApiVersionedFields.GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IEquatable<ComponentElement> Members

            public bool Equals(ComponentElement other) { return ((uint)element).Equals((uint)other.element); }

            public override bool Equals(object obj)
            {
                return obj as ComponentElement != null ? this.Equals(obj as ComponentElement) : false;
            }

            public override int GetHashCode()
            {
                return element.GetHashCode();
            }

            #endregion

            public TypedValue Data(ComponentDataList list, TGIBlockList tgiBlocks)
            {
                if (!ComponentDataMap.ContainsKey(element)) return null;
                if (!list.ContainsKey(ComponentDataMap[element])) return null;
                ComponentDataType cd = list[ComponentDataMap[element]];
                System.Reflection.PropertyInfo pi = cd.GetType().GetProperty("Data");
                if (pi == null || !pi.CanRead) return null;
                if (element == Component.Footprint || element == Component.Model || element == Component.Tree)
                    return new TypedValue(typeof(TGIBlock), tgiBlocks[(int)pi.GetValue(cd, null)], "X");
                else
                    return new TypedValue(pi.PropertyType, pi.GetValue(cd, null), "X");
            }

            public Component Element { get { return element; } set { if (element != value) { element = value; OnElementChanged(); } } }
            public string Value { get { return "0x" + ((uint)element).ToString("X8") + " (" + (Enum.IsDefined(typeof(Component), element) ? element + "" : "undefined") + ")"; } }
        }

        public class ComponentList : DependentList<ComponentElement>
        {
            #region Constructors
            public ComponentList(EventHandler handler) : base(handler, Byte.MaxValue) { }
            public ComponentList(EventHandler handler, IEnumerable<ComponentElement> luint) : base(handler, luint, Byte.MaxValue) { }
            internal ComponentList(EventHandler handler, Stream s) : base(handler, s, Byte.MaxValue) { }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return (new BinaryReader(s)).ReadByte(); }
            protected override ComponentElement CreateElement(Stream s) { return new ComponentElement(0, elementHandler, (new BinaryReader(s)).ReadUInt32()); }

            protected override void WriteCount(Stream s, int count) { (new BinaryWriter(s)).Write((byte)count); }
            protected override void WriteElement(Stream s, ComponentElement element) { (new BinaryWriter(s)).Write((uint)element.Element); }
            #endregion

            public bool HasComponent(Component component) { return Find(component) != null; }

            public ComponentElement Find(Component component)
            {
                foreach (ComponentElement ce in this)
                    if (ce.Element == component) return ce;
                return null;
            }
        }

        public abstract class ComponentDataType : AHandlerElement, IComparable<ComponentDataType>, IEqualityComparer<ComponentDataType>, IEquatable<ComponentDataType>
        {
            const int recommendedApiVersion = 1;

            #region Attributes
            protected string key;
            protected byte controlCode;
            #endregion

            #region Constructors
            protected ComponentDataType(int APIversion, EventHandler handler, string key, byte controlCode)
                : base(APIversion, handler) { this.key = key; this.controlCode = controlCode; }

            public static ComponentDataType CreateComponentData(int APIversion, EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks)
            {
                BinaryReader r = new BinaryReader(s);
                string key = new string(r.ReadChars(r.ReadInt32()));
                byte controlCode = r.ReadByte();
                switch (controlCode)
                {
                    case 0x00: return new CDTString(APIversion, handler, key, controlCode, new string(r.ReadChars(r.ReadInt32())));
                    case 0x01: return new CDTResourceKey(APIversion, handler, key, controlCode, r.ReadInt32(), ParentTGIBlocks);
                    case 0x02: return new CDTAssetResourceName(APIversion, handler, key, controlCode, r.ReadInt32(), ParentTGIBlocks);
                    case 0x03: return new CDTSteeringInstance(APIversion, handler, key, controlCode, new string(r.ReadChars(r.ReadInt32())));
                    case 0x04: return new CDTUInt32(APIversion, handler, key, controlCode, r.ReadUInt32());
                    default:
                        if (checking) throw new InvalidDataException(String.Format("Unknown control code 0x{0:X2} at position 0x{1:X8}", controlCode, s.Position));
                        return null;
                }
            }
            #endregion

            #region Data I/O
            internal virtual void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(key.Length);
                w.Write(key.ToCharArray());
                w.Write(controlCode);
            }
            #endregion

            #region AHandlerElement Members

            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

            public override List<string> ContentFields { get { return AApiVersionedFields.GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            #region IComparable<Key> Members

            public abstract int CompareTo(ComponentDataType other);

            #endregion

            #region IEqualityComparer<Key> Members

            public bool Equals(ComponentDataType x, ComponentDataType y) { return x.Equals(y); }

            public abstract int GetHashCode(ComponentDataType obj);

            #endregion

            #region IEquatable<Key> Members

            public bool Equals(ComponentDataType other) { return this.CompareTo(other) == 0; }

            public override bool Equals(object obj)
            {
                return obj as ComponentDataType != null ? this.Equals(obj as ComponentDataType) : false;
            }

            public override int GetHashCode()
            {
                return key.GetHashCode() ^ controlCode.GetHashCode();
            }

            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public string Key { get { return key; } set { if (key != value) { key = value; OnElementChanged(); } } }

            public virtual string Value { get { return this.GetType().Name + " -- Key: \"" + key + "\"; Control code: 0x" + controlCode.ToString("X2"); } }
            #endregion
        }
        public class CDTString : ComponentDataType
        {
            #region Attributes
            protected string data;
            #endregion

            #region Constructors
            public CDTString(int APIversion, EventHandler handler) : this(APIversion, handler, "CDTString-Key", (byte)0x00, "Value") { }
            public CDTString(int APIversion, EventHandler handler, CDTString basis) : this(APIversion, handler, basis.key, basis.controlCode, basis.data) { }
            public CDTString(int APIversion, EventHandler handler, string key, byte controlCode, string data) : base(APIversion, handler, key, controlCode) { this.data = data; }
            #endregion

            #region Data I/O
            internal override void UnParse(Stream s)
            {
                base.UnParse(s);
                BinaryWriter w = new BinaryWriter(s);
                w.Write(data.Length);
                w.Write(data.ToCharArray());
            }
            #endregion

            public override int CompareTo(ComponentDataType other)
            {
                if (this.GetType() != other.GetType()) return -1;
                CDTString oth = (CDTString)other;
                int res = key.CompareTo(oth.key); if (res != 0) return res;
                res = controlCode.CompareTo(oth.controlCode); if (res != 0) return res;
                return data.CompareTo(oth.data);
            }

            public override int GetHashCode(ComponentDataType obj) { return key.GetHashCode() ^ controlCode ^ data.GetHashCode(); }

            [ElementPriority(2)]
            public string Data { get { return data; } set { if (data != value) { data = value; OnElementChanged(); } } }

            public override string Value { get { return base.Value + "; Data: " + "\"" + data + "\""; } }
        }
        public class CDTResourceKey : ComponentDataType
        {
            public DependentList<TGIBlock> ParentTGIBlocks { get; set; }
            public override List<string> ContentFields { get { List<string> res = base.ContentFields; res.Remove("ParentTGIBlocks"); return res; } }

            #region Attributes
            protected int data;
            #endregion

            #region Constructors
            public CDTResourceKey(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, "CDTResourceKey-Key", (byte)0x01, (Int32)0) { this.ParentTGIBlocks = ParentTGIBlocks; }
            public CDTResourceKey(int APIversion, EventHandler handler, CDTResourceKey basis, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, basis.key, basis.controlCode, basis.data, ParentTGIBlocks ?? basis.ParentTGIBlocks) { }
            public CDTResourceKey(int APIversion, EventHandler handler, string key, byte controlCode, int data, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler, key, controlCode) { this.ParentTGIBlocks = ParentTGIBlocks; this.data = data; }
            #endregion

            #region Data I/O
            internal override void UnParse(Stream s)
            {
                base.UnParse(s);
                new BinaryWriter(s).Write(data);
            }
            #endregion

            public override int CompareTo(ComponentDataType other)
            {
                if (this.GetType() != other.GetType()) return -1;
                CDTResourceKey oth = (CDTResourceKey)other;
                int res = key.CompareTo(oth.key); if (res != 0) return res;
                res = controlCode.CompareTo(oth.controlCode); if (res != 0) return res;
                return data.CompareTo(oth.data);
            }

            public override int GetHashCode(ComponentDataType obj) { return key.GetHashCode() ^ controlCode ^ data; }

            [ElementPriority(2), TGIBlockListContentField("ParentTGIBlocks")]
            public int Data { get { return data; } set { if (data != value) { data = value; OnElementChanged(); } } }

            public override string Value { get { return base.Value + "; Data: " + "0x" + data.ToString("X8") + " (" + (ParentTGIBlocks == null ? "unknown" : ParentTGIBlocks[data]) + ")"; } }
        }
        public class CDTAssetResourceName : CDTResourceKey
        {
            public CDTAssetResourceName(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler, "CDTAssetResourceName-Key", (byte)0x02, (Int32)0, ParentTGIBlocks) { }
            public CDTAssetResourceName(int APIversion, EventHandler handler, CDTAssetResourceName basis, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler, basis, ParentTGIBlocks ?? basis.ParentTGIBlocks) { }
            public CDTAssetResourceName(int APIversion, EventHandler handler, string key, byte controlCode, int data, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler, key, controlCode, data, ParentTGIBlocks) { }
        }
        public class CDTSteeringInstance : CDTString
        {
            #region Constructors
            public CDTSteeringInstance(int APIversion, EventHandler handler) : base(APIversion, handler, "CDTSteeringInstance-Key", (byte)0x03, "Value") { }
            public CDTSteeringInstance(int APIversion, EventHandler handler, CDTSteeringInstance basis) : base(APIversion, handler, basis) { }
            public CDTSteeringInstance(int APIversion, EventHandler handler, string key, byte controlCode, string data) : base(APIversion, handler, key, controlCode, data) { }
            #endregion
        }
        public class CDTUInt32 : ComponentDataType
        {
            #region Attributes
            uint data;
            #endregion

            #region Constructors
            public CDTUInt32(int APIversion, EventHandler handler) : this(APIversion, handler, "CDTUInt32-Key", (byte)0x04, (UInt32)0) { }
            public CDTUInt32(int APIversion, EventHandler handler, CDTUInt32 basis) : this(APIversion, handler, basis.key, basis.controlCode, basis.data) { }
            public CDTUInt32(int APIversion, EventHandler handler, string key, byte controlCode, uint data) : base(APIversion, handler, key, controlCode) { this.data = data; }
            #endregion

            #region Data I/O
            internal override void UnParse(Stream s)
            {
                base.UnParse(s);
                new BinaryWriter(s).Write(data);
            }
            #endregion

            public override int CompareTo(ComponentDataType other)
            {
                if (this.GetType() != other.GetType()) return -1;
                CDTUInt32 oth = (CDTUInt32)other;
                int res = key.CompareTo(oth.key); if (res != 0) return res;
                res = controlCode.CompareTo(oth.controlCode); if (res != 0) return res;
                return data.CompareTo(oth.data);
            }

            public override int GetHashCode(ComponentDataType obj) { return (int)(key.GetHashCode() ^ controlCode ^ data); }

            [ElementPriority(2)]
            public uint Data { get { return data; } set { if (data != value) { data = value; OnElementChanged(); } } }

            public override string Value { get { return base.Value + "; Data: " + "0x" + data.ToString("X8"); } }
        }

        public class ComponentDataList : DependentList<ComponentDataType>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (var i in this.FindAll(e => e is CDTResourceKey)) (i as CDTResourceKey).ParentTGIBlocks = _ParentTGIBlocks; } }
            }

            #region Constructors
            public ComponentDataList(EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(handler, Byte.MaxValue) { _ParentTGIBlocks = ParentTGIBlocks; }
            internal ComponentDataList(EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; Parse(s); this.handler = handler; }
            public ComponentDataList(EventHandler handler, IEnumerable<ComponentDataType> lcdt, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; foreach (var t in lcdt) this.Add((ComponentDataType)t.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return (new BinaryReader(s)).ReadByte(); }
            protected override ComponentDataType CreateElement(Stream s) { return ComponentDataType.CreateComponentData(0, elementHandler, s, _ParentTGIBlocks); }

            protected override void WriteCount(Stream s, int count) { (new BinaryWriter(s)).Write((byte)count); }
            protected override void WriteElement(Stream s, ComponentDataType element) { element.UnParse(s); }
            #endregion

            public bool ContainsKey(string key) { return Find(x => x.Key.Equals(key)) != null; }

            public ComponentDataType this[string key]
            {
                get
                {
                    ComponentDataType cd = this.Find(x => x.Key.Equals(key));
                    if (cd != null) return cd;
                    throw new KeyNotFoundException();
                }
                set { this[IndexOf(this[key])] = value; }
            }

            public override void Add(ComponentDataType item)
            {
                if (item is CDTResourceKey) (item as CDTResourceKey).ParentTGIBlocks = _ParentTGIBlocks;
                else if (item is CDTAssetResourceName) (item as CDTAssetResourceName).ParentTGIBlocks = _ParentTGIBlocks;
                base.Add(item);
            }
            public override void Add(Type elementType)
            {
                if (elementType.IsAbstract)
                    throw new ArgumentException("Must pass a concrete element type.", "elementType");

                if (!typeof(ComponentDataType).IsAssignableFrom(elementType))
                    throw new ArgumentException("The element type must belong to the generic type of the list.", "elementType");

                ComponentDataType newElement;
                if (elementType == typeof(CDTResourceKey))
                    newElement = new CDTResourceKey(0, elementHandler, _ParentTGIBlocks);
                else if (elementType == typeof(CDTAssetResourceName))
                    newElement = new CDTAssetResourceName(0, elementHandler, _ParentTGIBlocks);
                else
                    newElement = Activator.CreateInstance(elementType, new object[] { (int)0, elementHandler, }) as ComponentDataType;
                base.Add(newElement);
            }
        }

        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public uint Format { get { return format; } set { if (format != value) { format = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public ComponentList Components { get { return components; } set { if (components != value) { components = value == null ? null : new ComponentList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public ComponentDataList ComponentData { get { return componentData; } set { if (componentData != value) { componentData = value == null ? null : new ComponentDataList(OnResourceChanged, value, tgiBlocks); OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(4)]
        public byte Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(5)]
        public TGIBlockList TGIBlocks { get { return tgiBlocks; } set { if (tgiBlocks != value) { tgiBlocks = value == null ? null : new TGIBlockList(OnResourceChanged, value); componentData.ParentTGIBlocks = tgiBlocks; OnResourceChanged(this, EventArgs.Empty); } } }

        public string Value { get { return ValueBuilder; } }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for ObjKeyResource wrapper
    /// </summary>
    public class ObjKeyResourceHandler : AResourceHandler
    {
        public ObjKeyResourceHandler()
        {
            this.Add(typeof(ObjKeyResource), new List<string>(new string[] { "0x02DC343F" }));
        }
    }
}
