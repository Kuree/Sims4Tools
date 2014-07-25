using System;
using System.Collections.Generic;
using s4pi.Interfaces;

namespace s4pi.Animation.S3CLIP
{
    public class Track : AHandlerElement,
        IEquatable<Track>
    {
        private CurveList m_curves;
        private UInt32 mTrackKey;

        public Track(int APIversion, EventHandler handler)
            : base(APIversion, handler)
        {
            m_curves = new CurveList(handler);
        }

        public Track(int APIversion, EventHandler handler, Track basis)
            : this(APIversion, handler, basis.mTrackKey, basis.Curves)
        {
        }

        public Track(int APIversion, EventHandler handler, UInt32 trackKey, IEnumerable<Curve> curves)
            : this(APIversion, handler)
        {
            mTrackKey = trackKey;
            m_curves = new CurveList(handler, curves);
        }

        [ElementPriority(1)]
        public uint TrackKey
        {
            get { return mTrackKey; }
            set
            {
                if (mTrackKey != value)
                {
                    mTrackKey = value;
                    OnElementChanged();
                }
            }
        }

        [ElementPriority(2)]
        public CurveList Curves
        {
            get { return m_curves; }
            set
            {
                m_curves = value;
                OnElementChanged();
            }
        }

        public override List<string> ContentFields
        {
            get { return GetContentFields(requestedApiVersion, GetType()); }
        }

        public override int RecommendedApiVersion
        {
            get { return 1; }
        }

        public string Value
        {
            get { return ValueBuilder; }
        }

        #region IEquatable<Track> Members

        public bool Equals(Track other)
        {
            return mTrackKey.Equals(other.mTrackKey);
        }

        #endregion

        public override AHandlerElement Clone(EventHandler handler)
        {
            return new Track(0, handler, this);
        }
    }
}