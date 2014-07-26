using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using s4pi.GenericRCOLResource;

namespace s3piwrappers.ModelViewer
{
    public class App
    {
        [STAThread]
        private static void Main(params String[] args)
        {
            GenericRCOLResource rcol = null;
            using (Stream s = File.OpenRead(args[0]))
            {
                rcol = new GenericRCOLResource(0, s);
            }
            var app = new Application();

//            try
//            {
//                var self = Process.GetCurrentProcess();
//                var caller = self.GetParent();
//                if (caller != null)
//                {
//                    caller.EnableRaisingEvents = true;
//                    caller.Exited += (sender, eventArgs) => self.CloseMainWindow(); ;
//                }
//            }
//            catch
//            {
//                MessageBox.Show(
//                    "Unable to locate parent process.  If this was started through another program, you will have to close it manually.");
//            }
            var win = new MainWindow(rcol);
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-texture":
                        win.TextureSource = args[++i];
                        break;
                    case "-title":
                        win.Title += " - " + args[++i];
                        break;
                    default:
                        break;
                }
            }
            app.Run(win);
        }
    }
}
