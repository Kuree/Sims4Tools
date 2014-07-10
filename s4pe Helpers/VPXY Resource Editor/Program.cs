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
using System.Windows.Forms;
using s3pi.Interfaces;
using s3pi.GenericRCOLResource;

namespace s3pe_VPXY_Resource_Editor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
#if DEBUG
            if (args.Length == 0)
            {
                TGIBlock tgib = new TGIBlock(0, null, "ITG", 0x736884F1, 0, 0);
                ARCOLBlock rcol = GenericRCOLResourceHandler.CreateRCOLBlock(0, null, 0x736884F1);
                GenericRCOLResource.ChunkEntry ce = new GenericRCOLResource.ChunkEntry(0, null, tgib, rcol);
                GenericRCOLResource grr = new GenericRCOLResource(0, null);
                grr.ChunkEntries.Add(ce);
                Clipboard.SetData(DataFormats.Serializable, grr.Stream);
            }
#endif
            return s3pi.Helpers.RunHelper.Run(typeof(MainForm), args);
        }
    }
}
