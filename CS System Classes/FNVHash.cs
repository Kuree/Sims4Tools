using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Security.Cryptography
{
    /// <summary>
    /// Base class implementing <see cref="System.Security.Cryptography.HashAlgorithm"/>.
    /// For full documentation, refer to http://www.sims2wiki.info/wiki.php?title=FNV
    /// </summary>
    public abstract class FNVHash : HashAlgorithm
    {
        ulong prime;
        ulong offset;
        /// <summary>
        /// Algorithm result, needs casting to appropriate size by concrete classes (because I'm lazy)
        /// </summary>
        protected ulong hash;
        /// <summary>
        /// Initialise the hash algorithm
        /// </summary>
        /// <param name="prime">algorithm-specific value</param>
        /// <param name="offset">algorithm-specific value</param>
        protected FNVHash(ulong prime, ulong offset) { this.prime = prime; this.offset = offset; hash = offset; }

        /// <summary>
        /// Method for hashing a string
        /// </summary>
        /// <param name="value">string</param>
        /// <returns>FNV hash of string</returns>
        public byte[] ComputeHash(string value) { return this.ComputeHash(System.Text.ASCIIEncoding.ASCII.GetBytes(value.ToLowerInvariant())); }

        /// <summary>
        /// Nothing to initialize
        /// </summary>
        public override void Initialize() { }

        /// <summary>
        /// Implements the algorithm
        /// </summary>
        /// <param name="array">The input to compute the hash code for.</param>
        /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
        /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            for (int i = ibStart; i < ibStart + cbSize; i++) { hash *= prime; hash ^= array[i]; }
        }

        /// <summary>
        /// Returns the computed hash code.
        /// </summary>
        /// <returns>The computed hash code.</returns>
        protected override byte[] HashFinal() { HashValue = BitConverter.GetBytes(hash); return HashValue; }
    }

    /// <summary>
    /// FNV32 hash routine
    /// </summary>
    public class FNV32 : FNVHash
    {
        /// <summary>
        /// Initialise the hash algorithm
        /// </summary>
        public FNV32() : base((uint)0x01000193, (uint)0x811C9DC5) { }
        /// <summary>
        /// Gets the value of the computed hash code.
        /// </summary>
        public override byte[] Hash { get { return BitConverter.GetBytes((uint)hash); } }
        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        public override int HashSize { get { return 32; } }
        /// <summary>
        /// Get the FNV32 hash for a string of text
        /// </summary>
        /// <param name="text">the text to get the hash for</param>
        /// <returns>the hash value</returns>
        public static uint GetHash(string text) { return BitConverter.ToUInt32(new System.Security.Cryptography.FNV32().ComputeHash(text), 0); }
    }

    /// <summary>
    /// FNV64 hash routine
    /// </summary>
    public class FNV64 : FNVHash
    {
        /// <summary>
        /// Initialise the hash algorithm
        /// </summary>
        public FNV64() : base((ulong)0x00000100000001B3, (ulong)0xCBF29CE484222325) { }
        /// <summary>
        /// Gets the value of the computed hash code.
        /// </summary>
        public override byte[] Hash { get { return BitConverter.GetBytes(hash); } }
        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        public override int HashSize { get { return 64; } }
        /// <summary>
        /// Get the FNV64 hash for a string of text
        /// </summary>
        /// <param name="text">the text to get the hash for</param>
        /// <returns>the hash value</returns>
        public static ulong GetHash(string text) { return BitConverter.ToUInt64(new System.Security.Cryptography.FNV64().ComputeHash(text), 0); }
    }


    /// <summary>
    /// FNV64CLIP hash routine
    /// </summary>
    public class FNV64CLIP : FNV64
    {
        /// <summary>
        /// Initialise the hash algorithm
        /// </summary>
        public FNV64CLIP() : base() { }

        /// <summary>
        /// Get the FNV64 hash for use as the IID for a CLIP of a given name.
        /// </summary>
        /// <param name="text">the CLIP name to get the hash for</param>
        /// <returns>the hash value</returns>
        public static new ulong GetHash(string text)
        {
            string value = text;
            ulong mask = 0;

            string[] split = text.Split(new char[] { '_', }, 2);
            if (split.Length > 1 && split[0].Length <= 5)
            {
                string[] x2y = split[0].Split(new char[] { '2', }, 2);
                if (x2y[0].Length > 0 && x2y[0].Length <= 2)
                {

                    byte xAge = Mask(x2y[0]);

                    if (x2y.Length > 1 && x2y[1].Length > 0 && x2y[1].Length <= 2)
                    {
                        if (!(ao.Contains(x2y[0]) && ao.Contains(x2y[1])))
                        {
                            byte yAge = Mask(x2y[1]);
                            value = string.Join("_", new string[] { (x2y[0][0] == 'o' ? "o" : "a") + "2" + (x2y[1][0] == 'o' ? "o" : "a"), split[1], });
                            mask = (ulong)(0x8000 | xAge << 8 | yAge);
                        }
                    }
                    else if (!ao.Contains(x2y[0]))
                    {
                        value = string.Join("_", new string[] { "a", split[1], });
                        mask = (ulong)(0x8000 | xAge << 8);
                    }

                }
            }

            ulong hash = FNV64.GetHash(value);
            hash &= 0x7FFFFFFFFFFFFFFF;
            hash ^= mask << 48;

            return hash;
        }

        /// <summary>
        /// Get the FNV64 hash for use as the IID for a CLIP but ignoring age and species.
        /// </summary>
        /// <param name="text">The CLIP name to get the generic hash for.</param>
        /// <returns>The generic hash value</returns>
        public static ulong GetHashGeneric(string text)
        {
            string value = GetGenericValue(text);

            ulong hash = FNV64.GetHash(value);
            hash &= 0x7FFFFFFFFFFFFFFF;

            return hash;
        }

        /// <summary>
        /// Get the "generic" CLIP, removing age and species.
        /// </summary>
        /// <param name="text">The CLIP name from which to et the generic value.</param>
        /// <returns>The generic CLIP name.</returns>
        public static string GetGenericValue(string text)
        {
            string value = text;

            string[] split = text.Split(new char[] { '_', }, 2);
            if (split.Length > 1 && split[0].Length <= 5)
            {
                string[] x2y = split[0].Split(new char[] { '2', }, 2);
                if (x2y[0].Length > 0 && x2y[0].Length <= 2)
                {

                    if (x2y.Length > 1 && x2y[1].Length > 0 && x2y[1].Length <= 2)
                    {
                        if (!(ao.Contains(x2y[0]) && ao.Contains(x2y[1])))
                        {
                            value = string.Join("_", new string[] { (x2y[0][0] == 'o' ? "o" : "a") + "2" + (x2y[1][0] == 'o' ? "o" : "a"), split[1], });
                        }
                    }
                    else if (!ao.Contains(x2y[0]))
                    {
                        value = string.Join("_", new string[] { "a", split[1], });
                    }

                }
            }

            return value;
        }

        static string[] ao = { "a", "o", };
        static byte Mask(string actor)
        {
            switch (actor)
            {
                case "b": return 0x01;
                case "p": return 0x02;
                case "c": return 0x03;
                case "t": return 0x04;
                case "h": return 0x05;
                case "e": return 0x06;
                case "ad": return 0x08;
                case "cd": return 0x09;
                case "al": return 0x0A;
                case "ac": return 0x0D;
                case "cc": return 0x0E;
                case "ah": return 0x10;
                case "ch": return 0x11;
                case "ab": return 0x12;
                case "ar": return 0x13;
                default: return 0x00;
            }
        }
    }
}
