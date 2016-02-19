/***************************************************************************
 *  Copyright (C) 2009, 2016 by the Sims 4 Tools development team          *
 *                                                                         *
 *  Contributors:                                                          *
 *  Inge Jones                                                             *
 *  Buzzler                                                                *
 *                                                                         *
 *  This file is part of the Sims 4 Package Interface (s4pi)               *
 *                                                                         *
 *  s4pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s4pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s4pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/

namespace CatalogResource
{
    using System.Collections.Generic;
    using System.IO;
    using s4pi.Interfaces;

    public class COBJResource : AbstractCatalogResource
	{
		public COBJResource(int APIversion, Stream s)
			: base(APIversion, s)
		{
		}
	}

	public class COBJResourceHandler : AResourceHandler
	{
		public COBJResourceHandler()
		{
			this.Add(typeof(COBJResource), new List<string>(new[] { "0x319E4F1D" }));
		}
	}
}
