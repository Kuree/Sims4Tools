/***************************************************************************
 *  Copyright (C) 2013 by Peter L Jones                                    *
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
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace VP6VIDExpImp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            List<string> largs = new List<string>(args);

            bool export = largs.Contains("/export");
            if (export) largs.Remove("/export");

            bool import = largs.Contains("/import");
            if (import) largs.Remove("/import");

            if ((export && import) || (!export && !import))
            {
                CopyableMessageBox.Show("Missing /export or /import on command line.", Application.ProductName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                Environment.Exit(1);
            }

            args = largs.ToArray();

            Filename = (args.Length > 0 ? Path.GetFileNameWithoutExtension(args[args.Length - 1]) : "*");

            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            //return s3pi.Helpers.RunHelper.Run(export ? typeof(ExportForm) : typeof(ImportForm), args);
            return 0;
        }

        public static string Filename { get; private set; }
    }
}
