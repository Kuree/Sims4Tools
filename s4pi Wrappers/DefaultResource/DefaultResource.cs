using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;

namespace s4pi.DefaultResource
{
    /// <summary>
    /// A resource wrapper that "does nothing", providing the minimal support required for any resource in a Package.
    /// </summary>
    public sealed class DefaultResource : AResource
    {
        const Int32 recommendedApiVersion = 1;

        #region AApiVersionedFields
        /// <summary>
        /// Return the version number that this wrapper prefers to be called with (the default if passed zero).
        /// </summary>
        /// <remarks>This wrapper returns <c>1</c> and is not sensitive to API version.</remarks>
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        #endregion

        /// <summary>
        /// Create a new instance of the resource.
        /// </summary>
        /// <param name="APIversion">Requested API version</param>
        /// <param name="s">Data stream to use, or null to create from scratch</param>
        public DefaultResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = new MemoryStream(); dirty = true; } }

        /// <summary>
        /// <see cref="DefaultResource"/> does not know how to parse anything, so this method is unimplemented.
        /// </summary>
        /// <returns>Throws <see cref="NotImplementedException"/>.</returns>
        /// <exception cref="NotImplementedException">Thrown if called.</exception>
        protected override Stream UnParse() { throw new NotImplementedException(); }

        /// <summary>
        /// The resource content as a Stream, with the stream position set to the begining.
        /// </summary>
        public override Stream Stream { get { stream.Position = 0; return stream; } }
    }

    /// <summary>
    /// ResourceHandler for DefaultResource wrapper
    /// </summary>
    public class DefaultResourceHandler : AResourceHandler
    {
        /// <summary>
        /// Populate the <see cref="AResourceHandler"/> Dictionary.
        /// <para>&quot;*&quot; is used to inform WrapperDealer this will happily wrap any resource.</para>
        /// </summary>
        public DefaultResourceHandler()
        {
            this.Add(typeof(DefaultResource), new List<string>(new string[] { "*" }));
        }
    }
}
