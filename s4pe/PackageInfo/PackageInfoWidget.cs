using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace S4PIDemoFE.PackageInfo
{
    public partial class PackageInfoWidget : UserControl
    {
        public PackageInfoWidget()
        {
            InitializeComponent();
        }

        IList<string> fields = null;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Specifies the list of fields to display")]
        public IList<string> Fields
        {
            get { return fields; }
            set
            {
                if (fields == value) return;
                fields = value;
                SetFields();
            }
        }

        Dictionary<string, Label> values = null;
        void SetFields()
        {
            values = new Dictionary<string, Label>();
            this.tableLayoutPanel1.RowCount = fields.Count;
            for (int i = 0; i < fields.Count; i++)
            {
                Label l = new Label();
                l.Text = fields[i];
                l.TextAlign = ContentAlignment.MiddleRight;
                l.AutoSize = true;
                tableLayoutPanel1.Controls.Add(l, 0, i);
                l = new Label();
                l.TextAlign = ContentAlignment.MiddleLeft;
                l.AutoSize = true;
                tableLayoutPanel1.Controls.Add(l, 1, i);
                values.Add(fields[i], l);
            }
            tableLayoutPanel1.Update();
        }

        s4pi.Interfaces.IPackage package = null;
        [Browsable(true)]
        [Category("Behavior")]
        [Description("Specifies the package to display info for")]
        public s4pi.Interfaces.IPackage Package
        {
            get { return package; }
            set
            {
                if (package == value) return;
                package = value;
                foreach (Label l in values.Values) l.Text = "";
                if (package == null) return;
                for (int i = 0; i < fields.Count; i++)
                    values[fields[i]].Text = package[fields[i]];
            }
        }
    }
}
