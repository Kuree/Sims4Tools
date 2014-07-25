using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.Animation.S3CLIP
{
    public class TrackList : DependentList<Track>
    {
        public TrackList(EventHandler handler)
            : base(handler)
        {
        }

        public TrackList(EventHandler handler, IEnumerable<Track> ilt)
            : base(handler, ilt)
        {
        }

        public TrackList(EventHandler handler, long size)
            : base(handler, size)
        {
        }

        protected override Track CreateElement(Stream s)
        {
            throw new NotSupportedException();
        }

        protected override void WriteElement(Stream s, Track element)
        {
            throw new NotSupportedException();
        }
    }
}