using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Scripting
{
    public abstract class ModelCollection<T> : IEnumerable<T> where T : ModelBase
    {
        protected List<T> items;

        public ModelCollection(IEnumerable<T> items)
        {
            this.items = items.ToList();
        }

        public T this[int index]
        {
            get { return items[index]; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            var props = typeof(T).GetProperties();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table>");
            sb.AppendLine("<thead>");
            foreach (var p in props)
                sb.AppendLine(string.Format("<th>{0}</th>", p.Name));
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");
            foreach (var i in items)
            {
                sb.AppendLine("<tr>");
                foreach (var p in props)
                    sb.AppendLine(string.Format("<td>{0}</td>", p.GetValue(i, null)));
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody>");
            sb.Append("</table>");
            return sb.ToString();
        }

        public T First()
        {
            return items.FirstOrDefault();
        }

        public ModelCollection<T> OrderBy<TKey>(Func<T, TKey> fn)
        {
            items = items.OrderBy(fn).ToList();
            return this;
        }
    }
}
