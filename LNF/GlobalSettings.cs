using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.Repository.Data;

namespace LNF
{
    public static class GlobalSettingsUtility
    {
        public static GlobalSettingsItemCollection Items
        {
            get
            {
                GlobalSettingsItemCollection items = new GlobalSettingsItemCollection();
                return items;
            }
        }
    }

    public class GlobalSettingsItemCollection : IEnumerable
    {
        private Dictionary<string, string> _Items;

        public GlobalSettingsItemCollection()
        {
            GlobalSettings[] query = DA.Current.Query<GlobalSettings>().ToArray();

            _Items = new Dictionary<string, string>();
            if (query != null && query.Length > 0)
            {
                foreach (GlobalSettings gs in query)
                {
                    _Items.Add(gs.SettingName, gs.SettingValue);
                }
            }
        }

        public string this[string key]
        {
            get
            {
                if (_Items.ContainsKey(key))
                    return _Items[key];
                return null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _Items.Values.GetEnumerator();
        }
    }
}
