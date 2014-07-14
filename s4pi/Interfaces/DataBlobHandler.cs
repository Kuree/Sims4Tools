using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace s4pi.Interfaces
{
    /// <summary>
    /// An extension to <see cref="AHandlerElement"/>, for handling an unknown blobs of data of a set size and displaying those blobs in a way similar to a hex editor.
    /// </summary>
    public class DataBlobHandler : AHandlerElement, IEquatable<DataBlobHandler>
    {
        private const int recommendedApiVersion = 1;

        private byte[] data;

        #region Constructors
        /// <summary>
        /// Initialize a new instance containing the given number of bytes, all of which are set to the default value.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="length">The number of bytes stored in this data blob.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given <paramref name="length"/> is less than zero.</exception>
        public DataBlobHandler(int APIversion, EventHandler handler, long length)
            : base(APIversion, handler)
        {
            if (length < 0) throw new ArgumentOutOfRangeException("length");
            this.data = new byte[length];
        }
        /// <summary>
        /// Initialize a new instance containing the given number of bytes read from the given stream.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="length">The number of bytes stored in this data blob.</param>
        /// <param name="s">The stream from which to read the data contained in the data blob.</param>
        /// <exception cref="ArgumentOutOfRangeException">The given <paramref name="length"/> is less than zero.</exception>
        /// <exception cref="ArgumentNullException">The given stream <paramref name="s"/> is null.</exception>
        public DataBlobHandler(int APIversion, EventHandler handler, int length, Stream s)
            : this(APIversion, handler, length)
        {
            if (s == null) throw new ArgumentNullException("s");
            s.Read(this.data, 0, this.data.Length);
        }
        /// <summary>
        /// Initialize a new instance with a copy of the data contained in the given <paramref name="basis"/>.
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="basis">The data blob from which to copy the data that will be contained in the new data blob.</param>
        /// <exception cref="ArgumentNullException">The given <paramref name="basis"/> is null.</exception>
        public DataBlobHandler(int APIversion, EventHandler handler, DataBlobHandler basis)
            : base(APIversion, handler)
        {
            if (basis == null) throw new ArgumentNullException("basis");
            this.data = new byte[basis.data.Length];
            Array.Copy(basis.data, 0, this.data, 0, basis.data.Length);
        }
        /// <summary>
        /// Initialize a new instance containing the given <paramref name="data"/>
        /// </summary>
        /// <param name="APIversion">The requested API version.</param>
        /// <param name="handler">The <see cref="EventHandler"/> delegate to invoke if the <see cref="AHandlerElement"/> changes.</param>
        /// <param name="data">The data that will be contained in the new data blob.</param>
        /// <exception cref="ArgumentNullException">The given <paramref name="data"/> is null.</exception>
        public DataBlobHandler(int APIversion, EventHandler handler, byte[] data)
            : base(APIversion, handler)
        {
            if (data == null) throw new ArgumentNullException("data");
            this.data = new byte[data.Length];
            Array.Copy(data, 0, this.data, 0, data.Length);
        }
        #endregion

        /// <summary>
        /// Write the data blob to the buffer of the given stream.
        /// </summary>
        /// <param name="s">The stream to write the data blob to.</param>
        public void UnParse(Stream s)
        {
            s.Write(this.data, 0, this.data.Length);
        }

        private static char[] conversionTable = new char[]
        {
            ' ', ' ', ' ', ' ', ' ', ' ', ' ',  ' ', ' ', ' ', ' ', ' ',  ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ',  ' ', ' ', ' ', ' ', ' ',  ' ', ' ', ' ', ' ',
            ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+',  ',', '-', '.', '/',
            '0', '1', '2', '3', '4', '5', '6',  '7', '8', '9', ':', ';',  '<', '=', '>', '?',
            '@', 'A', 'B', 'C', 'D', 'E', 'F',  'G', 'H', 'I', 'J', 'K',  'L', 'M', 'N', 'O',
            'P', 'Q', 'R', 'S', 'T', 'U', 'V',  'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_',
            '`', 'a', 'b', 'c', 'd', 'e', 'f',  'g', 'h', 'i', 'j', 'k',  'l', 'm', 'n', 'o',
            'p', 'q', 'r', 's', 't', 'u', 'v',  'w', 'x', 'y', 'z', '{',  '|', '}', '~', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ',  ' ', ' ', ' ', ' ', ' ',  ' ', ' ', ' ', ' ',
            ' ', ' ', ' ', ' ', ' ', ' ', ' ',  ' ', ' ', ' ', ' ', ' ',  ' ', ' ', ' ', ' ',
            ' ', '¡', '¢', '£', '¤', '¥', '¦',  '§', '¨', '©', 'ª', '«',  '¬', ' ', '®', '¯', 
            '°', '±', '²', '³', '´', 'µ', '¶', '·', '¸',  '¹', 'º', '»',  '¼', '½', '¾', '¿',
            'À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'Æ', 'Ç', 'È',  'É', 'Ê', 'Ë',  'Ì', 'Í', 'Î', 'Ï',
            'Ð', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', '×', 'Ø',  'Ù', 'Ú', 'Û',  'Ü', 'Ý', 'Þ', 'ß', 
            'à', 'á', 'â', 'ã', 'ä', 'å', 'æ', 'ç', 'è',  'é', 'ê', 'ë',  'ì', 'í', 'î', 'ï', 
            'ð', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö', '÷', 'ø',  'ù', 'ú', 'û',  'ü', 'ý', 'þ', 'ÿ'
        };

        /// <summary>
        /// Create a textual representation of the blob data similar which looks similar to a hex editor display,
        /// with 16 bytes displayed per row and an optional row of their ASCII values to the right.
        /// </summary>
        /// <param name="indent">The number of spaces preceding each line of output.</param>
        /// <param name="includeASCII">Whether to include a second column containing the ASCII data.</param>
        /// <returns>A string containing the textual representation of the blob data in hex format.</returns>
        public string BuildHexDisplay(int indent, bool includeASCII)
        {
            if (this.data.Length > 0)
            {
                int i, j, k, size;
                char[] buffer = null;
                if (includeASCII) buffer = new char[16];
                size = (this.data.Length / 16 + 1) * (indent + 60 + (includeASCII ? 11 : 0));
                StringBuilder sb = new StringBuilder(size);
                i = 2;
                size = 256;
                while (size < this.data.Length && size < 0x10000000)
                {
                    i++;
                    size *= 16;
                }
                size = this.data.Length;
                string format = string.Concat("X", i.ToString());
                sb.Append(new string(' ', indent + i));
                sb.Append(" | 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F");
                for (i = 0; i < size; i++)
                {
                    if (i % 16 == 0)
                    {
                        sb.AppendLine();
                        sb.Append(new string(' ', indent));
                        sb.Append(i.ToString(format));
                        sb.Append(" |");
                    }
                    sb.Append(" ");
                    sb.Append(this.data[i].ToString("X2"));
                    if (includeASCII && i % 16 == 15)
                    {
                        sb.Append(" | ");
                        Array.Clear(buffer, 0, 16);
                        k = (i / 16) * 16;
                        for (j = 0; j < 16; j++)
                        {
                            buffer[j] = conversionTable[this.data[k]];
                        }
                        sb.Append(buffer);
                    }
                }
                if (includeASCII && size % 16 != 0)
                {
                    sb.Append(new string(' ', 3 * (16 - (size % 16))));
                    sb.Append(" | ");
                    Array.Clear(buffer, 0, 16);
                    k = (size / 16) * 16;
                    size = size % 16;
                    for (j = 0; j < size; j++)
                    {
                        buffer[j] = conversionTable[this.data[k]];
                    }
                    sb.Append(buffer);
                }
                return sb.ToString();
            }
            else
            {
                return "{Empty Data Blob}";
            }
        }

        #region AHandlerElement
        // public override DataBlobHandler Clone(EventHandler handler) { return new DataBlobHandler(requestedApiVersion, handler, this.data); }
        /// <summary>
        /// The best supported version of the API available
        /// </summary>
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
        /// <summary>
        /// The list of available field names on this API object.
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        #region IEquatable<DataBlobHandler>
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(DataBlobHandler other)
        {
            if (this.data.Length != other.data.Length)
            {
                return false;
            }
            for (int i = this.data.Length - 1; i >= 0; i--)
            {
                if (this.data[i] != other.data[i])
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="DataBlobHandler"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="DataBlobHandler"/>.</param>
        /// <returns>true if the specified <see cref="System.Object"/> is equal to the current <see cref="DataBlobHandler"/>; otherwise, false.</returns>
        /// <exception cref="System.NullReferenceException">The obj parameter is null.</exception>
        public override bool Equals(object obj)
        {
            if (obj is byte[])
            {
                byte[] data = obj as byte[];
                if (this.data.Length != data.Length)
                {
                    return false;
                }
                for (int i = this.data.Length - 1; i >= 0; i--)
                {
                    if (this.data[i] != data[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            else if (obj is DataBlobHandler)
            {
                return this.Equals(obj as DataBlobHandler);
            }
            return false;
        }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return this.data.GetHashCode();
        }
        #endregion

        /// <summary>
        /// The results of <see cref="BuildHexDisplay(int,bool)"/> with zero indent and no included ASCII values.
        /// </summary>
        public string Value { get { return string.Concat("\r\n", this.BuildHexDisplay(0, false)); } }
    }
}
