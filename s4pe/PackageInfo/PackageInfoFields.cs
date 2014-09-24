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
using System.ComponentModel;
using s4pi.Interfaces;

namespace S4PIDemoFE.PackageInfo
{
    public partial class PackageInfoFields : Component
    {
        public PackageInfoFields() { }

        static List<string> fields = null;
        static PackageInfoFields()
        {
            fields = new List<string>();
            foreach (string s in AApiVersionedFields.GetContentFields(0, typeof(APackage)))
                if (!s.Contains("Stream") && !s.Contains("List"))
                    fields.Add(s);
        }

        [Browsable(true)]
        [Description("The list of known fields on a Package object")]
        public IList<string> Fields
        {
            get { return fields; }
            //set { }
        }
    }
}
