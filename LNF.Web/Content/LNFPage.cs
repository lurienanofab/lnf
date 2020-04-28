using LNF.Data;
using System.IO;
using System.Web;
using System.Web.UI;

namespace LNF.Web.Content
{
    public abstract class LNFPage : Page
    {
        public LNFPage()
        {
            ContextBase = new HttpContextWrapper(Context);
        }

        [Inject] public IProvider Provider { get; set; }

        public HttpContextBase ContextBase { get; }

        public virtual ClientPrivilege AuthTypes => 0;

        public string FileName => Path.GetFileName(Request.PhysicalPath);

        public new LNFMasterPage Master => (LNFMasterPage)base.Master;

        public new LNFPage Page => (LNFPage)base.Page;

        public IClient CurrentUser
        {
            get
            {
                IClient result;

                if (Context.Items["CurrentUser"] == null)
                {
                    result = Provider.Data.Client.GetClient(Context.User.Identity.Name);
                    Context.Items["CurrentUser"] = result;
                }
                else
                {
                    result = (IClient)Context.Items["CurrentUser"];
                }

                return result;
            }
        }

        public bool HasPriv(ClientPrivilege privs) => CurrentUser.HasPriv(privs);

        public System.Web.UI.Control FindControlRecursive(string id) => WebUtility.FindControlRecursive(this, id);
    }
}
