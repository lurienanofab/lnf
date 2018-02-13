using System;
using System.Data;
using System.Reflection;

namespace LNF.CommonTools
{
    public class DataItemHelper
    {
        private object _obj;

        public DataItemHelper(object obj)
        {
            _obj = obj ?? throw new ArgumentNullException("obj");
        }

        public bool IsDataRow
        {
            get { return _obj is System.Data.DataRow; }
        }

        public bool IsDataRowView
        {
            get { return _obj is System.Data.DataRowView; }
        }

        public object this[string key]
        {
            get
            {
                return Value(key);
            }
        }

        public object Value(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The parameter key must not be null or empty.");

            if (IsDataRow)
            {
                DataRow dr = _obj as DataRow;
                if (dr.Table.Columns.Contains(key))
                    return dr[key];
                else
                    return null;
            }
            else if (IsDataRowView)
            {
                DataRowView drv = _obj as DataRowView;
                if (drv.Row.Table.Columns.Contains(key))
                    return drv[key];
                else
                    return null;
            }
            else
            {
                PropertyInfo pinfo = _obj.GetType().GetProperty(key);
                if (pinfo != null)
                    return pinfo.GetValue(_obj, null);
                else
                    return null;
            }
        }

        public T Value<T>(string key, T defval)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("The parameter key must not be null or empty.");
            object temp = Value(key);
            T result = Utility.ConvertTo(temp, defval);
            return result;
        }

        public bool Compare<T>(string key, T test)
        {
            return Value<T>(key, default(T)).Equals(test);
        }
    }
}
