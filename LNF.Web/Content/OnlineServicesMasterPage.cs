using LNF.Authorization;
using LNF.Data;
using System;
using System.Text;
using System.Web;
using System.Web.UI;

namespace LNF.Web.Content
{
    public abstract class OnlineServicesMasterPage : MasterPage
    {
        public HttpContextBase ContextBase { get; }
        public virtual bool ShowMenu => true;
        public virtual bool AddScripts => true;
        public virtual bool AddStyles => true;
        public PageAuthorization Authorization { get; }
        public IClient CurrentUser => OnlineServicesPage.CurrentUser;
        public IProvider Provider => OnlineServicesPage.Provider;

        public OnlineServicesPage OnlineServicesPage
        {
            get
            {
                if (Page == null) return null;

                if (typeof(OnlineServicesPage).IsAssignableFrom(Page.GetType()))
                    return (OnlineServicesPage)Page;

                throw new Exception($"Cannot convert {Page.GetType().Name} to OnlineServicesPage.");
            }
        }

        public OnlineServicesMasterPage()
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
