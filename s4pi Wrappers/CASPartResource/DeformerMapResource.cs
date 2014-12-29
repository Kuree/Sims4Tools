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
        public RobeChannel robChannel { get; set; }

        private byte[] data;

        public string Value { get { return ValueBuilder; } }

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
            this.robChannel = (RobeChannel)r.ReadByte();
            this.data = r.ReadBytes(r.ReadInt32());
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
            if (this.data == null) this.data = new byte[0];
            w.Write(this.data.Length);
            w.Write(this.data);
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

            public ScanLine(int width, Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                UInt16 scanLineDataSize = r.ReadUInt16();
                this.IsCompressed = r.ReadBoolean();
                this.Width = width;
                this.RobeChannel = (RobeChannel)r.ReadByte();

                if (IsCompressed)
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
                    byte numIndex = r.ReadByte();
                    this.PixelPosIndexes = new UInt16[numIndex];
                    this.DataPosIndexes = new UInt16[numIndex];
                    for (int i = 0; i < numIndex; i++) this.PixelPosIndexes[i] = r.ReadUInt16();
                    for (int i = 0; i < numIndex; i++) this.DataPosIndexes[i] = r.ReadUInt16();
                    uint headerdatasize = 4U + 1U + (4U * numIndex);
                    this.RLEArrayOfPixels = new byte[scanLineDataSize - headerdatasize];
                    for (int i = 0; i < RLEArrayOfPixels.Length; i++) this.RLEArrayOfPixels[i] = r.ReadByte();
                }

            }
        }        
        #endregion

        #region Conversion
        public Stream ToBitMap()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            int height = (int)(maxRow - minRow + 1);
            int width = (int)(this.maxCol - this.minCol + 1);
            ScanLine[] scanLines = new ScanLine[height];
            for (int i = 0; i < scanLines.Length; i++) scanLines[i] = new ScanLine(width, ms);
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
