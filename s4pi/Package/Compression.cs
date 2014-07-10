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

        public static void Dechunk(Stream stream, BinaryWriter bw)
        {
            BinaryReader r = new BinaryReader(stream);
            int copysize = 0;
            int copyoffset = 0;
            int datalen;
            byte[] data;

            byte packing = r.ReadByte();

            #region Compressed
            if (packing < 0x80) // 0.......; new data 3; copy data 10 (min 3); offset 1024
            {
                data = r.ReadBytes(1);
                if (checking) if (data.Length != 1)
                        throw new InvalidDataException("Hit unexpected end of file at " + stream.Position);
                datalen = packing & 0x03;
                copysize = ((packing >> 2) & 0x07) + 3;
                copyoffset = (((packing << 3) & 0x300) | data[0]) + 1;
            }
            else if (packing < 0xC0) // 10......; new data 3; copy data 67 (min 4); offset 16384
            {
                data = r.ReadBytes(2);
                if (checking) if (data.Length != 2)
                        throw new InvalidDataException("Hit unexpected end of file at " + stream.Position);
                datalen = (data[0] >> 6) & 0x03;
                copysize = (packing & 0x3F) + 4;
                copyoffset = (((data[0] << 8) & 0x3F00) | data[1]) + 1;
            }
            else if (packing < 0xE0) // 110.....; new data 3; copy data 1028 (min 5); offset 131072
            {
                data = r.ReadBytes(3);
                if (checking) if (data.Length != 3)
                        throw new InvalidDataException("Hit unexpected end of file at " + stream.Position);
                datalen = packing & 0x03;
                copysize = (((packing << 6) & 0x300) | data[2]) + 5;
                copyoffset = (((packing << 12) & 0x10000) | data[0] << 8 | data[1]) + 1;
            }
            #endregion
            #region Uncompressed
            else if (packing < 0xFC) // 1110000 - 11101111; new data 4-128
                datalen = (((packing & 0x1F) + 1) << 2);
            else // 111111..; new data 3
                datalen = packing & 0x03;
            #endregion

            if (datalen > 0)
            {
                data = r.ReadBytes(datalen);
                if (checking) if (data.Length != datalen)
                        throw new InvalidDataException("Hit unexpected end of file at " + stream.Position);
                bw.Write(data);
            }

            if (checking) if (copyoffset > bw.BaseStream.Position)
                throw new InvalidDataException(String.Format("Invalid copy offset 0x{0:X8} at {1}.", copyoffset, stream.Position));

            if (copysize < copyoffset && copyoffset > 8) CopyA(bw.BaseStream, copyoffset, copysize); else CopyB(bw.BaseStream, copyoffset, copysize);
        }

        static void CopyA(Stream s, int offset, int len)
        {
            while (len > 0)
            {
                long dst = s.Position;
                byte[] b = new byte[Math.Min(offset, len)];
                len -= b.Length;

                s.Position -= offset;
                s.Read(b, 0, b.Length);

                s.Position = dst;
                s.Write(b, 0, b.Length);
            }
        }

        static void CopyB(Stream s, int offset, int len)
        {
            while (len > 0)
            {
                long dst = s.Position;
                len--;

                s.Position -= offset;
                byte b = (byte)s.ReadByte();

                s.Position = dst;
                s.WriteByte(b);
            }
        }

        public static byte[] CompressStream(byte[] data)
        {
#if true
            byte[] res;
            bool smaller = Tiger.DBPFCompression.Compress(data, out res);
            return smaller ? res : data;
#else
            if (data.Length < 10) return data;

            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            int len = 8;
            if (data.LongLength >= 0x800000000000) { len = 8; }
            else if (data.LongLength >= 0x000080000000) { len = 6; }
            else if (data.LongLength >= 0x000001000000) { len = 4; }
            else { len = 3; }

            bw.Write((ushort)(0xFB10 | (len == 8 ? 0x81 : len == 6 ? 0x01 : len == 4 ? 0x80 : 0x00)));
            byte[] reallength = BitConverter.GetBytes(data.LongLength);
            for (int i = len; i > 0; i--) bw.Write(reallength[i - 1]);

            int pos = 0;
            for (; data.Length - pos >= 4; pos += Enchunk(bw, data, pos)) { }
            WriteChunk(bw, data, pos, data.Length - pos, -1, 0);//EOF mark

            bw.Flush();
            ms.Position = 0;

            return (ms.Length < data.Length) ? (new BinaryReader(ms)).ReadBytes((int)ms.Length) : data;
#endif
        }

