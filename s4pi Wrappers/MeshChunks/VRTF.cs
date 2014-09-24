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
using System.IO;
using s4pi.Interfaces;
using System.Text;
using System.Linq;
using System.Collections.Generic;
namespace meshExpImp.ModelBlocks
{
    
    public class VRTF : ARCOLBlock
    {
        public class Default : VRTF
        {
            public Default(int APIversion, EventHandler handler)
                : base(APIversion, handler)
            {
            }
            public override bool IsDefault()
            {
                return true;
            }
        }
        public static VRTF CreateDefaultForMesh(MLOD.Mesh mesh)
        {
            return mesh.IsShadowCaster ? CreateDefaultForSunShadow() : CreateDefaultForDropShadow();
        }
        public static VRTF CreateDefaultForSunShadow()
        {
            VRTF v = new Default(0, null);
            v.Layouts.Add(new ElementLayout(0, null, ElementFormat.Short4, 0, ElementUsage.Position, 0));
            v.Stride = v.Layouts.Select(x => VRTF.ByteSizeFromFormat(x.Format)).Sum();
            return v;
        }
        public static VRTF CreateDefaultForDropShadow()
        {
            VRTF v = new Default(0, null);
            v.Layouts.Add(new ElementLayout(0, null, ElementFormat.UShort4N, 0, ElementUsage.Position, 0));
            v.Layouts.Add(new ElementLayout(0, null, ElementFormat.Short4_DropShadow, (byte)v.Layouts.Select(x => VRTF.ByteSizeFromFormat(x.Format)).Sum(), ElementUsage.UV, 0));
            // above replaces following two lines, following discussion with Atavera on 25 May 2012.
            //v.Layouts.Add(new ElementLayout(0, null, ElementFormat.Short2, (byte)v.Layouts.Select(x => VRTF.ByteSizeFromFormat(x.Format)).Sum(), ElementUsage.UV, 0));
            //v.Layouts.Add(new ElementLayout(0, null, ElementFormat.Short2, (byte)v.Layouts.Select(x => VRTF.ByteSizeFromFormat(x.Format)).Sum(), ElementUsage.UV, 1));
            //--
            //v.Layouts.Add(new ElementLayout(0, null, ElementFormat.ColorUByte4, (byte)v.Layouts.Select(x => VRTF.ByteSizeFromFormat(x.Format)).Sum(), ElementUsage.Normal, 0));
            v.Stride = v.Layouts.Select(x => VRTF.ByteSizeFromFormat(x.Format)).Sum();
            return v;
        }
        public enum ElementUsage : byte
        {
            Position,
            Normal,
            UV,
            BlendIndex,
            BlendWeight,
            Tangent,
            Colour
        }
        public enum ElementFormat : byte
        {
            Float1,
            Float2,
            Float3,
            Float4,
            UByte4,
            ColorUByte4,
            Short2,
            Short4,
            UByte4N,
            Short2N,
            Short4N,
            UShort2N,
            UShort4N,
            Dec3N,
            UDec3N,
            Float16_2,
            Float16_4,
            Short4_DropShadow = 0xFF,
        }
        public static int FloatCountFromFormat(VRTF.ElementFormat format)
        {
            switch (format)
            {
                case ElementFormat.Float1:
                    return 1;
                case ElementFormat.Float2:
                case ElementFormat.UShort2N:
                case ElementFormat.Short2:
                    return 2;
                case ElementFormat.Short4:
                case ElementFormat.Short4N:
                case ElementFormat.UByte4N:
                case ElementFormat.UShort4N:
                case ElementFormat.Float3:
                    return 3;
                case ElementFormat.ColorUByte4:
                case ElementFormat.Float4:
                case ElementFormat.Short4_DropShadow:
                    return 4;
                default:
                    throw new NotImplementedException();
            }
            
        }
        public static int ByteSizeFromFormat(ElementFormat f)
        {
            switch (f)
            {
                case ElementFormat.Float1:
                case ElementFormat.UByte4:
                case ElementFormat.ColorUByte4:
                case ElementFormat.UByte4N:
                case ElementFormat.UShort2N:
                case ElementFormat.Short2:
                    return 4;
                case ElementFormat.UShort4N:
                case ElementFormat.Float2:
                case ElementFormat.Short4:
                case ElementFormat.Short4N:
                case ElementFormat.Short4_DropShadow:
                    return 8;
                case ElementFormat.Float3:
                    return 12;
                case ElementFormat.Float4:
                    return 16;
                default:
                    throw new NotImplementedException();
            }
        }
        public class VertexElementLayoutList : DependentList<ElementLayout>
        {
            public VertexElementLayoutList(EventHandler handler)
                : base(handler)
            {
            }

