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
using s4pi.Interfaces;

namespace s4pi.Animation.S3CLIP
{
    public class CurveList : DependentList<Curve>
    {
        public CurveList(EventHandler handler, IEnumerable<Curve> ilt)
            : base(handler, ilt)
        {
        }

        public CurveList(EventHandler handler)
            : base(handler)
        {
        }

        #region Unused

        public override void Add()
        {
            throw new NotSupportedException();
        }

        protected override Curve CreateElement(Stream s)
        {
            throw new NotSupportedException();
        }

        protected override void WriteElement(Stream s, Curve element)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}