#if true
#else
        public static int Enchunk(BinaryWriter bw, byte[] buffer, int pos)
        {
            //if (buffer.Length - pos < 4)
            //    return WriteChunk(bw, buffer, pos, buffer.Length - pos, -1, 0);//EOF!

            if (buffer.Length - pos < 8)
                return WriteChunk(bw, buffer, pos, (buffer.Length - pos) & ~0x03, -1, 0);//too near EOF!

            int copysize = 3; // don't try to compress less than 3 bytes
            int uncsize = pos < 3 ? 3 : 0; // need at least copysize uncompressed bytes to copy!
            int buflen = (buffer.Length & ~0x03) - 1; // truncate to multiple of four and sub one as it's always needed


            int hit = Search(buffer, pos + uncsize, copysize, -1);
            while (hit == -1 /*not found*/ && uncsize < 0x70 /*max uncomp*/ && pos + uncsize + copysize < buflen /*EOF!*/)
            {
                uncsize++; /*skip*/
                hit = Search(buffer, pos + uncsize, copysize, -1); /*keep trying*/
            }

            int copypos = hit; /*remember last found position, if any*/
            if (hit != -1) /*found*/
            {
                while (copysize <= 0x403 /*max buffer - 1(lookahead)*/
                    && copysize < pos + uncsize /*max avail data*/
                    && pos + uncsize + copysize < buflen /*EOF!*/)
                {
                    hit = Search(buffer, pos + uncsize, copysize + 1 /*lookahead*/, copypos);
                    if (hit == -1) break; /*no more hits*/
                    /*record success*/
                    copysize++;
                    copypos = hit;
                }
            }
            else
                if (uncsize + copysize <= 0x70) uncsize += copysize;

            

            /*
             * uncsize -- bytes skipped before match, if any
             * copypos -- -1: nothing found; else position in buffer
             * copysize -- if copypos != -1, length of data matched
             * precomp -- uncompressed data passed with compressed
             */

            int precomp = uncsize & 0x03; // uncsize must be multiple of 4
            uncsize &= ~0x03;

            /*Write uncompressed*/
            if (uncsize > 0)
                uncsize = WriteChunk(bw, buffer, pos, uncsize, -1, 0);

            /*Write compressed*/
            if (/*precomp != 0 || */copypos != -1)
                uncsize += WriteChunk(bw, buffer, pos + uncsize, precomp, copypos, copypos == -1 ? 0 : copysize);

            return uncsize;
            /**/
        }

        /// <summary>
        /// Find a byte string in a byte array, return position of least distant match
        /// </summary>
        /// <param name="buffer">Byte array to search</param>
        /// <param name="keypos">Position in <paramref name="buffer"/> of key to find</param>
        /// <param name="keylen">Length of key to find</param>
        /// <param name="start">Position in <paramref name="buffer"/> to start searching, -1 to search from <paramref name="keylen"/> bytes before <paramref name="keypos"/></param>
        /// <returns></returns>
        static int Search(byte[] buffer, int keypos, int keylen, int start)
        {
            if (checking) if (keypos < keylen) // Otherwise we start before the start of the buffer
                    throw new InvalidOperationException(
                        String.Format("At position 0x{0:X8}, requested key length 0x{1:X4} exceeds current position.",
                        keypos, keylen));

            if (checking) if (keypos + keylen - 1 > buffer.Length) // The end of the key must be within the buffer
                    throw new InvalidOperationException(
                        String.Format("At position 0x{0:X8}, requested key length 0x{1:X4} exceeds input data length 0x{2:X8}.",
                        keypos, keylen, buffer.Length));

            //if (start == -1) start = keypos - keylen; // need at least keylen bytes before keypos to compare and copy
            if (start == -1) start = keypos - 1; // have to start with data already output

            /*if (checking) if (start + keylen > keypos)
                    throw new InvalidOperationException(
                        String.Format("At position 0x{0:X8}, requested start position 0x{1:X4} plus key length 0x{2:X4} exceeds current position.",
                        start, keylen, keypos));/**/

            int limit = keylen < 4 ? 1024 : keylen < 5 ? 16384 : 131072;

            retry:
            /*find first byte*/
            while (buffer[start] != buffer[keypos]) /*not found*/
            {
                if (start == 0 || keypos - start == limit) return -1;
                start--;
            }

            /*found first byte; check remainder*/
            for (int i = 1; i < keylen; i++)
            {
                if (buffer[start + i] == buffer[keypos + i]) continue; /*found*/
                if (start == 0 || keypos - start == limit) return -1; /*out of data*/
                start--;
                goto retry;
            }
            return start;
        }

        static int WriteChunk(BinaryWriter bw, byte[] data, int posn, int datalen, int copypos, int copysize)
        {
            #region Assertions
            if (checking) if (posn + datalen > data.Length)
                    throw new InvalidOperationException(
                        String.Format("At position 0x{0:X8}, requested uncompressed length 0x{1:X4} exceeds input data length 0x{2:X8}.",
                        posn, datalen, data.Length));
            #endregion

            byte packing = 0;
            byte[] parm = null;
            int retval = datalen + copysize; // save copysize from the ravages of compression

            if (copypos == -1)
            {
                #region No compression

                #region Assertions
                if (checking)
                {
                    if (datalen > 112)
                        throw new InvalidOperationException(
                            String.Format("At position 0x{0:X8}, requested uncompressed length 0x{1:X4} greater than 112.",
                            posn, datalen));

                    if (copysize != 0)
                        throw new ArgumentException(
                            String.Format("At position 0x{0:X8}, must pass zero copysize (got 0x{1:X4}) when copypos is -1.",
                            posn, copysize));
                }
                #endregion

                if (datalen > 3)
                {
                    #region Assertions
                    if (checking) if ((datalen & 0x03) != 0)
                            throw new InvalidOperationException(
                                String.Format("At position 0x{0:X8}, requested uncompressed length 0x{1:X4} not a multiple of 4.",
                                posn, datalen));
                    if (checking) if (datalen > 0x70)
                            throw new InvalidOperationException(
                                String.Format("At position 0x{0:X8}, requested uncompressed length 0x{1:X4} greater than 0x70.",
                                posn, datalen));
                    #endregion

                    packing = (byte)((datalen >> 2) - 1); //00000000 - 01110000 >> 00000000 - 00001111
                    packing |= 0xE0; // 0000aaaa >> 1110aaaa
                }
                else // Should only happen at end of file
                {
                    #region Assertions
                    if (checking) if (data.Length - posn > 3)
                            throw new InvalidOperationException(
                                String.Format("At position 0x{0:X8}, requested end of file with 0x{1:X4} bytes remaining: must be 3 or less.",
                                posn, data.Length - posn));
                    #endregion
                    packing = (byte)datalen;//(uncsize & 0x03)
                    packing |= 0xFC;
                }
                #endregion
            }
            else
            {
                #region Compression
                int copyoffset = posn + datalen - copypos - 1;

                #region Assertions
                if (checking)
                {
                    if (copypos > posn + datalen)
                        throw new InvalidOperationException(
                            String.Format("At position 0x{0:X8}, invalid copy position 0x{1:X8}.",
                            posn + datalen, copypos));

                    /*if (copypos + copysize > posn + datalen)
                        throw new InvalidOperationException(
                            String.Format("At position 0x{0:X8}, invalid copy length 0x{1:X4}.",
                            posn + datalen, copysize, copypos));/**/

                    if (copyoffset > 0x1ffff)
                        throw new InvalidOperationException(
                            String.Format("At position 0x{0:X8}, requested copy offset 0x{1:X8} exceeds 0x1ffff.",
                            posn, copyoffset));

                    if (copyoffset + 1 > posn + datalen)
                        throw new InvalidOperationException(
                            String.Format("At position 0x{0:X8}, requested copy offset 0x{1:X8} exceeds uncompressed position.",
                            posn, copyoffset));

                    if (datalen > 0x03)
                        throw new InvalidOperationException(
                            String.Format("At position 0x{0:X8}, requested uncompressed length 0x{1:X4} greater than 3.",
                            posn, datalen));
                }
                #endregion

                if (copyoffset < 0x400 && copysize <= 0x0A)
                {
                    parm = new byte[1];

                    packing = (byte)((copyoffset & 0x300) >> 3); // aa ........ >> 0aa.....
                    parm[0] = (byte)(copyoffset & 0xFF);

                    copysize -= 3;
                    packing |= (byte)((copysize & 0x07) << 2); // .....bbb >> ...bbb..

                    packing |= (byte)(datalen & 0x03); // >> ......cc
                }
                else if (copyoffset < 0x4000 && copysize <= 0x43)
                {
                    parm = new byte[2];

                    parm[0] = (byte)((copyoffset & 0x3f00) >> 8);
                    parm[1] = (byte)(copyoffset & 0xFF);

                    copysize -= 4;
                    packing = (byte)(copysize & 0x3F);

                    parm[0] |= (byte)((datalen & 0x03) << 6);

                    packing |= 0x80;
                }
                else // copyoffset < 0x20000 && copysize <= 0x404
                {
                    parm = new byte[3];

                    packing = (byte)((copyoffset & 0x10000) >> 12);
                    parm[0] = (byte)((copyoffset & 0x0FF00) >> 8);
                    parm[1] = (byte)(copyoffset & 0x000FF);

                    copysize -= 5;
                    packing |= (byte)((copysize & 0x300) >> 6);
                    parm[2] = (byte)(copysize & 0x0FF);

                    packing |= (byte)(datalen & 0x03);

                    packing |= 0xC0;
                }
                #endregion
            }

            bw.Write(packing);
            if (parm != null) bw.Write(parm);
            if (datalen > 0) bw.BaseStream.Write(data, posn, datalen);

            return retval;
        }
