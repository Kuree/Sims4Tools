using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using s4pi.Interfaces;
using s4pi.TextResource.Properties;

namespace TextResource
{
    [Obsolete("Deprecated.  Use StreamReader().ReadToEnd()")]
    public class TextResource : AResource
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
        public TextResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = new MemoryStream(); dirty = true; } }

        protected override Stream UnParse() { throw new NotImplementedException(); }

        /// <summary>
        /// The resource content as a Stream
        /// </summary>
        public override Stream Stream { get { stream.Position = 0; return stream; } }

        /// <summary>
        /// Wrap a StreamReader around the resource stream, leaving the underlying position unchanged
        /// </summary>
        [MinimumVersion(1)]
        [MaximumVersion(recommendedApiVersion)]
        public TextReader TextFileReader
        {
            get { return new StreamReader(this.stream, true); }
            set
            {
                MemoryStream ms = new MemoryStream();
                (new BinaryWriter(ms)).Write(value.ReadToEnd().ToCharArray());
                ms.Flush();
                ms.Position = 0;
                stream = ms;
                OnResourceChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Return the resource stream as a string (resetting stream position)
        /// </summary>
        [MinimumVersion(1)]
        [MaximumVersion(recommendedApiVersion)]
        public string Value { get { this.stream.Position = 0; return TextFileReader.ReadToEnd(); } }

        /// <summary>
        /// Wrap an XmlFileReader around the resource stream, leaving the underlying position unchanged
        /// </summary>
        [MinimumVersion(1)]
        [MaximumVersion(recommendedApiVersion)]
        public XmlReader XmlFileReader
        {
            get
            {
                try
                {
                    XmlReaderSettings xrs = new XmlReaderSettings();
                    xrs.CloseInput = false;
                    xrs.IgnoreComments = false;
                    xrs.IgnoreProcessingInstructions = false;
                    xrs.IgnoreWhitespace = false;
                    xrs.ValidationType = ValidationType.None;
                    return XmlReader.Create(this.stream, xrs);
                }
                catch { return null; }
            }
        }

        /// <summary>
        /// Return the resource stream as an XML document (resetting stream position)
        /// </summary>
        /// <remarks>Returns null on failure -- likely if the the stream isn't valid XML</remarks>
        [MinimumVersion(1)]
        [MaximumVersion(recommendedApiVersion)]
        public XmlDocument Xml
        {
            get
            {
                try
                {
                    XmlDocument xd = new XmlDocument();
                    this.stream.Position = 0;
                    xd.Load(XmlFileReader);
                    return xd;
                }
                catch { return null; }
            }
        }
    }

    public class TextResourceHandler : AResourceHandler
    {
        #region Read config file

        static List<string> resourceTypes = null;
        static TextResourceHandler()
        {
            StringReader sr = new StringReader(Resources.TextResources);
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
        public TextResourceHandler()
        {
            this.Add(typeof(TextResource), new List<string>(resourceTypes.ToArray()));
        }
    }
}
