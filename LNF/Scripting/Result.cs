using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;    

namespace LNF.Scripting
{
    public class Result
    {
        private IDictionary<string, Data> _DataSet;
        private StringBuilder _Buffer;
        private StringBuilder _HTML;

        public string Title { get; set; }
        public IDictionary<string, Data> DataSet { get { return _DataSet; } }
        public StringBuilder Buffer { get { return _Buffer; } }
        public StringBuilder Html { get { return _HTML; } }

        public Exception Exception { get; set; }

        public Result()
        {
            
            _DataSet = new Dictionary<string, Data>();
            _Buffer = new StringBuilder();
            _HTML = new StringBuilder();
        }

        public void AddData(string key, Data data)
        {
            _DataSet.Add(key, data);
        }
    }
}