#endif
    }
}




/*
 * The following code was provided by Tiger
**/

namespace Tiger
{
    class DBPFCompression
    {
        public DBPFCompression(int level)
        {
            mTracker = CreateTracker(level, out mBruteForceLength);
        }

        public DBPFCompression(int blockinterval, int lookupstart, int windowlength, int bucketdepth, int bruteforcelength)
        {
            mTracker = CreateTracker(blockinterval, lookupstart, windowlength, bucketdepth);
            mBruteForceLength = bruteforcelength;
        }

        private int mBruteForceLength;
        private IMatchtracker mTracker;

        private byte[] mData;

        private int mSequenceSource;
        private int mSequenceLength;
        private int mSequenceDest;
        private bool mSequenceFound;

        public static bool Compress(byte[] data, out byte[] compressed)
        {
            DBPFCompression compressor = new DBPFCompression(5);
            compressed = compressor.Compress(data);
            return (compressed != null);
        }

        public static bool Compress(byte[] data, out byte[] compressed, int level)
        {
            DBPFCompression compressor = new DBPFCompression(level);
            compressed = compressor.Compress(data);
            return (compressed != null);
        }

        public byte[] Compress(byte[] data)
        {
            bool endisvalid = false;
            List<byte[]> compressedchunks = new List<byte[]>();
            int compressedidx = 0;
            int compressedlen = 0;

            if (data.Length < 16 || data.LongLength > UInt32.MaxValue)
                return null;

            mData = data;

            try
            {
                int lastbytestored = 0;

                while (compressedidx < data.Length)
                {
                    if (data.Length - compressedidx < 4)
                    {
                        // Just copy the rest
                        byte[] chunk = new byte[data.Length - compressedidx + 1];
                        chunk[0] = (byte)(0xFC | (data.Length - compressedidx));
                        Array.Copy(data, compressedidx, chunk, 1, data.Length - compressedidx);
                        compressedchunks.Add(chunk);
                        compressedidx += chunk.Length - 1;
                        compressedlen += chunk.Length;

                        endisvalid = true;
                        continue;
                    }

                    while (compressedidx > lastbytestored - 3)
                        mTracker.Addvalue(data[lastbytestored++]);

                    // Search ahead in blocks of 4 bytes for a match until one is found
                    // Record the best match if multiple are found
                    mSequenceSource = 0;
                    mSequenceLength = 0;
                    mSequenceDest = int.MaxValue;
                    mSequenceFound = false;
                    do
                    {
                        for (int loop = 0; loop < 4; loop++)
                        {
                            if (lastbytestored < data.Length)
                                mTracker.Addvalue(data[lastbytestored++]);
                            FindSequence(lastbytestored - 4);
                        }
                    }
                    while (!mSequenceFound && lastbytestored + 4 <= data.Length);

                    if (!mSequenceFound)
                        mSequenceDest = mData.Length;

                    // If the next match is more than four bytes away, put in codes to read uncompressed data
                    while (mSequenceDest - compressedidx >= 4)
                    {
                        int tocopy = (mSequenceDest - compressedidx) & ~3;
                        if (tocopy > 112)
                            tocopy = 112;

                        byte[] chunk = new byte[tocopy + 1];
                        chunk[0] = (byte)(0xE0 | ((tocopy >> 2) - 1));
                        Array.Copy(data, compressedidx, chunk, 1, tocopy);
                        compressedchunks.Add(chunk);
                        compressedidx += tocopy;
                        compressedlen += chunk.Length;
                    }

                    if (mSequenceFound)
                    {
                        byte[] chunk = null;
                        /*
                         * 00-7F  0oocccpp oooooooo
                         *   Read 0-3
                         *   Copy 3-10
                         *   Offset 0-1023
                         *   
                         * 80-BF  10cccccc ppoooooo oooooooo
                         *   Read 0-3
                         *   Copy 4-67
                         *   Offset 0-16383
                         *   
                         * C0-DF  110cccpp oooooooo oooooooo cccccccc
                         *   Read 0-3
                         *   Copy 5-1028
                         *   Offset 0-131071
                         *   
                         * E0-FC  111ppppp
                         *   Read 4-128 (Multiples of 4)
                         *   
                         * FD-FF  111111pp
                         *   Read 0-3
                         */
                        //if (FindRunLength(data, seqstart, compressedidx + seqidx) < seqlength)
                        //{
                        //    break;
                        //}
                        while (mSequenceLength > 0)
                        {
                            int thislength = mSequenceLength;
                            if (thislength > 1028)
                                thislength = 1028;
                            mSequenceLength -= thislength;

                            int offset = mSequenceDest - mSequenceSource - 1;
                            int readbytes = mSequenceDest - compressedidx;

                            mSequenceSource += thislength;
                            mSequenceDest += thislength;

                            if (thislength > 67 || offset > 16383)
                            {
                                chunk = new byte[readbytes + 4];
                                chunk[0] = (byte)(0xC0 | readbytes | (((thislength - 5) >> 6) & 0x0C) | ((offset >> 12) & 0x10));
                                chunk[1] = (byte)((offset >> 8) & 0xFF);
                                chunk[2] = (byte)(offset & 0xFF);
                                chunk[3] = (byte)((thislength - 5) & 0xFF);
                            }
                            else if (thislength > 10 || offset > 1023)
                            {
                                chunk = new byte[readbytes + 3];
                                chunk[0] = (byte)(0x80 | ((thislength - 4) & 0x3F));
                                chunk[1] = (byte)(((readbytes << 6) & 0xC0) | ((offset >> 8) & 0x3F));
                                chunk[2] = (byte)(offset & 0xFF);
                            }
                            else
                            {
                                chunk = new byte[readbytes + 2];
                                chunk[0] = (byte)((readbytes & 0x3) | (((thislength - 3) << 2) & 0x1C) | ((offset >> 3) & 0x60));
                                chunk[1] = (byte)(offset & 0xFF);
                            }

                            if (readbytes > 0)
                                Array.Copy(data, compressedidx, chunk, chunk.Length - readbytes, readbytes);

                            compressedchunks.Add(chunk);
                            compressedidx += thislength + readbytes;
                            compressedlen += chunk.Length;
                        }
                    }
                }

                if (compressedlen + 6 < data.Length)
                {
                    int chunkpos;
                    byte[] compressed;

                    if (data.Length > 0xFFFFFF)
                    {
                        // Activate the large data bit for > 16mb uncompressed data
                        compressed = new byte[compressedlen + 6 + (endisvalid ? 0 : 1)];
                        compressed[0] = 0x10 | 0x80; // 0x80 = length is 4 bytes
                        compressed[1] = 0xFB;
                        compressed[2] = (byte)(data.Length >> 24);
                        compressed[3] = (byte)(data.Length >> 16);
                        compressed[4] = (byte)(data.Length >> 8);
                        compressed[5] = (byte)(data.Length);
                        chunkpos = 6;
                    }
                    else
                    {
                        compressed = new byte[compressedlen + 5 + (endisvalid ? 0 : 1)];
                        compressed[0] = 0x10;
                        compressed[1] = 0xFB;
                        compressed[2] = (byte)(data.Length >> 16);
                        compressed[3] = (byte)(data.Length >> 8);
                        compressed[4] = (byte)(data.Length);
                        chunkpos = 5;
                    }

                    for (int loop = 0; loop < compressedchunks.Count; loop++)
                    {
                        Array.Copy(compressedchunks[loop], 0, compressed, chunkpos, compressedchunks[loop].Length);
                        chunkpos += compressedchunks[loop].Length;
                    }
                    if (!endisvalid)
                        compressed[compressed.Length - 1] = 0xfc;
                    return compressed;
                }

                return null;
            }
            finally
            {
                mData = null;
                mTracker.Reset();
            }
        }

