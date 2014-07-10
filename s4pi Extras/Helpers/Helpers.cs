using System;
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;
using System.Windows.Forms;
using s4pi.WrapperDealer;

namespace s4pi.Helpers
{
    /// <summary>
    /// Use this class to turn {IPackage, IResourceIndexEntry} tuples into commands to be executed
    /// </summary>
    public class HelperManager : List<HelperManager.Helper>
    {
        static List<string> reserved = new List<string>(new string[] { // must be lower case
                "wrapper", "label", "desc", "command", "arguments", "readonly", "ignorewritetimestamp",
            });
        static List<string> keywords = new List<string>();
        static Dictionary<string, Dictionary<string, string>> helpers = null;

        static void ReadConfig()
        {
            keywords.AddRange(reserved.ToArray());
            keywords.AddRange(AApiVersionedFields.GetContentFields(0, typeof(IResourceKey)).ToArray()); // must be correct case

            helpers = new Dictionary<string, Dictionary<string, string>>();

            // Parse *.helper in Helpers/ in folder where this assembly lives.

            string folder = Path.Combine(Path.GetDirectoryName(typeof(HelperManager).Module.FullyQualifiedName), "Helpers");
            foreach (string file in Directory.GetFiles(folder, "*.helper"))
            {
                StreamReader sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read));
                Dictionary<string, string> target = new Dictionary<string, string>();
                target.Add("file", Path.GetFileNameWithoutExtension(file));

                bool inCommentBlock = false;

                for (string s = sr.ReadLine(); s != null; s = sr.ReadLine())
                {
                    s = s.Trim();

                    #region Comments
                    if (inCommentBlock)
                    {
                        int i = s.IndexOf("*/");
                        if (i > -1)
                        {
                            s = s.Substring(i + 2).Trim();
                            inCommentBlock = false;
                        }
                    }

                    string[] commentMarks = { "#", ";", "//" };
                    for (int i = 0; s.Length > 0 && i < commentMarks.Length; i++)
                    {
                        int j = s.IndexOf(commentMarks[i]);
                        if (j > -1) s = s.Substring(0, j).Trim();
                    }

                    if (inCommentBlock || s.Length == 0) continue;

                    if (s.Contains("/*"))
                    {
                        s = s.Substring(0, s.IndexOf("/*")).Trim();
                        inCommentBlock = true;
                    }
                    #endregion

                    string[] headtail = s.Trim().Split(new char[] { ':', '=' }, 2);
                    if (headtail.Length != 2) continue;
                    string keyword = headtail[0].Trim();
                    if (reserved.Contains(keyword.ToLower())) keyword = keyword.ToLower();
                    if (!keywords.Contains(keyword)) continue;
                    if (target.ContainsKey(keyword)) continue;
                    target.Add(keyword, headtail[1].Trim());
                }

                sr.Close();

