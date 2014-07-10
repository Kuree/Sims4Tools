using System;
using System.IO;
using System.Windows.Forms;

namespace CreateAssemblyVersion
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            /*
             * The purpose here is to replace the following LOCALE-specific BAT file with
             * a locale-independent method of creating AssembleyVersion.cs
             */
            if (args.Length != 1)
            {
                Usage("Invalid arguments: '{0}'", String.Join(", ", args));
                return 1;
            }

            if (new System.Collections.Generic.List<char>(Path.GetInvalidPathChars()).FindAll(x => args[0].Contains(x.ToString())).Count > 0)
            {
                Usage("'{0}' contains invalid characters.", args[0]);
                return 2;
            }

            DateTime now = DateTime.UtcNow;
            string assemblyVersion = String.Format(
                "[assembly: AssemblyVersion(\"{0:D2}{1:D2}.{2:D2}.{3:D2}{4:D2}.*\")]",
                now.Year % 100, now.Month,
                now.Day,
                now.Hour, now.Minute);

            string path = Path.Combine(args[0], "Properties/AssemblyVersion.cs");
            FileStream s = new FileStream(path, FileMode.Create, FileAccess.Write);
            StreamWriter w = new StreamWriter(s);

            w.WriteLine("using System.Reflection;");
            w.WriteLine("using System.Runtime.CompilerServices;");
            w.WriteLine("using System.Runtime.InteropServices;");
            w.WriteLine(assemblyVersion);

            w.Close();
            return 0;
        }

        static void Usage(string format, params object[] args)
        {
            Console.Error.WriteLine(format, args);
            MessageBox.Show(String.Format(format, args), "Usage", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
