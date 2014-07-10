using System;
using System.Collections.Generic;
using System.ComponentModel;
using s4pi.Interfaces;

namespace S4PIDemoFE.PackageInfo
{
    public partial class PackageInfoFields : Component
    {
        public PackageInfoFields() { }

        static List<string> fields = null;
        static PackageInfoFields()
        {
            fields = new List<string>();
            foreach (string s in AApiVersionedFields.GetContentFields(0, typeof(APackage)))
                if (!s.Contains("Stream") && !s.Contains("List"))
                    fields.Add(s);
        }

        [Browsable(true)]
        [Description("The list of known fields on a Package object")]
        public IList<string> Fields
        {
            get { return fields; }
            //set { }
        }
    }
}
