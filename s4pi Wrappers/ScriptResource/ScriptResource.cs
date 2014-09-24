/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
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
using System.Linq;
using s4pi.Interfaces;

namespace ScriptResource
{
    /// <summary>
    /// A resource wrapper that understands Encrypted Signed Assembly (0x073FAA07) resources
    /// </summary>
    public class ScriptResource : AResource
    {
        const Int32 recommendedApiVersion = 1;
        public override int RecommendedApiVersion { get { return recommendedApiVersion; } }

        static bool checking = s4pi.Settings.Settings.Checking;

        #region Attributes
        byte version = 1;
        string gameVersion;
        uint unknown2 = 0x2BC4F79F;
        byte[] md5sum = new byte[64];
        byte[] md5table = new byte[0];
        byte[] md5data = new byte[0];
        byte[] cleardata = new byte[0];
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of the resource
        /// </summary>
        /// <param name="APIversion">Requested API version</param>
        /// <param name="s">Data stream to use, or null to create from scratch</param>
        public ScriptResource(int APIversion, Stream s) : base(APIversion, s) { if (stream == null) { stream = UnParse(); OnResourceChanged(this, EventArgs.Empty); } stream.Position = 0; Parse(stream); }
        #endregion

        #region Data I/O
        void Parse(Stream s)
        {
            BinaryReader br = new BinaryReader(s);
            version = br.ReadByte();
            if (version > 1)
                gameVersion = System.Text.Encoding.Unicode.GetString(br.ReadBytes(br.ReadInt32() * 2));
            else
                gameVersion = "";
            unknown2 = br.ReadUInt32();
            md5sum = br.ReadBytes(64);
            ushort count = br.ReadUInt16();
            md5table = br.ReadBytes(count * 8);
            md5data = br.ReadBytes(count * 512);
            cleardata = decrypt();
        }

        byte[] decrypt()
        {
            ulong seed = 0;
            for (int i = 0; i < md5table.Length; i += 8) seed += BitConverter.ToUInt64(md5table, i);
            seed = (ulong)(md5table.Length - 1) & seed;

            MemoryStream w = new MemoryStream();
            MemoryStream r = new MemoryStream(md5data);

            for (int i = 0; i < md5table.Length; i += 8)
            {
                byte[] buffer = new byte[512];

                if ((md5table[i] & 1) == 0)
                {
                    r.Read(buffer, 0, buffer.Length);

                    for (int j = 0; j < 512; j++)
                    {
                        byte value = buffer[j];
                        buffer[j] ^= md5table[seed];
                        seed = (ulong)((seed + value) % (ulong)md5table.Length);
                    }
                }

                w.Write(buffer, 0, buffer.Length);
            }

            return w.ToArray();
        }

        protected override Stream UnParse()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(version);
			if (version > 1)
			{
                bw.Write(gameVersion.Length);
                bw.Write(System.Text.Encoding.Unicode.GetBytes(gameVersion));
            }
            bw.Write(unknown2);
            md5table = new byte[(((cleardata.Length & 0x1ff) == 0 ? 0 : 1) + (cleardata.Length >> 9)) << 3];
            md5data = encrypt();
            bw.Write(md5sum);
            bw.Write((ushort)(md5table.Length >> 3));
            bw.Write(md5table);
            bw.Write(md5data);
            return ms;
        }

        byte[] encrypt()
        {
            ulong seed = 0;
            for (int i = 0; i < md5table.Length; i += 8) seed += BitConverter.ToUInt64(md5table, i);
            seed = (ulong)(md5table.Length - 1) & seed;

            MemoryStream w = new MemoryStream();
            MemoryStream r = new MemoryStream(cleardata);

            for (int i = 0; i < md5table.Length; i += 8)
            {
                byte[] buffer = new byte[512];
                r.Read(buffer, 0, buffer.Length);

                for (int j = 0; j < 512; j++)
                {
                    buffer[j] ^= md5table[seed];
                    seed = (ulong)((seed + buffer[j]) % (ulong)md5table.Length);
                }

                w.Write(buffer, 0, buffer.Length);
            }

            return w.ToArray();
        }
        #endregion

