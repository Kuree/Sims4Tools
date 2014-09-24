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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace System
{
    /// <summary>
    /// Useful Extension Methods not provided by Linq (and without deferred execution).
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Convert all elements of an <c>Array</c> to <typeparamref name="TOut"/>.
        /// </summary>
        /// <typeparam name="TOut">The output element type.</typeparam>
        /// <param name="array">The input array</param>
        /// <returns>An <c>TOut[]</c> array containing converted input elements.</returns>
        /// <exception cref="InvalidCastException">The element type of <paramref name="array"/> does not provide the <c>IConvertible</c> interface.</exception>
        public static TOut[] ConvertAll<TOut>(this Array array) where TOut : IConvertible { return array.ConvertAll<TOut>(0, array.Length, CultureInfo.CurrentCulture); }
        /// <summary>
        /// Convert all elements of an <c>Array</c> to <typeparamref name="TOut"/>.
        /// </summary>
        /// <typeparam name="TOut">The output element type.</typeparam>
        /// <param name="array">The input array</param>
        /// <param name="provider">An <c>System.IFormatProvider</c> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An <c>TOut[]</c> array containing converted input elements.</returns>
        /// <exception cref="InvalidCastException">The element type of <paramref name="array"/> does not provide the <c>IConvertible</c> interface.</exception>
        public static TOut[] ConvertAll<TOut>(this Array array, IFormatProvider provider) where TOut : IConvertible { return array.ConvertAll<TOut>(0, array.Length, provider); }
        /// <summary>
        /// Convert elements of an <c>Array</c> to <typeparamref name="TOut"/>,
        /// starting at <paramref name="start"/>.
        /// </summary>
        /// <typeparam name="TOut">The output element type.</typeparam>
        /// <param name="array">The input array</param>
        /// <param name="start">The offset into <paramref name="array"/> from which to start creating the output.</param>
        /// <returns>An <c>TOut[]</c> array containing converted input elements.</returns>
        /// <exception cref="InvalidCastException">The element type of <paramref name="array"/> does not provide the <c>IConvertible</c> interface.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="start"/> is outside the bounds of <paramref name="array"/>.</exception>
        public static TOut[] ConvertAll<TOut>(this Array array, int start) where TOut : IConvertible { return array.ConvertAll<TOut>(start, array.Length - start, CultureInfo.CurrentCulture); }
        /// <summary>
        /// Convert elements of an <c>Array</c> to <typeparamref name="TOut"/>,
        /// starting at <paramref name="start"/>.
        /// </summary>
        /// <typeparam name="TOut">The output element type.</typeparam>
        /// <param name="array">The input array</param>
        /// <param name="start">The offset into <paramref name="array"/> from which to start creating the output.</param>
        /// <param name="provider">An <c>System.IFormatProvider</c> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An <c>TOut[]</c> array containing converted input elements.</returns>
        /// <exception cref="InvalidCastException">The element type of <paramref name="array"/> does not provide the <c>IConvertible</c> interface.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="start"/> is outside the bounds of <paramref name="array"/>.</exception>
        public static TOut[] ConvertAll<TOut>(this Array array, int start, IFormatProvider provider) where TOut : IConvertible { return array.ConvertAll<TOut>(start, array.Length - start, provider); }
        /// <summary>
        /// Convert elements of an <c>Array</c> to <typeparamref name="TOut"/>,
        /// starting at <paramref name="start"/> for <paramref name="length"/> elements.
        /// </summary>
        /// <typeparam name="TOut">The output element type.</typeparam>
        /// <param name="array">The input array</param>
        /// <param name="start">The offset into <paramref name="array"/> from which to start creating the output.</param>
        /// <param name="length">The number of elements in the output.</param>
        /// <returns>An <c>TOut[]</c> array containing converted input elements.</returns>
        /// <exception cref="InvalidCastException">The element type of <paramref name="array"/> does not provide the <c>IConvertible</c> interface.
        /// <br/>-or-<br/>
        /// this conversion is not supported.
        /// <br/>-or-<br/>
        /// an <paramref name="array"/> element is null and <typeparamref name="TOut"/> is a value type.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="start"/> is outside the bounds of <paramref name="array"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="length"/> has an invalid value.</exception>
        public static TOut[] ConvertAll<TOut>(this Array array, int start, int length) where TOut : IConvertible { return array.ConvertAll<TOut>(start, length, CultureInfo.CurrentCulture); }
        /// <summary>
        /// Convert elements of an <c>Array</c> to <typeparamref name="TOut"/>,
        /// starting at <paramref name="start"/> for <paramref name="length"/> elements.
        /// </summary>
        /// <typeparam name="TOut">The output element type.</typeparam>
        /// <param name="array">The input array</param>
        /// <param name="start">The offset into <paramref name="array"/> from which to start creating the output.</param>
        /// <param name="length">The number of elements in the output.</param>
        /// <param name="provider">An <c>System.IFormatProvider</c> interface implementation that supplies culture-specific formatting information.</param>
        /// <returns>An <c>TOut[]</c> array containing converted input elements.</returns>
        /// <exception cref="InvalidCastException">The element type of <paramref name="array"/> does not provide the <c>IConvertible</c> interface.
        /// <br/>-or-<br/>
        /// this conversion is not supported.
        /// <br/>-or-<br/>
        /// an <paramref name="array"/> element is null and <typeparamref name="TOut"/> is a value type.
        /// </exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="start"/> is outside the bounds of <paramref name="array"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="length"/> has an invalid value.</exception>
        public static TOut[] ConvertAll<TOut>(this Array array, int start, int length, IFormatProvider provider) where TOut : IConvertible
        {
            if (!typeof(IConvertible).IsAssignableFrom(array.GetType().GetElementType()))
                throw new InvalidCastException(array.GetType().GetElementType().Name + " is not IConvertible");

            if (start > array.Length)
                throw new IndexOutOfRangeException("'start' exceeds array length");

            if (length > array.Length - start)
                throw new ArgumentException("'length' exceeds constrained element count");

            TOut[] res = new TOut[length];

            for (int i = 0; i < res.Length; i++)
                res[i] = (TOut)System.Convert.ChangeType(((IList)array)[i + start], typeof(TOut), provider);

            return res;
        }


        /// <summary>
        /// Compares this instance to a specified list of type <typeparamref name="T"/>
        /// and returns an indication of their relative values.
        /// </summary>
        /// <typeparam name="T">A type supporting <c>IComparable{T}.</c></typeparam>
        /// <param name="value">This instance.</param>
        /// <param name="target">A list to compare.</param>
        /// <returns>An indication of the relative value of this instance and the specified list.</returns>
        public static int CompareTo<T>(this IList<T> value, IList<T> target) where T : IComparable<T>
        {
            if (value == null)//should never happen!
                return (target != null) ? -1 : 0;
            if (target == null) return 1;

            int lim = Math.Min(value.Count, target.Count);
            int cmp;
            for (int i = 0; i < lim; i++) { cmp = value[i].CompareTo(target[i]); if (cmp != 0) return cmp; }
            return value.Count.CompareTo(target.Count);
        }
        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified list of <typeparamref name="T"/> values.
        /// </summary>
        /// <typeparam name="T">A type supporting <see cref="IEquatable{T}"/>.</typeparam>
        /// <param name="value">This instance.</param>
        /// <param name="target">A list to compare.</param>
        /// <returns>And indication of the equality of the values of this instance and the specified list.</returns>
        public static bool Equals<T>(this IList<T> value, IList<T> target) where T : IEquatable<T>
        {
            if (value == null)//should never happen!
                return (target == null);
            if (target == null) return false;

            if (value.Count != target.Count) return false;
            for (int i = 0; i < value.Count; i++) if (!value[i].Equals(target[i])) return false;
            return true;
        }
    }
}
