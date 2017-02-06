using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace LNF.Web.Controls.Navigation
{
    public class AccordionChildItem
    {
        private string _NavigateURL = "#";
        private bool _Visible = true;

        public string Text { get; set; }

        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        public string NavigateURL
        {
            get { return _NavigateURL; }
            set { _NavigateURL = (value == string.Empty) ? "#" : value; }
        }

        public void Render(HtmlTextWriter output)
        {
            if (!_Visible) return;

            output.WriteLine("<a href=\"" + this.NavigateURL + "\">" + this.Text + "</a>");
        }
    }
}
