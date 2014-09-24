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
using System.Drawing;
using System.IO;
using s4pi.Interfaces;
using s4pi.ImageResource.Properties;

namespace ImageResource
{
    /// <summary>
    /// A resource wrapper that understands images -- any that .Net supports with Image.FromStream()
    /// </summary>
    [Obsolete("Deprecated.  Use Image.FromStream()")]
    public class ImageResource : AResource
    {
        const Int32 recommendedApiVersion = 1;

        #region AApiVersionedFields
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        /// <summary>
        /// Create a new instance of the resource
        /// </summary>
        /// <param name="APIversion">Requested API version</param>
        /// <param name="s">Data stream to use, or null to create from scratch</param>
        public ImageResource(int APIversion, Stream s)
            : base(APIversion, s)
        {
            if (stream != null) return;
            stream = new MemoryStream();
            (new Bitmap(128, 128)).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        }

        protected override Stream UnParse() { throw new NotImplementedException(); }

        /// <summary>
        /// The resource content as a Stream
        /// </summary>
        public override Stream Stream { get { stream.Position = 0; return stream; } }

        /// <summary>
        /// Return the resource data as a System.Drawing.Image (resetting stream position)
        /// </summary>
        [MinimumVersion(1)]
        [MaximumVersion(recommendedApiVersion)]
        public Image Value { get { this.stream.Position = 0; return Image.FromStream(this.stream); } }
    }

    public class ImageResourceHandler : AResourceHandler
    {
        #region Read config file
        static List<string> resourceTypes = null;
        static ImageResourceHandler()
        {
            StringReader sr = new StringReader(Resources.ImageResources);
            resourceTypes = new List<string>();
            string s;
            while ((s = sr.ReadLine()) != null)
            {
                string[] t = s.Split(new char[] { ' ' }, 2);
                resourceTypes.Add(t[0]);
            }
        }
        #endregion

        /// <summary>
        /// Create the content of the Dictionary.
        /// List of resource types is read once from a configuration file in the same folder as this assembly.
        /// </summary>
        public ImageResourceHandler()
        {
            this.Add(typeof(ImageResource), new List<string>(resourceTypes.ToArray()));
        }
    }
}
