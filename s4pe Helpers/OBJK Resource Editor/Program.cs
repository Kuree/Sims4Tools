using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace s3pe_OBJK_Resource_Editor
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
                Clipboard.SetData(DataFormats.Serializable, new ObjKeyResource.ObjKeyResource(0, null).Stream);
#endif
            return s3pi.Helpers.RunHelper.Run(typeof(MainForm), args);
        }
    }
}
