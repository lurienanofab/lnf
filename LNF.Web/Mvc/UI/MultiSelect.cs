using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LNF.Web.Mvc.UI
{
    public class MultiSelect
    {
        public MultiSelect(IEnumerable<MultiSelectItem> items = null, MultiSelectOptions options = null)
        {
            Items = items;
            Options = options;
        }

        public IEnumerable<MultiSelectItem> Items { get; set; }
        public MultiSelectOptions Options { get; set; }

        public string Render(string name)
        {
            if (Options == null)
                Options = new MultiSelectOptions();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table id=\"" + name + "_container\" class=\"multiselect\">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td valign=\"top\" class=\"select-container\">");
            sb.AppendLine("<select size=\"" + Options.Size.ToString() + "\" id=\"" + name + "\" name=\"" + name + "\" multiple>");
            int i = 0;
            if (Items != null)
            {
                foreach (MultiSelectItem li in Items)
                {
                    string list_item_id = name + "_" + i.ToString();
                    sb.AppendLine(li.Render(list_item_id));
                    i++;
                }
            }
            sb.AppendLine("</select>");
            sb.AppendLine("</td>");
            sb.AppendLine("<td valign=\"top\" class=\"display-container\">");
            sb.AppendLine("<div class=\"display-title\">" + Options.DisplayTitle + "</div>");
            sb.AppendLine("<div class=\"display\"></div>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }
    }
}
