using LNF.Models.Data;
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Navigation
{
    public class SiteMenu : WebControl
    {
        public HttpContextBase ContextBase { get; }
        public IClient CurrentUser => ContextBase.CurrentUser();

        public SiteMenu()
        {
            ContextBase = new HttpContextWrapper(Context);
        }

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
                Context.Session["SiteMenu"] = WebUtility.GetSiteMenu(CurrentUser.ClientID, Target);
            }

            return new HtmlString(Convert.ToString(Context.Session["SiteMenu"]));
        }
    }
}
