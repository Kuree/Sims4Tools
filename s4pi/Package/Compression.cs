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

                bool useDEFLATE = true;
                byte[] oldUncompressedData = null;

                if (header[0] == 0x78)
                {
                    useDEFLATE = true;
                }
                else if (header[1] == 0xFB)
                {
                    useDEFLATE = false;
                }
                else
                {
                    throw new InvalidDataException("Unrecognized compression format");
                }

                if (useDEFLATE)
                {
                    using (MemoryStream compressed = new MemoryStream(r.ReadBytes(filesize)))
                    {
                        using (DeflateStream ds = new DeflateStream(compressed, CompressionMode.Decompress))
                        {
                            ds.CopyTo(uncompressed);
                        }
                    }
                }
                else
                {
                    oldUncompressedData = OldDecompress(stream, header[0]);
                }

                long realsize = useDEFLATE ? uncompressed.Length : oldUncompressedData.Length;

                if (checking) if (realsize != memsize)
                        throw new InvalidDataException(String.Format(
                            "Resource data indicates size does not match index at 0x{0}.  Read 0x{1}.  Expected 0x{2}.",
                            stream.Position.ToString("X8"), realsize.ToString("X8"), memsize.ToString("X8")));

                return useDEFLATE ? uncompressed.ToArray() : oldUncompressedData;
            }
        }

        internal static byte[] OldDecompress(Stream compressed, byte compressionType)
        {
            BinaryReader r = new BinaryReader(compressed);

            bool type = compressionType != 0x80;

            byte[] sizeArray = new byte[4];


            for (int i = type ? 2 : 3; i >= 0; i--)
                sizeArray[i] = r.ReadByte();

            byte[] Data = new byte[BitConverter.ToInt32(sizeArray, 0)];

            int position = 0;
            while (position < Data.Length)
            {
                byte byte0 = r.ReadByte();
                if (byte0 <= 0x7F)
                {
                    // Read info
                    byte byte1 = r.ReadByte();
                    int numPlainText = byte0 & 0x03;
                    int numToCopy = ((byte0 & 0x1C) >> 2) + 3;
                    int copyOffest = ((byte0 & 0x60) << 3) + byte1 + 1;

                    CopyPlainText(ref r, ref Data, numPlainText, ref position);

                    CopyCompressedText(ref r, ref Data, numToCopy, ref position, copyOffest);

                }
                else if (byte0 <= 0XBF && byte0 > 0x7F)
                {
                    // Read info
                    byte byte1 = r.ReadByte();
                    byte byte2 = r.ReadByte();
                    int numPlainText = ((byte1 & 0xC0) >> 6) & 0x03;
                    int numToCopy = (byte0 & 0x3F) + 4;
                    int copyOffest = ((byte1 & 0x3F) << 8) + byte2 + 1;

                    CopyPlainText(ref r, ref Data, numPlainText, ref position);

                    CopyCompressedText(ref r, ref Data, numToCopy, ref position, copyOffest);
                }
                else if (byte0 <= 0xDF && byte0 > 0xBF)
                {
                    // Read info
                    byte byte1 = r.ReadByte();
                    byte byte2 = r.ReadByte();
                    byte byte3 = r.ReadByte();
                    int numPlainText = byte0 & 0x03;
                    int numToCopy = ((byte0 & 0x0C) << 6) + byte3 + 5;
                    int copyOffest = ((byte0 & 0x10) << 12) + (byte1 << 8) + byte2 + 1;

                    CopyPlainText(ref r, ref Data, numPlainText, ref position);

                    CopyCompressedText(ref r, ref Data, numToCopy, ref position, copyOffest);
                }
                else if (byte0 <= 0xFB && byte0 > 0xDF)
                {
                    // Read info
                    int numPlainText = ((byte0 & 0x1F) << 2) + 4;

                    CopyPlainText(ref r, ref Data, numPlainText, ref position);

                }
                else if (byte0 <= 0xFF && byte0 > 0xFB)
                {
                    // Read info
                    int numPlainText = (byte0 & 0x03);

                    CopyPlainText(ref r, ref Data, numPlainText, ref position);
                }
            }

            return Data;
        }

        static void CopyPlainText(ref BinaryReader r, ref byte[] Data, int numPlainText, ref int position)
        {
            // Copy data one at a time
            for (int i = 0; i < numPlainText; position++, i++)
            {
                Data[position] = r.ReadByte();
            }
        }

        static void CopyCompressedText(ref BinaryReader r, ref byte[] Data, int numToCopy, ref int position, int copyOffest)
        {
            int currentPosition = position;
            // Copy data one at a time
            for (int i = 0; i < numToCopy; i++, position++)
            {
                Data[position] = Data[currentPosition - copyOffest + i];
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
