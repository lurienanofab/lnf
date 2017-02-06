using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace LNF.Web.Mvc.UI
{
    public class OptGroupSelectListItem
    {
        public string GroupName { get; set; }
        public IEnumerable<SelectListItem> Items { get; set; }

        public OptGroupSelectListItem()
        {
            Items = new List<SelectListItem>();
        }

        public void Render(StringBuilder sb)
        {
            Render(sb, null);
        }

        public void Render(StringBuilder sb, object SelectedValue)
        {
            if (!string.IsNullOrEmpty(GroupName))
                sb.AppendLine(string.Format("<optgroup label=\"{0}\">", GroupName));
            foreach (SelectListItem sli in Items)
            {
                if (SelectedValue != null) sli.Selected = sli.Value.Equals(SelectedValue.ToString());
                sb.AppendLine(string.Format("<option value=\"{0}\"{1}>{2}</option>", sli.Value, (sli.Selected ? " selected" : ""), sli.Text));
            }
            if (!string.IsNullOrEmpty(GroupName))
                sb.AppendLine("</optgroup>");
        }
    }
}
