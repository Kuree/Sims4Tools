using System;
using System.Collections.Generic;

namespace System.Drawing
{
    /// <summary>
    /// Implement ColorHSVA class and conversion to ColorRGBA
    /// </summary>
    /// <remarks>See <a href="http://www.academictutorials.com/graphics/graphics-the-hsl-color-model.asp">this paper</a>
    /// for where this code originates.
    /// Note that I've changed disagreements between scaling by 255 or 256 to always use 255.  This means
    /// floats range from 0.0f to 1.0f and bytes from 0 to 255.  I hope.</remarks>
    public struct ColorHSVA
    {
        /// <summary>
        /// h for hue, s for saturation, v for value, a for alpha
        /// </summary>
        public byte h, s, v, a;

        /// <summary>
        /// Create a new <see cref="ColorHSVA"/> from the given values.
        /// </summary>
        /// <param name="h">Hue.</param>
        /// <param name="s">Saturation.</param>
        /// <param name="v">Value.</param>
        /// <param name="a">Alpha.</param>
        public ColorHSVA(byte h, byte s, byte v, byte a) { this.h = h; this.s = s; this.v = v; this.a = a; }
        /// <summary>
        /// Create a new <see cref="ColorHSVA"/> from an existing <see cref="ColorHSVA"/> value.
        /// </summary>
        /// <param name="hsva">An existing <see cref="ColorHSVA"/> value.</param>
        public ColorHSVA(ColorHSVA hsva) : this(hsva.h, hsva.s, hsva.v, hsva.a) { }

        /// <summary>
        /// Convert from ColorHSVA to UInt32 ARGB format.
        /// </summary>
        /// <returns>The equivalent UInt32 ARGB format value.</returns>
        public uint ToARGB()
        {
            float r = 0, g = 0, b = 0, h, s, v;
            h = this.h / 255.0f;
            s = this.s / 255.0f;
            v = this.v / 255.0f;

            if (s == 0) r = g = b = v;//Grey
            else
            {
                float f, p, q, t;
                int i;
                h *= 6; //to bring hue to a number between 0 and 6, better for the calculations
                i = (int)(Math.Floor(h));  //e.g. 2.7 becomes 2 and 3.01 becomes 3 or 4.9999 becomes 4
                f = h - i;  //the fractional part of h
                p = v * (1 - s);
                q = v * (1 - (s * f));
                t = v * (1 - (s * (1 - f)));
                switch (i)
                {
                    case 0: r = v; g = t; b = p; break;
                    case 1: r = q; g = v; b = p; break;
                    case 2: r = p; g = v; b = t; break;
                    case 3: r = p; g = q; b = v; break;
                    case 4: r = t; g = p; b = v; break;
                    case 5: r = v; g = p; b = q; break;
                }
            }

            return (uint)this.a << 24 | (uint)Math.Round(r * 255.0f) << 16| (uint)Math.Round(g * 255.0f) << 8 | (uint)Math.Round(b * 255.0f);
        }

        /// <summary>
        /// Cast from UInt32 ARGB format to ColorHSVA.
        /// </summary>
        /// <param name="argb"></param>
        /// <returns>The equivalent ColorHSVA value.</returns>
        public static explicit operator ColorHSVA(uint argb)
        {
            float r, g, b, h, s, v;

            r = argb.R() / 255.0f;
            g = argb.G() / 255.0f;
            b = argb.B() / 255.0f;

            float maxColor = Math.Max(r, Math.Max(g, b));
            float minColor = Math.Min(r, Math.Min(g, b));
            v = maxColor;

            s = v == 0 ? 0 : (maxColor - minColor) / maxColor;
            if (s == 0) h = 0;
            else
            {
                if (r == maxColor) h = (g - b) / (maxColor - minColor);
                else if (g == maxColor) h = 2.0f + (b - r) / (maxColor - minColor);
                else h = 4.0f + (r - g) / (maxColor - minColor);
                h /= 6.0f; //to bring it to a number between 0 and 1
                if (h < 0) h++;
            }

            return new ColorHSVA
            {
                h = (byte)Math.Round(h * 255.0),
                s = (byte)Math.Round(s * 255.0),
                v = (byte)Math.Round(v * 255.0),
                a = (byte)argb.A(),
            };
        }

