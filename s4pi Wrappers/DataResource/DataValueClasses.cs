using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using s4pi.DataResource;
using s4pi.Interfaces;

namespace s4pi.DataResource
{
    public class DataEntry
    {
        private s4pi.DataResource.DataResource.Data parentDataBlock;
        private int dataOffset;
        private s4pi.DataResource.DataResourceFlags.FieldDataTypeFlags type;
        private object dataValue;

        public DataEntry(s4pi.DataResource.DataResource.Data parent, int offset, s4pi.DataResource.DataResourceFlags.FieldDataTypeFlags type) { this.parentDataBlock = parent; this.dataOffset = offset; this.type = type; }

        private void ReadData()
        {

        }

        public object DataValue { get { return dataValue; } set { dataValue = value; } }
    }

    #region Data Class
    public abstract class DataBase
    {
        protected byte[] rawData;
        public DataBase(byte[] source) { this.rawData = source; }
        public virtual byte[] GetRawData() { return rawData; }
        public abstract Object Value { get; set; }
    }

    public class Int32Data : DataBase
    {
        private Int32 dataValue;
        public Int32Data(byte[] source) : base(source) { dataValue = BitConverter.ToInt32(source, 0); }
        public override object Value
        {
            get { return dataValue; }
            set { this.rawData = BitConverter.GetBytes((int)value); this.dataValue = (int)value; }
        }
    }

    public class FloatData : DataBase
    {
        private float dataValue;
        public FloatData(byte[] source) : base(source) { dataValue = BitConverter.ToSingle(source, 0); }
        public override object Value
        {
            get { return dataValue; }
            set { this.rawData = BitConverter.GetBytes((float)value); this.dataValue = (float)value; }
        }
    }

    public class TGIData : DataBase
    {
        private TGIBlock dataValue;
        public TGIData(byte[] source) : base(source) { dataValue = new TGIBlock(1, null, BitConverter.ToUInt32(source, 8), BitConverter.ToUInt32(source, 12), BitConverter.ToUInt64(source, 0)); }
        public override object Value
        {
            get { return dataValue; }
            set
            {
                TGIBlock tgi = (TGIBlock)value;
                Array.Copy(BitConverter.GetBytes(tgi.Instance), 0, this.rawData, 0, 8);
                Array.Copy(BitConverter.GetBytes(tgi.ResourceType), 0, this.rawData, 8, 4);
                Array.Copy(BitConverter.GetBytes(tgi.ResourceGroup), 0, this.rawData, 12, 4);
                dataValue = tgi;
            }
        }
    }

    public class RGBColorData : DataBase
    {
        private int[] dataValue;
        public RGBColorData(byte[] source) : base(source) { dataValue = new int[3] { BitConverter.ToInt32(source, 0), BitConverter.ToInt32(source, 4), BitConverter.ToInt32(source, 8) }; }

        public override object Value
        {
            get { return dataValue; }
            set
            {
                int[] arrayValue = (int[])value;
                for (int i = 0; i < 3; i++)
                {
                    Array.Copy(BitConverter.GetBytes(arrayValue[i]), 0, this.rawData, i * 4, 4);
                }
                dataValue = arrayValue;
            }
        }
    }

    public class ARGBColorData : DataBase
    {
        private int[] dataValue;
        public ARGBColorData(byte[] source) : base(source) { dataValue = new int[4] { BitConverter.ToInt32(source, 0), BitConverter.ToInt32(source, 4), BitConverter.ToInt32(source, 8), BitConverter.ToInt32(source, 12) }; }

        public override object Value
        {
            get { return dataValue; }
            set
            {
                int[] arrayValue = (int[])value;
                for (int i = 0; i < 4; i++)
                {
                    Array.Copy(BitConverter.GetBytes(arrayValue[i]), 0, this.rawData, i * 4, 4);
                }
                dataValue = arrayValue;
            }
        }
    }

    public class NotImplemented : DataBase
    {
        private string dataValue = "Not implemented yet";
        public NotImplemented(byte[] source) : base(source) { }
        public override object Value { get { return dataValue; } set { return; } }
    }

    #endregion
}
