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