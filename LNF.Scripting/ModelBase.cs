using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Scripting
{
    public abstract class ModelBase
    {
        public override string ToString()
        {
            var props = this.GetType().GetProperties();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<table>");
            sb.AppendLine("<tbody>");
            foreach (var p in props)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<th>{0}</th>", p.Name));
                sb.AppendLine(string.Format("<td>{0}</td>", p.GetValue(this, null)));
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</tbody>");
            sb.Append("</table>");

            return sb.ToString();
        }
    }
}
