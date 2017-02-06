using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Web.Mvc.UI
{
    public class CheckBoxListOptions
    {
        public int ItemsPerRow { get; set; }
        public string NoItemsText { get; set; }
        public string CssClass { get; set; }
        public bool ReadOnly { get; set; }
        public bool Disabled { get; set; }
        public string ColumnCssClass { get; set; }

        public CheckBoxListOptions(int itemsPerRow = 5, string noItemsText = null, string cssClass = null, bool readOnly = false, bool disabled = false, string columnCssClass = null)
        {
            ItemsPerRow = itemsPerRow;
            NoItemsText = noItemsText;
            CssClass = cssClass;
            ReadOnly = readOnly;
            Disabled = disabled;
            ColumnCssClass = columnCssClass;
        }
    }
}
