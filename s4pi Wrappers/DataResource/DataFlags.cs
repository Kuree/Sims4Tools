using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace s4pi.DataResource
{
    public static class DataResourceFlags
    {
        public enum FieldDataTypeFlags : uint
        {
            Boolean = 0x00000000,
            Int16 = 0x00000006,
            Unknown1 = 0x00000007,
            Unknown2 = 0x00000008,
            Float = 0x0000000A,
            VFX = 0x0000000B,
            Unknown3 = 0x0000000E,
            RGBColor = 0x00000010,
            ARGBColor = 0x00000011,
            DataInstance = 0x00000012,
            ImageInstance = 0x00000013,
            StringInstance = 0x00000014
        }

        public static Dictionary<FieldDataTypeFlags, int> DataSizeTable
        {
            get
            {
                return new Dictionary<FieldDataTypeFlags, int>()
                {
                    {FieldDataTypeFlags.Boolean , 4},
                    {FieldDataTypeFlags.Int16 , 4},
                    {FieldDataTypeFlags.Unknown1 , 4},
                    {FieldDataTypeFlags.Unknown2 , 8},
                    {FieldDataTypeFlags.Float , 4},
                    {FieldDataTypeFlags.VFX , 8},
                    {FieldDataTypeFlags.Unknown3 , 8},
                    {FieldDataTypeFlags.RGBColor , 12},
                    {FieldDataTypeFlags.ARGBColor , 16},
                    {FieldDataTypeFlags.DataInstance , 8},
                    {FieldDataTypeFlags.ImageInstance , 16},
                    {FieldDataTypeFlags.StringInstance , 4}
                };
            }
        }

    }
    
}
