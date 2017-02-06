using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Web.Mvc.UI
{
    public class DropDownMenuItem
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Target { get; set; }
        public string Text { get; set; }
        public string URL { get; set; }
        public string CssClass { get; set; }
        public int SortOrder { get; set; }
    }
}
