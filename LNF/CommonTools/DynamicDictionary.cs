using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace LNF.CommonTools
{
    public class DynamicDictionary : DynamicObject, IDictionary<object, object>, IDictionary<string, object>
    {
        protected IDictionary<object, object> _Items;

        public DynamicDictionary()
        {
            _Items = new Dictionary<object, object>();
        }

        public DynamicDictionary(IDictionary<object, object> items)
        {
            _Items = items;
        }

        public DynamicDictionary(Hashtable hash)
        {
            _Items = new Dictionary<object, object>();
            foreach (object k in hash.Keys)
                _Items.Add(k, hash[k]);
        }

        public object this[object key]
        {
            get { return GetValue(key); }
            set { SetValue(key, value); }
        }

        public object GetValue(object key)
        {
            if (_Items.ContainsKey(key))
                return _Items[key];
            else
                return null;
        }

        public void SetValue(object key, object value)
        {
            if (_Items.ContainsKey(key))
                _Items[key] = value;
            else
                _Items.Add(key, value);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetValue(binder.Name, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = GetValue(binder.Name);
            return true;
        }

        public void Add(object key, object value)
        {
            _Items.Add(key, value);
        }

        public bool ContainsKey(object key)
        {
            return _Items.ContainsKey(key);
        }

        public ICollection<object> Keys
        {
            get { return _Items.Keys; }
        }

        public bool Remove(object key)
        {
            return _Items.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            return _Items.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return _Items.Values; }
        }

        public void Add(KeyValuePair<object, object> item)
        {
            _Items.Add(item);
        }

        public void Clear()
        {
            _Items.Clear();
        }

        public bool Contains(KeyValuePair<object, object> item)
        {
            return _Items.Contains(item);
        }

        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _Items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<object, object> item)
        {
            return _Items.Remove(item);
        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDictionary<string, object>.Add(string key, object value)
        {
            Add(key, value);
        }

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return ContainsKey(key);
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return Keys.Select(x => x.ToString()).ToList(); }
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return Remove(key);
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return TryGetValue(key, out value);
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { return Values; }
        }

        object IDictionary<string, object>.this[string key]
        {
            get { return this[key]; }
            set { this[key] = value; }
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            Add(new KeyValuePair<object, object>(item.Key, item.Value));
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return Contains(new KeyValuePair<object, object>(item.Key, item.Value));
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            IDictionary<string, object> temp = _Items.ToDictionary(k => k.Key.ToString(), v => v.Value);
            temp.CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<string, object>>.Count
        {
            get { return Count; }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return IsReadOnly; }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return Remove(new KeyValuePair<object, object>(item.Key, item.Value));
        }

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            IDictionary<string, object> temp = _Items.ToDictionary(k => k.Key.ToString(), v => v.Value);
            return temp.GetEnumerator();
        }
    }
}
