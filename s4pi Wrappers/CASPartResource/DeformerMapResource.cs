/***************************************************************************
 *  Copyright (C) 2014 by Keyi Zhang                                       *
 *  kz005@bucknell.edu                                                     *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s4pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CASPartResource
{
    public class DeformerMapResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        
        static bool checking = s4pi.Settings.Settings.Checking;

        public uint version { get; set; }
        public uint doubledWidth { get; set; }
        public uint height { get; set; }
        public AgeGenderFlags ageGender { get; set; }
        public Physiques physique { get; set; }
        public ShapeOrNormals shapeOrNormals { get; set; }

        public uint minCol { get; private set; }
        public uint maxCol { get; private set; }
        public uint minRow { get; private set; }
        public uint maxRow { get; private set; }
        public RobeChannel robeChannel { get; set; }

        private byte[] scanLineData;

        public string WrapperValue 
        { 
            get {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Empty Deformer Map Resource.\nVersion: {0:X8}", this.version);
                sb.AppendFormat("AgeGender: {0}\nPhysiques: {1}\nShapeOrNormals: {2}\nRobeChannel: {3}", this.ageGender, this.physique, this.shapeOrNormals, this.robeChannel);
                return sb.ToString();
            } 
        }

        public DeformerMapResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }
        
        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);
            s.Position = 0;
            this.version = r.ReadUInt32();
            this.doubledWidth = r.ReadUInt32();
            this.height = r.ReadUInt32();
            this.ageGender = (AgeGenderFlags)r.ReadUInt32();
            this.physique = (Physiques)r.ReadByte();
            this.shapeOrNormals = (ShapeOrNormals)r.ReadByte();
            this.minCol = r.ReadUInt32();
            this.maxCol = r.ReadUInt32();
            this.minRow = r.ReadUInt32();
            this.maxRow = r.ReadUInt32();
            this.robeChannel = (RobeChannel)r.ReadByte();
            this.scanLineData = r.ReadBytes(r.ReadInt32());
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(this.version);
            w.Write(this.doubledWidth);
            w.Write(this.height);
            w.Write((uint)this.ageGender);
            w.Write((byte)this.physique);
            w.Write((byte)this.shapeOrNormals);
            w.Write(this.minCol);
            w.Write(this.maxCol);
            w.Write(this.minRow);
            w.Write(this.maxRow);
            w.Write((byte)this.robeChannel);
            if (this.scanLineData == null) this.scanLineData = new byte[0];
            w.Write(this.scanLineData.Length);
            w.Write(this.scanLineData);
            return ms;
        }
        #endregion

        #region Sub-Class
        public enum Physiques : byte
        {
            BODYBLENDTYPE_HEAVY = 0,
            BODYBLENDTYPE_FIT = 1,
            BODYBLENDTYPE_LEAN = 2,
            BODYBLENDTYPE_BONY = 3,
            BODYBLENDTYPE_PREGNANT = 4,
            BODYBLENDTYPE_HIPS_WIDE = 5,
            BODYBLENDTYPE_HIPS_NARROW = 6,
            BODYBLENDTYPE_WAIST_WIDE = 7,
            BODYBLENDTYPE_WAIST_NARROW = 8,
            BODYBLENDTYPE_IGNORE = 9,   // Assigned to deformation maps associated with sculpts or modifiers, instead of a physique.
            BODYBLENDTYPE_AVERAGE = 100, // Special case used to indicate an "average" deformation map always applied for a given age
        }

        public enum ShapeOrNormals : byte
        {
            SHAPE_DEFORMER = 0,     // This resource contains positional deltas
            NORMALS_DEFORMER = 1    // This resource contains normal deltas
        }

        /// <summary>
        /// Is the robe channel interleaved with the skin tight data.
        /// </summary>
        public enum RobeChannel : byte
        {
            ROBECHANNEL_PRESENT = 0,
            ROBECHANNEL_DROPPED = 1,
            ROBECHANNEL_ISCOPY = 2,     // Robe data not present but is the same as skin tight data.
        }

        private class ScanLine
        {
            public bool IsCompressed { get; set; }
            public byte[] UncompressedPixels { get; private set; }
            public int Width { get; private set; }
            public RobeChannel RobeChannel { get; private set; }
            public UInt16[] PixelPosIndexes { get; private set; }
            public UInt16[] DataPosIndexes { get; private set; }
            public byte[] RLEArrayOfPixels { get; private set; }
            public byte NumIndexes { get; private set; }

            public ScanLine(int width, Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                UInt16 scanLineDataSize = r.ReadUInt16();
                this.IsCompressed = r.ReadBoolean();
                this.Width = width;
                this.RobeChannel = (RobeChannel)r.ReadByte();

                if (!IsCompressed)
                {
                    if (RobeChannel == RobeChannel.ROBECHANNEL_PRESENT)
                    {
                        this.UncompressedPixels = r.ReadBytes(width * 6);
                    }
                    else
                    {
                        this.UncompressedPixels = r.ReadBytes(width * 3);
                    }
                }
                else
                {
                    NumIndexes = r.ReadByte();
                    this.PixelPosIndexes = new UInt16[NumIndexes];
                    this.DataPosIndexes = new UInt16[NumIndexes];
                    for (int i = 0; i < NumIndexes; i++) this.PixelPosIndexes[i] = r.ReadUInt16();
                    for (int i = 0; i < NumIndexes; i++) this.DataPosIndexes[i] = r.ReadUInt16();
                    uint headerdatasize = 4U + 1U + (4U * NumIndexes);
                    this.RLEArrayOfPixels = new byte[scanLineDataSize - headerdatasize];
                    for (int i = 0; i < RLEArrayOfPixels.Length; i++) this.RLEArrayOfPixels[i] = r.ReadByte();
                }

            }
        }        
        #endregion

        #region Conversion
        public enum OutputType
        {
            Skin,
            Robe
        }
        public Stream GetSkinBitMap() { return this.ToBitMap(OutputType.Skin); }

        public Stream ToBitMap(OutputType type)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            if (maxCol == 0) return null;
            int height = (int)(maxRow - minRow + 1);
            int width = (int)(this.maxCol - this.minCol + 1);
            ScanLine[] scanLines = new ScanLine[height];
            using (MemoryStream scanLineStream = new MemoryStream(this.scanLineData))
            {
                for (int i = 0; i < scanLines.Length; i++) scanLines[i] = new ScanLine(width, scanLineStream);
            }

            byte[] pixelArraySkinTight = new byte[width * height * 3];
            byte[] pixelArrayRobe = new byte[width * height * 3];

            int destIndexRobe = 0;
            int destSkinTight = 0;

            int pixelsize = 0;

            for (int i = 0; i < height; i++)
            {
                if (scanLines[i].RobeChannel == RobeChannel.ROBECHANNEL_PRESENT)
                {
                    pixelsize = 6;
                }
                else
                {
                    pixelsize = 3;
                }

                var scan = scanLines[i];
                if (!scan.IsCompressed)
                {
                    for (int j = 0; j < width; j++)
                    {
                        pixelArraySkinTight[destSkinTight++] = scan.UncompressedPixels[(j * pixelsize) + 0];
                        pixelArraySkinTight[destSkinTight++] = scan.UncompressedPixels[(j * pixelsize) + 1];
                        pixelArraySkinTight[destSkinTight++] = scan.UncompressedPixels[(j * pixelsize) + 2];

                        switch (scan.RobeChannel)
                        {
                            case RobeChannel.ROBECHANNEL_PRESENT:
                                pixelArrayRobe[destIndexRobe++] = scan.UncompressedPixels[(j * pixelsize) + 3];
                                pixelArrayRobe[destIndexRobe++] = scan.UncompressedPixels[(j * pixelsize) + 4];
                                pixelArrayRobe[destIndexRobe++] = scan.UncompressedPixels[(j * pixelsize) + 5];
                                break;
                            case RobeChannel.ROBECHANNEL_DROPPED:
                                pixelArrayRobe[destIndexRobe++] = 0;
                                pixelArrayRobe[destIndexRobe++] = 0;
                                pixelArrayRobe[destIndexRobe++] = 0;
                                break;
                            case RobeChannel.ROBECHANNEL_ISCOPY:
                                pixelArrayRobe[destIndexRobe++] = scan.UncompressedPixels[(j * pixelsize) + 0];
                                pixelArrayRobe[destIndexRobe++] = scan.UncompressedPixels[(j * pixelsize) + 1];
                                pixelArrayRobe[destIndexRobe++] = scan.UncompressedPixels[(j * pixelsize) + 2];
                                break;
                        }
                    }
                }
                else
                {

                    // Look up each pixel using index tables
                    for (int j = 0; j < width; j++)
                    {
                        // To get pointer to the RLE encoded data we need first find 
                        // proper RLE run in the buffer. Use index for this:

                        // Cache increment for indexing in pixel space?
                        int step = 1 + width / (scan.NumIndexes - 1); // 1 entry was added for the remainder of the division

                        // Find index into the positions and data table:
                        int idx = j / step;

                        // This is location of the run first covering this interval.
                        int pixelPosX = scan.PixelPosIndexes[idx];

                        // Position of the RLE data of the place where need to unwind to the pixel. 
                        int dataPos = scan.DataPosIndexes[idx] * (pixelsize + 1); // +1 for run length byte

                        // This is run length for the RLE entry found at 
                        int runLength = scan.RLEArrayOfPixels[dataPos];

                        // Loop forward unwinding RLE data from the found indexed position. 
                        // Continue until the pixel position in question is not covered 
                        // by the current run interval. By design the loop should execute 
                        // only few times until we find the value we are looking for.
                        while (j >= pixelPosX + runLength)
                        {
                            pixelPosX += runLength;
                            dataPos += (1 + pixelsize); // 1 for run length, +pixelSize for the run value

                            runLength = scan.RLEArrayOfPixels[dataPos];
                        }

                        // After breaking out of the cycle, we have the current run length interval
                        // covering the pixel position x we are interested in. So just return the pointer
                        // to the pixel data we were after:
                        int pixelStart = dataPos + 1;

                        //
                        pixelArraySkinTight[destSkinTight++] = scan.RLEArrayOfPixels[pixelStart + 0];
                        pixelArraySkinTight[destSkinTight++] = scan.RLEArrayOfPixels[pixelStart + 1];
                        pixelArraySkinTight[destSkinTight++] = scan.RLEArrayOfPixels[pixelStart + 2];
                        switch (scan.RobeChannel)
                        {
                            case RobeChannel.ROBECHANNEL_PRESENT:
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 3];
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 4];
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 5];
                                break;
                            case RobeChannel.ROBECHANNEL_DROPPED:
                                pixelArrayRobe[destIndexRobe++] = 0;
                                pixelArrayRobe[destIndexRobe++] = 0;
                                pixelArrayRobe[destIndexRobe++] = 0;
                                break;
                            case RobeChannel.ROBECHANNEL_ISCOPY:
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 0];
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 1];
                                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[pixelStart + 2];
                                break;
                        }
                    }

                    //// Unpack the RLE Scan line without using index tables
                    //numpixelsdecoded = 0;
                    //rleindex = 0;

                    //while (numpixelsdecoded < width)
                    //{
                    //    runlen = scan.RLEArrayOfPixels[rleindex++];
                    //    for (int j = 0; j < runlen; j++)
                    //    {
                    //        pixelArraySkinTight[destSkinTight++] = scan.RLEArrayOfPixels[rleindex + 0];
                    //        pixelArraySkinTight[destSkinTight++] = scan.RLEArrayOfPixels[rleindex + 1];
                    //        pixelArraySkinTight[destSkinTight++] = scan.RLEArrayOfPixels[rleindex + 2];
                    //        switch (scan.RobeChannel)
                    //        {
                    //            case RobeChannel.ROBECHANNEL_PRESENT:
                    //                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[rleindex + 0];
                    //                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[rleindex + 0];
                    //                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[rleindex + 0];
                    //                break;
                    //            case RobeChannel.ROBECHANNEL_DROPPED:
                    //                pixelArrayRobe[destIndexRobe++] = 0;
                    //                pixelArrayRobe[destIndexRobe++] = 0;
                    //                pixelArrayRobe[destIndexRobe++] = 0;
                    //                break;
                    //            case RobeChannel.ROBECHANNEL_ISCOPY:
                    //                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[rleindex + 0];
                    //                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[rleindex + 0];
                    //                pixelArrayRobe[destIndexRobe++] = scan.RLEArrayOfPixels[rleindex + 0];
                    //                break;
                    //        }
                    //        numpixelsdecoded++;
                    //    }
                    //    rleindex += pixelsize;
                    //}
                }
            }

            w.Write((ushort)0x4d42);
            w.Write(0);
            w.Write(0);
            w.Write(54);
            w.Write(40);
            w.Write(width);
            w.Write(height);
            w.Write((ushort)1);
            w.Write((ushort)24);
            for (int i = 0; i < 6; i++) w.Write(0);

            int bytesPerLine = (int)Math.Ceiling(width * 24.0 / 8.0);
            int padding = 4 - bytesPerLine % 4;
            long sourcePosition = 0;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width * 3; j++)
                {
                    w.Write(type == OutputType.Robe? pixelArrayRobe[sourcePosition++] : pixelArraySkinTight[sourcePosition++]);
                }

                for (int j = 0; j < padding; j++)
                {
                    w.Write((byte)0);
                }
            }

            return ms;
        }
        #endregion
    }

    public class DeformerMapResourceHandler : AResourceHandler
    {
        public DeformerMapResourceHandler()
        {
            if (s4pi.Settings.Settings.IsTS4)
                this.Add(typeof(DeformerMapResource), new List<string>(new string[] { "0xDB43E069", }));
        }
    }
}
