using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace LNF.Web.Mvc.UI
{
    public class DropDownMenu
    {
        public IEnumerable<DropDownMenuItem> Items { get; set; }
        public string LogoUrl { get; set; }
        public object HtmlAttributes { get; set; }

        public DropDownMenu(IEnumerable<DropDownMenuItem> items, string logoUrl = null, object htmlAttributes = null)
        {
            Items = items;
            LogoUrl = logoUrl;
            HtmlAttributes = htmlAttributes;
        }

        public string Render()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(CreateTag("div", new { @class = "menu-nav" }));
            sb.AppendLine(CreateTag("div", HtmlAttributes));
            sb.AppendLine(CreateTag("div", new { @class = "menu-container" }));
            sb.AppendLine(CreateTag("table"));
            sb.AppendLine(CreateTag("tbody"));
            sb.AppendLine(CreateTag("tr"));

            if (!string.IsNullOrEmpty(LogoUrl))
            {
                sb.AppendLine(CreateTag("td", new { @class = "logo-cell" }));
                sb.AppendLine(string.Format("<a href=\"/\"><img class=\"menu-logo\" border=\"0\" src=\"{0}\" /></a>", LogoUrl));
                sb.AppendLine("</td>");
            }

            sb.AppendLine(CreateTag("td", new { @class = "menu-cell" }));
            sb.AppendLine(CreateTag("ul", new { @class = "menu-root" }));

            var parents = Items.Where(x => x.ParentID == 0).OrderBy(x => x.SortOrder).ToArray();
            foreach (var p in parents)
            {
                sb.AppendLine(CreateTag("li", new { @class = string.Format("menu-parent menu-parent-off menu-item-level-0 {0}", p.CssClass).Trim() }));

                sb.AppendLine(CreateTag("div", new { @class = "menu-parent-text" }));
                if (string.IsNullOrEmpty(p.URL))
                {
                    sb.AppendLine(CreateTag("div", new { @class = "text-container" }));
                    sb.AppendLine(p.Text);
                    sb.AppendLine("</div>");
                }
                else
                {
                    sb.AppendLine(CreateTag("a", new { @class = "text-container", @href = p.URL }));
                    sb.AppendLine(p.Text);
                    sb.AppendLine("</a>");
                }
                sb.AppendLine("</div>");

                var children = Items.Where(x => x.ParentID == p.ID).OrderBy(x => x.SortOrder).ToArray();
                if (children.Length > 0)
                {
                    sb.AppendLine(CreateTag("ul", new { @class = "menu-parent-children", @style = "visibility: hidden;" }));
                    foreach (var subitem in Items.Where(x => x.ParentID == p.ID))
                    {
                        sb.AppendLine(CreateTag("li", new { @class = "menu-item menu-item-off menu-item-level-1" }));
                        sb.AppendLine(CreateTag("div", new { @class = "menu-item-text" }));
                        sb.AppendLine(CreateTag("a", new { @class = "text-container", @href = subitem.URL }));
                        sb.AppendLine(subitem.Text);
                        sb.AppendLine("</a>");
                        sb.AppendLine("</div>");
                        sb.AppendLine("</li>");
                    }
                    sb.AppendLine("</ul>");
                }
                sb.AppendLine("</li>");
            }

            sb.AppendLine("</ul>");
            sb.AppendLine("</td>");

            sb.AppendLine("</tr>");
            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.Append("</div>");

            return sb.ToString();
        }

        string CreateTag(string name, object attributes = null)
        {
            string result = "<" + name;
            if (attributes != null)
            {
                var rvd = HtmlHelper.AnonymousObjectToHtmlAttributes(attributes);
                if (rvd != null && rvd.Count > 0)
                {
                    foreach (var kvp in rvd)
                        result += " " + kvp.Key + "=\"" + kvp.Value + "\"";
                }
            }
            result += ">";
            return result;
        }
    }
}
