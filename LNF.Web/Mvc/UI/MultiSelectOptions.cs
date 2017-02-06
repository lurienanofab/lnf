using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Web.Mvc.UI
{
    public class MultiSelectOptions
    {
        public int Size { get; set; }
        public string DisplayTitle { get; set; }

        public MultiSelectOptions()
        {
            this.Size = 10;
            this.DisplayTitle = "Selected";
        }
        public MultiSelectOptions(int size)
        {
            this.Size = size;
            this.DisplayTitle = "Selected";
        }
        public MultiSelectOptions(string display_title)
        {
            this.Size = 10;
            this.DisplayTitle = display_title;
        }
        public MultiSelectOptions(int size, string display_title)
        {
            this.Size = size;
            this.DisplayTitle = display_title;
        }
    }
}
