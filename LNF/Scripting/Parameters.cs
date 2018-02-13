using LNF.CommonTools;
using System.Collections.Generic;

namespace LNF.Scripting
{
    public class Parameters : DynamicDictionary
    {
        private Parameters() { }

        public static Parameters Create()
        {
            Parameters result = new Parameters();

            foreach (string key in Providers.Context.Current.QueryString.AllKeys)
                result.Set(new KeyValuePair<string, string>(key, Providers.Context.Current.QueryString[key]));

            string parameters = Providers.Context.Current.PostData["params"];

            if (!string.IsNullOrEmpty(parameters))
            {
                Dictionary<string, object> dict = Providers.Serialization.Json.Deserialize<Dictionary<string, object>>(parameters);
                foreach (KeyValuePair<string, object> kvp in dict)
                    result.Set(kvp);
            }

            return result;
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
    }
}
