using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ViewDDS
{
    static class Program
    {
        static void IssueException(Exception ex)
        {
            string s = "";
            for (Exception inex = ex; inex != null; inex = ex.InnerException) s += "\n" + inex.Message;
            s += "\n-----";
            for (Exception inex = ex; inex != null; inex = ex.InnerException) s += "\n" + inex.StackTrace;
            MessageBox.Show(s, "Program Exception", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(params string[] args)
        {
            bool useClipboard = false;
            bool useFile = false;
            List<string> files = new List<string>();

            foreach (string s in args)
            {
                string p = s;
                if (p.StartsWith("/") || p.StartsWith("-"))
                {
                    if ("clipboard".StartsWith(p.Substring(1).ToLower()))
                        useClipboard = true;
                    else
                    {
                        MessageBox.Show(String.Format("Unrecognised switch: \"{0}\"", p),
                            "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return 1;
                    }
                }
                else
                    files.Add(s);
            }

            if (useClipboard && files.Count > 0)
            {
                MessageBox.Show("Do not use /Clipboard with other arguments",
                    "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return 2;
            }
            if (files.Count > 1)
            {
                MessageBox.Show("Only pass a single file argument",
                    "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return 3;
            }

            useFile = files.Count > 0;
            useClipboard = !useFile;

            Stream ms;

            if (useClipboard)
            {
                ms = Clipboard.GetData(DataFormats.Serializable) as MemoryStream;
                if (ms == null)
                {
                    MessageBox.Show("Invalid clipboard content",
                        "Fail", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return 4;
                }
                Clipboard.Clear();
            }
            else
            {
                try
                {
                    ms = File.Open(files[0], FileMode.Open, FileAccess.ReadWrite);
                }
                catch (Exception ex)
                {
                    IssueException(ex);
                    return -1;
                }
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new MainForm(ms));
            }
            catch (Exception ex)
            {
                IssueException(ex);
                return -1;
            }

            return 0;
        }
    }
}
