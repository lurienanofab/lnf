using System;
using System.Web.UI;

namespace LNF.Web.Content
{
    public class RedirectPage : Page
    {
        public string Location { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            if (!string.IsNullOrEmpty(Location))
                Response.Redirect(Location);
            else
                throw new Exception("Property Location must be set.");
        }
    }
}
