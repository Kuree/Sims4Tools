using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DDSHeader = s4pi.ImageResource.RLEResource.RLEInfo; // need to be improved
using s4pi.Interfaces; 

namespace s4pi.ImageResource
{
    public class DSTResource : AResource
    {
        const Int32 recommendedApiVersion = 1;

        #region AApiVersionedFields
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion
        private DDSHeader header;
        private byte[] data;

        public DSTResource(int APIversion, Stream s) : base(APIversion, s) { if (s == null) { OnResourceChanged(this, EventArgs.Empty); } else { Parse(s); } }

        protected void Parse(Stream s)
        {
            this.header = new DDSHeader();
            this.header.Parse(s);
            if (header.pixelFormat.Fourcc != FourCC.DST1 &&
                         header.pixelFormat.Fourcc != FourCC.DST3 &&
                         header.pixelFormat.Fourcc != FourCC.DST5)
            {
                throw new InvalidDataException("Texture does not need to be un-shuffled");
            }
            this.data = new byte[s.Length];
            s.Read(this.data, 0, (int)s.Length);
        }

        public Stream ToDDS()
        {
            using (MemoryStream ms = new MemoryStream(this.data))
            {
                return Unshuffle(this.header, ms);
            }
        }

        protected override Stream UnParse()
        {
            return new MemoryStream(this.data);
        }

        private static void Shuffle(DDSHeader header, Stream input, Stream output)
        {
            throw new NotImplementedException();
        }

        private Stream Unshuffle(DDSHeader header, Stream s)
        {
            const int dataOffset = 128;
            var dataSize = (int)(s.Length - dataOffset);

            s.Position = dataOffset;
            BinaryReader r = new BinaryReader(s);

            MemoryStream result = new MemoryStream();
            BinaryWriter w = new BinaryWriter(result);
            var temp = r.ReadBytes(dataSize);

            if (header.pixelFormat.Fourcc == FourCC.DST1)
            {
                header.pixelFormat.Fourcc = FourCC.DXT1;
                header.UnParse(result);

                var blockOffset2 = 0;
                var blockOffset3 = blockOffset2 + (dataSize >> 1);

                // probably a better way to do this
                var count = (blockOffset3 - blockOffset2) / 4;

                for (int i = 0; i < count; i++)
                {
                    result.Write(temp, blockOffset2, 4);
                    result.Write(temp, blockOffset3, 4);
                    blockOffset2 += 4;
                    blockOffset3 += 4;
                }
            }
            else if (header.pixelFormat.Fourcc == FourCC.DST3) // DST3
            {
                header.pixelFormat.Fourcc = FourCC.DXT3;
                header.UnParse(result);

                var blockOffset0 = 0;
                var blockOffset2 = blockOffset0 + (dataSize >> 1);
                var blockOffset3 = blockOffset2 + (dataSize >> 2);

                throw new NotImplementedException("no samples");
            }
            else if (header.pixelFormat.Fourcc ==  FourCC.DST5) // DST5
            {
                header.pixelFormat.Fourcc = FourCC.DXT5;
                header.UnParse(result);

                var blockOffset0 = 0;
                var blockOffset2 = blockOffset0 + (dataSize >> 3);
                var blockOffset1 = blockOffset2 + (dataSize >> 2);
                var blockOffset3 = blockOffset1 + (6 * dataSize >> 4);

                // probably a better way to do this
                var count = (blockOffset2 - blockOffset0) / 2;

                for (int i = 0; i < count; i++)
                {
                    result.Write(temp, blockOffset0, 2);
                    result.Write(temp, blockOffset1, 6);
                    result.Write(temp, blockOffset2, 4);
                    result.Write(temp, blockOffset3, 4);

                    blockOffset0 += 2;
                    blockOffset1 += 6;
                    blockOffset2 += 4;
                    blockOffset3 += 4;
                }
            }
            result.Position = 0;
            return result;
        }
    }

    public class DSTResourceHandler : AResourceHandler
    {
        public DSTResourceHandler()
        {
            this.Add(typeof(DSTResource), new List<string>(new string[] { "0x00B2D882", }));
        }
    }
}
