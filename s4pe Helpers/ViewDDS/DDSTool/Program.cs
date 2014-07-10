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
using System.IO;
using System.Windows.Forms;

namespace s3pe.DDSTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {

            if (args.Length < 0 || args.Length > 1)
            {
                MessageBox.Show("Pass only zero or one file arguments",
                    "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return 1;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 0)
                Application.Run(new MainForm());
            else
            {
                if (!File.Exists(args[0]))
                {
                    MessageBox.Show("File not found:\n" + args[0],
                        "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return 2;
                }

                Application.Run(new MainForm(args[0], args[0].ToLower().EndsWith(".dds")));
            }

            return 0;
        }
    }
}
