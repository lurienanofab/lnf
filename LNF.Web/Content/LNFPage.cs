using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using System;
using System.IO;
using System.Web.Security;
using System.Web.UI;

namespace LNF.Web.Content
{
    public abstract class LNFPage : Page
    {
        public virtual ClientPrivilege AuthTypes => 0;

        public string FileName => Path.GetFileName(Request.PhysicalPath);

        public new LNFMasterPage Master => (LNFMasterPage)base.Master;

        public new LNFPage Page => (LNFPage)base.Page;

        public ClientItem CurrentUser => Request.GetCurrentUser();

        protected bool HasPriv(ClientPrivilege privs) => CurrentUser.HasPriv(privs);

        public System.Web.UI.Control FindControlRecursive(string id) => WebUtility.FindControlRecursive(this, id);

        protected override void OnPreLoad(EventArgs e)
        {
            CheckAuth();
            base.OnPreLoad(e);
        }

        protected void CheckAuth()
        {
            CacheManager.Current.CheckSession();

            if (AuthTypes == 0)
                return; //0 means open to everyone

            if (CurrentUser != null)
            {
                if (CurrentUser.HasPriv(ClientPrivilege.Developer))
                    //developers can do anything they want!
                    return;

                else if (!CurrentUser.HasPriv(AuthTypes))
                {
                    Session.Abandon();
                    Response.Redirect(FormsAuthentication.LoginUrl);
                }
            }
        }
    }
}
