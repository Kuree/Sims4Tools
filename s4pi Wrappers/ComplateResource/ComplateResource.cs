/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using s4pi.Interfaces;

namespace ComplateResource
{
    /// <summary>
    /// A resource wrapper that understands Complate Resources
    /// </summary>
    public class ComplateResource : AResource
    {
        static bool checking = s4pi.Settings.Settings.Checking;
        const Int32 recommendedApiVersion = 1;

        #region Attributes
        uint unknown1 = 0x00000002;
        byte[] data = new byte[0];
        uint unknown2 = 0x00000000;
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of the resource
        /// </summary>
        /// <param name="APIversion">Requested API version</param>
        /// <param name="s">Data stream to use, or null to create from scratch</param>
        public ComplateResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); dirty = true; } stream.Position = 0; Parse(stream); }
        #endregion

        #region AApiVersionedFields
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            unknown1 = br.ReadUInt32();
            data = br.ReadBytes(2 * br.ReadInt32());
            unknown2 = br.ReadUInt32();
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(unknown1);
            bw.Write(data.Length / 2);
            bw.Write(data);
            bw.Write(unknown2);
            bw.Flush();
            return ms;
        }

        StreamReader streamReader(Stream baseStream) { return new StreamReader(baseStream, System.Text.Encoding.Unicode); }
        StreamWriter streamWriter(Stream baseStream) { return new StreamWriter(baseStream, System.Text.Encoding.Unicode); }
        #endregion

        #region Content Fields
        public string Value { get { return ValueBuilder; } }

        public uint Unknown1 { get { return unknown1; } set { if (unknown1 != value) { unknown1 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        public string UnicodeString
        {
            get { return UnicodeData.ReadToEnd(); }
            set
            {
                if (UnicodeString == value) return;
                MemoryStream ms = new MemoryStream();
                StreamWriter sw = streamWriter(ms);
                sw.Write(value);
                sw.Flush();
                if (ms.Length >= 2)
                {
                    ms.Position = 2;
                    data = (new BinaryReader(ms)).ReadBytes((int)(ms.Length - 2));
                }
                else data = new byte[0];
                OnResourceChanged(this, EventArgs.Empty);
            }
        }
        public TextReader UnicodeData
        {
            get { return streamReader(new MemoryStream(data)); }
            set { UnicodeString = value.ReadToEnd(); }
        }
        public uint Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        #endregion
    }

    public class ComplateResourceHandler : AResourceHandler
    {
        /// <summary>
        /// Create the content of the Dictionary.
        /// List of resource types is read once from a configuration file in the same folder as this assembly.
        /// </summary>
        public ComplateResourceHandler()
        {
            this.Add(typeof(ComplateResource), new List<string>(new string[] { "0x044AE110", }));
        }
    }
}
