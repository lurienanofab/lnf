using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace LNF.Web.Mvc.UI
{
    public class SubMenu : IEnumerable<SubMenu.MenuItem>
    {
        private List<MenuItem> items;
        
        private SubMenu() { }

        public bool Bootstrap { get; set; }

        public static SubMenu Create(IEnumerable<MenuItem> items, bool bootstrap = true)
        {
            SubMenu result = new SubMenu();
            result.items = items.ToList();
            result.Bootstrap = bootstrap;
            return result;
        }

        //public MenuItem this[int index]
        //{
        //    get { return items.Select(x => x.Value).ElementAt(index); }
        //}

        //public MenuItem this[string key]
        //{
        //    get { return items[key]; }
        //}

        public int Count
        {
            get { return items.Count; }
        }

        public SubMenu Add(MenuItem item)
        {
            items.Add(item);
            return this;
        }

        public SubMenu Clear()
        {
            items.Clear();
            return this;
        }

        public class MenuItem
        {
            public string LinkText { get; set; }
            public string ActionName { get; set; }
            public string ControllerName { get; set; }
            public bool Active { get; set; }
        }

        public IEnumerator<SubMenu.MenuItem> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //public string URL(string key, string value = null)
        //{
        //    if (key != null && items.ContainsKey(key))
        //    {
        //        if (value != null)
        //            items[key].URL = value;
        //        return items[key].URL;
        //    }
        //    else
        //        return value;
        //}

        //public string Text(string key, string value = null)
        //{
        //    if (key != null && items.ContainsKey(key))
        //    {
        //        if (value != null)
        //            items[key].Text = value;
        //        return items[key].Text;
        //    }
        //    else
        //        return value;
        //}

        //public string Class(string key, string value)
        //{
        //    if (key != null && items.ContainsKey(key))
        //    {
        //        if (value != null)
        //            items[key].Class = value;
        //        return items[key].Class;
        //    }
        //    else
        //        return value;
        //}
    }
}