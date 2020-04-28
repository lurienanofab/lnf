using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace LNF.Web.Mvc.Utility
{
    public class GoogleAnalytics
    {
        public bool IsProduction() => ServiceProvider.Current.IsProduction();

        public string GetTrackingID() => ConfigurationManager.AppSettings["GoogleAnalyticsTrackingID"];

        public IHtmlString Render()
        {
            if (IsProduction())
            {
                string trackingId = GetTrackingID();
                if (!string.IsNullOrEmpty(trackingId))
                {
                    TagBuilder script = new TagBuilder("script");
                    script.InnerHtml += "(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)})(window,document,'script','//www.google-analytics.com/analytics.js','ga');ga('create', '" + trackingId + "', 'auto');ga('send', 'pageview');";
                    return new HtmlString(script.ToString());
                }
                else
                    return new HtmlString("<!-- Google Analytics disabled: GoogleAnalyticsTrackingID not found in AppSettings. -->");
            }
            else
                return new HtmlString("<!-- Google Analytics disabled in development. -->");
        }
    }
}