            public VertexElementLayoutList(EventHandler handler, Stream s, int count)
                : base(handler)
            {
                Parse(s, count);
            }

            public VertexElementLayoutList(EventHandler handler, IEnumerable<ElementLayout> ilt) : base(handler, ilt) {}

            protected override void WriteCount(Stream s, int count){}
            private void Parse(Stream s, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    ((IList<ElementLayout>)this).Add(CreateElement(s));
                }
            }
            protected override ElementLayout CreateElement(Stream s)
            {
                return new ElementLayout(0, handler, s);
            }

            protected override void WriteElement(Stream s, ElementLayout element)
            {
                element.UnParse(s);
            }
        }
        public class ElementLayout : AHandlerElement, IEquatable<ElementLayout>
        {
            private ElementUsage mUsage;
            private byte mUsageIndex;
            private ElementFormat mFormat;
            private byte mOffset;

            public ElementLayout(int APIversion, EventHandler handler): base(APIversion, handler){}
            public ElementLayout(int APIversion, EventHandler handler, ElementLayout basis): this(APIversion, handler,basis.Format,basis.Offset,basis.Usage,basis.UsageIndex){}
            public ElementLayout(int APIversion, EventHandler handler, Stream s): base(APIversion, handler){Parse(s);}
            public ElementLayout(int APIversion, EventHandler handler, ElementFormat format, byte offset, ElementUsage usage, byte usageIndex) : base(APIversion, handler)
            {
                mFormat = format;
                mOffset = offset;
                mUsage = usage;
                mUsageIndex = usageIndex;
            }
            [ElementPriority(1)]
            public ElementUsage Usage
            {
                get { return mUsage; }
                set { if(mUsage!=value){mUsage = value; OnElementChanged();} }
            }
            [ElementPriority(2)]
            public byte UsageIndex
            {
                get { return mUsageIndex; }
                set { if(mUsageIndex!=value){mUsageIndex = value; OnElementChanged();} }
            }
            [ElementPriority(3)]
            public ElementFormat Format
            {
                get { return mFormat; }
                set { if(mFormat!=value){mFormat = value; OnElementChanged();} }
            }
            [ElementPriority(4)]
            public byte Offset
            {
                get { return mOffset; }
                set { if(mOffset!=value){mOffset = value; OnElementChanged();} }
            }

            public string Value
            {
                get
                {
                    return ValueBuilder;
                    /*return String.Format(
                        //"Usage: 0x{0:X2} ({1,-11})[{2}]; Format: 0x{3:X2} ({4,-11}); Offset: 0x{5:X2}",
                        //"{1,11} (0x{0:X2})[{2}]; {4,-11} (0x{3:X2}); Offset: 0x{5:X2}",
                        "0x{0:X2} ({1,-11})[{2}]; 0x{3:X2} ({4,-11}); Offset: 0x{5:X2}",
                        (int)Usage, Usage + "", mUsageIndex, (int)Format, Format, mOffset);
                    /*
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("Usage:\t{0}\n", mUsage);
                    sb.AppendFormat("Index:\t0x{0:X2}\n", mUsageIndex);
                    sb.AppendFormat("Format:\t{0}\n", mFormat);
                    sb.AppendFormat("Offset:\t0x{0:X2}\n", mOffset);
                    return sb.ToString();
                    /**/
                }
            }
            private void Parse(Stream s)
            {
                BinaryReader br = new BinaryReader(s);
                mUsage = (ElementUsage)br.ReadByte();
                mUsageIndex = br.ReadByte();
                mFormat = (ElementFormat)br.ReadByte();
                mOffset = br.ReadByte();
            }
            public void UnParse(Stream s)
            {
                BinaryWriter bw = new BinaryWriter(s);
                bw.Write((byte)mUsage);
                bw.Write((byte)mUsageIndex);
                bw.Write((byte)mFormat);
                bw.Write((byte)mOffset);
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(base.requestedApiVersion, GetType()); }
            }

