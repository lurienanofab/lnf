using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Web.Mvc.UI
{
    public class CheckBoxList
    {
        public CheckBoxList(IEnumerable<CheckBoxListItem> items = null, CheckBoxListOptions options = null)
        {
            Items = items;
            Options = options;
        }

        public IEnumerable<CheckBoxListItem> Items { get; set; }
        public CheckBoxListOptions Options { get; set; }

        public string Render(string id)
        {
            if (Options == null)
                Options = new CheckBoxListOptions();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("<table id=\"{0}\"{1}>", id, GetClassAttribute()));
            int i = 0;
            if (Items != null && Items.Count() > 0)
            {
                foreach (CheckBoxListItem li in Items)
                {
                    if (i % Options.ItemsPerRow == 0)
                    {
                        if (i > 0) sb.AppendLine("</tr>");
                        sb.AppendLine("<tr>");
                    }
                    string list_item_id = id + "_" + i.ToString();
                    li.Disabled = Options.ReadOnly;
                    sb.AppendLine(li.Render(list_item_id));
                    i++;
                }
                while (i % Options.ItemsPerRow > 0)
                {
                    sb.AppendLine("<td>&nbsp;</td>");
                    i++;
                }
            }
            else
            {
                sb.AppendLine("<tr>");
                sb.AppendLine("<td class=\"nodata\">" + Options.NoItemsText + "</td>");
            }
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        private string GetClassAttribute()
        {
            if (Options == null)
                return string.Empty;

            if (string.IsNullOrEmpty(Options.CssClass))
                return string.Empty;

            return string.Format(" class=\"{0}\"", Options.CssClass);
        }
    }
}
