/***************************************************************************
 *  Copyright (C) 2009 by Peter L Jones                                    *
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
using s3pi.Interfaces;
using ObjKeyResource;

namespace s3pe_OBJK_Resource_Editor
{
    public partial class MainForm : Form, s3pi.Helpers.IRunHelper
    {
        const string myName = "s3pe OBJK Resource Editor";
        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(Stream s)
            : this()
        {
            try
            {
                Application.UseWaitCursor = true;
                loadObjKey(s);
            }
            finally { Application.UseWaitCursor = false; }
        }

        static Dictionary<string, TypeKey> ComponentDataTypeMap;
        struct TypeKey
        {
            public Type type;
            public string key;
            public TypeKey(Type t, string k) { type = t; key = k; }
        }
        static MainForm()
        {
            ComponentDataTypeMap = new Dictionary<string, TypeKey>();
            ComponentDataTypeMap.Add("Sim", new TypeKey(typeof(ObjKeyResource.ObjKeyResource.CDTResourceKey), "simOutfitKey"));
            ComponentDataTypeMap.Add("Script", new TypeKey(typeof(ObjKeyResource.ObjKeyResource.CDTString), "scriptClass"));
            ComponentDataTypeMap.Add("Model", new TypeKey(typeof(ObjKeyResource.ObjKeyResource.CDTAssetResourceName), "modelKey"));
            ComponentDataTypeMap.Add("Steering", new TypeKey(typeof(ObjKeyResource.ObjKeyResource.CDTSteeringInstance), "steeringInstance"));
            ComponentDataTypeMap.Add("Tree", new TypeKey(typeof(ObjKeyResource.ObjKeyResource.CDTResourceKey), "modelKey"));
            ComponentDataTypeMap.Add("Footprint", new TypeKey(typeof(ObjKeyResource.ObjKeyResource.CDTResourceKey), "footprintKey"));
        }

        byte[] result = null;
        public byte[] Result { get { return result; } }

        ObjKeyResource.ObjKeyResource objk;
        List<string> tgis = new List<string>();
        void loadObjKey(Stream data)
        {
            objk = new ObjKeyResource.ObjKeyResource(0, data);

            foreach (TGIBlock tgi in objk.TGIBlocks)
                tgis.Add(tgi);

            InitialiseTable();
        }

        void saveObjKey()
        {
            objk.Components.Clear();
            objk.ComponentData.Clear();
            foreach (string name in ComponentNames)
            {
                if (!hasComponent(name)) continue;
                objk.Components.Add(new ObjKeyResource.ObjKeyResource.ComponentElement(0, null, (ObjKeyResource.ObjKeyResource.Component)Enum.Parse(typeof(ObjKeyResource.ObjKeyResource.Component), name)));
                if (ComponentDataTypeMap.ContainsKey(name))
                {
                    int row = ComponentNames.IndexOf(name);
                    row++;
                    if (name.Equals("Sim") && !((CheckBox)tableLayoutPanel1.GetControlFromPosition(1, row)).Checked) continue;

                    TypeKey tk = ComponentDataTypeMap[name];
                    string type = tk.type.Name.Substring(3);
                    if (type == "String")
                    {
                        TextBox tb = (TextBox)tableLayoutPanel1.GetControlFromPosition(2, row);
                        string value = tb.Text;
                        objk.ComponentData.Add(new ObjKeyResource.ObjKeyResource.CDTString(0, null, tk.key, 0x00, value));
                    }
                    else if (type == "ResourceKey")
                    {
                        TGIBlockCombo cb = (TGIBlockCombo)tableLayoutPanel1.GetControlFromPosition(2, row);
                        Int32 value = cb.SelectedIndex;
                        objk.ComponentData.Add(new ObjKeyResource.ObjKeyResource.CDTResourceKey(0, null, tk.key, 0x01, value));
                    }
                    else if (type == "AssetResourceName")
                    {
                        TGIBlockCombo cb = (TGIBlockCombo)tableLayoutPanel1.GetControlFromPosition(2, row);
                        Int32 value = cb.SelectedIndex;
                        objk.ComponentData.Add(new ObjKeyResource.ObjKeyResource.CDTAssetResourceName(0, null, tk.key, 0x02, value));
                    }
                    else if (type == "SteeringInstance")
                    {
                        TextBox tb = (TextBox)tableLayoutPanel1.GetControlFromPosition(2, row);
                        string value = tb.Text;
                        objk.ComponentData.Add(new ObjKeyResource.ObjKeyResource.CDTSteeringInstance(0, null, tk.key, 0x03, value));
                    }
                    else if (type == "UInt32")
                    {
                        TextBox tb = (TextBox)tableLayoutPanel1.GetControlFromPosition(2, row);
                        string s = tb.Text.ToLower().Trim();
                        UInt32 value;
                        if (s.StartsWith("0x")) value = uint.Parse(s.Substring(2), System.Globalization.NumberStyles.HexNumber);
                        else value = uint.Parse(s);
                        objk.ComponentData.Add(new ObjKeyResource.ObjKeyResource.CDTUInt32(0, null, tk.key, 0x04, value));
                    }
                }
            }
            if (ckbAllowObjectHiding.Checked)
                objk.ComponentData.Add(new ObjKeyResource.ObjKeyResource.CDTUInt32(0, null, "allowObjectHiding", 0x04, 0));

            result = (byte[])objk.AsBytes.Clone();
        }

        List<string> unused;
        List<string> ComponentNames;
        void InitialiseTable()
        {
            unused = new List<string>(new string[] { "Slot", "VisualState", "Effect", "Lighting", });
            tableLayoutPanel1.SuspendLayout();
            ComponentNames = new List<string>(Enum.GetNames(typeof(ObjKeyResource.ObjKeyResource.Component)));
            int tabIndex = 1;
            foreach (string name in ComponentNames)
                AddRow(tableLayoutPanel1, name, ref tabIndex);

            tableLayoutPanel1.RowCount++;
            tableLayoutPanel1.RowStyles.Insert(tableLayoutPanel1.RowCount - 2, new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Controls.Add(ckbAllowObjectHiding, 2, tableLayoutPanel1.RowCount - 2);

            ckbAllowObjectHiding.Checked = objk.ComponentData.ContainsKey("allowObjectHiding");
            tableLayoutPanel1.ResumeLayout();
        }
        void AddRow(TableLayoutPanel tlp, string name, ref int tabIndex)
        {
            tlp.RowCount++;
            tlp.RowStyles.Insert(tlp.RowCount - 2, new RowStyle(SizeType.AutoSize));

            ObjKeyResource.ObjKeyResource.Component component = (ObjKeyResource.ObjKeyResource.Component)Enum.Parse(typeof(ObjKeyResource.ObjKeyResource.Component), name);
            ObjKeyResource.ObjKeyResource.ComponentElement ce = objk.Components.Find(component);

            CheckBox ckb = new CheckBox();
            ckb.Anchor = AnchorStyles.Left;
            ckb.AutoSize = true;
            ckb.Checked = ce != null;
            ckb.Enabled = !unused.Contains(name);
            ckb.Name = "ckb" + name;
            ckb.TabIndex = tabIndex++;
            ckb.Text = name + (unused.Contains(name) ? " (unused)" : "");
            ckb.CheckedChanged += new EventHandler(ckb_CheckedChanged);
            tlp.Controls.Add(ckb, 0, tlp.RowCount - 2);

            if (ComponentDataTypeMap.ContainsKey(name))
            {
                TypeKey tk = ComponentDataTypeMap[name];
                string type = tk.type.Name.Substring(3);
                ObjKeyResource.ObjKeyResource.ComponentDataType cdt = null;
                if (objk.ComponentData.ContainsKey(tk.key)) cdt = objk.ComponentData[tk.key];

                if (name.Equals("Sim"))
                {
                    CheckBox ckb2 = new CheckBox();
                    ckb2.Anchor = AnchorStyles.Right;
                    ckb2.AutoSize = true;
                    ckb2.Enabled = ckb.Checked;
                    ckb2.Checked = cdt != null;
                    ckb2.Name = "ckbData" + tabIndex;
                    ckb2.TabIndex = tabIndex++;
                    ckb2.Text = type + ":";
                    ckb2.CheckedChanged += new EventHandler(ckb2_CheckedChanged);
                    tlp.Controls.Add(ckb2, 1, tlp.RowCount - 2);
                }
                else
                {
                    Label lb = new Label();
                    lb.Anchor = AnchorStyles.Right;
                    lb.AutoSize = true;
                    lb.Name = "lbData" + tabIndex;
                    lb.TabIndex = tabIndex++;
                    lb.Text = type + ":";
                    tlp.Controls.Add(lb, 1, tlp.RowCount - 2);
                }
                if (type == "String" || type == "SteeringInstance")
                {
                    tlp.Controls.Add(StringEditor(ckb.Checked, name, ref tabIndex, cdt), 2, tlp.RowCount - 2);
                }
                else if (type == "UInt32")
                {
                    tlp.Controls.Add(UInt32Editor(ckb.Checked, name, ref tabIndex, cdt), 2, tlp.RowCount - 2);
                }
                else
                {
                    tlp.Controls.Add(ResourceKeyEditor(ckb.Checked, name, ref tabIndex, cdt), 2, tlp.RowCount - 2);
                }
            }
        }
        Control StringEditor(bool enabled, string name, ref int tabIndex, ObjKeyResource.ObjKeyResource.ComponentDataType cdt)
        {
            ObjKeyResource.ObjKeyResource.CDTString cdtString = cdt as ObjKeyResource.ObjKeyResource.CDTString;
            TextBox tb = new TextBox();
            tb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            tb.Enabled = enabled;
            tb.Name = "tb" + name;
            tb.TabIndex = tabIndex++;
            tb.Text = cdtString == null ? "" : cdtString.Data;
            return tb;
        }
        Control UInt32Editor(bool enabled, string name, ref int tabIndex, ObjKeyResource.ObjKeyResource.ComponentDataType cdt)
        {
            ObjKeyResource.ObjKeyResource.CDTUInt32 cdtUInt32 = cdt as ObjKeyResource.ObjKeyResource.CDTUInt32;
            Label lb = new Label();
            lb.AutoSize = true;
            lb.Text = "0xDDDDDDDD";

            TextBox tb = new TextBox();
            tb.Anchor = AnchorStyles.Left;
            tb.Enabled = enabled;
            tb.Name = "tb" + name;
            tb.TabIndex = tabIndex++;
            tb.Text = cdtUInt32 == null ? "" : "0x" + cdtUInt32.Data.ToString("X8");
            tb.Width = lb.PreferredWidth;
            tb.TextChanged += new EventHandler(tb_TextChanged);
            return tb;
        }
        Control ResourceKeyEditor(bool enabled, string name, ref int tabIndex, ObjKeyResource.ObjKeyResource.ComponentDataType cdt)
        {
            ObjKeyResource.ObjKeyResource.CDTResourceKey cdtResourceKey = cdt as ObjKeyResource.ObjKeyResource.CDTResourceKey;
            Label lb = new Label();
            lb.AutoSize = true;
            lb.Text = "(WWWW) 0xDDDDDDDD-0xDDDDDDDD-0xDDDDDDDDDDDDDDDD";

            TGIBlockCombo tbc = new TGIBlockCombo(objk.TGIBlocks, cdtResourceKey == null ? -1 : cdtResourceKey.Data, false);
            tbc.Anchor = AnchorStyles.Left;
            tbc.Enabled = enabled;
            tbc.Name = "tbc" + name;
            tbc.TabIndex = tabIndex++;
            tbc.Width = lb.PreferredWidth;
            tbc.TGIBlockListChanged += new EventHandler(tbc_TGIBlockListChanged);
            return tbc;
        }

        void tbc_TGIBlockListChanged(object sender, EventArgs e)
        {
            (sender as TGIBlockCombo).Refresh();
        }

        bool hasComponent(string name) { CheckBox ckb = ckbComponent(name); return (ckb == null) ? false : ckb.Checked; }

        CheckBox ckbComponent(string name)
        {
            int row = ComponentNames.IndexOf(name);
            if (row == -1) return null;
            row++;
            return (CheckBox)tableLayoutPanel1.GetControlFromPosition(0, row);
        }

        void ckb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb = sender as CheckBox;
            TableLayoutPanelCellPosition tlpcp = tableLayoutPanel1.GetPositionFromControl(ckb);
            if (ckb.Name == "ckbPhysics" && ckb.Checked)
            {
                ckbComponent("Tree").Checked = false;
            }
            else if (ckb.Name == "ckbSim")
            {
                CheckBox ckb2 = (CheckBox)tableLayoutPanel1.GetControlFromPosition(1, tlpcp.Row);
                ckb2.Enabled = ckb.Checked;
            }
            else if (ckb.Name == "ckbModel" && ckb.Checked)
            {
                ckbComponent("Tree").Checked = false;
                ckbComponent("Footprint").Checked = false;
            }
            else if (ckb.Name == "ckbTree" && ckb.Checked)
            {
                ckbComponent("Physics").Checked = false;
                ckbComponent("Model").Checked = false;
            }
            else if (ckb.Name == "ckbFootprint" && ckb.Checked)
            {
                ckbComponent("Model").Checked = false;
            }
            else
            {
            }
            if (!ComponentDataTypeMap.ContainsKey(ckb.Name.Substring(3))) return;
            Control ctrl = tableLayoutPanel1.GetControlFromPosition(2, tlpcp.Row);
            ctrl.Enabled = ckb.Checked;
        }

        void ckb2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox ckb2 = sender as CheckBox;
            TableLayoutPanelCellPosition tlpcp = tableLayoutPanel1.GetPositionFromControl(ckb2);
            Control c = tableLayoutPanel1.GetControlFromPosition(2, tlpcp.Row);
            c.Enabled = ckb2.Checked;
        }

        void tb_TextChanged(object sender, EventArgs e)
        {
            uint value;
            TextBox tb = sender as TextBox;
            string s = tb.Text.Trim().ToLower();
            btnOK.Enabled = s.Length == 0
                || (s.StartsWith("0x") && uint.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out value))
                || (uint.TryParse(s, out value))
                ;
        }


        private void btnOK_Click(object sender, EventArgs e)
        {
            saveObjKey();
            Environment.ExitCode = 0;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTGIEditor_Click(object sender, EventArgs e)
        {
            DialogResult dr = TGIBlockListEditor.Show(objk.TGIBlocks);
            if (dr != DialogResult.OK) return;

            foreach (Control c in tableLayoutPanel1.Controls)
                if (c as TGIBlockCombo != null)
                    (c as TGIBlockCombo).Refresh();
        }
    }
}
