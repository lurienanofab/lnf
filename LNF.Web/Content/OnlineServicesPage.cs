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
    public abstract class OnlineServicesPage : Page
    {
        public OnlineServicesPage()
        {
            ContextBase = new HttpContextWrapper(Context);
        }

        [Inject] public IProvider Provider { get; set; }

        public HttpContextBase ContextBase { get; }

        public virtual ClientPrivilege AuthTypes => 0;

        public string FileName => Path.GetFileName(Request.PhysicalPath);

        public OnlineServicesMasterPage LNFMaster
        {
            get
            {
                if (Master == null) return null;

                if (typeof(OnlineServicesMasterPage).IsAssignableFrom(Master.GetType()))
                    return (OnlineServicesMasterPage)Master;

                throw new Exception($"Cannot convert {Master.GetType().Name} to OnlineServicesMasterPage.");
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
