/***************************************************************************
 *  Copyright (C) 2013 by Peter L Jones                                    *
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
using s3pi.Interfaces;

namespace VP6VIDExpImp
{
    public class VideoResource : AResource
    {
        const int recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        #region Attributes
        _audio audio;
        _video video;
        #endregion

        public VideoResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, new EventArgs()); } stream.Position = 0; Parse(stream); }

        #region Sub-classes
        /// <summary>
        /// See http://wiki.multimedia.cx/index.php?title=Electronic_Arts_SCxl for audio.
        /// </summary>
        class _audio : AHandlerElement, IEquatable<_audio>
        {
            const int recommendedApiVersion = 1;
            static uint _hdrTag = (uint)FOURCC("SCHl");
            static uint _audioTag = (uint)FOURCC("GSTR");

            #region Attributes
            byte[] audioStream;
            #endregion

            #region Constructors
            public _audio(int APIversion, EventHandler handler) : base(APIversion, handler)
            {
                audioStream = new byte[]
                {
                    0x01, 0x00, 0x00, 0x00,
                    0x00, 0x04, 0x1D, 0xF8, 0x51, 0xE0,
                    0x06, 0x01, 0x65,
                    0xFD,
                        0x80, 0x01, 0x03,
                        0x85, 0x03, 0x00, 0x00, 0x00,
                        0x82, 0x01, 0x02,
                    0xFF,
                    0x00, 0x00,
                };
            }
            public _audio(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public _audio(int APIversion, EventHandler handler, _audio basis) : this(APIversion, handler, basis.audioStream) { }
            public _audio(int APIversion, EventHandler handler, byte[] audioStream) : base(APIversion, handler) { this.audioStream = (byte[])audioStream.Clone(); }
            #endregion

            #region Data I/O
            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);

                uint tag = r.ReadUInt32();
                if (tag != _hdrTag)
                    throw new InvalidDataException("Expected: '" + FOURCC(_hdrTag) + "', read: '" + FOURCC(tag) + "'; position: 0x" + s.Position.ToString("X8"));

                int length = r.ReadInt32();

                tag = r.ReadUInt32();
                if (tag != _audioTag)
                    throw new InvalidDataException("Expected: '" + FOURCC(_audioTag) + "', read: '" + FOURCC(tag) + "'; position: 0x" + s.Position.ToString("X8"));

                audioStream = r.ReadBytes(length - (int)s.Position);
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(_hdrTag);
                w.Write((int)(audioStream.Length + sizeof(uint) + sizeof(int) + sizeof(uint)));
                w.Write(_audioTag);
                w.Write(audioStream);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return new List<string>(); } }
            #endregion

            #region IEquatable<_audio>
            public bool Equals(_audio other) { return false; }

            public override bool Equals(object obj) { return false; }

            public override int GetHashCode() { return audioStream.GetHashCode(); }
            #endregion

            public string Value { get { return ""; } }
        }

        /// <summary>
        /// See http://wiki.multimedia.cx/index.php?title=Electronic_Arts_VP6 for video.
        /// See http://www.afterdawn.com/software/audio_video/codecs/vp6_vfw_codec.cfm for codec.
        /// </summary>
        class _video : AHandlerElement, IEquatable<_video>
        {
            const int recommendedApiVersion = 1;
            static uint _hdrTag = (uint)FOURCC("MVhd");
            static uint _videoTag = (uint)FOURCC("06PV");

            #region Attributes
            int unknown;
            uint videoTag;
            byte[] videoStream;
            #endregion

            #region Constructors
            public _video(int APIversion, EventHandler handler) : base(APIversion, handler)
            {
                unknown = 0x20;
                videoTag = _videoTag;
                videoStream = new byte[] {
                    0x00, 0x00, // width
                    0x00, 0x00, // height
                    0x00, 0x00, 0x00, 0x00, // frame count
                    0x00, 0x00, 0x00, 0x00, // largest chunk ("buffer size")
                    0x00, 0x00, 0x00, 0x01, // frame rate
                    0x00, 0x00, 0x00, 0x01, // frame rate
                };
            }
            public _video(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public _video(int APIversion, EventHandler handler, _video basis) : this(APIversion, handler, basis.unknown, basis.videoTag, basis.videoStream) { }
            public _video(int APIversion, EventHandler handler, int unknown, uint videoTag, byte[] videoStream) : base(APIversion, handler) { this.videoStream = (byte[])videoStream.Clone(); }
            #endregion

            #region Data I/O
            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);

                uint tag = r.ReadUInt32();
                if (tag != _hdrTag)
                    throw new InvalidDataException("Expected: '" + FOURCC(_hdrTag) + "', read: '" + FOURCC(tag) + "'; position: 0x" + s.Position.ToString("X8"));

                unknown = r.ReadInt32();
                videoTag = r.ReadUInt32();
                videoStream = r.ReadBytes((int)(s.Length - s.Position));
            }

            internal void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(_hdrTag);
                w.Write(unknown);
                w.Write(videoTag);
                w.Write(videoStream);
            }
            #endregion

            #region AHandlerElement Members
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            public override List<string> ContentFields { get { return new List<string>(); } }
            #endregion

            #region IEquatable<_video>
            public bool Equals(_video other) { return false; }

            public override bool Equals(object obj) { return false; }

            public override int GetHashCode() { return videoStream.GetHashCode(); }
            #endregion

            public string Value { get { return ""; } }
        }
        #endregion

        #region Data I/O
        private void Parse(System.IO.Stream stream)
        {
            audio = new _audio(requestedApiVersion, OnResourceChanged, stream);
            video = new _video(requestedApiVersion, OnResourceChanged, stream);
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();

            if (audio == null) audio = new _audio(requestedApiVersion, OnResourceChanged);
            audio.UnParse(ms);

            if (video == null) video = new _video(requestedApiVersion, OnResourceChanged);
            video.UnParse(ms);

            return ms;
       }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public BinaryReader GSTR_Audio
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                audio.UnParse(ms);
                return new BinaryReader(ms);
            }
            set
            {
                if (value.BaseStream.CanSeek) { value.BaseStream.Position = 0; audio = new _audio(0, OnResourceChanged, value.BaseStream); }
                else
                {
                    MemoryStream ms = new MemoryStream();
                    byte[] buffer = new byte[1024 * 1024];
                    for (int read = value.BaseStream.Read(buffer, 0, buffer.Length); read > 0; read = value.BaseStream.Read(buffer, 0, buffer.Length))
                        ms.Write(buffer, 0, read);
                    audio = new _audio(0, OnResourceChanged, ms); 
                }
                OnResourceChanged(this, new EventArgs());
            }
        }
        [ElementPriority(2)]
        public BinaryReader VP6_Video
        {
            get
            {
                MemoryStream ms = new MemoryStream();
                video.UnParse(ms);
                return new BinaryReader(ms);
            }
            set
            {
                if (value.BaseStream.CanSeek) { value.BaseStream.Position = 0; video = new _video(0, OnResourceChanged, value.BaseStream); }
                else
                {
                    MemoryStream ms = new MemoryStream();
                    byte[] buffer = new byte[1024 * 1024];
                    for (int read = value.BaseStream.Read(buffer, 0, buffer.Length); read > 0; read = value.BaseStream.Read(buffer, 0, buffer.Length))
                        ms.Write(buffer, 0, read);
                    video = new _video(0, OnResourceChanged, ms);
                }
                OnResourceChanged(this, new EventArgs());
            }
        }

        protected override List<string> ValueBuilderFields { get { return new List<string>(); } }

        public string Value { get { return ""; } }
        #endregion
    }
}
