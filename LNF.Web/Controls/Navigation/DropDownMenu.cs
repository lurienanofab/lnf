using LNF.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Navigation
{
    [ParseChildren(true)]
    [ToolboxData("<{0}:Menu runat=server></{0}:Menu>")]
    public class DropDownMenu : WebControl
    {
        public LNF.SiteMenu DataSource { get; set; }

        public bool UseJavascriptNavigation { get; set; }

        public string Target { get; set; }

        public string LogoImageUrl { get; set; }

        public bool IsSecureConnection { get; set; }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [NotifyParentProperty(true)]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public List<DropDownMenuItem> Items { get; } = new List<DropDownMenuItem>();

        public override void DataBind()
        {
            //this only handles one-level menus, needs to be made recursive

            Items.Clear();

            var parents = DataSource.Where(x => x.MenuParentID == 0).OrderBy(x => x.SortOrder);

            foreach (var pmenu in parents)
            {
                DropDownMenuItem p = new DropDownMenuItem(pmenu.MenuText, pmenu.MenuURL, pmenu.IsVisible(DataSource.Client))
                {
                    CssClass = pmenu.MenuCssClass
                };

                SetTarget(p, pmenu);

                var children = DataSource.Where(x => x.MenuParentID == pmenu.MenuID).OrderBy(x => x.SortOrder);

                foreach (var cmenu in children)
                {
                    DropDownMenuItem c = new DropDownMenuItem(cmenu.MenuText, cmenu.MenuURL, cmenu.IsVisible(DataSource.Client))
                    {
                        CssClass = cmenu.MenuCssClass
                    };

                    SetTarget(c, cmenu);
                    
                    p.Items.Add(c);
                }

                Items.Add(p);
            }

            base.DataBind();
        }

        private void SetTarget(DropDownMenuItem i, IMenu m)
        {
            if (m.TopWindow)
                i.Target = "_top";
            else if (m.NewWindow)
                i.Target = "_blank";
            else
            {
                if (string.IsNullOrEmpty(Target))
                    i.Target = "_self";
                else
                    i.Target = Target;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            if (!Visible) return;

            try
            {
                output.WriteLine("<div class=\"site-menu\"><div class=\"menu-container\"><table><tr>");

                if (LogoImageUrl != string.Empty)
                    output.WriteLine("<td class=\"logo-cell\"><a href=\"/\"><img src=\"{0}\" border=\"0\" class=\"menu-logo\" /></a></td>", LogoImageUrl);

                output.WriteLine("<td class=\"menu-cell\"><ul class=\"menu-root\">");

                if (Items.Count > 0)
                {
                    foreach (DropDownMenuItem i in Items)
                    {
                        if (i.Visible)
                        {
                            i.UseJavascriptNavigation = UseJavascriptNavigation;
                            i.Render(output, IsSecureConnection);
                        }
                    }
                }

                output.WriteLine("</ul>");
                output.WriteLine("</td></tr></table></div></div><div class=\"menu-spacer\"></div>");
            }
            catch (Exception ex)
            {
                output.Write("<div class=\"lnf-controls-error\">An error occurred while trying to render the Menu control:<div style=\"font-family: 'courier new'; font-size: 8pt; color: #000000; padding: 8px;\">" + ex.Message + "</div></div>");
            }

        }
    }
}
