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
using System.Runtime.Serialization;
using System.Text;

namespace s4pi.Interfaces
{
	/// <summary>
	///     A tuple associating a data type (or class) with a value object (of the given type)
	/// </summary>
	[Serializable]
	public class TypedValue
		: IComparable<TypedValue>, IEqualityComparer<TypedValue>, IEquatable<TypedValue>, IConvertible, ICloneable,
		  ISerializable
	{
		/// <summary>
		///     The data type
		/// </summary>
		public readonly Type Type;

		/// <summary>
		///     The value
		/// </summary>
		public readonly object Value;

		private readonly string format = "";

		/// <summary>
		///     Create a new <see cref="TypedValue" />
		/// </summary>
		/// <param name="t">The data type</param>
		/// <param name="v">The value</param>
		public TypedValue(Type t, object v) : this(t, v, "")
		{
		}

		/// <summary>
		///     Create a new <see cref="TypedValue" />
		/// </summary>
		/// <param name="t">The data type</param>
		/// <param name="v">The value</param>
		/// <param name="f">The default format for <see cref="ToString()" /></param>
		public TypedValue(Type t, object v, string f)
		{
			this.Type = t;
			this.Value = v;
			this.format = f;
		}

		/// <summary>
		///     Return a string representing the <see cref="TypedValue" />
		///     <paramref name="tv" />.
		/// </summary>
		/// <param name="tv">The value to convert.</param>
		/// <returns>A string representing <paramref name="tv" />.</returns>
		public static implicit operator string(TypedValue tv)
		{
			return tv.ToString(tv.format);
		}

		/// <summary>
		///     Return the Value as a string using the default format
		/// </summary>
		/// <returns>String representation of Value in default format</returns>
		public override string ToString()
		{
			return this.ToString(this.format);
		}

		/// <summary>
		///     Return the Value as a string using the given format
		/// </summary>
		/// <param name="format">Format to use for result</param>
		/// <returns>String representation of Value in given format</returns>
		public string ToString(string format)
		{
			if (format == "X")
			{
				if (this.Type == typeof(long))
				{
					return "0x" + ((long)this.Value).ToString("X16");
				}
				if (this.Type == typeof(ulong))
				{
					return "0x" + ((ulong)this.Value).ToString("X16");
				}
				if (this.Type == typeof(int))
				{
					return "0x" + ((int)this.Value).ToString("X8");
				}
				if (this.Type == typeof(uint))
				{
					return "0x" + ((uint)this.Value).ToString("X8");
				}
				if (this.Type == typeof(short))
				{
					return "0x" + ((short)this.Value).ToString("X4");
				}
				if (this.Type == typeof(ushort))
				{
					return "0x" + ((ushort)this.Value).ToString("X4");
				}
				if (this.Type == typeof(sbyte))
				{
					return "0x" + ((sbyte)this.Value).ToString("X2");
				}
				if (this.Type == typeof(byte))
				{
					return "0x" + ((byte)this.Value).ToString("X2");
				}

				//well, no... but...
				if (this.Type == typeof(double))
				{
					return ((double)this.Value).ToString("F4");
				}
				if (this.Type == typeof(float))
				{
					return ((float)this.Value).ToString("F4");
				}

				if (typeof(Enum).IsAssignableFrom(this.Type))
				{
					var etv = new TypedValue(Enum.GetUnderlyingType(this.Type), this.Value, "X");
					return string.Format("{0} ({1})", "" + etv, "" + this.Value);
				}
			}

			if (typeof(string).IsAssignableFrom(this.Type) || typeof(char[]).IsAssignableFrom(this.Type))
			{
				var s = typeof(string).IsAssignableFrom(this.Type)
                    ? (string)this.Value
                    : new string((char[])this.Value);
				// -- It is not necessarily correct that a zero byte indicates a unicode string; these should have been
				// correctly read in already so no translation should be needed... so the ToANSIString is currently commented out
				if (s.IndexOf((char)0) != -1)
				{
					return TypedValue.ToDisplayString(s.ToCharArray());
				}
				return s.Normalize();
			}

			if (this.Type.HasElementType) // it's an array
			{
				if (typeof(AApiVersionedFields).IsAssignableFrom(this.Type.GetElementType()))
				{
					return TypedValue.FromAApiVersionedFieldsArray(this.Type.GetElementType(), (Array)this.Value);
				}
				return TypedValue.FromSimpleArray(this.Type.GetElementType(), (Array)this.Value);
			}

			return this.Value.ToString();
		}

		#region ToString helpers

		private static string ToANSIString(string unicode)
		{
			var t = new StringBuilder();
			for (var i = 0; i < unicode.Length; i += 2)
			{
				t.Append((char)((unicode[i] << 8) + unicode[i + 1]));
			}
			return t.ToString().Normalize();
		}

		private static string FromSimpleArray(Type type, Array ary)
		{
			var sb = new StringBuilder();
			for (var i = 0; i < ary.Length; i++)
			{
				var v = ary.GetValue(i);
				var tv = new TypedValue(v.GetType(), v, "X");
				sb.Append(string.Format(" [{0:X}:'{1}']", i, "" + tv));
				if (i % 16 == 15)
				{
					sb.Append("\n");
				}
			}
			return sb.ToString().TrimStart().TrimEnd('\n');
		}

		private static string FromAApiVersionedFieldsArray(Type type, Array ary)
		{
			var sb = new StringBuilder();
			var fmt = "[{0:X}" + (type.IsAbstract ? " {1}" : "") + "]: {2}\n";
			for (var i = 0; i < ary.Length; i++)
			{
				var value = (AApiVersionedFields)ary.GetValue(i);
				if (value.ContentFields.Contains("Value"))
				{
					sb.Append(string.Format(fmt, i, value.GetType(), value["Value"]));
				}
				else
				{
					sb.Append(string.Format(fmt, i, value.GetType(), value));
				}
			}
			return sb.ToString().Trim('\n');
		}

		private static readonly string[] LowNames =
		{
			"NUL",
			"SOH",
			"STX",
			"ETX",
			"EOT",
			"ENQ",
			"ACK",
			"BEL",
			"BS",
			"HT",
			/*"LF",**/ "VT",
			"FF",
			"CR",
			"SO",
			"SI",
			"DLE",
			"DC1",
			"DC2",
			"DC3",
			"DC4",
			"NAK",
			"SYN",
			"ETB",
			"CAN",
			"EM",
			"SUB",
			"ESC",
			"FS",
			"GS",
			"RS",
			"US"
		};

		private static string ToDisplayString(char[] text)
		{
			var t = new StringBuilder();
			foreach (var c in text)
			{
				if (c < 32 && c != '\n')
				{
					t.Append(string.Format("<{0}>", TypedValue.LowNames[c]));
				}
				else if (c == 127)
				{
					t.Append("<DEL>");
				}
				else if (c > 127)
				{
					t.Append(string.Format("<U+{0:X4}>", (int)c));
				}
				else
				{
					t.Append(c);
				}
			}
			return t.ToString().Normalize();
		}

		#endregion

		#region IComparable<TypedValue> Members

		/// <summary>
		///     Compare this <see cref="TypedValue" /> to another for sort order purposes
		/// </summary>
		/// <param name="other">Target <see cref="TypedValue" /></param>
		/// <returns>
		///     A 32-bit signed integer that indicates the relative order of the objects being compared.  The return value has
		///     these meanings:
		///     <table>
		///         <thead>
		///             <tr>
		///                 <td>
		///                     <strong>Value</strong>
		///                 </td>
		///                 <td>
		///                     <strong>Meaning</strong>
		///                 </td>
		///             </tr>
		///         </thead>
		///         <tbody>
		///             <tr>
		///                 <td>Less than zero</td><td>This instance is less than <paramref name="other" />.</td>
		///             </tr>
		///             <tr>
		///                 <td>Zero</td><td>This instance is equal to <paramref name="other" />.</td>
		///             </tr>
		///             <tr>
		///                 <td>Greater than zero</td><td>This instance is greater than <paramref name="other" />.</td>
		///             </tr>
		///         </tbody>
		///     </table>
		/// </returns>
		/// <exception cref="NotImplementedException">Either this object's Type or the target's is not comparable</exception>
		/// <exception cref="ArgumentException">The target is not comparable with this object</exception>
		public int CompareTo(TypedValue other)
		{
			if (!this.Type.IsAssignableFrom(other.Type) || !(this.Type is IComparable) || !(other.Type is IComparable))
			{
				throw new NotImplementedException();
			}
			return ((IComparable)this.Value).CompareTo((IComparable)other.Value);
		}

		#endregion

		#region IEqualityComparer<TypedValue> Members

		/// <summary>
		///     Determines whether the specified <see cref="TypedValue" /> instances are equal.
		/// </summary>
		/// <param name="x">The first <see cref="TypedValue" /> to compare.</param>
		/// <param name="y">The second <see cref="TypedValue" /> to compare.</param>
		/// <returns>true if the specified <see cref="TypedValue" /> instances are equal; otherwise, false.</returns>
		public bool Equals(TypedValue x, TypedValue y)
		{
			return x.Equals(y);
		}

		/// <summary>
		///     Returns a hash code for the specified <see cref="TypedValue" />.
		/// </summary>
		/// <param name="obj">The <see cref="TypedValue" /> for which a hash code is to be returned.</param>
		/// <returns>A hash code for the specified object.</returns>
		/// <exception cref="ArgumentNullException">
		///     The type of <paramref name="obj" /> is a reference type and
		///     <paramref name="obj" /> is null.
		/// </exception>
		public int GetHashCode(TypedValue obj)
		{
			return obj.GetHashCode();
		}

		#endregion

		#region IEquatable<TypedValue> Members

		/// <summary>
		///     Indicates whether the current <see cref="TypedValue" /> instance is equal to another <see cref="TypedValue" />
		///     instance.
		/// </summary>
		/// <param name="other">An <see cref="TypedValue" /> instance to compare with this instance.</param>
		/// <returns>true if the current instance is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		public bool Equals(TypedValue other)
		{
			return this.Value.Equals(other.Value);
		}

		/// <summary>
		///     Determines whether the specified <see cref="System.Object" /> is equal to the current <see cref="TypedValue" />.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with the current <see cref="TypedValue" />.</param>
		/// <returns>
		///     true if the specified <see cref="System.Object" /> is equal to the current <see cref="TypedValue" />;
		///     otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			return obj as TypedValue != null ? this.Equals((TypedValue)obj) : false;
		}