        #region AApiVersionedFields
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }
        #endregion

        #region Content Fields
        [ElementPriority(1)]
        public byte Version { get { return version; } set { if (version != value) { version = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(2)]
        public string GameVersion { get { return gameVersion; } set { if (gameVersion != value) { gameVersion = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(3)]
        public uint Unknown2 { get { return unknown2; } set { if (unknown2 != value) { unknown2 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(99)]
        public BinaryReader Assembly
        {
            get { return new BinaryReader(new MemoryStream(cleardata)); }
            set
            {
                if (value.BaseStream.CanSeek) { value.BaseStream.Position = 0; cleardata = value.ReadBytes((int)value.BaseStream.Length); }
                else
                {
                    MemoryStream ms = new MemoryStream();
                    byte[] buffer = new byte[1024 * 1024];
                    for (int read = value.BaseStream.Read(buffer, 0, buffer.Length); read > 0; read = value.BaseStream.Read(buffer, 0, buffer.Length))
                        ms.Write(buffer, 0, read);
                    cleardata = ms.ToArray();
                }
                OnResourceChanged(this, EventArgs.Empty);
            }
        }

        [MinimumVersion(1)]
        [MaximumVersion(recommendedApiVersion)]
        public string Value
        {
            get
            {
                List<string> fields = this.ValueBuilderFields;
                string s = "";

                foreach (string f in fields)
                {
                    if (f.Equals("Assembly"))
                    {
                        if (cleardata.Length > 0)
                        {
                            AppDomain ap = AppDomain.CreateDomain("assy");
                            try
                            {
                                SafeLoader loader = (SafeLoader)ap.CreateInstanceAndUnwrap(this.GetType().Assembly.FullName, typeof(SafeLoader).FullName);
                                s += loader.Value(cleardata);
                            }
                            catch (Exception ex) { s += string.Format("{0}: Error: {1}\n", f, ex.Message); }
                            finally { AppDomain.Unload(ap); }
                        }
                        else
                        {
                            s += "Assembly: no data\n";
                        }
                    }
                    else
                        s += string.Format("{0}: {1}\n", f, "" + this[f]);
                }

                return s;
            }
        }

        class SafeLoader : MarshalByRefObject
        {
            public string Value(byte[] rawAssembly)
            {
                string s = "";
                try
                {
                    System.Reflection.Assembly assy = System.Reflection.Assembly.Load(rawAssembly);
                    string h = String.Format("\n---------\n---------\n{0}: Assembly\n---------\n", assy.GetType().Name);
                    string t = "---------\n";
                    s += h;
                    s += assy.ToString() + "\n";

                    foreach (var p in typeof(System.Reflection.Assembly).GetProperties())
                    {
                        try { s += string.Format("  {0}: {1}\n", p.Name, "" + p.GetValue(assy, null)); }
                        catch (Exception ex) { s += string.Format("  {0}: Error reading Value: {1}\n", p.Name, ex.Message); }
                    }

                    s += "\nReferences:\n";
                    foreach (var p in assy.GetReferencedAssemblies())
                        s += string.Format("  Ref: {0}\n", p.ToString());

                    s += "\nExported Types:\n";
                    try
                    {
                        Type[] exportedTypes = assy.GetExportedTypes();
                        foreach (var p in exportedTypes)
                            s += string.Format("  Type: {0}\n", p.ToString());
                    }
                    catch (Exception ex) { s += "  Cannot get Exported Types: " + ex.Message + "\n"; }

                    s += t;
                }
                catch (Exception ex)
                {
                    s = "\n---------\n---------\n Assembly\n---------\n";
                    for (Exception inex = ex; inex != null; inex = inex.InnerException)
                    {
                        s += "\n" + inex.Message;
                        s += "\n" + inex.StackTrace;
                        s += "\n-----";
                    }
                    s += "---------\n";
                }
                return s;
            }
        }
        #endregion
    }

    /// <summary>
    /// ResourceHandler for NameMapResource wrapper
    /// </summary>
    public class ScriptResourceHandler : AResourceHandler
    {
        /// <summary>
        /// Create the content of the Dictionary
        /// </summary>
        public ScriptResourceHandler()
        {
            this.Add(typeof(ScriptResource), new List<string>(new string[] { "0x073FAA07" }));
        }
    }
}
