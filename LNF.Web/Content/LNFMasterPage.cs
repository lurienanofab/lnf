using LNF.Authorization;
using LNF.Data;
using System.Text;
using System.Web;
using System.Web.UI;

namespace LNF.Web.Content
{
    public abstract class LNFMasterPage : MasterPage
    {
        public HttpContextBase ContextBase { get; }
        public virtual bool ShowMenu => true;
        public virtual bool AddScripts => true;
        public virtual bool AddStyles => true;
        public new LNFPage Page => (LNFPage)base.Page;
        public PageAuthorization Authorization { get; }
        public IClient CurrentUser => Page.CurrentUser;

        public LNFMasterPage()
        {
            ContextBase = new HttpContextWrapper(Context);
            Authorization = new PageAuthorization();
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