        private void FindSequence(int startindex)
        {
            // Try a straight forward brute force first
            int end = -mBruteForceLength;
            if (startindex < mBruteForceLength)
                end = -startindex;

            byte searchforbyte = mData[startindex];

            for (int loop = -1; loop >= end && mSequenceLength < 1028; loop--)
            {
                byte curbyte = mData[loop + startindex];
                if (curbyte != searchforbyte)
                    continue;

                int len = FindRunLength(startindex + loop, startindex);

                if (len <= mSequenceLength
                    || len < 3
                    || len < 4 && loop <= -1024
                    || len < 5 && loop <= -16384)
                    continue;

                mSequenceFound = true;
                mSequenceSource = startindex + loop;
                mSequenceLength = len;
                mSequenceDest = startindex;
            }

            // Use the look-up table next
            int matchloc;
            if (mSequenceLength < 1028 && mTracker.FindMatch(out matchloc))
            {
                do
                {
                    int len = FindRunLength(matchloc, startindex);
                    if (len < 5)
                        continue;

                    mSequenceFound = true;
                    mSequenceSource = matchloc;
                    mSequenceLength = len;
                    mSequenceDest = startindex;
                }
                while (mSequenceLength < 1028 && mTracker.Nextmatch(out matchloc));
            }
        }

