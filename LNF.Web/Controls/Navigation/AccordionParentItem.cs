using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Web.UI;

namespace LNF.Web.Controls.Navigation
{
    [ParseChildren(true)]
    public class AccordionParentItem
    {
        private List<AccordionChildItem> _ChildItems = new List<AccordionChildItem>();
        private string _NavigateURL = "#";
        private bool _Visible = true;

        public Accordion Parent { get; set; }

        public string Text { get; set; }

        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        public string NavigateURL
        {
            get
            {
                _NavigateURL = (_ChildItems.Count > 0) ? "#" : _NavigateURL;
                return _NavigateURL;
            }
            set
            {
                _NavigateURL = (value == string.Empty) ? "#" : value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public List<AccordionChildItem> ChildItems
        {
            get { return _ChildItems; }
        }

        public void Render(HtmlTextWriter output)
        {
            if (!_Visible) return;

            output.WriteLine("<h3" + ((_ChildItems.Count > 0) ? string.Empty : " class=\"empty\"") + "><a href=\"" + this.NavigateURL + "\">" + this.Text + "</a></h3>");
            output.WriteLine("<div>");

            List<AccordionChildItem> vc = this.VisibleChildren();
            if (this.Parent.ChildrenLayoutMode == AccordionChildrenLayout.Table)
            {
                int cols = Math.Max(this.Parent.ChildrenTableColumnCount, 1);
                int i = 0;
                output.WriteLine("<table" + (this.Parent.ChildrenTableCssClass == string.Empty ? string.Empty : " class=\"" + this.Parent.ChildrenTableCssClass + "\"") + ">");
                foreach (AccordionChildItem c in vc)
                {
                    if (i % cols == 0) { output.WriteLine("<tr>"); }
                    output.WriteLine("<td>");
                    c.Render(output);
                    output.WriteLine("</td>");
                    if (i % cols == (cols - 1)) { output.WriteLine("</tr>"); }
                    i++;
                }

                //finish the row
                if ((i - 1) % cols < (cols - 1))
                {
                    i--;
                    while (i % cols < (cols - 1))
                    {
                        output.WriteLine("<td class=\"empty\">&nbsp;</td>");
                        i++;
                    }
                    output.WriteLine("</tr>");
                }

                output.WriteLine("</table>");
            }
            else
            {
                int i = 0;
                foreach (AccordionChildItem c in vc)
                {
                    if (this.Parent.ChildrenLayoutMode == AccordionChildrenLayout.Div)
                    {
                        output.WriteLine("<div" + (i == 0 ? " class=\"first\"" : (i == vc.Count - 1 ? " class=\"last\"" : string.Empty)) + ">");
                    }
                    c.Render(output);
                    if (this.Parent.ChildrenLayoutMode == AccordionChildrenLayout.Div)
                    {
                        output.WriteLine("</div>");
                    }

                    i++;
                }
            }
            output.WriteLine("</div>");
        }

        public AccordionChildItem FindChild(string text)
        {
            AccordionChildItem result = null;

            foreach (AccordionChildItem c in _ChildItems)
            {
                if (c.Text == text)
                {
                    result = c;
                    break;
                }
            }

            return result;
        }

        public List<AccordionChildItem> VisibleChildren()
        {
            List<AccordionChildItem> result = new List<AccordionChildItem>();

            foreach (AccordionChildItem c in _ChildItems)
            {
                if (c.Visible)
                {
                    result.Add(c);
                }
            }

            return result;
        }
    }
}