		/// <summary>
		///     Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
		public override int GetHashCode()
		{
			return this.Value.GetHashCode();
		}

		#endregion

		#region IConvertible Members

		/// <summary>
		///     Returns the <see cref="TypeCode" /> for this instance.
		/// </summary>
		/// <returns>The enumerated constant that is the <see cref="TypeCode" /> of the <see cref="TypedValue" /> class.</returns>
		public TypeCode GetTypeCode()
		{
			return TypeCode.String;
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="Boolean" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="Boolean" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="Boolean" />.
		/// </exception>
		public bool ToBoolean(IFormatProvider provider)
		{
			if (typeof(bool).IsAssignableFrom(this.Type))
			{
				return (bool)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="Byte" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="Byte" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="Byte" />.
		/// </exception>
		public byte ToByte(IFormatProvider provider)
		{
			if (typeof(byte).IsAssignableFrom(this.Type))
			{
				return (byte)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="Char" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="Char" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="Char" />.
		/// </exception>
		public char ToChar(IFormatProvider provider)
		{
			if (typeof(char).IsAssignableFrom(this.Type))
			{
				return (char)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="DateTime" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="DateTime" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="DateTime" />.
		/// </exception>
		public DateTime ToDateTime(IFormatProvider provider)
		{
			if (typeof(DateTime).IsAssignableFrom(this.Type))
			{
				return (DateTime)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="Decimal" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="Decimal" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="Decimal" />.
		/// </exception>
		public decimal ToDecimal(IFormatProvider provider)
		{
			if (typeof(decimal).IsAssignableFrom(this.Type))
			{
				return (decimal)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="Double" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="Double" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="Double" />.
		/// </exception>
		public double ToDouble(IFormatProvider provider)
		{
			if (typeof(double).IsAssignableFrom(this.Type))
			{
				return (double)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="Int16" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="Int16" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="Int16" />.
		/// </exception>
		public short ToInt16(IFormatProvider provider)
		{
			if (typeof(short).IsAssignableFrom(this.Type))
			{
				return (short)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="Int32" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="Int32" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="Int32" />.
		/// </exception>
		public int ToInt32(IFormatProvider provider)
		{
			if (typeof(int).IsAssignableFrom(this.Type))
			{
				return (int)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="Int64" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="Int64" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="Int64" />.
		/// </exception>
		public long ToInt64(IFormatProvider provider)
		{
			if (typeof(long).IsAssignableFrom(this.Type))
			{
				return (long)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="SByte" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="SByte" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="SByte" />.
		/// </exception>
		public sbyte ToSByte(IFormatProvider provider)
		{
			if (typeof(sbyte).IsAssignableFrom(this.Type))
			{
				return (sbyte)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="Single" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="Single" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="Single" />.
		/// </exception>
		public float ToSingle(IFormatProvider provider)
		{
			if (typeof(float).IsAssignableFrom(this.Type))
			{
				return (float)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="String" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="String" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="String" />.
		/// </exception>
		public string ToString(IFormatProvider provider)
		{
			if (typeof(string).IsAssignableFrom(this.Type))
			{
				return (string)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an <see cref="Object" /> of the specified <see cref="Type" />
		///     that has an equivalent value, using the specified culture-specific formatting information.
		/// </summary>
		/// <param name="conversionType">The <see cref="Type" /> to which the value of this instance is converted.</param>
		/// <param name="provider">
		///     An <see cref="IFormatProvider" /> interface implementation that supplies culture-specific
		///     formatting information.
		/// </param>
		/// <returns>
		///     An <see cref="Object" /> instance of type <paramref name="conversionType" /> whose value is equivalent to the
		///     value of this instance.
		/// </returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <paramref name="conversionType" /> object.
		/// </exception>
		public object ToType(Type conversionType, IFormatProvider provider)
		{
			if (conversionType.IsAssignableFrom(this.Type))
			{
				return Convert.ChangeType(this.Value, conversionType, provider);
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="UInt16" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="UInt16" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="UInt16" />.
		/// </exception>
		public ushort ToUInt16(IFormatProvider provider)
		{
			if (typeof(ushort).IsAssignableFrom(this.Type))
			{
				return (ushort)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="UInt32" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="UInt32" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="UInt32" />.
		/// </exception>
		public uint ToUInt32(IFormatProvider provider)
		{
			if (typeof(uint).IsAssignableFrom(this.Type))
			{
				return (uint)this.Value;
			}
			throw new NotImplementedException();
		}

		/// <summary>
		///     Converts the value of this instance to an equivalent <see cref="UInt64" /> value
		///     (ignoring the specified culture-specific formatting information).
		/// </summary>
		/// <param name="provider">
		///     (unused, may be null) An <see cref="IFormatProvider" /> interface implementation
		///     that supplies culture-specific formatting information.
		/// </param>
		/// <returns>A <see cref="UInt64" /> value equivalent to the value of this instance.</returns>
		/// <exception cref="NotImplementedException">
		///     Thrown if the <see cref="TypedValue" /> value
		///     cannot be assigned to a <see cref="UInt64" />.
		/// </exception>
		public ulong ToUInt64(IFormatProvider provider)
		{
			if (typeof(ulong).IsAssignableFrom(this.Type))
			{
				return (ulong)this.Value;
			}
			throw new NotImplementedException();
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		///     Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>A new object that is a copy of this instance.</returns>
		/// <exception cref="NotImplementedException">Thrown if the value cannot be cloned.</exception>
		public object Clone()
		{
			if (typeof(ICloneable).IsAssignableFrom(this.Type))
			{
				return new TypedValue(this.Type, ((ICloneable)this.Value).Clone(), this.format);
			}
			throw new NotImplementedException();
		}

		#endregion

		#region ISerializable Members

		/// <summary>
		///     Populates a <see cref="SerializationInfo" /> with the data needed to serialize the target object.
		/// </summary>
		/// <param name="info">The System.Runtime.Serialization.SerializationInfo to populate with data.</param>
		/// <param name="context">The destination (see System.Runtime.Serialization.StreamingContext) for this serialization.</param>
		/// <exception cref="System.Security.SecurityException">The caller does not have the required permission.</exception>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Type", this.Type, typeof(Type));
			info.AddValue("Value", this.Value, this.Type);
			info.AddValue("format", this.format);
		}

		#endregion
	}
}
