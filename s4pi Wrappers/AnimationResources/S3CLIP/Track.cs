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