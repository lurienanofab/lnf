using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls
{
    public enum AlertType
    {
        Primary = 1,
        Secondary = 2,
        Success = 3,
        Danger = 4,
        Warning = 5,
        Info = 6,
        Light = 7,
        Dark = 8
    }

    public class BootstrapAlert : WebControl
    {
        public AlertType AlertType { get; set; }
        public string Text { get; set; }
        public bool Dismissable { get; set; }

        public BootstrapAlert() : base(HtmlTextWriterTag.Div)
        {
            AlertType = AlertType.Danger;
            Visible = false;
            EnableViewState = false;
            Dismissable = false;
        }

        private string GetCssClass()
        {
            string result = "alert alert-" + AlertType.ToString().ToLower();

            if (Dismissable)
                result += " alert-dismissable";

            if (!string.IsNullOrEmpty(CssClass))
                result += " " + CssClass;

            return result.Trim();
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.AddAttribute("class", GetCssClass());
            writer.AddAttribute("role", "alert");
            base.RenderBeginTag(writer);
        }

        protected override void CreateChildControls()
        {
            if (Dismissable)
            {
                var button = new HtmlButton();
                button.Attributes["class"] = "close btn-close";
                button.Attributes["data-dismiss"] = "alert";
                button.Attributes["data-bs-dismiss"] = "alert";
                button.Attributes["aria-label"] = "Close";

                var span = new HtmlGenericControl("span");
                span.Attributes["aria-hidden"] = "true";
                span.InnerHtml = "&times;";

                button.Controls.Add(span);
                Controls.Add(button);
            }

            LiteralControl content = new LiteralControl(Text);

            Controls.Add(content);
        }

        public void Show(string text, AlertType alertType = AlertType.Danger)
        {
            Visible = true;
            AlertType = alertType;
            Text = text;
        }
    }
}