                if (target.Count > 0 && target.ContainsKey("command"))
                    helpers.Add(Path.GetFileNameWithoutExtension(file), target);
            }
        }

        public static void Reload() { helpers = null; }
        static List<Helper> allHelpers = null;
        public static List<Helper> Helpers
        {
            get
            {
                if (helpers == null) { ReadConfig(); allHelpers = null; }
                if (allHelpers == null)
                {
                    allHelpers = new List<Helper>();
                    foreach (string key in helpers.Keys)
                        allHelpers.Add(new Helper(helpers[key], new s4pi.Extensions.TGIN(), null));
                }
                return allHelpers;
            }
        }


        public struct Helper
        {
            public readonly string id;
            public readonly string label;
            public readonly string desc;
            public readonly string command;
            public readonly string arguments;
            public readonly bool isReadOnly;
            public readonly bool ignoreWriteTimestamp;
            public readonly bool export;
            public readonly string filename;
            public readonly IResource res;
            public Helper(Dictionary<string, string> match, s4pi.Extensions.TGIN tgin, IResource res)
            {
                id = getString(match, "file");
                label = getString(match, "label");
                desc = getString(match, "desc");
                command = getString(match, "command");
                arguments = getString(match, "arguments");
                isReadOnly = getString(match, "readonly").Length > 0;
                export = command.IndexOf("{}") >= 0 || arguments.IndexOf("{}") >= 0;
                ignoreWriteTimestamp = export && getString(match, "ignorewritetimestamp").Length > 0;
                filename = export ? Path.Combine(Path.GetTempPath(), tgin) : null;
                this.res = res;
            }
            static string getString(Dictionary<string, string> cfg, string key) { return cfg.ContainsKey(key) ? cfg[key] : ""; }
        }
        bool hasID(string id) { foreach (Helper helper in this) if (helper.id.Equals(id)) return true; return false; }
        Helper getID(string id) { foreach (Helper helper in this) if (helper.id.Equals(id)) return helper; throw new ArgumentOutOfRangeException(); }

        /// <summary>
        /// Initialise a new helpers list for a given resource
        /// </summary>
        /// <param name="key">The resource index entry</param>
        /// <param name="res">The resource</param>
        /// <param name="resname">(Optional) The resource name</param>
        public HelperManager(IResourceIndexEntry key, IResource res, string resname = null)
        {
            if (helpers == null) ReadConfig();

            if (res == null || key == null) return;

            string wrapper = res.GetType().Name.ToLower();

            foreach (string cfg in helpers.Keys)
            {
                Dictionary<string, string> match = null;

                foreach (string kwd in helpers[cfg].Keys)
                {
                    if (kwd.Equals("wrapper"))
                    {
                        if ((new List<string>(helpers[cfg]["wrapper"].Split(' '))).Contains("*")) { match = helpers[cfg]; goto matched; }
                        if ((new List<string>(helpers[cfg]["wrapper"].ToLower().Split(' '))).Contains(wrapper)) { match = helpers[cfg]; goto matched; }
                        continue;
                    }

                    if (reserved.Contains(kwd)) continue;

                    if (keywords.Contains(kwd))
                    {
                        if ((new List<string>(helpers[cfg][kwd].Split(' '))).Contains("" + key[kwd])) { match = helpers[cfg]; goto matched; }
                        if ((new List<string>(helpers[cfg][kwd].Split(' '))).Contains("*")) { match = helpers[cfg]; goto matched; }
                        continue;
                    }
                }
            matched:
                if (match != null)
                    this.Add(new Helper(match, new s4pi.Extensions.TGIN(key, resname), res));
            }
        }

        public MemoryStream execHelper(int i)
        {
            Helper helper = this[i];
            try
            {
                DateTime lastWriteTime = new DateTime();
                if (helper.export)
                    lastWriteTime = pasteTo(helper.res, helper.filename);
                else
                    Clipboard.SetData(DataFormats.Serializable, helper.res.Stream);

                bool result = Execute(helper.res, helper, helper.command, helper.arguments);
                if (!helper.isReadOnly && result)
                {
                    if (helper.export)
                    {
                        return copyFrom(helper.filename, helper.ignoreWriteTimestamp, lastWriteTime);
                    }
                    else if (Clipboard.ContainsData(DataFormats.Serializable))
                    {
                        return Clipboard.GetData(DataFormats.Serializable) as MemoryStream;
                    }
                }
                return null;
            }
            finally
            {
                if (helper.filename != null) File.Delete(helper.filename);
            }
        }

        public static MemoryStream Edit(IResourceKey key, IResource res, string command, bool wantsQuotes, bool ignoreWriteTimestamp)
        {
            string filename = Path.Combine(Path.GetTempPath(), (s4pi.Extensions.TGIN)(key as AResourceKey));
            try
            {
                DateTime lastWriteTime = pasteTo(res, filename);

                string quote = wantsQuotes ? new string(new char[] { '"' }) : "";
                bool result = Execute(res, new Helper(), command, quote + filename + quote);
                if (!result) return null;

                return copyFrom(filename, ignoreWriteTimestamp, lastWriteTime);
            }
            finally
            {
                File.Delete(filename);
            }
        }

        static DateTime pasteTo(IResource res, string filename)
        {
            BinaryWriter bw = new BinaryWriter((new FileStream(filename, FileMode.Create, FileAccess.Write)));
            MemoryStream ms = res.Stream as MemoryStream;
            if (ms != null) bw.Write(ms.ToArray());
            else
            {
                res.Stream.Position = 0;
                bw.Write(new BinaryReader(res.Stream).ReadBytes((int)res.Stream.Length));
            }
            bw.Close();
            return File.GetLastWriteTime(filename);
        }
        static MemoryStream copyFrom(string filename, bool ignoreWriteTimestamp, DateTime lastWriteTime)
        {
            if (ignoreWriteTimestamp || File.GetLastWriteTime(filename) != lastWriteTime)
            {
                MemoryStream ms = new MemoryStream();
                FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
                (new BinaryWriter(ms)).Write((new BinaryReader(fs)).ReadBytes((int)fs.Length));
                fs.Close();
                return ms;
            }
            return null;
        }

        static bool Execute(IResource res, Helper helper, string command, string arguments)
        {
            command = command.Replace("{}", helper.filename);
            arguments = arguments.Replace("{}", helper.filename);
            foreach (string prop in res.ContentFields)
                if (arguments.IndexOf("{" + prop.ToLower() + "}") >= 0) arguments = arguments.Replace("{" + prop.ToLower() + "}", "" + res[prop]);

            System.Diagnostics.Process p = new System.Diagnostics.Process();

            p.StartInfo.FileName = command;
            p.StartInfo.Arguments = arguments;
            p.StartInfo.UseShellExecute = false;

            try { p.Start(); }
            catch (Exception ex)
            {
                CopyableMessageBox.IssueException(ex,
                    typeof(HelperManager).Assembly.FullName + "\n" + String.Format("Application failed to start:\n{0}\n{1}", command, arguments),
                    "Launch failed");
                return false;
            }

            Application.DoEvents();
            while (!p.WaitForExit(500))
                Application.DoEvents();

            return p.ExitCode == 0;
        }
    }
}