        private int FindRunLength(int src, int dst)
        {
            int endsrc = src + 1;
            int enddst = dst + 1;
            while (enddst < mData.Length && mData[endsrc] == mData[enddst] && enddst - dst < 1028)
            {
                endsrc++;
                enddst++;
            }

            return enddst - dst;
        }

        private interface IMatchtracker
        {
            bool FindMatch(out int where);
            bool Nextmatch(out int where);
            void Addvalue(byte val);
            void Reset();
        }

        static IMatchtracker CreateTracker(int blockinterval, int lookupstart, int windowlength, int bucketdepth)
        {
            if (bucketdepth <= 1)
                return new SingledepthMatchTracker(blockinterval, lookupstart, windowlength);
            else
                return new DeepMatchTracker(blockinterval, lookupstart, windowlength, bucketdepth);
        }

        static IMatchtracker CreateTracker(int level, out int bruteforcelength)
        {
            switch (level)
            {
                case 0:
                    bruteforcelength = 0;
                    return CreateTracker(4, 0, 16384, 1);
                case 1:
                    bruteforcelength = 0;
                    return CreateTracker(2, 0, 32768, 1);
                case 2:
                    bruteforcelength = 0;
                    return CreateTracker(1, 0, 65536, 1);
                case 3:
                    bruteforcelength = 0;
                    return CreateTracker(1, 0, 131000, 2);
                case 4:
                    bruteforcelength = 16;
                    return CreateTracker(1, 16, 131000, 2);
                case 5:
                    bruteforcelength = 16;
                    return CreateTracker(1, 16, 131000, 5);
                case 6:
                    bruteforcelength = 32;
                    return CreateTracker(1, 32, 131000, 5);
                case 7:
                    bruteforcelength = 32;
                    return CreateTracker(1, 32, 131000, 10);
                case 8:
                    bruteforcelength = 64;
                    return CreateTracker(1, 64, 131000, 10);
                case 9:
                    bruteforcelength = 128;
                    return CreateTracker(1, 128, 131000, 20);
                default:
                    return CreateTracker(5, out bruteforcelength);
            }
        }

