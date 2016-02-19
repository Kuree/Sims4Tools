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
using System.Linq;
using s4pi.Interfaces;

namespace NGMPHashMapResource
{
    /// <summary>
    /// A resource wrapper that understands 0xF3A38370 resources
    /// </summary>
    public class NGMPHashMapResource : AResource
    {
        static bool checking = s4pi.Settings.Settings.Checking;
        const Int32 recommendedApiVersion = 1;
        uint version = 1;

        NGMPPairList data;

        #region AApiVersionedFields
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        /// <summary>
        /// Create a new instance of the resource
        /// </summary>
        /// <param name="APIversion">Requested API version</param>
        /// <param name="s">Data stream to use, or null to create from scratch</param>
        public NGMPHashMapResource(int APIversion, Stream s)
            : base(APIversion, s)
        {
            if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); }
            stream.Position = 0;
            Parse(stream);
        }

        void Parse(Stream s)
        {
            BinaryReader br = new BinaryReader(s);

            version = br.ReadUInt32();
            if (checking) if (version != 1)
                    throw new InvalidDataException(String.Format("{0}: unsupported 'version'.  Read '0x{1:X8}', supported: '0x00000001'", this.GetType().Name, version));

            data = new NGMPPairList(OnResourceChanged, s);

            if (checking) if (s.Position != s.Length)
                    throw new InvalidDataException(String.Format("{0}: Length {1} bytes, parsed {2} bytes", this.GetType().Name, s.Length, s.Position));
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);
            w.Write(version);
            if (data == null) data = new NGMPPairList(OnResourceChanged);
            data.UnParse(ms);
            w.Flush();
            return ms;
        }

        public class NGMPPair : AHandlerElement, IEquatable<NGMPPair>
        {
            const Int32 recommendedApiVersion = 1;

            ulong nameHash;
            ulong instance;

            public NGMPPair(int apiVersion, EventHandler handler) : base(apiVersion, handler) { }
            public NGMPPair(int apiVersion, EventHandler handler, NGMPPair basis) : this(apiVersion, handler, basis.nameHash, basis.instance) { }
            public NGMPPair(int apiVersion, EventHandler handler, ulong nameHash, ulong instance) : base(apiVersion, handler) { this.nameHash = nameHash; this.instance = instance; }
            public NGMPPair(int apiVersion, EventHandler handler, Stream s) : base(apiVersion, handler) { Parse(s); }

            void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                nameHash = r.ReadUInt64();
                instance = r.ReadUInt64();
            }
            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(nameHash);
                w.Write(instance);
            }

            #region AHandlerElement
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(0, this.GetType()); } }
            #endregion

            #region IEquatable<NGMPPair>
            public bool Equals(NGMPPair other) { return nameHash.Equals(other.nameHash) && instance.Equals(other.instance); }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public ulong NameHash { get { return nameHash; } set { if (nameHash != value) { nameHash = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public ulong Instance { get { return instance; } set { if (instance != value) { instance = value; OnElementChanged(); } } }

            public string Value { get { return ValueBuilder.Replace("\n", "; "); } }
            #endregion
        }

        public class NGMPPairList : DependentList<NGMPPair>
        {
            public NGMPPairList(EventHandler handler) : base(handler) { }
            public NGMPPairList(EventHandler handler, Stream s) : base(handler, s) { }
            public NGMPPairList(EventHandler handler, IEnumerable<NGMPPair> ln) : base(handler, ln) { }

            #region DependentList<NGMPPair>
            protected override NGMPPair CreateElement(Stream s) { return new NGMPPair(0, elementHandler, s); }
            protected override void WriteElement(Stream s, NGMPPair element) { element.UnParse(s); }
            #endregion
        }

        [MinimumVersion(1)]
        [MaximumVersion(recommendedApiVersion)]
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public NGMPPairList Data { get { return data; } set { if (!data.Equals(value)) { data = value == null ? null : new NGMPPairList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); } } }

        public String Value { get { return ValueBuilder; } }
    }

    /// <summary>
    /// ResourceHandler for NameMapResource wrapper
    /// </summary>
    public class NGMPHashMapResourceHandler : AResourceHandler
    {
        /// <summary>
        /// Create the content of the Dictionary
        /// </summary>
        public NGMPHashMapResourceHandler()
        {
            this.Add(typeof(NGMPHashMapResource), new List<string>(new string[] { "0xF3A38370" }));
        }
    }
}