        /// <summary>
        /// Determine if <paramref name="obj"/> is a <see cref="ColorHSVA"/> equal
        /// to the current value.
        /// </summary>
        /// <param name="obj">Value to test for equality.</param>
        /// <returns>True if <paramref name="obj"/> is a <see cref="ColorHSVA"/> equal
        /// to the current value; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ColorHSVA)) return false;
            ColorHSVA other = (ColorHSVA)obj;
            return h == other.h && s == other.s && v == other.v && a == other.a;
        }
        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>The hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return h.GetHashCode() ^ s.GetHashCode() ^ v.GetHashCode() ^ a.GetHashCode();
        }

        /// <summary>
        /// Determine whether two <see cref="T:ColorHSVA"/> objects have the same value.
        /// </summary>
        /// <param name="val1">First object.</param>
        /// <param name="val2">Second object.</param>
        /// <returns>True if the Equals method on <paramref name="val1"/> returns true when passed <paramref name="val2"/>.</returns>
        public static bool operator ==(ColorHSVA val1, ColorHSVA val2) { return val1.Equals(val2); }

        /// <summary>
        /// Determine whether two <see cref="T:ColorHSVA"/> objects do not have the same value.
        /// </summary>
        /// <param name="val1">First object.</param>
        /// <param name="val2">Second object.</param>
        /// <returns>True if the Equals method on <paramref name="val1"/> returns false when passed <paramref name="val2"/>.</returns>
        public static bool operator !=(ColorHSVA val1, ColorHSVA val2) { return !val1.Equals(val2); }

        /// <summary>
        /// Return the current <see cref="ColorHSVA"/> adjusted by a given <see cref="HSVShift"/> value.
        /// </summary>
        /// <param name="hsvShift">The <see cref="HSVShift"/> value by which to adjust this <see cref="ColorHSVA"/>.</param>
        /// <returns>The current <see cref="ColorHSVA"/> adjusted by a given <see cref="HSVShift"/> value.</returns>
        public ColorHSVA HSVShift(HSVShift hsvShift)
        {
            float h = this.h / 255.0f + hsvShift.h;
            if (h < 0) h += (float)Math.Floor(Math.Abs(h)) + 1;
            if (h > 1) h -= (float)Math.Floor(Math.Abs(h)) + 1;
            return new ColorHSVA
            {
                h = (byte)Math.Round(h * 255.0f),
                s = (byte)Math.Round(Math.Max(0, Math.Min(1, this.s / 255.0f + hsvShift.s)) * 255.0f),
                v = (byte)Math.Round(Math.Max(0, Math.Min(1, this.v / 255.0f + hsvShift.v)) * 255.0f),
                a = this.a,
            };
        }

        /// <summary>
        /// Creates the HSVA data values corresponding to an array of UInt32 ARGB values.
        /// </summary>
        /// <param name="argbData">An array of UInt32 ARGB values.</param>
        /// <returns>HSVA data values corresponding to <paramref name="argbData"/>.</returns>
        public static ColorHSVA[] ToArrayColorHSVA(uint[] argbData)
        {
            ColorHSVA[] hsvaData = new ColorHSVA[argbData.Length];
            for (int i = 0; i < argbData.Length; i++)
                hsvaData[i] = (ColorHSVA)argbData[i];
            return hsvaData;
        }

        /// <summary>
        /// Creates a Bitmap image corresponding to an HSVA image.
        /// </summary>
        /// <param name="hsvaData">An HSVA image.</param>
        /// <param name="hsvShift">An optional <see cref="HSVShift"/> to apply.</param>
        /// <returns>A Bitmap image corresponding to <paramref name="hsvaData"/>.</returns>
        public static uint[] ToArrayARGB(ColorHSVA[] hsvaData, HSVShift hsvShift = new HSVShift())
        {
            uint[] argbData = new uint[hsvaData.Length];
            for (int i = 0; i < hsvaData.Length; i++)
                argbData[i] = hsvShift.IsEmpty ? hsvaData[i].ToARGB() : hsvaData[i].HSVShift(hsvShift).ToARGB();
            return argbData;
        }
    }

    /// <summary>
    /// Describes a hue, saturation and value shift to be applied to an image
    /// </summary>
    public struct HSVShift
    {
        /// <summary>
        /// Hue, Saturation and Value shift amounts.
        /// </summary>
        public float h, s, v;
        /// <summary>
        /// Returns true if <paramref name="obj"/> is a <see cref="HSVShift"/> with the same value as this instance.
        /// </summary>
        /// <param name="obj">An object to compare.</param>
        /// <returns>True if <paramref name="obj"/> is a <see cref="HSVShift"/> with the same value as this instance; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is HSVShift)) return false;
            HSVShift other = (HSVShift)obj;
            return h == other.h && s == other.s && v == other.v;
        }
        /// <summary>
        /// Returns the hashcode for this instance.
        /// </summary>
        /// <returns>The hashcode for this instance.</returns>
        public override int GetHashCode()
        {
            return h.GetHashCode() ^ s.GetHashCode() ^ v.GetHashCode();
        }

        /// <summary>
        /// Determine whether two <see cref="T:HSVShift"/> objects have the same value.
        /// </summary>
        /// <param name="val1">First object.</param>
        /// <param name="val2">Second object.</param>
        /// <returns>True if the Equals method on <paramref name="val1"/> returns true when passed <paramref name="val2"/>.</returns>
        public static bool operator ==(HSVShift val1, HSVShift val2) { return val1.Equals(val2); }

        /// <summary>
        /// Determine whether two <see cref="T:HSVShift"/> objects do not have the same value.
        /// </summary>
        /// <param name="val1">First object.</param>
        /// <param name="val2">Second object.</param>
        /// <returns>True if the Equals method on <paramref name="val1"/> returns false when passed <paramref name="val2"/>.</returns>
        public static bool operator !=(HSVShift val1, HSVShift val2) { return !val1.Equals(val2); }

        /// <summary>
        /// True if this HSVShift is non-zero.
        /// </summary>
        public bool IsEmpty { get { return h == 0 && s == 0 && v == 0; } }

        static readonly HSVShift _Empty = new HSVShift();
        /// <summary>
        /// An empty HSVShift
        /// </summary>
        public static HSVShift Empty { get { return _Empty; } }
    }
}
