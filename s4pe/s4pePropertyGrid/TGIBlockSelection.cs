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
    public partial class TGIBlockSelection : UserControl
    {
        public TGIBlockSelection()
        {
            InitializeComponent();
        }

        AApiVersionedFields owner = null;
        string field;
        DependentList<TGIBlock> tgiBlocks;
        public void SetField(AApiVersionedFields owner, string field, DependentList<TGIBlock> tgiBlocks)
        {
            this.owner = owner;
            this.field = field;
            this.tgiBlocks = tgiBlocks;

            tgiBlockCombo1.TGIBlocks = this.tgiBlocks;
            tgiBlockCombo1.SelectedIndex = Convert.ToInt32(this.owner[this.field].Value);
        }

        public int Value { get { return tgiBlockCombo1.SelectedIndex; } }

        IWindowsFormsEditorService edSvc;
        public IWindowsFormsEditorService EdSvc { get { return edSvc; } set { edSvc = value; } }

        private void tgiBlockCombo1_SelectedIndexChanged(object sender, EventArgs e)
        {
            owner[field] = new TypedValue(owner[field].Type, Convert.ChangeType(tgiBlockCombo1.SelectedIndex, owner[field].Type));
            if (edSvc != null) edSvc.CloseDropDown();
        }

        private void tgiBlockCombo1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 27 || e.KeyChar==13)
            {
                e.Handled = true;
                edSvc.CloseDropDown();
            }
        }
    }
}
