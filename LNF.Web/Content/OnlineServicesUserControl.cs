using LNF.Data;
using LNF.DataAccess;
using LNF.Repository;
using System;
using System.Data;
using System.Web;
using System.Web.UI;

namespace LNF.Web.Content
{
    public class OnlineServicesUserControl : UserControl
    {
        public IProvider Provider => OnlineServicesPage.Provider;

        public HttpContextBase ContextBase => OnlineServicesPage.ContextBase;

        public IClient CurrentUser => OnlineServicesPage.CurrentUser;

        public ISession DataSession => OnlineServicesPage.DataSession;

        public IDataCommand DataCommand(CommandType type = CommandType.StoredProcedure) => OnlineServicesPage.DataCommand(type);

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
    }
}
