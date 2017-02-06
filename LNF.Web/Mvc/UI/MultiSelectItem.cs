using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Web.Mvc.UI
{
    public class MultiSelectItem
    {
        public string Value { get; set; }
        public string Text { get; set; }

        public string Render(string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option id=\"" + id + "\" value=\"" + this.Value + "\">" + this.Text + "</option>");
            return sb.ToString();
        }
    }
}