        private class SingledepthMatchTracker : IMatchtracker
        {
            public SingledepthMatchTracker(int blockinterval, int lookupstart, int windowlength)
            {
                mInterval = blockinterval;
                if (lookupstart > 0)
                {
                    mPendingValues = new UInt32[lookupstart / blockinterval];
                    mQueueLength = mPendingValues.Length * blockinterval;
                }
                else
                    mQueueLength = 0;
                mInsertedValues = new UInt32[windowlength / blockinterval - lookupstart / blockinterval];
                mWindowStart = -(mInsertedValues.Length + lookupstart / blockinterval) * blockinterval - 4;
            }

            public void Reset()
            {
                mLookupTable.Clear();
                mRunningValue = 0;

                mRollingInterval = 0;
                mWindowStart = -(mInsertedValues.Length + (mPendingValues != null ? mPendingValues.Length : 0)) * mInterval - 4;
                mDataLength = 0;

                mInitialized = false;
                mInsertLocation = 0;
                mPendingOffset = 0;

                // No need to clear the arrays, the values will be ignored by the next time around
            }

            // Current 32 bit value of the last 4 bytes
            private UInt32 mRunningValue;

            // How often to insert into the table
            private int mInterval;
            // Avoid division by using a rolling count instead
            private int mRollingInterval;

