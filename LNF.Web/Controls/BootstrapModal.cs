using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls
{
    public enum ModalSize
    {
        Small = 1,
        Normal = 2,
        Large = 3
    }

    public class BootstrapModal : WebControl, INamingContainer
    {
        public string Text { get; set; }

        public string Title { get; set; }

        public bool Fade { get; set; }

        public ModalSize Size { get; set; }

        [TemplateContainer(typeof(BootstrapModal))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate BodyTemplate { get; set; }

        [TemplateContainer(typeof(BootstrapModal))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate FooterTemplate { get; set; }

        public BootstrapModal() : base(HtmlTextWriterTag.Div)
        {
            Fade = true;
            Size = ModalSize.Normal;
        }

        private string GetCssClass()
        {
            string result = "modal" + (Fade ? " fade" : string.Empty);
            if (!string.IsNullOrEmpty(CssClass))
                result += " " + CssClass;
            return result.Trim();
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            writer.AddAttribute("class", GetCssClass());
            writer.AddAttribute("tabindex", "-1");
            writer.AddAttribute("role", "dialog");
            base.RenderBeginTag(writer);
        }

        protected override void CreateChildControls()
        {
            string cssClass = "modal-dialog";

            if (Size == ModalSize.Small)
                cssClass += " modal-sm";
            else if (Size == ModalSize.Large)
                cssClass += " modal-lg";

            HtmlGenericControl dialog = new HtmlGenericControl("div");
            dialog.Attributes.Add("class", cssClass);

            CreateContent(dialog);

            Controls.Add(dialog);
        }

        private void CreateContent(HtmlGenericControl parent)
        {
            HtmlGenericControl content = new HtmlGenericControl("div");
            content.Attributes.Add("class", "modal-content");

            CreateHeader(content);

            CreateBody(content);

            CreateFooter(content);

            parent.Controls.Add(content);
        }

        private void CreateHeader(HtmlGenericControl parent)
        {
            HtmlGenericControl header = new HtmlGenericControl("div");
            header.Attributes.Add("class", "modal-header");

            HtmlButton closeButton = new HtmlButton();
            closeButton.Attributes.Add("class", "close");
            closeButton.Attributes.Add("data-dismiss", "modal");
            closeButton.Attributes.Add("aria-label", "Close");

            HtmlGenericControl span = new HtmlGenericControl("span");
            span.Attributes.Add("aria-hidden", "true");
            span.InnerHtml = "&times;";

            closeButton.Controls.Add(span);

            header.Controls.Add(closeButton);

            HtmlGenericControl title = new HtmlGenericControl("h4");
            title.Attributes.Add("class", "modal-title");
            title.InnerHtml = Title;

            header.Controls.Add(title);

            parent.Controls.Add(header);
        }

        public void CreateBody(HtmlGenericControl parent)
        {
            HtmlGenericControl body = new HtmlGenericControl("div");
            body.Attributes.Add("class", "modal-body");
            if (BodyTemplate != null)
                BodyTemplate.InstantiateIn(body);
            parent.Controls.Add(body);
        }

        public void CreateFooter(HtmlGenericControl parent)
        {
            if (FooterTemplate != null)
            {
                HtmlGenericControl footer = new HtmlGenericControl("div");
                footer.Attributes.Add("class", "modal-footer");
                FooterTemplate.InstantiateIn(footer);
                parent.Controls.Add(footer);
            }
        }
    }
}
