using RestSharp;
using RestSharp.Authenticators;
using System.Configuration;
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
            // Why get the html from an ajax service?
            //  Because then there is one place where the menu is generated and can be used in different projects
            //  without having to copy and paste a partial view cshtml file resulting in a bunch of copies that
            //  can't be easily changed globally, or deal with hard to maintain html generation code. This method
            //  allows for maintaining a single cshtml partial view which is the best scenario.

            if (Context.Session["SiteMenu"] == null)
            {
                var client = Context.Request.GetCurrentUser();
                var basicAuth = new HttpBasicAuthenticator(ConfigurationManager.AppSettings["BasicAuthUsername"], ConfigurationManager.AppSettings["BasicAuthPassword"]);
                var rc = new RestClient(ConfigurationManager.AppSettings["ApiHost"]) { Authenticator = basicAuth };

                var request = new RestRequest("data/ajax/menu");
                request.AddParameter("clientId", client.ClientID);
                request.AddParameter("target", Target);

                var response = rc.Execute(request);

                if (response.IsSuccessful)
                    Context.Session["SiteMenu"] = new HtmlString(response.Content);
                else
                    Context.Session["SiteMenu"] = new HtmlString($"SiteMenu Error: [{(int)response.StatusCode}] {response.StatusDescription}");
            }

            return new HtmlString(Context.Session["SiteMenu"].ToString());
        }
    }
}
