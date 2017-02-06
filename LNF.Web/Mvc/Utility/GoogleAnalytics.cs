using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace LNF.Web.Mvc.Utility
{
    public class GoogleAnalytics
    {
        public bool IsProduction()
        {
            return Providers.IsProduction();
        }

        public string GetTrackingID()
        {
            return ConfigurationManager.AppSettings["GoogleAnalyticsTrackingID"];
        }

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
