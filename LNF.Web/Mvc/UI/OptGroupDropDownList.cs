using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace LNF.Web.Mvc.UI
{
    public class OptGroupDropDownList
    {
        private string _Name;
        public string Name { get { return _Name; } }
        public IEnumerable<OptGroupSelectListItem> Items { get; set; }
        private RouteValueDictionary HtmlAttributes { get; set; }
        public object SelectedValue { get; set; }

        public OptGroupDropDownList(string name, IEnumerable<OptGroupSelectListItem> items = null, object htmlAttributes = null)
        {
            _Name = name;
            Items = items;
            HtmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        }

        public OptGroupDropDownList(string name, IEnumerable<OptGroupSelectListItem> items = null, RouteValueDictionary htmlAttributes = null)
        {
            _Name = name;
            Items = items;
            HtmlAttributes = htmlAttributes;
        }

        public void Render(StringBuilder sb)
        {
            string attribs = HtmlAttributes.ToHtmlAttributesString();
            sb.AppendLine(string.Format("<select id=\"{0}\" name=\"{0}\"{1}>", Name, attribs));
            foreach (OptGroupSelectListItem optgroup in Items)
            {
                optgroup.Render(sb, SelectedValue);
            }
            sb.AppendLine("</select>");
        }
    }
}
