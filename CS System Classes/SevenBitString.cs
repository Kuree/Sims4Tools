using System;
using System.IO;

namespace System.Text
{
    /// <summary>
    /// Read and write a seven-bit encoded length-prefixed string in a given <see cref="Encoding"/> from or to a <see cref="Stream"/>.
    /// </summary>
    public static class SevenBitString
    {
        /// <summary>
        /// Read a string from <see cref="Stream"/> <paramref name="s"/> using <see cref="Encoding"/> <paramref name="enc"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> from which to read string.</param>
        /// <param name="enc"><see cref="Encoding"/> to use when reading.</param>
        /// <returns>A <see cref="String"/> value.</returns>
        public static string Read(Stream s, Encoding enc) { return new BinaryReader(s, enc).ReadString(); }

        /// <summary>
        /// Write a string to <see cref="Stream"/> <paramref name="s"/> using <see cref="Encoding"/> <paramref name="enc"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> to which to write string.</param>
        /// <param name="enc"><see cref="Encoding"/> to use when writing.</param>
        /// <param name="value">The <see cref="String"/> to write.</param>
        public static void Write(Stream s, Encoding enc, string value)
        {
            byte[] bytes = enc.GetBytes(value == null ? "" : value);
            System.IO.BinaryWriter w = new System.IO.BinaryWriter(s, enc);
            for (int i = bytes.Length; true; ) { w.Write((byte)((i & 0x7F) | (i > 0x7F ? 0x80 : 0))); i = i >> 7; if (i == 0) break; }//zero length? write a zero
            w.Write(bytes);
        }
    }

    /// <summary>
    /// Read and write a seven-bit encoded length-prefixed string in <see cref="Encoding.BigEndianUnicode"/> from or to a <see cref="Stream"/>.
    /// </summary>
    public static class BigEndianUnicodeString
    {
        /// <summary>
        /// Read a string from <see cref="Stream"/> <paramref name="s"/> using <see cref="Encoding.BigEndianUnicode"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> from which to read string.</param>
        /// <returns>A <see cref="String"/> value.</returns>
        public static string Read(Stream s) { return SevenBitString.Read(s, Encoding.BigEndianUnicode); }

        /// <summary>
        /// Write a string to <see cref="Stream"/> <paramref name="s"/> using <see cref="Encoding.BigEndianUnicode"/>.
        /// </summary>
        /// <param name="s"><see cref="Stream"/> to which to write string.</param>
        /// <param name="value">The <see cref="String"/> to write.</param>
        public static void Write(Stream s, string value) { SevenBitString.Write(s, Encoding.BigEndianUnicode, value); }
    }
}
