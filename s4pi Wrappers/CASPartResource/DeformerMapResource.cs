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
using System.Drawing;
using System.Drawing.Imaging;

namespace CASPartResource
{
    public class DeformerMapResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        
        static bool checking = s4pi.Settings.Settings.Checking;

        private uint version;
        private uint doubledWidth;
        private uint height;
        private AgeGenderFlags ageGender;
        private Physiques physique;
        private ShapeOrNormals shapeOrNormals;

        private uint minCol;
        private uint maxCol;
        private uint minRow;
        private uint maxRow;
        private RobeChannel robeChannel;

        private ScanLine[] scanLines;

      //  public Bitmap SkinImage { get
      //      {
      //          if (this.maxCol > 0) return (Bitmap)Bitmap.FromStream(this.ToBitMap(OutputType.Skin));
      //          else return new Bitmap(1, 1); ;
      //      }
      //  }

      //  public string Value { get { return ValueBuilder; } }

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
            int totalBytes = r.ReadInt32();
            if (totalBytes == 0)
            {
                this.scanLines = new ScanLine[0];
            }
            else
            {
                int width = (int)(maxCol - minCol + 1);
                uint numScanLines = maxRow - minRow + 1;
                this.scanLines = new ScanLine[numScanLines];
                for (int i = 0; i < numScanLines; i++)
                {
                    scanLines[i] = new ScanLine(recommendedApiVersion, OnResourceChanged, s, width);
                }
            }
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
            if (this.scanLines == null) this.scanLines = new ScanLine[0];
            w.Write(this.scanLines.Length);
            for (int i = 0; i < this.scanLines.Length; i++)
            {
                this.scanLines[i].UnParse(ms);
            }
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

        public class ScanLine : AHandlerElement, IEquatable<ScanLine>
        {
            private UInt16 scanLineDataSize;
            private bool isCompressed;
            private byte[] uncompressedPixels;
            private int width;
            private RobeChannel robeChannel;
            private UInt16[] pixelPosIndexes;
            private UInt16[] dataPosIndexes;
            private byte[] rleArrayOfPixels;
            private byte numIndexes;

            public ScanLine(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public ScanLine(int APIversion, EventHandler handler, Stream s, int width) : base(APIversion, handler) { this.width = width; Parse(s); }

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            #endregion

            #region Content Fields
            [ElementPriority(0)]
            public UInt16  ScanLineDataSize { get { return this.scanLineDataSize; } }
            [ElementPriority(1)]
            public bool IsCompressed { get { return this.isCompressed; } }
            [ElementPriority(2)]
            public int Width { get { return this.width; } }
            [ElementPriority(3)]
            public RobeChannel RobeChannel { get { return this.robeChannel; } }
            [ElementPriority(4)]
            public byte[] UncompressedPixels { get { return this.uncompressedPixels; } }
            [ElementPriority(5)]
            public byte NumIndexes { get { return this.numIndexes; } }
            [ElementPriority(6)]
            public UInt16[] PixelPosIndexes { get { return this.pixelPosIndexes; } }
            [ElementPriority(7)]
            public UInt16[] DataPosIndexes { get { return this.dataPosIndexes; } }
            [ElementPriority(8)]
            public byte[] RLEArrayOfPixels { get { return this.rleArrayOfPixels; } }
            #endregion

            public string Value { get { return this.ValueBuilder; } }
            public override List<string> ContentFields
            {
                get
                {
                    var res = GetContentFields(requestedApiVersion, this.GetType());
                    if (this.isCompressed)
                    {
                        res.Remove("UncompressedPixels");
                    }
                    else
                    {
                        res.Remove("NumIndexes");
                        res.Remove("PixelPosIndexes");
                        res.Remove("DataPosIndexes");
                        res.Remove("RLEArrayOfPixels");
                    }
                    res.Remove("Width");
                    return res;
                }
            }

            public void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.scanLineDataSize = r.ReadUInt16();
                this.isCompressed = r.ReadBoolean();
                this.robeChannel = (RobeChannel)r.ReadByte();

                if (!isCompressed)
                {
                    if (robeChannel == RobeChannel.ROBECHANNEL_PRESENT)
                    {
                        this.uncompressedPixels = r.ReadBytes(this.width * 6);
                    }
                    else
                    {
                        this.uncompressedPixels = r.ReadBytes(this.width * 3);
                    }
                }
                else
                {
                    this.numIndexes = r.ReadByte();
                    this.pixelPosIndexes = new UInt16[numIndexes];
                    this.dataPosIndexes = new UInt16[numIndexes];
                    for (int i = 0; i < numIndexes; i++) this.pixelPosIndexes[i] = r.ReadUInt16();
                    for (int i = 0; i < numIndexes; i++) this.dataPosIndexes[i] = r.ReadUInt16();
                    uint headerdatasize = 4U + 1U + (4U * numIndexes);
                    this.rleArrayOfPixels = new byte[scanLineDataSize - headerdatasize];
                    for (int i = 0; i < rleArrayOfPixels.Length; i++) this.rleArrayOfPixels[i] = r.ReadByte();
                }
            }

            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(this.scanLineDataSize);
                w.Write(this.isCompressed);
                w.Write((byte)this.robeChannel);

