using LNF.CommonTools;
using LNF.Repository;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace LNF
{
    public class QueryParameters : DynamicObject, IEnumerable<QueryParameter>, IDictionary<string, object>
    {
        private IList<QueryParameter> items;

        private QueryParameters() { }

        public static QueryParameters Create()
        {
            var result = new QueryParameters();
            result.items = new List<QueryParameter>();
            return result;
        }

        public static QueryParameters Create(IDictionary<object, object> dictionary)
        {
            var result = new QueryParameters();
            result.items = dictionary.Select(kvp => new QueryParameter(kvp.Key.ToString(), kvp.Value)).ToList();
            return result;
        }

        public static QueryParameters Create(IDictionary<string, object> dictionary)
        {
            var result = new QueryParameters();
            result.items = dictionary.Select(kvp => new QueryParameter(kvp.Key, kvp.Value)).ToList();
            return result;
        }

        public static QueryParameters Create(object obj)
        {
            IDictionary<string, object> dict = Utility.ObjectToDictionary(obj);
            return Create(dict);
        }

        private IEnumerable<QueryParameter> AsEnumerable()
        {
            return items.AsEnumerable();
        }

        private IDictionary<string, object> AsDictionary()
        {
            IDictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var item in items)
            {
                if (!dict.ContainsKey(item.Name))
                    dict.Add(item.Name, item.Value);
            }
            return dict;
        }

        public IEnumerator<QueryParameter> GetEnumerator()
        {
            return AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public QueryParameters AddParameter(QueryParameter item)
        {
            items.Add(item);
            return this;
        }

        public QueryParameters AddParameter<T>(string name, T value)
        {
            if (value == null)
                items.Add(new QueryParameter(name, typeof(T)));
            else
                items.Add(new QueryParameter(name, value));

            return this;
        }

        public QueryParameters AddParameterIf<T>(string name, bool condition, T value)
        {
            if (condition)
                AddParameter(name, value);
            return this;
        }

        public QueryParameters AddRange(IEnumerable<QueryParameter> collection)
        {
            foreach (var item in collection)
                items.Add(item);
            return this;
        }

        public QueryParameter Find(string name)
        {
            return AsEnumerable().FirstOrDefault(x => x.Name == name);
        }

        public bool ContainsParameter(string name)
        {
            return Find(name) != null;
        }

        public void Remove(string name)
        {
            items.Remove(Find(name));
        }

        public T GetValue<T>(string name, T defval)
        {
            QueryParameter item = Find(name);
            if (item != null)
                return Utility.ConvertTo(item.Value, defval);
            else
                return defval;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            QueryParameter item = Find(binder.Name);
            if (item != null)
            {
                result = item.Value;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            QueryParameter item = Find(binder.Name);

            if (item != null)
                item.Value = value;
            else
                AddParameter(binder.Name, value);

            return true;
        }

        /* IDictionary<string, object> */

        public void Add(string key, object value)
        {
            items.Add(new QueryParameter(key, value));
        }

        public bool ContainsKey(string key)
        {
            return Find(key) != null;
        }

        public ICollection<string> Keys
        {
            get { return items.Select(x => x.Name).ToArray(); }
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return items.Remove(Find(key));
        }

        public bool TryGetValue(string key, out object value)
        {
            value = Find(key);
            return value != null;
        }

        public ICollection<object> Values
        {
            get { return items.Select(x => x.Value).ToArray(); }
        }

        public object this[string key]
        {
            get { return Find(key).Value; }
            set { Find(key).Value = value; }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            items.Add(new QueryParameter(item.Key, item.Value));
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return Find(item.Key) != null;
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            AsDictionary().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return items.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return items.Remove(Find(item.Key));
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return AsDictionary().GetEnumerator();
        }
    }
}
