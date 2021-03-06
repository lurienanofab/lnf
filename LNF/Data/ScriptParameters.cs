﻿using LNF.CommonTools;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class ScriptParameters : DynamicDictionary
    {
        public static readonly ScriptParameters Empty = new ScriptParameters(new Dictionary<object, object>());

        private ScriptParameters(IDictionary<object, object> items) : base(items) { }

        public static ScriptParameters Create(IDictionary<object, object> items)
        {
            return new ScriptParameters(items);
        }

        public void Set(KeyValuePair<object, object> kvp)
        {
            this[kvp.Key] = kvp.Value;
        }

        public void Set(KeyValuePair<string, object> kvp)
        {
            this[kvp.Key] = kvp.Value;
        }

        public void Set(KeyValuePair<string, string> kvp)
        {
            this[kvp.Key] = kvp.Value;
        }

        public T Get<T>(string key, T defval)
        {
            object result = this[key];
            if (result == null) return defval;
            else return Utility.ConvertTo(result, defval);
        }

        public string Replace(string input)
        {
            string result = input;
            if (!string.IsNullOrEmpty(result))
                foreach (var kvp in this)
                    result = result.Replace(string.Format("?{0}?", kvp.Key), kvp.Value.ToString());
            return result;
        }

        public void Merge(IDictionary<object, object> parameters)
        {
            if (parameters != null)
                foreach (KeyValuePair<object, object> kvp in parameters)
                    Set(kvp);
        }

        public override string ToString()
        {
            var pairs = _Items.Select(x => $"{x.Key}={x.Value}").ToArray();
            var result = string.Join("&", pairs);
            return result;
        }
    }
}
