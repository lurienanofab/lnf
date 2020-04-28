using LNF.Authorization;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Navigation
{
    public class ButtonMenuItem
    {
        public ButtonMenuItem(PageAuthorization.AppPage page)
        {
            Page = page;
        }

        public PageAuthorization.AppPage Page { get; }

        public Button CreateButton()
        {
            Button result = new Button
            {
                ID = "btnNav" + Page.ID,
                CssClass = "command-button",
                Text = Page.ButtonText,
                ToolTip = Page.ToolTip,
                OnClientClick = "window.location = '" + Page.PageUrl + "'; return false;"
            };

            return result;
        }
    }
}
