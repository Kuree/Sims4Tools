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

namespace s4pi.Interfaces
{
    /// <summary>
    /// Indicates that the element should be expandable rather than requiring a popup
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class TGIBlockListContentFieldAttribute : Attribute
    {
        string tgiBlockListContentField;
        /// <summary>
        /// Attaches a TGIBlockList to an Index ContentField.
        /// </summary>
        /// <param name="value">The TGIBlockList to which this ContentField Property applies.</param>
        public TGIBlockListContentFieldAttribute(string value) { tgiBlockListContentField = value; }
        /// <summary>
        /// The TGIBlockList to which this ContentField Property applies.
        /// </summary>
        public string TGIBlockListContentField { get { return tgiBlockListContentField; } set { tgiBlockListContentField = value; } }

        /// <summary>
        /// Return the TGIBlockListContentField value for a Content Field.
        /// </summary>
        /// <param name="t">Type on which Content Field exists.</param>
        /// <param name="index">Content Field name.</param>
        /// <returns>The value of the TGIBlockListContentFieldAttribute TGIBlockListContentField field, if found;
        /// otherwise <c>null</c>.</returns>
        public static string GetTGIBlockListContentField(Type t, string index)
        {
            System.Reflection.PropertyInfo pi = t.GetProperty(index);
            if (pi != null)
                foreach (Attribute attr in pi.GetCustomAttributes(typeof(TGIBlockListContentFieldAttribute), true))
                    return (attr as TGIBlockListContentFieldAttribute).TGIBlockListContentField;
            return null;
        }
    }
}
