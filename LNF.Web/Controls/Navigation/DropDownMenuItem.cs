using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Configuration;

namespace LNF.Web.Controls.Navigation
{
    [ParseChildren(true)]
    public class DropDownMenuItem
    {
        public DropDownMenuItem()
        {
            Text = string.Empty;
            NavigateURL = string.Empty;
        }

        public DropDownMenuItem(string text)
        {
            Text = text;
            NavigateURL = string.Empty;
        }

        public DropDownMenuItem(string text, bool visible)
        {
            Text = text;
            NavigateURL = string.Empty;
            Visible = visible;
        }

        public DropDownMenuItem(string text, string navURL)
        {
            Text = text;
            NavigateURL = navURL;
        }

        public DropDownMenuItem(string text, string navURL, bool visible)
        {
            Text = text;
            NavigateURL = navURL;
            Visible = visible;
        }

        private List<DropDownMenuItem> _Items = new List<DropDownMenuItem>();
        private DropDownMenuChildrenContainer _ChildrenContainer = new DropDownMenuChildrenContainer();
        private bool _Visible = true;
        private bool _Enabled = true;

        protected DropDownMenuItem _Parent;
        protected int _Level;

        public string Text { get; set; }

        public Unit Width { get; set; }

        public string CssClass { get; set; }

        public string NavigateURL { get; set; }

        //public bool NewWindow { get; set; }
        public string Target { get; set; }

        public bool UseJavascriptNavigation { get; set; }

        public DropDownMenuItem Parent
        {
            get { return _Parent; }
        }

        public int Level
        {
            get { return _Level; }
        }

        public bool Visible
        {
            get { return _Visible; }
            set { _Visible = value; }
        }

        public bool Enabled
        {
            get { return _Enabled; }
            set { _Enabled = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<DropDownMenuItem> Items
        {
            get { return _Items; }
        }

        [PersistenceMode(PersistenceMode.InnerProperty)]
        public DropDownMenuChildrenContainer ChildrenContainer
        {
            get { return _ChildrenContainer; }
        }

        public void Render(HtmlTextWriter writer)
        {
            if (!_Visible) return;

            string children_class = string.Empty;

            if (this.Parent == null)
            {
                //writer.WriteLine("<div class=\"menu-parent" + (_Enabled ? string.Empty : "-disabled" ) + " menu-parent-off menu-item-level-" + this.Level.ToString() + this.GetClass() + "\"" + this.GetWidthStyle(this.Width) + ">");
                //writer.WriteLine("<div class=\"menu-parent-group\">");
                //writer.WriteLine("<div class=\"menu-parent-text\">" + this.GetNavLink() + "</div>");
                //children_class = "menu-parent-children ";

                writer.WriteLine("<li class=\"menu-parent" + (_Enabled ? string.Empty : "-disabled") + " menu-parent-off menu-item-level-" + this.Level.ToString() + this.GetClass() + "\"" + this.GetWidthStyle(this.Width) + ">");
                //writer.WriteLine("<div class=\"menu-parent-group\">");
                writer.WriteLine("<div class=\"menu-parent-text\">" + this.GetNavLink() + "</div>");
                children_class = "menu-parent-children ";
            }
            else
            {
                //writer.WriteLine("<div class=\"menu-item" + (_Enabled ? string.Empty : "-disabled") + " menu-item-off menu-item-level-" + this.Level.ToString() + this.GetClass() + "\"" + this.GetWidthStyle(this.Width) + ">");
                //writer.WriteLine("<div class=\"menu-item-group\">");
                //writer.WriteLine("<div class=\"menu-item-text\">" + this.GetNavLink() + "</div>");
                //children_class = "menu-item-children ";

                writer.WriteLine("<li class=\"menu-item" + (_Enabled ? string.Empty : "-disabled") + " menu-item-off menu-item-level-" + this.Level.ToString() + this.GetClass() + "\"" + this.GetWidthStyle(this.Width) + ">");
                // writer.WriteLine("<div class=\"menu-item-group\">");
                writer.WriteLine("<div class=\"menu-item-text\">" + this.GetNavLink() + "</div>");
                children_class = "menu-item-children ";
            }

            if (_Items.Count > 0 && ChildrenContainer.Visible)
            {
                writer.WriteLine("<ul class=\"" + children_class + "\">");
                foreach (DropDownMenuItem c in _Items)
                {
                    c._Parent = this;
                    c._Level = this.Level + 1;
                    c.UseJavascriptNavigation = this.UseJavascriptNavigation;
                    c.Render(writer);
                }
                writer.WriteLine("</ul>");
            }

            //writer.WriteLine("</div></div>");
            writer.WriteLine("</li>");
        }

        private string GetWidthStyle(Unit width)
        {
            if (width.IsEmpty)
            {
                return string.Empty;
            }
            else
            {
                return " style=\"width: " + width.ToString() + ";\"";
            }
        }

        private string GetClass()
        {
            if (string.IsNullOrEmpty(CssClass))
            {
                return string.Empty;
            }
            else
            {
                return " " + CssClass;
            }
        }

        private string GetNavLink()
        {
            if (string.IsNullOrEmpty(NavigateURL))
                return string.Format("<div class=\"text-container\">{0}</div>", Text);

            string appServer = string.Empty;
            string schedServer = string.Empty;

            if (Providers.Context.Current.GetRequestIsSecureConnection())
            { 
                appServer = "https://" + ConfigurationManager.AppSettings["AppServer"];
                schedServer = "https://" + ConfigurationManager.AppSettings["SchedServer"];
            }
            else
            { 
                appServer = "http://" + ConfigurationManager.AppSettings["AppServer"];
                schedServer = "http://" + ConfigurationManager.AppSettings["SchedServer"];
            }

            string url = NavigateURL.Replace("{AppServer}", appServer).Replace("{SchedServer}", schedServer);

            if (UseJavascriptNavigation)
                return string.Format("<a href=\"javascript:menuNav('{0}', '{1}')\" class=\"text-container\">{2}</a>", url, Target, Text);
            else
                return string.Format("<a href=\"{0}\"{1} class=\"text-container\">{2}</a>", url, string.IsNullOrEmpty(Target) ? string.Empty : " target=\"" + Target + "\"", Text);
        }
    }
}
