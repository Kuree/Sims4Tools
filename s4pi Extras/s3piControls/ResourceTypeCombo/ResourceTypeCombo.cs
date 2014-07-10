using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using s4pi.Interfaces;
using s4pi.Extensions;

namespace System.Windows.Forms
{
    public partial class ResourceTypeCombo : UserControl
    {
        static string[] tagDropDown;
        static ResourceTypeCombo()
        {
            List<string> tagList = new List<string>();
            try
            {
                foreach (var kvp in ExtList.Ext)
                    if (kvp.Key.StartsWith("0x")) tagList.Add(kvp.Value[0] + " " + kvp.Key);
            }
            catch { }
            tagDropDown = tagList.ToArray();
        }
        public ResourceTypeCombo()
        {
            InitializeComponent();
            cbType.Items.Clear();
            cbType.Items.AddRange(tagDropDown);
        }

        bool valid = true;
        public bool Valid { get { return valid; } private set { if (valid != value) { valid = value; OnValidChanged(this, EventArgs.Empty); } } }

        public event EventHandler ValidChanged;
        protected void OnValidChanged(object sender, EventArgs e) { if (ValidChanged != null)ValidChanged(sender, e); }

        public uint Value
        {
            get
            {
                if (cbType.Text.Length == 0) return 0;
                if (cbType.Items.IndexOf(cbType.Text) < 0)
                {
                    string s = cbType.Text.Trim().ToLower();
                    if (s.StartsWith("0x")) return UInt32.Parse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null);
                    else return UInt32.Parse(s);
                }
                else
                {
                    string s = cbType.Text.Split(' ')[1];
                    return UInt32.Parse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null);
                }
            }
            set
            {
                string type = "0x" + value.ToString("X8");
                if (ExtList.Ext.ContainsKey(type))
                    type = ExtList.Ext[type][0] + " " + type;

                if (cbType.Text == type) return;
                cbType.Text = type;
            }
        }

        [Browsable(false)]
        public override string Text { get { return Valid ? "0x" + Value.ToString("X8") : ""; } set { cbType.Text = value; } }

        public event EventHandler ValueChanged;
        protected void OnValueChanged(object sender, EventArgs e) { if (ValueChanged != null) ValueChanged(sender, e); }

        private void cbType_TextUpdate(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb.Items.IndexOf(cb.Text) < 0)
            {
                string s = cb.Text.Trim().ToLower();
                if (s.Length == 0) Valid = false;
                else
                {
                    UInt32 res;
                    if (s.StartsWith("0x")) Valid = UInt32.TryParse(s.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out res);
                    else Valid = UInt32.TryParse(s, out res);
                }
            }
            else Valid = true;

            if (Valid)
                OnValueChanged(this, EventArgs.Empty);
        }

        private void cbType_SelectedValueChanged(object sender, EventArgs e)
        {
            cbType_TextUpdate(sender, e);
        }
    }
}
