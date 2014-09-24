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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using s4pi.Interfaces;
using System.Windows.Forms.Design;

namespace S4PIDemoFE
{
    public partial class ReaderEditorPanel : UserControl
    {
        public ReaderEditorPanel()
        {
            InitializeComponent();
        }

        AApiVersionedFields owner;
        string field;
        Type type;
        public void SetField(AApiVersionedFields owner, string field)
        {
            type = AApiVersionedFields.GetContentFieldTypes(0, owner.GetType())[field];
            if (!(typeof(TextReader).IsAssignableFrom(type) || typeof(BinaryReader).IsAssignableFrom(type)))
                throw new InvalidCastException();
            this.owner = owner;
            this.field = field;
            btnImport.Enabled = owner.GetType().GetProperty(field).CanWrite;
            btnExport.Enabled = owner.GetType().GetProperty(field).CanRead;

            //TextReader does not have BaseStream, which is needed for ViewHex
            btnViewHex.Enabled = typeof(BinaryReader).IsAssignableFrom(type);

            btnEdit.Enabled = false;
            if (btnExport.Enabled && btnImport.Enabled)
            {
                bool hasText = S4PIDemoFE.Properties.Settings.Default.TextEditorCmd != null && S4PIDemoFE.Properties.Settings.Default.TextEditorCmd.Length > 0;
                bool hasHex = S4PIDemoFE.Properties.Settings.Default.HexEditorCmd != null && S4PIDemoFE.Properties.Settings.Default.HexEditorCmd.Length > 0;

                btnEdit.Enabled = (typeof(TextReader).IsAssignableFrom(type) && hasText) || (typeof(BinaryReader).IsAssignableFrom(type) && hasHex);
            }
        }

        IWindowsFormsEditorService edSvc;
        public IWindowsFormsEditorService EdSvc { get { return edSvc; } set { edSvc = value; } }

        private void Import_TextReader()
        {
            openFileDialog1.Filter = "Text files|*.txt;*.xml|All files|*.*";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            TextReader sr = owner[field].Value as TextReader;
            string oldval = sr.ReadToEnd();

            FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
            try
            {
                owner[field] = new TypedValue(type, new StreamReader(fs));
            }
            catch (Exception ex)
            {
                owner[field] = new TypedValue(type, new StringReader(oldval));
                MainForm.IssueException(ex, "Text import failed.  Recovery attempted but resource may be corrupt.");
            }
            fs.Close();
        }
        private void Import_BinaryReader()
        {
            openFileDialog1.Filter = "All files|*.*";
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            BinaryReader br = owner[field].Value as BinaryReader;
            byte[] oldval = new byte[br.BaseStream.Length];
            br.BaseStream.Seek(0, SeekOrigin.Begin);
            br.Read(oldval, 0, oldval.Length);

            FileStream fs = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
            try
            {
                owner[field] = new TypedValue(type, new BinaryReader(fs));
            }
            catch (Exception ex)
            {
                owner[field] = new TypedValue(type, new BinaryReader(new MemoryStream(oldval)));
                MainForm.IssueException(ex, "Binary import failed.  Recovery attempted but resource may be corrupt.");
            }
            fs.Close();
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (typeof(TextReader).IsAssignableFrom(type)) Import_TextReader();
            if (typeof(BinaryReader).IsAssignableFrom(type)) Import_BinaryReader();
            edSvc.CloseDropDown();
        }

        private void Export_TextReader()
        {
            saveFileDialog1.Filter = "Text files|*.txt;*.xml|All files|*.*";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
            TextReader r = owner[field].Value as TextReader;
            try { ((StreamReader)r).BaseStream.Position = 0; }
            catch { }
            (new BinaryWriter(fs)).Write(r.ReadToEnd().ToCharArray());
            fs.Close();
        }
        private void Export_BinaryReader()
        {
            saveFileDialog1.Filter = "All files|*.*";
            DialogResult dr = saveFileDialog1.ShowDialog();
            if (dr != DialogResult.OK) return;

            FileStream fs = new FileStream(saveFileDialog1.FileName, FileMode.Create, FileAccess.Write);
            BinaryReader r = owner[field].Value as BinaryReader;
            if (r.BaseStream.CanSeek) r.BaseStream.Position = 0;
            (new BinaryWriter(fs)).Write(r.ReadBytes((int)r.BaseStream.Length));
            fs.Close();
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (typeof(TextReader).IsAssignableFrom(type)) Export_TextReader();
            if (typeof(BinaryReader).IsAssignableFrom(type)) Export_BinaryReader();
            edSvc.CloseDropDown();
        }

