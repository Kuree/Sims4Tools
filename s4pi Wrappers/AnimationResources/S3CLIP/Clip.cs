/***************************************************************************
 *  Copyright (C) 2009, 2015 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Peter L Jones (pljones@users.sf.net)                                   *
 *  Keyi Zhang                                                             *
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

namespace s4pi.Animation.S3CLIP
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using s4pi.Interfaces;

    public class Clip : AHandlerElement
    {
        #region Fields

        private string mAnimName;
        private float mFrameDuration;
        private ushort mMaxFrameCount;
        private string mSrcName;
        private TrackList mTracks;
        private uint mUnknown01;
        private ushort mUnknown02;
        private uint mVersion;

        #endregion

        #region ContentFields

        [ElementPriority(1)]
        public uint Version
        {
            get { return this.mVersion; }
            set
            {
                this.mVersion = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(2)]
        public uint Unknown01
        {
            get { return this.mUnknown01; }
            set
            {
                this.mUnknown01 = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(3)]
        public float FrameDuration
        {
            get { return this.mFrameDuration; }
            set
            {
                this.mFrameDuration = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(4)]
        public ushort MaxFrameCount
        {
            get { return this.mMaxFrameCount; }
            set
            {
                this.mMaxFrameCount = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(5)]
        public ushort Unknown02
        {
            get { return this.mUnknown02; }
            set
            {
                this.mUnknown02 = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(6)]
        public string AnimName
        {
            get { return this.mAnimName; }
            set
            {
                this.mAnimName = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(7)]
        public string SrcName
        {
            get { return this.mSrcName; }
            set
            {
                this.mSrcName = value;
                this.OnElementChanged();
            }
        }

        [ElementPriority(8)]
        public TrackList Tracks
        {
            get { return this.mTracks; }
            set
            {
                this.mTracks = value;
                this.OnElementChanged();
            }
        }

        public string Value
        {
            get { return this.ValueBuilder; }
        }

        #endregion

        #region Constructors

        public Clip(int apiVersion, EventHandler handler)
            : base(apiVersion, handler)
        {
            this.mTracks = new TrackList(handler);
        }

        public Clip(int apiVersion, EventHandler handler, Stream s)
            : base(apiVersion, handler)
        {
            s.Position = 0L;
            this.Parse(s);
        }

        #endregion

        #region I/O

        bool checking = Settings.Settings.Checking;

        protected void Parse(Stream s)
        {
            var br = new BinaryReader(s);
            string foo = FOURCC(br.ReadUInt64());
            if (foo != "_pilC3S_")
            {
                throw new Exception("Bad clip header: Expected \"_S3Clip_\"");
            }
            this.mVersion = br.ReadUInt32();
            this.mUnknown01 = br.ReadUInt32();
            this.mFrameDuration = br.ReadSingle();
            this.mMaxFrameCount = br.ReadUInt16();
            this.mUnknown02 = br.ReadUInt16();

            uint curveCount = br.ReadUInt32();
            uint indexedFloatCount = br.ReadUInt32();
            uint curveDataOffset = br.ReadUInt32();
            uint frameDataOffset = br.ReadUInt32();
            uint animNameOffset = br.ReadUInt32();
            uint srcNameOffset = br.ReadUInt32();

            if (this.checking && s.Position != curveDataOffset)
            {
                throw new InvalidDataException("Bad Curve Data Offset");
            }

            var curveDataInfos = new List<CurveDataInfo>();
            for (int i = 0; i < curveCount; i++)
            {
                var p = new CurveDataInfo();
                p.FrameDataOffset = br.ReadUInt32();
                p.TrackKey = br.ReadUInt32();
                p.Offset = br.ReadSingle();
                p.Scale = br.ReadSingle();
                p.FrameCount = br.ReadUInt16();
                p.Flags = new CurveDataFlags(br.ReadByte());
                p.Type = (CurveType)br.ReadByte();
                curveDataInfos.Add(p);
            }

            if (this.checking && s.Position != animNameOffset)
            {
                throw new InvalidDataException("Bad Name Offset");
            }
            this.mAnimName = br.ReadZString();
            if (this.checking && s.Position != srcNameOffset)
            {
                throw new InvalidDataException("Bad SourceName Offset");
            }
            this.mSrcName = br.ReadZString();

            if (this.checking && s.Position != frameDataOffset)
            {
                throw new InvalidDataException("Bad Indexed Floats Offset");
            }
            var indexedFloats = new List<float>();
            for (int i = 0; i < indexedFloatCount; i++)
            {
                indexedFloats.Add(br.ReadSingle());
            }

            var trackMap = new Dictionary<uint, List<Curve>>();
            var test = curveDataInfos.Where(x => x.Type != CurveType.Position && x.Type != CurveType.Orientation).ToArray();
            for (int i = 0; i < curveDataInfos.Count; i++)
            {
                CurveDataInfo curveDataInfo = curveDataInfos[i];
                try
                {

                    if (this.checking && s.Position != curveDataInfo.FrameDataOffset)
                        throw new InvalidDataException("Bad FrameData offset.");
                    Curve curve;

                    curve = new Curve(0, this.handler, curveDataInfo.Type, curveDataInfo.Flags.Type, s, curveDataInfo, indexedFloats);

                    if (!trackMap.ContainsKey(curveDataInfo.TrackKey))
                    {
                        trackMap[curveDataInfo.TrackKey] = new List<Curve>();
                    }
                    trackMap[curveDataInfo.TrackKey].Add(curve);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Can't load channel with type: " + curveDataInfo.Type);
                }

            }

            var tracks = new List<Track>();
            foreach (uint k in trackMap.Keys)
            {
                tracks.Add(new Track(0, this.handler, k, trackMap[k]));
            }
            this.mTracks = new TrackList(this.handler, tracks);

            if (this.checking && s.Position != s.Length)
                throw new InvalidDataException("Unexpected End of Clip.");
        }

        public void UnParse(Stream s)
        {
            var bw = new BinaryWriter(s);
            bw.Write(Encoding.ASCII.GetBytes("_pilC3S_"));
            bw.Write(this.mVersion);
            bw.Write(this.mUnknown01);
            bw.Write(this.mFrameDuration);
            bw.Write(this.mMaxFrameCount);
            bw.Write(this.Unknown02);

            var indexedFloats = new List<float>();
            var curveDataInfos = new List<CurveDataInfo>();


            uint curveCount = 0;
            byte[] frameData;
            using (var frameStream = new MemoryStream())
            {
                foreach (Track track in this.Tracks)
                {
                    foreach (Curve curve in track.Curves)
                    {
                        curveCount++;
                        float scale = 1f;
                        float offset = 0f;

                        IEnumerable<float> values = curve.SelectFloats();
                        if (values.Any())
                        {
                            float min = values.Min();
                            float max = values.Max();
                            offset = (min + max) / 2f;
                            scale = (min - max) / 2f;
                        }
                        //not sure what really determines whether to index or not
                        bool isIndexed = curve.Frames.Count == 0 ? true : curve.Type == CurveType.Position ? IsIndexed(curve.Frames.Cast<Frame>()) : false;
                        var flags = new CurveDataFlags();
                        flags.Format = isIndexed ? CurveDataFormat.Indexed : CurveDataFormat.Packed;
                        flags.Type = curve.Frames.DataType;
                        flags.Static = curve.Frames.Count == 0;
                        var curveDataInfo = new CurveDataInfo { Offset = offset, Flags = flags, FrameCount = curve.Frames.Count, FrameDataOffset = (uint)frameStream.Position, Scale = scale, TrackKey = track.TrackKey, Type = curve.Type };
                        curve.UnParse(frameStream, curveDataInfo, indexedFloats);
                        curveDataInfos.Add(curveDataInfo);
                    }
                }
                frameData = frameStream.ToArray();
            }

            bw.Write(curveCount);
            bw.Write(indexedFloats.Count);
            long offsets = s.Position;
            uint curveDataOffset = 0;
            uint frameDataOffset = 0;
            uint animNameOffset = 0;
            uint srcNameOffset = 0;
            s.Seek(4 * sizeof(uint), SeekOrigin.Current);


            curveDataOffset = (uint)s.Position;
            var frameOffset = (uint)(curveDataOffset + (20 * curveDataInfos.Count) + this.mAnimName.Length + this.mSrcName.Length + 2 + (sizeof(float) * indexedFloats.Count));
            foreach (CurveDataInfo curveDataInfo in curveDataInfos)
            {
                bw.Write((curveDataInfo.FrameDataOffset + frameOffset));
                bw.Write(curveDataInfo.TrackKey);
                bw.Write(curveDataInfo.Offset);
                bw.Write(curveDataInfo.Scale);
                bw.Write((ushort)curveDataInfo.FrameCount);
                bw.Write(curveDataInfo.Flags.Raw);
                bw.Write((byte)curveDataInfo.Type);
            }

            animNameOffset = (uint)s.Position;
            bw.WriteZString(this.AnimName);
            srcNameOffset = (uint)s.Position;
            bw.WriteZString(this.SrcName);

            frameDataOffset = (uint)s.Position;
            foreach (float f in indexedFloats) bw.Write(f);
            bw.Write(frameData);
            s.Seek(offsets, SeekOrigin.Begin);
            bw.Write(curveDataOffset);
            bw.Write(frameDataOffset);
            bw.Write(animNameOffset);
            bw.Write(srcNameOffset);
            s.Position = s.Length;
        }

        #endregion

        private const int kRecommendedApiVersion = 1;

        public override List<string> ContentFields
        {
            get { return GetContentFields(0, this.GetType()); }
        }

        public override int RecommendedApiVersion
        {
            get { return kRecommendedApiVersion; }
        }

        private static bool IsIndexed(IEnumerable<Frame> source)
        {
            if (source.Count() < 5) return false;
            IEnumerable<float> x = source.Select(frame => frame.Data[0]).Distinct();
            IEnumerable<float> y = source.Select(frame => frame.Data[1]).Distinct();
            IEnumerable<float> z = source.Select(frame => frame.Data[2]).Distinct();

            return x.Count() == 1 && x.First() == 0 || y.Count() == 1 && y.First() == 0 || z.Count() == 1 && z.First() == 0;
        }

        public override AHandlerElement Clone(EventHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}
