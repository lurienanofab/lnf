using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace LNF.Data.ClientAccountMatrix
{
    public class MatrixUserHeader
    {
        public int ClientOrgID { get; set; }
        public string DisplayName { get; set; }
        public string Key { get; set; }

        public void Render(StringBuilder sb)
        {
            sb.AppendFormat("<th class=\"user-column {0}\">{1}</th>", Key, HeaderText());
        }

        public string HeaderText()
        {
            if (string.IsNullOrEmpty(DisplayName))
                return string.Empty;
            return DisplayName.Replace(" ", "<br />");
        }
    }
}