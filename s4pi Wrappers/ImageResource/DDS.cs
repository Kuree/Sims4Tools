using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s4pi.ImageResource
{
    public enum FourCC : uint
    {
        DST1 = 0x31545344,
        DST3 = 0x33545344,
        DST5 = 0x35545344,
        DXT1 = 0x31545844,
        DXT3 = 0x33545844,
        DXT5 = 0x35545844
    }

    public class PixelFormat
    {
        public uint size { get { return 32; } }
        public PixelFormatFlags pixelFormatFlag = PixelFormatFlags.FourCC;
        public FourCC Fourcc { get { return this.fourcc; } internal set { this.fourcc = value; } }
        public uint RGBBitCount { get; set; }
        public uint redBitMask { get; set; }
        public uint greenBitMask { get; set; }
        public uint blueBitMask { get; set; }
        public uint alphaBitMask { get; set; }
        private FourCC fourcc = FourCC.DXT5;


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
            w.Write((uint)this.Fourcc);
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
            //this.pixelFormatFlag = (PixelFormatFlags)r.ReadUInt32();
            //if ((this.pixelFormatFlag & PixelFormatFlags.FourCC) != PixelFormatFlags.FourCC) throw new InvalidDataException("Bad pixel format flag");
            uint pixelFormatFlag = r.ReadUInt32();
            if (!Enum.IsDefined(typeof(PixelFormatFlags), pixelFormatFlag)) throw new InvalidDataException("Bad pixel format flag"); else this.pixelFormatFlag = (PixelFormatFlags) pixelFormatFlag;
            uint fourCC = r.ReadUInt32();
            if (!Enum.IsDefined(typeof(FourCC), fourCC)) throw new InvalidDataException(string.Format("Unexpected data, read 0x{0:xX8}", fourCC)); else this.fourcc = (FourCC)fourCC;
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
}
