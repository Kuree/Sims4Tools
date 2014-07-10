/***************************************************************************
 *  Copyright (C) 2010 by Peter L Jones                                    *
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

namespace S3SA_DLL_ExpImp
{
    static class Program
    {
        static bool view_go = false;

        [Flags]
        enum Option
        {
            export = 1,
            import = 2,
            view = 4,
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            List<string> largs = new List<string>(args);

            Option option = 0;
            if (largs.Contains("/export"))
            {
                largs.Remove("/export");
                option |= Option.export;
            }
            if (largs.Contains("/import"))
            {
                largs.Remove("/import");
                option |= Option.import;
            }
            if (largs.Contains("/view"))
            {
                largs.Remove("/view");
                option |= Option.view;

                if (largs.Contains("/go"))
                {
                    largs.Remove("/go");
                    view_go = true;
                }
                else
                {
                    view_go = false;
                }
            }

            if (!Enum.IsDefined(typeof(Option), option))
            {
                CopyableMessageBox.Show("Invalid command line.", Application.ProductName, CopyableMessageBoxButtons.OK, CopyableMessageBoxIcon.Error);
                Environment.Exit(1);
            }

            args = largs.ToArray();

#if DEBUG
            if (args.Length == 0)
            {
                FileStream fs = new FileStream(Application.ExecutablePath, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                Clipboard.SetData(DataFormats.Serializable, new ScriptResource.ScriptResource(0, null) { Assembly = br }.Stream);
                br.Close();
                fs.Close();
            }
#endif

            Type mainForm = typeof(object);
            switch (option)
            {
                case Option.export: mainForm = typeof(Export); break;
                case Option.import: mainForm = typeof(Import); break;
                case Option.view: mainForm = typeof(View); break;
            }
            return s3pi.Helpers.RunHelper.Run(mainForm, args);
        }

        public static string getAssemblyName(ScriptResource.ScriptResource s3sa)
        {
            try
            {
                byte[] data = new byte[s3sa.Assembly.BaseStream.Length];
                s3sa.Assembly.BaseStream.Read(data, 0, data.Length);
                Assembly assy = Assembly.Load(data);

                return assy.FullName.Split(',')[0] + ".dll";
            }
            catch
            {
            }
            return "*.dll";
        }

        public static bool ViewGo { get { return view_go; } }
    }
}
