using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.GenericRCOLResource
{
    /// <summary>
    /// An RCOL block handler that "does nothing", providing the minimal support required for any RCOL block in a <see cref="GenericRCOLResource"/>.
    /// </summary>
    public sealed class DefaultRCOL : ARCOLBlock
    {
        byte[] data = new byte[0];

        /// <summary>
        /// <see cref="DefaultRCOL"/> does not provide a constructor that <see cref="GenericRCOLResourceHandler.CreateRCOLBlock"/> can call.
        /// <para>Calling this constructor throws a <see cref="NotImplementedException"/>.</para>
        /// </summary>
        /// <param name="APIversion">Unused.</param>
        /// <param name="handler">Unused.</param>
        /// <exception cref="NotImplementedException">Thrown if this constructor is called.</exception>
        public DefaultRCOL(int APIversion, EventHandler handler) : base(APIversion, handler, null) { throw new NotImplementedException(); }
        /// <summary>
        /// Read the block data from the <see cref="Stream"/> provided.
        /// </summary>
        /// <param name="APIversion">Unused; requested API version.</param>
        /// <param name="handler">Unused; change <see cref="EventHandler"/>.</param>
        /// <param name="s"><see cref="Stream"/> containing the data to read.</param>
        public DefaultRCOL(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler, s) { }
        /// <summary>
        /// Create a new instance from an existing instance.
        /// </summary>
        /// <param name="APIversion">Unused; requested API version.</param>
        /// <param name="handler">Unused; change <see cref="EventHandler"/>.</param>
        /// <param name="basis">An existing <see cref="DefaultRCOL"/> to use as a basis.</param>
        public DefaultRCOL(int APIversion, EventHandler handler, DefaultRCOL basis) : base(APIversion, null, null) { this.handler = handler; data = (byte[])basis.data.Clone(); }

        /// <summary>
        /// Read the data.
        /// </summary>
        /// <param name="s">The <see cref="Stream"/> containing the data.</param>
        protected override void Parse(Stream s) { data = new byte[s.Length]; s.Read(data, 0, (int)s.Length); }

        // /// <summary>
        // /// Creating a copy of this instance with a new change <see cref="EventHandler"/>.
        // /// </summary>
        // /// <param name="handler">The <see cref="EventHandler"/> for the new instance.</param>
        // /// <returns>A new instance with a copy of the data and the given change <see cref="EventHandler"/>.</returns>
        // public override AHandlerElement Clone(EventHandler handler) { return new DefaultRCOL(requestedApiVersion, handler, this); }

        /// <summary>
        /// DefaultRCOL only supplies &quot;Tag&quot; for <see cref="GenericRCOLResourceHandler"/>.
        /// <para>It returns &quot;*&quot; to indicate it is the default RCOL block handler.</para>
        /// </summary>
        [ElementPriority(2)]
        public override string Tag { get { return "*"; } } // For RCOLDealer

        /// <summary>
        /// DefaultRCOL only supplies &quot;ResourceType&quot; for <see cref="GenericRCOLResourceHandler"/>.
        /// <para>It returns <c>(uint)-1</c>.</para>
        /// </summary>
        [ElementPriority(3)]
        public override uint ResourceType { get { return 0xFFFFFFFF; } }

        /// <summary>
        /// Return the data in a <see cref="Stream"/>.
        /// </summary>
        /// <returns>The data in a <see cref="Stream"/>.</returns>
        public override Stream UnParse() { MemoryStream ms = new MemoryStream(); ms.Write(data, 0, data.Length); return ms; }

        /// <summary>
        /// A default string to display, identifying any tag within the data and the length of the data block.
        /// </summary>
        public string Value { get { return "Tag: " + FOURCC(BitConverter.ToUInt32(data, 0)) + "\nLength: " + data.Length; } }
    }
}
