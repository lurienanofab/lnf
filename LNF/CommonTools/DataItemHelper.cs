using System;
using System.Data;
using System.Reflection;

namespace LNF.CommonTools
{
    public class DataItemValue : IConvertible
    {
        public DataItemValue(object value)
        {
            Value = value;
        }     

        public object Value { get; }
        public bool AsBoolean => Convert.ToBoolean(Value);
        public char AsChar => Convert.ToChar(Value);
        public sbyte AsSByte => Convert.ToSByte(Value);
        public byte AsByte => Convert.ToByte(Value);
        public short AsInt16 => Convert.ToInt16(Value);
        public ushort AsUInt16 => Convert.ToUInt16(Value);
        public int AsInt32 => Convert.ToInt32(Value);
        public uint AsUInt32 => Convert.ToUInt32(Value);
        public long AsInt64 => Convert.ToInt64(Value);
        public ulong AsUInt64 => Convert.ToUInt64(Value);
        public float AsSingle => Convert.ToSingle(Value);
        public double AsDouble => Convert.ToDouble(Value);
        public decimal AsDecimal => Convert.ToDecimal(Value);
        public DateTime AsDateTime => Convert.ToDateTime(Value);
        public string AsString => Convert.ToString(Value);
        public bool IsNull => Value == null || IsDBNull;
        public bool IsDBNull => Value == DBNull.Value;

        TypeCode IConvertible.GetTypeCode() => Convert.GetTypeCode(Value);
        bool IConvertible.ToBoolean(IFormatProvider provider) => Convert.ToBoolean(Value, provider);
        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(Value, provider);
        char IConvertible.ToChar(IFormatProvider provider) => Convert.ToChar(Value, provider);
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => Convert.ToDateTime(Value, provider);
        decimal IConvertible.ToDecimal(IFormatProvider provider) => Convert.ToDecimal(Value, provider);
        double IConvertible.ToDouble(IFormatProvider provider) => Convert.ToDouble(Value, provider);
        short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(Value, provider);
        int IConvertible.ToInt32(IFormatProvider provider) => Convert.ToInt32(Value, provider);
        long IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64(Value, provider);
        sbyte IConvertible.ToSByte(IFormatProvider provider) => Convert.ToSByte(Value, provider);
        float IConvertible.ToSingle(IFormatProvider provider) => Convert.ToSingle(Value, provider);
        string IConvertible.ToString(IFormatProvider provider) => Convert.ToString(Value, provider);
        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => Convert.ChangeType(Value, conversionType, provider);
        ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Value, provider);
        uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Value, provider);
        ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Value, provider);
    }

    public class DataItemHelper
    {
        private object _obj;

        public DataItemHelper(object obj)
        {
            _obj = obj ?? throw new ArgumentNullException("obj");
        }

        public bool IsDataRow
        {
            get { return _obj is DataRow; }
        }

        public bool IsDataRowView
        {
            get { return _obj is DataRowView; }
        }

        public DataItemValue this[string key] => Value(key);

        private DataItemValue Value(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The parameter key must not be null or empty.");

            if (IsDataRow)
            {
                DataRow dr = _obj as DataRow;
                if (dr.Table.Columns.Contains(key))
                    return new DataItemValue(dr[key]);
                else
                    return null;
            }
            else if (IsDataRowView)
            {
                DataRowView drv = _obj as DataRowView;
                if (drv.Row.Table.Columns.Contains(key))
                    return new DataItemValue(drv[key]);
                else
                    return null;
            }
            else
            {
                PropertyInfo pinfo = _obj.GetType().GetProperty(key);
                if (pinfo != null)
                    return new DataItemValue(pinfo.GetValue(_obj, null));
                else
                    return null;
            }
        }
    }
}
