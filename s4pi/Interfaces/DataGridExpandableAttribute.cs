/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
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

namespace s3pi.Interfaces
{
    /// <summary>
    /// Indicates that the element should be expandable rather than requiring a popup
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    [Obsolete]
    public class DataGridExpandableAttribute : Attribute
    {
        bool dataGridExpandable;
        /// <summary>
        /// Indicates that the element should be expandable rather than requiring a popup
        /// </summary>
        public DataGridExpandableAttribute() { dataGridExpandable = true; }
        /// <summary>
        /// If true, indicates that the element should be expandable rather than requiring a popup
        /// </summary>
        /// <param name="value">True to indicate the element should be expandable</param>
        public DataGridExpandableAttribute(bool value) { dataGridExpandable = value; }
        /// <summary>
        /// Indicate whether the element should be expandable (true) or not (false)
        /// </summary>
        public bool DataGridExpandable { get { return dataGridExpandable; } set { dataGridExpandable = value; } }

        /// <summary>
        /// Return the DataGridExpandable value for a Content Field.
        /// </summary>
        /// <param name="t">Type on which Content Field exists.</param>
        /// <param name="index">Content Field name.</param>
        /// <returns>The value of the DataGridExpandableAttribute DataGridExpandable field, if found;
        /// otherwise <c>false</c>.</returns>
        public static bool GetDataGridExpandable(Type t, string index)
        {
            System.Reflection.PropertyInfo pi = t.GetProperty(index);

            if (pi != null)
                foreach (var attr in pi.GetCustomAttributes(typeof(DataGridExpandableAttribute), true))
                    return (attr as DataGridExpandableAttribute).DataGridExpandable;

            return false;
        }
    }
}
