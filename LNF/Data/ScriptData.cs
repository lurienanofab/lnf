using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public class ScriptData : IEnumerable<DynamicDictionary>
    {
        private IList<DynamicDictionary> items;
        private IList<ScriptHeader> headers;

        public ScriptData()
        {
            items = new List<DynamicDictionary>();
            headers = new List<ScriptHeader>();
        }

        public void AddHeader(ScriptHeader header)
        {
            if (headers.FirstOrDefault(x => x.FieldName == header.FieldName) == null)
                headers.Add(header);
        }

        public ScriptHeader[] GetHeaders()
        {
            if (items.Count > 0 && (headers == null || headers.Count == 0))
            {
                headers = new List<ScriptHeader>();
                var keys = items[0].Keys;
                foreach (string k in keys)
                {
                    ScriptHeader h;
                    object val = items[0][k];

                    if (val == null)
                        h = new ScriptHeader() { FieldName = k, DisplayText = k, Type = typeof(object) };
                    else
                        h = new ScriptHeader() { FieldName = k, DisplayText = k, Type = items[0][k].GetType() };

                    headers.Add(h);
                }
            }
            return headers.ToArray();
        }

        public void AddItem(IDictionary<object, object> item)
        {
            items.Add(new DynamicDictionary(item));
        }

        public IEnumerable<IEnumerable<KeyValuePair<object, object>>> GetItems()
        {
            List<IEnumerable<KeyValuePair<object, object>>> result = new List<IEnumerable<KeyValuePair<object, object>>>();
            foreach (DynamicDictionary dd in Items)
            {
                IDictionary<object, object> dict = new Dictionary<object, object>();
                foreach (KeyValuePair<object, object> kvp in dd)
                    dict.Add(kvp);
                result.Add(dict);
            }
            return result;
        }

        public IEnumerable<dynamic> Items
        {
            get
            {
                List<DynamicDictionary> result = new List<DynamicDictionary>();
                foreach (DynamicDictionary item in items)
                {
                    dynamic i = new DynamicDictionary();
                    foreach (ScriptHeader h in GetHeaders())
                    {
                        string key = h.FieldName;
                        object val = (item.ContainsKey(key)) ? item[key] : null;
                        if (val != null && val.GetType() == typeof(DateTime))
                            val = ((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss");
                        i.Add(key, val);
                    }
                    result.Add(i);
                }
                return result;
            }
        }

        public void RemoveItem(DynamicDictionary item)
        {
            items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            items.RemoveAt(index);
        }

        public DataTable AsDataTable()
        {
            DataTable dt = new DataTable();

            foreach (ScriptHeader h in GetHeaders())
            {
                DataColumn dc;
                if (h.Type == null)
                    dc = new DataColumn(h.DisplayText, typeof(object));
                else
                    dc = new DataColumn(h.DisplayText, h.Type);

                dc.ExtendedProperties.Add("FieldName", h.FieldName);

                dt.Columns.Add(dc);
            }

            if (items.Count == 0) return dt;

            foreach (DynamicDictionary item in items)
            {
                DataRow ndr = dt.NewRow();
                int index = 0;
                foreach (DataColumn dc in dt.Columns)
                {
                    string fieldName = dc.ExtendedProperties["FieldName"].ToString();
                    ndr[index] = item[fieldName];
                    index++;
                }
                dt.Rows.Add(ndr);
            }

            return dt;
        }

        public IEnumerator<DynamicDictionary> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}