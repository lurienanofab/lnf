using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF
{
    public class QueryParameter
    {
        private Type _Type;

        private QueryParameter() { }

        public QueryParameter(string name, Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Name = name;
            Value = null;
            _Type = type;
        }

        public QueryParameter(string name, object value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Name = name;
            Value = value;
            _Type = value.GetType();
        }

        public string Name { get; set; }
        public object Value { get; set; }
        public Type Type { get { return _Type; } }
    }
}
