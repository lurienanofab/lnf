using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Tools
{
    [ToolboxData("<{0}:GoogleAnalytics runat=server></{0}:ScrollPositionTool>")]
    public class GoogleAnalytics : WebControl
    {
        public string TrackingID { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            RenderContents(writer);
        }

        protected bool IsProduction => ServiceProvider.Current.IsProduction();

        protected override void CreateChildControls()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<script type=""text/javascript"">");
            sb.AppendLine("<!--");
            if (IsProduction)
            {
                if (!string.IsNullOrEmpty(this.TrackingID))
                {
                    sb.AppendLine("var _gaq = _gaq || [];");
                    sb.AppendLine("_gaq.push(['_setAccount', '" + this.TrackingID + "']);");
                    sb.AppendLine("_gaq.push(['_trackPageview']);");
                    sb.AppendLine("(function () {");
                    sb.AppendLine("var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;");
                    sb.AppendLine("ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';");
                    sb.AppendLine("var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);");
                    sb.AppendLine("})();");
                }
                else
                {
                    sb.AppendLine("// Google Analytics TrackingID has not been set.");
                }
            }
            else
            {
                sb.AppendLine("// Google Analytics is turned off while in development.");
            }
            sb.AppendLine("// -->");
            sb.AppendLine("</script>");
            LiteralControl lc = new LiteralControl(sb.ToString());
            this.Controls.Add(lc);
            base.CreateChildControls();
        }
    }
}
