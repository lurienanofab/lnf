using LNF.CommonTools;
using LNF.Models.Data;
using System.Text;
using System.Web.UI;

namespace LNF.Web.Content
{
    public abstract class LNFMasterPage : MasterPage
    {
        //private DropDownMenu _Menu = new DropDownMenu();
        private Authorization _Authorization = new Authorization();

        public virtual bool ShowMenu
        {
            get { return true; }
        }

        public virtual bool AddScripts
        {
            get { return true; }
        }

        public virtual bool AddStyles
        {
            get { return true; }
        }

        public new LNFPage Page
        {
            get { return (LNFPage)base.Page; }
        }

        //public DropDownMenu Menu
        //{
        //    get { return _Menu; }
        //}

        public Authorization Authorization
        {
            get { return _Authorization; }
        }

        public ClientModel CurrentUser
        {
            get { return Page.CurrentUser; }
        }

        //private void CreateMenu()
        //{
        //    _Menu.ID = "DropDownMenu1";
        //    _Menu.CssClass = "menu-nav";

        //    _Menu.LogoImageUrl = GetStaticUrl("images/lnfbanner.jpg");

        //    _Menu.DataSource = SiteMenu.Create(CurrentUser);
        //    _Menu.DataBind();

        //    DropDownMenuItem parent;

        //    StringBuilder sb = new StringBuilder();
        //    string DisplayName = (string.IsNullOrEmpty(WebContext.Current.DisplayName)) ? "[unknown]" : WebContext.Current.DisplayName;
        //    sb.AppendLine("<div>Current User: " + DisplayName + "</div>");
        //    sb.AppendLine("<div id=\"jclock\"></div>");
        //    parent = new DropDownMenuItem(sb.ToString());
        //    parent.CssClass = "menu-clock";
        //    parent.Enabled = false;
        //    _Menu.Items.Add(parent);
        //}

        protected virtual void InitAuthorization()
        {
            if (_Authorization == null)
            {
                _Authorization = new Authorization();
            }
        }

        public void AddWindowOpenScript()
        {
            StringBuilder sb = new StringBuilder();
            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "winopen", sb.ToString());
        }

        public System.Web.UI.Control FindControlRecursive(string id)
        {
            return WebUtility.FindControlRecursive(this, id);
        }

        public string GetStaticUrl(string path)
        {
            return Utility.GetStaticUrl(path);
        }
    }
}
