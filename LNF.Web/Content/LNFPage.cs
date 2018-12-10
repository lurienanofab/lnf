using LNF.Data;
using LNF.Models.Data;
using System.IO;
using System.Web.UI;

namespace LNF.Web.Content
{
    public abstract class LNFPage : Page
    {
        public virtual ClientPrivilege AuthTypes => 0;

        public string FileName => Path.GetFileName(Request.PhysicalPath);

        public new LNFMasterPage Master => (LNFMasterPage)base.Master;

        public new LNFPage Page => (LNFPage)base.Page;

        public ClientItem CurrentUser => Context.CurrentUser();

        public bool HasPriv(ClientPrivilege privs) => CurrentUser.HasPriv(privs);

        public System.Web.UI.Control FindControlRecursive(string id) => WebUtility.FindControlRecursive(this, id);
    }
}
