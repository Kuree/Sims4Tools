using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.IO.Compression;

namespace s4pi.Package
{
    /// <summary>
    /// Internal -- used by Package to handle compression routines
    /// </summary>
    internal static class Compression
    {
        static bool checking = Settings.Settings.Checking;

        public static byte[] UncompressStream(Stream stream, int filesize, int memsize)
        {
            using (MemoryStream uncompressed = new MemoryStream())
            {
                BinaryReader r = new BinaryReader(stream);
                long end = stream.Position + filesize;

                byte[] header = r.ReadBytes(2);

                if (checking) if (header.Length != 2)
                        throw new InvalidDataException("Hit unexpected end of file at " + stream.Position);

                if (header[0] != 0x78)
                    throw new InvalidDataException("Unrecognized compression format");

                using (MemoryStream compressed = new MemoryStream(r.ReadBytes(filesize)))
                {
                    using (DeflateStream ds = new DeflateStream(compressed, CompressionMode.Decompress))
                    {
                        ds.CopyTo(uncompressed);
                    }
                }

                long realsize = uncompressed.Length;

                if (checking) if (realsize != memsize)
                        throw new InvalidDataException(String.Format(
                            "Resource data indicates size does not match index at 0x{0}.  Read 0x{1}.  Expected 0x{2}.",
                            stream.Position.ToString("X8"), realsize.ToString("X8"), memsize.ToString("X8")));

                return uncompressed.ToArray();
            }
        }


        public static byte[] CompressStream(byte[] data)
        {

            byte[] result;
            using (MemoryStream ms = new MemoryStream(data))
            {
                bool smaller = _compress(ms, out result);
                return smaller ? result : data;
            }
        }

        internal static bool _compress(Stream uncompressed, out byte[] res)
        {
            using (MemoryStream result = new MemoryStream())
            {
                BinaryWriter w = new BinaryWriter(result);

                w.Write((byte)0x78);
                w.Write((byte)0xDA);
                using (DeflateStream ds = new DeflateStream(uncompressed, CompressionMode.Compress))
                {
                    ds.CopyTo(result);
                }

                if (result.Length < uncompressed.Length)
                {
                    res = result.ToArray();
                    return true;
                }
                else
                {
                    res = null;
                    return false;
                }
            }
        }
    }
}