            public override int RecommendedApiVersion
            {
                get { return kRecommendedApiVersion; }
            }

            public bool Equals(ElementLayout other)
            {
                return
                    mUsage.Equals(other.mUsage)
                    && mUsageIndex.Equals(other.mUsageIndex)
                    && mFormat.Equals(other.mFormat)
                    && mOffset.Equals(other.mOffset)
                    ;
            }
            public override bool Equals(object obj)
            {
                return obj as ElementLayout != null ? this.Equals(obj as ElementLayout) : false;
            }
            public override int GetHashCode()
            {
                return
                    mUsage.GetHashCode()
                    ^ mUsageIndex.GetHashCode()
                    ^ mFormat.GetHashCode()
                    ^ mOffset.GetHashCode()
                    ;
            }
        }
        private UInt32 mVersion;
        private Int32 mStride;
        private bool mExtendedFormat;
        private VertexElementLayoutList mLayouts;

        public VRTF(int APIversion, EventHandler handler)
            : base(APIversion, handler, null)
        {
            mVersion = 0x00000002;
        }
        public VRTF(int APIversion, EventHandler handler, VRTF basis): this(APIversion, handler,basis.Version,basis.Stride, basis.Layouts, basis.ExtendedFormat){}
        public VRTF(int APIversion, EventHandler handler, Stream s): base(APIversion, handler, s){}
        public VRTF(int APIversion, EventHandler handler, uint version, int stride, VertexElementLayoutList layouts, bool extendedFormat) : base(APIversion, handler, null)
        {
            mExtendedFormat = extendedFormat;
            mLayouts = layouts == null ? null : new VertexElementLayoutList(handler, layouts);
            mStride = stride;
            mVersion = version;
        }

        public virtual bool IsDefault ()
        {
            return false;
        }

        [ElementPriority(1)]
        public uint Version
        {
            get { return mVersion; }
            set { if(mVersion!=value){mVersion = value; OnRCOLChanged(this, EventArgs.Empty);} }
        }
        [ElementPriority(2)]
        public int Stride
        {
            get { return mStride; }
            set { if(mStride!=value){mStride = value; OnRCOLChanged(this, EventArgs.Empty);} }
        }
        [ElementPriority(3)]
        public bool ExtendedFormat
        {
            get { return mExtendedFormat; }
            set { if(mExtendedFormat!=value){mExtendedFormat = value; OnRCOLChanged(this, EventArgs.Empty);} }
        }
        [ElementPriority(4)]
        public VertexElementLayoutList Layouts
        {
            get { return mLayouts; }
            set { if (mLayouts != value) { mLayouts = value == null ? null : new VertexElementLayoutList(handler, value); OnRCOLChanged(this, EventArgs.Empty); } }
        }

        public string Value { get { return ValueBuilder; } }

        protected override void Parse(Stream s)
        {

            BinaryReader br = new BinaryReader(s);
            string tag = FOURCC(br.ReadUInt32());
            if (checking && tag != Tag)
            {
                throw new InvalidDataException(string.Format("Invalid Tag read: '{0}'; expected: '{1}'; at 0x{2:X8}", tag, Tag, s.Position));
            }
            mVersion = br.ReadUInt32();
            mStride = br.ReadInt32();
            int count = br.ReadInt32();
            mExtendedFormat = br.ReadUInt32() > 0 ? true : false;
            mLayouts = new VertexElementLayoutList(handler, s, count);


        }
        public override Stream UnParse()
        {

            if (mLayouts == null) mLayouts = new VertexElementLayoutList(handler);
            MemoryStream s = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(s);
            bw.Write((uint)FOURCC(Tag));
            bw.Write(mVersion);
            bw.Write(mStride);
            bw.Write(mLayouts.Count);
            bw.Write(mExtendedFormat?1:0);
            mLayouts.UnParse(s);
            return s;
        }

        public override string Tag
        {
            get { return "VRTF"; }
        }

        public override uint ResourceType
        {
            get { return 0x01D0E723; }
        }

        private static bool checking = s4pi.Settings.Settings.Checking;
        private const int kRecommendedApiVersion = 1;
    }
}