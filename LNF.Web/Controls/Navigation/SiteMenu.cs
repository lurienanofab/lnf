using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Navigation
{
    public class SiteMenu : WebControl
    {
        protected override void CreateChildControls()
        {
            Controls.Add(new Literal() { Text = GetSiteMenu().ToHtmlString() });
        }

        protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

        public string Target { get; set; }

        private HtmlString GetSiteMenu()
        {
            if (Context.Session["SiteMenu"] == null)
            {
                var client = Context.CurrentUser();
                Context.Session["SiteMenu"] = WebUtility.GetSiteMenu(client.ClientID);
            }

            return new HtmlString(Context.Session["SiteMenu"].ToString());
        }
    }
}
