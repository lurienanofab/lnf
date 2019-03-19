using LNF.CommonTools;
using LNF.Models.Data;
using System.Text;
using System.Web;
using System.Web.UI;

namespace LNF.Web.Content
{
    public abstract class LNFMasterPage : MasterPage
    {
        //private DropDownMenu _Menu = new DropDownMenu();
        private Authorization _Authorization = new Authorization();

        public LNFMasterPage()
        {
            ContextBase = new HttpContextWrapper(Context);
        }

        public HttpContextBase ContextBase { get; }

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

        public Authorization Authorization
        {
            get { return _Authorization; }
        }

        public ClientItem CurrentUser
        {
            get { return Page.CurrentUser; }
        }

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
    }
}