            // How many bytes to queue up before adding it to the lookup table
            private int mQueueLength;

            // Queued up values for future matches
            private UInt32[] mPendingValues;
            private int mPendingOffset;

            // Bytes processed so far
            private int mDataLength;
            private int mWindowStart;

            // Four or more bytes read
            private bool mInitialized;

            // Values values pending removal
            private UInt32[] mInsertedValues;
            private int mInsertLocation;

            // Hash of seen values
            private Dictionary<UInt32, int> mLookupTable = new Dictionary<uint, int>();

            #region IMatchtracker Members

            // Never more than one match with a depth of 1
            public bool Nextmatch(out int where)
            {
                where = 0;
                return false;
            }

            public void Addvalue(byte val)
            {
                if (mInitialized)
                {
                    mRollingInterval++;
                    // Time to add a value to the table
                    if (mRollingInterval == mInterval)
                    {
                        mRollingInterval = 0;
                        // Remove a value from the table if the window just rolled past it
                        if (mWindowStart >= 0)
                        {
                            int idx;
                            if (mInsertLocation == mInsertedValues.Length)
                                mInsertLocation = 0;
                            UInt32 oldval = mInsertedValues[mInsertLocation];
                            if (mLookupTable.TryGetValue(oldval, out idx) && idx == mWindowStart)
                                mLookupTable.Remove(oldval);
                        }
                        if (mPendingValues != null)
                        {
                            // Pop the top of the queue and put it in the table
                            if (mDataLength > mQueueLength + 4)
                            {
                                UInt32 poppedval = mPendingValues[mPendingOffset];
                                mInsertedValues[mInsertLocation] = poppedval;
                                mInsertLocation++;
                                if (mInsertLocation > mInsertedValues.Length)
                                    mInsertLocation = 0;

                                // Put it into the table
                                mLookupTable[poppedval] = mDataLength - mQueueLength - 4;
                            }
                            // Push the next value onto the queue
                            mPendingValues[mPendingOffset] = mRunningValue;
                            mPendingOffset++;
                            if (mPendingOffset == mPendingValues.Length)
                                mPendingOffset = 0;
                        }
                        else
                        {
                            // No queue, straight to the dictionary
                            mInsertedValues[mInsertLocation] = mRunningValue;
                            mInsertLocation++;
                            if (mInsertLocation > mInsertedValues.Length)
                                mInsertLocation = 0;

                            mLookupTable[mRunningValue] = mDataLength - 4;
                        }
                    }
                }
                else
                {
                    mRollingInterval++;
                    if (mRollingInterval == mInterval)
                        mRollingInterval = 0;
                    mInitialized = (mDataLength == 3);
                }

                mRunningValue = (mRunningValue << 8) | val;
                mDataLength++;
                mWindowStart++;
            }

            public bool FindMatch(out int where)
            {
                return mLookupTable.TryGetValue(mRunningValue, out where);
            }

            #endregion
        }

        private class DeepMatchTracker : IMatchtracker
        {
            public DeepMatchTracker(int blockinterval, int lookupstart, int windowlength, int bucketdepth)
            {
                mInterval = blockinterval;
                if (lookupstart > 0)
                {
                    mPendingValues = new UInt32[lookupstart / blockinterval];
                    mQueueLength = mPendingValues.Length * blockinterval;
                }
                else
                    mQueueLength = 0;
                mInsertedValues = new UInt32[windowlength / blockinterval - lookupstart / blockinterval];
                mWindowStart = -(mInsertedValues.Length + lookupstart / blockinterval) * blockinterval - 4;
                mBucketDepth = bucketdepth;
            }

            public void Reset()
            {
                mLookupTable.Clear();
                mRunningValue = 0;

                mRollingInterval = 0;
                mWindowStart = -(mInsertedValues.Length + (mPendingValues != null ? mPendingValues.Length : 0)) * mInterval - 4;
                mDataLength = 0;

                mInitialized = false;
                mInsertLocation = 0;
                mPendingOffset = 0;

                mCurrentMatch = null;

                // No need to clear the arrays, the values will be ignored by the next time around
            }

            private int mBucketDepth;

            // Current 32 bit value of the last 4 bytes
            private UInt32 mRunningValue;

            // How often to insert into the table
            private int mInterval;
            // Avoid division by using a rolling count instead
            private int mRollingInterval;

            // How many bytes to queue up before adding it to the lookup table
            private int mQueueLength;

            // Queued up values for future matches
            private UInt32[] mPendingValues;
            private int mPendingOffset;

