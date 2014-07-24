/*
 * This wrapper is based on Rick's code and is transformed into s4pi wrapper by Kuree.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using s4pi.Interfaces;

namespace s4pi.ImageResource
{
    public class RLEResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        private RLEInfo info;
        private MipHeader[] MipHeaders;
        private byte[] data;
        #endregion

        public RLEResource(int APIversion, Stream s) : base(APIversion, s) { if (s == null) { OnResourceChanged(this, EventArgs.Empty); } else { Parse(s); } }

        #region Data I/O
        public void Parse(Stream s)
        {
            if (s == null) return;
            BinaryReader r = new BinaryReader(s);
            info = new RLEInfo(s);
            this.MipHeaders = new MipHeader[this.info.mipCount + 1];

            for (int i = 0; i < this.info.mipCount; i++)
            {
                MipHeaders[i] = new MipHeader
                {
                    CommandOffset = r.ReadInt32(),
                    Offset2 = r.ReadInt32(),
                    Offset3 = r.ReadInt32(),
                    Offset0 = r.ReadInt32(),
                    Offset1 = r.ReadInt32(),
                    Offset4 = this.info.HasSpecular ? r.ReadInt32() : 0
                };
            }

            this.MipHeaders[this.info.mipCount] = new MipHeader
            {
                CommandOffset = MipHeaders[0].Offset2,
                Offset2 = MipHeaders[0].Offset3,
                Offset3 = MipHeaders[0].Offset0,
                Offset0 = MipHeaders[0].Offset1,
                Offset1 = this.info.HasSpecular ? MipHeaders[0].Offset4 : (int)s.Length,
                Offset4 = (int)s.Length
            };
            s.Position = 0;
            this.data = r.ReadBytes((int)s.Length);
        }


        protected override Stream UnParse()
        {
            return new MemoryStream(this.data);
        }

        public Stream ToDDS()
        {
            MemoryStream s = new MemoryStream();
            BinaryWriter w = new BinaryWriter(s);
            w.Write(RLEInfo.Signature);
            this.info.UnParse(s);

            for (int i = 0; i < this.info.mipCount; i++)
            {
                var mipHeader = this.MipHeaders[i];
                var nextMipHeader = this.MipHeaders[i + 1];

                int blockOffset2, blockOffset3, blockOffset0, blockOffset1;
                blockOffset2 = mipHeader.Offset2;
                blockOffset3 = mipHeader.Offset3;
                blockOffset0 = mipHeader.Offset0;
                blockOffset1 = mipHeader.Offset1;


                for (int commandOffset = mipHeader.CommandOffset; commandOffset < nextMipHeader.CommandOffset; commandOffset += 2)
                {
                    var command = BitConverter.ToUInt16(this.data, commandOffset);

                    var op = command & 3;
                    var count = command >> 2;
                    if (op == 0)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            w.Write((ushort)0);

                            w.Write((ushort)0);
                            w.Write((short)0);
                            w.Write((short)0);

                            w.Write((uint)0);
                            w.Write((uint)0);
                        }
                    }
                    else if (op == 1)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            w.Write(this.data, blockOffset0, 2);
                            w.Write(this.data, blockOffset1, 6);
                            w.Write(this.data, blockOffset2, 4);
                            w.Write(this.data, blockOffset3, 4);
                            blockOffset2 += 4;
                            blockOffset3 += 4;
                            blockOffset0 += 2;
                            blockOffset1 += 6;
                        }
                    }
                    else if (op == 2)
                    {
                        for (int j = 0; j < count; j++)
                        {
                            if (this.info.HasSpecular == false)
                            {
                                // TODO: fix me
                                w.Write(0xFFFFFFFFFFFF0500ul);
                                //w.WriteValueU64(0ul, endian);
                            }
                            else
                            {
                                w.Write(this.data, blockOffset0, 2);
                                w.Write(this.data, blockOffset1, 6);
                                blockOffset0 += 2;
                                blockOffset1 += 6;
                            }

                            w.Write(this.data, blockOffset2, 4);
                            w.Write(this.data, blockOffset3, 4);
                            blockOffset2 += 4;
                            blockOffset3 += 4;
                        }
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                if (blockOffset0 != nextMipHeader.Offset0 ||
                    blockOffset1 != nextMipHeader.Offset1 ||
                    blockOffset2 != nextMipHeader.Offset2 ||
                    blockOffset3 != nextMipHeader.Offset3)
                {
                    throw new InvalidOperationException();
                }
            }
            s.Position = 0;
            return s;

        }


        public void ImportToRLE(Stream input)
        {
            MemoryStream output = new MemoryStream();
            BinaryReader r = new BinaryReader(input);
            BinaryWriter w = new BinaryWriter(output);
            this.info = new RLEInfo();
            this.info.Parse(input);

            if (this.info.Depth == 0) this.info.Depth = 1;


            w.Write((uint)FourCC.DXT5);
            w.Write((uint)0x32454C52);
            w.Write((ushort)this.info.Width);
            w.Write((ushort)this.info.Height);
            w.Write((ushort)this.info.mipCount);
            w.Write((ushort)0);

            var headerOffset = 16;
            var dataOffset = 16 + (20 * this.info.mipCount);
            this.MipHeaders = new MipHeader[this.info.mipCount];

            using (var commandData = new MemoryStream())
            using (var block2Data = new MemoryStream())
            using (var block3Data = new MemoryStream())
            using (var block0Data = new MemoryStream())
            using (var block1Data = new MemoryStream())
            {
                BinaryWriter commandDataWriter = new BinaryWriter(commandData);
                for (int i = 0; i < this.info.mipCount; i++)
                {
                    this.MipHeaders[i] = new MipHeader()
                    {
                        CommandOffset = (int)commandData.Length,
                        Offset2 = (int)block2Data.Length,
                        Offset3 = (int)block3Data.Length,
                        Offset0 = (int)block0Data.Length,
                        Offset1 = (int)block1Data.Length,
                    };

                    var mipWidth = Math.Max(4, this.info.Width >> i);
                    var mipHeight = Math.Max(4, this.info.Height >> i);
                    var mipDepth = Math.Max(1, this.info.Depth >> i);

                    var mipSize = Math.Max(1, (mipWidth + 3) / 4) * Math.Max(1, (mipHeight + 3) / 4) * 16;
                    var mipData = r.ReadBytes(mipSize);

                    for (int j = 0; j < mipSize; )
                    {
                        ushort nullCount = 0;
                        while (nullCount < 0x3FFF &&
                               j < mipSize &&
                               TrueForAll(mipData, j, 16, b => b == 0) == true)
                        {
                            nullCount++;
                            j += 16;
                        }

                        if (nullCount > 0)
                        {
                            nullCount <<= 2;
                            nullCount |= 0;
                            commandDataWriter.Write(nullCount);
                            continue;
                        }

                        var start = j;
                        ushort fullCount = 0;
                        while (fullCount < 0x3FFF &&
                               j < mipSize &&
                               TrueForAny(mipData, j, 16, b => b != 0) == true)
                        {
                            fullCount++;
                            j += 16;
                        }

                        if (fullCount > 0)
                        {
                            for (int k = 0; k < fullCount; k++, start += 16)
                            {
                                block0Data.Write(mipData, start + 0, 2);
                                block1Data.Write(mipData, start + 2, 6);
                                block2Data.Write(mipData, start + 8, 4);
                                block3Data.Write(mipData, start + 12, 4);
                            }

                            fullCount <<= 2;
                            fullCount |= 1;
                            commandDataWriter.Write(fullCount);
                            continue;
                        }

                        throw new NotImplementedException();
                    }
                }

                output.Position = dataOffset;

                commandData.Position = 0;
                var commandOffset = (int)output.Position;
                output.Write(commandData.ToArray(), 0, (int)commandData.Length);

                block2Data.Position = 0;
                var block2Offset = (int)output.Position;
                output.Write(block2Data.ToArray(), 0, (int)block2Data.Length);

                block3Data.Position = 0;
                var block3Offset = (int)output.Position;
                output.Write(block3Data.ToArray(), 0, (int)block3Data.Length);

                block0Data.Position = 0;
                var block0Offset = (int)output.Position;
                output.Write(block0Data.ToArray(), 0, (int)block0Data.Length);

                block1Data.Position = 0;
                var block1Offset = (int)output.Position;
                output.Write(block1Data.ToArray(), 0, (int)block1Data.Length);

                output.Position = headerOffset;
                for (int i = 0; i < this.info.mipCount; i++)
                {
                    var mipHeader = this.MipHeaders[i];
                    w.Write(mipHeader.CommandOffset + commandOffset);
                    w.Write(mipHeader.Offset2 + block2Offset);
                    w.Write(mipHeader.Offset3 + block3Offset);
                    w.Write(mipHeader.Offset0 + block0Offset);
                    w.Write(mipHeader.Offset1 + block1Offset);
                }
                this.data = output.ToArray();
            }
        }


        private static bool TrueForAll<T>(T[] array, int offset, int count, Predicate<T> match)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            if (offset < 0 || offset > array.Length)
            {
                throw new IndexOutOfRangeException();
            }

            var end = offset + count;
            if (end < 0 || end > array.Length)
            {
                throw new IndexOutOfRangeException();
            }

            for (int index = offset; index < end; index++)
            {
                if (match(array[index]) == false)
                {
                    return false;
                }
            }
            return true;
        }

        private static bool TrueForAny<T>(T[] array, int offset, int count, Predicate<T> match)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            if (offset < 0 || offset > array.Length)
            {
                throw new IndexOutOfRangeException();
            }

            var end = offset + count;
            if (end < 0 || end > array.Length)
            {
                throw new IndexOutOfRangeException();
            }

            for (int index = offset; index < end; index++)
            {
                if (match(array[index]) == true)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Content Fields
        //public String Value
        //{
        //    get
        //    {
        //        if (this.info == null)
        //            return "Resource is null";
        //        else
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            sb.AppendFormat("Image Width: {0}\n", this.info.Width);
        //            sb.AppendFormat("Image Height: {0}\n", this.info.Height);
        //            sb.AppendFormat("RLE Version: {0}\n", this.info.Version);
        //            sb.AppendLine("-".PadRight(20, '-'));
        //            sb.AppendLine("To see the image, please use the helper distributed with s4pe --Kuree");
        //            return sb.ToString();
        //        }
        //    }
        //}
        #endregion

        #region Sub-Types

        public class MipHeader
        {
            public int CommandOffset { get; internal set; }
            public int Offset0 { get; internal set; }
            public int Offset1 { get; internal set; }
            public int Offset2 { get; internal set; }
            public int Offset3 { get; internal set; }
            public int Offset4 { get; internal set; }
        }

        public class RLEInfo
        {
            public const uint Signature = 0x20534444;
            public uint size { get { return (18 * 4) + PixelFormat.StructureSize + (5 * 4); } }
            public HeaderFlags headerFlags { get; internal set; }
            public int Height { get; internal set; }
            public int Width { get; internal set; }
            public uint PitchOrLinearSize { get; internal set; }
            public int Depth = 1;
            //public uint mipMapCount { get; internal set; }
            private byte[] Reserved1 = new byte[11 * 4];
            public PixelFormat pixelFormat { get; internal set; }
            public uint surfaceFlags { get; internal set; }
            public uint cubemapFlags { get; internal set; }
            public byte[] reserved2 = new byte[3 * 4];
            public RLEVersion Version { get; internal set; }
            public RLEInfo() { this.pixelFormat = new PixelFormat(); }
            public bool HasSpecular { get { return this.Version == RLEVersion.RLES; } }
            public uint mipCount { get; internal set; }
            public ushort Unknown0E { get; internal set; }

            public RLEInfo(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                uint fourcc = r.ReadUInt32();
                if (fourcc != (uint)FourCC.DXT5) throw new NotImplementedException(string.Format("Expected format: 0x{0:X8}, read 0x{1:X8}", FourCC.DXT5, fourcc));
                this.Version = (RLEVersion)r.ReadUInt32();
                this.Width = r.ReadUInt16();
                this.Height = r.ReadUInt16();
                this.mipCount = r.ReadUInt16();
                this.Unknown0E = r.ReadUInt16();
                this.headerFlags = HeaderFlags.Texture;
                if (this.Unknown0E != 0) throw new InvalidDataException(string.Format("Expected 0, read 0x{0:X8}", this.Unknown0E));
                this.pixelFormat = new PixelFormat();
            }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                uint signature = r.ReadUInt32();
                if (signature != Signature) throw new InvalidDataException(string.Format("Expected signature 0x{0:X8}, read 0x{1:X8}", Signature, signature));
                uint size = r.ReadUInt32();
                if (size != this.size) throw new InvalidDataException(string.Format("Expected size: 0x{0:X8}, read 0x{1:X8}", this.size, size));
                this.headerFlags = (HeaderFlags)r.ReadUInt32();
                if (this.headerFlags != HeaderFlags.Texture) throw new InvalidDataException(string.Format("Expected 0x{0:X8}, read 0x{1:X8}", HeaderFlags.Texture, this.headerFlags));
                this.Height = r.ReadInt32();
                this.Width = r.ReadInt32();
                if (this.Height > ushort.MaxValue || this.Width > ushort.MaxValue) throw new InvalidDataException("Invalid width or length");
                this.PitchOrLinearSize = r.ReadUInt32();
                this.Depth = r.ReadInt32();
                if (this.Depth != 0 && this.Depth != 1) throw new InvalidDataException(string.Format("Expected depth 1 or 0, read 0x{0:X8}", this.Depth));
                this.mipCount = r.ReadUInt32();
                if (this.mipCount > 16) throw new InvalidDataException(string.Format("Expected mini map count less than 16, read 0x{0:X8}", this.mipCount));
                r.ReadBytes(this.Reserved1.Length);
                this.pixelFormat = new PixelFormat();
                this.pixelFormat.Parse(s);
                this.surfaceFlags = r.ReadUInt32();
                this.cubemapFlags = r.ReadUInt32();
                r.ReadBytes(this.reserved2.Length);
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.size);
                w.Write((uint)this.headerFlags);
                w.Write(this.Height);
                w.Write(this.Width);
                w.Write(this.PitchOrLinearSize);
                w.Write(this.Depth);
                w.Write((uint)this.mipCount);
                w.Write(this.Reserved1);
                this.pixelFormat.UnParse(s);
                w.Write(this.surfaceFlags);
                w.Write(this.cubemapFlags);
                w.Write(this.reserved2);
            }
        }

        public enum FourCC : uint
        {
            DST1 = 0x31545344,
            DST3 = 0x33545344,
            DST5 = 0x35545344,
            DXT1 = 0x31545844,
            DXT3 = 0x33545844,
            DXT5 = 0x35545844
        }


        public enum RLEVersion : uint
        {
            RLE2 = 0x32454C52,
            RLES = 0x53454C52
        }

        public enum HeaderFlags : uint
        {
            Texture = 0x00001007, // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT 
            Mipmap = 0x00020000, // DDSD_MIPMAPCOUNT
            Volume = 0x00800000, // DDSD_DEPTH
            Pitch = 0x00000008, // DDSD_PITCH
            LinerSize = 0x00080000, // DDSD_LINEARSIZE
        }


        public class PixelFormat
        {
            public uint size { get { return 32; } }
            public PixelFormatFlags pixelFormatFlag = PixelFormatFlags.FourCC;
            public uint fourCC = (uint)FourCC.DXT5;
            public uint RGBBitCount { get; set; }
            public uint redBitMask { get; set; }
            public uint greenBitMask { get; set; }
            public uint blueBitMask { get; set; }
            public uint alphaBitMask { get; set; }


            public static uint StructureSize
            {
                get { return 8 * 4; }
            }

            public PixelFormat() { }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.size);
                w.Write((uint)this.pixelFormatFlag);
                w.Write(this.fourCC);
                w.Write(this.RGBBitCount);
                w.Write(redBitMask);
                w.Write(greenBitMask);
                w.Write(blueBitMask);
                w.Write(this.alphaBitMask);
            }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                uint size = r.ReadUInt32();
                if (size != this.size) throw new InvalidDataException(string.Format("Expected size: 0x{0:X8}, read 0x{1:X8}", this.size, size));
                this.pixelFormatFlag = (PixelFormatFlags)r.ReadUInt32();
                if (this.pixelFormatFlag != PixelFormatFlags.FourCC) throw new InvalidDataException("Bad pixel format flag");
                this.fourCC = r.ReadUInt32();
                if (this.fourCC != (uint)FourCC.DXT5) throw new InvalidDataException("Expected DTX5");
                this.RGBBitCount = r.ReadUInt32();
                this.redBitMask = r.ReadUInt32();
                this.greenBitMask = r.ReadUInt32();
                this.blueBitMask = r.ReadUInt32();
                this.alphaBitMask = r.ReadUInt32();
            }
        }

        [Flags]
        public enum PixelFormatFlags : uint
        {
            FourCC = 0x00000004,
            RGB = 0x00000040,
            RGBA = 0x00000041,
            Luminance = 0x00020000,
        }

        #endregion
    }

    public class RLEResourceTS4Handler : AResourceHandler
    {
        public RLEResourceTS4Handler()
        {
            this.Add(typeof(RLEResource), new List<string>(new string[] { "0x3453CF95", }));
        }
    }
}
