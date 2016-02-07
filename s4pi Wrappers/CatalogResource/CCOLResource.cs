/***************************************************************************
 *  Copyright (C) 2009, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Inge Jones                                                             *
 *  Buzzler                                                                *
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

using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace CatalogResource
{
    public class CCOLResource : AbstractCatalogResource
    {
        private uint unknown01;
        private uint refIndicator;
        private Gp9references modlRefs; //only if unk15 != 0
        private Gp9references ftptRefs; //only if unk15 != 0
        private Gp4references nullRefs; //only if unk15 == 0

        #region Constructors

        public CCOLResource(int apiVersion, Stream s)
            : base(apiVersion, s)
        {
            if (s == null || s.Length == 0)
            {
                s = this.UnParse();
                this.OnResourceChanged(this, EventArgs.Empty);
            }
            s.Position = 0;
            this.Parse(s);
        }

        #endregion

        #region Content Fields

        /*
         * Start counting at 40, because base class already goes up to 38.
         * 
         * Remember to check base class when you add properties!
         */

        [ElementPriority(40)]
        public uint Unknown01
        {
            get { return this.unknown01; }
            set
            {
                if (this.unknown01 != value)
                {
                    this.unknown01 = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(43)]
        public uint ReferencesIndicator
        {
            get { return this.refIndicator; }
            set
            {
                if (this.refIndicator != value)
                {
                    this.refIndicator = value;
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(44)]
        public Gp9references MODLRefs
        {
            get { return this.modlRefs; }
            set
            {
                if (this.modlRefs != value)
                {
                    this.modlRefs = new Gp9references(this.RecommendedApiVersion, this.OnResourceChanged, value);
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(45)]
        public Gp9references FTPTRefs
        {
            get { return this.ftptRefs; }
            set
            {
                if (this.ftptRefs != value)
                {
                    this.ftptRefs = new Gp9references(this.RecommendedApiVersion, this.OnResourceChanged, value);
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        [ElementPriority(46)]
        public Gp4references NullRefs
        {
            get { return this.nullRefs; }
            set
            {
                if (this.nullRefs != value)
                {
                    this.nullRefs = new Gp4references(this.RecommendedApiVersion, this.OnResourceChanged, value);
                    this.OnResourceChanged(this, EventArgs.Empty);
                }
            }
        }

        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                if (this.Version < 0x00000019)
                {
                    res.Remove("Unk10");
                    res.Remove("Unk11");
                    res.Remove("Unk12");
                    res.Remove("Unk13");
                }
                if (this.refIndicator == 0)
                {
                    res.Remove("MODLRefs");
                    res.Remove("FTPTRefs");
                }
                else
                {
                    res.Remove("NullRefs");
                }
                return res;
            }
        }

        #endregion ContentFields ===========================================================================

        #region Data I/O

        protected override void Parse(Stream s)
        {
            base.Parse(s);
            var reader = new BinaryReader(s);

            this.unknown01 = reader.ReadUInt32();
            this.refIndicator = reader.ReadUInt32();
            if (this.refIndicator == 0)
            {
                this.nullRefs = new Gp4references(this.RecommendedApiVersion, this.OnResourceChanged, s);
            }
            else
            {
                this.modlRefs = new Gp9references(this.RecommendedApiVersion, this.OnResourceChanged, s);
                this.ftptRefs = new Gp9references(this.RecommendedApiVersion, this.OnResourceChanged, s);
            }
        }

        protected override Stream UnParse()
        {
            Stream stream = base.UnParse();
            var writer = new BinaryWriter(stream);

            writer.Write(this.unknown01);
            writer.Write(this.refIndicator);
            if (this.refIndicator == 0)
            {
                if (this.nullRefs == null)
                {
                    this.nullRefs = new Gp4references(this.RecommendedApiVersion, this.OnResourceChanged);
                }
                this.nullRefs.UnParse(stream);
            }
            else
            {
                if (this.modlRefs == null)
                {
                    this.modlRefs = new Gp9references(this.RecommendedApiVersion, this.OnResourceChanged);
                }
                this.modlRefs.UnParse(stream);
                if (this.ftptRefs == null)
                {
                    this.ftptRefs = new Gp9references(this.RecommendedApiVersion, this.OnResourceChanged);
                }
                this.ftptRefs.UnParse(stream);
            }

            return stream;
        }

        #endregion DataIO ================================
    }

    /// <summary>
    /// ResourceHandler for CCOLResource wrapper
    /// </summary>
    public class CCOLResourceHandler : AResourceHandler
    {
        public CCOLResourceHandler()
        {
            this.Add(typeof(CCOLResource), new List<string>(new string[] { "0x1D6DF1CF", }));
        }
    }
}