            // Bytes processed so far
            private int mDataLength;
            private int mWindowStart;

            // Four or more bytes read
            private bool mInitialized;

            // Values values pending removal
            private UInt32[] mInsertedValues;
            private int mInsertLocation;

            // Hash of seen values
            private Dictionary<UInt32, List<int>> mLookupTable = new Dictionary<uint, List<int>>();

            // Save allocating items unnecessarily
            private Stack<List<int>> mUnusedLists = new Stack<List<int>>();

            private List<int> mCurrentMatch;
            private int mCurrentMatchIndex;

            #region IMatchtracker Members

            public void Addvalue(byte val)
            {
                if (mInitialized)
                {
                    mRollingInterval++;
                    // Time to add a value to the table
                    if (mRollingInterval == mInterval)
                    {
                        mRollingInterval = 0;
                        // Remove a value from the table if the window just rolled past it
                        if (mWindowStart > 0)
                        {
                            List<int> locations;
                            if (mInsertLocation == mInsertedValues.Length)
                                mInsertLocation = 0;
                            UInt32 oldval = mInsertedValues[mInsertLocation];
                            if (mLookupTable.TryGetValue(oldval, out locations) && locations[0] == mWindowStart)
                            {
                                locations.RemoveAt(0);
                                if (locations.Count == 0)
                                {
                                    mLookupTable.Remove(oldval);
                                    mUnusedLists.Push(locations);
                                }
                            }
                        }
                        if (mPendingValues != null)
                        {
                            // Pop the top of the queue and put it in the table
                            if (mDataLength > mQueueLength + 4)
                            {
                                UInt32 poppedval = mPendingValues[mPendingOffset];
                                mInsertedValues[mInsertLocation] = poppedval;
                                mInsertLocation++;
                                if (mInsertLocation > mInsertedValues.Length)
                                    mInsertLocation = 0;

                                // Put it into the table
                                List<int> locations;
                                if (mLookupTable.TryGetValue(poppedval, out locations))
                                {
                                    // Check if the bucket runneth over
                                    if (locations.Count == mBucketDepth)
                                        locations.RemoveAt(0);
                                }
                                else
                                {
                                    // Allocate a new bucket
                                    if (mUnusedLists.Count > 0)
                                        locations = mUnusedLists.Pop();
                                    else
                                        locations = new List<int>();
                                    mLookupTable[poppedval] = locations;
                                }
                                locations.Add(mDataLength - mQueueLength - 4);
                            }
                            // Push the next value onto the queue
                            mPendingValues[mPendingOffset] = mRunningValue;
                            mPendingOffset++;
                            if (mPendingOffset == mPendingValues.Length)
                                mPendingOffset = 0;
                        }
                        else
                        {
                            mInsertedValues[mInsertLocation] = mRunningValue;
                            mInsertLocation++;
                            if (mInsertLocation > mInsertedValues.Length)
                                mInsertLocation = 0;

                            // Put it into the table
                            List<int> locations;
                            if (mLookupTable.TryGetValue(mRunningValue, out locations))
                            {
                                // Check if the bucket runneth over
                                if (locations.Count == mBucketDepth)
                                    locations.RemoveAt(0);
                            }
                            else
                            {
                                // Allocate a new bucket
                                if (mUnusedLists.Count > 0)
                                    locations = mUnusedLists.Pop();
                                else
                                    locations = new List<int>();
                                mLookupTable[mRunningValue] = locations;
                            }
                            locations.Add(mDataLength - 4);
                        }
                    }
                }
                else
                {
                    mRollingInterval++;
                    if (mRollingInterval == mInterval)
                        mRollingInterval = 0;
                    mInitialized = (mDataLength == 3);
                }
                mRunningValue = (mRunningValue << 8) | val;
                mDataLength++;
                mWindowStart++;
            }

            public bool Nextmatch(out int where)
            {
                if (mCurrentMatch != null && mCurrentMatchIndex < mCurrentMatch.Count)
                {
                    where = mCurrentMatch[mCurrentMatchIndex];
                    mCurrentMatchIndex++;
                    return true;
                }
                where = -1;
                return false;
            }

            public bool FindMatch(out int where)
            {
                if (mLookupTable.TryGetValue(mRunningValue, out mCurrentMatch))
                {
                    mCurrentMatchIndex = 1;
                    where = mCurrentMatch[0];
                    return true;
                }
                mCurrentMatch = null;
                where = -1;
                return false;
            }

            #endregion
        }
    }
}