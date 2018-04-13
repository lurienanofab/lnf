using LNF.Cache;
using LNF.CommonTools;
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
        public virtual ClientPrivilege AuthTypes
        {
            get { return 0; }
        }

        public string FileName
        {
            get { return Path.GetFileName(Request.PhysicalPath); }
        }

        protected override void OnPreLoad(EventArgs e)
        {
            CheckAuth();
            base.OnPreLoad(e);
        }

        public new LNFMasterPage Master
        {
            get { return (LNFMasterPage)base.Master; }
        }

        public new LNFPage Page
        {
            get { return (LNFPage)base.Page; }
        }

        public ClientItem CurrentUser
        {
            get
            {
                return CacheManager.Current.CurrentUser;
            }
        }

        public System.Web.UI.Control FindControlRecursive(string id)
        {
            return WebUtility.FindControlRecursive(this, id);
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

        protected bool HasPriv(ClientPrivilege privs)
        {
            return CacheManager.Current.CurrentUser.HasPriv(privs);
        }
    }
}
