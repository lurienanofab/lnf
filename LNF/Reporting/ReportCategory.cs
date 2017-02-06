using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Reporting
{
    public sealed class ReportCategory<T> : IEnumerable<IReport<T>>
        where T : IReportCriteria
    {
        private string _Name;
        private IList<IReport<T>> items;

        internal ReportCategory(string name)
        {
            _Name = name;
            items = new List<IReport<T>>();
        }

        internal ReportCategory<T> AddReport(IReport<T> r)
        {
            items.Add(r);
            return this;
        }

        public IReport<T> Find(string key)
        {
            return items.FirstOrDefault(x => x.Key == key);
        }

        public string Name
        {
            get { return _Name; }
        }

        public int Count
        {
            get { return items.Count; }
        }

        public IReport<T> this[int index]
        {
            get { return items[index]; }
        }

        public IReport<T> this[string key]
        {
            get { return Find(key); }
        }

        public IEnumerator<IReport<T>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
