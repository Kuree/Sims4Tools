using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using s4pi.Helpers;
using System.IO;
using s4pi.ImageResource;

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
           if(args.Contains("/import"))
           {
               List<string> largs = new List<string>(args);
               largs.Remove("/import");
               s4pi.Helpers.RunHelper.Run(typeof(Import), largs.ToArray());
           }
           else if(args.Contains("/export"))
           {
               using (FileStream fs = new FileStream(args[1], FileMode.Open))
               {
                   using (SaveFileDialog save = new SaveFileDialog() { Filter = "DDS DXT5|*.dds" })
                   {
                       if (save.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                       {
                           using (FileStream fs2 = new FileStream(save.FileName, FileMode.Create))
                           {
                               RLEResource r = new RLEResource(1, fs);
                               r.ToDDS().CopyTo(fs2);
                           }
                       }
                   }
               }
           }
        }
    }
}
