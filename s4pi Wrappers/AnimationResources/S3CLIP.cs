/***************************************************************************
 *  Copyright (C) 2016 by Cmar, Peter Jones                                *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
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
using System.Text;
using System.Linq;
using s4pi.Interfaces;

namespace s4pi.Animation
{
    public class S3CLIP : AHandlerElement, IEquatable<S3CLIP>
    {
        char[] formatToken;
        uint version;
        uint flags;
        float tickLength;
        ushort numTicks;
        ushort padding;
        uint channelCount;
        uint f1PaletteSize;
        uint channelDataOffset;
        uint f1PaletteDataOffset;
        uint nameOffset;
        uint sourceAssetNameOffset;
        string clipName;            //null-terminated string
        string sourceAssetName;     //null-terminated string
        float[] f1Palette;
        S3Channel[] channels;

        public S3CLIP(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
        public S3CLIP(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler)
        {
            this.Parse(s);
        }

        public override int RecommendedApiVersion
        {
            get { return 1; }
        }

        public void Parse(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            long startPos = s.Position;
            formatToken = br.ReadChars(8);
            version = br.ReadUInt32();
            flags = br.ReadUInt32();
            tickLength = br.ReadSingle();
            numTicks = br.ReadUInt16();
            padding = br.ReadUInt16();
            channelCount = br.ReadUInt32();
            f1PaletteSize = br.ReadUInt32();
            channelDataOffset = br.ReadUInt32();
            f1PaletteDataOffset = br.ReadUInt32();
            nameOffset = br.ReadUInt32();
            sourceAssetNameOffset = br.ReadUInt32();
            long currPos = s.Position;
            s.Position = startPos + nameOffset;
            clipName = br.ReadZString();
            s.Position = startPos + sourceAssetNameOffset;
            sourceAssetName = br.ReadZString();
            s.Position = startPos + f1PaletteDataOffset;
            f1Palette = new float[f1PaletteSize];
            for (int i = 0; i < f1PaletteSize; i++)
            {
                f1Palette[i] = br.ReadSingle();
            }
            s.Position = startPos + channelDataOffset;
            this.channels = new S3Channel[channelCount];
            for (int i = 0; i < channelCount; i++)
            {
                this.channels[i] = new S3Channel(RecommendedApiVersion, handler, s, startPos);
            }
            s.Position = startPos;
        }

        public void UnParse(Stream s)
        {
            BinaryWriter bw = new BinaryWriter(s);
            long startPos = s.Position;
            bw.Write(formatToken);
            bw.Write(version);
            bw.Write(flags);
            bw.Write(tickLength);
            bw.Write(numTicks);
            bw.Write(padding);
            bw.Write((uint)channels.Length);
            f1PaletteSize = (uint)F1Palette.Length;
            bw.Write(f1PaletteSize);
            long currPos = s.Position;
            bw.Write(0U);           //channel data offset
            bw.Write(0U);           //pallette data offset
            bw.Write(0U);           //name offset
            bw.Write(0U);           //source asset name offset
            s.Position = startPos + nameOffset;
            bw.Write(clipName);
            s.Position = startPos + sourceAssetNameOffset;
            bw.Write(sourceAssetName);
            s.Position = startPos + f1PaletteDataOffset;
            s.Position = startPos + channelDataOffset;

            long dataPos = s.Position;
            s.Position = currPos;
            bw.Write((uint)(dataPos - startPos));    //channelDataOffset
            currPos = s.Position;
            s.Position = dataPos;
            long channelsPos = s.Position;
            for (int i = 0; i < channelCount; i++)
            {
                this.channels[i].UnParse(s);
            }

            dataPos = s.Position;
            s.Position = currPos;
            bw.Write((uint)(dataPos - startPos));    //nameOffset
            currPos = s.Position;
            s.Position = dataPos;
            bw.WriteZString(clipName);

            dataPos = s.Position;
            s.Position = currPos;
            bw.Write((uint)(dataPos - startPos));    //sourceAssetNameOffset
            currPos = s.Position;
            s.Position = dataPos;
            bw.WriteZString(sourceAssetName);

            dataPos = s.Position;
            s.Position = currPos;
            bw.Write((uint)(dataPos - startPos));    //f1PaletteDataOffset
            currPos = s.Position;
            s.Position = dataPos;
            for (int i = 0; i < f1PaletteSize; i++)
            {
                bw.Write(f1Palette[i]);
            }

            for (int i = 0; i < channelCount; i++)
            {
                dataPos = s.Position;
                s.Position = channelsPos;
                this.channels[i].UpdateOffset(s, startPos, dataPos);
                channelsPos = s.Position;
                s.Position = dataPos;
                this.channels[i].UnParseFrames(s);
            }
        }

        public bool Equals(S3CLIP other)
        {
            return base.Equals(other);
        }

        [ElementPriority(0)]
        public char[] FormatToken
        {
            get { return this.formatToken; }
            set { if (this.formatToken != value) { this.formatToken = value; OnElementChanged(); } }
        }
        [ElementPriority(1)]
        public UInt32 Version
        {
            get { return this.version; }
            set { if (this.version != value) { this.version = value; OnElementChanged(); } }
        }
        [ElementPriority(2)]
        public uint Flags
        {
            get { return this.flags; }
            set { if (this.flags != value) { this.flags = value; OnElementChanged(); } }
        }
        [ElementPriority(3)]
        public float TickLength
        {
            get { return this.tickLength; }
            set { if (this.tickLength != value) { this.tickLength = value; OnElementChanged(); } }
        }
        [ElementPriority(4)]
        public ushort NumTicks
        {
            get { return this.numTicks; }
            set { if (this.numTicks != value) { this.numTicks = value; OnElementChanged(); } }
        }
        [ElementPriority(5)]
        public ushort Padding
        {
            get { return this.padding; }
            set { if (this.padding != value) { this.padding = value; OnElementChanged(); } }
        }
        [ElementPriority(6)]
        public string ClipName
        {
            get { return this.clipName; }
            set { if (this.clipName != value) { this.clipName = value; OnElementChanged(); } }
        }
        [ElementPriority(7)]
        public string SourceAssetName
        {
            get { return this.sourceAssetName; }
            set { if (this.sourceAssetName != value) { this.sourceAssetName = value; OnElementChanged(); } }
        }
        [ElementPriority(8)]
        public float[] F1Palette
        {
            get { return this.f1Palette; }
            set { if (this.f1Palette != value) { this.f1Palette = value; OnElementChanged(); } }
        }
        [ElementPriority(9)]
        public S3Channel[] Channels
        {
            get { return this.channels; }
            set { if (this.channels != value) { this.channels = value; OnElementChanged(); } }
        }

        public override List<string> ContentFields
        {
            get { return GetContentFields(requestedApiVersion, GetType()); }
        }
        public string Value { get { return ValueBuilder; } }
      //  public string Value
      //  {
      //      get
      //      {
      //          string tmp = this.channelDataOffset.ToString() + ", " + this.nameOffset.ToString() + ", " +
      //              this.sourceAssetNameOffset.ToString() + ", " + this.f1PaletteDataOffset.ToString() + Environment.NewLine;
      //          for (int i = 0; i < this.channels.Length; i++)
      //          {
      //              tmp += this.channels[i].DataOffset.ToString() + Environment.NewLine;
      //          }
      //          return tmp;
      //      }
      //  }

        #region sub-types
        public class S3Channel : AHandlerElement, IEquatable<S3Channel>
        {            
            protected S3Channel(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                this.channelType = ChannelType.ChannelType_Unknown;
                this.channelSubTarget = SubTargetType.SubTarget_Unknown;
            }

            public S3Channel(int apiVersion, EventHandler handler, Stream s, long startPos)
                : base(apiVersion, handler)
            {
                Parse(s, startPos);
            }

            protected S3Channel(int apiVersion, EventHandler handler, S3Channel basis)
                : this(apiVersion, handler, basis.target, basis.offset, basis.scale, basis.numFrames, basis.channelType, basis.channelSubTarget, basis.frames)
            {
            }

            protected S3Channel(int APIversion, EventHandler handler, uint target, float offset, float scale, ushort numFrames, ChannelType channelType, SubTargetType channelSubTarget, Frame[] frames)
                : base(APIversion, handler)
            {
                this.target = target;
                this.offset = offset;
                this.scale = scale;
                this.numFrames = numFrames;
                this.channelType = channelType;
                this.channelSubTarget = channelSubTarget;
                this.frames = frames;
            }

            public string Value
            {
                get { return ValueBuilder; }
            }

            uint target;
            float offset;
            float scale;
            ushort numFrames;
            ChannelType channelType;
            SubTargetType channelSubTarget;
            Frame[] frames;

            [ElementPriority(0)]
            public uint Target
            {
                get { return target; }
                set { if (this.target != value) { this.target = value; OnElementChanged(); } }
            }
            [ElementPriority(1)]
            public float Offset
            {
                get { return offset; }
                set { if (this.offset != value) { this.offset = value; OnElementChanged(); } }
            }
            [ElementPriority(2)]
            public float Scale
            {
                get { return scale; }
                set { if (this.scale != value) { this.scale = value; OnElementChanged(); } }
            }
            [ElementPriority(3)]
            public ushort NumberFrames
            {
                get { return numFrames; }
                set { if (this.numFrames != value) { this.numFrames = value; OnElementChanged(); } }
            }
            [ElementPriority(4)]
            public ChannelType Channel_Type
            {
                get { return channelType; }
                set { if (this.channelType != value) { this.channelType = value; OnElementChanged(); } }
            }
            [ElementPriority(5)]
            public SubTargetType ChannelSubTarget
            {
                get { return channelSubTarget; }
                set { if (this.channelSubTarget != value) { this.channelSubTarget = value; OnElementChanged(); } }
            }
            [ElementPriority(6)]
            public Frame[] Frames
            {
                get { return frames; }
                set { if (this.frames != value) { this.frames = value; OnElementChanged(); } }
            }

            public void Parse(Stream s, long startPos)
            {
                var br = new BinaryReader(s);
                uint dataOffset = br.ReadUInt32();
                target = br.ReadUInt32();
                offset = br.ReadSingle();
                scale = br.ReadSingle();
                numFrames = br.ReadUInt16();
                channelType = (ChannelType)br.ReadByte();
                channelSubTarget = (SubTargetType)br.ReadByte();
                long currentPos = s.Position;
                uint width = GetWidthForChannelType(channelType);
                uint count = GetCountForChannelType(channelType);
                frames = new Frame[numFrames];
                s.Position = startPos + dataOffset;
                for (int i = 0; i < numFrames; i++)
                {
                    frames[i] = Frame.Create(handler, width);
                    frames[i].Parse(s, width, count);
                }
                s.Position = currentPos;
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(0U);           //dataOffset
                bw.Write(target);
                bw.Write(offset);
                bw.Write(scale);
                bw.Write(numFrames);
                bw.Write((byte)channelType);
                bw.Write((byte)channelSubTarget);
            }
            public void UpdateOffset(Stream s, long startPos, long dataPos)
            {
                var bw = new BinaryWriter(s);
                bw.Write((uint)(dataPos - startPos));           //dataOffset
                s.Position = s.Position + 16;
            }
            public void UnParseFrames(Stream s)
            {
                for (int i = 0; i < numFrames; i++)
                {
                    frames[i].UnParse(s);
                }
            }

            public override int RecommendedApiVersion
            {
                get { return 1; }
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(requestedApiVersion, GetType()); }
            }

            public bool Equals(S3Channel other)
            {
                return false;
            }
        }

        public abstract class Frame : AHandlerElement, IEquatable<Frame>
        {
            protected Frame(int apiVersion, EventHandler handler)
                : this(apiVersion, handler, 0, 0)
            {
            }

            protected Frame(int apiVersion, EventHandler handler, Stream s, uint width, uint count)
                : this(apiVersion, handler)
            {
                Parse(s, width, count);
            }

            protected Frame(int apiVersion, EventHandler handler, Frame basis)
                : this(apiVersion, handler, basis.startTick, basis.flags)
            {
            }

            protected Frame(int APIversion, EventHandler handler, ushort startTick, ushort flags)
                : base(APIversion, handler)
            {
                this.startTick = startTick;
                this.flags = flags;
            }

            public string Value
            {
                get { return ValueBuilder; }
            }

            ushort startTick;
            ushort flags;

            [ElementPriority(0)]
            public ushort StartTick
            {
                get { return startTick; }
                set { if (this.startTick != value) { this.startTick = value; OnElementChanged(); } }
            }
            [ElementPriority(1)]
            public ushort Flags
            {
                get { return flags; }
                set { if (this.flags != value) { this.flags = value; OnElementChanged(); } }
            }

            public void Parse(Stream s, uint width, uint count)
            {
                var br = new BinaryReader(s);
                this.startTick = br.ReadUInt16();
                this.flags = br.ReadUInt16();
                this.ReadTypeData(s, count);
            }
            public abstract void ReadTypeData(Stream ms, uint count);

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write(this.startTick);
                bw.Write(this.flags);
                this.WriteTypeData(s);
            }
            public abstract void WriteTypeData(Stream ms);

            public override int RecommendedApiVersion
            {
                get { return 1; }
            }

            public override List<string> ContentFields
            {
                get { return GetContentFields(requestedApiVersion, GetType()); }
            }

            public bool Equals(Frame other)
            {
                return this.StartTick == other.StartTick && this.Flags == other.Flags && this.isEqual(other);
            }
            public abstract bool isEqual(Frame other);

            public static Frame Create(EventHandler handler, uint width)
            {
                switch (width)
                {
                    case 1:
                        return new Frame1(0, handler);
                    case 2:
                        return new Frame2(0, handler);
                    case 10:
                        return new Frame10(0, handler);
                    default:
                        return null;
                }
            }
        }

        public class Frame1 : Frame
        {
            private byte[] indices;

            public byte[] Indices
            {
                get { return indices; }
                set { if (this.indices != value) { this.indices = value; OnElementChanged(); } }
            }

            public override bool isEqual(Frame other)
            {
                if (other is Frame1)
                {
                    Frame1 f = other as Frame1;
                    return Enumerable.SequenceEqual(this.indices, f.indices);
                }
                else
                {
                    return false;
                }
            }

            public Frame1(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
            }
            protected Frame1(int apiVersion, EventHandler handler, Stream s, uint width, uint count)
                : base(apiVersion, handler, s, width, count)
            {
                this.ReadTypeData(s, count);
            }
            protected Frame1(int apiVersion, EventHandler handler, Frame1 basis)
                : base(apiVersion, handler, basis)
            {
                this.indices = basis.indices;
            }

            public override void ReadTypeData(Stream ms, uint count)
            {
                BinaryReader br = new BinaryReader(ms);
                this.indices = br.ReadBytes((int)count);
                if ((4 - count) > 0) br.ReadBytes((int)(4 - count));
            }
            public override void WriteTypeData(Stream ms)
            {
                BinaryWriter bw = new BinaryWriter(ms);
                bw.Write(this.indices);
                for (int i = 0; i < 4 - this.indices.Length; i++)
                {
                    bw.Write((byte)0);
                }
            }
        }

        public class Frame2 : Frame
        {
            private ushort[] indices;

            public ushort[] Indices
            {
                get { return indices; }
                set { if (this.indices != value) { this.indices = value; OnElementChanged(); } }
            }

            public override bool isEqual(Frame other)
            {
                if (other is Frame2)
                {
                    Frame2 f = other as Frame2;
                    return Enumerable.SequenceEqual(this.indices, f.indices);
                }
                else
                {
                    return false;
                }
            }

            public Frame2(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
            }
            protected Frame2(int apiVersion, EventHandler handler, Stream s, uint width, uint count)
                : base(apiVersion, handler, s, width, count)
            {
                this.ReadTypeData(s, count);
            }
            protected Frame2(int apiVersion, EventHandler handler, Frame2 basis)
                : base(apiVersion, handler, basis)
            {
                this.indices = basis.indices;
            }

            public override void ReadTypeData(Stream ms, uint count)
            {
                BinaryReader br = new BinaryReader(ms);
                this.indices = new ushort[count];
                for (int i = 0; i < count; i++)
                {
                    this.indices[i] = br.ReadUInt16();
                }
                if (count % 2 != 0) br.ReadBytes(2);
            }
            public override void WriteTypeData(Stream ms)
            {
                BinaryWriter bw = new BinaryWriter(ms);
                for (int i = 0; i < this.indices.Length; i++)
                {
                    bw.Write(this.indices[i]);
                }
                if (this.indices.Length % 2 != 0) bw.Write(new byte[] { 0, 0 });
            }
        }

        public class Frame10 : Frame
        {
            private ushort[] indices;

            public ushort[] Indices
            {
                get { return indices; }
                set { if (this.indices != value) { this.indices = value; OnElementChanged(); } }
            }

            public override bool isEqual(Frame other)
            {
                if (other is Frame10)
                {
                    Frame10 f = other as Frame10;
                    return Enumerable.SequenceEqual(this.indices, f.indices);
                }
                else
                {
                    return false;
                }
            }

            public Frame10(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
            }
            protected Frame10(int apiVersion, EventHandler handler, Stream s, uint width, uint count)
                : base(apiVersion, handler, s, width, count)
            {
                this.ReadTypeData(s, count);
            }
            protected Frame10(int apiVersion, EventHandler handler, Frame10 basis)
                : base(apiVersion, handler, basis)
            {
                this.indices = basis.indices;
            }

            public override void ReadTypeData(Stream ms, uint count)
            {
                BinaryReader br = new BinaryReader(ms);
                uint data = br.ReadUInt32();
                indices = new ushort[3];
                indices[0] = (ushort)(data >> 22);
                indices[1] = (ushort)((data << 10) >> 22);
                indices[2] = (ushort)((data << 20) >> 22);
            }
            public override void WriteTypeData(Stream ms)
            {
                BinaryWriter bw = new BinaryWriter(ms);
                uint data = ((uint)(indices[0] << 22)) + ((uint)(indices[1] << 12)) + ((uint)(indices[2] << 2));
                bw.Write(data);
            }
        }

        #endregion

        #region enums

        public enum ChannelType : byte
        {
            ChannelType_Unknown = 0,
            F1,
            F2,
            F3,
            F4,

            F1_Normalized,
            F2_Normalized,
            F3_Normalized,
            F4_Normalized,

            F1_Zero,
            F2_Zero,
            F3_Zero,
            F4_Zero,

            F1_One,
            F2_One,
            F3_One,
            F4_One,

            F4_QuaternionIdentity,

            F3_HighPrecisionNormalized,
            F4_HighPrecisionNormalized_Quaternion,
            F4_SuperHighPrecision_Quaternion,
            F3_HighPrecisionNormalized_Quaternion,// channel type does not support offset or scale

        }

        public enum SubTargetType : byte
        {
            SubTarget_Unknown = 0,
            Translation,
            Orientation,
            Scale,

            Translation_X,  // UNSUPPORTED
            Translation_Y,  // UNSUPPORTED
            Translation_Z,  // UNSUPPORTED

            Orientation_X,  // UNSUPPORTED
            Orientation_Y,  // UNSUPPORTED
            Orientation_Z,  // UNSUPPORTED
            Orientation_W,  // UNSUPPORTED

            Scale_X,  // UNSUPPORTED
            Scale_Y,  // UNSUPPORTED
            Scale_Z,  // UNSUPPORTED

            IK_TargetWeight_World,
            IK_TargetWeight_1,
            IK_TargetWeight_2,
            IK_TargetWeight_3,
            IK_TargetWeight_4,
            IK_TargetWeight_5,
            IK_TargetWeight_6,
            IK_TargetWeight_7,
            IK_TargetWeight_8,
            IK_TargetWeight_9,
            IK_TargetWeight_10,

            IK_TargetOffset_Translation_World,
            IK_TargetOffset_Orientation_World,
            IK_TargetOffset_Translation_1,
            IK_TargetOffset_Orientation_1,
            IK_TargetOffset_Translation_2,
            IK_TargetOffset_Orientation_2,
            IK_TargetOffset_Translation_3,
            IK_TargetOffset_Orientation_3,
            IK_TargetOffset_Translation_4,
            IK_TargetOffset_Orientation_4,
            IK_TargetOffset_Translation_5,
            IK_TargetOffset_Orientation_5,
            IK_TargetOffset_Translation_6,
            IK_TargetOffset_Orientation_6,
            IK_TargetOffset_Translation_7,
            IK_TargetOffset_Orientation_7,
            IK_TargetOffset_Translation_8,
            IK_TargetOffset_Orientation_8,
            IK_TargetOffset_Translation_9,
            IK_TargetOffset_Orientation_9,
            IK_TargetOffset_Translation_10,
            IK_TargetOffset_Orientation_10,

        }
        #endregion

        #region utilities
        internal static uint GetWidthForChannelType(ChannelType t)
        {
            switch (t)
            {
                case ChannelType.F1_Normalized:
                case ChannelType.F2_Normalized:
                case ChannelType.F3_Normalized:
                case ChannelType.F4_Normalized:
                    return 1;

                case ChannelType.F3_HighPrecisionNormalized:
                case ChannelType.F4_HighPrecisionNormalized_Quaternion:
                case ChannelType.F3_HighPrecisionNormalized_Quaternion:
                    return 10;

                case ChannelType.F1:
                case ChannelType.F2:
                case ChannelType.F3:
                case ChannelType.F4:
                case ChannelType.F4_SuperHighPrecision_Quaternion:
                    return 2;

                case ChannelType.F1_Zero:
                case ChannelType.F2_Zero:
                case ChannelType.F3_Zero:
                case ChannelType.F4_Zero:
                case ChannelType.F1_One:
                case ChannelType.F2_One:
                case ChannelType.F3_One:
                case ChannelType.F4_One:
                case ChannelType.F4_QuaternionIdentity:
                    return 0;

                default:
                    return 0;
            }
        }

        internal static uint GetCountForChannelType(ChannelType t)
        {
            switch (t)
            {
                case ChannelType.F1:
                case ChannelType.F1_Normalized:
                    return 1;

                case ChannelType.F2:
                case ChannelType.F2_Normalized:
                    return 2;

                case ChannelType.F3:
                case ChannelType.F3_Normalized:
                    return 3;

                case ChannelType.F4:
                case ChannelType.F4_Normalized:
                case ChannelType.F4_SuperHighPrecision_Quaternion:
                case ChannelType.F4_HighPrecisionNormalized_Quaternion:
                case ChannelType.F3_HighPrecisionNormalized_Quaternion:
                    return 4;

                case ChannelType.F3_HighPrecisionNormalized:
                    return 3;

                case ChannelType.F1_Zero:
                case ChannelType.F2_Zero:
                case ChannelType.F3_Zero:
                case ChannelType.F4_Zero:
                case ChannelType.F1_One:
                case ChannelType.F2_One:
                case ChannelType.F3_One:
                case ChannelType.F4_One:
                case ChannelType.F4_QuaternionIdentity:
                    return 0;

                default:
                    return 0;
            }
        }
        #endregion
    }
}
