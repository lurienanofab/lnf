using LNF.Data;
using LNF.DataAccess;
using LNF.Repository;
using System;
using System.Data;
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

        public LNFMasterPage LNFMaster
        {
            get
            {
                if (Master == null) return null;

                if (typeof(LNFMasterPage).IsAssignableFrom(Master.GetType()))
                    return (LNFMasterPage)Master;

                throw new Exception($"Cannot convert {Master.GetType().Name} to LNFMasterPage.");
            }
        }

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

        public virtual ContextHelper Helper
        {
            get
            {
                return new ContextHelper(ContextBase, Provider);
            }
        }

        public bool HasPriv(ClientPrivilege privs) => CurrentUser.HasPriv(privs);

        public IDataCommand DataCommand(CommandType type = CommandType.StoredProcedure) => Repository.DataCommand.Create(type);

        public ISession DataSession => Provider.DataAccess.Session;

        public System.Web.UI.Control FindControlRecursive(string id) => WebUtility.FindControlRecursive(this, id);
    }
}
