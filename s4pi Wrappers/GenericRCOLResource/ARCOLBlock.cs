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

namespace s4pi.Interfaces
{
    /// <summary>
    /// Represents the abstract RCOL block within a <see cref="GenericRCOLResource"/>.
    /// </summary>
    public abstract class ARCOLBlock : AHandlerElement, IRCOLBlock, IEquatable<ARCOLBlock>
    {
        const int recommendedApiVersion = 1;

        /// <summary>
        /// For ValueBuilder purposes, the list of external resource keys is passed down here.
        /// Not all RCOLs need it, so it may end up unused.  Override this property to take
        /// specific action (such as passing the reference further on).
        /// </summary>
        public virtual DependentList<TGIBlock> ParentTGIBlocks { get; set; }

        /// <summary>
        /// Holds the requested API version.
        /// </summary>
        protected int requestedAPIversion;
        /// <summary>
        /// Holds the stream from which the block was read.
        /// </summary>
        /// <remarks>Note that the state of the stream may change once the <see cref="Parse"/> method completes.</remarks>
        protected Stream stream;

        /// <summary>
        /// Create a new instance based on the data in the supplied <see cref="Stream"/>.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The change event handler for the resource.</param>
        /// <param name="s">The <see cref="Stream"/> containing the data.</param>
        /// <seealso cref="Parse"/>
        public ARCOLBlock(int APIversion, EventHandler handler, Stream s)
            : base(APIversion, handler)
        {
            stream = s;
            if (stream == null) { stream = UnParse(); OnElementChanged(); }
            stream.Position = 0;
            Parse(stream);
        }

        /// <summary>
        /// Any implementation must provide a method of reading data from the resource.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> referencing the data in the resource, position to the start of this block.</param>
        /// <remarks>Any implementation must ensure the stream position ends at the first byte of the next block.</remarks>
        protected abstract void Parse(Stream s);

        #region AApiVersionedFields
        static List<string> ARCOLBlockBanlist = new List<string>(new string[] {
            "Tag", "ResourceType", "Data",
        });
        /// <summary>
        /// Remove the additional &quot;banned&quot; fields for an RCOL block.
        /// </summary>
        protected override List<string> ValueBuilderFields
        {
            get
            {
                List<string> fields = base.ValueBuilderFields;
                fields.RemoveAll(ARCOLBlockBanlist.Contains);
                return fields;
            }
        }
        #endregion

        #region AHandlerElement
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields
        {
            get
            {
                List<string> res = GetContentFields(requestedApiVersion, this.GetType());
                res.Remove("ParentTGIBlocks");
                return res;
            }
        }

        /// <summary>
        /// Unless overridden in an implementing class, returns <c>1</c>.
        /// </summary>
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        // /// <summary>
        // /// Implementing classes must provide a means of creating a copy of themselves.
        // /// </summary>
        // /// <param name="handler">The <see cref="EventHandler"/> for the new instance.</param>
        // /// <returns>A new instance of the class with identical values, with the given change <see cref="EventHandler"/>.</returns>
        // public abstract override AHandlerElement Clone(EventHandler handler);
        #endregion

        #region IRCOLBlock Members
        /// <summary>
        /// The &quot;FOUR CC&quot; tag identifying this RCOL block; may be <c>null</c> if not present.
        /// </summary>
        [ElementPriority(2)]
        public abstract string Tag { get; }
        /// <summary>
        /// The Resource Type identifying this block format.
        /// </summary>
        [ElementPriority(3)]
        public abstract uint ResourceType { get; }
        /// <summary>
        /// Implementing classes must provide a means of storing themselves in a <see cref="Stream"/>.
        /// </summary>
        /// <returns>The <see cref="Stream"/> containing the data from the RCOL block.</returns>
        public abstract Stream UnParse();
        #endregion

        #region IResource Members
        /// <summary>
        /// The resource content as a Stream
        /// </summary>
        public virtual Stream Stream
        {
            get
            {
                if (dirty || s4pi.Settings.Settings.AsBytesWorkaround)
                {
                    stream = UnParse();
                    dirty = false;
                    //Console.WriteLine(this.GetType().Name + " flushed.");
                }
                stream.Position = 0;
                return stream;
            }
        }

        /// <summary>
        /// The resource content as a byte array
        /// </summary>
        [ElementPriority(0)]
        public virtual byte[] AsBytes
        {
            get
            {
                MemoryStream s = this.Stream as MemoryStream;
                if (s != null) return s.ToArray();

                stream.Position = 0;
                return (new BinaryReader(stream)).ReadBytes((int)stream.Length);
            }
            set { MemoryStream ms = new MemoryStream(value); Parse(ms); OnRCOLChanged(this, EventArgs.Empty); }
        }

        //disable "Never used" warning, as this is used by library users rather than the library itself.
#pragma warning disable 67
        /// <summary>
        /// Raised if the resource is changed
        /// </summary>
        public event EventHandler ResourceChanged;
#pragma warning restore 67

        #endregion

        #region IEquatable<ARCOLBlock> Members
        /// <summary>
        /// Indicates whether the current <see cref="ARCOLBlock"/> is equal to another instance.
        /// </summary>
        /// <param name="other">Another instance to compare with this <see cref="ARCOLBlock"/>.</param>
        /// <returns><c>true</c> if the current <see cref="ARCOLBlock"/> is equal to the <paramref name="other"/> parameter;
        /// otherwise, <c>false</c>.</returns>
        public virtual bool Equals(ARCOLBlock other) { return this.AsBytes.Equals<byte>(other.AsBytes); }

        /// <summary>
        /// Indicates whether the current <see cref="ARCOLBlock"/> is equal to another object of the same type.
        /// </summary>
        /// <param name="obj">An object to compare with this <see cref="ARCOLBlock"/>.</param>
        /// <returns><c>true</c> if the current <see cref="ARCOLBlock"/> is equal to the <paramref name="obj"/> parameter;
        /// otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) { return obj as ARCOLBlock != null && this.Equals(obj as ARCOLBlock); }

        /// <summary>
        /// Serves as a hash function for an <see cref="ARCOLBlock"/>.
        /// </summary>
        /// <returns>A hash code for the current <see cref="ARCOLBlock"/>.</returns>
        public override int GetHashCode() { return AsBytes.GetHashCode(); }
        #endregion

        /// <summary>
        /// Used to indicate the RCOL has changed.
        /// </summary>
        protected virtual void OnRCOLChanged(object sender, EventArgs e) { OnElementChanged(); }

        /// <summary>
        /// To allow editor import/export as a minimum.
        /// </summary>
        [ElementPriority(1)]
        public virtual BinaryReader Data
        {
            get { return new BinaryReader(UnParse()); }
            set
            {
                if (value.BaseStream.CanSeek) { value.BaseStream.Position = 0; Parse(value.BaseStream); }
                else
                {
                    MemoryStream ms = new MemoryStream();
                    byte[] buffer = new byte[1024 * 1024];
                    for (int read = value.BaseStream.Read(buffer, 0, buffer.Length); read > 0; read = value.BaseStream.Read(buffer, 0, buffer.Length))
                        ms.Write(buffer, 0, read);
                    Parse(ms);
                }
                OnRCOLChanged(this, EventArgs.Empty);
            }
        }
    }
}
