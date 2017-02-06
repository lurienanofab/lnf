using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Web.UI.WebControls;

namespace LNF.Web.Controls.Navigation
{
    public class ButtonMenuItem
    {
        private Authorization.AppPage _Page;

        public ButtonMenuItem(Authorization.AppPage page)
        {
            _Page = page;
        }

        public Authorization.AppPage Page
        {
            get { return _Page; }
        }

        public Button CreateButton()
        {
            System.Web.UI.WebControls.Button result = new System.Web.UI.WebControls.Button();
            result.ID = "btnNav" + _Page.ID;
            result.CssClass = "command-button";
            result.Text = _Page.ButtonText;
            result.ToolTip = _Page.ToolTip;
            result.OnClientClick = "window.location = '" + _Page.PageUrl + "'; return false;";
            return result;
        }
    }
}
