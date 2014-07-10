/***************************************************************************
 *  Copyright (C) 2011 by Peter L Jones                                    *
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

namespace CLIPexportAsNewName
{
    static class Program
    {
        static string name = "";
        static bool isDefault = false;
        static uint group = 0;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            if (args.Length == 1)
            {
                string[] split = System.IO.Path.GetFileNameWithoutExtension(args[0]).Split(new char[] { '_' }, 5);
                // S3_{T}_{G}_{I}_{name%%+tag}
                if (split.Length == 5)
                {
                    group = uint.Parse(split[2], System.Globalization.NumberStyles.HexNumber);
                    isDefault = split[3][0] < '8';
                    name = split[4].Split('%')[0];
                }
            }

#if DEBUG
            if (args.Length == 0)
            {
                name = "DEBUG";
                Clipboard.SetData(DataFormats.Serializable, new System.IO.MemoryStream(new byte[1024]));
            }
#endif

            return s3pi.Helpers.RunHelper.Run(typeof(MainForm), args);
        }

        public static uint CurrentGroup { get { return group; } }
        public static string CurrentName { get { return name; } }
        public static bool IsDefault { get { return isDefault; } }
    }
}
