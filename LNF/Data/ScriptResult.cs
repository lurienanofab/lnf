using System;
using System.Collections.Generic;
using System.Text;

namespace LNF.Data
{
    public class ScriptResult
    {
        public string Title { get; set; }
        public IDictionary<string, ScriptData> DataSet { get; }
        public StringBuilder Buffer { get; }
        public StringBuilder Html { get; }

        public Exception Exception { get; set; }

        public ScriptResult()
        {
            
            DataSet = new Dictionary<string, ScriptData>();
            Buffer = new StringBuilder();
            Html = new StringBuilder();
        }

        public void AddData(string key, ScriptData data)
        {
            DataSet.Add(key, data);
        }
    }
}