        private void btnViewHex_Click(object sender, EventArgs e)
        {
            try
            {
                this.Enabled = false;
                Application.DoEvents();

                IResource resource = s4pi.WrapperDealer.WrapperDealer.CreateNewResource(0, "0x00000000");
                BinaryReader r = owner[field].Value as BinaryReader;
                if (r.BaseStream.CanSeek) r.BaseStream.Position = 0;
                (new BinaryWriter(resource.Stream)).Write(r.ReadBytes((int)r.BaseStream.Length));
                if (resource.Stream.CanSeek) resource.Stream.Position = 0;

                Control c = (new HexControl(resource.Stream)).ValueControl;

                Form f = new Form();
                f.SuspendLayout();
                f.Controls.Add(c);
                c.Dock = DockStyle.Fill;
                f.Icon = ((System.Drawing.Icon)(new System.ComponentModel.ComponentResourceManager(typeof(MainForm)).GetObject("$this.Icon")));
                f.Text = "Hex view";
                f.ClientSize = new Size(this.ClientSize.Width - (this.ClientSize.Width / 5), this.ClientSize.Height - (this.ClientSize.Height / 5));
                f.StartPosition = FormStartPosition.CenterParent;
                f.ResumeLayout();
                f.FormClosed += new FormClosedEventHandler((s, fce) => { if (!(s as Form).IsDisposed) (s as Form).Dispose(); });
                f.Show(this);
            }
            finally { this.Enabled = true; edSvc.CloseDropDown(); }
        }

        private void Edit_TextReader()
        {
            byte[] data = null;
            TextReader r = owner[field].Value as TextReader;
            try { data = ((MemoryStream)((StreamReader)r).BaseStream).ToArray(); }
            catch { data = System.Text.Encoding.UTF8.GetBytes(r.ReadToEnd()); }
            data = Edit(data, TextEdit);
            if (data != null)
                owner[field] = new TypedValue(type, new StringReader(System.Text.Encoding.UTF8.GetString(data)));
        }
        private void Edit_BinaryReader()
        {
            byte[] data = null;
            BinaryReader r = owner[field].Value as BinaryReader;
            if (r.BaseStream.CanSeek) r.BaseStream.Position = 0;
            data = r.ReadBytes((int)r.BaseStream.Length);
            data = Edit(data, HexEdit);
            if (data != null)
                owner[field] = new TypedValue(type, new BinaryReader(new MemoryStream(data)));
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (typeof(TextReader).IsAssignableFrom(type)) Edit_TextReader();
            if (typeof(BinaryReader).IsAssignableFrom(type)) Edit_BinaryReader();
            edSvc.CloseDropDown();
            foreach (Form f in Application.OpenForms) { f.TopMost = true; Application.DoEvents(); }
            foreach (Form f in Application.OpenForms) { f.Focus(); Application.DoEvents(); }
            foreach (Form f in Application.OpenForms) { f.TopMost = false; Application.DoEvents(); }
        }

        delegate MemoryStream Editor(IResourceKey key, IResource res);
        private byte[] Edit(byte[] data, Editor editor)
        {
            IResourceKey rk = new TGIBlock(0, null);
            IResource res = s4pi.WrapperDealer.WrapperDealer.CreateNewResource(0, "0x00000000");
            res.Stream.Position = 0;
            new BinaryWriter(res.Stream).Write(data);

            MemoryStream ms = editor(rk, res);

            return ms == null ? null : ms.ToArray();
        }
        MemoryStream TextEdit(IResourceKey key, IResource res)
        {
            return s4pi.Helpers.HelperManager.Edit(key, res,
                S4PIDemoFE.Properties.Settings.Default.TextEditorCmd,
                S4PIDemoFE.Properties.Settings.Default.TextEditorWantsQuotes,
                S4PIDemoFE.Properties.Settings.Default.TextEditorIgnoreTS);
        }
        MemoryStream HexEdit(IResourceKey key, IResource res)
        {
            return s4pi.Helpers.HelperManager.Edit(key, res,
                S4PIDemoFE.Properties.Settings.Default.HexEditorCmd,
                S4PIDemoFE.Properties.Settings.Default.HexEditorWantsQuotes,
                S4PIDemoFE.Properties.Settings.Default.HexEditorIgnoreTS);
        }
    }
}
