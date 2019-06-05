using System;
using System.Collections;
using System.Collections.Generic;

namespace LNF.Models.Data
{
    public class DataFeedResult
    {
        public int ID { get; set; }
        public Guid GUID { get; set; }
        public string Alias { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Private { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DataFeedResultSet Data { get; set; }
    }

    public interface IDataFeedResultItemConverter<T>
    {
        string Key { get; }
        T Convert(DataFeedResultItem item);
    }

    public class DataFeedResultSet : IEnumerable<KeyValuePair<string, DataFeedResultItemCollection>>
    {
        private readonly IDictionary<string, DataFeedResultItemCollection> _source = new Dictionary<string, DataFeedResultItemCollection>();

        public int Count => _source.Count;

        public void Add(string key, DataFeedResultItemCollection items)
        {
            _source.Add(key, items);
        }

        public DataFeedResultItemCollection this[string key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        private DataFeedResultItemCollection GetValue(string key)
        {
            if (_source.ContainsKey(key))
                return _source[key];
            else
                return null;
        }

        private void SetValue(string key, DataFeedResultItemCollection value)
        {
            if (_source.ContainsKey(key))
                _source[key] = value;
            else
                _source.Add(key, value);
        }

        public IEnumerable<T> Items<T>(IDataFeedResultItemConverter<T> converter)
        {
            var items = new List<T>();

            if (_source.ContainsKey(converter.Key))
            {
                foreach (var item in _source[converter.Key])
                {
                    items.Add(converter.Convert(item));
                }
            }
            
            return items;
        }

        public IEnumerator<KeyValuePair<string, DataFeedResultItemCollection>> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class DataFeedResultItemCollection : IEnumerable<DataFeedResultItem>
    {
        private readonly IList<DataFeedResultItem> _source = new List<DataFeedResultItem>();

        public DataFeedResultItem this[int index]
        {
            get => GetValue(index);
            set => SetValue(index, value);
        }

        public void Add(DataFeedResultItem item)
        {
            _source.Add(item);
        }

        private DataFeedResultItem GetValue(int index)
        {
            if (_source.Count > index)
                return _source[index];
            else
                return null;
        }

        private void SetValue(int index, DataFeedResultItem value)
        {
            if (_source.Count > index)
                _source[index] = value;
            else
                _source.Add(value);
        }

        public IEnumerator<DataFeedResultItem> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<string> Keys() => _source[0].Keys();
    }

    public class DataFeedResultItem : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly IDictionary<string, string> _source = new Dictionary<string, string>();

        public void Add(string key, string value)
        {
            _source.Add(key, value);
        }

        public string this[string key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        private string GetValue(string key)
        {
            if (_source.ContainsKey(key))
                return _source[key];
            else
                return null;
        }

        private void SetValue(string key, string value)
        {
            if (_source.ContainsKey(key))
                _source[key] = value;
            else
                _source.Add(key, value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<string> Keys() => _source.Keys;
    }
}