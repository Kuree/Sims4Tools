using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using s4pi.Helpers;

namespace RLEDDSHelper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
           if(args.Contains("import"))
           {
               RunHelper.Run(typeof(Import), args);
           }
           else if(args.Contains("export"))
           {
               RunHelper.Run(typeof(Export), args);
           }
        }
    }
}
