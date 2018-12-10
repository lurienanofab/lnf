using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.CommonTools;
using LNF.Repository;

namespace LNF.Reporting
{
    public class CriteriaWriter
    {
        private Stack<string> tags;
        private StringBuilder sb;

        internal CriteriaWriter(StringBuilder sb)
        {
            this.sb = sb;
            this.tags = new Stack<string>();
        }

        public CriteriaWriter WriteMonthSelect()
        {
            List<GenericListItem> items = new List<GenericListItem>();
            items = Utility.GetMonths().Select(x => new GenericListItem(x.Month, x.ToString("MMMM"), x.Month == DateTime.Now.AddMonths(-1).Month)).ToList();
            return WriteSelect("Month", items, new { @class = "month-select" });
        }

        public CriteriaWriter WriteYearSelect(int startYear = 2003)
        {
            List<GenericListItem> items = new List<GenericListItem>();
            items = Utility.GetYears(startYear).Select(x => new GenericListItem(x, x.ToString(), x == DateTime.Now.AddMonths(-1).Year)).ToList();
            return WriteSelect("Year", items, new { @class = "year-select" });
        }

        public CriteriaWriter WriteSelect(string name, IEnumerable<LNF.GenericListItem> items, object htmlAttributes = null)
        {
            sb.AppendFormat("<select name=\"{0}\" id=\"{0}\"{1}>\n", name, GetAttributes(htmlAttributes));
            foreach (LNF.GenericListItem item in items)
            {
                sb.Append(item.AsHtml());
            }
            sb.Append("</select>\n");
            return this;
        }

        public CriteriaWriter WriteTextbox(string name, string value, object htmlAttributes = null)
        {
            sb.AppendFormat("<input type=\"text\" name=\"{0}\" id=\"{0}\" value=\"{1}\"{2} />\n", name, value, GetAttributes(htmlAttributes));
            return this;
        }

        public CriteriaWriter WriteRadios(string name, IEnumerable<GenericListItem> items, object htmlAttributes = null)
        {
            int index = 0;
            string attributes = GetAttributes(htmlAttributes);
            foreach (GenericListItem item in items)
            {
                sb.AppendFormat("<label><input type=\"radio\" name=\"{0}\" id=\"{0}_{1}\"{2} value=\"{3}\"{4} />{5}</label>\n",
                    name,
                    index,
                    attributes,
                    item.Value,
                    (item.Selected) ? " checked" : string.Empty,
                    item.Text);
                index++;
            }
            return this;
        }

        public CriteriaWriter WriteCheckbox(string name, GenericListItem item, object htmlAttributes = null)
        {
            sb.AppendFormat("<label><input type=\"checkbox\" name=\"{0}\" id=\"{0}\"{1} value=\"{2}\"{3} />{4}</label>\n",
                name,
                GetAttributes(htmlAttributes),
                item.Value,
                (item.Selected) ? " checked" : string.Empty,
                item.Text);
            return this;
        }

        public CriteriaWriter WriteButton(string name, string value, object htmlAttributes = null)
        {
            sb.AppendFormat("<input type=\"button\" name=\"{0}\" id=\"{0}\" value=\"{1}\"{2} />\n", name, value, GetAttributes(htmlAttributes));
            return this;
        }

        public CriteriaWriter WriteBeginTag(string tag, object htmlAttributes = null)
        {
            tags.Push(tag);
            sb.AppendFormat("<{0}{1}>\n", tag, GetAttributes(htmlAttributes));
            return this;
        }

        public CriteriaWriter WriteEndTag()
        {
            if (tags.Count > 0)
            {
                string currentTag = tags.Pop();
                if (!string.IsNullOrEmpty(currentTag))
                    sb.AppendFormat("</{0}>\n", currentTag);
            }
            return this;
        }

        public CriteriaWriter WriteText(string text)
        {
            sb.Append(text);
            return this;
        }

        public CriteriaWriter WriteAction(Action<StringBuilder> action)
        {
            action.Invoke(sb);
            return this;
        }

        public string GetAttributes(object htmlAttributes)
        {
            if (htmlAttributes == null) return string.Empty;
            IDictionary<string, object> attrs = Utility.ObjectToDictionary(htmlAttributes);
            string attributes = string.Empty;
            foreach (KeyValuePair<string, object> kvp in attrs)
            {
                attributes += string.Format(" {0}=\"{1}\"", kvp.Key, kvp.Value);
            }
            return attributes;
        }
    }
}
