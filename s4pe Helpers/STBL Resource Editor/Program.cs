using System;
using System.Collections.Generic;
using System.Windows.Forms;
using s4pi;

namespace s3pi_STBL_Resource_Editor
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
                Clipboard.SetData(DataFormats.Serializable, new StblResource.StblResource(0, null).Stream);
#endif
            return s4pi.Helpers.RunHelper.Run(typeof(MainForm), args);
        }
    }
}