                if (!isCompressed)
                {
                    w.Write(this.uncompressedPixels);
                }
                else
                {
                    w.Write(this.numIndexes);
                    for (int i = 0; i < numIndexes; i++) w.Write(this.pixelPosIndexes[i]);
                    for (int i = 0; i < numIndexes; i++) w.Write(this.dataPosIndexes[i]);
                    w.Write(this.rleArrayOfPixels);
                }
            }

            public bool Equals(ScanLine other)
            {
                return this.scanLineDataSize == other.scanLineDataSize && this.isCompressed == other.isCompressed && this.robeChannel == other.robeChannel;
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
            uint height = this.maxRow - this.minRow + 1;
            uint width = this.maxCol - this.minCol + 1;

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

                ScanLine scan = scanLines[i];
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
                        uint step = 1U + width / (scan.NumIndexes - 1U); // 1 entry was added for the remainder of the division

                        // Find index into the positions and data table:
                        uint idx = (uint)(j / step);

                        // This is location of the run first covering this interval.
                        uint pixelPosX = scan.PixelPosIndexes[idx];

                        // Position of the RLE data of the place where need to unwind to the pixel. 
                        uint dataPos = scan.DataPosIndexes[idx] * (uint)(pixelsize + 1); // +1 for run length byte

                        // This is run length for the RLE entry found at 
                        uint runLength = scan.RLEArrayOfPixels[dataPos];

                        // Loop forward unwinding RLE data from the found indexed position. 
                        // Continue until the pixel position in question is not covered 
                        // by the current run interval. By design the loop should execute 
                        // only few times until we find the value we are looking for.
                        while (j >= pixelPosX + runLength)
                        {
                            pixelPosX += runLength;
                            dataPos += (uint)(1 + pixelsize); // 1 for run length, +pixelSize for the run value

                            runLength = scan.RLEArrayOfPixels[dataPos];
                        }

                        // After breaking out of the cycle, we have the current run length interval
                        // covering the pixel position x we are interested in. So just return the pointer
                        // to the pixel data we were after:
                        uint pixelStart = dataPos + 1;

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
            if (padding == 4) padding = 0;
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

        #region Content Fields
        [ElementPriority(0)]
        public uint Version { get { return this.version; } set { if (!this.version.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.version = value; } } }
        [ElementPriority(1)]
        public uint DoubledWidth { get { return this.doubledWidth; } set { } }
        [ElementPriority(2)]
        public uint Height { get { return this.height; } set { } }
        [ElementPriority(3)]
        public AgeGenderFlags AgeGender { get { return this.ageGender; } set { if (!this.ageGender.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ageGender = value; } } }
        [ElementPriority(4)]
        public Physiques Physique { get { return this.physique; } set { if (!this.physique.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.physique = value; } } }
        [ElementPriority(5)]
        public ShapeOrNormals IsShapeOrNormals { get { return this.shapeOrNormals; } set { if (!this.shapeOrNormals.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.shapeOrNormals = value; } } }
        [ElementPriority(6)]
        public uint MinCol { get { return minCol; } set { if (value != minCol) minCol = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(7)]
        public uint MaxCol { get { return maxCol; } set { if (value != maxCol) maxCol = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(8)]
        public uint MinRow { get { return minRow; } set { if (value != minRow) minRow = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(9)]
        public uint MaxRow { get { return maxRow; } set { if (value != maxRow) maxRow = value; OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(10)]
        public RobeChannel HasRobeChannel { get { return this.robeChannel; } set { if (!this.robeChannel.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.robeChannel = value; } } }
        [ElementPriority(11)]
        public ScanLine[] ScanLines { get { return this.scanLines; } set { if (!this.scanLines.Equals(value)) { OnResourceChanged(this, EventArgs.Empty); this.ScanLines = value; } } }
        public string Value { get { return ValueBuilder; } }
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
