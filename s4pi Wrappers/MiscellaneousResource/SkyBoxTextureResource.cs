/***************************************************************************
 *  Copyright (C) 2016 by Sims 4 Tools Development Team                    *
 *  Credits: Peter Jones, Cmar                                             *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
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
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

using s4pi.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace s4pi.Miscellaneous
{
    public class SkyBoxTextureResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        uint version;
        SkyBoxTextureList textures;
        #endregion

        public SkyBoxTextureResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }

        #region Data I/O
        void Parse(Stream s)
        {
            s.Position = 0;
            BinaryReader r = new BinaryReader(s);

            this.version = r.ReadUInt32();
            this.textures = new SkyBoxTextureList(this.OnResourceChanged, s);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms);

            w.Write(version);
            this.textures.UnParse(ms);

            return ms;
        }
        #endregion

        #region Subclasses
        public class SkyBoxTextureList : DependentList<SkyBoxTexture>
        {
            public SkyBoxTextureList(EventHandler handler, long maxSize = -1)
                : base(handler, maxSize)
            {
            }

            public SkyBoxTextureList(EventHandler handler, IEnumerable<SkyBoxTexture> sky, long maxSize = -1)
                : base(handler, sky, maxSize)
            {
            }

            public SkyBoxTextureList(EventHandler handler, Stream s, long maxSize = -1)
                : base(handler, s, maxSize)
            {
            }

            protected override SkyBoxTexture CreateElement(Stream s)
            {
                return new SkyBoxTexture(recommendedApiVersion, this.elementHandler, s);
            }

            protected override void WriteElement(Stream s, SkyBoxTexture element)
            {
                element.UnParse(s);
            }

        }

        public class SkyBoxTexture : AHandlerElement, IEquatable<SkyBoxTexture>
        {
            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
            #endregion

            SkyBoxTextureType type;
            TGIBlock tgiKey;
            string texturePath;

            public SkyBoxTexture(int apiVersion, EventHandler handler, SkyBoxTexture other)
                : this(apiVersion, handler, other.type, other.tgiKey, other.texturePath)
            {
            }
            public SkyBoxTexture(int apiVersion, EventHandler handler)
                : base(apiVersion, handler)
            {
                this.type = SkyBoxTextureType.SkyBox_Texture_None;
                this.tgiKey = new TGIBlock(1, handler);
                this.texturePath = "";
            }
            public SkyBoxTexture(int apiVersion, EventHandler handler, Stream s)
                : this(apiVersion, handler)
            {
                this.Parse(s);
            }
            public SkyBoxTexture(int apiVersion, EventHandler handler, SkyBoxTextureType type, TGIBlock tgiKey, string texturePath)
                : base(apiVersion, handler)
            {
                this.type = type;
                this.tgiKey = tgiKey;
                this.texturePath = texturePath;
            }

            void Parse(Stream s)
            {
                var br = new BinaryReader(s);
                this.type = (SkyBoxTextureType)br.ReadInt32();
                this.tgiKey = new TGIBlock(recommendedApiVersion, handler, s);
                int strCount = br.ReadInt32();
                this.texturePath = System.Text.Encoding.Unicode.GetString(br.ReadBytes(strCount * 2));
            }

            public void UnParse(Stream s)
            {
                var bw = new BinaryWriter(s);
                bw.Write((int)this.type);
                this.tgiKey.UnParse(s);
                byte[] str = System.Text.Encoding.Unicode.GetBytes(this.texturePath);
                bw.Write(str.Length / 2);
                bw.Write(str);
            }

            [ElementPriority(0)]
            public SkyBoxTextureType Type { get { return this.type; } set { if (this.type != value) { OnElementChanged(); this.type = value; } } }
            [ElementPriority(1)]
            public TGIBlock TGIKey { get { return this.tgiKey; } set { if (this.tgiKey != value) { OnElementChanged(); this.tgiKey = value; } } }
            [ElementPriority(2)]
            public string TexturePath { get { return this.texturePath; } set { if (String.Compare(this.texturePath, value) != 0) { OnElementChanged(); this.texturePath = value; } } }

            public string Value { get { return ValueBuilder; } }

            public bool Equals(SkyBoxTexture other)
            {
                return this.type == other.type &&
                       this.tgiKey == other.tgiKey &&
                       Enumerable.SequenceEqual(this.texturePath, other.texturePath);
            }

            public enum SkyBoxTextureType : int
            {
                SkyBox_Texture_Clouds = 0,
                SkyBox_Texture_Sun = 1,
                SkyBox_Texture_SunHalo = 2,
                SkyBox_Texture_Moon = 3,
                SkyBox_Texture_Stars = 4,
                SkyBox_Texture_CubeMap = 5,
                SkyBox_Texture_NumTextures,
                SkyBox_Texture_None = -1,
            }
        }
        #endregion


        #region Content Fields
        [ElementPriority(1)]
        public uint Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public SkyBoxTextureList Textures
        {
            get { return textures; }
            set { if (this.textures != value) { this.textures = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        public string Value { get { return ValueBuilder; } }

        #endregion
    }

    /// <summary>
    /// ResourceHandler for SkyBoxTextureResource wrapper
    /// </summary>
    public class SkyBoxTextureResourceHandler : AResourceHandler
    {
        public SkyBoxTextureResourceHandler()
        {
            this.Add(typeof(SkyBoxTextureResource), new List<string>(new string[] { "0x71A449C9", }));
        }
    }
}
